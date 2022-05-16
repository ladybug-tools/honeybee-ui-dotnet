using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ProgramTypeManager : Dialog<List<HB.Energy.IProgramtype>>
    {
        private bool _returnSelectedOnly;
        private ProgramTypeManagerViewModel _vm { get; set; }
        private Dialog_ProgramTypeManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Program Type Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 900;

            this.Icon = DialogHelper.HoneybeeIcon;
        }



        public Dialog_ProgramTypeManager(ref ModelProperties libSource, bool returnSelectedOnly = false):this()
        {
            libSource.FillNulls();

            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new ProgramTypeManagerViewModel(libSource, this);
            Content = Init(out var gd);
            this._vm.GridControl = gd;
        }


        private DynamicLayout Init(out GridView gd)
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(5);

            var addNew = new Button { Text = "Add" };
            addNew.Command = _vm.AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = _vm.DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = _vm.EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = _vm.RemoveCommand;

            layout.AddSeparateRow("Program Types:", null, addNew, duplicate, edit, remove);

            // search bar
            var filter = new TextBox() { PlaceholderText = "Filter" };
            filter.TextBinding.Bind(_vm, _ => _.FilterKey);
            layout.AddSeparateRow(filter);

            gd = GenProgramType_GV();
            gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);
            layout.AddSeparateRow(controls: new[] { gd }, xscale: true, yscale: true);

            // counts
            var counts = new Label();
            counts.TextBinding.Bind(_vm, _ => _.Counts);

            layout.AddSeparateRow(counts, null);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => OkCommand.Execute(null);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);

            return layout;
        }

        private GridView GenProgramType_GV()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedData = gd.SelectedItem as ProgramTypeViewData;
            };

            gd.Height = 250;

            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, string>(r => r.Name)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name", Sortable = true });

            var peopleTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>( r => r.HasPeople)
            };
            gd.Columns.Add(new GridColumn { DataCell = peopleTB, HeaderText = "People", Sortable = true });

            var lightingTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.HasLighting)
            };
            gd.Columns.Add(new GridColumn { DataCell = lightingTB, HeaderText = "Lighting", Sortable = true });

            var equipTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.HasElecEquip)
            };
            gd.Columns.Add(new GridColumn { DataCell = equipTB, HeaderText = "ElecEquip", Sortable = true });

            var gasTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.HasGasEquip)
            };
            gd.Columns.Add(new GridColumn { DataCell = gasTB, HeaderText = "GasEquip", Sortable = true });

            var infTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.HasInfiltration)
            };
            gd.Columns.Add(new GridColumn { DataCell = infTB, HeaderText = "Infiltration", Sortable = true });

            var ventTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.HasVentilation)
            };
            gd.Columns.Add(new GridColumn { DataCell = ventTB, HeaderText = "Ventilation", Sortable = true });

            var setpointTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.HasSetpoint)
            };
            gd.Columns.Add(new GridColumn { DataCell = setpointTB, HeaderText = "Setpoint", Sortable = true });

            var serviceHotWaterTB = new CheckBoxCell
            {
                Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.HasServiceHotWater)
            };
            gd.Columns.Add(new GridColumn { DataCell = serviceHotWaterTB, HeaderText = "ServiceHotWater", Sortable = true });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ProgramTypeViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ProgramTypeViewData, string>(r => r.Source) },
                HeaderText = "Source",
                Sortable = true
            });

            // sorting by header
            gd.ColumnHeaderClick += OnColumnHeaderClick;
            return gd;
        }

        private string _currentSortByColumn;
        private void OnColumnHeaderClick(object sender, GridColumnEventArgs e)
        {
            var cell = e.Column.DataCell;
            var colName = e.Column.HeaderText;
            System.Func<ProgramTypeViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {

                case "Name":
                    sortFunc = (ProgramTypeViewData _) => _.Name;
                    break;
                case "People":
                    sortFunc = (ProgramTypeViewData _) => _.HasPeople.ToString();
                    break;
                case "Lighting":
                    sortFunc = (ProgramTypeViewData _) => _.HasLighting.ToString();
                    break;
                case "ElecEquip":
                    sortFunc = (ProgramTypeViewData _) => _.HasElecEquip.ToString();
                    break;
                case "GasEquip":
                    sortFunc = (ProgramTypeViewData _) => _.HasGasEquip.ToString();
                    break;
                case "Infiltration":
                    sortFunc = (ProgramTypeViewData _) => _.HasInfiltration.ToString();
                    break;
                case "Ventilation":
                    sortFunc = (ProgramTypeViewData _) => _.HasVentilation.ToString();
                    break;
                case "Setpoint":
                    sortFunc = (ProgramTypeViewData _) => _.HasSetpoint.ToString();
                    break;
                case "ServiceHotWater":
                    sortFunc = (ProgramTypeViewData _) => _.HasServiceHotWater.ToString();
                    break;
                case "Locked":
                    sortFunc = (ProgramTypeViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (ProgramTypeViewData _) => _.Source;
                    break;
                default:
                    break;
            }

            if (sortFunc == null) return;

            var descend = colName == _currentSortByColumn;
            _vm.SortList(sortFunc, isNumber, descend);

            _currentSortByColumn = colName == _currentSortByColumn ? string.Empty : colName;

        }


        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var pTypesToReturn = _vm.GetUserItems(_returnSelectedOnly);
            Close(pTypesToReturn);
        });
    
    
    }
}
