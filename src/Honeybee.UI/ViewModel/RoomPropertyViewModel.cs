using Eto.Forms;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;



namespace Honeybee.UI
{

    public class RoomPropertyViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        public string Unconditioned => "Unconditioned";
        public string Unoccupied => "Unoccupied, No Loads";
        public string ByGlobalConstructionSet => "By Global Construction Set";
        public string ByProgramType => "By Room Program Type";
        public string ByGlobalModifierSet => "By Global Modifier Set";

        private Room _refHBObj;
  

        private List<Room> _hbObjs;

        #region RoomProperty

        public string Identifier
        {
            get => _refHBObj.Identifier;
            private set { this.Set(() => _refHBObj.Identifier = value, nameof(_refHBObj.Identifier)); }
        }


        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            private set {
                _isDisplayNameVaries = value == this.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName)); 
            }
        }

        private bool _isStoryVaries;
        public string Story
        {
            get => _refHBObj.Story;
            private set {
                _isStoryVaries = value == this.Varies; 
                this.Set(() => _refHBObj.Story = value, nameof(Story)); 
            }
        }

        private bool _isMultiplierVaries;
     
        private string _multiplierText;
        public string MultiplierText
        {
            get => _multiplierText;
            private set
            {
                _isMultiplierVaries = value == this.Varies;
                if (_isMultiplierVaries) {
                    _multiplierText = value;
                    this.RefreshControl(nameof(MultiplierText));
                    return;
                }
                else if (int.TryParse(value, out var number))
                {
                    _multiplierText = number.ToString();
                    this.Set(() => _refHBObj.Multiplier = number, nameof(MultiplierText));

                }

            }
        }

        #endregion


        #region ConstructionSet


        private bool _isConstructionSetVaries;
        private HoneybeeSchema.Energy.IBuildingConstructionset _constructionSet
        {
            get => GetConstrutionSet(_refHBObj.Properties.Energy?.ConstructionSet);
            set
            {
                _refHBObj.Properties.Energy.ConstructionSet = value?.Identifier;
                ConstructionSetName = value?.DisplayName ?? value?.Identifier;
            }
        }

        private HoneybeeSchema.Energy.IBuildingConstructionset GetConstrutionSet(string name)
        {
            return _libSource.Energy.ConstructionSets
                .OfType<HoneybeeSchema.Energy.IBuildingConstructionset>()
                .FirstOrDefault(_ => _.Identifier == name);
        }

        private string _constructionSetName = "Override";
        public string ConstructionSetName
        {
            get => _isConstructionSetVaries ? this.Varies: _constructionSetName;
            private set {
                IsByGlobalConstructionSet = _constructionSet == null;
                IsCSetBtnEnabled = !IsByGlobalConstructionSet;

                _isConstructionSetVaries = value == this.Varies;
                if (_isConstructionSetVaries)
                    IsByGlobalConstructionSet = false;

                if (IsCSetBtnEnabled)
                    this.Set(() => _constructionSetName = value, nameof(ConstructionSetName));
            
            }
        }

        private bool _isCSetBtnEnabled;

        public bool IsCSetBtnEnabled
        {
            get => _isCSetBtnEnabled;
            private set { this.Set(() => _isCSetBtnEnabled = value, nameof(IsCSetBtnEnabled)); }
        }

        private bool _isByGlobalConstructionSet;

        public bool IsByGlobalConstructionSet
        {
            get => _isByGlobalConstructionSet;

            private set {
                this.Set(() => _isByGlobalConstructionSet = value, nameof(IsByGlobalConstructionSet));
                IsCSetBtnEnabled = !IsByGlobalConstructionSet;
                if (_isByGlobalConstructionSet) {
                    _refHBObj.Properties.Energy.ConstructionSet = null;
                }
                
            }
        }

        #endregion


        #region ProgramType


        private bool _isProgramTypeVaries;
        private HoneybeeSchema.Energy.IProgramtype _programType
        {
            get => GetProgramtype(_refHBObj.Properties.Energy?.ProgramType);
            set
            {
                _refHBObj.Properties.Energy.ProgramType = value?.Identifier;
                ProgramTypeName = value?.DisplayName ?? value?.Identifier;
            }
        }
        private HoneybeeSchema.Energy.IProgramtype GetProgramtype(string name)
        {
            return _libSource.Energy.ProgramTypes
               .OfType<HoneybeeSchema.Energy.IProgramtype>()
               .FirstOrDefault(_ => _.Identifier == name);
        }

        private string _programTypeName = "Override";
        public string ProgramTypeName
        {
            get => _isProgramTypeVaries ? this.Varies : _programTypeName;
            private set
            {
                IsUnoccupied = _programType == null;
                IsPTypeBtnEnabled = !IsUnoccupied;
       
                _isProgramTypeVaries = value == this.Varies;
                if (_isProgramTypeVaries)
                    IsUnoccupied = false;

                if (IsPTypeBtnEnabled)
                    this.Set(() => _programTypeName = value, nameof(ProgramTypeName));
            }
        }

        private bool _isPTypeBtnEnabled;

        public bool IsPTypeBtnEnabled
        {
            get => _isPTypeBtnEnabled;
            private set { this.Set(() => _isPTypeBtnEnabled = value, nameof(IsPTypeBtnEnabled)); }
        }

        private bool _isUnoccupied;

        public bool IsUnoccupied
        {
            get => _isUnoccupied;

            private set
            {
                this.Set(() => _isUnoccupied = value, nameof(IsUnoccupied));
                IsPTypeBtnEnabled = !value;
                if (_isUnoccupied)
                    _refHBObj.Properties.Energy.ProgramType = null;
            }
        }

        #endregion


        #region HVAC
        private bool _isHVACVaries;

        private HoneybeeSchema.Energy.IHvac _hvac
        {
            get => GetHvac(_refHBObj.Properties.Energy?.Hvac);
            set
            {
                _refHBObj.Properties.Energy.Hvac = value?.Identifier;
                HVACName = value?.DisplayName ?? value?.Identifier;
            }
        }
        private HoneybeeSchema.Energy.IHvac GetHvac(string name)
        {
            return _libSource.Energy.Hvacs
                .OfType<HoneybeeSchema.Energy.IHvac>()
                .FirstOrDefault(_ => _.Identifier == name);
        }

        private string _HvacName = "Override";
        public string HVACName
        {
            get => _isHVACVaries ? this.Varies : _HvacName;
            private set
            {
                IsUnconditioned = _hvac == null;
                IsHVACBtnEnabled = !IsUnconditioned;

                _isHVACVaries = value == this.Varies;
                if (_isHVACVaries)
                    IsUnconditioned = false;

                if (IsHVACBtnEnabled)
                    this.Set(() => _HvacName = value, nameof(HVACName));
            }
        }

        private bool _isHVACBtnEnabled;
        public bool IsHVACBtnEnabled
        {
            get => _isHVACBtnEnabled;
            private set { this.Set(() => _isHVACBtnEnabled = value, nameof(IsHVACBtnEnabled)); }
        }

        private bool _isUnconditioned;

        public bool IsUnconditioned
        {
            get => _isUnconditioned;
            private set
            {
                this.Set(() => _isUnconditioned = value, nameof(IsUnconditioned));
                IsHVACBtnEnabled = !value;
                if (_isUnconditioned)
                    _refHBObj.Properties.Energy.Hvac = null;
            }
        }


        #endregion


        #region ModifierSet


        private bool _isModifierSetVaries;
        private ModifierSetAbridged _modifierSet
        {
            get => GetModifierSet(_refHBObj.Properties.Radiance?.ModifierSet);
            set
            {
                _refHBObj.Properties.Radiance.ModifierSet = value?.Identifier;
                ModifierSetName = value?.DisplayName ?? value?.Identifier;
            }
        }
        private ModifierSetAbridged GetModifierSet(string name)
        {
            return _libSource.Radiance.ModifierSets
               .OfType<ModifierSetAbridged>()
               .FirstOrDefault(_ => _.Identifier == name);
        }

        private string _ModifierSetName = "Override";
        public string ModifierSetName
        {
            get => _isModifierSetVaries ? this.Varies : _ModifierSetName;
            private set
            {
                IsByGlobalModifierSet = _modifierSet == null;
                IsModifierSetBtnEnabled = !IsByGlobalModifierSet;

                _isModifierSetVaries = value == this.Varies;
                if (_isModifierSetVaries)
                    IsByGlobalModifierSet = false;

                if (IsModifierSetBtnEnabled)
                    this.Set(() => _ModifierSetName = value, nameof(ModifierSetName));
            }
        }

        private bool _isModifierSetBtnEnabled;

        public bool IsModifierSetBtnEnabled
        {
            get => _isModifierSetBtnEnabled;
            private set { this.Set(() => _isModifierSetBtnEnabled = value, nameof(IsModifierSetBtnEnabled)); }
        }

        private bool _isByGlobalModifierSet;

        public bool IsByGlobalModifierSet
        {
            get => _isByGlobalModifierSet;

            private set
            {
                this.Set(() => _isByGlobalModifierSet = value, nameof(IsByGlobalModifierSet));
                IsModifierSetBtnEnabled = !value;
                if (_isByGlobalModifierSet)
                    _refHBObj.Properties.Radiance.ModifierSet = null;
            }
        }

        #endregion


        #region Lighting
        private bool _isLightingVaries;
        public string LightingName
        {
            get => _refHBObj.DisplayName;
            private set
            {
                _isDisplayNameVaries = value == this.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName));
            }
        }
        #endregion


        private View.RoomProperty _control;
        private ModelProperties _libSource { get; set; } 
        internal RoomPropertyViewModel(View.RoomProperty roomPanel)
        {
            _refHBObj = new Room("", new List<Face>(), new RoomPropertiesAbridged());
            _libSource = new ModelProperties(ModelEnergyProperties.Default, ModelRadianceProperties.Default);
            this._control = roomPanel;
        }
        
        public void Update(ModelProperties libSource, List<Room> rooms)
        {
            this._libSource = libSource;
            this._refHBObj = rooms.FirstOrDefault().DuplicateRoom();
            _refHBObj.Properties.Energy = _refHBObj.Properties.Energy ?? new RoomEnergyPropertiesAbridged();
            _refHBObj.Properties.Radiance = _refHBObj.Properties.Radiance ?? new RoomRadiancePropertiesAbridged();

            if (rooms.Select(_ => _.Identifier).Distinct().Count() > 1)
                this.Identifier = this.Varies;

            if (rooms.Select(_ => _.DisplayName).Distinct().Count() > 1)
                this.DisplayName = this.Varies;

            if (rooms.Select(_ => _.Story).Distinct().Count() > 1)
                this.Story = this.Varies;

            if (rooms.Select(_ => _.Multiplier).Distinct().Count() > 1)
                MultiplierText = this.Varies;


         
            //construction
            if (rooms.Select(_ => _.Properties.Energy?.ConstructionSet).Distinct().Count() > 1)
                this.ConstructionSetName = this.Varies;
            else
                this._constructionSet = GetConstrutionSet(_refHBObj.Properties.Energy.ConstructionSet);

            //program type
            if (rooms.Select(_ => _.Properties.Energy?.ProgramType).Distinct().Count() > 1)
                this.ProgramTypeName = this.Varies;
            else
                this._programType = GetProgramtype(_refHBObj.Properties.Energy.ProgramType);

            // hvac
            if (rooms.Select(_ => _.Properties.Energy?.Hvac).Distinct().Count() > 1)
                this.HVACName = this.Varies;
            else
                this._hvac = GetHvac(_refHBObj.Properties.Energy.Hvac);

            // ModifierSet
            if (rooms.Select(_ => _.Properties.Radiance?.ModifierSet).Distinct().Count() > 1)
                this.ModifierSetName = this.Varies;
            else
                this._modifierSet = GetModifierSet(_refHBObj.Properties.Radiance.ModifierSet);

            // Load
            if (rooms.Select(_ => _.Properties.Energy?.Lighting?.Identifier).Distinct().Count() > 1)
                this.ModifierSetName = this.Varies;
            else
                this._modifierSet = GetModifierSet(_refHBObj.Properties.Radiance.ModifierSet);


            this._hbObjs = rooms.Select(_ => _.DuplicateRoom()).ToList();

        }

      

        public List<Room> GetRooms()
        {
            var refObj = this._refHBObj;
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this._isStoryVaries)
                    item.Story = refObj.Story;

                if (!this._isMultiplierVaries)
                    item.Multiplier = refObj.Multiplier;


                item.Properties.Energy = item.Properties.Energy ?? new RoomEnergyPropertiesAbridged();

                if (!this._isConstructionSetVaries)
                    item.Properties.Energy.ConstructionSet = refObj.Properties.Energy.ConstructionSet;

                if (!this._isProgramTypeVaries)
                    item.Properties.Energy.ProgramType = refObj.Properties.Energy.ProgramType;

                if (!this._isHVACVaries)
                    item.Properties.Energy.Hvac = refObj.Properties.Energy.Hvac;
            }

            return this._hbObjs;
        }

        public ICommand RoomConstructionSetCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_ConstructionSetSelector(_libSource.Energy);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this._constructionSet = dialog_rc;
            }
        });

        public ICommand RoomProgramTypeCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_ProgramTypeSelector(_libSource.Energy);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this._programType = dialog_rc;
            }
        });

        public ICommand RoomHVACCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_HVACSelector(_libSource.Energy);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this._hvac = dialog_rc;
            }
        });

        public ICommand ModifierSetCommand => new RelayCommand(() =>
        {
            //var lib = _libSource.Energy;
            var dialog = new Dialog_ModifierSetSelector(_libSource.Radiance);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this._modifierSet = dialog_rc;
            }
        });

        public ICommand HBDataBtnClick => new RelayCommand(() => {
            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });

    }



}
