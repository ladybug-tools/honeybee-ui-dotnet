﻿using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;



namespace Honeybee.UI.ViewModel
{

    public class RoomPropertyViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        public string Unconditioned => "Unconditioned";
        public string Unoccupied => "Unoccupied, No Loads";
        public string ByGlobalConstructionSet => "By Global Construction Set";
        public string ByProgramType => "By Room Program Type";
        public string ByGlobalModifierSet => "By Global Modifier Set";
        public string NoControl => "No Control";

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
            this._libSource = libSource;
            this._refHBObj = rooms.FirstOrDefault().DuplicateRoom();
            var defaultEnergy = new RoomEnergyPropertiesAbridged();
            var defaultRadiance = new RoomRadiancePropertiesAbridged();
            _refHBObj.Properties.Energy = _refHBObj.Properties.Energy ?? defaultEnergy;
            _refHBObj.Properties.Radiance = _refHBObj.Properties.Radiance ?? defaultRadiance;

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

            

            var cSet = _libSource.Energy.ConstructionSets?
                .OfType<HoneybeeSchema.Energy.IBuildingConstructionset>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.ConstructionSet); 
            this.ConstructionSet = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ConstructionSet = s?.Identifier);


            //construction
            if (rooms.Select(_ => _.Properties.Energy?.ConstructionSet).Distinct().Count() > 1)
                this.ConstructionSet.SetBtnName(this.Varies);
            else
                this.ConstructionSet.SetPropetyObj(cSet);


            var pType = _libSource.Energy.ProgramTypes?
               .OfType<HoneybeeSchema.Energy.IProgramtype>()?
               .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.ProgramType); 
            this.PropgramType = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.ProgramType = s?.Identifier);


            //program type
            if (rooms.Select(_ => _.Properties.Energy?.ProgramType).Distinct().Count() > 1)
                this.PropgramType.SetBtnName(this.Varies);
            else
                this.PropgramType.SetPropetyObj(pType);


            var hvac = _libSource.Energy.Hvacs?
                .OfType<HoneybeeSchema.Energy.IHvac>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.Hvac); 
            this.HVAC = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Hvac = s?.Identifier);

            // hvac
            if (rooms.Select(_ => _.Properties.Energy?.Hvac).Distinct().Count() > 1)
                this.HVAC.SetBtnName(this.Varies);
            else
                this.HVAC.SetPropetyObj(hvac);


            var mSet = _libSource.Radiance.ModifierSets?
               .OfType<HoneybeeSchema.ModifierSetAbridged>()?
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
                

                // loads
                item.Properties.Energy.Lighting = this.Lighting.MatchObj(item.Properties.Energy.Lighting);
                item.Properties.Energy.ElectricEquipment = this.ElecEquipment.MatchObj(item.Properties.Energy.ElectricEquipment);
                item.Properties.Energy.GasEquipment = this.Gas.MatchObj(item.Properties.Energy.GasEquipment);
                item.Properties.Energy.People = this.People.MatchObj(item.Properties.Energy.People);
                item.Properties.Energy.Infiltration = this.Infiltration.MatchObj(item.Properties.Energy.Infiltration);
                item.Properties.Energy.Ventilation = this.Ventilation.MatchObj(item.Properties.Energy.Ventilation);
                item.Properties.Energy.Setpoint = this.Setpoint.MatchObj(item.Properties.Energy.Setpoint);
                item.Properties.Energy.ServiceHotWater = this.ServiceHotWater.MatchObj(item.Properties.Energy.ServiceHotWater);

                // internal mass
                var internalMass = this.InternalMass.MatchObj(item.Properties.Energy.InternalMasses?.FirstOrDefault());
                item.Properties.Energy.InternalMasses = internalMass == null ? null : new List<InternalMassAbridged>() { internalMass };

                // controls
                item.Properties.Energy.WindowVentControl = this.VentilationControl.MatchObj(item.Properties.Energy.WindowVentControl);
                item.Properties.Energy.DaylightingControl = this.DaylightingControl.MatchObj(item.Properties.Energy.DaylightingControl);

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
            var lib = _libSource.Energy;
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

        public ICommand HBDataBtnClick => new RelayCommand(() => {

            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });



    }



}
