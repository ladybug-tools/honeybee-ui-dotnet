using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Energy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{

    public class ProgramTypeViewModel : ViewModelBase
    {
        //private static IEnumerable<IDdEnergyBaseModel> _schedules;
        //public static IEnumerable<IDdEnergyBaseModel> Schedules
        //{
        //    get
        //    {
        //        var libObjs = HoneybeeSchema.Helper.EnergyLibrary.StandardsSchedules.ToList<IDdEnergyBaseModel>();
        //        var inModelObjs = HoneybeeSchema.Helper.EnergyLibrary.InModelEnergyProperties.Schedules
        //            .Select(_ => _.Obj as IDdEnergyBaseModel);

        //        libObjs.AddRange(inModelObjs);
        //        _schedules = libObjs;

        //        return _schedules;
        //    }
        //}

        private ProgramTypeAbridged _hbObj;
        public ProgramTypeAbridged hbObj
        {
            get => _hbObj;
            set => Set(() => _hbObj = value, nameof(hbObj));
        }

        #region People
        public PeopleAbridged People
        {
            get
            {
                if (_hbObj.People == null)
                {
                    // this is only needed to initialize UI
                    return new PeopleAbridged("", 0, "", "");
                }
                return _hbObj.People;
            }
            set
            {
                _hbObj.People = value;
                var props = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("PPL_")).Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }
        public string PPL_DisplayName
        {
            get => People.DisplayName ?? People.Identifier;
            set => Set(() => _hbObj.People.DisplayName = value, nameof(PPL_DisplayName));
        }

        public double PPL_PeoplePerArea
        {
            get => People.PeoplePerArea;
            set => Set(() => _hbObj.People.PeoplePerArea = value, nameof(PPL_PeoplePerArea));
        }
        public string PPL_OccupancySchedule
        {
            get => People.OccupancySchedule;
            set => Set(() => _hbObj.People.OccupancySchedule = value, nameof(PPL_OccupancySchedule));
        }
        //public IDdEnergyBaseModel PPL_OccupancySchedule
        //{
        //    get => Schedules.First(_=>_.Identifier == People.OccupancySchedule);
        //    set => Set(() => _hbObj.People.OccupancySchedule = value.Identifier, nameof(PPL_OccupancySchedule));
        //}
        public string PPL_ActivitySchedule
        {
            get => People.ActivitySchedule;
            set => Set(() => _hbObj.People.ActivitySchedule = value, nameof(PPL_ActivitySchedule));
        }
        //public IDdEnergyBaseModel PPL_ActivitySchedule
        //{
        //    get => Schedules.First(_ => _.Identifier == People.ActivitySchedule);
        //    set => Set(() => _hbObj.People.ActivitySchedule = value.Identifier, nameof(PPL_ActivitySchedule));
        //}
        public double PPL_RadiantFraction
        {
            get => People.RadiantFraction;
            set => Set(() => _hbObj.People.RadiantFraction = value, nameof(PPL_RadiantFraction));
        }
        public double PPL_LatentFraction
        {
            get
            {
                if (People.LatentFraction == null)
                    return 0;
                if (People.LatentFraction.Obj is double v)
                    return v;
                return 0;
            }
            
            set => Set(() => _hbObj.People.LatentFraction = value, nameof(PPL_LatentFraction));
        }

        public bool PPL_IsLatentFractionAutocalculate
        {
            get 
            {
                if (People.LatentFraction == null)
                    return true;
                return _hbObj.People.LatentFraction.Obj is Autocalculate;
            } 
            set => Set(
                () => {
                    if (value)
                        _hbObj.People.LatentFraction = new Autocalculate();
                },
                nameof(PPL_IsLatentFractionAutocalculate));
        }
        #endregion

        #region Lighting
        public LightingAbridged Lighting
        {
            get
            {
                if (_hbObj.Lighting == null)
                {
                    // this is only needed to initialize UI
                    return new LightingAbridged("", 0, "");
                }
                return _hbObj.Lighting;
            }
            set 
            {
                _hbObj.Lighting = value;
                var props = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("LPD_")).Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }

        public string LPD_DisplayName
        {
            get => Lighting.DisplayName ?? Lighting.Identifier;
            set => Set(() => _hbObj.Lighting.DisplayName = value, nameof(LPD_DisplayName));
        }

        public double LPD_WattsPerArea
        {
            get => Lighting.WattsPerArea;
            set => Set(() => _hbObj.Lighting.WattsPerArea = value, nameof(LPD_WattsPerArea));
        }
        public string LPD_Schedule
        {
            get => Lighting.Schedule;
            set => Set(() => _hbObj.Lighting.Schedule = value, nameof(LPD_Schedule));
        }
        public double LPD_VisibleFraction
        {
            get => Lighting.VisibleFraction;
            set => Set(() => _hbObj.Lighting.VisibleFraction = value, nameof(LPD_VisibleFraction));
        }
        public double LPD_RadiantFraction
        {
            get => Lighting.RadiantFraction;
            set => Set(() => _hbObj.Lighting.RadiantFraction = value, nameof(LPD_RadiantFraction));
        }
        public double LPD_ReturnAirFraction
        {
            get => Lighting.ReturnAirFraction;
            set => Set(() => _hbObj.Lighting.ReturnAirFraction = value, nameof(LPD_ReturnAirFraction));
        }
        #endregion

        #region ElectricEquipment
        public ElectricEquipmentAbridged ElectricEquipment
        {
            get
            {
                if (_hbObj.ElectricEquipment == null)
                {
                    // this is only needed to initialize UI
                    return new ElectricEquipmentAbridged("", 0, "", "");
                }
                return _hbObj.ElectricEquipment;
            }
            set
            {
                _hbObj.ElectricEquipment = value;
                var props = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("EQP_")).Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }
        public string EQP_DisplayName
        {
            get => ElectricEquipment.DisplayName ?? ElectricEquipment.Identifier;
            set => Set(() => _hbObj.ElectricEquipment.DisplayName = value, nameof(EQP_DisplayName));
        }
        public double EQP_WattsPerArea
        {
            get => ElectricEquipment.WattsPerArea;
            set => Set(() => _hbObj.ElectricEquipment.WattsPerArea = value, nameof(EQP_WattsPerArea));
        }
        public string EQP_Schedule
        {
            get => ElectricEquipment.Schedule;
            set => Set(() => _hbObj.ElectricEquipment.Schedule = value, nameof(EQP_Schedule));
        }
        public double EQP_LostFraction
        {
            get => ElectricEquipment.LostFraction;
            set => Set(() => _hbObj.ElectricEquipment.LostFraction = value, nameof(EQP_LostFraction));
        }
        public double EQP_RadiantFraction
        {
            get => ElectricEquipment.RadiantFraction;
            set => Set(() => _hbObj.ElectricEquipment.RadiantFraction = value, nameof(EQP_RadiantFraction));
        }
        public double EQP_LatentFraction
        {
            get => ElectricEquipment.LatentFraction;
            set => Set(() => _hbObj.ElectricEquipment.LatentFraction = value, nameof(EQP_LatentFraction));
        }
        #endregion

        #region GasEquipment
        public GasEquipmentAbridged GasEquipment
        {
            get
            {
                if (_hbObj.GasEquipment == null)
                {
                    // this is only needed to initialize UI
                    return new GasEquipmentAbridged("", 0, "", "");
                }
                return _hbObj.GasEquipment;
            }
            set
            {
                _hbObj.GasEquipment = value;
                var props = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("GAS_")).Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }
        public string GAS_DisplayName
        {
            get => GasEquipment.DisplayName ?? GasEquipment.Identifier;
            set => Set(() => _hbObj.GasEquipment.DisplayName = value, nameof(GAS_DisplayName));
        }
        public double GAS_WattsPerArea
        {
            get => GasEquipment.WattsPerArea;
            set => Set(() => _hbObj.GasEquipment.WattsPerArea = value, nameof(GAS_WattsPerArea));
        }
        public string GAS_Schedule
        {
            get => GasEquipment.Schedule;
            set => Set(() => _hbObj.GasEquipment.Schedule = value, nameof(GAS_Schedule));
        }
        public double GAS_LostFraction
        {
            get => GasEquipment.LostFraction;
            set => Set(() => _hbObj.GasEquipment.LostFraction = value, nameof(GAS_LostFraction));
        }
        public double GAS_RadiantFraction
        {
            get => GasEquipment.RadiantFraction;
            set => Set(() => _hbObj.GasEquipment.RadiantFraction = value, nameof(GAS_RadiantFraction));
        }
        public double GAS_LatentFraction
        {
            get => GasEquipment.LatentFraction;
            set => Set(() => _hbObj.GasEquipment.LatentFraction = value, nameof(GAS_LatentFraction));
        }
        #endregion

        #region Infiltration
        public InfiltrationAbridged Infiltration
        {
            get
            {
                if (_hbObj.Infiltration == null)
                {
                    // this is only needed to initialize UI
                    return new InfiltrationAbridged("", 0, "", "");
                }
                return _hbObj.Infiltration;
            }
            set
            {
                _hbObj.Infiltration = value;
                var props = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("INF_")).Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }
        public string INF_DisplayName
        {
            get => Infiltration.DisplayName ?? Infiltration.Identifier;
            set => Set(() => _hbObj.Infiltration.DisplayName = value, nameof(INF_DisplayName));
        }
        public double INF_FlowPerExteriorArea
        {
            get => Infiltration.FlowPerExteriorArea;
            set => Set(() => _hbObj.Infiltration.FlowPerExteriorArea = value, nameof(INF_FlowPerExteriorArea));
        }
        public string INF_Schedule
        {
            get => Infiltration.Schedule;
            set => Set(() => _hbObj.Infiltration.Schedule = value, nameof(INF_Schedule));
        }
        public double INF_VelocityCoefficient
        {
            get => Infiltration.VelocityCoefficient;
            set => Set(() => _hbObj.Infiltration.VelocityCoefficient = value, nameof(INF_VelocityCoefficient));
        }
        public double INF_TemperatureCoefficient
        {
            get => Infiltration.TemperatureCoefficient;
            set => Set(() => _hbObj.Infiltration.TemperatureCoefficient = value, nameof(INF_TemperatureCoefficient));
        }
        public double INF_ConstantCoefficient
        {
            get => Infiltration.ConstantCoefficient;
            set => Set(() => _hbObj.Infiltration.ConstantCoefficient = value, nameof(INF_ConstantCoefficient));
        }
        #endregion

        #region Ventilation
        public VentilationAbridged Ventilation
        {
            get
            {
                if (_hbObj.Ventilation == null)
                {
                    // this is only needed to initialize UI
                    return new VentilationAbridged("");
                }
                return _hbObj.Ventilation;
            }
            set
            {
                _hbObj.Ventilation = value;
                var props = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("VNT_")).Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }
        public string VNT_DisplayName
        {
            get => Ventilation.DisplayName ?? Ventilation.Identifier;
            set => Set(() => _hbObj.Ventilation.DisplayName = value, nameof(VNT_DisplayName));
        }
        public double VNT_AirChangesPerHour
        {
            get => Ventilation.AirChangesPerHour;
            set => Set(() => _hbObj.Ventilation.AirChangesPerHour = value, nameof(VNT_AirChangesPerHour));
        }
        public string VNT_Schedule
        {
            get => Ventilation.Schedule;
            set => Set(() => _hbObj.Ventilation.Schedule = value, nameof(VNT_Schedule));
        }
        public double VNT_FlowPerArea
        {
            get => Ventilation.FlowPerArea;
            set => Set(() => _hbObj.Ventilation.FlowPerArea = value, nameof(VNT_FlowPerArea));
        }
        public double VNT_FlowPerPerson
        {
            get => Ventilation.FlowPerPerson;
            set => Set(() => _hbObj.Ventilation.FlowPerPerson = value, nameof(VNT_FlowPerPerson));
        }
        public double VNT_FlowPerZone
        {
            get => Ventilation.FlowPerZone;
            set => Set(() => _hbObj.Ventilation.FlowPerZone = value, nameof(VNT_FlowPerZone));
        }
        #endregion

        #region Setpoint
        public SetpointAbridged Setpoint
        {
            get
            {
                if (_hbObj.Setpoint == null)
                {
                    // this is only needed to initialize UI
                    return new SetpointAbridged("", "", "");
                }
                return _hbObj.Setpoint;
            }
            set
            {
                _hbObj.Setpoint = value;
                var props = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("SPT_")).Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }
        public string SPT_DisplayName
        {
            get => Setpoint.DisplayName ?? Setpoint.Identifier;
            set => Set(() => _hbObj.Setpoint.DisplayName = value, nameof(SPT_DisplayName));
        }
        public string SPT_CoolingSchedule
        {
            get => Setpoint.CoolingSchedule;
            set => Set(() => _hbObj.Setpoint.CoolingSchedule = value, nameof(SPT_CoolingSchedule));
        }
        public string SPT_HeatingSchedule
        {
            get => Setpoint.HeatingSchedule;
            set => Set(() => _hbObj.Setpoint.HeatingSchedule = value, nameof(SPT_HeatingSchedule));
        }
        public string SPT_HumidifyingSchedule
        {
            get => Setpoint.HumidifyingSchedule;
            set => Set(() => _hbObj.Setpoint.HumidifyingSchedule = value, nameof(SPT_HumidifyingSchedule));
        }
        public string SPT_DehumidifyingSchedule
        {
            get => Setpoint.DehumidifyingSchedule;
            set => Set(() => _hbObj.Setpoint.DehumidifyingSchedule = value, nameof(SPT_DehumidifyingSchedule));
        }
        #endregion

        private static ProgramTypeViewModel _instance;
        public static ProgramTypeViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProgramTypeViewModel();
                }
                return _instance;
            }
        }

        //public void UpdatePeople(PeopleAbridged newObj)
        //{
        //    this.hbObj.People = newObj;
        //    var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("PPL_")).Select(_ => _.Name);
        //    this.RefreshControls(propleProps);
        //}
        //public void UpdateLighting(LightingAbridged newObj)
        //{
        //    this.hbObj.Lighting = newObj;
        //    var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("LPD_")).Select(_ => _.Name);
        //    this.RefreshControls(propleProps);
        //}
        //public void UpdateEquipment(ElectricEquipmentAbridged newObj)
        //{
        //    this.hbObj.ElectricEquipment = newObj;
        //    var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("EQP_")).Select(_ => _.Name);
        //    this.RefreshControls(propleProps);
        //}
        //public void UpdateGasEquipment(GasEquipmentAbridged newObj)
        //{
        //    this.hbObj.GasEquipment = newObj;
        //    var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("GAS_")).Select(_ => _.Name);
        //    this.RefreshControls(propleProps);
        //}
        //public void UpdateInfiltration(InfiltrationAbridged newObj)
        //{
        //    this.hbObj.Infiltration = newObj;
        //    var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("INF_")).Select(_ => _.Name);
        //    this.RefreshControls(propleProps);
        //}
        //public void UpdateVentilation(VentilationAbridged newObj)
        //{
        //    this.hbObj.Ventilation = newObj;
        //    var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("VNT_")).Select(_ => _.Name);
        //    this.RefreshControls(propleProps);
        //}
        //public void UpdateSetpoint(SetpointAbridged newObj)
        //{
        //    this.hbObj.Setpoint = newObj;
        //    var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("SPT_")).Select(_ => _.Name);
        //    this.RefreshControls(propleProps);
        //}

        private ProgramTypeViewModel()
        {
        }

    }



}
