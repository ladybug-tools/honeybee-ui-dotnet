using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ScheduleRulesetManager : Dialog<List<HB.ScheduleRulesetAbridged>>
    {
        private GridView _gd;
        private bool _returnSelectedOnly;
        private ModelEnergyProperties _modelEnergyProperties;


        private List<ScheduleTypeLimit> _typeLimits;
        private ScheduleRuleset AbridgedToReal (ScheduleRulesetAbridged obj)
        {
            var typeLimit = _typeLimits.FirstOrDefault(_ => _.Identifier == obj.ScheduleTypeLimit);
            
            var realObj = new ScheduleRuleset(obj.Identifier, obj.DaySchedules, obj.DefaultDaySchedule, obj.DisplayName,
               obj.ScheduleRules, obj.HolidaySchedule, obj.SummerDesigndaySchedule, obj.WinterDesigndaySchedule, typeLimit);

            return realObj;
        }
        private ScheduleRulesetAbridged ToAbridged(ScheduleRuleset obj)
        {
            var abridged = new ScheduleRulesetAbridged(obj.Identifier, obj.DaySchedules, obj.DefaultDaySchedule, obj.DisplayName,
               obj.ScheduleRules, obj.HolidaySchedule, obj.SummerDesigndaySchedule, obj.WinterDesigndaySchedule, obj.ScheduleTypeLimit?.Identifier);

            return abridged;
        }

        private Dialog_ScheduleRulesetManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Schedule Ruleset Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(600, 400);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        [Obsolete("This is deprecated", true)]
        public Dialog_ScheduleRulesetManager(List<HB.ScheduleRulesetAbridged> scheduleRulesets, List<HB.ScheduleTypeLimit> scheduleTypeLimits):this()
        {
            _typeLimits = scheduleTypeLimits;
            Content = Init(scheduleRulesets);
        }

        public Dialog_ScheduleRulesetManager(ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this._modelEnergyProperties = libSource;

            var allSches = libSource.Schedules?.OfType<ScheduleRulesetAbridged>();
            var schTypes = libSource.ScheduleTypeLimits;

            _typeLimits = schTypes;
            Content = Init(allSches);
        }

        private DynamicLayout Init(IEnumerable<HB.ScheduleRulesetAbridged> allSches)
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


            layout.AddSeparateRow("Schedule Rulesets:", null, addNew, duplicate, edit, remove);
            var gd = GenGridView(allSches);
            _gd = gd;
            layout.AddSeparateRow(gd);



            gd.CellDoubleClick += (s, e) => EditCommand.Execute(null);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null);
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
            return layout;
        }

        private GridView GenGridView(IEnumerable<object> items)
        {
            items = items ?? new List<HB.ScheduleRulesetAbridged>();
            var gd = new GridView() { DataStore = items };
            gd.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ScheduleRulesetAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ScheduleRulesetAbridged, string>(
                    r => {
                        if (r.ScheduleTypeLimit == null)
                            return null;
                        
                        var typeLimit = _typeLimits.FirstOrDefault(_ => _.Identifier == r.ScheduleTypeLimit);
                        return typeLimit.DisplayName ?? typeLimit.Identifier;
                    })
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type" });

            return gd;
        }



        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var dayId = Guid.NewGuid().ToString();
            var dayName = $"New Schedule Day {dayId.Substring(0, 5)}";
            var newDay = new ScheduleDay(
                dayId,
                new List<double> { 0.3 },
                dayName,
                new List<List<int>>() { new List<int> { 0, 0 } }
                );

            var id = Guid.NewGuid().ToString();
            var newSch = new ScheduleRuleset(
                id,
                new List<ScheduleDay> { newDay },
                dayId,
                $"New Schedule Ruleset {id.Substring(0, 5)}"
                );
            //TODO: needs to create type limit library.
            newSch.ScheduleTypeLimit = new ScheduleTypeLimit("Fractional", "", 0, 1);
            var dialog = new Honeybee.UI.Dialog_Schedule(newSch);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = gd.DataStore.Select(_ => _ as HB.ScheduleRulesetAbridged).ToList();
                d.Add(ToAbridged(dialog_rc));
                gd.DataStore = d;


            }
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            if (gd.SelectedRow == -1)
            {
                MessageBox.Show(this, "Nothing is selected to duplicate!");
                return;
            }

            var id = Guid.NewGuid().ToString();

            var newDup = (gd.SelectedItem as ScheduleRulesetAbridged).DuplicateScheduleRulesetAbridged();
            newDup.Identifier = id;
            newDup.DisplayName = string.IsNullOrEmpty(newDup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{newDup.DisplayName}_dup";
            //var rules = newDup.ScheduleRules;
            //foreach (var rule in rules)
            //{
            //    var dayname = rule.ScheduleDay

            //}
            var realObj = AbridgedToReal(newDup);
            var dialog = new Honeybee.UI.Dialog_Schedule(realObj);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = gd.DataStore.Select(_ => _ as ScheduleRulesetAbridged).ToList();
                d.Add(ToAbridged(dialog_rc));
                gd.DataStore = d;

            }
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as ScheduleRulesetAbridged;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            var newDup = selected.DuplicateScheduleRulesetAbridged();
            var realObj = AbridgedToReal(newDup);
            var dialog = new Honeybee.UI.Dialog_Schedule(realObj);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var index = gd.SelectedRow;
                var newDataStore = gd.DataStore.Select(_ => _ as ScheduleRulesetAbridged).ToList();
                newDataStore.RemoveAt(index);
                newDataStore.Insert(index, ToAbridged(dialog_rc));

                gd.DataStore = newDataStore;

            }
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as ScheduleRulesetAbridged;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            if (selected.Identifier.StartsWith("Generic_"))
            {
                MessageBox.Show(this, $"{selected.DisplayName ?? selected.Identifier } cannot be removed, because it is Honeybee default modifier set.");
                return;
            }

            var res = MessageBox.Show(this, $"Are you sure you want to delete:\n {selected.DisplayName ?? selected.Identifier }", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                var newDataStore = gd.DataStore.Where(_ => _ != selected).ToList();
                gd.DataStore = newDataStore;
            }
        });

        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var gd = this._gd;

            var allItems = gd.DataStore.OfType<ScheduleRulesetAbridged>().ToList();
            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as ScheduleRulesetAbridged;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<ScheduleRulesetAbridged>() { d };
            }

      
            this._modelEnergyProperties.ScheduleTypeLimits.Clear();
            this._modelEnergyProperties.AddScheduleTypeLimits(_typeLimits);

            this._modelEnergyProperties.Schedules.Clear();
            this._modelEnergyProperties.AddSchedules(allItems);

            Close(itemsToReturn);
        });



    }
}
