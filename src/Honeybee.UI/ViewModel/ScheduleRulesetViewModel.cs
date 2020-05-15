using HoneybeeSchema;
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

        public ScheduleTypeLimit ScheduleTypeLimit
        {
            get => _hbObj.ScheduleTypeLimit;
            set => Set(() => _hbObj.ScheduleTypeLimit = value, nameof(ScheduleTypeLimit));
        }
        public double LowerLimit
        {
            get
            {
                if (_hbObj.ScheduleTypeLimit.LowerLimit.Obj is double low)
                {
                    return low;
                }
                return 0;

            } 
            set => Set(() => _hbObj.ScheduleTypeLimit.LowerLimit = value, nameof(LowerLimit));
        }
        public double UpperLimit
        {
            get
            {
                if (_hbObj.ScheduleTypeLimit.UpperLimit.Obj is double low)
                {
                    return low;
                }
                return 1;

            }
            set => Set(() => _hbObj.ScheduleTypeLimit.UpperLimit = value, nameof(UpperLimit));
        }
       


        public List<ScheduleDay> DaySchedules
        {
            get
            {
                if (_hbObj.DaySchedules == null)
                {
                    _hbObj.DaySchedules = new List<ScheduleDay>();
                }
                return _hbObj.DaySchedules;
            }
            set => Set(() => _hbObj.DaySchedules = value, nameof(DaySchedules));

        }
        public List<ScheduleRuleAbridged> ScheduleRules
        {
            get
            {
                if (_hbObj.ScheduleRules == null)
                {
                    _hbObj.ScheduleRules = new List<ScheduleRuleAbridged>() { new ScheduleRuleAbridged("") };
                }
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
            get
            {
                return _currentScheduleRule;
            }
            set => Set(() => _currentScheduleRule = value, nameof(CurrentScheduleRule));

        }

        public ScheduleDay CurrentDaySchedule
        {
            get => DaySchedules.First(_ => _.Identifier == CurrentScheduleRule.ScheduleDay);
        }

      
        private static ScheduleRulesetViewModel _instance;
        public static ScheduleRulesetViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScheduleRulesetViewModel();
                }
                return _instance;
            }
        }


        private ScheduleRulesetViewModel()
        {
        }

    }



}
