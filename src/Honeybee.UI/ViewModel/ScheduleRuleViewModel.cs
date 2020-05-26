using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{

    public class ScheduleRuleViewModel : ViewModelBase
    {
        private ScheduleRuleset _scheduleRuleset;

        private ScheduleRuleAbridged _hbObj;
        public ScheduleRuleAbridged hbObj
        {
            get 
            {
                if (_hbObj == null)
                {
                    _hbObj = new ScheduleRuleAbridged("DefaultDayThatDoesnotHaveScheduleDay");
                }
                return _hbObj;
            }
            set
            {
                _hbObj = value;
                var props = this.GetType().GetProperties().Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }
        public bool ApplySunday
        {
            get => hbObj.ApplySunday;
            set => Set(() => hbObj.ApplySunday = value, nameof(ApplySunday));
        }
        public bool ApplyMonday
        {
            get => hbObj.ApplyMonday;
            set => Set(() => hbObj.ApplyMonday = value, nameof(ApplyMonday));
        }
        public bool ApplyTuesday
        {
            get => hbObj.ApplyTuesday;
            set => Set(() => hbObj.ApplyTuesday = value, nameof(ApplyTuesday));
        }
        public bool ApplyThursday
        {
            get => hbObj.ApplyThursday;
            set => Set(() => hbObj.ApplyThursday = value, nameof(ApplyThursday));
        }
        public bool ApplyWednesday
        {
            get => hbObj.ApplyWednesday;
            set => Set(() => hbObj.ApplyWednesday = value, nameof(ApplyWednesday));
        }
        public bool ApplyFriday
        {
            get => hbObj.ApplyFriday;
            set => Set(() => hbObj.ApplyFriday = value, nameof(ApplyFriday));
        }
        public bool ApplySaturday
        {
            get => hbObj.ApplySaturday;
            set => Set(() => hbObj.ApplySaturday = value, nameof(ApplySaturday));
        }

        public DateTime StartDate
        {
            get 
            {
                //if (_hbObj == null)
                //{
                //    // This means it is default scheduleDay selected, which has no coupled scheduleRule
                //    // Applies to the full year.
                //    return new DateTime(2017, 1, 1);
                //}
                hbObj.StartDate = _hbObj.StartDate ?? new List<int> { 1, 1 };
                return new DateTime(2017, hbObj.StartDate[0], hbObj.StartDate[1]);
            } 
            set => Set(() => hbObj.StartDate = new List<int> { value.Month, value.Day }, nameof(StartDate));
        }
        public DateTime EndDate
        {
            get
            {
                //if (_hbObj == null)
                //{
                //    // This means it is default scheduleDay selected, which has no coupled scheduleRule
                //    // Applies to the full year.
                //    return new DateTime(2017, 12, 31);
                //}
                hbObj.EndDate = hbObj.EndDate ?? new List<int> { 12, 31 };
                return new DateTime(2017, hbObj.EndDate[0], hbObj.EndDate[1]);
            }
            set => Set(() => _hbObj.EndDate = new List<int> { value.Month, value.Day }, nameof(EndDate));
        }


        public ScheduleDay ScheduleDay
        {
            get => _scheduleRuleset.DaySchedules.First(_ => _.Identifier == _hbObj.ScheduleDay);
        }


        private static readonly ScheduleRuleViewModel _instance = new ScheduleRuleViewModel();
        public static ScheduleRuleViewModel Instance => _instance;



        private ScheduleRuleViewModel()
        {
        }

    }



}
