using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class ConstructionSetViewModel_Interior : ViewModelBase
    {

        #region Wall Set

        private SubConstructionSetViewModel _wallIntSet;
        public SubConstructionSetViewModel WallIntSet
        {
            get => _wallIntSet;
            set { this.Set(() => _wallIntSet = value, nameof(WallIntSet)); }
        }
        #endregion

        #region Floor Set

        private SubConstructionSetViewModel _floorIntSet;
        public SubConstructionSetViewModel FloorIntSet
        {
            get => _floorIntSet;
            set { this.Set(() => _floorIntSet = value, nameof(FloorIntSet)); }
        }

        #endregion

        #region Roof Set

        private SubConstructionSetViewModel _roofIntSet;
        public SubConstructionSetViewModel RoofIntSet
        {
            get => _roofIntSet;
            set { this.Set(() => _roofIntSet = value, nameof(RoofIntSet)); }
        }

        #endregion

        #region Aperture Set

        private SubConstructionSetViewModel _ApertureIntSet;
        public SubConstructionSetViewModel ApertureIntSet
        {
            get => _ApertureIntSet;
            set { this.Set(() => _ApertureIntSet = value, nameof(ApertureIntSet)); }
        }

        #endregion

        #region Door Set

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

        #endregion

        
        private InteriorSet _refHBObj { get; set; }
        private Control _control;
        //private ModelEnergyProperties _libSource { get; set; }
        public ConstructionSetViewModel_Interior(Control control, ref ModelEnergyProperties libSource, InteriorSet constructionSet)
        {
            _control = control;
            libSource.FillNulls();

            _refHBObj = constructionSet?.Duplicate() ?? new InteriorSet();

            //Wall
            this.WallIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.Wall, (s) => _refHBObj.Wall = s?.Identifier);

            //Floor 
            this.FloorIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.Floor, (s) => _refHBObj.Floor = s?.Identifier);

            //Roof 
            this.RoofIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.Ceiling, (s) => _refHBObj.Ceiling = s?.Identifier);

            //Aperture 
            this.ApertureIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.Window, (s) => _refHBObj.Window = s?.Identifier);

            //Door 
            this.DoorIntGlassSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.GlassDoor, (s) => _refHBObj.GlassDoor = s?.Identifier);
            this.DoorIntSet = new SubConstructionSetViewModel(ref libSource, _refHBObj.Door, (s) => _refHBObj.Door = s?.Identifier);
  
        }

        public InteriorSet GetHBObject()
        {
            var obj = this._refHBObj.Duplicate();
            return obj;
        }
    }



}
