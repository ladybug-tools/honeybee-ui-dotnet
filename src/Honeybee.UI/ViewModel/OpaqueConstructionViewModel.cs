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
            get => _hbObj.Layers;
            set => Set(() => _hbObj.Layers = value, nameof(Layers));
        }

        


        private static OpaqueConstructionViewModel _instance;
        public static OpaqueConstructionViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OpaqueConstructionViewModel();
                }
                return _instance;
            }
        }


        private OpaqueConstructionViewModel()
        {
        }

    }



}
