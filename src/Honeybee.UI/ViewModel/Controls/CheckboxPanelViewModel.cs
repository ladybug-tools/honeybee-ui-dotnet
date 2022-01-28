using Eto.Forms;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class CheckboxPanelViewModel : CheckboxPanelViewModel<IIDdBase>
    {
        public CheckboxPanelViewModel(ModelProperties libSource, Action<IIDdBase> setAction):base(libSource, setAction)
        {
        }

    }

    public class CheckboxPanelViewModel<T> : ViewModelBase 
        where T: IHoneybeeObject
    {
        private T _refObjProperty;
        protected T refObjProperty
        {
            get => _refObjProperty;
            set
            {
                _refObjProperty = value;
                SetHBProperty(value);
            }
        }
        public Action<T> SetHBProperty { get; private set; }


        private bool _isPanelEnabled = true;

        public bool IsPanelEnabled
        {
            get => _isPanelEnabled;
            set { this.Set(() => _isPanelEnabled = value, nameof(IsPanelEnabled)); }
        }

        private bool _isCheckboxChecked;

        public bool IsCheckboxChecked
        {
            get => _isCheckboxChecked;

            set
            {
                this.Set(() => _isCheckboxChecked = value, nameof(IsCheckboxChecked));
                IsPanelEnabled = !value;
                if (_isCheckboxChecked)
                    SetHBProperty(default(T));
                else
                {
                    SetHBProperty(_refObjProperty);
                }
            }
        }

        internal ModelProperties _libSource { get; set; }

        public CheckboxPanelViewModel(ModelProperties libSource, Action<T> setAction)
        {
            this._libSource = libSource;
            this.SetHBProperty = setAction;
        }


        public void SetPropetyObj(T obj)
        {
            this.refObjProperty = obj;
        }

    }

}
