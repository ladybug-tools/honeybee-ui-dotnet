using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class Dialog_HVACManager : Dialog<List<HB.Energy.IHvac>>
    {
        
        private GridView _gridView { get; set; }

        public Dialog_HVACManager(List<HB.Energy.IHvac> hvacs)
        {
            try
            {
                var hvacsInModel = hvacs;

                Padding = new Padding(5);
                Title = "HVAC Manager - Honeybee";
                WindowStyle = WindowStyle.Default;
                Width = 650;
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                var addNew = new Button { Text = "Add" };
                var duplicate = new Button { Text = "Duplicate" };
                var edit = new Button { Text = "Edit" };
                var remove = new Button { Text = "Remove" };

                layout.AddSeparateRow("HVACs:", null, addNew, duplicate, edit, remove);

                this._gridView = GenGridView(hvacsInModel);
                this._gridView.Height = 250;
                layout.AddRow(this._gridView);

                var gd = this._gridView;

                var AddHvacsDic = new Dictionary<string, ICommand>()
                {
                    { "Ideal Air Load", AddIdealAirLoadCommand},
                    { "From OpenStudio library", AddOpsHVACCommand}
                };

                addNew.Click += (s, e) =>
                {
                    var contextMenu = new ContextMenu();

                    foreach (var item in AddHvacsDic)
                    {
                        contextMenu.Items.Add(
                          new Eto.Forms.ButtonMenuItem()
                          {
                              Text = item.Key,
                              Command = item.Value
                          });
                    }
                    contextMenu.Show();
                };
                duplicate.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.Energy.IHvac;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to duplicate!");
                        return;
                    }

                   
                    var dup = selected.Duplicate() as HB.Energy.IHvac;
                    var id = Guid.NewGuid().ToString();
                    dup.Identifier = id;
                    dup.DisplayName = string.IsNullOrEmpty(selected.DisplayName) ? $"{selected.GetType()} {id.Substring(0, 5)}" : $"{selected.DisplayName}_dup";
                    if (dup is IdealAirSystemAbridged obj)
                        AddIdealAirLoadCommand.Execute(obj);
                    else
                        AddOpsHVACCommand.Execute(dup);

                };

                Action<int> editAction = (int index) =>{
                    var selected = gd.SelectedItem as HB.Energy.IHvac;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }
                    var dup = selected.Duplicate() as HB.Energy.IHvac;
                    HB.Energy.IHvac dialog_rc = null;
                    if (dup is IdealAirSystemAbridged obj)
                    {
                        // dialog for ideal air load
                    }
                    else
                    {
                        var dialog = new Dialog_OpsHVACs(dup);
                        dialog_rc = dialog.ShowModal(this);
                    }

                   
                    if (dialog_rc == null) return;
                    var newDataStore = gd.DataStore.OfType<HB.Energy.IHvac>().ToList();
                    newDataStore.RemoveAt(index);
                    newDataStore.Insert(index, dialog_rc);
                    gd.DataStore = newDataStore;

                };
                edit.Click += (s, e) =>
                {
                    editAction(gd.SelectedRow);
                };
                remove.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.Energy.IHvac;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var index = gd.SelectedRow;
                    var res = MessageBox.Show(this, $"Are you sure you want to delete:\n {selected.DisplayName ?? selected.Identifier }", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                    {
                        var newDataStore = gd.DataStore.ToList();
                        newDataStore.RemoveAt(index);
                        gd.DataStore = newDataStore;
                    }

                };

                gd.CellDoubleClick += (s, e) =>
                {
                    editAction(e.Row);
                };

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => {
                    var d = gd.DataStore.OfType<HB.Energy.IHvac>().ToList();
                    Close(d);
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                layout.AddSeparateRow(null, DefaultButton, AbortButton, null);

                //Create layout
                Content = layout;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
                //Rhino.RhinoApp.WriteLine(e.Message);
            }
            
            
        }

   

        private GridView GenGridView(IEnumerable<object> items)
        {
            var gd = new GridView() { DataStore = items };
            gd.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IHvac, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IHvac, string>(r => GetSystemType(r))
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type" });
            return gd;

            string GetSystemType(HB.Energy.IHvac hvac)
            {
                var type = hvac.GetType().GetProperty("EquipmentType")?.GetValue(hvac)?.ToString() ?? hvac.GetType().Name;
                return OpsHVACsViewModel.HVACUserFriendlyNamesDic.TryGetValue(type, out var userFriendly) ? userFriendly : type;

            }
        }

        #region AddModifier
        public ICommand AddIdealAirLoadCommand => new RelayCommand<HB.ModifierBase>((obj) => {

            //var id = Guid.NewGuid().ToString();
            //var newModifier = obj as Plastic ?? new Plastic(id, $"Plastic {id.Substring(0, 5)}", new HB.Void() );

            //var dialog = new Honeybee.UI.Dialog_Modifier<Plastic>(newModifier);
            //var dialog_rc = dialog.ShowModal(this);
            //AddModifier(dialog_rc);
        });
        public ICommand AddOpsHVACCommand => new RelayCommand(() => {
            var dialog = new Honeybee.UI.Dialog_OpsHVACs();
            var dialog_rc = dialog.ShowModal(this);
            AddHVACToGridView(dialog_rc);
        });
 
        private void AddHVACToGridView(HB.Energy.IHvac newModifier)
        {
            if (newModifier == null) return;
            var d = this._gridView.DataStore.OfType<HB.Energy.IHvac>().ToList();
            d.Add(newModifier);
            this._gridView.DataStore = d;
        }

        #endregion

    }
}
