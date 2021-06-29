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
        
        private GridView _gd { get; set; }
        private bool _returnSelectedOnly;
        private ModelEnergyProperties _modelEnergyProperties { get; set; }

        private Dialog_HVACManager()
        {
            Padding = new Padding(5);
            Title = $"HVAC Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 650;
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        [Obsolete("This is deprecated", true)]
        public Dialog_HVACManager(List<HB.Energy.IHvac> hvacs): this()
        {
            var hvacsInModel = hvacs;
            Content = Init(hvacsInModel);

        }
        public Dialog_HVACManager(ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            var hvacsInModel = libSource.HVACList;
            this._returnSelectedOnly = returnSelectedOnly;
            this._modelEnergyProperties = libSource;

            Content = Init(hvacsInModel);
        }

        private DynamicLayout Init(IEnumerable<HB.Energy.IHvac> hvacs)
        {

            var layout = new DynamicLayout();
            layout.DefaultPadding = new Padding(10);
            layout.DefaultSpacing = new Size(5, 5);

            var addNew = new Button { Text = "Add" };
            addNew.Command = AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = RemoveCommand;

            layout.AddSeparateRow("HVACs:", null, addNew, duplicate, edit, remove);

            this._gd = GenGridView(hvacs);
            this._gd.Height = 250;
            layout.AddRow(this._gd);

            var gd = this._gd;

   
            gd.CellDoubleClick += (s, e) => EditCommand.Execute(null);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
            return layout;
        }

        private GridView GenGridView(IEnumerable<object> items)
        {
            items = items ?? new List<HB.Energy.IHvac>();
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

   
        public ICommand AddIdealAirLoadCommand => new RelayCommand<HoneybeeSchema.IdealAirSystemAbridged>((obj) => {

            var dialog = new Honeybee.UI.Dialog_IdealAirLoad(obj);
            var dialog_rc = dialog.ShowModal(this);
            AddHVACToGridView(dialog_rc);
        });
        public ICommand AddOpsHVACCommand => new RelayCommand<HoneybeeSchema.Energy.IHvac>((obj) => {
            var dialog = new Honeybee.UI.Dialog_OpsHVACs(obj);
            var dialog_rc = dialog.ShowModal(this);
            AddHVACToGridView(dialog_rc);
        });
 
        private void AddHVACToGridView(HB.Energy.IHvac newModifier)
        {
            if (newModifier == null) return;
            var d = this._gd.DataStore.OfType<HB.Energy.IHvac>().ToList();
            d.Add(newModifier);
            this._gd.DataStore = d;
        }


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var contextMenu = new ContextMenu();

            var AddHvacsDic = new Dictionary<string, ICommand>()
                {
                    { "Ideal Air Load", AddIdealAirLoadCommand},
                    { "From OpenStudio library", AddOpsHVACCommand}
                };

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
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
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

        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
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
                var dialog = new Dialog_IdealAirLoad(obj);
                dialog_rc = dialog.ShowModal(this);
            }
            else
            {
                var dialog = new Dialog_OpsHVACs(dup);
                dialog_rc = dialog.ShowModal(this);
            }


            if (dialog_rc == null) return;
            var index = gd.SelectedRow;
            var newDataStore = gd.DataStore.OfType<HB.Energy.IHvac>().ToList();
            newDataStore.RemoveAt(index);
            newDataStore.Insert(index, dialog_rc);
            gd.DataStore = newDataStore;

        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
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
                var newDataStore = gd.DataStore.OfType<HB.Energy.IHvac>().ToList();
                var found = newDataStore.FindIndex(_ => _.Identifier == selected.Identifier);
                newDataStore.RemoveAt(found);
                gd.DataStore = newDataStore;
            }
        });

        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var allItems = gd.DataStore.Select(_ => _ as HB.Energy.IHvac).ToList();
            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as HB.Energy.IHvac;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<HB.Energy.IHvac>() { d };
            }

            this._modelEnergyProperties.Hvacs.Clear();
            this._modelEnergyProperties.AddHVACs(allItems);
            Close(itemsToReturn);
        });



    }
}
