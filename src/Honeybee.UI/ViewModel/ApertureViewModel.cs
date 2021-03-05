using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class ApertureViewModel : ViewModelBase
    {
        private Aperture _hbObj;
        public Aperture HoneybeeObject
        {
            get { return _hbObj; }
            private set { this.Set(() => _hbObj = value, nameof(HoneybeeObject)); }
        }

        public List<AnyOf<Outdoors, Surface>> Bcs =>
            new List<AnyOf<Outdoors, Surface>>()
            {
                new Outdoors(), new Surface(new List<string>())
            };

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value == -1)
                    throw new Exception("selected index set to -1");

                this.Set(() => _selectedIndex = value, nameof(SelectedIndex));

                if (this.HoneybeeObject.BoundaryCondition.Obj.GetType().Name != Bcs[value].Obj.GetType().Name)
                {
                    //MessageBox.Show(Bcs[value]);
                    this.HoneybeeObject.BoundaryCondition = Bcs[value];
                    this.ActionWhenChanged("Set boundary condition");
                    
                }
                
            }
        }


        private bool _isOutdoor = true;
        public bool IsOutdoor
        {
            get { return _isOutdoor; }
            set { this.Set(() => _isOutdoor = value, nameof(IsOutdoor)); }

        }

        public Action<string> ActionWhenChanged { get; private set; }
        public ModelProperties ModelProperties { get; set; }

        internal ApertureViewModel()
        {
        }

        public void Update(ModelProperties libSource, Aperture honeybeeObj, Action<string> actionWhenChanged)
        {
            this.ModelProperties = libSource;

            ActionWhenChanged = actionWhenChanged;
            //HoneybeeObject.DisplayName = honeybeeObj.DisplayName ?? string.Empty;

            HoneybeeObject = honeybeeObj;
            IsOutdoor = honeybeeObj.BoundaryCondition.Obj is Outdoors;
            SelectedIndex = Bcs.FindIndex(_ => _.Obj.GetType().Name == this.HoneybeeObject.BoundaryCondition.Obj.GetType().Name);
            
            
        }

        public ICommand ApertureEnergyPropertyBtnClick => new RelayCommand(() => {
            var energyProp = this.HoneybeeObject.Properties.Energy ?? new ApertureEnergyPropertiesAbridged();
            energyProp = energyProp.DuplicateApertureEnergyPropertiesAbridged();
            var dialog = new Dialog_ApertureEnergyProperty(this.ModelProperties.Energy, energyProp);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Energy = dialog_rc;
                this.ActionWhenChanged($"Set {this.HoneybeeObject.Identifier} Energy Properties ");
            }
        });

        public ICommand ApertureRadiancePropertyBtnClick => new RelayCommand(() => {
            var energyProp = this.HoneybeeObject.Properties.Radiance ?? new ApertureRadiancePropertiesAbridged();
            energyProp = energyProp.DuplicateApertureRadiancePropertiesAbridged();
            var dialog = new Dialog_ApertureRadianceProperty(this.ModelProperties.Radiance, energyProp);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Radiance = dialog_rc;
                this.ActionWhenChanged($"Set {this.HoneybeeObject.Identifier} Radiance Properties ");
            }
        });

        public ICommand EditBoundaryConditionBtnClick => new RelayCommand(() => {
            if (this.HoneybeeObject.BoundaryCondition.Obj is Outdoors outdoors)
            {
                var od = outdoors.DuplicateOutdoors();
                var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                var dialog_rc = dialog.ShowModal(Config.Owner);
                if (dialog_rc != null)
                {
                    this.HoneybeeObject.BoundaryCondition = dialog_rc;
                    this.ActionWhenChanged($"Set Aperture Boundary Condition");
                }
            }
            else
            {
                MessageBox.Show(Config.Owner, "Only Outdoors type has additional properties to edit!");
            }
        });
    }



}
