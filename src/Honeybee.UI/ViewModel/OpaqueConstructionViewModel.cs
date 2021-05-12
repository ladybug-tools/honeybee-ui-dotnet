using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{

    public class OpaqueConstructionViewModel : ViewModelBase
    {
        private OpaqueConstructionAbridged _hbObj;
        public OpaqueConstructionAbridged hbObj
        {
            get => _hbObj;
            set
            {
                _hbObj = value;
                var props = this.GetType().GetProperties().Select(_ => _.Name);
                this.RefreshControls(props);
            }
        }

        public string DisplayName
        {
            get 
            {
                _hbObj.DisplayName = _hbObj.DisplayName ?? _hbObj.Identifier;
                return _hbObj.DisplayName;
            }
            set => Set(() => _hbObj.DisplayName = value, nameof(DisplayName));
        }

        public List<string> Layers
        {
            get => _hbObj.Materials;
            set => Set(() => _hbObj.Materials = value, nameof(Layers));
        }


        private static readonly OpaqueConstructionViewModel _instance = new OpaqueConstructionViewModel();
        public static OpaqueConstructionViewModel Instance => _instance;



        private OpaqueConstructionViewModel()
        {
        }

    }



}
