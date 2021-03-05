using Eto.Drawing;
using Eto.Forms;

namespace Honeybee.UI
{
    public static partial class Config
    {
        /// <summary>
        /// Set this to host control if you want dialogs pop up in the middle of the host control.
        /// </summary>
        public static Control Owner;

        /// <summary>
        /// Override default HoneybeeIcon
        /// </summary>
        public static Icon HoneybeeIcon;

        /// <summary>
        /// Override default plugin name that is used for dialog title 
        /// </summary>
        public static string PluginName;

    }
}
