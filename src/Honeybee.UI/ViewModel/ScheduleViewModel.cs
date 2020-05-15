using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{

    public class ScheduleViewModel : ViewModelBase
    {
        private ScheduleRulesetAbridged _hbObj;
        public ScheduleRulesetAbridged hbObj
        {
            get => _hbObj;
            set => Set(() => _hbObj = value, nameof(hbObj));
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
        private ScheduleRuleAbridged _currentScheduleRule;

        public ScheduleRuleAbridged CurrentScheduleRule
        {
            get
            {
                return _currentScheduleRule;
            }
            set => Set(() => _currentScheduleRule = value, nameof(CurrentScheduleRule));

        }
        private (bool SU, bool M, bool TU, bool W, bool TH, bool F, bool SA) _currentApplyWeekDays;
        public (bool SU, bool M, bool TU, bool W, bool TH, bool F, bool SA) CurrentApplyWeekDays
        {
            get
            {
                var c = CurrentScheduleRule;
                if (c == null)
                {
                    return (false, false, false, false, false, false, false);
                }
                return (c.ApplySunday, c.ApplyMonday, c.ApplyTuesday, c.ApplyWednesday, c.ApplyThursday, c.ApplyFriday, c.ApplySaturday);
            }
            set => Set(() => _currentApplyWeekDays = value, nameof(CurrentApplyWeekDays));
        }


        private static ScheduleViewModel _instance;
        public static ScheduleViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScheduleViewModel();
                }
                return _instance;
            }
        }


        private ScheduleViewModel()
        {
        }

    }



}
