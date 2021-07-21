using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{

    public class OpsConstructionSetsViewModel : ViewModelBase
    {
        private IEnumerable<string> VintageJsonPaths => EnergyLibrary.BuildingVintages;
        public IEnumerable<string> VintageNames => VintageJsonPaths.Select(_ => _.Split('\\').Last().Replace("_registry.json", ""));
        private string DefaultVintageName => VintageNames.First(_ => _.Contains("2019"));

        public IEnumerable<string> ConstructionSetTypes => new List<string>() { "SteelFramed", "WoodFramed", "Mass", "Metal Building" };
        private string DefaultConstructionSetType => ConstructionSetTypes.First();

        public IEnumerable<string> ClimateZones => new List<string>() { "ClimateZone1", "ClimateZone2", "ClimateZone3", "ClimateZone4", "ClimateZone5", "ClimateZone6", "ClimateZone7", "ClimateZone8" };
        private string DefaultClimateZone => ClimateZones.First();



        private string _vintage;
        public string Vintage
        {
            get => _vintage ?? DefaultVintageName;
            set => Set(() => _vintage = value, nameof(Vintage));
        }

        private string _constructionSetType;
        public string ConstructionSetType
        {
            get => _constructionSetType ?? DefaultConstructionSetType;
            set => Set(() => _constructionSetType = value, nameof(ConstructionSetType));
        }

        private string _climateZone;
        public string ClimateZone
        {
            get => _climateZone ?? DefaultClimateZone;
            set => Set(() => _climateZone = value, nameof(ClimateZone));
        }

        public string FullConstructionSet => $"{Vintage}::{ClimateZone}::{ConstructionSetType}";
        public (ConstructionSetAbridged ConstructionSet, IEnumerable<HoneybeeSchema.Energy.IConstruction> constructions, IEnumerable<HoneybeeSchema.Energy.IMaterial> materials) ConstructionWithMats
        {
            get => EnergyLibrary.GetStandardConstructionSetByIdentifier(FullConstructionSet);
        }


        

        private static readonly OpsConstructionSetsViewModel _instance = new OpsConstructionSetsViewModel();
        public static OpsConstructionSetsViewModel Instance => _instance;


        private OpsConstructionSetsViewModel()
        {
        }


    }



}
