using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class ShadeViewModel : ViewModelBase
    {
        private Shade _hbObj;
        public Shade HoneybeeObject
        {
            get { return _hbObj; }
            private set { this.Set(() => _hbObj = value, nameof(HoneybeeObject)); }
        }

        public Action<string> ActionWhenChanged { get; private set; }

        public ModelProperties ModelProperties { get; set; }
        public ShadeViewModel(ModelProperties libSource)
        {
            this.ModelProperties = libSource;
        }

        public void Update(Shade honeybeeObj, Action<string> actionWhenChanged)
        {
            ActionWhenChanged = actionWhenChanged;
            HoneybeeObject = honeybeeObj;
        }
        public ICommand ShadeEnergyPropertyBtnClick => new RelayCommand(() => {
            var energyProp = this.HoneybeeObject.Properties.Energy ?? new ShadeEnergyPropertiesAbridged();
            energyProp = energyProp.DuplicateShadeEnergyPropertiesAbridged();
            var dialog = new Dialog_ShadeEnergyProperty(this.ModelProperties.Energy, energyProp);
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
            var dialog = new Dialog_ShadeRadianceProperty(this.ModelProperties.Radiance, energyProp);
            var dialog_rc = dialog.ShowModal(Helper.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Radiance = dialog_rc;
                this.ActionWhenChanged($"Set {this.HoneybeeObject.Identifier} Radiance Properties ");
            }
        });
    }



}
