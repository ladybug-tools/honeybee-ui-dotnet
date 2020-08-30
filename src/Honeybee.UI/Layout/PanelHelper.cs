using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HB = HoneybeeSchema;

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
        //    //if (bc is HB.Ground || bc is HB.Adiabatic)
        //    //{
        //    //    //bcGround.
        //    //}
        //    //else if (bc is HB.Outdoors bcOutdoors)
        //    //{

        //    //}

        //}

        public static DynamicLayout GetLayout(HB.AnyOf boundaryCondition)
        {
            var bc = boundaryCondition.Obj;
            if (bc is HB.Surface)
            {
                return EmptyLayout;
            }
            else if (bc is HB.Outdoors bcOutdoors)
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

        public static DynamicLayout CreateOutdoorLayout(HB.Outdoors bcOutdoors)
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
                    bcOutdoors.ViewFactor = new HB.Autocalculate();
                }
            };

            // View Factor Autocalculate
            var vF_CB = new CheckBox();
            vF_CB.CheckedBinding.Bind(bcOutdoors, v => v.ViewFactor.Obj is HB.Autocalculate);
            vF_CB.Text = "Autocalculate";
            vF_CB.CheckedChanged += (s, e) =>
            {
                vF_NS.Enabled = !vF_CB.Checked.Value;
                if (vF_CB.Checked.Value)
                    bcOutdoors.ViewFactor = new HB.Autocalculate();
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
        /// Use Honeybee.UI.View.Room.Instance.UpdateRoomView() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <param name="redrawDisplay"></param>
        /// <returns></returns>
        public static Panel UpdateRoomPanel(HB.Room HoneybeeObj, Action<string> geometryReset = default, Action<string> redrawDisplay = default)
        {
            var panel = View.Room.Instance;
            panel.UpdateRoomView(HoneybeeObj, geometryReset, redrawDisplay);
            return panel;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Face.Instance.UpdateRoomView() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel UpdateFacePanel(HB.Face HoneybeeObj, System.Action<string> geometryReset = default)
        {
            var panel = View.Face.Instance;
            panel.UpdateRoomView(HoneybeeObj, geometryReset);
            return panel;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Face.Instance.UpdateRoomView() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel UpdateDoorPanel(HB.Door HoneybeeObj, System.Action<string> geometryReset = default)
        {
            var panel = View.Door.Instance;
            panel.UpdateRoomView(HoneybeeObj, geometryReset);
            return panel;
        }

        /// <summary>
        /// This old method for not breaking depending programs.
        /// Use Honeybee.UI.View.Face.Instance.UpdateRoomView() instead.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel UpdateAperturePanel(HB.Aperture HoneybeeObj, System.Action<string> geometryReset = default)
        {
            var panel = View.Aperture.Instance;
            panel.UpdateRoomView(HoneybeeObj, geometryReset);
            return panel;
        }
    }
}
