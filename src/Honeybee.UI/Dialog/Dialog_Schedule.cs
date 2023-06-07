using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Honeybee.UI
{
    public class Dialog_Schedule : Dialog_ResourceEditor<ScheduleRuleset>
    {
        
        public Dialog_Schedule(ScheduleRuleset scheduleRuleset, bool lockedMode = false)
        {
            Padding = new Padding(10);
            Title = $"Schedule - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 1110;
            this.Icon = DialogHelper.HoneybeeIcon;

            var schDup = scheduleRuleset.DuplicateScheduleRuleset();
            // check units
            schDup = CheckUnit(schDup, toDisplayUnit: true);

            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OkButton = new Button { Text = "OK", Enabled = !lockedMode };
            OkButton.Click += (sender, e) =>
            {
                try
                {
                    var sch = CheckUnit(schDup, toDisplayUnit: false);
                    OkCommand.Execute(sch);
                }
                catch (Exception ex)
                {
                    Dialog_Message.Show(this, ex);
                }
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            var sche = new Panel_Schedule(schDup);
            var panel = new DynamicLayout() { DefaultSpacing = new Size(5, 5) };
            panel.AddRow(sche);
            panel.AddSeparateRow(locked, OkButton, AbortButton, null);
            Content = panel;
        }

        private static ScheduleRuleset CheckUnit(ScheduleRuleset schedule, bool toDisplayUnit)
        {
            var typeLimit = schedule.ScheduleTypeLimit;
            if (typeLimit == null) return schedule;

            var hasUnits = _mapUnits.TryGetValue(typeLimit.UnitType, out var units);
            if (!hasUnits) return schedule;
            if (units.displayUnit == units.baseUnit)
            {
                return schedule;
            }

            var baseUnit = units.baseUnit.ToUnitsNetEnum();
            var displayUnit = units.displayUnit.ToUnitsNetEnum();
            Func<double, double> converter = (num) 
                => toDisplayUnit ? 
                Units.ConvertValueWithUnits(num, baseUnit, displayUnit):
                Units.ConvertValueWithUnits(num, displayUnit, baseUnit);

            // convert type limit
            if (typeLimit.LowerLimit != null && typeLimit.LowerLimit.Obj is double ld)
            {
                schedule.ScheduleTypeLimit.LowerLimit = converter(ld);
            }
            if (typeLimit.UpperLimit != null && typeLimit.UpperLimit.Obj is double ud)
            {
                schedule.ScheduleTypeLimit.UpperLimit = converter(ud);
            }

            // convert schedule values
            schedule.DaySchedules = schedule.DaySchedules.Select(s =>
            {
                var schDay = s.DuplicateScheduleDay();
                schDay.Values = schDay.Values.Select(v => converter(v)).ToList();
                return schDay;

            }).ToList();
            
            return schedule;
        }

        private static Dictionary<ScheduleUnitType, (Enum baseUnit, Enum displayUnit)> _mapUnits => new Dictionary<ScheduleUnitType, (Enum, Enum)>()
        {
            { ScheduleUnitType.Temperature, (Units.TemperatureUnit.DegreeCelsius, Units.CustomUnitSettings[Units.UnitType.Temperature])},
            { ScheduleUnitType.DeltaTemperature, (Units.TemperatureDeltaUnit.DegreeCelsius, Units.CustomUnitSettings[Units.UnitType.TemperatureDelta])},
            //{ ScheduleUnitType.Power, (Units.PowerUnit.Watt, Units.CustomUnitSettings[Units.UnitType.Power])}
            
        };


    }
}
