using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI
{

    public class ScheduleRulesetViewModel : ViewModelBase
    {
        #region Schedule Ruleset
        private ScheduleRuleset _schRuleset_hbObj;
        public ScheduleRuleset SchRuleset_hbObj
        {
            get => _schRuleset_hbObj;
            set
            {
                _schRuleset_hbObj = value;
                //reset lower/upper limit based on ScheduleTypeLimit
                _lowerLimit = -999;
                _upperLimit = 999;

                Set(() => _schRuleset_hbObj = value, nameof(SchRuleset_hbObj));
            }
        }

        public string DisplayName
        {
            get
            {
                _schRuleset_hbObj.DisplayName = _schRuleset_hbObj.DisplayName ?? _schRuleset_hbObj.Identifier;
                return _schRuleset_hbObj.DisplayName;
            }
            set => Set(() => _schRuleset_hbObj.DisplayName = value, nameof(DisplayName));
        }

        private ScheduleTypeLimit ScheduleTypeLimit => _schRuleset_hbObj.ScheduleTypeLimit ?? new ScheduleTypeLimit("Fractional") { LowerLimit = 0 };

        private double _lowerLimit = -999;
        public double LowerLimit
        {
            get
            {
                if (_lowerLimit != -999)
                    return _lowerLimit;

                // Find the lower value
                var lowValue = 0.0;

                if (ScheduleTypeLimit.LowerLimit?.Obj is double low)
                {
                    lowValue = low;
                }
                else
                {
                    // no limit
                    var min = Math.Floor(DaySchedules.SelectMany(_ => _.Values).Min());
                    // Do not change schedule type limit, NEVER! ScheduleTypeLimits should only be readable
                    //_hbObj.ScheduleTypeLimit.LowerLimit = min;
                    lowValue = min;
                }


                //Only for temperature -273.15, reset to 0
                if (ScheduleTypeLimit.UnitType == ScheduleUnitType.Temperature && lowValue == -273.15)
                {
                    lowValue = 0;
                }

                _lowerLimit = lowValue;
                return lowValue;

            }
            set => Set(() => _lowerLimit = value, nameof(LowerLimit));
        }

        private double _upperLimit = 999;
        public double UpperLimit
        {
            get
            {
                if (_upperLimit != 999)
                    return _upperLimit;

                // Find the lower value
                var upperLimit = 0.0;
                if (ScheduleTypeLimit.UpperLimit?.Obj is double v)
                {
                    upperLimit = v;
                }
                else
                {
                    // no limit
                    var max = Math.Ceiling(DaySchedules.SelectMany(_ => _.Values).Max());
                    // Do not change schedule type limit, NEVER! ScheduleTypeLimits should only be readable
                    //_hbObj.ScheduleTypeLimit.UpperLimit = max;
                    upperLimit = max;
                }

                _upperLimit = upperLimit;
                return upperLimit;

            }
            set => Set(() => _upperLimit = value, nameof(UpperLimit));
        }

        public double SchTypelength => Math.Abs(this.UpperLimit - this.LowerLimit);

        public List<ScheduleDay> DaySchedules
        {
            get
            {
                SchRuleset_hbObj.DaySchedules = SchRuleset_hbObj.DaySchedules ?? new List<ScheduleDay>();
                return SchRuleset_hbObj.DaySchedules;
            }
            set => Set(() => SchRuleset_hbObj.DaySchedules = value, nameof(DaySchedules));

        }
        public List<ScheduleRuleAbridged> ScheduleRules
        {
            get
            {
                SchRuleset_hbObj.ScheduleRules = SchRuleset_hbObj.ScheduleRules ?? new List<ScheduleRuleAbridged>() { };
                return SchRuleset_hbObj.ScheduleRules;
            }
            set => Set(() => SchRuleset_hbObj.ScheduleRules = value, nameof(ScheduleRules));

        }
        public ScheduleDay DefaultDaySchedule
        {
            get => DaySchedules.First(_ => _.Identifier == _schRuleset_hbObj.DefaultDaySchedule);
        }

        private Eto.Drawing.Color _currentColor;

        public Eto.Drawing.Color Currentolor
        {
            get => _currentColor;
            set => Set(() => _currentColor = value, nameof(Currentolor));
        }

        private ScheduleRuleAbridged _currentScheduleRule;

        public ScheduleRuleAbridged CurrentScheduleRule
        {
            get => _currentScheduleRule;
            set => Set(() => _currentScheduleRule = value, nameof(CurrentScheduleRule));
        }

        public ScheduleDay CurrentDaySchedule
        {
            get => DaySchedules.First(_ => _.Identifier == CurrentScheduleRule.ScheduleDay);
        }

        private int _intervals;

        public int Intervals
        {
            get => _intervals;
            set 
            {
                var interval = value;
                switch (value)
                {
                    case 60:
                        this.Interval_60 = false;
                        this.Interval_15 = true;
                        this.Interval_1 = true;
                        break;
                    case 15:
                        this.Interval_60 = true;
                        this.Interval_15 = false;
                        this.Interval_1 = true;
                        break;
                    case 1:
                        this.Interval_60 = true;
                        this.Interval_15 = true;
                        this.Interval_1 = false;
                        break;
                    default:
                        this.Interval_60 = false;
                        this.Interval_15 = true;
                        this.Interval_1 = true;
                        interval = 60;
                        break;
                }
                _intervals = interval;
            }
        }


        private bool _interval_60;

        public bool Interval_60
        {
            get => _interval_60;
            set => Set(() => _interval_60 = value, nameof(Interval_60));
        }
        private bool _interval_15 = true;

        public bool Interval_15
        {
            get => _interval_15;
            set => Set(() => _interval_15 = value, nameof(Interval_15));
        }
        private bool _interval_1 = true;

        public bool Interval_1
        {
            get => _interval_1;
            set => Set(() => _interval_1 = value, nameof(Interval_1));
        }

        public ICommand UpdateIntervalCommand => new RelayCommand<int>((interval) => {
            this.Intervals = interval;
        });
        #endregion

        #region Schedule Rule

        private ScheduleRuleAbridged _schRule_hbObj;
        public ScheduleRuleAbridged SchRule_hbObj
        {
            get
            {
                _schRule_hbObj = _schRule_hbObj ?? new ScheduleRuleAbridged("DefaultDayThatDoesnotHaveScheduleDay");
                return _schRule_hbObj;
            }
            set => Set(() => _schRule_hbObj = value, nameof(SchRule_hbObj));
        }
        public bool ApplySunday
        {
            get => SchRule_hbObj.ApplySunday;
            set => Set(() => SchRule_hbObj.ApplySunday = value, nameof(ApplySunday));
        }
        public bool ApplyMonday
        {
            get => SchRule_hbObj.ApplyMonday;
            set => Set(() => SchRule_hbObj.ApplyMonday = value, nameof(ApplyMonday));
        }
        public bool ApplyTuesday
        {
            get => SchRule_hbObj.ApplyTuesday;
            set => Set(() => SchRule_hbObj.ApplyTuesday = value, nameof(ApplyTuesday));
        }
        public bool ApplyThursday
        {
            get => SchRule_hbObj.ApplyThursday;
            set => Set(() => SchRule_hbObj.ApplyThursday = value, nameof(ApplyThursday));
        }
        public bool ApplyWednesday
        {
            get => SchRule_hbObj.ApplyWednesday;
            set => Set(() => SchRule_hbObj.ApplyWednesday = value, nameof(ApplyWednesday));
        }
        public bool ApplyFriday
        {
            get => SchRule_hbObj.ApplyFriday;
            set => Set(() => SchRule_hbObj.ApplyFriday = value, nameof(ApplyFriday));
        }
        public bool ApplySaturday
        {
            get => SchRule_hbObj.ApplySaturday;
            set => Set(() => SchRule_hbObj.ApplySaturday = value, nameof(ApplySaturday));
        }

        public DateTime StartDate
        {
            get
            {
                SchRule_hbObj.StartDate = _schRule_hbObj.StartDate ?? new List<int> { 1, 1 };
                return new DateTime(2017, SchRule_hbObj.StartDate[0], SchRule_hbObj.StartDate[1]);
            }
            set => Set(() => SchRule_hbObj.StartDate = new List<int> { value.Month, value.Day }, nameof(StartDate));
        }
        public DateTime EndDate
        {
            get
            {
                SchRule_hbObj.EndDate = SchRule_hbObj.EndDate ?? new List<int> { 12, 31 };
                return new DateTime(2017, SchRule_hbObj.EndDate[0], SchRule_hbObj.EndDate[1]);
            }
            set => Set(() => _schRule_hbObj.EndDate = new List<int> { value.Month, value.Day }, nameof(EndDate));
        }
        #endregion

        #region Schedule Day

        private ScheduleDay _SchDay_hbObj;
        public ScheduleDay SchDay_hbObj
        {
            get => _SchDay_hbObj;
            set => Set(() => _SchDay_hbObj = value, nameof(SchDay_hbObj));
        }

        public string SchDayName
        {
            get
            {
                _SchDay_hbObj.DisplayName = _SchDay_hbObj.DisplayName ?? _SchDay_hbObj.Identifier;
                return _SchDay_hbObj.DisplayName;
            }
            set => Set(() => _SchDay_hbObj.DisplayName = value, nameof(SchDayName));
        }

        public List<double> SchDayValues
        {
            get => SchDay_hbObj.Values;
            set => Set(() => SchDay_hbObj.Values = value, nameof(SchDayValues));
        }


        public List<List<int>> SchDayTimes
        {
            get => _SchDay_hbObj.Times;
            set => Set(() => SchDay_hbObj.Times = value, nameof(SchDayTimes));
        }
        #endregion


        public Eto.Drawing.Color DefaultRuleColor { get; set; } = Eto.Drawing.Color.FromArgb(184, 229, 255);
        public List<Eto.Drawing.Color> RuleColors { get; set; } 
            = new List<Eto.Drawing.Color>() 
            { 
               Eto.Drawing.Color.FromArgb(153, 57, 142),
               Eto.Drawing.Color.FromArgb(95, 87, 193),
               Eto.Drawing.Color.FromArgb(51, 145, 80),
               Eto.Drawing.Color.FromArgb(175, 155, 70),
               Eto.Drawing.Color.FromArgb(237, 217, 131),
               Eto.Drawing.Color.FromArgb(216, 119, 141),
               Eto.Drawing.Color.FromArgb(204, 101, 43),
               Eto.Drawing.Color.FromArgb(68, 170, 153),
               Eto.Drawing.Color.FromArgb(102, 153, 204),
               Eto.Drawing.Color.FromArgb(145, 29, 10),
               Eto.Drawing.Color.FromArgb(135, 188, 66),
               Eto.Drawing.Color.FromArgb(112, 111, 112)

            };

        public ScheduleRulesetViewModel(ScheduleRuleset scheduleRuleset)
        {
            this.SchRuleset_hbObj = scheduleRuleset;
            this.SchDay_hbObj = this.DefaultDaySchedule;
            this.Intervals = 60;
            this.Currentolor = this.DefaultRuleColor;
        }

    }



}
