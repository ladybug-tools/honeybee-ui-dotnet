using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    [Obsolete("This is deprecated, please use FacePropertyViewModel instead", true)]
    public class FaceViewModel : ViewModelBase
    {
        private Face _hbObj;
        public Face HoneybeeObject
        {
            get { return _hbObj; }
            private set { this.Set(() => _hbObj = value, nameof(HoneybeeObject)); }
        }

        private string _apertureCount = "0";
        public string ApertureCount
        {
            get { return _apertureCount.ToString(); }
            private set { this.Set(() => _apertureCount = value, nameof(ApertureCount)); }
        }

        public List<AnyOf<Ground, Outdoors, Adiabatic, Surface>> Bcs =>
            new List<AnyOf<Ground, Outdoors, Adiabatic, Surface>>()
            {
                new Ground(), new Outdoors(), new Adiabatic(), new Surface(new List<string>())
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
                    this.ActionWhenChanged?.Invoke("Set boundary condition");
                    
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
        internal FaceViewModel()
        {
        }

        public void Update(ModelProperties libSource, Face honeybeeObj, Action<string> actionWhenChanged)
        {
            this.ModelProperties = libSource;
            ActionWhenChanged = actionWhenChanged;

            HoneybeeObject = honeybeeObj;
            HoneybeeObject.Apertures = HoneybeeObject.Apertures ?? new List<Aperture>();
            HoneybeeObject.Doors = HoneybeeObject.Doors ?? new List<Door>();
            HoneybeeObject.IndoorShades = HoneybeeObject.IndoorShades ?? new List<Shade>();
            HoneybeeObject.OutdoorShades = HoneybeeObject.OutdoorShades ?? new List<Shade>();

            //HoneybeeObject.DisplayName = honeybeeObj.DisplayName ?? string.Empty;
            ApertureCount = honeybeeObj.Apertures?.Count.ToString();
            IsOutdoor = honeybeeObj.BoundaryCondition.Obj is Outdoors;
            //BC = new Outdoors();
            //BC = honeybeeObj.BoundaryCondition.Obj.GetType().Name;
            SelectedIndex = Bcs.FindIndex(_ => _.Obj.GetType().Name == this.HoneybeeObject.BoundaryCondition.Obj.GetType().Name);
            
        }


        public ICommand FaceEnergyPropertyBtnClick => new RelayCommand(() => {
            var energyProp = this.HoneybeeObject.Properties.Energy ?? new FaceEnergyPropertiesAbridged();
            energyProp = energyProp.DuplicateFaceEnergyPropertiesAbridged();
            var dialog = new Dialog_FaceEnergyProperty(ModelProperties.Energy, energyProp);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Energy = dialog_rc;
                this.ActionWhenChanged?.Invoke($"Set {this.HoneybeeObject.Identifier} Energy Properties ");
            }
        });

        public ICommand FaceRadiancePropertyBtnClick => new RelayCommand(() => {
            var prop = this.HoneybeeObject.Properties.Radiance ?? new FaceRadiancePropertiesAbridged();
            prop = prop.DuplicateFaceRadiancePropertiesAbridged();
            var dialog = new Dialog_FaceRadianceProperty(this.ModelProperties.Radiance, prop);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Radiance = dialog_rc;
                this.ActionWhenChanged?.Invoke($"Set {this.HoneybeeObject.Identifier} Radiance Properties ");
            }
        });

        public ICommand EditFaceBoundaryConditionBtnClick => new RelayCommand(() => {
            if (this.HoneybeeObject.BoundaryCondition.Obj is Outdoors outdoors)
            {
                var od = outdoors.DuplicateOutdoors();
                var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                var dialog_rc = dialog.ShowModal(Config.Owner);
                if (dialog_rc != null)
                {
                    this.HoneybeeObject.BoundaryCondition = dialog_rc;
                    this.ActionWhenChanged?.Invoke($"Set Face Boundary Condition");
                }
            }
            else
            {
                MessageBox.Show(Config.Owner, "Only Outdoors type has additional properties to edit!");
            }
        });

    }



}
