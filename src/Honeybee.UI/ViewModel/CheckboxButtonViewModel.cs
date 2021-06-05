using Eto.Forms;
using System;

namespace Honeybee.UI
{
   
    public class CheckboxButtonViewModel : ViewModelBase
    {
        private string Varies => "<varies>";
        private static string None => "<None>";
        public bool IsVaries { get; private set; }
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


        private string _btnName = None;
        public string BtnName
        {
            get => IsVaries ? this.Varies : _btnName;
            private set
            {
                IsCheckboxChecked = refObjProperty == null;
                IsBtnEnabled = !IsCheckboxChecked;

                IsVaries = value == this.Varies;
                if (IsVaries)
                    IsCheckboxChecked = false;

                if (string.IsNullOrEmpty(value))
                    value = None;

                if (IsBtnEnabled || string.IsNullOrEmpty(_btnName))
                    this.Set(() => _btnName = value, nameof(BtnName));
            }
        }

        private bool _isBtnEnabled;

        public bool IsBtnEnabled
        {
            get => _isBtnEnabled;
            private set { this.Set(() => _isBtnEnabled = value, nameof(IsBtnEnabled)); }
        }

        private bool _isCheckboxChecked;

        public bool IsCheckboxChecked
        {
            get => _isCheckboxChecked;

            private set
            {
                this.Set(() => _isCheckboxChecked = value, nameof(IsCheckboxChecked));
                IsBtnEnabled = !value;
                if (_isCheckboxChecked)
                    SetHBProperty(null);
                else
                {
                    SetHBProperty(_refObjProperty);
                }
            }
        }


        public CheckboxButtonViewModel(Action<HoneybeeSchema.IIDdBase> setAction)
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

}
