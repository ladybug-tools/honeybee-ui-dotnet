using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;

namespace Honeybee.UI
{

    public class Dialog_ConstructionManager : Dialog<List<HB.Energy.IConstruction>>
    {
        private bool _returnSelectedOnly;
        private ConstructionManagerViewModel _vm { get; set; }
        private Dialog_ConstructionManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Construction Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(800, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }


        public Dialog_ConstructionManager(ref HB.ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            libSource.FillNulls();

            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new ConstructionManagerViewModel(libSource, this);
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

            layout.AddSeparateRow("Constructions:", null, addNew, duplicate, edit, remove);

            // search bar
            var filter = new TextBox() { PlaceholderText = "Filter" };
            filter.TextBinding.Bind(_vm, _ => _.FilterKey);
            layout.AddRow(filter);

            gd = GenGridView();
            layout.AddRow(gd);
           

            // counts
            var counts = new Label();
            counts.TextBinding.Bind(_vm, _ => _.Counts);

            // unit switchs
            var unit = new RadioButtonList();
            unit.Items.Add("Metric");
            unit.Items.Add("Imperial");
            unit.SelectedIndex = 0;
            unit.Spacing = new Size(5, 0);
            unit.SelectedIndexChanged += (s, e) => _vm.UseIPUnit = unit.SelectedIndex == 1;

            layout.AddSeparateRow(counts, null, unit);

            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => OkCommand.Execute(this._returnSelectedOnly);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, OKButton, AbortButton, null);
            layout.AddRow(null);

            gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);

            return layout;

        }

        private GridView GenGridView()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _=>_.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedData = gd.SelectedItem as ConstructionViewData;
            };

            gd.Height = 250;

            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell {Binding = Binding.Delegate<ConstructionViewData, string>(r =>r.Name ) }, 
                HeaderText = "Name",
                Sortable = true,
                Width = 250
            });

          
            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell {Binding = Binding.Delegate<ConstructionViewData, string>(r => r.CType)}, 
                HeaderText = "Type",
                Sortable = true,
                Width = 80
            });

            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.RValue) }, 
                HeaderText = "RValue",
                Sortable = true
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.UValue) },
                HeaderText = "UValue",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.UFactor) },
                HeaderText = "UFactor",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.SHGC) },
                HeaderText = "SHGC",
                Sortable = true
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.TSolar) },
                HeaderText = "TSolar",
                Sortable = true
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.TVis) },
                HeaderText = "TVis",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ConstructionViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.Source) },
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
            System.Func<ConstructionViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {
              
                case "Name":
                    sortFunc = (ConstructionViewData _) => _.Name;
                    break;
                case "Type":
                    sortFunc = (ConstructionViewData _) => _.CType;
                    break;
                case "RValue":
                    sortFunc = (ConstructionViewData _) => _.RValue;
                    isNumber = true;
                    break;
                case "UValue":
                    sortFunc = (ConstructionViewData _) => _.UValue;
                    isNumber = true;
                    break;
                case "UFactor":
                    sortFunc = (ConstructionViewData _) => _.UFactor;
                    isNumber = true;
                    break;
                case "SHGC":
                    sortFunc = (ConstructionViewData _) => _.SHGC;
                    isNumber = true;
                    break;
                case "TSolar":
                    sortFunc = (ConstructionViewData _) => _.TSolar;
                    isNumber = true;
                    break;
                case "TVis":
                    sortFunc = (ConstructionViewData _) => _.TVis;
                    isNumber = true;
                    break;
                case "Locked":
                    sortFunc = (ConstructionViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (ConstructionViewData _) => _.Source;
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

       

    }
}
