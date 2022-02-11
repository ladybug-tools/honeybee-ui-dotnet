using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ConstructionSetManager : Dialog<List<HB.Energy.IBuildingConstructionset>>
    {
        private bool _returnSelectedOnly;
        private ConstructionSetManagerViewModel _vm { get; set; }

        private Dialog_ConstructionSetManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"ConstructionSet Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(750, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        public Dialog_ConstructionSetManager(ref ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            libSource.FillNulls();

            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new ConstructionSetManagerViewModel(libSource, this);
            Content = Init(out var gd);
            this._vm.GridControl = gd;
        }

        protected override void OnSizeChanged(System.EventArgs e)
        {
            base.OnSizeChanged(e);
            _vm?.DialogSizeChanged();
        }

        private DynamicLayout Init(out GridView gd)
        {
            var layout = new DynamicLayout();
            layout.DefaultPadding = new Padding(5);
            layout.DefaultSpacing = new Size(5, 5);

            var addNew = new Button { Text = "Add" };
            addNew.Command = _vm.AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = _vm.DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = _vm.EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = _vm.RemoveCommand;

            layout.AddSeparateRow("Construction Sets:", null, addNew, duplicate, edit, remove);

            // search bar
            var filter = new TextBox() { PlaceholderText = "Filter" };
            filter.TextBinding.Bind(_vm, _ => _.FilterKey);
            layout.AddRow(filter);

            gd = GenGridView();
            gd.Height = 250;
            layout.AddRow(gd);

            // counts
            var counts = new Label();
            counts.TextBinding.Bind(_vm, _ => _.Counts);
            layout.AddSeparateRow(counts, null);

            gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => OkCommand.Execute(null);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
            layout.AddRow(null);
            return layout;

        }

        private GridView GenGridView()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedData = gd.SelectedItem as ConstructionSetViewData;
            };

            gd.Height = 250;
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, string>(r => r.Name) },
                HeaderText = "Name",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.HasWallSet) },
                HeaderText = "Wall",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.HasRoofCeilingSet) },
                HeaderText = "RoofCeiling",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.HasFloorSet) },
                HeaderText = "Floor",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.HasApertureSet) },
                HeaderText = "Aperture",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.HasDoorSet) },
                HeaderText = "Door",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.HasAirBoundaryConstruction) },
                HeaderText = "AirBoundary",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.HasShadeSet) },
                HeaderText = "Shade",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionSetViewData, string>(r => r.Source) },
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
            System.Func<ConstructionSetViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {

                case "Name":
                    sortFunc = (ConstructionSetViewData _) => _.Name;
                    break;
                case "Wall":
                    sortFunc = (ConstructionSetViewData _) => _.HasWallSet.ToString();
                    break;
                case "RoofCeiling":
                    sortFunc = (ConstructionSetViewData _) => _.HasRoofCeilingSet.ToString();
                    break;
                case "Floor":
                    sortFunc = (ConstructionSetViewData _) => _.HasFloorSet.ToString();
                    break;
                case "Aperture":
                    sortFunc = (ConstructionSetViewData _) => _.HasApertureSet.ToString();
                    break;
                case "Door":
                    sortFunc = (ConstructionSetViewData _) => _.HasDoorSet.ToString();
                    break;
                case "AirBoundary":
                    sortFunc = (ConstructionSetViewData _) => _.HasAirBoundaryConstruction.ToString();
                    break;
                case "Shade":
                    sortFunc = (ConstructionSetViewData _) => _.HasShadeSet.ToString();
                    break;
                case "Locked":
                    sortFunc = (ConstructionSetViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (ConstructionSetViewData _) => _.Source;
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
            var itemsToReturn = _vm.GetUserItems(_returnSelectedOnly);
            Close(itemsToReturn);
        });


    }
}
