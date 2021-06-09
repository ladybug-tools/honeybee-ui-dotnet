﻿using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class FacePropertyViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        public string ByParentSetting => "By Parent Setting";
        private Face _refHBObj;

        private List<Face> _hbObjs;

        public string Identifier
        {
            get => _refHBObj.Identifier;
            private set => this.Set(() => _refHBObj.Identifier = value, nameof(_refHBObj.Identifier)); 
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

        public FaceType FaceType
        {
            get => _refHBObj.FaceType;
            private set
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
            private set
            {
                _isFaceTypeVaries = value == this.Varies;
                this.Set(() => _faceTypeText = value, nameof(FaceTypeText));
            }
        }

        #region Radiance

        private CheckboxButtonViewModel _modifier;
        public CheckboxButtonViewModel Modifier
        {
            get => _modifier;
            private set { this.Set(() => _modifier = value, nameof(Modifier)); }
        }

        private CheckboxButtonViewModel _modifierBlk;
        public CheckboxButtonViewModel ModifierBlk
        {
            get => _modifierBlk;
            private set => this.Set(() => _modifierBlk = value, nameof(ModifierBlk)); 
        }

        #endregion

        #region Energy

      

        private CheckboxButtonViewModel _construction;
        public CheckboxButtonViewModel Construction
        {
            get => _construction;
            private set => this.Set(() => _construction = value, nameof(Construction)); 
        }

        public static Dictionary<string, AnyOf<Ground, Outdoors, Adiabatic, Surface>> Bcs =>
            new Dictionary<string, AnyOf<Ground, Outdoors, Adiabatic, Surface>>()
            {
                {nameof(Ground), new Ground()},
                {nameof(Outdoors), new Outdoors()},
                {nameof(Adiabatic), new Adiabatic()},
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
                    if (value == nameof(Surface))
                    {
                        // reset
                        _isBoundaryConditionVaries = _boundaryConditionText == this.Varies;
                        MessageBox.Show("Boundary condition cannot be changed to Surface manually. please use check SolveAdjacency");
                        return;
                    }
                    else
                    {
                        this._refHBObj.BoundaryCondition = Bcs[value];
                    }
                }

                this.IsOutdoorBoundary = value == nameof(Outdoors);
                if (this.IsOutdoorBoundary)
                {
                    var outdoorBc = this._refHBObj.BoundaryCondition.Obj as Outdoors;
                    this.BCOutdoor = new BoundaryConditionOutdoorViewModel(new List<Outdoors>() { outdoorBc }, (o) => _refHBObj.BoundaryCondition = o);
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



        // AFNCrack
        private AFNCrackViewModel _AFNCrack;

        public AFNCrackViewModel AFNCrack
        {
            get => _AFNCrack;
            private set => this.Set(() => _AFNCrack = value, nameof(AFNCrack));
        }
        #endregion






        private View.FaceProperty _control;
        private ModelProperties _libSource { get; set; }
        public Face Default { get; private set; }
        internal FacePropertyViewModel(View.FaceProperty panel)
        {
            this.Default = new Face("", new Face3D(new List<List<double>>()), FaceType.Wall, new Outdoors(), new FacePropertiesAbridged());
            _refHBObj = this.Default.DuplicateFace();
            _libSource = new ModelProperties(ModelEnergyProperties.Default, ModelRadianceProperties.Default);
            this._control = panel;
            Update(_libSource, new List<Face>() { _refHBObj });

        }

        public void Update(ModelProperties libSource, List<Face> objs)
        {
            this._libSource = libSource;
            this._refHBObj = objs.FirstOrDefault().DuplicateFace();
            var defaultEnergy = new FaceEnergyPropertiesAbridged();
            var defaultRadiance = new FaceRadiancePropertiesAbridged();
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

            // FaceType
            if (objs.Select(_ => _.FaceType).Distinct().Count() > 1)
                this.FaceTypeText = this.Varies;
            else
                this.FaceType = this._refHBObj.FaceType;


            // Modifier
            var mdf = _libSource.Radiance.Modifiers
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()
                .FirstOrDefault(_ => _.Identifier  == _refHBObj.Properties.Radiance.Modifier);
            this.Modifier = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.Modifier = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.Modifier).Distinct().Count() > 1)
                this.Modifier.SetBtnName(this.Varies);
            else
                this.Modifier.SetPropetyObj(mdf);

            // ModifierBlk
            var mdfblk = _libSource.Radiance.Modifiers
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Radiance.ModifierBlk);
            this.ModifierBlk = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.ModifierBlk = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.ModifierBlk).Distinct().Count() > 1)
                this.ModifierBlk.SetBtnName(this.Varies);
            else
                this.ModifierBlk.SetPropetyObj(mdfblk);


            // Construction
            var cts = _libSource.Energy.Constructions
                .OfType<HoneybeeSchema.Energy.IIDdEnergyBaseModel>()
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
         
            var afns = objs.Select(_ => _.Properties.Energy?.VentCrack).Distinct().ToList();
            this.AFNCrack = new AFNCrackViewModel(libSource, afns, _ => _refHBObj.Properties.Energy.VentCrack = _);
            
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

            }

            return this._hbObjs;
        }

        public ICommand ModifierCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ModifierManager(_libSource.Radiance, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Modifier.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand ModifierBlkCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ModifierManager(_libSource.Radiance, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.ModifierBlk.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand ConstructionCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ConstructionManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Construction.SetPropetyObj(dialog_rc[0]);
            }
        });

       
        public ICommand HBDataBtnClick => new RelayCommand(() => {

            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });
    }



}
