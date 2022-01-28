﻿using Eto.Forms;
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
                    BtnName = (point == null || !point.Any()) ? ReservedText.None : $"{string.Join(",", point)}";
                else
                    BtnName = value.GetType().Name;

            }
        }
        public Action<T> SetHBProperty { get; private set; }


        private string _btnName;
        public string BtnName
        {
            get => IsVaries ? ReservedText.Varies : _btnName;
            set
            {
                IsVaries = value == ReservedText.Varies;
                if (string.IsNullOrEmpty(value))
                    value = ReservedText.None;
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

}