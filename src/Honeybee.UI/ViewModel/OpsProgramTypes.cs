using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{

    public class OpsProgramTypesViewModel : ViewModelBase
    {
        private IEnumerable<string> VintageJsonPaths => EnergyLibrary.BuildingVintages;
        public IEnumerable<string> VintageNames => VintageJsonPaths.Select(_ => System.IO.Path.GetFileNameWithoutExtension(_).Replace("_registry", ""));
        private string DefaultVintageName => VintageNames.First(_ => _.Contains("2013"));
        private Dictionary<string, IEnumerable<string>> DefaultBuildingTypes => EnergyLibrary.LoadBuildingVintage(VintageJsonPaths.First(_=>_.Contains(DefaultVintageName)));
        private IEnumerable<string> DefaultProgramTypes => DefaultBuildingTypes["LargeOffice"];

        private Dictionary<string, IEnumerable<string>> CurrentBuildingTypes { get; set; } 

        private string _vintage;
        public string Vintage
        {
            get => _vintage ?? DefaultVintageName;
            set
            {
                if (_vintage == value || string.IsNullOrEmpty(value))
                    return;

                Set(() => _vintage = value, nameof(Vintage));

                CurrentBuildingTypes= EnergyLibrary.LoadBuildingVintage(VintageJsonPaths.First(_ => _.Contains(value)));
                BuildingTypes = CurrentBuildingTypes.Keys;

            }
        }


        private IEnumerable<string> _buildingTypes;
        public IEnumerable<string> BuildingTypes
        {
            get => _buildingTypes ?? DefaultBuildingTypes.Keys;
            set
            {
                Set(() => _buildingTypes = value, nameof(BuildingTypes));
                BuildingType = value.First();
            }
        }

        private string _buildingType;
        public string BuildingType
        {
            get => _buildingType ?? BuildingTypes.First();
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _buildingType = value, nameof(BuildingType));
                ProgramTypes = CurrentBuildingTypes[value];
            }
        }

        public IEnumerable<string> _programTypes;
        public IEnumerable<string> ProgramTypes
        {
            get => _programTypes ?? DefaultProgramTypes;
            set
            {
                Set(() => _programTypes = value, nameof(ProgramTypes));
                ProgramType = value.First();
            }
        }

        private string _programType;
        public string ProgramType
        {
            get => _programType ?? ProgramTypes.First();
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _programType = value, nameof(ProgramType));
                //ProgramTypeJson = FullProgramType;

            }
        }

        //private ProgramTypeAbridged _programTypeJson;
        public (ProgramTypeAbridged programType, IEnumerable<ScheduleRulesetAbridged> schedules) ProgramTypeWithSches
        {
            get => EnergyLibrary.GetStandardProgramTypeByIdentifier(FullProgramType);
            //set
            //{
            //    var standardProgram = FullProgramType;
            //    _programTypeJson = EnergyLibrary.GetStandardProgramTypeByIdentifier(standardProgram);
            //    Set(() => {}, nameof(ProgramTypeJson));
            //    //var json = @"C:\Users\mingo\ladybug_tools\resources\standards\honeybee_energy_standards\programtypes\2013_data.json";
                
            //}
        }


        public string FullProgramType => $"{Vintage}::{BuildingType}::{ProgramType}";
       


        private static readonly OpsProgramTypesViewModel _instance = new OpsProgramTypesViewModel();
        public static OpsProgramTypesViewModel Instance => _instance;


        private OpsProgramTypesViewModel()
        {
            CurrentBuildingTypes = DefaultBuildingTypes;
        }

    }



}
