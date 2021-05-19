using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Honeybee.UI
{
    public class OpsHVACsViewModel : ViewModelBase
    {

        //private IEnumerable<string> VintageJsonPaths => EnergyLibrary.BuildingVintages;
        //public IEnumerable<string> VintageNames => VintageJsonPaths.Select(_ => System.IO.Path.GetFileNameWithoutExtension(_).Replace("_registry", ""));
        //private string DefaultVintageName => VintageNames.First(_ => _.Contains("2013"));

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
            get => _vintage ?? nameof(HoneybeeSchema.Vintages.ASHRAE_2013);
            set
            {
                if (_vintage == value || string.IsNullOrEmpty(value))
                    return;

                Set(() => _vintage = value, nameof(Vintage));
            }
        }

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
                    EconomizerVisable = true;
                    LatentHRVisable = true;
                    SensibleHRVisable = true;
                }
                else if (value == HvacGroups[1])
                {
                    HvacTypes = GetDOASTypes();
                    EconomizerVisable = false;
                    LatentHRVisable = true;
                    SensibleHRVisable = true;
                }
                else if (value == HvacGroups[2])
                {
                    HvacTypes = GetOtherTypes();
                    EconomizerVisable = false;
                    LatentHRVisable = false;
                    SensibleHRVisable = false;
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
                HvacEquipmentTypes = GetHVACTypes(value);
                this.Name = $"{this.HVACTypesDic[this.HvacType]} {Guid.NewGuid().ToString().Substring(0, 5)}";
            }
        }

        private IEnumerable<string> _hvacEquipmentTypes;
        public IEnumerable<string> HvacEquipmentTypes
        {
            get => _hvacEquipmentTypes ?? GetHVACTypes(HvacType);
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
            get => _name ?? $"{this.HVACTypesDic[this.HvacType]} {Guid.NewGuid().ToString().Substring(0,5)}";
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _name = value, nameof(Name));
            }
        }

        private IEnumerable<string> _economizers = Enum.GetNames(typeof(AllAirEconomizerType)).ToList();
        public IEnumerable<string> Economizers => _economizers;

        private string _economizer;
        public string Economizer
        {
            get => _economizer ?? Economizers.First();
            set
            {
                Set(() => _economizer = value, nameof(Economizer));
            }
        }

        private bool _economizerVisable = true;
        public bool EconomizerVisable
        {
            get => _economizerVisable;
            set
            {
                Set(() => _economizerVisable = value, nameof(EconomizerVisable));
            }
        }

        private double _sensibleHR = 0.5;
        public double SensibleHR
        {
            get => _sensibleHR;
            set
            {
                Set(() => _sensibleHR = value, nameof(SensibleHR));
            }
        }

       
        private bool _sensibleHRVisable = true;
        public bool SensibleHRVisable
        {
            get => _sensibleHRVisable;
            set
            {
                Set(() => _sensibleHRVisable = value, nameof(SensibleHRVisable));
            }
        }


        private double _latentHR = 0.5;
        public double LatentHR
        {
            get => _latentHR;
            set
            {
                Set(() => _latentHR = value, nameof(LatentHR));
            }
        }

     

      

        private bool _latentHRVisable = true;
        public bool LatentHRVisable
        {
            get => _latentHRVisable;
            set
            {
                Set(() => _latentHRVisable = value, nameof(LatentHRVisable));
            }
        }

        public OpsHVACsViewModel(HoneybeeSchema.Energy.IHvac hvac)
        {
            if (hvac == null)
                return;

            this.Name = hvac.DisplayName;

            var dummy = new VAV("");
            // vintage
            var vintage = hvac.GetType().GetProperty(nameof(dummy.Vintage)).GetValue(hvac);
            if (vintage is Vintages v)
                this.Vintage = v.ToString();

            // equipment type
            var eqpType = hvac.GetType().GetProperty(nameof(dummy.EquipmentType)).GetValue(hvac);
            this.HvacEquipmentType = eqpType.ToString();

            // LatentHeatRecovery
            var lat = hvac.GetType().GetProperty(nameof(dummy.LatentHeatRecovery)).GetValue(hvac) as AnyOf<Autosize, double>;
            if (lat != null)
            {
                if (double.TryParse(lat.Obj.ToString(), out var latValue))
                    LatentHR = latValue;
            }

            // SensibleHeatRecovery
            var sen = hvac.GetType().GetProperty(nameof(dummy.SensibleHeatRecovery)).GetValue(hvac) as AnyOf<Autosize, double>;
            if (sen != null)
            {
                if (double.TryParse(sen.Obj.ToString(), out var senValue))
                    SensibleHR = senValue;
            }


            if (this.IsAllAirGroup(hvac))
            {
                EconomizerVisable = true;
                LatentHRVisable = true;
                SensibleHRVisable = true;

                // Economizer
                Economizer = hvac.GetType().GetProperty(nameof(dummy.EconomizerType)).GetValue(hvac).ToString();
            
            }
            else if (this.IsDOASGroup(hvac))
            {
                EconomizerVisable = false;
                LatentHRVisable = true;
                SensibleHRVisable = true;

            }
            else if (this.IsOtherGroup(hvac))
            {
                EconomizerVisable = false;
                LatentHRVisable = false;
                SensibleHRVisable = false;
            }

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

        private bool IsAllAirGroup(HoneybeeSchema.Energy.IHvac hvac)
        {
            var isGroup = hvac is VAV;
            isGroup |= hvac is PVAV;
            isGroup |= hvac is PSZ;
            isGroup |= hvac is PTAC;
            isGroup |= hvac is ForcedAirFurnace;
            return isGroup;
        }

        //private IEnumerable<string> GetAllAir()
        //{
        //    var names = new List<string>();

        //    // All air
        //    names.AddRange(Enum.GetNames(typeof(VAVEquipmentType)));
        //    names.AddRange(Enum.GetNames(typeof(PVAVEquipmentType)));
        //    names.AddRange(Enum.GetNames(typeof(PSZEquipmentType)));
        //    names.AddRange(Enum.GetNames(typeof(PTACEquipmentType)));
        //    names.AddRange(Enum.GetNames(typeof(FurnaceEquipmentType)));

        //    return names;
        //}
        private IEnumerable<Type> GetDOASTypes()
        {
            var types = new List<Type>()
            {
                typeof(FCUwithDOASEquipmentType),
                typeof(VRFwithDOASEquipmentType),
                typeof(WSHPwithDOASEquipmentType),
            };

            return types;
        }

        private bool IsDOASGroup(HoneybeeSchema.Energy.IHvac hvac)
        {
            var isDoas = hvac is FCUwithDOASAbridged;
            isDoas |= hvac is VRFwithDOASAbridged;
            isDoas |= hvac is WSHPwithDOASAbridged;

            return isDoas;
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
            };

            return types;
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
            return isGroup;
        }

        public Dictionary<Type, string> HVACTypesDic
            = new Dictionary<Type, string>()
            {
                {  typeof(VAVEquipmentType), "VAV systems"},
                {  typeof(PVAVEquipmentType), "PVAC"},
                {  typeof(PSZEquipmentType), "PSZ-AC"},
                {  typeof(PTACEquipmentType), "PTAC"},
                {  typeof(FurnaceEquipmentType), "Forced air furnace"},
                {  typeof(FCUwithDOASEquipmentType), "DOAS with fan coil unit"},
                {  typeof(VRFwithDOASEquipmentType), "DOAS with VRF"},
                {  typeof(WSHPwithDOASEquipmentType), "DOAS with water source heat pumps"},
                {  typeof(BaseboardEquipmentType), "Baseboard"},
                {  typeof(EvaporativeCoolerEquipmentType), "Direct evaporative coolers"},
                {  typeof(FCUEquipmentType),"Fan coil systems"},
                {  typeof(GasUnitHeaterEquipmentType), "Gas unit heaters"},
                {  typeof(ResidentialEquipmentType), "Residential Equipments"},
                {  typeof(VRFEquipmentType), "VRF"},
                {  typeof(WSHPEquipmentType), "Water source heat pumps"},
                {  typeof(WindowACEquipmentType), "Window AC"},

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
                {  typeof(BaseboardEquipmentType), typeof(Baseboard)},
                {  typeof(EvaporativeCoolerEquipmentType), typeof(EvaporativeCooler)},
                {  typeof(FCUEquipmentType),typeof(FCU)},
                {  typeof(GasUnitHeaterEquipmentType), typeof(GasUnitHeater)},
                {  typeof(ResidentialEquipmentType), typeof(Residential)},
                {  typeof(VRFEquipmentType), typeof(VRF)},
                {  typeof(WSHPEquipmentType), typeof(WSHP)},
                {  typeof(WindowACEquipmentType), typeof(WindowAC)},

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
                { "PSZHP","PSZ-HP"},
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
                { "PTHP","PTHP"},
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

            };

        private IEnumerable<string> GetHVACTypes(Type HVACType)
        {
            //var assem = typeof(VAVEquipmentType).Assembly;
            //var enumType = assem.GetType(HVACTypeName, false);
            //enumType = assem.GetType("HoneybeeSchema." + HVACTypeName);
            var names = Enum.GetNames(HVACType);

            return names;
        }

        public HoneybeeSchema.Energy.IHvac GreateHvac(HoneybeeSchema.Energy.IHvac existing = default)
        {
            
            var sysType = this.HvacType;
            var sysName = this.HvacEquipmentType;
            var vintage = Enum.Parse( typeof(Vintages), this.Vintage);

            var hvacType = HVACTypeMapper[sysType];
            hvacType = existing == null ? hvacType : existing.GetType();
            var id = Guid.NewGuid().ToString().Substring(0, 8);
            id = $"{hvacType.Name}_{id}";
            id = existing == null ? id : existing.Identifier;

            var dumy = new VAV(id);

            var constructor = hvacType.GetConstructors().FirstOrDefault();
            var parameters = constructor.GetParameters().Select(_ => _.HasDefaultValue ? _.DefaultValue : default).ToArray();
            parameters[0] = id;
            var obj = constructor.Invoke(parameters);
            var sys = obj as HoneybeeSchema.Energy.IHvac;

            // assign Name
            hvacType.GetProperty(nameof(dumy.DisplayName)).SetValue(sys, this.Name);

            // assign Vintage
            hvacType.GetProperty(nameof(dumy.Vintage)).SetValue(sys, vintage);

            // equipment type 
            var prop = hvacType.GetProperty(nameof(dumy.EquipmentType));
            var hvacSysType = Enum.Parse(prop.PropertyType, this.HvacEquipmentType);
            prop.SetValue(sys, hvacSysType);

            // SensibleHeatRecovery
            var sen = this.SensibleHR;
            // LatentHeatRecovery
            var lat = this.LatentHR;

            if (HvacGroup == HvacGroups[0])     // All air
            {
                // economizer
                var economizer = Enum.Parse(typeof(AllAirEconomizerType), this.Economizer);
                hvacType.GetProperty(nameof(dumy.EconomizerType)).SetValue(sys, economizer);

                // SensibleHeatRecovery
                hvacType.GetProperty(nameof(dumy.SensibleHeatRecovery)).SetValue(sys, sen);
                // LatentHeatRecovery
                hvacType.GetProperty(nameof(dumy.LatentHeatRecovery)).SetValue(sys, lat);
            }
            else if (HvacGroup == HvacGroups[1])      // DOAS
            {
                // SensibleHeatRecovery
                hvacType.GetProperty(nameof(dumy.SensibleHeatRecovery)).SetValue(sys, sen);
                // LatentHeatRecovery
                hvacType.GetProperty(nameof(dumy.LatentHeatRecovery)).SetValue(sys, lat);
            }
            
            return sys;
        }



    }





}
