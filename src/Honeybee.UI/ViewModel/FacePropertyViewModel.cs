using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class FacePropertyViewModel : ViewModelBase
    {
        private Face _refHBObj;

        private List<Face> _hbObjs;

        public int TabIndex
        {
            get => 0;
            set { this.Set(null, nameof(TabIndex)); }
        }

        public string Identifier
        {
            get => _refHBObj.Identifier;
            set => this.Set(() => _refHBObj.Identifier = value, nameof(_refHBObj.Identifier)); 
        }


        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            set
            {
                _isDisplayNameVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName));
            }
        }

        public FaceType FaceType
        {
            get => _refHBObj.FaceType;
            set
            {
                FaceTypeText = value.ToString();
                this.Set(() => _refHBObj.FaceType = value, nameof(FaceType));
            }
        }

        private bool _isFaceTypeVaries;
        private string _faceTypeText;
        public string FaceTypeText
        {
            get => _faceTypeText;
            set
            {
                _isFaceTypeVaries = value == ReservedText.Varies;
                this.Set(() => _faceTypeText = value, nameof(FaceTypeText));
            }
        }

        #region Radiance

        private CheckboxButtonViewModel _modifier;
        public CheckboxButtonViewModel Modifier
        {
            get => _modifier;
            set { this.Set(() => _modifier = value, nameof(Modifier)); }
        }

        private CheckboxButtonViewModel _modifierBlk;
        public CheckboxButtonViewModel ModifierBlk
        {
            get => _modifierBlk;
            set => this.Set(() => _modifierBlk = value, nameof(ModifierBlk)); 
        }

        #endregion

        #region Energy

      

        private CheckboxButtonViewModel _construction;
        public CheckboxButtonViewModel Construction
        {
            get => _construction;
            set => this.Set(() => _construction = value, nameof(Construction)); 
        }

        public static Dictionary<string, AnyOf<Ground, Outdoors, Adiabatic, Surface, OtherSideTemperature>> Bcs =>
            new Dictionary<string, AnyOf<Ground, Outdoors, Adiabatic, Surface, OtherSideTemperature>>()
            {
                {nameof(Ground), new Ground()},
                {nameof(Outdoors), new Outdoors()},
                {nameof(Adiabatic), new Adiabatic()},
                {nameof(Surface), new Surface(new List<string>())},
                {nameof(OtherSideTemperature), new OtherSideTemperature(temperature : new Autocalculate())}
            };

        public List<string> BoundaryConditionTexts { get; } = Bcs.Keys.ToList();

        private bool _isBoundaryConditionVaries;
        private string _boundaryConditionText;
        public string BoundaryConditionText
        {
            get => _boundaryConditionText;
            set
            {
                _isBoundaryConditionVaries = value == ReservedText.Varies;
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
                this.IsOtherSideTemperatureBoundary = value == nameof(OtherSideTemperature);
                if (this.IsOtherSideTemperatureBoundary)
                {
                    var other = this._refHBObj.BoundaryCondition.Obj as OtherSideTemperature;
                    this.BCOtherSideTemperature = new BoundaryConditionOtherSideTemperatureViewModel(new List<OtherSideTemperature>() { other }, (o) => _refHBObj.BoundaryCondition = o);
                }
                this.Set(()=> _boundaryConditionText = value, nameof(BoundaryConditionText));
            }
        }

        private bool _isOutdoorBoundary = true;
        public bool IsOutdoorBoundary
        {
            get => _isOutdoorBoundary;
            set => this.Set(() => _isOutdoorBoundary = value, nameof(IsOutdoorBoundary)); 
        }

        private BoundaryConditionOutdoorViewModel _bcOutdoor;
        public BoundaryConditionOutdoorViewModel BCOutdoor
        {
            get => _bcOutdoor;
            set => this.Set(() => _bcOutdoor = value, nameof(BCOutdoor)); 
        }

        private bool _isSurfaceBoundary = false;
        public bool IsSurfaceBoundary
        {
            get => _isSurfaceBoundary;
            set => this.Set(() => _isSurfaceBoundary = value, nameof(IsSurfaceBoundary));
        }


        private BoundaryConditionSurfaceViewModel _bcSurface;
        public BoundaryConditionSurfaceViewModel BCSurface
        {
            get => _bcSurface;
            set => this.Set(() => _bcSurface = value, nameof(BCSurface));
        }

        private bool _isOtherSideTemperatureBoundary = false;
        public bool IsOtherSideTemperatureBoundary
        {
            get => _isOtherSideTemperatureBoundary;
            set => this.Set(() => _isOtherSideTemperatureBoundary = value, nameof(IsOtherSideTemperatureBoundary));
        }

        private BoundaryConditionOtherSideTemperatureViewModel _bcOtherSideTemperature;
        public BoundaryConditionOtherSideTemperatureViewModel BCOtherSideTemperature
        {
            get => _bcOtherSideTemperature;
            set => this.Set(() => _bcOtherSideTemperature = value, nameof(BCOtherSideTemperature));
        }



        // AFNCrack
        private AFNCrackViewModel _AFNCrack;

        public AFNCrackViewModel AFNCrack
        {
            get => _AFNCrack;
            set => this.Set(() => _AFNCrack = value, nameof(AFNCrack));
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




        private View.FaceProperty _control;
        private ModelProperties _libSource { get; set; }
        public Face Default { get; private set; }
        internal FacePropertyViewModel(View.FaceProperty panel)
        {
            this.Default = new Face("id", new Face3D(new List<List<double>>()), FaceType.Wall, new Outdoors(), new FacePropertiesAbridged());
            _refHBObj = this.Default.DuplicateFace();
            _libSource = new ModelProperties(ModelEnergyProperties.Default, ModelRadianceProperties.Default);
            this._control = panel;
            Update(_libSource, new List<Face>() { _refHBObj });

        }

        public void Update(ModelProperties libSource, List<Face> objs)
        {
            this.TabIndex = 0;
            this._libSource = libSource;
            this._refHBObj = objs.FirstOrDefault().DuplicateFace();
            var defaultEnergy = new FaceEnergyPropertiesAbridged();
            var defaultRadiance = new FaceRadiancePropertiesAbridged();
            _refHBObj.Properties.Energy = _refHBObj.Properties.Energy ?? defaultEnergy;
            _refHBObj.Properties.Radiance = _refHBObj.Properties.Radiance ?? defaultRadiance;

            // Identifier
            if (objs.Select(_ => _.Identifier).Distinct().Count() > 1)
                this.Identifier = ReservedText.Varies;
            else
                this.Identifier = this._refHBObj.Identifier;

            // DisplayName
            if (objs.Select(_ => _.DisplayName).Distinct().Count() > 1)
                this.DisplayName = ReservedText.Varies;
            else
                this.DisplayName = this._refHBObj.DisplayName;

            // FaceType
            if (objs.Select(_ => _.FaceType).Distinct().Count() > 1)
                this.FaceTypeText = ReservedText.Varies;
            else
                this.FaceType = this._refHBObj.FaceType;


            // Modifier
            var mdf = _libSource.Radiance.Modifiers?
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()?
                .FirstOrDefault(_ => _.Identifier  == _refHBObj.Properties.Radiance.Modifier);
            this.Modifier = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.Modifier = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.Modifier).Distinct().Count() > 1)
                this.Modifier.SetBtnName(ReservedText.Varies);
            else
                this.Modifier.SetPropetyObj(mdf);

            // ModifierBlk
            var mdfblk = _libSource.Radiance.Modifiers?
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Radiance.ModifierBlk);
            this.ModifierBlk = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.ModifierBlk = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.ModifierBlk).Distinct().Count() > 1)
                this.ModifierBlk.SetBtnName(ReservedText.Varies);
            else
                this.ModifierBlk.SetPropetyObj(mdfblk);


            // Construction
            var cts = _libSource.Energy.Constructions?
                .OfType<HoneybeeSchema.Energy.IIDdEnergyBaseModel>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.Construction);
            this.Construction = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Construction = s?.Identifier);

            if (objs.Select(_ => _.Properties.Energy?.Construction).Distinct().Count() > 1)
                this.Construction.SetBtnName(ReservedText.Varies);
            else
                this.Construction.SetPropetyObj(cts);


            // BoundaryCondition
            var bcs = objs.Select(_ => _.BoundaryCondition.Obj).Distinct();
            if (bcs.Count() > 1)
                this.BoundaryConditionText = ReservedText.Varies;
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
            else if (this.IsOtherSideTemperatureBoundary)
            {
                var others = objs.Select(_ => _.BoundaryCondition).OfType<OtherSideTemperature>().Distinct().ToList();
                this.BCOtherSideTemperature = new BoundaryConditionOtherSideTemperatureViewModel(others, (o)=> _refHBObj.BoundaryCondition = o);
            }

         
            var afns = objs.Select(_ => _.Properties.Energy?.VentCrack).Distinct().ToList();
            this.AFNCrack = new AFNCrackViewModel(libSource, afns, _ => _refHBObj.Properties.Energy.VentCrack = _);


            // User data
            var allUserData = objs.Select(_ => _.UserData).Distinct().ToList();
            this.UserData = new UserDataViewModel(allUserData, (s) => _refHBObj.UserData = s, _control);

            this._hbObjs = objs.Select(_ => _.DuplicateFace()).ToList();
        }

        public List<Face> GetFaces()
        {
            var refObj = this._refHBObj;
           
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this._isFaceTypeVaries)
                    item.FaceType = refObj.FaceType;

                // energy
                item.Properties.Energy = item.Properties.Energy ?? new FaceEnergyPropertiesAbridged();

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
                    else if (this.IsOtherSideTemperatureBoundary)
                    {
                        item.BoundaryCondition = this.BCOtherSideTemperature.MatchObj(item.BoundaryCondition.Obj as OtherSideTemperature);
                    }
                    else
                    {
                        item.BoundaryCondition = refObj.BoundaryCondition;
                    }
                    
                }

                // radiance 
                item.Properties.Radiance = item.Properties.Radiance ?? new FaceRadiancePropertiesAbridged();

                if (!this.Modifier.IsVaries)
                    item.Properties.Radiance.Modifier = refObj.Properties.Radiance.Modifier;


                if (!this.ModifierBlk.IsVaries)
                    item.Properties.Radiance.ModifierBlk = refObj.Properties.Radiance.ModifierBlk;


                // User data
                item.UserData = this.UserData.MatchObj(item.UserData);

                // validate
                item.IsValid(true);
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
                var c = dialog_rc[0];
                if (c is HoneybeeSchema.Energy.IWindowConstruction)
                {
                    Honeybee.UI.Dialog_Message.Show($"Cannot assign WindowConstruction to the {this.FaceTypeText} face!");
                    return;
                }

                if (this.FaceType != FaceType.AirBoundary && c is HoneybeeSchema.Energy.IAirBoundaryConstruction)
                {
                    Honeybee.UI.Dialog_Message.Show($"Cannot assign AirBoundaryConstruction to the {this.FaceTypeText} face!");
                    return;
                }
                this.Construction.SetPropetyObj(c);
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
