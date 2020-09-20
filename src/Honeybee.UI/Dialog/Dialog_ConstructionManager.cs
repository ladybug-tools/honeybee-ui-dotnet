using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
//using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class Dialog_ConstructionManager : Dialog<List<HB.Energy.IConstruction>>
    {
        
        private GridView _gridView { get; set; }

        public Dialog_ConstructionManager(List<HB.Energy.IConstruction> constructions)
        {
            try
            {
                //var md = model;
                var constrcutionsInModel = constructions;


                Padding = new Padding(5);
                Resizable = true;
                Title = "Construction Manager - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 300);
                this.Icon = DialogHelper.HoneybeeIcon;

                //var constrs = constrcutionsInModel;


                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                var addNew = new Button { Text = "Add" };
                var duplicate = new Button { Text = "Duplicate" };
                var edit = new Button { Text = "Edit" };
                var remove = new Button { Text = "Remove" };

                layout.AddSeparateRow("Constructions:", null, addNew, duplicate, edit, remove);

                this._gridView = GenGridView(constrcutionsInModel);
                this._gridView.Height = 250;
                layout.AddRow(this._gridView);

                var gd = this._gridView;





                addNew.Click += (s, e) =>
                {
                    var contextMenu = new ContextMenu();

                    // Opaque 
                    contextMenu.Items.Add(
                        new Eto.Forms.ButtonMenuItem()
                        {
                            Text = "Opaque Construction",
                            Command = AddOpaqueConstructionCommand
                        });

                    // Window 
                    contextMenu.Items.Add(
                      new Eto.Forms.ButtonMenuItem()
                      {
                          Text = "Window Construction",
                          Command = AddOpaqueConstructionCommand
                      });
                    // Shade 
                    contextMenu.Items.Add(
                      new Eto.Forms.ButtonMenuItem()
                      {
                          Text = "Shade Construction",
                          Command = AddShadeConstructionCommand,
                          Enabled = false
                      });
                    // AirBoundary 
                    contextMenu.Items.Add(
                      new Eto.Forms.ButtonMenuItem()
                      {
                          Text = "AirBoundary Construction",
                          Command = AddAirBoundaryConstructionCommand,
                          Enabled = false
                      });


                };
                duplicate.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.Energy.IConstruction;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to duplicate!");
                        return;
                    }

                    var id = Guid.NewGuid().ToString();
                    var dup = selected.Duplicate() as HB.Energy.IConstruction;

                    dup.Identifier = id;
                    dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
                    var dialog = new Honeybee.UI.Dialog_Construction(dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
                        d.Add(dialog_rc);
                        gd.DataStore = d;

                    }
                };

                Action editAction = () =>{
                    var selected = gd.SelectedItem as HB.Energy.IConstruction;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var dup = selected.Duplicate() as HB.Energy.IConstruction;

                    var dialog = new Honeybee.UI.Dialog_Construction(dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var index = gd.SelectedRow;
                        var newDataStore = gd.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
                        newDataStore.RemoveAt(index);
                        newDataStore.Insert(index, dialog_rc);
                        gd.DataStore = newDataStore;

                    }
                };
                edit.Click += (s, e) =>
                {
                    editAction();
                };
                remove.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.Energy.IConstruction;
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
                    editAction();

                };

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => {
                    var d = gd.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
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
                Binding = Binding.Delegate<HB.Energy.IConstruction, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IConstruction, string>(r => r.GetType().Name.Replace("Abridged", ""))
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type" });
            return gd;
        }

        public ICommand AddOpaqueConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new OpaqueConstructionAbridged(id, new List<string>(), $"New Opaque Construction {id.Substring(0, 5)}");

            var dialog = new Honeybee.UI.Dialog_Construction(newConstrucion);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = this._gridView.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
                d.Add(dialog_rc);
                this._gridView.DataStore = d;

            }
        });
        public ICommand AddWindowConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new WindowConstructionAbridged(id, new List<string>(), $"New Window Construction {id.Substring(0, 5)}");

            var dialog = new Honeybee.UI.Dialog_Construction(newConstrucion);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = this._gridView.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
                d.Add(dialog_rc);
                this._gridView.DataStore = d;

            }
        });
        public ICommand AddShadeConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new ShadeConstruction(id, $"New Window Construction {id.Substring(0, 5)}");

            //var dialog = new Honeybee.UI.Dialog_Construction(newConstrucion);
            //var dialog_rc = dialog.ShowModal(this);
            //if (dialog_rc != null)
            //{
            //    var d = this._gridView.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
            //    d.Add(dialog_rc);
            //    this._gridView.DataStore = d;

            //}
        });

        public ICommand AddAirBoundaryConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new AirBoundaryConstructionAbridged(id, string.Empty, $"New AirBoundary Construction {id.Substring(0, 5)}");

            //var dialog = new Honeybee.UI.Dialog_Construction(newConstrucion);
            //var dialog_rc = dialog.ShowModal(this);
            //if (dialog_rc != null)
            //{
            //    var d = this._gridView.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
            //    d.Add(dialog_rc);
            //    this._gridView.DataStore = d;

            //}
        });

    }
}
