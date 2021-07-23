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
    class ConstructionViewData
    {
        public string Name { get; }
        public string CType { get; }
        public string RValue { get; }
        public string RValueIP { get; }
        public string UFactor { get; }
        public string UFactorIP { get; }
        public HB.Energy.IConstruction Construction { get; }
        public static ModelEnergyProperties LibSource { get; set; }
        public ConstructionViewData(HB.Energy.IConstruction c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            this.CType = c.GetType().Name.Replace("Abridged", "").Replace("Construction", "");
            if (c is HB.Energy.IThermalConstruction tc)
            {
                tc.CalThermalValues(LibSource);
                this.RValue = Math.Round(tc.RValue, 5).ToString();
                this.UFactor = Math.Round(tc.UFactor, 5).ToString();

                this.RValueIP = Math.Round(tc.RValue *   5.678263337, 5).ToString();
                this.UFactorIP = Math.Round(tc.UFactor / 5.678263337, 5).ToString();
            }
            this.Construction = c;
         
        }
    }
    public class Dialog_ConstructionManager : Dialog<List<HB.Energy.IConstruction>>
    {
        private bool _returnSelectedOnly;
        private GridView _gd { get; set; }
        private ModelEnergyProperties _modelEnergyProperties { get; set; }

        private Dialog_ConstructionManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Construction Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(800, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }


        public Dialog_ConstructionManager(ref ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this._modelEnergyProperties = libSource;
            var constructions = libSource.ConstructionList;

            Content = Init(constructions);
        }

        private DynamicLayout Init(IEnumerable<HB.Energy.IConstruction> constructions)
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

            layout.AddSeparateRow("Constructions:", null, addNew, duplicate, edit, remove);

            ConstructionViewData.LibSource = _modelEnergyProperties;
            var data = constructions.Select(_ => new ConstructionViewData(_));
            this._gd = GenGridView(data);
            this._gd.Height = 250;
            layout.AddRow(this._gd);
            var gd = this._gd;


            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, OKButton, AbortButton, null);


            gd.CellDoubleClick += (s, e) => EditCommand.Execute(null);

            return layout;

        }

        private GridView GenGridView(IEnumerable<ConstructionViewData> items)
        {
            items = items ?? new List<ConstructionViewData>();
            var gd = new GridView() { DataStore = items };
            gd.Height = 250;

            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell {Binding = Binding.Delegate<ConstructionViewData, string>(r =>r.Name ) }, 
                HeaderText = "Name" 
            });

          
            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell {Binding = Binding.Delegate<ConstructionViewData, string>(r => r.CType)}, 
                HeaderText = "Type" 
            });

            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.RValue) }, 
                HeaderText = "RValue[m2·K/W]"
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.RValueIP) },
                HeaderText = "RValue[h·ft2·F/Btu]"
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.UFactor) },
                HeaderText = "UFactor[W/m2·K]"
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.UFactorIP) },
                HeaderText = "UFactor[Btu/h·ft2·F]"
            });
            return gd;
        }

        public ICommand AddOpaqueConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new OpaqueConstructionAbridged(id, new List<string>(), $"New Opaque Construction {id.Substring(0, 5)}");

            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, newConstrucion);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = this._gd.DataStore.OfType<ConstructionViewData>().ToList();
                d.Add(new ConstructionViewData(dialog_rc));
                this._gd.DataStore = d;

            }
        });
        public ICommand AddWindowConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new WindowConstructionAbridged(id, new List<string>(), $"New Window Construction {id.Substring(0, 5)}");

            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, newConstrucion);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = this._gd.DataStore.OfType<ConstructionViewData>().ToList();
                d.Add(new ConstructionViewData(dialog_rc));
                this._gd.DataStore = d;

            }
        });
        public ICommand AddShadeConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new ShadeConstruction(id, $"New Shade Construction {id.Substring(0, 5)}");

            var dialog = new Honeybee.UI.Dialog_Construction_Shade(newConstrucion);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = this._gd.DataStore.OfType<ConstructionViewData>().ToList();
                d.Add(new ConstructionViewData(dialog_rc));
                this._gd.DataStore = d;
            }
        });

        public ICommand AddAirBoundaryConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newConstrucion = new AirBoundaryConstructionAbridged(id, $"New AirBoundary Construction {id.Substring(0, 5)}");

            var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(newConstrucion);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = this._gd.DataStore.OfType<ConstructionViewData>().ToList();
                d.Add(new ConstructionViewData(dialog_rc));
                this._gd.DataStore = d;
            }
        });


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var menuDic = new Dictionary<string, ICommand>()
            {
                { "Opaque", AddOpaqueConstructionCommand},
                { "Window", AddWindowConstructionCommand},
                { "Shade",AddShadeConstructionCommand },
                { "AirBoundary", AddAirBoundaryConstructionCommand}
            };

            var gd = this._gd;
            var contextMenu = new ContextMenu();

            foreach (var item in menuDic)
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
            var selected = gd.SelectedItem as ConstructionViewData;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to duplicate!");
                return;
            }

            var id = Guid.NewGuid().ToString();
            var dup = selected.Construction.Duplicate() as HB.Energy.IConstruction;

            dup.Identifier = id;
            dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, dup);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = gd.DataStore.OfType<ConstructionViewData>().ToList();
                var newItem = new ConstructionViewData(dialog_rc);
                d.Add(newItem);
                gd.DataStore = d;

            }
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = (gd.SelectedItem as ConstructionViewData)?.Construction;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            HB.Energy.IConstruction dialog_rc;
            if (selected is ShadeConstruction shd)
            {
                var dup = shd.DuplicateShadeConstruction();
                var dialog = new Honeybee.UI.Dialog_Construction_Shade(dup);
                dialog_rc = dialog.ShowModal(this);
            }
            else if (selected is AirBoundaryConstructionAbridged airBoundary)
            {
                var dup = airBoundary.DuplicateAirBoundaryConstructionAbridged();
                var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(dup);
                dialog_rc = dialog.ShowModal(this);
            }
            else
            {
                // Opaque Construction or Window Construciton
                var dup = selected.Duplicate() as HB.Energy.IConstruction;
                var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, dup);
                dialog_rc = dialog.ShowModal(this);
            }

            if (dialog_rc == null) return;
            var newItem = new ConstructionViewData(dialog_rc);
            var index = gd.SelectedRow;
            var newDataStore = gd.DataStore.OfType<ConstructionViewData>().ToList();
            newDataStore.RemoveAt(index);
            newDataStore.Insert(index, newItem);
            gd.DataStore = newDataStore;


        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as ConstructionViewData;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            var res = MessageBox.Show(this, $"Are you sure you want to delete:\n {selected.Name}", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                var newDataStore = gd.DataStore.Where(_ => _ != selected).ToList();
                gd.DataStore = newDataStore;
            }
        });

        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var allItems = gd.DataStore.OfType<ConstructionViewData>().Select(_ => _.Construction).ToList();
            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as ConstructionViewData;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<HB.Energy.IConstruction>() { d.Construction };
            }

            this._modelEnergyProperties.Constructions.Clear();
            this._modelEnergyProperties.AddConstructions(allItems);
            Close(itemsToReturn);
        });



    }
}
