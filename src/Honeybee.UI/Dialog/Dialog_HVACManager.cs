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
        private Button _editT;

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
            if (this._returnSelectedOnly )
            {
                Title = $"Select a system - {DialogHelper.PluginName}";
            }

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
            addNew.Visible = !this._returnSelectedOnly;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = _vm.DuplicateCommand;
            duplicate.Visible = !this._returnSelectedOnly;

            var edit = new Button { Text = "Edit" };
            edit.Command = _vm.EditCommand;
            edit.Visible = !this._returnSelectedOnly;

            var remove = new Button { Text = "Remove" };
            remove.Command = _vm.RemoveCommand;
            remove.Visible = !this._returnSelectedOnly;

            _editT = new Button { Text = "Edit IB" };
            _editT.Command = _vm.EditIBSystemCommand;
            _editT.Visible = !_returnSelectedOnly;
            _editT.Enabled = false;

            layout.AddSeparateRow("HVACs:", null, addNew, duplicate, edit, remove);

            // search bar
            var filter = new TextBox() { PlaceholderText = "Filter" };
            filter.TextBinding.Bind(_vm, _ => _.FilterKey);
            layout.AddSeparateRow(filter);

            gd = GenGridView();
            layout.AddSeparateRow(controls: new[] { gd }, xscale: true, yscale: true);

            
            // counts
            var counts = new Label();
            counts.TextBinding.Bind(_vm, _ => _.Counts);

            layout.AddSeparateRow(counts, null);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(_editT, null, DefaultButton, AbortButton);

            return layout;
        }

        private GridView GenGridView()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
           
            gd.SelectedItemsChanged += (s, e) => {
                var sd = gd.SelectedItem as HVACViewData;
                _vm.SelectedData = sd;
                if (_editT != null && sd != null)
                    _editT.Enabled = sd.HasIB;
                else
                    _editT.Enabled = false;
            };

            if (!this._returnSelectedOnly)
            {
                gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);
            }
          

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
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<HVACViewData, bool?>(r => r.HasIB) },
                HeaderText = "IB",
                Sortable = true
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
                case "IB":
                    sortFunc = (HVACViewData _) => _.HasIB.ToString();
                    break;
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

        public void SetGetDetailedHVACItemsFunc(Func<Action,ButtonMenuItem> func)
        {
            HVACManagerViewModel.GetDetailedHVACItems = func;
        }

        public void SetValidatePlaceholderFunc(Func<DetailedHVAC, bool> func)
        {
            HVACManagerViewModel.ValidatePlaceholder = func;
        }

        public void SetEditDetailedHVACFunc(Action<DetailedHVAC> func)
        {
            HVACManagerViewModel.EditDetailedHVAC = func;
        }

        public void SetEditIBFunc(Action<DetailedHVAC> func)
        {
            HVACManagerViewModel.EditIBDoc = func;
        }
        public void SetDuplicateIBFunc(Action<DetailedHVAC> func)
        {
            HVACManagerViewModel.DuplicateIBDoc = func;
        }

    }
}
