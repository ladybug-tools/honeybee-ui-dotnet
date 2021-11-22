using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    // duplicted from AperturePropertyViewModel, expect the difference of IsGlass property
    public class DoorPropertyViewModel : ViewModelBase
    {
        private Door _refHBObj;

        private List<Door> _hbObjs;

        public string Identifier
        {
            get => _refHBObj.Identifier;
            private set { this.Set(() => _refHBObj.Identifier = value, nameof(_refHBObj.Identifier)); }
        }


        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            private set
            {
                _isDisplayNameVaries = value == this.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName));
            }
        }



        private CheckboxViewModel _isGlass;
        public CheckboxViewModel IsGlass
        {
            get => _isGlass;
            private set => this.Set(() => _isGlass = value, nameof(IsGlass));
        }

        #region Radiance
        // string modifier = null, string modifierBlk = null, string dynamicGroupIdentifier = null, List<RadianceSubFaceStateAbridged> states = null
        private CheckboxButtonViewModel _modifier;
        public CheckboxButtonViewModel Modifier
        {
            get => _modifier;
            private set => this.Set(() => _modifier = value, nameof(Modifier));
        }

        private CheckboxButtonViewModel _modifierBlk;
        public CheckboxButtonViewModel ModifierBlk
        {
            get => _modifierBlk;
            private set => this.Set(() => _modifierBlk = value, nameof(ModifierBlk)); 
        }

        private bool _isDynamicGroupIdentifierVaries;
        public string DynamicGroupIdentifier
        {
            get => _refHBObj.Properties.Radiance.DynamicGroupIdentifier;
            private set
            {
                _isDynamicGroupIdentifierVaries = value == this.Varies;
                this.Set(() => _refHBObj.Properties.Radiance.DynamicGroupIdentifier = value, nameof(DynamicGroupIdentifier));
            }
        }

        #endregion

        #region Energy

        //string construction = null, VentilationOpening ventOpening = null

        private CheckboxButtonViewModel _construction;
        public CheckboxButtonViewModel Construction
        {
            get => _construction;
            private set => this.Set(() => _construction = value, nameof(Construction)); 
        }

        public static Dictionary<string, AnyOf<Outdoors, Surface>> Bcs =>
            new Dictionary<string, AnyOf<Outdoors, Surface>>()
            {
                {nameof(Outdoors), new Outdoors()},
                {nameof(Surface), new Surface(new List<string>())}
            };

        public List<string> BoundaryConditionTexts { get; } = Bcs.Keys.ToList();

        private bool _isBoundaryConditionVaries;
        private string _boundaryConditionText;
        public string BoundaryConditionText
        {
            get => _boundaryConditionText;
            private set
            {
                _isBoundaryConditionVaries = value == this.Varies;
                if (!_isBoundaryConditionVaries)
                {
                    this._refHBObj.BoundaryCondition = Bcs[value];
                }

                this.IsOutdoorBoundary = value == nameof(Outdoors);
                if (this.IsOutdoorBoundary)
                {
                    var outdoorBc = this._refHBObj.BoundaryCondition.Obj as Outdoors;
                    this.BCOutdoor = new BoundaryConditionOutdoorViewModel(new List<Outdoors>() { outdoorBc }, (o) => _refHBObj.BoundaryCondition = o);
                }
                this.IsSurfaceBoundary = value == nameof(Surface);
                if (this.IsSurfaceBoundary)
                {
                    var srf = this._refHBObj.BoundaryCondition.Obj as Surface;
                    this.BCSurface = new BoundaryConditionSurfaceViewModel(new List<Surface>() { srf }, (o) => _refHBObj.BoundaryCondition = o);
                }
                this.Set(()=> _boundaryConditionText = value, nameof(BoundaryConditionText));
            }
        }

        private bool _isOutdoorBoundary = true;
        public bool IsOutdoorBoundary
        {
            get => _isOutdoorBoundary;
            private set => this.Set(() => _isOutdoorBoundary = value, nameof(IsOutdoorBoundary));
        }

        private BoundaryConditionOutdoorViewModel _bcOutdoor;
        public BoundaryConditionOutdoorViewModel BCOutdoor
        {
            get => _bcOutdoor;
            private set => this.Set(() => _bcOutdoor = value, nameof(BCOutdoor));
        }
        private bool _isSurfaceBoundary = false;
        public bool IsSurfaceBoundary
        {
            get => _isSurfaceBoundary;
            private set => this.Set(() => _isSurfaceBoundary = value, nameof(IsSurfaceBoundary));
        }

        private BoundaryConditionSurfaceViewModel _bcSurface;
        public BoundaryConditionSurfaceViewModel BCSurface
        {
            get => _bcSurface;
            private set => this.Set(() => _bcSurface = value, nameof(BCSurface));
        }

        private VentilationOpeningViewModel _ventilationOpening;
        public VentilationOpeningViewModel VentilationOpening
        {
            get => _ventilationOpening;
            private set => this.Set(() => _ventilationOpening = value, nameof(VentilationOpening));
        }

        #endregion

        #region UserData
        private UserDataViewModel _userData;
        public UserDataViewModel UserData
        {
            get => _userData;
            set { this.Set(() => _userData = value, nameof(UserData)); }
        }

        #endregion

        private View.DoorProperty _control;
        private ModelProperties _libSource { get; set; }
        public Door Default { get; private set; }
        internal DoorPropertyViewModel(View.DoorProperty panel)
        {
            this.Default = new Door("id", new Face3D(new List<List<double>>()), new Outdoors(), new DoorPropertiesAbridged());
            _refHBObj = this.Default.DuplicateDoor();
            _libSource = new ModelProperties(ModelEnergyProperties.Default, ModelRadianceProperties.Default);
            this._control = panel;
            Update(_libSource, new List<Door>() { _refHBObj });

        }

        public void Update(ModelProperties libSource, List<Door> objs)
        {
            this._libSource = libSource;
            this._refHBObj = objs.FirstOrDefault().DuplicateDoor();
            var defaultEnergy = new DoorEnergyPropertiesAbridged();
            var defaultRadiance = new DoorRadiancePropertiesAbridged();
            _refHBObj.Properties.Energy = _refHBObj.Properties.Energy ?? defaultEnergy;
            _refHBObj.Properties.Radiance = _refHBObj.Properties.Radiance ?? defaultRadiance;

            // Identifier
            if (objs.Select(_ => _.Identifier).Distinct().Count() > 1)
                this.Identifier = this.Varies;
            else
                this.Identifier = this._refHBObj.Identifier;

            // DisplayName
            if (objs.Select(_ => _.DisplayName).Distinct().Count() > 1)
                this.DisplayName = this.Varies;
            else
                this.DisplayName = this._refHBObj.DisplayName;

            // IsOperable
            this.IsGlass = new CheckboxViewModel(_ => _refHBObj.IsGlass = _);
            if (objs.Select(_ => _?.IsGlass).Distinct().Count() > 1)
                this.IsGlass.SetCheckboxVaries();
            else
                this.IsGlass.SetCheckboxChecked(this._refHBObj.IsGlass);


            // Modifier
            var mdf = _libSource.Radiance.Modifiers?
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()?
                .FirstOrDefault(_ => _.Identifier  == _refHBObj.Properties.Radiance.Modifier);
            this.Modifier = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.Modifier = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.Modifier).Distinct().Count() > 1)
                this.Modifier.SetBtnName(this.Varies);
            else
                this.Modifier.SetPropetyObj(mdf);

            // ModifierBlk
            var mdfblk = _libSource.Radiance.Modifiers?
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Radiance.ModifierBlk);
            this.ModifierBlk = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.ModifierBlk = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.ModifierBlk).Distinct().Count() > 1)
                this.ModifierBlk.SetBtnName(this.Varies);
            else
                this.ModifierBlk.SetPropetyObj(mdfblk);

            // DynamicGroupIdentifier
            if (objs.Select(_ => _.Properties.Radiance?.DynamicGroupIdentifier).Distinct().Count() > 1)
                this.DynamicGroupIdentifier = this.Varies;
            else
                this.DynamicGroupIdentifier = this._refHBObj.Properties.Radiance.DynamicGroupIdentifier;


            // Construction
            var cts = _libSource.Energy.Constructions?
                .OfType<HoneybeeSchema.Energy.IIDdEnergyBaseModel>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.Construction);
            this.Construction = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Construction = s?.Identifier);

            if (objs.Select(_ => _.Properties.Energy?.Construction).Distinct().Count() > 1)
                this.Construction.SetBtnName(this.Varies);
            else
                this.Construction.SetPropetyObj(cts);


            // BoundaryCondition
            var bcs = objs.Select(_ => _.BoundaryCondition.Obj).Distinct();
            if (bcs.Count() > 1)
                this.BoundaryConditionText = this.Varies;
            else
                this.BoundaryConditionText = this._refHBObj.BoundaryCondition;

            if (this.IsOutdoorBoundary)
            {
                var outdoors = objs.Select(_ => _.BoundaryCondition).OfType<Outdoors>().Distinct().ToList();
                this.BCOutdoor = new BoundaryConditionOutdoorViewModel(outdoors, (o)=> _refHBObj.BoundaryCondition = o);
            }
            else if (this.IsSurfaceBoundary)
            {
                var srfs = objs.Select(_ => _.BoundaryCondition).OfType<Surface>().Distinct().ToList();
                this.BCSurface = new BoundaryConditionSurfaceViewModel(srfs, (o) => _refHBObj.BoundaryCondition = o);
            }

            // VentOpening
            var vents = objs.Select(_ => _.Properties.Energy?.VentOpening).Distinct().ToList();
            this.VentilationOpening = new VentilationOpeningViewModel(libSource, vents, _ => _refHBObj.Properties.Energy.VentOpening = _);


            // User data
            var allUserData = objs.Select(_ => _.UserData).Distinct().ToList();
            this.UserData = this.UserData ?? new UserDataViewModel(allUserData, (s) => _refHBObj.UserData = s, _control);

            this._hbObjs = objs.Select(_ => _.DuplicateDoor()).ToList();
        }

        public List<Door> GetDoors()
        {
            var refObj = this._refHBObj;
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this.IsGlass.IsVaries)
                    item.IsGlass = refObj.IsGlass;

                //string construction = null, VentilationOpening ventOpening = null
                // energy
                item.Properties.Energy = item.Properties.Energy ?? new DoorEnergyPropertiesAbridged();

                if (!this.Construction.IsVaries)
                    item.Properties.Energy.Construction = refObj.Properties.Energy.Construction;

                if (!this._isBoundaryConditionVaries)
                {
                    if (this.IsOutdoorBoundary)
                    {
                        item.BoundaryCondition = this.BCOutdoor.MatchObj(item.BoundaryCondition.Obj as Outdoors);
                    }
                    else if (this.IsSurfaceBoundary)
                    {
                        item.BoundaryCondition = this.BCSurface.MatchObj(item.BoundaryCondition.Obj as Surface);
                    }
                    else
                    {
                        item.BoundaryCondition = refObj.BoundaryCondition;
                    }

                }

                item.Properties.Energy.VentOpening = this.VentilationOpening.MatchObj(item.Properties.Energy.VentOpening);


                // string modifier = null, string modifierBlk = null, string dynamicGroupIdentifier = null, List<RadianceSubFaceStateAbridged> states = null
                // radiance 
                item.Properties.Radiance = item.Properties.Radiance ?? new DoorRadiancePropertiesAbridged();

                if (!this.Modifier.IsVaries)
                    item.Properties.Radiance.Modifier = refObj.Properties.Radiance.Modifier;

                if (!this.ModifierBlk.IsVaries)
                    item.Properties.Radiance.ModifierBlk = refObj.Properties.Radiance.ModifierBlk;

                if (!this._isDynamicGroupIdentifierVaries)
                    item.Properties.Radiance.DynamicGroupIdentifier = refObj.Properties.Radiance.DynamicGroupIdentifier;


                // User data
                item.UserData = this.UserData.MatchObj();
            }

            return this._hbObjs;
        }

        public ICommand ModifierCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Radiance;
            var dialog = new Dialog_ModifierManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.Modifier.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand ModifierBlkCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Radiance;
            var dialog = new Dialog_ModifierManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.ModifierBlk.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand ConstructionCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ConstructionManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.Construction.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand SurfaceBCCommand => new Eto.Forms.RelayCommand(() =>
        {
            this.BCSurface.EditSurfaceBC(this._control);
        });

        public ICommand HBDataBtnClick => new RelayCommand(() => {

            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });
    }



}
