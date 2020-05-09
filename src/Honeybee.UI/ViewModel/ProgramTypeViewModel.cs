using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{

    public class ProgramTypeViewModel : ViewModelBase
    {
        private ProgramTypeAbridged _hbObj;
        public ProgramTypeAbridged hbObj
        {
            get => _hbObj;
            set => Set(() => _hbObj = value, nameof(hbObj));
        }

        #region People
        public double PPL_PeoplePerArea
        {
            get => _hbObj.People.PeoplePerArea;
            set => Set(() => _hbObj.People.PeoplePerArea = value, nameof(PPL_PeoplePerArea));
        }
        public string PPL_OccupancySchedule
        {
            get => _hbObj.People.OccupancySchedule;
            set => Set(() => _hbObj.People.OccupancySchedule = value, nameof(PPL_OccupancySchedule));
        }
        public string PPL_ActivitySchedule
        {
            get => _hbObj.People.ActivitySchedule;
            set => Set(() => _hbObj.People.ActivitySchedule = value, nameof(PPL_ActivitySchedule));
        }
        public double PPL_RadiantFraction
        {
            get => _hbObj.People.RadiantFraction;
            set => Set(() => _hbObj.People.RadiantFraction = value, nameof(PPL_RadiantFraction));
        }
        public double PPL_LatentFraction
        {
            get => _hbObj.People.LatentFraction.Obj is double ? Double.Parse(_hbObj.People.LatentFraction.ToString()) : 0;
            set => Set(() => _hbObj.People.LatentFraction = value, nameof(PPL_LatentFraction));
        }

        public bool PPL_IsLatentFractionAutocalculate
        {
            get => _hbObj.People.LatentFraction.Obj is Autocalculate;
            set => Set(
                () => {
                    if (value)
                        _hbObj.People.LatentFraction = new Autocalculate();
                },
                nameof(PPL_IsLatentFractionAutocalculate));
        }
        #endregion

        #region Lighting
        public double LPD_WattsPerArea
        {
            get => _hbObj.Lighting.WattsPerArea;
            set => Set(() => _hbObj.Lighting.WattsPerArea = value, nameof(LPD_WattsPerArea));
        }
        public string LPD_Schedule
        {
            get => _hbObj.Lighting.Schedule;
            set => Set(() => _hbObj.Lighting.Schedule = value, nameof(LPD_Schedule));
        }
        public double LPD_VisibleFraction
        {
            get => _hbObj.Lighting.VisibleFraction;
            set => Set(() => _hbObj.Lighting.VisibleFraction = value, nameof(LPD_VisibleFraction));
        }
        public double LPD_RadiantFraction
        {
            get => _hbObj.Lighting.RadiantFraction;
            set => Set(() => _hbObj.Lighting.RadiantFraction = value, nameof(LPD_RadiantFraction));
        }
        public double LPD_ReturnAirFraction
        {
            get => _hbObj.Lighting.ReturnAirFraction;
            set => Set(() => _hbObj.Lighting.ReturnAirFraction = value, nameof(LPD_ReturnAirFraction));
        }
        #endregion

        #region ElectricEquipment
        public double EQP_WattsPerArea
        {
            get => _hbObj.ElectricEquipment.WattsPerArea;
            set => Set(() => _hbObj.ElectricEquipment.WattsPerArea = value, nameof(EQP_WattsPerArea));
        }
        public string EQP_Schedule
        {
            get => _hbObj.ElectricEquipment.Schedule;
            set => Set(() => _hbObj.ElectricEquipment.Schedule = value, nameof(EQP_Schedule));
        }
        public double EQP_LostFraction
        {
            get => _hbObj.ElectricEquipment.LostFraction;
            set => Set(() => _hbObj.ElectricEquipment.LostFraction = value, nameof(EQP_LostFraction));
        }
        public double EQP_RadiantFraction
        {
            get => _hbObj.ElectricEquipment.RadiantFraction;
            set => Set(() => _hbObj.ElectricEquipment.RadiantFraction = value, nameof(EQP_RadiantFraction));
        }
        public double EQP_LatentFraction
        {
            get => _hbObj.ElectricEquipment.LatentFraction;
            set => Set(() => _hbObj.ElectricEquipment.LatentFraction = value, nameof(EQP_LatentFraction));
        }
        #endregion

        #region GasEquipment
        public double GAS_WattsPerArea
        {
            get => _hbObj.GasEquipment.WattsPerArea;
            set => Set(() => _hbObj.GasEquipment.WattsPerArea = value, nameof(GAS_WattsPerArea));
        }
        public string GAS_Schedule
        {
            get => _hbObj.GasEquipment.Schedule;
            set => Set(() => _hbObj.GasEquipment.Schedule = value, nameof(GAS_Schedule));
        }
        public double GAS_LostFraction
        {
            get => _hbObj.GasEquipment.LostFraction;
            set => Set(() => _hbObj.GasEquipment.LostFraction = value, nameof(GAS_LostFraction));
        }
        public double GAS_RadiantFraction
        {
            get => _hbObj.GasEquipment.RadiantFraction;
            set => Set(() => _hbObj.GasEquipment.RadiantFraction = value, nameof(GAS_RadiantFraction));
        }
        public double GAS_LatentFraction
        {
            get => _hbObj.GasEquipment.LatentFraction;
            set => Set(() => _hbObj.GasEquipment.LatentFraction = value, nameof(GAS_LatentFraction));
        }
        #endregion

        #region Infiltration
        public double INF_FlowPerExteriorArea
        {
            get => _hbObj.Infiltration.FlowPerExteriorArea;
            set => Set(() => _hbObj.Infiltration.FlowPerExteriorArea = value, nameof(INF_FlowPerExteriorArea));
        }
        public string INF_Schedule
        {
            get => _hbObj.Infiltration.Schedule;
            set => Set(() => _hbObj.Infiltration.Schedule = value, nameof(INF_Schedule));
        }
        public double INF_VelocityCoefficient
        {
            get => _hbObj.Infiltration.VelocityCoefficient;
            set => Set(() => _hbObj.Infiltration.VelocityCoefficient = value, nameof(INF_VelocityCoefficient));
        }
        public double INF_TemperatureCoefficient
        {
            get => _hbObj.Infiltration.TemperatureCoefficient;
            set => Set(() => _hbObj.Infiltration.TemperatureCoefficient = value, nameof(INF_TemperatureCoefficient));
        }
        public double INF_ConstantCoefficient
        {
            get => _hbObj.Infiltration.ConstantCoefficient;
            set => Set(() => _hbObj.Infiltration.ConstantCoefficient = value, nameof(INF_ConstantCoefficient));
        }
        #endregion

        #region Ventilation
        public double VNT_AirChangesPerHour
        {
            get => _hbObj.Ventilation.AirChangesPerHour;
            set => Set(() => _hbObj.Ventilation.AirChangesPerHour = value, nameof(VNT_AirChangesPerHour));
        }
        public string VNT_Schedule
        {
            get => _hbObj.Ventilation.Schedule;
            set => Set(() => _hbObj.Ventilation.Schedule = value, nameof(VNT_Schedule));
        }
        public double VNT_FlowPerArea
        {
            get => _hbObj.Ventilation.FlowPerArea;
            set => Set(() => _hbObj.Ventilation.FlowPerArea = value, nameof(VNT_FlowPerArea));
        }
        public double VNT_FlowPerPerson
        {
            get => _hbObj.Ventilation.FlowPerPerson;
            set => Set(() => _hbObj.Ventilation.FlowPerPerson = value, nameof(VNT_FlowPerPerson));
        }
        public double VNT_FlowPerZone
        {
            get => _hbObj.Ventilation.FlowPerZone;
            set => Set(() => _hbObj.Ventilation.FlowPerZone = value, nameof(VNT_FlowPerZone));
        }
        #endregion

        #region Setpoint
        public string SPT_CoolingSchedule
        {
            get => _hbObj.Setpoint.CoolingSchedule;
            set => Set(() => _hbObj.Setpoint.CoolingSchedule = value, nameof(SPT_CoolingSchedule));
        }
        public string SPT_HeatingSchedule
        {
            get => _hbObj.Setpoint.HeatingSchedule;
            set => Set(() => _hbObj.Setpoint.HeatingSchedule = value, nameof(SPT_HeatingSchedule));
        }
        public string SPT_HumidifyingSchedule
        {
            get => _hbObj.Setpoint.HumidifyingSchedule;
            set => Set(() => _hbObj.Setpoint.HumidifyingSchedule = value, nameof(SPT_HumidifyingSchedule));
        }
        public string SPT_DehumidifyingSchedule
        {
            get => _hbObj.Setpoint.DehumidifyingSchedule;
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

        public void UpdatePeople(PeopleAbridged newObj)
        {
            this.hbObj.People = newObj;
            var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("PPL_")).Select(_ => _.Name);
            this.RefreshControls(propleProps);
        }
        public void UpdateLighting(LightingAbridged newObj)
        {
            this.hbObj.Lighting = newObj;
            var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("LPD_")).Select(_ => _.Name);
            this.RefreshControls(propleProps);
        }
        public void UpdateEquipment(ElectricEquipmentAbridged newObj)
        {
            this.hbObj.ElectricEquipment = newObj;
            var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("EQP_")).Select(_ => _.Name);
            this.RefreshControls(propleProps);
        }
        public void UpdateGasEquipment(GasEquipmentAbridged newObj)
        {
            this.hbObj.GasEquipment = newObj;
            var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("GAS_")).Select(_ => _.Name);
            this.RefreshControls(propleProps);
        }
        public void UpdateInfiltration(InfiltrationAbridged newObj)
        {
            this.hbObj.Infiltration = newObj;
            var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("INF_")).Select(_ => _.Name);
            this.RefreshControls(propleProps);
        }
        public void UpdateVentilation(VentilationAbridged newObj)
        {
            this.hbObj.Ventilation = newObj;
            var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("VNT_")).Select(_ => _.Name);
            this.RefreshControls(propleProps);
        }
        public void UpdateSetpoint(SetpointAbridged newObj)
        {
            this.hbObj.Setpoint = newObj;
            var propleProps = this.GetType().GetProperties().Where(_ => _.Name.StartsWith("SPT_")).Select(_ => _.Name);
            this.RefreshControls(propleProps);
        }

        private ProgramTypeViewModel()
        {
        }

    }



}
