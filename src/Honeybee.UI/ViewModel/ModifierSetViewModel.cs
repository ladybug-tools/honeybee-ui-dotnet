using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class ModifierSetViewModel : ViewModelBase
    {

        public string Name
        {
            get => _refHBObj.DisplayName ?? _refHBObj.Identifier;
            set { this.Set(() => _refHBObj.DisplayName = value, nameof(Name)); }
        }

        #region Wall Set

        private SubModifierSetViewModel _wallExtSet;
        public SubModifierSetViewModel WallExtSet
        {
            get => _wallExtSet;
            set { this.Set(() => _wallExtSet = value, nameof(WallExtSet)); }
        }


        private SubModifierSetViewModel _wallIntSet;
        public SubModifierSetViewModel WallIntSet
        {
            get => _wallIntSet;
            set { this.Set(() => _wallIntSet = value, nameof(WallIntSet)); }
        }
        #endregion

        #region Floor Set

        private SubModifierSetViewModel _floorExtSet;
        public SubModifierSetViewModel FloorExtSet
        {
            get => _floorExtSet;
            set { this.Set(() => _floorExtSet = value, nameof(FloorExtSet)); }
        }

 
        private SubModifierSetViewModel _floorIntSet;
        public SubModifierSetViewModel FloorIntSet
        {
            get => _floorIntSet;
            set { this.Set(() => _floorIntSet = value, nameof(FloorIntSet)); }
        }

        #endregion

        #region Roof Set

        private SubModifierSetViewModel _roofExtSet;
        public SubModifierSetViewModel RoofExtSet
        {
            get => _roofExtSet;
            set { this.Set(() => _roofExtSet = value, nameof(RoofExtSet)); }
        }

        private SubModifierSetViewModel _roofIntSet;
        public SubModifierSetViewModel RoofIntSet
        {
            get => _roofIntSet;
            set { this.Set(() => _roofIntSet = value, nameof(RoofIntSet)); }
        }

        #endregion

        #region Aperture Set

        private SubModifierSetViewModel _ApertureExtSet;
        public SubModifierSetViewModel ApertureExtSet
        {
            get => _ApertureExtSet;
            set { this.Set(() => _ApertureExtSet = value, nameof(ApertureExtSet)); }
        }

        private SubModifierSetViewModel _ApertureOptSet;
        public SubModifierSetViewModel ApertureOptSet
        {
            get => _ApertureOptSet;
            set { this.Set(() => _ApertureOptSet = value, nameof(ApertureOptSet)); }
        }

        private SubModifierSetViewModel _ApertureSkySet;
        public SubModifierSetViewModel ApertureSkySet
        {
            get => _ApertureSkySet;
            set { this.Set(() => _ApertureSkySet = value, nameof(ApertureSkySet)); }
        }

        private SubModifierSetViewModel _ApertureIntSet;
        public SubModifierSetViewModel ApertureIntSet
        {
            get => _ApertureIntSet;
            set { this.Set(() => _ApertureIntSet = value, nameof(ApertureIntSet)); }
        }

        #endregion

        #region Door Set

        private SubModifierSetViewModel _DoorExtSet;
        public SubModifierSetViewModel DoorExtSet
        {
            get => _DoorExtSet;
            set { this.Set(() => _DoorExtSet = value, nameof(DoorExtSet)); }
        }

        private SubModifierSetViewModel _DoorExtGlassSet;
        public SubModifierSetViewModel DoorExtGlassSet
        {
            get => _DoorExtGlassSet;
            set { this.Set(() => _DoorExtGlassSet = value, nameof(DoorExtGlassSet)); }
        }

        private SubModifierSetViewModel _DoorIntGlassSet;
        public SubModifierSetViewModel DoorIntGlassSet
        {
            get => _DoorIntGlassSet;
            set { this.Set(() => _DoorIntGlassSet = value, nameof(DoorIntGlassSet)); }
        }

        private SubModifierSetViewModel _DoorIntSet;
        public SubModifierSetViewModel DoorIntSet
        {
            get => _DoorIntSet;
            set { this.Set(() => _DoorIntSet = value, nameof(DoorIntSet)); }
        }

        private SubModifierSetViewModel _DoorOverheadSet;
        public SubModifierSetViewModel DoorOverheadSet
        {
            get => _DoorOverheadSet;
            set { this.Set(() => _DoorOverheadSet = value, nameof(DoorOverheadSet)); }
        }
        #endregion

        #region Shade/ Air boundary Set

        private SubModifierSetViewModel _ShadeExtSet;
        public SubModifierSetViewModel ShadeExtSet
        {
            get => _ShadeExtSet;
            set { this.Set(() => _ShadeExtSet = value, nameof(ShadeExtSet)); }
        }
        private SubModifierSetViewModel _ShadeIntSet;
        public SubModifierSetViewModel ShadeIntSet
        {
            get => _ShadeIntSet;
            set { this.Set(() => _ShadeIntSet = value, nameof(ShadeIntSet)); }
        }

        private SubModifierSetViewModel _AirBoundarySet;
        public SubModifierSetViewModel AirBoundarySet
        {
            get => _AirBoundarySet;
            set { this.Set(() => _AirBoundarySet = value, nameof(AirBoundarySet)); }
        }

        #endregion

        private HB.ModifierSetAbridged _refHBObj { get; set; }
        private Control _control;
        //private ModelEnergyProperties _libSource { get; set; }
        public ModifierSetViewModel(Control control, ref ModelRadianceProperties libSource, HB.ModifierSetAbridged ModifierSet)
        {
            _control = control;
            libSource.FillNulls();

            //this._libSource = libSource;
            var cSet = ModifierSet?.DuplicateModifierSetAbridged() ?? new HB.ModifierSetAbridged(identifier: System.Guid.NewGuid().ToString().Substring(0, 5));
            _refHBObj = cSet;

            this.Name = _refHBObj.DisplayName ?? _refHBObj.Identifier;

            //Wall
            _refHBObj.WallSet = _refHBObj.WallSet ?? new WallModifierSetAbridged();
            this.WallExtSet = new SubModifierSetViewModel(ref libSource, _refHBObj.WallSet.ExteriorModifier, (s) => _refHBObj.WallSet.ExteriorModifier = s?.Identifier);
            this.WallIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.WallSet.InteriorModifier, (s) => _refHBObj.WallSet.InteriorModifier = s?.Identifier);

            //Floor 
            _refHBObj.FloorSet = _refHBObj.FloorSet ?? new FloorModifierSetAbridged();
            this.FloorExtSet = new SubModifierSetViewModel(ref libSource, _refHBObj.FloorSet.ExteriorModifier, (s) => _refHBObj.FloorSet.ExteriorModifier = s?.Identifier);
            this.FloorIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.FloorSet.InteriorModifier, (s) => _refHBObj.FloorSet.InteriorModifier = s?.Identifier);

            //Roof 
            _refHBObj.RoofCeilingSet = _refHBObj.RoofCeilingSet ?? new RoofCeilingModifierSetAbridged();
            this.RoofExtSet = new SubModifierSetViewModel(ref libSource, _refHBObj.RoofCeilingSet.ExteriorModifier, (s) => _refHBObj.RoofCeilingSet.ExteriorModifier = s?.Identifier);
            this.RoofIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.RoofCeilingSet.InteriorModifier, (s) => _refHBObj.RoofCeilingSet.InteriorModifier = s?.Identifier);

            //Aperture 
            _refHBObj.ApertureSet = _refHBObj.ApertureSet ?? new ApertureModifierSetAbridged();
            this.ApertureExtSet = new SubModifierSetViewModel(ref libSource, _refHBObj.ApertureSet.WindowModifier, (s) => _refHBObj.ApertureSet.WindowModifier = s?.Identifier);
            this.ApertureOptSet = new SubModifierSetViewModel(ref libSource, _refHBObj.ApertureSet.OperableModifier, (s) => _refHBObj.ApertureSet.OperableModifier = s?.Identifier);
            this.ApertureSkySet = new SubModifierSetViewModel(ref libSource, _refHBObj.ApertureSet.SkylightModifier, (s) => _refHBObj.ApertureSet.SkylightModifier = s?.Identifier);
            this.ApertureIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.ApertureSet.InteriorModifier, (s) => _refHBObj.ApertureSet.InteriorModifier = s?.Identifier);

            //Door 
            _refHBObj.DoorSet = _refHBObj.DoorSet ?? new DoorModifierSetAbridged();
            this.DoorExtSet = new SubModifierSetViewModel(ref libSource, _refHBObj.DoorSet.ExteriorModifier, (s) => _refHBObj.DoorSet.ExteriorModifier = s?.Identifier);
            this.DoorExtGlassSet = new SubModifierSetViewModel(ref libSource, _refHBObj.DoorSet.ExteriorGlassModifier, (s) => _refHBObj.DoorSet.ExteriorGlassModifier = s?.Identifier);
            this.DoorIntGlassSet = new SubModifierSetViewModel(ref libSource, _refHBObj.DoorSet.InteriorGlassModifier, (s) => _refHBObj.DoorSet.InteriorGlassModifier = s?.Identifier);
            this.DoorIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.DoorSet.InteriorModifier, (s) => _refHBObj.DoorSet.InteriorModifier = s?.Identifier);
            this.DoorOverheadSet = new SubModifierSetViewModel(ref libSource, _refHBObj.DoorSet.OverheadModifier, (s) => _refHBObj.DoorSet.OverheadModifier = s?.Identifier);

            //Shade 
            this.ShadeExtSet = new SubModifierSetViewModel(ref libSource, _refHBObj.ShadeSet.ExteriorModifier, (s) => _refHBObj.ShadeSet.ExteriorModifier = s?.Identifier);
            this.ShadeIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.ShadeSet.InteriorModifier, (s) => _refHBObj.ShadeSet.InteriorModifier = s?.Identifier);

            //Air boundary 
            this.AirBoundarySet = new SubModifierSetViewModel(ref libSource, _refHBObj.AirBoundaryModifier, (s) => _refHBObj.AirBoundaryModifier = s?.Identifier);


        }

        public ICommand HBDataBtnClick => new RelayCommand(() =>
        {
            Honeybee.UI.Dialog_Message.Show(this._control, _refHBObj.ToJson(true), "Schema Data");
        });

        public ModifierSetAbridged GetHBObject()
        {
            var obj = this._refHBObj.DuplicateModifierSetAbridged();
            
            return obj;
        }
    }



}
