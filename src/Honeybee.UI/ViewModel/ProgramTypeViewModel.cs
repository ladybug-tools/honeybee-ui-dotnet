using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Energy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;

namespace Honeybee.UI
{

    public class ProgramTypeViewModel : ViewModelBase
    {

        public ModelProperties ModelEnergyProp { get; set; }

      

        private IEnumerable<HoneybeeSchema.Energy.ILoad> _loads;

        public IEnumerable<HoneybeeSchema.Energy.ILoad> Loads
        {
            get
            {
                if (_loads == null)
                {
                    var libObjs = new List<HoneybeeSchema.Energy.ILoad>();
                    libObjs.AddRange(EnergyLibrary.DefaultPeopleLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultLightingLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultElectricEquipmentLoads);
                    libObjs.AddRange(EnergyLibrary.GasEquipmentLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultInfiltrationLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultVentilationLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultSetpoints);
                    libObjs.AddRange(EnergyLibrary.DefaultServiceHotWaterLoads);
                    _loads = libObjs;
                }


                return _loads;
            }
        }

        private ProgramTypeAbridged _refHBObj;
        public string Identifier
        {
            get => _refHBObj.Identifier;
            set { this.Set(() => _refHBObj.Identifier = value, nameof(Identifier)); }
        }
        public string Name
        {
            get => _refHBObj.DisplayName ?? _refHBObj.Identifier;
            set { this.Set(() => _refHBObj.DisplayName = value, nameof(Name)); }
        }

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

        #endregion


        private Control _control;
        private ProgramTypeAbridged _hbObj;

        public ProgramTypeViewModel(Control control, ref ModelProperties libSource, ProgramTypeAbridged programType)
        {
            this._control = control;
            libSource.FillNulls();

            this.ModelEnergyProp = libSource;
            _refHBObj = programType?.DuplicateProgramTypeAbridged() ?? new ProgramTypeAbridged(Guid.NewGuid().ToString());
            _hbObj = _refHBObj.DuplicateProgramTypeAbridged();

            this.Identifier = _refHBObj.Identifier;
            this.Name = _refHBObj.DisplayName ?? _refHBObj.Identifier;

            // Lighting
            this.Lighting = new LightingViewModel(libSource, new[] { _refHBObj.Lighting }.ToList() , (s) => _refHBObj.Lighting = s as LightingAbridged);

            // ElecEqp
            this.ElecEquipment = new ElecEquipmentViewModel(libSource, new[] { _refHBObj.ElectricEquipment }.ToList(), (s) => _refHBObj.ElectricEquipment = s as ElectricEquipmentAbridged);

            // GasEqp
            this.Gas = new GasEquipmentViewModel(libSource, new[] { _refHBObj.GasEquipment }.ToList(), (s) => _refHBObj.GasEquipment = s as GasEquipmentAbridged);

            // People
            this.People = new PeopleViewModel(libSource, new[] { _refHBObj.People }.ToList(), (s) => _refHBObj.People = s as PeopleAbridged);

            // Infiltration
            this.Infiltration = new InfiltrationViewModel(libSource, new[] { _refHBObj.Infiltration }.ToList(), (s) => _refHBObj.Infiltration = s as InfiltrationAbridged);

            // Ventilation
            this.Ventilation = new VentilationViewModel(libSource, new[] { _refHBObj.Ventilation }.ToList(), (s) => _refHBObj.Ventilation = s as VentilationAbridged);

            // Setpoint
            this.Setpoint = new SetpointViewModel(libSource, new[] { _refHBObj.Setpoint }.ToList(), (s) => _refHBObj.Setpoint = s as SetpointAbridged);

            // ServiceHotWater
            this.ServiceHotWater = new ServiceHotWaterViewModel(libSource, new[] { _refHBObj.ServiceHotWater }.ToList(), (s) => _refHBObj.ServiceHotWater = s as ServiceHotWaterAbridged);

        }

        public System.Windows.Input.ICommand HBDataBtnClick => new RelayCommand(() =>
        {
            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });

        public ProgramTypeAbridged GetHBObject()
        {
            _hbObj.Identifier = this.Identifier;
            _hbObj.DisplayName = this.Name;

            // loads
            _hbObj.Lighting = this.Lighting.MatchObj(_hbObj.Lighting);
            _hbObj.ElectricEquipment = this.ElecEquipment.MatchObj(_hbObj.ElectricEquipment);
            _hbObj.GasEquipment = this.Gas.MatchObj(_hbObj.GasEquipment);
            _hbObj.People = this.People.MatchObj(_hbObj.People);
            _hbObj.Infiltration = this.Infiltration.MatchObj(_hbObj.Infiltration);
            _hbObj.Ventilation = this.Ventilation.MatchObj(_hbObj.Ventilation);
            _hbObj.Setpoint = this.Setpoint.MatchObj(_hbObj.Setpoint);
            _hbObj.ServiceHotWater = this.ServiceHotWater.MatchObj(_hbObj.ServiceHotWater);

            var obj = _hbObj.DuplicateProgramTypeAbridged();
            return obj;

           
        }
    }



}
