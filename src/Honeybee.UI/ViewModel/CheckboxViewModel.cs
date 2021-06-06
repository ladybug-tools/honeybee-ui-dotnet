using System;

namespace Honeybee.UI
{
    public class CheckboxViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        public bool IsVaries => !IsChecked.HasValue;
        public Action<bool> SetHBProperty { get; private set; }


        private bool? _isChecked;

        public bool? IsChecked
        {
            get => _isChecked;
            private set {

                if (value.HasValue)
                    SetHBProperty?.Invoke(value.Value);

                this.Set(() => _isChecked = value, nameof(IsChecked));
            }
        }


        public CheckboxViewModel(Action<bool> setAction)
        {
            this.SetHBProperty = setAction;
        }

        public void SetCheckboxVaries()
        {
            this.IsChecked = null;
        }

        public void SetCheckboxChecked(bool isChecked)
        {
            this.IsChecked = isChecked;
        }
    }

}
