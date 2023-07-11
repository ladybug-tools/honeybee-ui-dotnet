using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class OpsHVACSelectorViewModel : ViewModelBase
    {
        public List<string> HvacGroups => new List<string>() { "All Air (non DOAS)", "Heating Cooling with DOAS", "Heating Cooling Only" };


        private string _hvacGroup;
        public string HvacGroup
        {
            get => _hvacGroup ?? HvacGroups.First();
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _hvacGroup = value, nameof(HvacGroup));

                if (value == HvacGroups[0])
                {
                    HvacTypes = GetAllAirTypes();
                }
                else if (value == HvacGroups[1])
                {
                    HvacTypes = GetDOASTypes();
                }
                else if (value == HvacGroups[2])
                {
                    HvacTypes = GetOtherTypes();
                }
            }
        }

        private IEnumerable<Type> _hvacTypes;
        public IEnumerable<Type> HvacTypes
        {
            get => _hvacTypes ?? GetAllAirTypes();
            set
            {
                if (value == null)
                    return;
                Set(() => _hvacTypes = value, nameof(HvacTypes));
                HvacType = HvacTypes.FirstOrDefault();
            }
        }

        private Type _hvacType;
        public Type HvacType
        {
            get => _hvacType ?? HvacTypes.First();
            set
            {
                if (value == null)
                    return;
                Set(() => _hvacType = value, nameof(HvacType));
                
                try
                {
                    HvacTypeSummary = "";
                    HvacTypeSummary = HoneybeeSchema.SummaryAttribute.GetSummary(HVACTypeMapper[value]);
                }
                catch (Exception)
                {
                    // do nothing
                }
              
            }
        }

        private string _hvacTypeSummary;

        public string HvacTypeSummary
        {
            get => _hvacTypeSummary;
            set => Set(() => _hvacTypeSummary = value, nameof(HvacTypeSummary));
        }



        private Dialog_OpsHVACSelector _control;

        public OpsHVACSelectorViewModel(Dialog_OpsHVACSelector control)
        {
            _control = control;
            this.HvacGroup = HvacGroups[0];
        }

        private IEnumerable<Type> GetAllAirTypes()
        {
            
            var types = new List<Type>() 
            {
                typeof(VAVEquipmentType),
                typeof(PVAVEquipmentType),
                typeof(PSZEquipmentType),
                typeof(PTACEquipmentType),
                typeof(FurnaceEquipmentType),
            };

            return types;
        }

        private IEnumerable<Type> GetDOASTypes()
        {
            var types = new List<Type>()
            {
                typeof(FCUwithDOASEquipmentType),
                typeof(VRFwithDOASEquipmentType),
                typeof(WSHPwithDOASEquipmentType),
                typeof(RadiantwithDOASEquipmentType)
            };

            return types;
        }

        private IEnumerable<Type> GetOtherTypes()
        {
            var types = new List<Type>()
            {
                typeof(BaseboardEquipmentType),
                typeof(EvaporativeCoolerEquipmentType),
                typeof(FCUEquipmentType),
                typeof(GasUnitHeaterEquipmentType),
                typeof(ResidentialEquipmentType),
                typeof(VRFEquipmentType),
                typeof(WSHPEquipmentType),
                typeof(WindowACEquipmentType),
                typeof(RadiantEquipmentType),
            };

            return types;
        }


        public Dictionary<Type, string> HVACTypesDic
            = new Dictionary<Type, string>()
            {
                {  typeof(VAVEquipmentType), "Variable air volume (VAV)"},
                {  typeof(PVAVEquipmentType), "Packaged variable air volume (PVAV)"},
                {  typeof(PSZEquipmentType), "Packaged rooftop air conditioner (PSZ-AC)"},
                {  typeof(PTACEquipmentType), "Packaged terminal air conditioner (PTAC) or heat pump (PTHP)"},
                {  typeof(FurnaceEquipmentType), "Forced air furnace"},
                {  typeof(FCUwithDOASEquipmentType), "DOAS with fan coil unit"},
                {  typeof(VRFwithDOASEquipmentType), "DOAS with VRF"},
                {  typeof(WSHPwithDOASEquipmentType), "DOAS with water source heat pumps"},
                {  typeof(RadiantwithDOASEquipmentType), "DOAS with radiant systems"},
                {  typeof(BaseboardEquipmentType), "Baseboard"},
                {  typeof(EvaporativeCoolerEquipmentType), "Direct evaporative coolers"},
                {  typeof(FCUEquipmentType),"Fan coil systems"},
                {  typeof(GasUnitHeaterEquipmentType), "Gas unit heaters"},
                {  typeof(ResidentialEquipmentType), "Residential equipments"},
                {  typeof(VRFEquipmentType), "Variable refrigerant flow (VRF)"},
                {  typeof(WSHPEquipmentType), "Water source heat pumps"},
                {  typeof(WindowACEquipmentType), "Window AC"},
                {  typeof(RadiantEquipmentType), "Radiant equipments"},

            };

        private Dictionary<Type, Type> HVACTypeMapper
          = new Dictionary<Type, Type>()
          {
                {  typeof(VAVEquipmentType), typeof(VAV)},
                {  typeof(PVAVEquipmentType),typeof(PVAV)},
                {  typeof(PSZEquipmentType), typeof(PSZ)},
                {  typeof(PTACEquipmentType), typeof(PTAC)},
                {  typeof(FurnaceEquipmentType), typeof(ForcedAirFurnace)},
                {  typeof(FCUwithDOASEquipmentType), typeof(FCUwithDOASAbridged)},
                {  typeof(VRFwithDOASEquipmentType), typeof(VRFwithDOASAbridged)},
                {  typeof(WSHPwithDOASEquipmentType), typeof(WSHPwithDOASAbridged)},
                {  typeof(RadiantwithDOASEquipmentType), typeof(RadiantwithDOASAbridged)},
                {  typeof(BaseboardEquipmentType), typeof(Baseboard)},
                {  typeof(EvaporativeCoolerEquipmentType), typeof(EvaporativeCooler)},
                {  typeof(FCUEquipmentType),typeof(FCU)},
                {  typeof(GasUnitHeaterEquipmentType), typeof(GasUnitHeater)},
                {  typeof(ResidentialEquipmentType), typeof(Residential)},
                {  typeof(VRFEquipmentType), typeof(VRF)},
                {  typeof(WSHPEquipmentType), typeof(WSHP)},
                {  typeof(WindowACEquipmentType), typeof(WindowAC)},
                {  typeof(RadiantEquipmentType), typeof(Radiant)},

          };


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

   
        public HoneybeeSchema.Energy.IHvac GreateHvac()
        {
            var sysType = this.HvacType;

            var hvacType = HVACTypeMapper[sysType];
            //hvacType = existing == null ? hvacType : existing.GetType();
            var id = Guid.NewGuid().ToString().Substring(0, 8);
       
            var constructor = hvacType.GetConstructors().FirstOrDefault();
            var parameters = constructor.GetParameters().Select(_ => _.HasDefaultValue ? _.DefaultValue : default).ToArray();
            parameters[0] = $"{hvacType.Name}_{id}";
            var obj = constructor.Invoke(parameters);
            var sys = obj as HoneybeeSchema.Energy.IHvac;

            sys.DisplayName = $"{hvacType.Name} {id}";

            // validate
            (sys as OpenAPIGenBaseModel)?.IsValid(true);

            return sys;
        }


    }





}
