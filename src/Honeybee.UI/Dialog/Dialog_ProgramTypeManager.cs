using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ProgramTypeManager : Dialog<List<HB.Energy.IProgramtype>>
    {
        private GridView _gd;
        private bool _returnSelectedOnly;
        private ModelEnergyProperties _modelEnergyProperties { get; set; }

        private Dialog_ProgramTypeManager()
        {
            Padding = new Padding(5);
            Title = $"Program Type Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 900;

            this.Icon = DialogHelper.HoneybeeIcon;
        }


        [Obsolete("This is deprecated", false)]
        public Dialog_ProgramTypeManager(ModelEnergyProperties libSource, List<HB.ProgramTypeAbridged> programTypes) : this()
        {
            this._modelEnergyProperties = libSource;
            Content = Init(libSource, programTypes);
        }


        public Dialog_ProgramTypeManager(ModelEnergyProperties libSource, bool returnSelectedOnly = false):this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this._modelEnergyProperties = libSource;
            var pTypes = libSource.ProgramTypes.OfType<ProgramTypeAbridged>().ToList();

            Content = Init(libSource, pTypes);
        }

        private DynamicLayout Init(ModelEnergyProperties libSource, List<HB.ProgramTypeAbridged> pTypes)
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(10, 5);

            var addNew = new Button { Text = "Add" };
            addNew.Command = AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = RemoveCommand;

            layout.AddSeparateRow("Program Types:", null, addNew, duplicate, edit, remove);
            var gd = GenProgramType_GV(pTypes);
            _gd = gd;
            layout.AddSeparateRow(gd);


            gd.CellDoubleClick += (s, e) => EditCommand.Execute(null);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => OkCommand.Execute(null);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null);
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
            layout.AddSeparateRow(null);

            return layout;
        }

        private GridView GenProgramType_GV(IEnumerable<object> items)
        {
            var pType_GD = new GridView() { DataStore = items };
            pType_GD.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var peopleTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>( r => r.People == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = peopleTB, HeaderText = "People", Sortable = true });

            var lightingTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>(r => r.Lighting == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = lightingTB, HeaderText = "Lighting", Sortable = true });

            var equipTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>(r => r.ElectricEquipment == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = equipTB, HeaderText = "ElecEquip", Sortable = true });

            var gasTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>(r => r.GasEquipment == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = gasTB, HeaderText = "GasEquip", Sortable = true });

            var infTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>(r => r.Infiltration == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = infTB, HeaderText = "Infiltration", Sortable = true });

            var ventTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>(r => r.Ventilation == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = ventTB, HeaderText = "Ventilation", Sortable = true });

            var setpointTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>(r => r.Setpoint == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = setpointTB, HeaderText = "Setpoint", Sortable = true });

            var serviceHotWaterTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, bool?>(r => r.ServiceHotWater == null ? false : true)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = serviceHotWaterTB, HeaderText = "ServiceHotWater", Sortable = true });


            return pType_GD;
        }


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var dialog = new Honeybee.UI.Dialog_OpsProgramTypes(this._modelEnergyProperties);
            var dialog_rc = dialog.ShowModal(this);

            var type = dialog_rc.programType;
            var sches = dialog_rc.schedules;
            if (type != null)
            {
                // add schedules
                var existingScheduleIds = this._modelEnergyProperties.Schedules.Select(_ => (_.Obj as HB.IDdEnergyBaseModel).Identifier);
                foreach (var sch in sches)
                {
                    if (existingScheduleIds.Any(_ => _ == sch.Identifier))
                        continue;
                    this._modelEnergyProperties.Schedules.Add(sch);
                }

                this._modelEnergyProperties.AddProgramType(type);

                // add program type
                gd.DataStore = this._modelEnergyProperties.ProgramTypes.OfType<ProgramTypeAbridged>().ToList();

            }
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            if (gd.SelectedItem == null)
            {
                MessageBox.Show(this, "Nothing is selected to duplicate!");
                return;
            }

            var id = Guid.NewGuid().ToString();
            var newPType = (gd.SelectedItem as ProgramTypeAbridged).DuplicateProgramTypeAbridged();
            newPType.Identifier = id;
            newPType.DisplayName = string.IsNullOrEmpty(newPType.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{newPType.DisplayName}_dup";
            var dialog = new Honeybee.UI.Dialog_ProgramType(this._modelEnergyProperties, newPType);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = gd.DataStore.Select(_ => _ as ProgramTypeAbridged).ToList();
                d.Add(dialog_rc);
                gd.DataStore = d;
            }
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            var newPType = (selected as ProgramTypeAbridged).DuplicateProgramTypeAbridged();
            var dialog = new Honeybee.UI.Dialog_ProgramType(this._modelEnergyProperties, newPType);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var index = gd.SelectedRow;
                var newDataStore = gd.DataStore.Select(_ => _ as ProgramTypeAbridged).ToList();
                newDataStore.RemoveAt(index);
                newDataStore.Insert(index, dialog_rc);
                gd.DataStore = newDataStore;

            }
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as ProgramTypeAbridged;
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
        });

        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var allPTypes = gd.DataStore.OfType<HB.Energy.IProgramtype>().ToList();
            var pTypesToReturn = allPTypes;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as HB.Energy.IProgramtype;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                pTypesToReturn = new List<HB.Energy.IProgramtype>() { d };
            }

            this._modelEnergyProperties.ProgramTypes.Clear();
            this._modelEnergyProperties.AddProgramTypes(allPTypes);
            Close(pTypesToReturn);
        });
    
    
    }
}
