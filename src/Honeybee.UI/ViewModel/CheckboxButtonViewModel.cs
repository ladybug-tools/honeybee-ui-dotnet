﻿using Eto.Forms;
using System;

namespace Honeybee.UI
{

    public class CheckboxButtonViewModel : ViewModelBase
    {
        
        public bool IsVaries => _isVaries && !_isCheckboxChecked;
        private bool _isVaries;
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
        public Action<HoneybeeSchema.IIDdBase> SetHBProperty { get; protected set; }


        private string _btnName = None;
        public string BtnName
        {
            get => _isVaries ? this.Varies : _btnName;
            protected set
            {
                IsCheckboxChecked = refObjProperty == null;
                IsBtnEnabled = !IsCheckboxChecked;

                _isVaries = value == this.Varies;
                if (_isVaries)
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
            protected set { this.Set(() => _isBtnEnabled = value, nameof(IsBtnEnabled)); }
        }

        private bool _isCheckboxChecked;

        public bool IsCheckboxChecked
        {
            get => _isCheckboxChecked;

            protected set
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
