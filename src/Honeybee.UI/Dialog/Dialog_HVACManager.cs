using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class Dialog_HVACManager : Dialog<List<HB.Energy.IHvac>>
    {
        private bool _returnSelectedOnly;
        private HVACManagerViewModel _vm { get; set; }

        private Dialog_HVACManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"HVAC Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 650;
            this.Icon = DialogHelper.HoneybeeIcon;
        }

     
        public Dialog_HVACManager(ref ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            libSource.FillNulls();


            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new HVACManagerViewModel(libSource, this);
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

            layout.AddSeparateRow("HVACs:", null, addNew, duplicate, edit, remove);

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
                _vm.SelectedData = gd.SelectedItem as HVACViewData;
            };

            gd.Height = 250;
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<HVACViewData, string>(r => r.Name) },
                HeaderText = "Name",
                Sortable = true,
                Width = 200
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<HVACViewData, string>(r => r.CType) },
                HeaderText = "Type",
                Sortable = true,
                Width = 250
            });

           
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<HVACViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<HVACViewData, string>(r => r.Source) },
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
            System.Func<HVACViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {

                case "Name":
                    sortFunc = (HVACViewData _) => _.Name;
                    break;
                case "Type":
                    sortFunc = (HVACViewData _) => _.CType;
                    break;
                case "Locked":
                    sortFunc = (HVACViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (HVACViewData _) => _.Source;
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
            Close(itemsToReturn);
        });


        private bool DisableDetailedHVAC()
        {
            var disable = this._returnSelectedOnly;
            if (disable)
            {
                Dialog_Message.Show(this, "DetailedHVAC cannot be used under the current selecting mode! Use the HVAC Manager to enable DetailedHVAC editor!");
            }
            return disable;
        }

        public void SetAddDetailedHVACFunc(Action func)
        {
            if (DisableDetailedHVAC())
                return;
            _vm.AddDetailedHVAC = func;
        }
        public void SetEditDetailedHVACFunc(Action<Guid> func)
        {
            if (DisableDetailedHVAC())
                return;
            _vm.EditDetailedHVAC = func;
        }

        public void SetRemoveDetailedHVACFunc(Action<Guid> func)
        {
            _vm.RemoveDetailedHVAC = func;
        }
        public void SetDuplicateDetailedHVACFunc(Action<Guid> func)
        {
            if (DisableDetailedHVAC())
                return;
            _vm.DuplicateDetailedHVAC = func;
        }
    }
}
