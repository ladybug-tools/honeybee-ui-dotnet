using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;



namespace Honeybee.UI.ViewModel
{

    public class RoomPropertyViewModel : ViewModelBase
    {
      
        

        private Room _refHBObj;
  

        private List<Room> _hbObjs;

        public int TabIndex
        {
            get => 0;
            set { this.Set(null, nameof(TabIndex)); }
        }

        #region RoomProperty

        public string Identifier
        {
            get => _refHBObj.Identifier;
            set { this.Set(() => _refHBObj.Identifier = value, nameof(_refHBObj.Identifier)); }
        }


        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            set {
                _isDisplayNameVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName)); 
            }
        }

        private bool _isStoryVaries;
        public string Story
        {
            get => _refHBObj.Story;
            set {
                _isStoryVaries = value == ReservedText.Varies; 
                this.Set(() => _refHBObj.Story = value, nameof(Story)); 
            }
        }

        private bool _isMultiplierVaries;
     
        private string _multiplierText;
        public string MultiplierText
        {
            get => _multiplierText;
            set
            {
                _isMultiplierVaries = value == ReservedText.Varies;
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

        private CheckboxViewModel _isExcludeFloor;
        public CheckboxViewModel IsExcludeFloor
        {
            get => _isExcludeFloor;
            set => this.Set(() => _isExcludeFloor = value, nameof(IsExcludeFloor));
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

        private ElecEquipmentViewModel _equip;
        public ElecEquipmentViewModel ElecEquipment
        {
            get => _equip;
            set { this.Set(() => _equip = value, nameof(ElecEquipment)); }
        }

        private GasEquipmentViewModel _gas;
        public GasEquipmentViewModel Gas
        {
            get => _gas;
            set { this.Set(() => _gas = value, nameof(Gas)); }
        }

        private PeopleViewModel _people;
        public PeopleViewModel People
        {
            get => _people;
            set { this.Set(() => _people = value, nameof(People)); }
        }

        private InfiltrationViewModel _infiltration;
        public InfiltrationViewModel Infiltration
        {
            get => _infiltration;
            set { this.Set(() => _infiltration = value, nameof(Infiltration)); }
        }

        private VentilationViewModel _ventilation;
        public VentilationViewModel Ventilation
        {
            get => _ventilation;
            set { this.Set(() => _ventilation = value, nameof(Ventilation)); }
        }

        private SetpointViewModel _setpoint;
        public SetpointViewModel Setpoint
        {
            get => _setpoint;
            set { this.Set(() => _setpoint = value, nameof(Setpoint)); }
        }

        private ServiceHotWaterViewModel _serviceHotWater;
        public ServiceHotWaterViewModel ServiceHotWater
        {
            get => _serviceHotWater;
            set { this.Set(() => _serviceHotWater = value, nameof(ServiceHotWater)); }
        }

        private InternalMassViewModel _internalMass;
        public InternalMassViewModel InternalMass
        {
            get => _internalMass;
            set { this.Set(() => _internalMass = value, nameof(InternalMass)); }
        }

        private ProcessLoadViewModel _processLoad;
        public ProcessLoadViewModel ProcessLoad
        {
            get => _processLoad;
            set { this.Set(() => _processLoad = value, nameof(ProcessLoad)); }
        }

        private VentilationControlViewModel _ventilationControl;
        public VentilationControlViewModel VentilationControl
        {
            get => _ventilationControl;
            set { this.Set(() => _ventilationControl = value, nameof(VentilationControl)); }
        }

        private DaylightingControlViewModel _daylightingControl;
        public DaylightingControlViewModel DaylightingControl
        {
            get => _daylightingControl;
            set { this.Set(() => _daylightingControl = value, nameof(DaylightingControl)); }
        }


        #endregion

        #region UserData
        private UserDataViewModel _userData;
        public UserDataViewModel UserData
        {
            get => _userData;
            set { this.Set(() => _userData = value, nameof(UserData)); }
        }

        #endregion


        private View.RoomProperty _control;
        private ModelProperties _libSource { get; set; }
        public Room Default { get; private set; }
        internal RoomPropertyViewModel(View.RoomProperty roomPanel)
        {
            this.Default = new Room("id", new List<Face>(), new RoomPropertiesAbridged());
            _refHBObj = this.Default.DuplicateRoom();
            _libSource = new ModelProperties(ModelEnergyProperties.Default, ModelRadianceProperties.Default);
            this._control = roomPanel;
            Update(_libSource, new List<Room>() { _refHBObj });
        }
        
        public void Update(ModelProperties libSource, List<Room> rooms)
        {
            this.TabIndex = 0;
            this._libSource = libSource;
            this._refHBObj = rooms.FirstOrDefault().DuplicateRoom();
            var defaultEnergy = new RoomEnergyPropertiesAbridged();
            var defaultRadiance = new RoomRadiancePropertiesAbridged();
            _refHBObj.Properties.Energy = _refHBObj.Properties.Energy ?? defaultEnergy;
            _refHBObj.Properties.Radiance = _refHBObj.Properties.Radiance ?? defaultRadiance;

            if (rooms.Select(_ => _.Identifier).Distinct().Count() > 1)
                this.Identifier = ReservedText.Varies;
            else
                this.Identifier = this._refHBObj.Identifier;

            if (rooms.Select(_ => _.DisplayName).Distinct().Count() > 1)
                this.DisplayName = ReservedText.Varies;
            else
                this.DisplayName = this._refHBObj.DisplayName;

            if (rooms.Select(_ => _.Story).Distinct().Count() > 1)
                this.Story = ReservedText.Varies;
            else
                this.Story = this._refHBObj.Story;

            if (rooms.Select(_ => _.Multiplier).Distinct().Count() > 1)
                this.MultiplierText = ReservedText.Varies;
            else
                this.MultiplierText = this._refHBObj.Multiplier.ToString();

            // ExcludeFloorArea
            this.IsExcludeFloor = new CheckboxViewModel(_ => _refHBObj.ExcludeFloorArea = _);
            if (rooms.Select(_ => _?.ExcludeFloorArea).Distinct().Count() > 1)
                this.IsExcludeFloor.SetCheckboxVaries();
            else
                this.IsExcludeFloor.SetCheckboxChecked(this._refHBObj.ExcludeFloorArea);



            var cSet = _libSource.Energy.ConstructionSets?
                .OfType<HoneybeeSchema.Energy.IBuildingConstructionset>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.ConstructionSet); 
            this.ConstructionSet = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ConstructionSet = s?.Identifier);


            //construction
            if (rooms.Select(_ => _.Properties.Energy?.ConstructionSet).Distinct().Count() > 1)
                this.ConstructionSet.SetBtnName(ReservedText.Varies);
            else
                this.ConstructionSet.SetPropetyObj(cSet);


            var pType = _libSource.Energy.ProgramTypes?
               .OfType<HoneybeeSchema.Energy.IProgramtype>()?
               .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.ProgramType); 
            this.PropgramType = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ProgramType = s?.Identifier);


            //program type
            if (rooms.Select(_ => _.Properties.Energy?.ProgramType).Distinct().Count() > 1)
                this.PropgramType.SetBtnName(ReservedText.Varies);
            else
                this.PropgramType.SetPropetyObj(pType);


            var hvac = _libSource.Energy.Hvacs?
                .OfType<HoneybeeSchema.Energy.IHvac>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.Hvac); 
            this.HVAC = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Hvac = s?.Identifier);

            // hvac
            if (rooms.Select(_ => _.Properties.Energy?.Hvac).Distinct().Count() > 1)
                this.HVAC.SetBtnName(ReservedText.Varies);
            else
                this.HVAC.SetPropetyObj(hvac);


            var mSet = _libSource.Radiance.ModifierSets?
               .OfType<HoneybeeSchema.ModifierSetAbridged>()?
               .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Radiance.ModifierSet);
            this.ModifierSet = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.ModifierSet = s?.Identifier);


            // ModifierSet
            if (rooms.Select(_ => _.Properties.Radiance?.ModifierSet).Distinct().Count() > 1)
                this.ModifierSet.SetBtnName(ReservedText.Varies);
            else
                this.ModifierSet.SetPropetyObj(mSet);


            // Lighting
            var allLpds = rooms.Select(_ => _.Properties.Energy?.Lighting).Distinct().ToList();
            this.Lighting = new LightingViewModel(libSource, allLpds, (s) => _refHBObj.Properties.Energy.Lighting = s as LightingAbridged);

            // ElecEqp
            var allEqps = rooms.Select(_ => _.Properties.Energy?.ElectricEquipment).Distinct().ToList();
            this.ElecEquipment = new ElecEquipmentViewModel(libSource, allEqps, (s) => _refHBObj.Properties.Energy.ElectricEquipment = s as ElectricEquipmentAbridged);

            // GasEqp
            var allGas = rooms.Select(_ => _.Properties.Energy?.GasEquipment).Distinct().ToList();
            this.Gas = new GasEquipmentViewModel(libSource, allGas, (s) => _refHBObj.Properties.Energy.GasEquipment = s as GasEquipmentAbridged);

            // People
            var allPpls = rooms.Select(_ => _.Properties.Energy?.People).Distinct().ToList();
            this.People = new PeopleViewModel(libSource, allPpls, (s) => _refHBObj.Properties.Energy.People = s as PeopleAbridged);

            // Infiltration
            var allInfs = rooms.Select(_ => _.Properties.Energy?.Infiltration).Distinct().ToList();
            this.Infiltration = new InfiltrationViewModel(libSource, allInfs, (s) => _refHBObj.Properties.Energy.Infiltration = s as InfiltrationAbridged);

            // Ventilation
            var allVents = rooms.Select(_ => _.Properties.Energy?.Ventilation).Distinct().ToList();
            this.Ventilation = new VentilationViewModel(libSource, allVents, (s) => _refHBObj.Properties.Energy.Ventilation = s as VentilationAbridged);

            // Setpoint
            var allStps = rooms.Select(_ => _.Properties.Energy?.Setpoint).Distinct().ToList();
            this.Setpoint = new SetpointViewModel(libSource, allStps, (s) => _refHBObj.Properties.Energy.Setpoint = s as SetpointAbridged);

            // ServiceHotWater
            var allSHW = rooms.Select(_ => _.Properties.Energy?.ServiceHotWater).Distinct().ToList();
            this.ServiceHotWater = new ServiceHotWaterViewModel(libSource, allSHW, (s) => _refHBObj.Properties.Energy.ServiceHotWater = s as ServiceHotWaterAbridged);

            // Process load
            var allProcessLoads = rooms.Select(_ => _.Properties.Energy?.ProcessLoads?.FirstOrDefault()).Distinct().ToList();
            this.ProcessLoad = new ProcessLoadViewModel(libSource, allProcessLoads, (s) => _refHBObj.Properties.Energy.ProcessLoads = new List<ProcessAbridged>() { s as ProcessAbridged });

            // InternalMass
            if (rooms.Select(_ => _.Properties.Energy?.InternalMasses).Any(_=>_?.Count>1))
            {
                MessageBox.Show("The current room property editor doesn't support multiple internal masses in one room.");
            }
            var allInMasses = rooms.Select(_ => _.Properties.Energy?.InternalMasses?.FirstOrDefault()).Distinct().ToList();
            this.InternalMass = new InternalMassViewModel(libSource, allInMasses, (s) => _refHBObj.Properties.Energy.InternalMasses = new List<InternalMassAbridged>() { s as InternalMassAbridged });



            // VentControl
            var allVentCtrls = rooms.Select(_ => _.Properties.Energy?.WindowVentControl).Distinct().ToList();
            this.VentilationControl = new VentilationControlViewModel(libSource, allVentCtrls, (s) => _refHBObj.Properties.Energy.WindowVentControl = s);

            // DaylightingControl
            var allDltCtrls = rooms.Select(_ => _.Properties.Energy?.DaylightingControl).Distinct().ToList();
            this.DaylightingControl = new DaylightingControlViewModel(libSource, allDltCtrls, (s) => _refHBObj.Properties.Energy.DaylightingControl = s);


            // User data
            var allUserData = rooms.Select(_ => _.UserData).Distinct().ToList();
            this.UserData = new UserDataViewModel(allUserData, (s) => _refHBObj.UserData = s, _control);


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

                if (!this.IsExcludeFloor.IsVaries)
                    item.ExcludeFloorArea = refObj.ExcludeFloorArea;

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
                

                // loads
                item.Properties.Energy.Lighting = this.Lighting.MatchObj(item.Properties.Energy.Lighting);
                item.Properties.Energy.ElectricEquipment = this.ElecEquipment.MatchObj(item.Properties.Energy.ElectricEquipment);
                item.Properties.Energy.GasEquipment = this.Gas.MatchObj(item.Properties.Energy.GasEquipment);
                item.Properties.Energy.People = this.People.MatchObj(item.Properties.Energy.People);
                item.Properties.Energy.Infiltration = this.Infiltration.MatchObj(item.Properties.Energy.Infiltration);
                item.Properties.Energy.Ventilation = this.Ventilation.MatchObj(item.Properties.Energy.Ventilation);
                item.Properties.Energy.Setpoint = this.Setpoint.MatchObj(item.Properties.Energy.Setpoint);
                item.Properties.Energy.ServiceHotWater = this.ServiceHotWater.MatchObj(item.Properties.Energy.ServiceHotWater);
                var processLoad = this.ProcessLoad.MatchObj(item.Properties.Energy.ProcessLoads?.FirstOrDefault());
                item.Properties.Energy.ProcessLoads = processLoad == null ? null:  new List<ProcessAbridged>() { processLoad };

                // internal mass
                var internalMass = this.InternalMass.MatchObj(item.Properties.Energy.InternalMasses?.FirstOrDefault());
                item.Properties.Energy.InternalMasses = internalMass == null ? null : new List<InternalMassAbridged>() { internalMass };

                // controls
                item.Properties.Energy.WindowVentControl = this.VentilationControl.MatchObj(item.Properties.Energy.WindowVentControl);
                item.Properties.Energy.DaylightingControl = this.DaylightingControl.MatchObj(item.Properties.Energy.DaylightingControl);

                // User data
                item.UserData = this.UserData.MatchObj(item.UserData);

            }

            return this._hbObjs;
        }

        public ICommand RoomConstructionSetCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ConstructionSetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.ConstructionSet.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand RoomProgramTypeCommand => new RelayCommand(() =>
        {
            var lib = _libSource;
            var dialog = new Dialog_ProgramTypeManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.PropgramType.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand RoomHVACCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_HVACManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.HVAC.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand ModifierSetCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Radiance;
            var dialog = new Dialog_ModifierSetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.ModifierSet.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand HBDataBtnClick => new RelayCommand(() => 
        {
            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });



    }



}
