using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ScheduleRulesetManager : Dialog<List<HB.Energy.ISchedule>>
    {
        private bool _returnSelectedOnly;
        private ScheduleRulesetManagerViewModel _vm { get; set; }


        private Dialog_ScheduleRulesetManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Schedule Ruleset Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(600, 400);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

    
        public Dialog_ScheduleRulesetManager(ref ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            libSource.FillNulls();

            this._returnSelectedOnly = returnSelectedOnly;
            this._vm = new ScheduleRulesetManagerViewModel(libSource, this);
            Content = Init();
        }

        private DynamicLayout Init()
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


            layout.AddSeparateRow("Schedule Rulesets:", null, addNew, duplicate, edit, remove);

            // search bar
            var filter = new TextBox() { PlaceholderText = "Filter" };
            filter.TextBinding.Bind(_vm, _ => _.FilterKey);
            layout.AddRow(filter);

            var gd = GenGridView();
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
                _vm.SelectedData = gd.SelectedItem as ScheduleRulesetViewData;
            };

            gd.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<ScheduleRulesetViewData, string>(r => r.Name)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name", Sortable = true });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<ScheduleRulesetViewData, string>(  r => r.TypeLimit)
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type", Sortable = true });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new CheckBoxCell { Binding = Binding.Delegate<ScheduleRulesetViewData, bool?>(r => r.Locked) },
                HeaderText = "Locked",
                Sortable = true
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ScheduleRulesetViewData, string>(r => r.Source) },
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
            System.Func<ScheduleRulesetViewData, string> sortFunc = null;
            var isNumber = false;
            switch (colName)
            {

                case "Name":
                    sortFunc = (ScheduleRulesetViewData _) => _.Name;
                    break;
                case "Type":
                    sortFunc = (ScheduleRulesetViewData _) => _.TypeLimit;
                    break;
                case "Locked":
                    sortFunc = (ScheduleRulesetViewData _) => _.Locked.ToString();
                    break;
                case "Source":
                    sortFunc = (ScheduleRulesetViewData _) => _.Source;
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
