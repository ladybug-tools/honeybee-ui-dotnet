using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class Dialog_SHWManager : Dialog<List<HB.SHWSystem>>
    {
        private bool _returnSelectedOnly;
        private SHWManagerViewModel _vm { get; set; }

        private Dialog_SHWManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Service Hot Water System Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 650;
            this.Icon = DialogHelper.HoneybeeIcon;
        }

     
        public Dialog_SHWManager(ref ModelEnergyProperties libSource, bool returnSelectedOnly = false, Func<string> roomIDPicker = default) : this()
        {
            libSource.FillNulls();

            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new SHWManagerViewModel(libSource, this, roomIDPicker: roomIDPicker);
            Content = Init(out var gd);
            this._vm.GridControl = gd;
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

            layout.AddSeparateRow("Service Hot Water Systems:", null, addNew, duplicate, edit, remove);

            // search bar
            var filter = new TextBox() { PlaceholderText = "Filter" };
            filter.TextBinding.Bind(_vm, _ => _.FilterKey);
            layout.AddSeparateRow(filter);

            gd = GenGridView();
            layout.AddSeparateRow(controls: new[] { gd }, xscale: true, yscale: true);

            gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);

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

        private GridView GenGridView()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedData = gd.SelectedItem as SHWViewData;
            };

            gd.Height = 250;
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<SHWViewData, string>(r => r.Name) },
                HeaderText = "Name",
                Sortable = true,
                Width = 200
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<SHWViewData, string>(r => r.EType) },
                HeaderText = "Type",
                Sortable = true,
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<SHWViewData, string>(r => r.HeaterEfficiency) },
                HeaderText = "Efficiency",
                Sortable = true,
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<SHWViewData, string>(r => r.Condition) },
                HeaderText = "Condition",
                Sortable = true,
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<SHWViewData, string>(r => r.LossCoeff) },
                HeaderText = "LossCoeff",
                Sortable = true,
            });


            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<SHWViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<SHWViewData, string>(r => r.Source) },
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
            System.Func<SHWViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {

                case "Name":
                    sortFunc = (SHWViewData _) => _.Name;
                    break;
                case "EType":
                    sortFunc = (SHWViewData _) => _.EType;
                    break;
                case "Efficiency":
                    sortFunc = (SHWViewData _) => _.HeaterEfficiency;
                    break;
                case "Condition":
                    sortFunc = (SHWViewData _) => _.Condition;
                    break;
                case "LossCoeff":
                    sortFunc = (SHWViewData _) => _.LossCoeff;
                    break;
                case "Locked":
                    sortFunc = (SHWViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (SHWViewData _) => _.Source;
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
            var itemsToReturn = _vm.GetUserItems(this._returnSelectedOnly);
            _doneAction?.Invoke(itemsToReturn);
            Close(itemsToReturn);
        });

        private Action<List<HB.SHWSystem>> _doneAction;
        public void SetDoneAction(Action<List<HB.SHWSystem>> doneAction)
        {
            _doneAction = doneAction;
        }

    }
}
