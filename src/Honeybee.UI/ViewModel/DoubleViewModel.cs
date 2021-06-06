using System;

namespace Honeybee.UI
{
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
