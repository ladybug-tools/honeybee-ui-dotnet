using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{

    public class ScheduleDayViewModel : ViewModelBase
    {
        private ScheduleDay _hbObj;
        public ScheduleDay hbObj
        {
            get => _hbObj;
            set
            {
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

        public List<double> Values
        {
            get => _hbObj.Values;
            set => Set(() => _hbObj.Values = value, nameof(Values));
        }

        

        public List<List<int>> Times
        {
            get => _hbObj.Times;
            set => Set(() => hbObj.Times = value, nameof(Times));
        }

        private static ScheduleDayViewModel _instance;
        public static ScheduleDayViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScheduleDayViewModel();
                }
                return _instance;
            }
        }


        private ScheduleDayViewModel()
        {
        }

    }



}
