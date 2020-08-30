using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class ShadeViewModel : ViewModelBase
    {
        private Shade _hbObj;
        public Shade HoneybeeObject
        {
            get { return _hbObj; }
            private set
            {
                if (_hbObj != value)
                {
                    _hbObj = value;
                    OnPropertyChanged();
                }
            }
        }

        public Action<string> ActionWhenChanged { get; set; }
       

        public ShadeViewModel()
        {
        }

        public void Update(Shade honeybeeObj, Action<string> actionWhenChanged)
        {
            HoneybeeObject = honeybeeObj;
            ActionWhenChanged = actionWhenChanged ?? delegate (string m) { };
        }
        public ICommand ShadeEnergyPropertyBtnClick => new RelayCommand(() => {
            var energyProp = this.HoneybeeObject.Properties.Energy ?? new ShadeEnergyPropertiesAbridged();
            energyProp = energyProp.DuplicateShadeEnergyPropertiesAbridged();
            var dialog = new Dialog_ShadeEnergyProperty(energyProp);
            var dialog_rc = dialog.ShowModal(Helper.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Energy = dialog_rc;
                this.ActionWhenChanged($"Set {this.HoneybeeObject.Identifier} Energy Properties ");
            }
        });

        public ICommand ShadeRadiancePropertyBtnClick => new RelayCommand(() => {
            var energyProp = this.HoneybeeObject.Properties.Radiance ?? new ShadeRadiancePropertiesAbridged();
            energyProp = energyProp.DuplicateShadeRadiancePropertiesAbridged();
            var dialog = new Dialog_ShadeRadianceProperty(energyProp);
            var dialog_rc = dialog.ShowModal(Helper.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Radiance = dialog_rc;
                this.ActionWhenChanged($"Set {this.HoneybeeObject.Identifier} Radiance Properties ");
            }
        });
    }



}
