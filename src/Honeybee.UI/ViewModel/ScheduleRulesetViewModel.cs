using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{

    public class ScheduleRulesetViewModel : ViewModelBase
    {
        private ScheduleRuleset _hbObj;
        public ScheduleRuleset hbObj
        {
            get => _hbObj;
            set {
                _hbObj = value;
                //reset lower/upper limit based on ScheduleTypeLimit
                _lowerLimit = -999;
                _upperLimit = 999;
                var props = this.GetType().GetProperties().Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }

        public string DisplayName
        {
            get
            {
                _hbObj.DisplayName = _hbObj.DisplayName ?? _hbObj.Identifier;
                return _hbObj.DisplayName;
            }
            set => Set(() => _hbObj.DisplayName = value, nameof(DisplayName));
        }

        private ScheduleTypeLimit ScheduleTypeLimit => _hbObj.ScheduleTypeLimit ?? new ScheduleTypeLimit("Fractional") { LowerLimit = 0 };

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
                    upperLimit =  max;
                }

                _upperLimit = upperLimit;
                return upperLimit;

            }
            set => Set(() => _upperLimit = value, nameof(UpperLimit));
        }
       


        public List<ScheduleDay> DaySchedules
        {
            get
            {
                _hbObj.DaySchedules = _hbObj.DaySchedules ?? new List<ScheduleDay>();
                return _hbObj.DaySchedules;
            }
            set => Set(() => _hbObj.DaySchedules = value, nameof(DaySchedules));

        }
        public List<ScheduleRuleAbridged> ScheduleRules
        {
            get
            {
                _hbObj.ScheduleRules = _hbObj.ScheduleRules ?? new List<ScheduleRuleAbridged>() { new ScheduleRuleAbridged("") };
                return _hbObj.ScheduleRules;
            }
            set => Set(() => _hbObj.ScheduleRules = value, nameof(ScheduleRules));

        }
        public ScheduleDay DefaultDaySchedule
        {
            get => DaySchedules.First(_ => _.Identifier == _hbObj.DefaultDaySchedule);
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

      

        private static readonly ScheduleRulesetViewModel _instance = new ScheduleRulesetViewModel();
        public static ScheduleRulesetViewModel Instance => _instance;


        private ScheduleRulesetViewModel()
        {
        }

    }



}
