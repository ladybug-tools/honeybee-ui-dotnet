using System.Collections.Generic;
using System;
using LB = LadybugDisplaySchema;

namespace Honeybee.UI
{
    [System.Obsolete("Use the new LadybugDisplaySchema.LegendColorSet instead")]
    /// <summary>
    /// https://github.com/ladybug-tools/ladybug/blob/master/ladybug/color.py
    /// </summary>
    public static class LegendColorSet
    {
        internal static Dictionary<string, List<LB.Color>> GetUserColorSets()
        {
            try
            {
                return LB.LegendColorSet.GetUserColorSets();
            }
            catch (Exception e)
            {
                Eto.Forms.MessageBox.Show(e.ToString());
                return null;
            }
        }

        internal static bool SaveUserColorSet(string name, List<LB.Color> colors)
        {
            try
            {
                var dic = GetUserColorSets();
                if (dic.ContainsKey(name))
                {
                    var rs = Eto.Forms.MessageBox.Show($"Name [{name}] already exists! Do you want to overwrite it?", Eto.Forms.MessageBoxButtons.YesNo, Eto.Forms.MessageBoxType.Question);
                    if (rs != Eto.Forms.DialogResult.Yes)
                        return false;
                }

                return LB.LegendColorSet.SaveUserColorSet(name, colors);
            }
            catch (Exception e)
            {
                Eto.Forms.MessageBox.Show(e.ToString());
                return false;
            }
        }


        public static Dictionary<string, List<LB.Color>> Presets => LB.LegendColorSet.Presets;


    }
}

