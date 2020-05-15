using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ScheduleRulesetManager : Dialog<HB.ScheduleRulesetAbridged>
    {
        private List<ScheduleTypeLimit> _typeLimits;
        private ScheduleRuleset AbridgedToReal (ScheduleRulesetAbridged obj)
        {
            var typeLimit = _typeLimits.First(_ => _.Identifier == obj.ScheduleTypeLimit);
            var realObj = new ScheduleRuleset(obj.Identifier, obj.DaySchedules, obj.DefaultDaySchedule, obj.DisplayName,
               obj.ScheduleRules, obj.HolidaySchedule, obj.SummerDesigndaySchedule, obj.WinterDesigndaySchedule, typeLimit);

            return realObj;
        }
        private ScheduleRulesetAbridged ToAbridged(HB.Model model, ScheduleRuleset obj)
        {
            var ifTypeLimitExist = model.Properties.Energy.ScheduleTypeLimits.Any(_ => _.Identifier == obj.ScheduleTypeLimit.Identifier);
            if (!ifTypeLimitExist)
            {
                model.Properties.Energy.ScheduleTypeLimits.Add(obj.ScheduleTypeLimit);
            }
            var abridged = new ScheduleRulesetAbridged(obj.Identifier, obj.DaySchedules, obj.DefaultDaySchedule, obj.DisplayName,
               obj.ScheduleRules, obj.HolidaySchedule, obj.SummerDesigndaySchedule, obj.WinterDesigndaySchedule, obj.ScheduleTypeLimit.Identifier);

            return abridged;
        }

        public Dialog_ScheduleRulesetManager(HB.Model model)
        {
            try
            {
                var md = model;
                var allSches = md.Properties.Energy.Schedules.Where(_ => _.Obj is HB.ScheduleRulesetAbridged).Select(_ => _.Obj as HB.ScheduleRulesetAbridged);
                _typeLimits = md.Properties.Energy.ScheduleTypeLimits;
                
                //var convertedReal = schAbridgeds.Select(_ => AbridgedToReal(_));
                //var sches = md.Properties.Energy.Schedules.Where(_=> _.Obj is HB.ScheduleRuleset).Select(_=>_.Obj as HB.ScheduleRuleset);
                

                Padding = new Padding(5);
                Resizable = true;
                Title = "Schedule Ruleset Manager - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(600, 400);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout();
                layout.DefaultSpacing = new Size(5, 5);
                layout.DefaultPadding = new Padding(10, 5);

                var addNew = new Button { Text = "Add" };
                var duplicate = new Button { Text = "Duplicate" };
                var edit = new Button { Text = "Edit" };
                var remove = new Button { Text = "Remove" };


                layout.AddSeparateRow("Schedule Rulesets:", null, addNew, duplicate, edit, remove);
                var gd = GenGridView(allSches);
                layout.AddSeparateRow(gd);

                addNew.Click += (s, e) =>
                {
                    var dayId = Guid.NewGuid().ToString();
                    var dayName = $"New Schedule Day {dayId.Substring(0, 5)}";
                    var newDay = new ScheduleDay(
                        dayId, 
                        new List<double> { 0.3 },
                        dayName, 
                        new List<List<int>>() { new List<int> { 0, 0 } }
                        );

                    var id = Guid.NewGuid().ToString();
                    var newPType = new ScheduleRuleset(
                        id, 
                        new List<ScheduleDay> { newDay},
                        dayName,
                        $"New Schedule Ruleset {id.Substring(0, 5)}"
                        );

                    var dialog = new Honeybee.UI.Dialog_Schedule(newPType);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.Select(_ => _ as HB.ScheduleRulesetAbridged).ToList();
                        d.Add(ToAbridged(model, dialog_rc));
                        gd.DataStore = d;


                    }
                };
                duplicate.Click += (s, e) =>
                {
                    if (gd.SelectedRow == -1)
                    {
                        MessageBox.Show(this, "Nothing is selected to duplicate!");
                        return;
                    }
                    
                    var id = Guid.NewGuid().ToString();

                    var newDup = ScheduleRulesetAbridged.FromJson((gd.SelectedItem as ScheduleRulesetAbridged).ToJson());
                    newDup.Identifier = id;
                    newDup.DisplayName = string.IsNullOrEmpty( newDup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}": $"{newDup.DisplayName}_dup";
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
                        var d = gd.DataStore.Select(_=>_ as ScheduleRulesetAbridged).ToList();
                        d.Add(ToAbridged(model, dialog_rc));
                        gd.DataStore = d;

                    }
                };
                edit.Click += (s, e) =>
                {
                    if (gd.SelectedRow == -1)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }
                    var selected = gd.SelectedItem;
                    var newDup = ScheduleRulesetAbridged.FromJson((selected as ScheduleRulesetAbridged).ToJson());
                    var realObj = AbridgedToReal(newDup);
                    var dialog = new Honeybee.UI.Dialog_Schedule(realObj);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var index = gd.SelectedRow;
                        var newDataStore = gd.DataStore.Select(_ => _ as ScheduleRulesetAbridged).ToList();
                        newDataStore.RemoveAt(index);
                        newDataStore.Insert(index, ToAbridged(model, dialog_rc));
          
                        gd.DataStore = newDataStore;

                    }
                };
                remove.Click += (s, e) =>
                {
                    
                    if (gd.SelectedRow == -1)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }
                    if (gd.SelectedItem is ScheduleRulesetAbridged obj)
                    {
                        var index = gd.SelectedRow;
                        var res = MessageBox.Show(this, $"Are you sure you want to delete:\n {obj.DisplayName ?? obj.Identifier }", MessageBoxButtons.YesNo);
                        if (res == DialogResult.Yes)
                        {
                            var newDataStore = gd.DataStore.ToList();
                            newDataStore.RemoveAt(index);
                            gd.DataStore = newDataStore;
                        }
                    }
                  
                };

                gd.CellDoubleClick += (s, e) =>
                {
                    var doubleClk = e.Item;
                    if (doubleClk is HB.ScheduleRulesetAbridged obj)
                    {
                        var newDup = HB.ScheduleRulesetAbridged.FromJson(obj.ToJson());
                        var realObj = AbridgedToReal(newDup);
                        var dialog = new Honeybee.UI.Dialog_Schedule(realObj);
                        var dialog_rc = dialog.ShowModal(this);

                        if (dialog_rc != null)
                        {
                            var index = gd.SelectedRow;
                            var newDataStore = gd.DataStore.Select(_ => _ as ScheduleRulesetAbridged).ToList();
                            newDataStore.RemoveAt(index);
                            newDataStore.Insert(index, ToAbridged(model, dialog_rc));
                            gd.DataStore = newDataStore;


                        }
                    }

                };

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(gd.SelectedItem as HB.ScheduleRulesetAbridged);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                layout.AddSeparateRow(null);
                layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
                

                //Create layout
                Content = layout;


            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }

        private GridView GenGridView(IEnumerable<object> items)
        {
            var gd = new GridView() { DataStore = items };
            gd.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ScheduleRulesetAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });


            return gd;
        }





    }
}
