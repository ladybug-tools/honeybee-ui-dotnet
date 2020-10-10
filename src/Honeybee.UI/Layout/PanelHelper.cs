using Eto.Drawing;
using Eto.Forms;
using System;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public static partial class PanelHelper
    {
        public static DynamicLayout EmptyLayout { get; set; } = new DynamicLayout();

        
        //public DynamicLayout AdiabaticLayout { get; set; }
        //public DynamicLayout OutdoorLayout { get; set; } = new DynamicLayout();
        //public DynamicLayout SurfaceLayout { get; set; } = new DynamicLayout();
        //public Layout_BoundaryCondition()
        //{
        //    //var layout = this;
        //    //layout.Spacing = new Size(8, 8);

        //    //var bc = boundaryCondition.Obj;
        //    //if (bc is Ground || bc is Adiabatic)
        //    //{
        //    //    //bcGround.
        //    //}
        //    //else if (bc is Outdoors bcOutdoors)
        //    //{

        //    //}

        //}
        public static ModelProperties ModelProperties { get; set; }

        public static DynamicLayout GetLayout(AnyOf boundaryCondition)
        {
            var bc = boundaryCondition.Obj;
            if (bc is Surface)
            {
                return EmptyLayout;
            }
            else if (bc is Outdoors bcOutdoors)
            {
                return CreateOutdoorLayout(bcOutdoors);
            }
            else
            {
                return EmptyLayout;
            }
            
        }
        //private static DynamicLayout EmptyLayout()
        //{
        //    return new DynamicLayout();
        //}

        public static DynamicLayout CreateOutdoorLayout(Outdoors bcOutdoors)
        {
            var layout = new DynamicLayout() { Spacing = new Size(8, 8) };
            var sun_CB = new CheckBox();
            sun_CB.CheckedBinding.Bind(bcOutdoors, v => v.SunExposure);
            sun_CB.Text = "Sun Exposure";

            var wind_CB = new CheckBox();
            wind_CB.CheckedBinding.Bind(bcOutdoors, v => v.WindExposure);
            wind_CB.Text = "Wind Exposure";

            // View Factor number
            var vF_NS = new NumericStepper();
            vF_NS.DecimalPlaces = 2;
            vF_NS.MinValue = 0;
            vF_NS.MaxValue = 1;
            vF_NS.ValueBinding.Bind(
                () => {
                    if (bcOutdoors.ViewFactor.Obj is double v)
                        return v;
                    return 1.0;
                },
                (d) => bcOutdoors.ViewFactor = d
                );
            vF_NS.Enabled = bcOutdoors.ViewFactor.Obj is double;
            vF_NS.EnabledChanged += (s, e) =>
            {
                if (vF_NS.Enabled)
                {
                    bcOutdoors.ViewFactor = vF_NS.Value;
                }
                else
                {
                    bcOutdoors.ViewFactor = new Autocalculate();
                }
            };

            // View Factor Autocalculate
            var vF_CB = new CheckBox();
            vF_CB.CheckedBinding.Bind(bcOutdoors, v => v.ViewFactor.Obj is Autocalculate);
            vF_CB.Text = "Autocalculate";
            vF_CB.CheckedChanged += (s, e) =>
            {
                vF_NS.Enabled = !vF_CB.Checked.Value;
                if (vF_CB.Checked.Value)
                    bcOutdoors.ViewFactor = new Autocalculate();
            };

            layout.AddRow("Outdoors:");
            layout.AddRow(sun_CB);
            layout.AddRow(wind_CB);
            layout.AddRow("View Factor:");
            layout.AddRow(vF_CB);
            layout.AddRow(vF_NS);

            return layout;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Room.Instance.UpdatePanel() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <param name="redrawDisplay"></param>
        /// <returns></returns>
        public static Panel UpdateRoomPanel(ModelProperties libSource, Room HoneybeeObj, Action<string> geometryReset = default, Action<string> redrawDisplay = default)
        {
            ModelProperties = libSource;
            var panel = View.Room.Instance;
            panel.UpdatePanel(HoneybeeObj, geometryReset, redrawDisplay);
            return panel;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Face.Instance.UpdatePanel() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel UpdateFacePanel(ModelProperties libSource, Face HoneybeeObj, System.Action<string> geometryReset = default)
        {
            ModelProperties = libSource;
            var panel = View.Face.Instance;
            panel.UpdatePanel( HoneybeeObj, geometryReset);
            return panel;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Face.Instance.UpdatePanel() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel UpdateDoorPanel(ModelProperties libSource, Door HoneybeeObj, System.Action<string> geometryReset = default)
        {
            ModelProperties = libSource;
            var panel = View.Door.Instance;
            panel.UpdatePanel(HoneybeeObj, geometryReset);
            return panel;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Face.Instance.UpdatePanel() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel UpdateAperturePanel(ModelProperties libSource, Aperture HoneybeeObj, System.Action<string> geometryReset = default)
        {
            ModelProperties = libSource;
            var panel = View.Aperture.Instance;
            panel.UpdatePanel(HoneybeeObj, geometryReset);
            return panel;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Face.Instance.UpdatePanel() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel UpdateShadePanel(ModelProperties libSource, Shade HoneybeeObj, System.Action<string> geometryReset = default)
        {
            ModelProperties = libSource;
            var panel = View.Shade.Instance;
            panel.UpdatePanel(HoneybeeObj, geometryReset);
            return panel;
        }
    }
}
