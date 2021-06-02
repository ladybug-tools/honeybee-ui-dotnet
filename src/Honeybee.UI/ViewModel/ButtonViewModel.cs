using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class ButtonViewModel: ButtonViewModel<HoneybeeSchema.IIDdBase>
    {
        public ButtonViewModel(Action<HoneybeeSchema.IIDdBase> setAction): base(setAction)
        {
        }
    }

    public class ButtonViewModel<T> : ViewModelBase
    {
        public string Varies => "<varies>";
        public bool IsVaries;
        private T _refObjProperty;
        private T refObjProperty
        {
            get => _refObjProperty;
            set
            {
                _refObjProperty = value;
                SetHBProperty(value);

                if (value == null)
                {
                    BtnName = null;
                    return;
                }

                if (value is HoneybeeSchema.IIDdBase idd)
                    BtnName = idd?.DisplayName ?? idd?.Identifier;
                else if (value is List<double> point)
                    BtnName = $"({string.Join(",", point.Take(3))})";
                else
                    BtnName = value.GetType().Name;

            }
        }
        public Action<T> SetHBProperty { get; private set; }


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

      
        public ButtonViewModel(Action<T> setAction)
        {
            this.SetHBProperty = setAction;
        }

        public void SetBtnName(string name)
        {
            this.BtnName = name;
        }

        public void SetPropetyObj(T obj)
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
