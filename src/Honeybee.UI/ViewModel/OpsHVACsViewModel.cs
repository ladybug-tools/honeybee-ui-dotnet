using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnitsNet;

namespace Honeybee.UI
{
    public class OpsHVACsViewModel : ViewModelBase
    {

        public IEnumerable<string> _vintages = Enum.GetNames(typeof(HoneybeeSchema.Vintages)).ToList();
        public IEnumerable<string> Vintages
        {
            get => _vintages;
            set
            {
                if (_vintages == value )
                    return;
                Set(() => _vintages = value, nameof(Vintages));
            }
        }

        private string _vintage;
        public string Vintage
        {
            get => _vintage ?? nameof(HoneybeeSchema.Vintages.ASHRAE_2019);
            set
            {
                if (_vintage == value || string.IsNullOrEmpty(value))
                    return;

                Set(() => _vintage = value, nameof(Vintage));
            }
        }

        private string _VintageTip;
        public string VintageTip
        {
            get => _VintageTip;
            set => Set(() => _VintageTip = value, nameof(VintageTip));
        }

    

        private IEnumerable<string> _hvacEquipmentTypes;
        public IEnumerable<string> HvacEquipmentTypes
        {
            get => _hvacEquipmentTypes;
            set
            {
                if (value == null)
                    return;
                Set(() => _hvacEquipmentTypes = value, nameof(HvacEquipmentTypes));
                HvacEquipmentType = HvacEquipmentTypes.FirstOrDefault();
            }
        }
        private string _hvacEquipmentType;
        public string HvacEquipmentType
        {
            get => _hvacEquipmentType ?? HvacEquipmentTypes.First();
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _hvacEquipmentType = value, nameof(HvacEquipmentType));
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _name = value, nameof(Name));
            }
        }

        private string _NameTip;
        public string NameTip
        {
            get => _NameTip;
            set => Set(() => _NameTip = value, nameof(NameTip));
        }

        private IEnumerable<string> _economizers = Enum.GetNames(typeof(AllAirEconomizerType)).ToList();
        public IEnumerable<string> Economizers => _economizers;

        private string _economizer = dummy.EconomizerType.ToString();
        public string Economizer
        {
            get => _economizer ?? Economizers.First();
            set => Set(() => _economizer = value, nameof(Economizer));
        }

        private string _EconomizerTip;
        public string EconomizerTip
        {
            get => _EconomizerTip;
            set => Set(() => _EconomizerTip = value, nameof(EconomizerTip));
        }

        private bool _economizerVisable = true;
        public bool EconomizerVisable
        {
            get => _economizerVisable;
            set => Set(() => _economizerVisable = value, nameof(EconomizerVisable));
        }

        private bool _dcvVisable = false;
        public bool DcvVisable
        {
            get => _dcvVisable;
            set => Set(() => _dcvVisable = value, nameof(DcvVisable));
        }

        private bool _dcvChecked = dummyDoas.DemandControlledVentilation;
        public bool DcvChecked
        {
            get => _dcvChecked;
            set => Set(() => _dcvChecked = value, nameof(DcvChecked));
        }

        private string _DCVTip;
        public string DCVTip
        {
            get => _DCVTip;
            set => Set(() => _DCVTip = value, nameof(DCVTip));
        }

        private double _sensibleHR = dummy.SensibleHeatRecovery;
        public double SensibleHR
        {
            get => _sensibleHR;
            set => Set(() => _sensibleHR = value, nameof(SensibleHR));
        }

       
        private bool _sensibleHRVisable = true;
        public bool SensibleHRVisable
        {
            get => _sensibleHRVisable;
            set => Set(() => _sensibleHRVisable = value, nameof(SensibleHRVisable));
        }

        private string _SensibleHRTip;
        public string SensibleHRTip
        {
            get => _SensibleHRTip;
            set => Set(() => _SensibleHRTip = value, nameof(SensibleHRTip));
        }

        private double _latentHR = dummy.LatentHeatRecovery;
        public double LatentHR
        {
            get => _latentHR;
            set => Set(() => _latentHR = value, nameof(LatentHR));
        }


        private bool _latentHRVisable = true;
        public bool LatentHRVisable
        {
            get => _latentHRVisable;
            set => Set(() => _latentHRVisable = value, nameof(LatentHRVisable));
        }

        private string _LatentHRTip;
        public string LatentHRTip
        {
            get => _LatentHRTip;
            set => Set(() => _LatentHRTip = value, nameof(LatentHRTip));
        }

        // AvailabilitySchedule
        private bool _AvaliabilityVisable = false;
        public bool AvaliabilityVisable
        {
            get => _AvaliabilityVisable;
            set => Set(() => _AvaliabilityVisable = value, nameof(AvaliabilityVisable));
        }

        private string _scheduleID;

        private OptionalButtonViewModel _AvaliabilitySchedule;
        public OptionalButtonViewModel AvaliabilitySchedule
        {
            get => _AvaliabilitySchedule;
            set { this.Set(() => _AvaliabilitySchedule = value, nameof(AvaliabilitySchedule)); }
        }

        private string _AvaliabilityTip;
        public string AvaliabilityTip
        {
            get => _AvaliabilityTip;
            set => Set(() => _AvaliabilityTip = value, nameof(AvaliabilityTip));
        }

        private bool _RadiantVisable;
        public bool RadiantVisable
        {
            get => _RadiantVisable;
            set => Set(() => _RadiantVisable = value, nameof(RadiantVisable));
        }

        private string _RadiantFaceTip;
        public string RadiantFaceTip
        {
            get => _RadiantFaceTip;
            set => Set(() => _RadiantFaceTip = value, nameof(RadiantFaceTip));
        }

        private RadiantFaceTypes _RadiantFaceType = dummyRad.RadiantFaceType;
        public RadiantFaceTypes RadiantFaceType
        {
            get => _RadiantFaceType;
            set { this.Set(() => _RadiantFaceType = value, nameof(RadiantFaceType)); }
        }


        private double _MinOptTime = dummyRad.MinimumOperationTime;
        public double MinOptTime
        {
            get => _MinOptTime;
            set => Set(() => _MinOptTime = value, nameof(MinOptTime));
        }

        private string _MinOptTimeTip;
        public string MinOptTimeTip
        {
            get => _MinOptTimeTip;
            set => Set(() => _MinOptTimeTip = value, nameof(MinOptTimeTip));
        }

        private double _SwitchTime = dummyRad.SwitchOverTime;
        public double SwitchTime
        {
            get => _SwitchTime;
            set => Set(() => _SwitchTime = value, nameof(SwitchTime));
        }

        private string _SwitchTimeTip;
        public string SwitchTimeTip
        {
            get => _SwitchTimeTip;
            set => Set(() => _SwitchTimeTip = value, nameof(SwitchTimeTip));
        }

        private HoneybeeSchema.Energy.IHvac _hvac;
        private static VAV dummy = new VAV("dummy");
        private static FCUwithDOASAbridged dummyDoas = new FCUwithDOASAbridged("doas");
        private static RadiantwithDOASAbridged dummyRad = new RadiantwithDOASAbridged("rad");


        private ModelEnergyProperties _libSource;
        private Dialog_OpsHVACs _control;

       
        public OpsHVACsViewModel(ModelEnergyProperties libSource, HoneybeeSchema.Energy.IHvac hvac, Dialog_OpsHVACs control)
        {
            if (hvac == null)
                throw new ArgumentNullException(nameof(hvac));

            _hvac = hvac;
            _libSource = libSource;
            _control = control;
            UpdateUI_Group();


            var hvacClassType = hvac.GetType();

            // vintage
            var vintage = hvacClassType.GetProperty(nameof(dummy.Vintage))?.GetValue(hvac);
            if (vintage is Vintages v)
                this.Vintage = v.ToString();
            this.VintageTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hvacClassType, nameof(dummy.Vintage)));

         
            // equipment type
            var eqpType = hvacClassType.GetProperty(nameof(dummy.EquipmentType))?.GetValue(hvac);
            this.HvacEquipmentTypes = GetHVACEquipmentTypes(eqpType.GetType());
            this.HvacEquipmentType = eqpType.ToString();


            this.Name = hvac.DisplayName;
            this.NameTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hvacClassType, nameof(dummy.DisplayName)));

            // LatentHeatRecovery
            var lat = hvacClassType?.GetProperty(nameof(dummy.LatentHeatRecovery))?.GetValue(hvac);
            if (lat != null)
            {
                if (double.TryParse(lat.ToString(), out var latValue))
                    LatentHR = latValue;
            }
            LatentHRTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hvacClassType, nameof(dummy.LatentHeatRecovery)));

            // SensibleHeatRecovery
            var sen = hvacClassType?.GetProperty(nameof(dummy.SensibleHeatRecovery))?.GetValue(hvac);
            if (sen != null)
            {
                if (double.TryParse(sen.ToString(), out var senValue))
                    SensibleHR = senValue;
            }
            SensibleHRTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hvacClassType, nameof(dummy.SensibleHeatRecovery)));

            // Economizer
            Economizer = hvacClassType.GetProperty(nameof(dummy.EconomizerType))?.GetValue(hvac)?.ToString();
            EconomizerTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hvacClassType, nameof(dummy.EconomizerType)));
      

            // DCV
            var dcv = hvacClassType.GetProperty(nameof(dummy.DemandControlledVentilation))?.GetValue(hvac)?.ToString();
            if (dcv != null)
            {
                var hasDcvValue = bool.TryParse(dcv, out var dcvOn);
                DcvChecked = hasDcvValue ? dcvOn : false;
            }
            DCVTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hvacClassType, nameof(dummy.DemandControlledVentilation)));

            //AvaliabilitySchedule

            this.AvaliabilitySchedule = new OptionalButtonViewModel((n) => _scheduleID = n?.Identifier);

            var schId = hvacClassType.GetProperty(nameof(dummyDoas.DoasAvailabilitySchedule))?.GetValue(hvac)?.ToString();
            var sch = libSource.ScheduleList.FirstOrDefault(_ => _.Identifier == schId);
            sch = sch ?? GetDummyScheduleObj(schId);
            this.AvaliabilitySchedule.SetPropetyObj(sch);
            this.AvaliabilityTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(dummyDoas.GetType(), nameof(dummyDoas.DoasAvailabilitySchedule)));


            //Radiant system
            var hasRad = Enum.TryParse<RadiantFaceTypes>(hvacClassType.GetProperty(nameof(dummyRad.RadiantFaceType))?.GetValue(hvac)?.ToString(), out var radFaceType); 
        
            if (hasRad)
            {
                var radType = hvac.GetType();

                //RadiantFaceType
                this.RadiantFaceType = radFaceType;

                //SwitchOverTime
                var sotProp = radType.GetProperty(nameof(dummyRad.SwitchOverTime));
                var sot = sotProp?.GetValue(hvac)?.ToString();
                double.TryParse(sot, out var switchTime);
                this.SwitchTime = switchTime;

                //MinimumOperationTime
                var mot = radType.GetProperty(nameof(dummyRad.MinimumOperationTime))?.GetValue(hvac)?.ToString();
                double.TryParse(mot, out var mintime);
                this.MinOptTime = mintime;

                this.RadiantFaceTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(radType, nameof(dummyRad.RadiantFaceType)));
                this.SwitchTimeTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(radType, nameof(dummyRad.SwitchOverTime)));
                this.MinOptTimeTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(radType, nameof(dummyRad.MinimumOperationTime)));
            }

            this.RadiantVisable = hasRad;
        }


        private void UpdateUI_Group()
        {
            this.RadiantVisable = this.IsRadiantSystem(_hvac);

            if (this.IsAllAirGroup(_hvac))
            {
                EconomizerVisable = true;
                LatentHRVisable = true;
                SensibleHRVisable = true;
                DcvVisable = true;
                AvaliabilityVisable = false;
            }
            else if (this.IsDOASGroup(_hvac))
            {
                EconomizerVisable = false;
                LatentHRVisable = true;
                SensibleHRVisable = true;
                DcvVisable = true;
                AvaliabilityVisable = true;
            }
            else if ( this.IsAllAirZoneGroup(_hvac) || this.IsOtherGroup(_hvac))
            {
                EconomizerVisable = false;
                LatentHRVisable = false;
                SensibleHRVisable = false;
                DcvVisable = false;
                AvaliabilityVisable = false;
            }
            else
            {
                throw new ArgumentException($"{_hvac.GetType().Name} is not a supported HVAC system, please contact developers!");
            }

        }


        private bool IsRadiantSystem(HoneybeeSchema.Energy.IHvac hvac)
        {
            var isGroup = hvac is RadiantwithDOASAbridged;
            isGroup |= hvac is Radiant;
            return isGroup;
        }

      
        private bool IsAllAirGroup(HoneybeeSchema.Energy.IHvac hvac)
        {
            var isGroup = hvac is VAV;
            isGroup |= hvac is PVAV;
            isGroup |= hvac is PSZ;
            return isGroup;
        }

        private bool IsAllAirZoneGroup(HoneybeeSchema.Energy.IHvac hvac)
        {
            var isGroup = hvac is PTAC;
            isGroup |= hvac is ForcedAirFurnace;
            return isGroup;
        }



        private bool IsDOASGroup(HoneybeeSchema.Energy.IHvac hvac)
        {
            var isDoas = hvac is FCUwithDOASAbridged;
            isDoas |= hvac is VRFwithDOASAbridged;
            isDoas |= hvac is WSHPwithDOASAbridged;
            isDoas |= hvac is RadiantwithDOASAbridged;
            return isDoas;
        }
      

        private bool IsOtherGroup(HoneybeeSchema.Energy.IHvac hvac)
        {
            var isGroup = hvac is Baseboard;
            isGroup |= hvac is EvaporativeCooler;
            isGroup |= hvac is FCU;
            isGroup |= hvac is GasUnitHeater;
            isGroup |= hvac is Residential;
            isGroup |= hvac is VRF;
            isGroup |= hvac is WSHP;
            isGroup |= hvac is WindowAC;
            isGroup |= hvac is Radiant;
            return isGroup;
        }

      

        //private Dictionary<Type, Type> HVACEquipmentTypeMapper
        //  = new Dictionary<Type, Type>()
        //  {
        //        {  typeof(VAVEquipmentType), typeof(VAV)},
        //        {  typeof(PVAVEquipmentType),typeof(PVAV)},
        //        {  typeof(PSZEquipmentType), typeof(PSZ)},
        //        {  typeof(PTACEquipmentType), typeof(PTAC)},
        //        {  typeof(FurnaceEquipmentType), typeof(ForcedAirFurnace)},
        //        {  typeof(FCUwithDOASEquipmentType), typeof(FCUwithDOASAbridged)},
        //        {  typeof(VRFwithDOASEquipmentType), typeof(VRFwithDOASAbridged)},
        //        {  typeof(WSHPwithDOASEquipmentType), typeof(WSHPwithDOASAbridged)},
        //        {  typeof(RadiantwithDOASEquipmentType), typeof(RadiantwithDOASAbridged)},
        //        {  typeof(BaseboardEquipmentType), typeof(Baseboard)},
        //        {  typeof(EvaporativeCoolerEquipmentType), typeof(EvaporativeCooler)},
        //        {  typeof(FCUEquipmentType),typeof(FCU)},
        //        {  typeof(GasUnitHeaterEquipmentType), typeof(GasUnitHeater)},
        //        {  typeof(ResidentialEquipmentType), typeof(Residential)},
        //        {  typeof(VRFEquipmentType), typeof(VRF)},
        //        {  typeof(WSHPEquipmentType), typeof(WSHP)},
        //        {  typeof(WindowACEquipmentType), typeof(WindowAC)},
        //        {  typeof(RadiantEquipmentType), typeof(Radiant)},

        //  };

        public Dictionary<string, string> HVACsDic => HVACUserFriendlyNamesDic;

        public static Dictionary<string, string> HVACUserFriendlyNamesDic
            = new Dictionary<string, string>()
            {
                { string.Empty,string.Empty},
                { nameof(IdealAirSystemAbridged),"Ideal air load system"},
                { "VAV_Chiller_Boiler","VAV chiller with gas boiler reheat"},
                { "VAV_Chiller_ASHP","VAV chiller with central air source heat pump reheat"},
                { "VAV_Chiller_DHW","VAV chiller with district hot water reheat"},
                { "VAV_Chiller_PFP","VAV chiller with PFP boxes"},
                { "VAV_Chiller_GasCoil","VAV chiller with gas coil reheat"},
                { "VAV_ACChiller_Boiler","VAV air-cooled chiller with gas boiler reheat"},
                { "VAV_ACChiller_ASHP","VAV air-cooled chiller with central air source heat pump reheat"},
                { "VAV_ACChiller_DHW","VAV air-cooled chiller with district hot water reheat"},
                { "VAV_ACChiller_PFP","VAV air-cooled chiller with PFP boxes"},
                { "VAV_ACChiller_GasCoil","VAV air-cooled chiller with gas coil reheat"},
                { "VAV_DCW_Boiler","VAV district chilled water with gas boiler reheat"},
                { "VAV_DCW_ASHP","VAV district chilled water with central air source heat pump reheat"},
                { "VAV_DCW_DHW","VAV district chilled water with district hot water reheat"},
                { "VAV_DCW_PFP","VAV district chilled water with PFP boxes"},
                { "VAV_DCW_GasCoil","VAV district chilled water with gas coil reheat"},
                { "PVAV_Boiler","PVAV with gas boiler reheat"},
                { "PVAV_ASHP","PVAV with central air source heat pump reheat"},
                { "PVAV_DHW","PVAV with district hot water reheat"},
                { "PVAV_PFP","PVAV with PFP boxes"},
                { "PVAV_BoilerElectricReheat","PVAV with gas heat with electric reheat"},
                { "PSZAC_ElectricBaseboard","PSZ-AC with baseboard electric"},
                { "PSZAC_BoilerBaseboard","PSZ-AC with baseboard gas boiler"},
                { "PSZAC_DHWBaseboard","PSZ-AC with baseboard district hot water"},
                { "PSZAC_GasHeaters","PSZ-AC with gas unit heaters"},
                { "PSZAC_ElectricCoil","PSZ-AC with electric coil"},
                { "PSZAC_GasCoil","PSZ-AC with gas coil"},
                { "PSZAC_Boiler","PSZ-AC with gas boiler"},
                { "PSZAC_ASHP","PSZ-AC with central air source heat pump"},
                { "PSZAC_DHW","PSZ-AC with district hot water"},
                { "PSZAC","PSZ-AC with no heat"},
                { "PSZAC_DCW_ElectricBaseboard","PSZ-AC district chilled water with baseboard electric"},
                { "PSZAC_DCW_BoilerBaseboard","PSZ-AC district chilled water with baseboard gas boiler"},
                { "PSZAC_DCW_GasHeaters","PSZ-AC district chilled water with gas unit heaters"},
                { "PSZAC_DCW_ElectricCoil","PSZ-AC district chilled water with electric coil"},
                { "PSZAC_DCW_GasCoil","PSZ-AC district chilled water with gas coil"},
                { "PSZAC_DCW_Boiler","PSZ-AC district chilled water with gas boiler"},
                { "PSZAC_DCW_ASHP","PSZ-AC district chilled water with central air source heat pump"},
                { "PSZAC_DCW_DHW","PSZ-AC district chilled water with district hot water"},
                { "PSZAC_DCW","PSZ-AC district chilled water with no heat"},
                { "PSZHP","Packaged rooftop heat pump (PSZHP)"},
                { "PTAC_ElectricBaseboard","PTAC with baseboard electric"},
                { "PTAC_BoilerBaseboard","PTAC with baseboard gas boiler"},
                { "PTAC_DHWBaseboard","PTAC with baseboard district hot water"},
                { "PTAC_GasHeaters","PTAC with gas unit heaters"},
                { "PTAC_ElectricCoil","PTAC with electric coil"},
                { "PTAC_GasCoil","PTAC with gas coil"},
                { "PTAC_Boiler","PTAC with gas boiler"},
                { "PTAC_ASHP","PTAC with central air source heat pump"},
                { "PTAC_DHW","PTAC with district hot water"},
                { "PTAC","PTAC with no heat"},
                { "PTHP","Packaged terminal heat pump (PTHP)"},
                { "Furnace","Forced air furnace"},
                { "Furnace_Electric", "Forced air electric furnace"  },

                { "DOAS_FCU_Chiller_Boiler","DOAS with fan coil chiller with boiler"},
                { "DOAS_FCU_Chiller_ASHP","DOAS with fan coil chiller with central air source heat pump"},
                { "DOAS_FCU_Chiller_DHW","DOAS with fan coil chiller with district hot water"},
                { "DOAS_FCU_Chiller_ElectricBaseboard","DOAS with fan coil chiller with baseboard electric"},
                { "DOAS_FCU_Chiller_GasHeaters","DOAS with fan coil chiller with gas unit heaters"},
                { "DOAS_FCU_Chiller","DOAS with fan coil chiller with no heat"},
                { "DOAS_FCU_ACChiller_Boiler","DOAS with fan coil air-cooled chiller with boiler"},
                { "DOAS_FCU_ACChiller_ASHP","DOAS with fan coil air-cooled chiller with central air source heat pump"},
                { "DOAS_FCU_ACChiller_DHW","DOAS with fan coil air-cooled chiller with district hot water"},
                { "DOAS_FCU_ACChiller_ElectricBaseboard","DOAS with fan coil air-cooled chiller with baseboard electric"},
                { "DOAS_FCU_ACChiller_GasHeaters","DOAS with fan coil air-cooled chiller with gas unit heaters"},
                { "DOAS_FCU_ACChiller","DOAS with fan coil air-cooled chiller with no heat"},
                { "DOAS_FCU_DCW_Boiler","DOAS with fan coil district chilled water with boiler"},
                { "DOAS_FCU_DCW_ASHP","DOAS with fan coil district chilled water with central air source heat pump"},
                { "DOAS_FCU_DCW_DHW","DOAS with fan coil district chilled water with district hot water"},
                { "DOAS_FCU_DCW_ElectricBaseboard","DOAS with fan coil district chilled water with baseboard electric"},
                { "DOAS_FCU_DCW_GasHeaters","DOAS with fan coil district chilled water with gas unit heaters"},
                { "DOAS_FCU_DCW","DOAS with fan coil district chilled water with no heat"},
                { "DOAS_VRF","DOAS with VRF"},
                { "DOAS_WSHP_FluidCooler_Boiler","DOAS with water source heat pumps fluid cooler with boiler"},
                { "DOAS_WSHP_CoolingTower_Boiler","DOAS with water source heat pumps cooling tower with boiler"},
                { "DOAS_WSHP_GSHP","DOAS with water source heat pumps with ground source heat pump"},
                { "DOAS_WSHP_DCW_DHW","DOAS with water source heat pumps district chilled water with district hot water"},

                { "DOAS_Radiant_Chiller_Boiler","DOAS with low temperature radiant chiller with boiler" },
                { "DOAS_Radiant_Chiller_ASHP","DOAS with low temperature radiant chiller with air source heat pump" },
                { "DOAS_Radiant_Chiller_DHW", "DOAS with low temperature radiant chiller with district hot water"},
                { "DOAS_Radiant_ACChiller_Boiler", "DOAS with low temperature radiant air-cooled chiller with boiler"},
                { "DOAS_Radiant_ACChiller_ASHP", "DOAS with low temperature radiant air-cooled chiller with air source heat pump" },
                { "DOAS_Radiant_ACChiller_DHW",  "DOAS with low temperature radiant air-cooled chiller with district hot water"},
                { "DOAS_Radiant_DCW_Boiler","DOAS with low temperature radiant district chilled water with boiler"},
                { "DOAS_Radiant_DCW_ASHP", "DOAS with low temperature radiant district chilled water with air source heat pump"},
                { "DOAS_Radiant_DCW_DHW", "DOAS with low temperature radiant district chilled water with district hot water" },

                { "ElectricBaseboard","Baseboard electric"},
                { "BoilerBaseboard","Baseboard gas boiler"},
                { "ASHPBaseboard","Baseboard central air source heat pump"},
                { "DHWBaseboard","Baseboard district hot water"},
                { "EvapCoolers_ElectricBaseboard","Direct evap coolers with baseboard electric"},
                { "EvapCoolers_BoilerBaseboard","Direct evap coolers with baseboard gas boiler"},
                { "EvapCoolers_ASHPBaseboard","Direct evap coolers with baseboard central air source heat pump"},
                { "EvapCoolers_DHWBaseboard","Direct evap coolers with baseboard district hot water"},
                { "EvapCoolers_Furnace","Direct evap coolers with forced air furnace"},
                { "EvapCoolers_UnitHeaters","Direct evap coolers with gas unit heaters"},
                { "EvapCoolers","Direct evap coolers with no heat"},
                { "FCU_Chiller_Boiler","Fan coil chiller with boiler"},
                { "FCU_Chiller_ASHP","Fan coil chiller with central air source heat pump"},
                { "FCU_Chiller_DHW","Fan coil chiller with district hot water"},
                { "FCU_Chiller_ElectricBaseboard","Fan coil chiller with baseboard electric"},
                { "FCU_Chiller_GasHeaters","Fan coil chiller with gas unit heaters"},
                { "FCU_Chiller","Fan coil chiller with no heat"},
                { "FCU_ACChiller_Boiler","Fan coil air-cooled chiller with boiler"},
                { "FCU_ACChiller_ASHP","Fan coil air-cooled chiller with central air source heat pump"},
                { "FCU_ACChiller_DHW","Fan coil air-cooled chiller with district hot water"},
                { "FCU_ACChiller_ElectricBaseboard","Fan coil air-cooled chiller with baseboard electric"},
                { "FCU_ACChiller_GasHeaters","Fan coil air-cooled chiller with gas unit heaters"},
                { "FCU_ACChiller","Fan coil air-cooled chiller with no heat"},
                { "FCU_DCW_Boiler","Fan coil district chilled water with boiler"},
                { "FCU_DCW_ASHP","Fan coil district chilled water with central air source heat pump"},
                { "FCU_DCW_DHW","Fan coil district chilled water with district hot water"},
                { "FCU_DCW_ElectricBaseboard","Fan coil district chilled water with baseboard electric"},
                { "FCU_DCW_GasHeaters","Fan coil district chilled water with gas unit heaters"},
                { "FCU_DCW","Fan coil district chilled water with no heat"},
                { "GasHeaters","Gas unit heaters"},
                { "ResidentialAC_ElectricBaseboard","Residential AC with baseboard electric"},
                { "ResidentialAC_BoilerBaseboard","Residential AC with baseboard gas boiler"},
                { "ResidentialAC_ASHPBaseboard","Residential AC with baseboard central air source heat pump"},
                { "ResidentialAC_DHWBaseboard","Residential AC with baseboard district hot water"},
                { "ResidentialAC_ResidentialFurnace","Residential AC with residential forced air furnace"},
                { "ResidentialAC","Residential AC with no heat"},
                { "ResidentialHP","Residential heat pump"},
                { "ResidentialHPNoCool","Residential heat pump with no cooling"},
                { "ResidentialFurnace","Residential forced air furnace"},
                { "VRF","VRF"},
                { "WSHP_FluidCooler_Boiler","Water source heat pumps fluid cooler with boiler"},
                { "WSHP_CoolingTower_Boiler","Water source heat pumps cooling tower with boiler"},
                { "WSHP_GSHP","Water source heat pumps with ground source heat pump"},
                { "WSHP_DCW_DHW","Water source heat pumps district chilled water with district hot water"},
                { "WindowAC_ElectricBaseboard","Window AC with baseboard electric"},
                { "WindowAC_BoilerBaseboard","Window AC with baseboard gas boiler"},
                { "WindowAC_ASHPBaseboard","Window AC with baseboard central air source heat pump"},
                { "WindowAC_DHWBaseboard","Window AC with baseboard district hot water"},
                { "WindowAC_Furnace","Window AC with forced air furnace"},
                { "WindowAC_GasHeaters","Window AC with unit heaters"},
                { "WindowAC","Window AC with no heat"},
                { "Radiant_Chiller_Boiler", "Low temperature radiant chiller with boiler"},
                { "Radiant_Chiller_ASHP", "Low temperature radiant chiller with air source heat pump"},
                { "Radiant_Chiller_DHW", "Low temperature radiant chiller with district hot water"},
                { "Radiant_ACChiller_Boiler", "Low temperature radiant air-cooled chiller with boiler"},
                { "Radiant_ACChiller_ASHP", "Low temperature radiant air-cooled chiller with air source heat pump"},
                { "Radiant_ACChiller_DHW",  "Low temperature radiant air-cooled chiller with district hot water"},
                { "Radiant_DCW_Boiler",  "Low temperature radiant district chilled water with boiler"},
                { "Radiant_DCW_ASHP", "Low temperature radiant district chilled water with air source heat pump"},
                { "Radiant_DCW_DHW", "Low temperature radiant district chilled water with district hot water"},

            };

        private IEnumerable<string> GetHVACEquipmentTypes(Type HVACType)
        {
            //var assem = typeof(VAVEquipmentType).Assembly;
            //var enumType = assem.GetType(HVACTypeName, false);
            //enumType = assem.GetType("HoneybeeSchema." + HVACTypeName);
            var names = Enum.GetNames(HVACType);

            return names;
        }


        public HoneybeeSchema.Energy.IHvac GreateHvac()
        {
            var vintage = Enum.Parse( typeof(Vintages), this.Vintage);

            var sys = _hvac;
            var tp = sys.GetType();

            // assign Name
            tp.GetProperty(nameof(dummy.DisplayName)).SetValue(sys, this.Name);

            // assign Vintage
            tp.GetProperty(nameof(dummy.Vintage)).SetValue(sys, vintage);

            // equipment type 
            var prop = tp.GetProperty(nameof(dummy.EquipmentType));
            var hvacSysType = Enum.Parse(prop.PropertyType, this.HvacEquipmentType);
            prop.SetValue(sys, hvacSysType);

            // SensibleHeatRecovery
            var sen = this.SensibleHR;
            // LatentHeatRecovery
            var lat = this.LatentHR;

            if (this.IsAllAirGroup(_hvac))     // All air 
            {
                // economizer
                var economizer = Enum.Parse(typeof(AllAirEconomizerType), this.Economizer);
                tp.GetProperty(nameof(dummy.EconomizerType)).SetValue(sys, economizer);

                // SensibleHeatRecovery
                tp.GetProperty(nameof(dummy.SensibleHeatRecovery)).SetValue(sys, sen);
                // LatentHeatRecovery
                tp.GetProperty(nameof(dummy.LatentHeatRecovery)).SetValue(sys, lat);

                // DCV
                tp.GetProperty(nameof(dummy.DemandControlledVentilation)).SetValue(sys, this.DcvChecked);

            }
            else if (this.IsDOASGroup(_hvac))      // DOAS
            {
                // SensibleHeatRecovery
                tp.GetProperty(nameof(dummy.SensibleHeatRecovery)).SetValue(sys, sen);
                // LatentHeatRecovery
                tp.GetProperty(nameof(dummy.LatentHeatRecovery)).SetValue(sys, lat);

                // DCV
                tp.GetProperty(nameof(dummy.DemandControlledVentilation)).SetValue(sys, this.DcvChecked);

                //AvaliabilitySchedule
                tp.GetProperty(nameof(dummyDoas.DoasAvailabilitySchedule)).SetValue(sys, this._scheduleID);

            }
            else if (IsRadiantSystem(sys))
            {
                //RadiantFaceType
                tp.GetProperty(nameof(dummyRad.RadiantFaceType)).SetValue(sys, this.RadiantFaceType);

                //SwitchOverTime
                tp.GetProperty(nameof(dummyRad.SwitchOverTime)).SetValue(sys, this.SwitchTime);

                //MinimumOperationTime
                tp.GetProperty(nameof(dummyRad.MinimumOperationTime)).SetValue(sys, this.MinOptTime);

            }

            // validate
            (sys as OpenAPIGenBaseModel)?.IsValid(true);
           
            return sys;
        }



        public RelayCommand AvaliabilityCommand => new RelayCommand(() =>
        {
            var lib = _libSource;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                this.AvaliabilitySchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand RemoveAvaliabilityCommand => new RelayCommand(() =>
        {
            this.AvaliabilitySchedule.SetPropetyObj(null);
        });


    }





}
