using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ModifierManager : Dialog<List<HB.Radiance.IModifier>>
    {
        private bool _returnSelectedOnly;
        private ModifierManagerViewModel _vm { get; set; }
        private Dialog_ModifierManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Modifiers Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(650, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

 
        public Dialog_ModifierManager(ref ModelRadianceProperties libSource, bool returnSelectedOnly = false) : this()
        {
            libSource.FillNulls();

            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new ModifierManagerViewModel(libSource, this);
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

            layout.AddSeparateRow("Modifiers:", null, addNew, duplicate, edit, remove);

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

            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, OKButton, AbortButton, null);

            gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);

            return layout;
        }



        private GridView GenGridView()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedData = gd.SelectedItem as ModifierViewData;
            };

            gd.Height = 250;

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierViewData, string>(r => r.Name) },
                HeaderText = "Name",
                Sortable = true,
                Width = 250
            });


            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierViewData, string>(r => r.CType) },
                HeaderText = "Type",
                Sortable = true,
                Width = 80
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierViewData, string>(r => r.Reflectance) },
                HeaderText = "Reflectance",
                Sortable = true
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierViewData, string>(r => r.Transmittance) },
                HeaderText = "Transmittance",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierViewData, string>(r => r.Emittance) },
                HeaderText = "Emittance",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ModifierViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ModifierViewData, string>(r => r.Source) },
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
            System.Func<ModifierViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {

                case "Name":
                    sortFunc = (ModifierViewData _) => _.Name;
                    break;
                case "Type":
                    sortFunc = (ModifierViewData _) => _.CType;
                    break;
                case "Reflectance":
                    sortFunc = (ModifierViewData _) => _.Reflectance;
                    isNumber = true;
                    break;
                case "Transmittance":
                    sortFunc = (ModifierViewData _) => _.Transmittance;
                    isNumber = true;
                    break;
                case "Emittance":
                    sortFunc = (ModifierViewData _) => _.Emittance;
                    isNumber = true;
                    break;
                case "Locked":
                    sortFunc = (ModifierViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (ModifierViewData _) => _.Source;
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
