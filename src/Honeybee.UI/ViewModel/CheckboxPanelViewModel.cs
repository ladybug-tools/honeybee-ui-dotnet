using Eto.Forms;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class CheckboxPanelViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        //public bool IsVaries { get; private set; }
        private HoneybeeSchema.IIDdBase _refObjProperty;
        protected HoneybeeSchema.IIDdBase refObjProperty
        {
            get => _refObjProperty;
            set
            {
                _refObjProperty = value;
                SetHBProperty(value);
            }
        }
        public Action<HoneybeeSchema.IIDdBase> SetHBProperty { get; private set; }


        private bool _isPanelEnabled;

        public bool IsPanelEnabled
        {
            get => _isPanelEnabled;
            private set { this.Set(() => _isPanelEnabled = value, nameof(IsPanelEnabled)); }
        }

        private bool _isCheckboxChecked;

        public bool IsCheckboxChecked
        {
            get => _isCheckboxChecked;

            protected set
            {
                this.Set(() => _isCheckboxChecked = value, nameof(IsCheckboxChecked));
                IsPanelEnabled = !value;
                if (_isCheckboxChecked)
                    SetHBProperty(null);
                else
                {
                    SetHBProperty(_refObjProperty);
                }
            }
        }

        internal ModelProperties _libSource { get; set; }

        public CheckboxPanelViewModel(ModelProperties libSource, Action<HoneybeeSchema.IIDdBase> setAction)
        {
            this._libSource = libSource;
            this.SetHBProperty = setAction;
        }


        public void SetPropetyObj(HoneybeeSchema.IIDdBase obj)
        {
            this.refObjProperty = obj;
        }

    }


}
