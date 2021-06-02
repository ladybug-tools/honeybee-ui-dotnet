using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class ButtonViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        public bool IsVaries;
        private HoneybeeSchema.IIDdBase _refObjProperty;
        private HoneybeeSchema.IIDdBase refObjProperty
        {
            get => _refObjProperty;
            set
            {
                _refObjProperty = value;
                SetHBProperty(value);
                BtnName = value?.DisplayName ?? value?.Identifier;
            }
        }
        public Action<HoneybeeSchema.IIDdBase> SetHBProperty { get; private set; }


        private string _btnName;
        public string BtnName
        {
            get => IsVaries ? this.Varies : _btnName;
            private set
            {
                IsVaries = value == this.Varies;
                this.Set(() => _btnName = value, nameof(BtnName));
            }
        }

      
        public ButtonViewModel(Action<HoneybeeSchema.IIDdBase> setAction)
        {
            this.SetHBProperty = setAction;
        }

        public void SetBtnName(string name)
        {
            this.BtnName = name;
        }

        public void SetPropetyObj(HoneybeeSchema.IIDdBase obj)
        {
            this.refObjProperty = obj;
        }
    }
    public class DoubleViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        public bool IsVaries;
        public Action<double> SetHBProperty { get; private set; }


        private string _numberText;
        public string NumberText
        {
            get => _numberText;
            private set
            {
                IsVaries = value == this.Varies;
                if (IsVaries)
                {
                    this.Set(() => _numberText = value, nameof(NumberText));
                }
                else if (double.TryParse(value, out var number))
                {
                    SetHBProperty?.Invoke(number);
                    this.Set(() => _numberText = number.ToString(), nameof(NumberText));
                }
            }
        }


        public DoubleViewModel(Action<double> setAction)
        {
            this.SetHBProperty = setAction;
        }

        public void SetNumberText(string name)
        {
            this.NumberText = name;
        }
        //public void SetNumber(double? number)
        //{
        //    this.NumberText = number?.ToString();
        //}
    }

}
