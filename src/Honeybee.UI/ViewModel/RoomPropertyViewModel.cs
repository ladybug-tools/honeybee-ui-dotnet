using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;



namespace Honeybee.UI
{

    public class RoomPropertyViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        public string Unconditioned => "Unconditioned";
        public string Unoccupied => "Unoccupied, No Loads";
        public string ByGlobalConstructionSet => "By Global Construction Set";
        public string ByProgramType => "By Room Program Type";
        public string ByGlobalModifierSet => "By Global Modifier Set";

        private Room _refHBObj;
  

        private List<Room> _hbObjs;

        #region RoomProperty

        public string Identifier
        {
            get => _refHBObj.Identifier;
            private set { this.Set(() => _refHBObj.Identifier = value, nameof(_refHBObj.Identifier)); }
        }


        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            private set {
                _isDisplayNameVaries = value == this.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName)); 
            }
        }

        private bool _isStoryVaries;
        public string Story
        {
            get => _refHBObj.Story;
            private set {
                _isStoryVaries = value == this.Varies; 
                this.Set(() => _refHBObj.Story = value, nameof(Story)); 
            }
        }

        private bool _isMultiplierVaries;
     
        private string _multiplierText;
        public string MultiplierText
        {
            get => _multiplierText;
            private set
            {
                _isMultiplierVaries = value == this.Varies;
                if (_isMultiplierVaries) {
                    this.Set(() => _multiplierText = value, nameof(MultiplierText));
                }
                else if (int.TryParse(value, out var number))
                {
                    _multiplierText = number.ToString();
                    this.Set(() => _refHBObj.Multiplier = number, nameof(MultiplierText));
                }

            }
        }

        #endregion


        #region ConstructionSet

        private CheckboxButtonViewModel _constructionSet;
        public CheckboxButtonViewModel ConstructionSet
        {
            get => _constructionSet;
            set { this.Set(() => _constructionSet = value, nameof(ConstructionSet)); }
        }

        #endregion



        #region ProgramType
        private CheckboxButtonViewModel _propgramType;
        public CheckboxButtonViewModel PropgramType
        {
            get => _propgramType;
            set { this.Set(() => _propgramType = value, nameof(PropgramType)); }
        }

        #endregion


        #region HVAC
        private CheckboxButtonViewModel _hvac;
        public CheckboxButtonViewModel HVAC
        {
            get => _hvac;
            set { this.Set(() => _hvac = value, nameof(HVAC)); }
        }

        #endregion


        #region ModifierSet

        private CheckboxButtonViewModel _modifierSet;
        public CheckboxButtonViewModel ModifierSet
        {
            get => _modifierSet;
            set { this.Set(() => _modifierSet = value, nameof(ModifierSet)); }
        }

        #endregion


        #region Loads
        private LightingViewModel _lighting;
        public LightingViewModel Lighting
        {
            get => _lighting;
            set { this.Set(() => _lighting = value, nameof(Lighting)); }
        }

        private CheckboxButtonViewModel _equip;
        public CheckboxButtonViewModel ElecEquipment
        {
            get => _equip;
            set { this.Set(() => _equip = value, nameof(ElecEquipment)); }
        }

        private CheckboxButtonViewModel _gas;
        public CheckboxButtonViewModel Gas
        {
            get => _gas;
            set { this.Set(() => _gas = value, nameof(Gas)); }
        }

        private CheckboxButtonViewModel _people;
        public CheckboxButtonViewModel People
        {
            get => _people;
            set { this.Set(() => _people = value, nameof(People)); }
        }

        private CheckboxButtonViewModel _infiltration;
        public CheckboxButtonViewModel Infiltration
        {
            get => _infiltration;
            set { this.Set(() => _infiltration = value, nameof(Infiltration)); }
        }

        private CheckboxButtonViewModel _ventilation;
        public CheckboxButtonViewModel Ventilation
        {
            get => _ventilation;
            set { this.Set(() => _ventilation = value, nameof(Ventilation)); }
        }

        private CheckboxButtonViewModel _setpoint;
        public CheckboxButtonViewModel Setpoint
        {
            get => _setpoint;
            set { this.Set(() => _setpoint = value, nameof(Setpoint)); }
        }

        private CheckboxButtonViewModel _serviceHotWater;
        public CheckboxButtonViewModel ServiceHotWater
        {
            get => _serviceHotWater;
            set { this.Set(() => _serviceHotWater = value, nameof(ServiceHotWater)); }
        }
        #endregion


        private View.RoomProperty _control;
        private ModelProperties _libSource { get; set; } 
        internal RoomPropertyViewModel(View.RoomProperty roomPanel)
        {
            _refHBObj = new Room("", new List<Face>(), new RoomPropertiesAbridged());
            _libSource = new ModelProperties(ModelEnergyProperties.Default, ModelRadianceProperties.Default);
            this._control = roomPanel;
        }
        
        public void Update(ModelProperties libSource, List<Room> rooms)
        {
            this._libSource = libSource;
            this._refHBObj = rooms.FirstOrDefault().DuplicateRoom();
            _refHBObj.Properties.Energy = _refHBObj.Properties.Energy ?? new RoomEnergyPropertiesAbridged();
            _refHBObj.Properties.Radiance = _refHBObj.Properties.Radiance ?? new RoomRadiancePropertiesAbridged();

            if (rooms.Select(_ => _.Identifier).Distinct().Count() > 1)
                this.Identifier = this.Varies;
            else
                this.Identifier = this._refHBObj.Identifier;

            if (rooms.Select(_ => _.DisplayName).Distinct().Count() > 1)
                this.DisplayName = this.Varies;
            else
                this.DisplayName = this._refHBObj.DisplayName;

            if (rooms.Select(_ => _.Story).Distinct().Count() > 1)
                this.Story = this.Varies;
            else
                this.Story = this._refHBObj.Story;

            if (rooms.Select(_ => _.Multiplier).Distinct().Count() > 1)
                this.MultiplierText = this.Varies;
            else
                this.MultiplierText = this._refHBObj.Multiplier.ToString();

            

            var cSet = _libSource.Energy.ConstructionSets
                .OfType<HoneybeeSchema.Energy.IBuildingConstructionset>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.ConstructionSet); 
            this.ConstructionSet = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ConstructionSet = s?.Identifier);


            //construction
            if (rooms.Select(_ => _.Properties.Energy?.ConstructionSet).Distinct().Count() > 1)
                this.ConstructionSet.SetBtnName(this.Varies);
            else
                this.ConstructionSet.SetPropetyObj(cSet);


            var pType = _libSource.Energy.ProgramTypes
               .OfType<HoneybeeSchema.Energy.IProgramtype>()
               .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.ProgramType); 
            this.PropgramType = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ProgramType = s?.Identifier);


            //program type
            if (rooms.Select(_ => _.Properties.Energy?.ProgramType).Distinct().Count() > 1)
                this.PropgramType.SetBtnName(this.Varies);
            else
                this.PropgramType.SetPropetyObj(pType);


            var hvac = _libSource.Energy.Hvacs
                .OfType<HoneybeeSchema.Energy.IHvac>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.Hvac); 
            this.HVAC = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Hvac = s?.Identifier);

            // hvac
            if (rooms.Select(_ => _.Properties.Energy?.Hvac).Distinct().Count() > 1)
                this.HVAC.SetBtnName(this.Varies);
            else
                this.HVAC.SetPropetyObj(hvac);


            var mSet = _libSource.Radiance.ModifierSets
               .OfType<HoneybeeSchema.ModifierSetAbridged>()
               .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Radiance.ModifierSet);
            this.ModifierSet = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.ModifierSet = s?.Identifier);


            // ModifierSet
            if (rooms.Select(_ => _.Properties.Radiance?.ModifierSet).Distinct().Count() > 1)
                this.ModifierSet.SetBtnName(this.Varies);
            else
                this.ModifierSet.SetPropetyObj(mSet);


            // Lighting
            var allLpds = rooms.Select(_ => _.Properties.Energy?.Lighting).Distinct().ToList();
            this.Lighting = new LightingViewModel(libSource, allLpds, (s) => _refHBObj.Properties.Energy.Lighting = s as LightingAbridged);

           
            


            this.ElecEquipment = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ElectricEquipment = s as ElectricEquipmentAbridged);

            // ElecEqp
            if (rooms.Select(_ => _.Properties.Energy?.ElectricEquipment?.Identifier).Distinct().Count() > 1)
                this.ElecEquipment.SetBtnName(this.Varies);
            else
                this.ElecEquipment.SetPropetyObj(_refHBObj.Properties.Energy?.ElectricEquipment);


            this.Gas = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.GasEquipment = s as GasEquipmentAbridged);

            // GasEqp
            if (rooms.Select(_ => _.Properties.Energy?.GasEquipment?.Identifier).Distinct().Count() > 1)
                this.Gas.SetBtnName(this.Varies);
            else
                this.Gas.SetPropetyObj(_refHBObj.Properties.Energy?.GasEquipment);


            this.People = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.People = s as PeopleAbridged);

            // People
            if (rooms.Select(_ => _.Properties.Energy?.People?.Identifier).Distinct().Count() > 1)
                this.People.SetBtnName(this.Varies);
            else
                this.People.SetPropetyObj(_refHBObj.Properties.Energy?.People);


            this.Infiltration = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Infiltration = s as InfiltrationAbridged);

            // Infiltration
            if (rooms.Select(_ => _.Properties.Energy?.Infiltration?.Identifier).Distinct().Count() > 1)
                this.Infiltration.SetBtnName(this.Varies);
            else
                this.Infiltration.SetPropetyObj(_refHBObj.Properties.Energy?.Infiltration);


            this.Ventilation = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Ventilation = s as VentilationAbridged);

            // Ventilation
            if (rooms.Select(_ => _.Properties.Energy?.Ventilation?.Identifier).Distinct().Count() > 1)
                this.Ventilation.SetBtnName(this.Varies);
            else
                this.Ventilation.SetPropetyObj(_refHBObj.Properties.Energy?.Ventilation);


            this.Setpoint = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Setpoint = s as SetpointAbridged);

            // Setpoint
            if (rooms.Select(_ => _.Properties.Energy?.Setpoint?.Identifier).Distinct().Count() > 1)
                this.Setpoint.SetBtnName(this.Varies);
            else
                this.Setpoint.SetPropetyObj(_refHBObj.Properties.Energy?.Setpoint);


            this.ServiceHotWater = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ServiceHotWater = s as ServiceHotWaterAbridged);

            // ServiceHotWater
            if (rooms.Select(_ => _.Properties.Energy?.ServiceHotWater?.Identifier).Distinct().Count() > 1)
                this.ServiceHotWater.SetBtnName(this.Varies);
            else
                this.ServiceHotWater.SetPropetyObj(_refHBObj.Properties.Energy?.ServiceHotWater);


            //this.VentControl = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.WindowVentControl = s as VentilationControlAbridged);

            //// VentControl
            //if (rooms.Select(_ => _.Properties.Energy?.WindowVentControl).Distinct().Count() > 1)
            //    this.VentControl.SetBtnName(this.Varies);
            //else
            //    this.VentControl.SetPropetyObj(_refHBObj.Properties.Energy?.WindowVentControl);

            //this.DaylightingControl = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.DaylightingControl = s as DaylightingControl);

            //// DaylightingControl
            //if (rooms.Select(_ => _.Properties.Energy?.DaylightingControl).Distinct().Count() > 1)
            //    this.DaylightingControl.SetBtnName(this.Varies);
            //else
            //    this.DaylightingControl.SetPropetyObj(_refHBObj.Properties.Energy?.DaylightingControl);


            this._hbObjs = rooms.Select(_ => _.DuplicateRoom()).ToList();

        }

      

        public List<Room> GetRooms()
        {
            var refObj = this._refHBObj;
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this._isStoryVaries)
                    item.Story = refObj.Story;

                if (!this._isMultiplierVaries)
                    item.Multiplier = refObj.Multiplier;


                item.Properties.Energy = item.Properties.Energy ?? new RoomEnergyPropertiesAbridged();

                if (!this.ConstructionSet.IsVaries)
                    item.Properties.Energy.ConstructionSet = refObj.Properties.Energy.ConstructionSet;

                if (!this.PropgramType.IsVaries)
                    item.Properties.Energy.ProgramType = refObj.Properties.Energy.ProgramType;

                if (!this.HVAC.IsVaries)
                    item.Properties.Energy.Hvac = refObj.Properties.Energy.Hvac;

                if (!this.ModifierSet.IsVaries)
                {
                    item.Properties.Radiance = item.Properties.Radiance ?? new RoomRadiancePropertiesAbridged();
                    item.Properties.Radiance.ModifierSet = refObj.Properties.Radiance.ModifierSet;
                }
                

                // lighting
                item.Properties.Energy.Lighting = this.Lighting.MatchObj(item.Properties.Energy.Lighting);
            
            }

            return this._hbObjs;
        }

        public ICommand RoomConstructionSetCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_ConstructionSetSelector(_libSource.Energy);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.ConstructionSet.SetPropetyObj(dialog_rc);
            }
        });

        public ICommand RoomProgramTypeCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_ProgramTypeSelector(_libSource.Energy);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.PropgramType.SetPropetyObj(dialog_rc);
            }
        });

        public ICommand RoomHVACCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_HVACSelector(_libSource.Energy);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HVAC.SetPropetyObj(dialog_rc);
            }
        });

        public ICommand ModifierSetCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_ModifierSetSelector(_libSource.Radiance);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.ModifierSet.SetPropetyObj(dialog_rc);
            }
        });

        public ICommand HBDataBtnClick => new RelayCommand(() => {

            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });



    }



}
