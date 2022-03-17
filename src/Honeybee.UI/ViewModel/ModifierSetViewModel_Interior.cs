using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class ModifierSetViewModel_Interior : ViewModelBase
    {

        #region Wall Set

        private SubModifierSetViewModel _wallIntSet;
        public SubModifierSetViewModel WallIntSet
        {
            get => _wallIntSet;
            set { this.Set(() => _wallIntSet = value, nameof(WallIntSet)); }
        }
        #endregion

        #region Floor Set

        private SubModifierSetViewModel _floorIntSet;
        public SubModifierSetViewModel FloorIntSet
        {
            get => _floorIntSet;
            set { this.Set(() => _floorIntSet = value, nameof(FloorIntSet)); }
        }

        #endregion

        #region Roof Set

        private SubModifierSetViewModel _roofIntSet;
        public SubModifierSetViewModel RoofIntSet
        {
            get => _roofIntSet;
            set { this.Set(() => _roofIntSet = value, nameof(RoofIntSet)); }
        }

        #endregion

        #region Aperture Set

        private SubModifierSetViewModel _ApertureIntSet;
        public SubModifierSetViewModel ApertureIntSet
        {
            get => _ApertureIntSet;
            set { this.Set(() => _ApertureIntSet = value, nameof(ApertureIntSet)); }
        }

        #endregion

        #region Door Set

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

        #endregion

        
        private InteriorSet _refHBObj { get; set; }
        private Control _control;
        //private ModelEnergyProperties _libSource { get; set; }
        public ModifierSetViewModel_Interior(Control control, ref ModelRadianceProperties libSource, InteriorSet ModifierSet)
        {
            _control = control;
            libSource.FillNulls();

            _refHBObj = ModifierSet?.Duplicate() ?? new InteriorSet();

            //Wall
            this.WallIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.Wall, (s) => _refHBObj.Wall = s?.Identifier);

            //Floor 
            this.FloorIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.Floor, (s) => _refHBObj.Floor = s?.Identifier);

            //Roof 
            this.RoofIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.Ceiling, (s) => _refHBObj.Ceiling = s?.Identifier);

            //Aperture 
            this.ApertureIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.Window, (s) => _refHBObj.Window = s?.Identifier);

            //Door 
            this.DoorIntGlassSet = new SubModifierSetViewModel(ref libSource, _refHBObj.GlassDoor, (s) => _refHBObj.GlassDoor = s?.Identifier);
            this.DoorIntSet = new SubModifierSetViewModel(ref libSource, _refHBObj.Door, (s) => _refHBObj.Door = s?.Identifier);
  
        }

        public InteriorSet GetHBObject()
        {
            var obj = this._refHBObj.Duplicate();
            return obj;
        }
    }



}
