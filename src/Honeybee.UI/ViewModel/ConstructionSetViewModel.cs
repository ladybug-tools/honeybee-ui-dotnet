using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class ConstructionSetViewModel : ViewModelBase
    {

        public string Name
        {
            get => _refHBObj.DisplayName ?? _refHBObj.Identifier;
            set { this.Set(() => _refHBObj.DisplayName = value, nameof(Name)); }
        }

        #region Wall Set

        private SubConstructionSetViewModel _wallExtSet;
        public SubConstructionSetViewModel WallExtSet
        {
            get => _wallExtSet;
            set { this.Set(() => _wallExtSet = value, nameof(WallExtSet)); }
        }

        private SubConstructionSetViewModel _wallGndSet;
        public SubConstructionSetViewModel WallGndSet
        {
            get => _wallGndSet;
            set { this.Set(() => _wallGndSet = value, nameof(WallGndSet)); }
        }

        private SubConstructionSetViewModel _wallIntSet;
        public SubConstructionSetViewModel WallIntSet
        {
            get => _wallIntSet;
            set { this.Set(() => _wallIntSet = value, nameof(WallIntSet)); }
        }
        #endregion

        #region Floor Set

        private SubConstructionSetViewModel _floorExtSet;
        public SubConstructionSetViewModel FloorExtSet
        {
            get => _floorExtSet;
            set { this.Set(() => _floorExtSet = value, nameof(FloorExtSet)); }
        }

        private SubConstructionSetViewModel _floorGndSet;
        public SubConstructionSetViewModel FloorGndSet
        {
            get => _floorGndSet;
            set { this.Set(() => _floorGndSet = value, nameof(FloorGndSet)); }
        }

        private SubConstructionSetViewModel _floorIntSet;
        public SubConstructionSetViewModel FloorIntSet
        {
            get => _floorIntSet;
            set { this.Set(() => _floorIntSet = value, nameof(FloorIntSet)); }
        }

        #endregion

        #region Roof Set

        private SubConstructionSetViewModel _roofExtSet;
        public SubConstructionSetViewModel RoofExtSet
        {
            get => _roofExtSet;
            set { this.Set(() => _roofExtSet = value, nameof(RoofExtSet)); }
        }

        private SubConstructionSetViewModel _roofGndSet;
        public SubConstructionSetViewModel RoofGndSet
        {
            get => _roofGndSet;
            set { this.Set(() => _roofGndSet = value, nameof(RoofGndSet)); }
        }

        private SubConstructionSetViewModel _roofIntSet;
        public SubConstructionSetViewModel RoofIntSet
        {
            get => _roofIntSet;
            set { this.Set(() => _roofIntSet = value, nameof(RoofIntSet)); }
        }

        #endregion

        #region Aperture Set

        private SubConstructionSetViewModel _ApertureExtSet;
        public SubConstructionSetViewModel ApertureExtSet
        {
            get => _ApertureExtSet;
            set { this.Set(() => _ApertureExtSet = value, nameof(ApertureExtSet)); }
        }

        private SubConstructionSetViewModel _ApertureOptSet;
        public SubConstructionSetViewModel ApertureOptSet
        {
            get => _ApertureOptSet;
            set { this.Set(() => _ApertureOptSet = value, nameof(ApertureOptSet)); }
        }

        private SubConstructionSetViewModel _ApertureSkySet;
        public SubConstructionSetViewModel ApertureSkySet
        {
            get => _ApertureSkySet;
            set { this.Set(() => _ApertureSkySet = value, nameof(ApertureSkySet)); }
        }

        private SubConstructionSetViewModel _ApertureIntSet;
        public SubConstructionSetViewModel ApertureIntSet
        {
            get => _ApertureIntSet;
            set { this.Set(() => _ApertureIntSet = value, nameof(ApertureIntSet)); }
        }

        #endregion

        #region Door Set

        private SubConstructionSetViewModel _DoorExtSet;
        public SubConstructionSetViewModel DoorExtSet
        {
            get => _DoorExtSet;
            set { this.Set(() => _DoorExtSet = value, nameof(DoorExtSet)); }
        }

        private SubConstructionSetViewModel _DoorExtGlassSet;
        public SubConstructionSetViewModel DoorExtGlassSet
        {
            get => _DoorExtGlassSet;
            set { this.Set(() => _DoorExtGlassSet = value, nameof(DoorExtGlassSet)); }
        }

        private SubConstructionSetViewModel _DoorIntGlassSet;
        public SubConstructionSetViewModel DoorIntGlassSet
        {
            get => _DoorIntGlassSet;
            set { this.Set(() => _DoorIntGlassSet = value, nameof(DoorIntGlassSet)); }
        }

        private SubConstructionSetViewModel _DoorIntSet;
        public SubConstructionSetViewModel DoorIntSet
        {
            get => _DoorIntSet;
            set { this.Set(() => _DoorIntSet = value, nameof(DoorIntSet)); }
        }

        private SubConstructionSetViewModel _DoorOverheadSet;
        public SubConstructionSetViewModel DoorOverheadSet
        {
            get => _DoorOverheadSet;
            set { this.Set(() => _DoorOverheadSet = value, nameof(DoorOverheadSet)); }
        }
        #endregion

        #region Shade/ Air boundary Set

        private SubConstructionSetViewModel _ShadeSet;
        public SubConstructionSetViewModel ShadeSet
        {
            get => _ShadeSet;
            set { this.Set(() => _ShadeSet = value, nameof(ShadeSet)); }
        }

        private SubConstructionSetViewModel _AirBoundarySet;
        public SubConstructionSetViewModel AirBoundarySet
        {
            get => _AirBoundarySet;
            set { this.Set(() => _AirBoundarySet = value, nameof(AirBoundarySet)); }
        }

        #endregion

        private HB.ConstructionSetAbridged _refHBObj { get; set; }
        private Control _control;
        //private ModelEnergyProperties _libSource { get; set; }
        public ConstructionSetViewModel(Control control, ref ModelEnergyProperties libSource, HB.ConstructionSetAbridged constructionSet)
        {
            _control = control;
            libSource.FillNulls();

            //this._libSource = libSource;
            var cSet = constructionSet?.DuplicateConstructionSetAbridged() ?? new HB.ConstructionSetAbridged(identifier: System.Guid.NewGuid().ToString());
            _refHBObj = cSet;

            this.Name = _refHBObj.DisplayName ?? _refHBObj.Identifier;

            //Wall
            _refHBObj.WallSet = _refHBObj.WallSet ?? new WallConstructionSetAbridged();
            this.WallExtSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.WallSet.ExteriorConstruction, (s) => _refHBObj.WallSet.ExteriorConstruction = s?.Identifier);
            this.WallGndSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.WallSet.GroundConstruction, (s) => _refHBObj.WallSet.GroundConstruction = s?.Identifier);
            this.WallIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.WallSet.InteriorConstruction, (s) => _refHBObj.WallSet.InteriorConstruction = s?.Identifier);

            //Floor 
            _refHBObj.FloorSet = _refHBObj.FloorSet ?? new FloorConstructionSetAbridged();
            this.FloorExtSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.FloorSet.ExteriorConstruction, (s) => _refHBObj.FloorSet.ExteriorConstruction = s?.Identifier);
            this.FloorGndSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.FloorSet.GroundConstruction, (s) => _refHBObj.FloorSet.GroundConstruction = s?.Identifier);
            this.FloorIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.FloorSet.InteriorConstruction, (s) => _refHBObj.FloorSet.InteriorConstruction = s?.Identifier);

            //Roof 
            _refHBObj.RoofCeilingSet = _refHBObj.RoofCeilingSet ?? new RoofCeilingConstructionSetAbridged();
            this.RoofExtSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.RoofCeilingSet.ExteriorConstruction, (s) => _refHBObj.RoofCeilingSet.ExteriorConstruction = s?.Identifier);
            this.RoofGndSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.RoofCeilingSet.GroundConstruction, (s) => _refHBObj.RoofCeilingSet.GroundConstruction = s?.Identifier);
            this.RoofIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.RoofCeilingSet.InteriorConstruction, (s) => _refHBObj.RoofCeilingSet.InteriorConstruction = s?.Identifier);

            //Aperture 
            _refHBObj.ApertureSet = _refHBObj.ApertureSet ?? new ApertureConstructionSetAbridged();
            this.ApertureExtSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.ApertureSet.WindowConstruction, (s) => _refHBObj.ApertureSet.WindowConstruction = s?.Identifier);
            this.ApertureOptSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.ApertureSet.OperableConstruction, (s) => _refHBObj.ApertureSet.OperableConstruction = s?.Identifier);
            this.ApertureSkySet = new SubConstructionSetViewModel(ref libSource, _refHBObj.ApertureSet.SkylightConstruction, (s) => _refHBObj.ApertureSet.SkylightConstruction = s?.Identifier);
            this.ApertureIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.ApertureSet.InteriorConstruction, (s) => _refHBObj.ApertureSet.InteriorConstruction = s?.Identifier);

            //Door 
            _refHBObj.DoorSet = _refHBObj.DoorSet ?? new DoorConstructionSetAbridged();
            this.DoorExtSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.DoorSet.ExteriorConstruction, (s) => _refHBObj.DoorSet.ExteriorConstruction = s?.Identifier);
            this.DoorExtGlassSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.DoorSet.ExteriorGlassConstruction, (s) => _refHBObj.DoorSet.ExteriorGlassConstruction = s?.Identifier);
            this.DoorIntGlassSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.DoorSet.InteriorGlassConstruction, (s) => _refHBObj.DoorSet.InteriorGlassConstruction = s?.Identifier);
            this.DoorIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.DoorSet.InteriorConstruction, (s) => _refHBObj.DoorSet.InteriorConstruction = s?.Identifier);
            this.DoorOverheadSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.DoorSet.OverheadConstruction, (s) => _refHBObj.DoorSet.OverheadConstruction = s?.Identifier);

            //Shade 
            this.ShadeSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.ShadeConstruction, (s) => _refHBObj.ShadeConstruction = s?.Identifier);

            //Air boundary 
            this.AirBoundarySet = new SubConstructionSetViewModel(ref libSource, _refHBObj.AirBoundaryConstruction, (s) => _refHBObj.AirBoundaryConstruction = s?.Identifier);


        }

        public ICommand HBDataBtnClick => new RelayCommand(() =>
        {
            Honeybee.UI.Dialog_Message.Show(this._control, _refHBObj.ToJson(true), "Schema Data");
        });

        public ConstructionSetAbridged GetHBObject()
        {
            var obj = this._refHBObj.DuplicateConstructionSetAbridged();
            
            return obj;
        }
    }



}
