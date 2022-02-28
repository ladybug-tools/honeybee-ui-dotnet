using System.Collections.Generic;
using HoneybeeSchema;
using System.Linq;
using System;

namespace Honeybee.UI
{
    /// <summary>
    /// https://github.com/ladybug-tools/ladybug/blob/master/ladybug/color.py
    /// </summary>
    public static class LegendColorSet
    {
        private static string GetUserPresetFilePath()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            folderPath = System.IO.Path.Combine(folderPath, ".pollination");
            if (!System.IO.Directory.Exists(folderPath)) 
                System.IO.Directory.CreateDirectory(folderPath);

            var userFile = System.IO.Path.Combine(folderPath, "LegendPreset.txt");
            return userFile;
        }
        internal static Dictionary<string, List<Color>> GetUserColorSets()
        {
            try
            {
                var userFile = GetUserPresetFilePath();
               
                if (!System.IO.File.Exists(userFile))
                    return new Dictionary<string, List<HoneybeeSchema.Color>>();

                var json = System.IO.File.ReadAllText(userFile);
                var legend = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<HoneybeeSchema.Color>>>(json);
                return legend;

            }
            catch (Exception e)
            {
                Eto.Forms.MessageBox.Show(e.ToString());
                return null;
            }
        }

        internal static bool SaveUserColorSet(string name, List<Color> colors)
        {
            try
            {
                var userFile = GetUserPresetFilePath();
                var dic = new Dictionary<string, List<HoneybeeSchema.Color>>();
                if (System.IO.File.Exists(userFile))
                    dic = GetUserColorSets();

                if (dic.ContainsKey(name))
                {
                    var rs = Eto.Forms.MessageBox.Show($"Name [{name}] already exists! Do you want to overwrite it?", Eto.Forms.MessageBoxButtons.YesNo, Eto.Forms.MessageBoxType.Question);
                    if (rs != Eto.Forms.DialogResult.Yes)
                        return false;
                    dic[name] = colors;
                }
                else
                {
                    dic.Add(name, colors);
                }
               
             
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                System.IO.File.WriteAllText(userFile, json);
                return true;
                
            }
            catch (Exception e)
            {
                Eto.Forms.MessageBox.Show(e.ToString());
                return false;
            }
        }


        public static Dictionary<string, List<Color>> Presets = new Dictionary<string, List<Color>>()
        {
            {"0: Original Ladybug", _0 },
            {"1: Nuanced  Ladybug", _1 },
            {"2: Multi-colored", _2 },
            {"3: Ecotect colors", _3 },
            {"4: View analysis", _4 },

            {"5: Shadow study", _5 },
            {"6: Spatial glare", _6 },
            {"7: Annual comfort", _7 },
            {"8: Thermal comfort", _8 },
            {"9: Peak load balance", _9 },

            {"10: Heat sensation", _10 },
            {"11: Cold sensation", _11 },
            {"12: Benefit / harm study", _12 },
            {"13: Harm colors", _13 },
            {"14: Benefit colors", _14 },

            {"15: Shade benefit / harm colors", _15 },
            {"16: Shade harm colors", _16 },
            {"17: Shade benefit colors", _17 },
            {"18: Energy Balance colors", _18 },
            {"19: Energy Balance colors with storage", _19 },

            {"20: THERM colors", _20 },
            {"21: Cloud cover", _21 },
            {"22: Black to white", _22 },
            {"23: Blue to Green to Red", _23 },
            {"24: Multi-colored", _24 },

            {"25: Multi-colored2", _25 },

        };

        //        """Ladybug Color-range repository.
        //    A list of default Ladybug colorsets for color range:
        //        * 0 - original Ladybug
        //        * 1 - nuanced Ladybug
        //        * 2 - Multi-colored Ladybug
        //        * 3 - View Analysis 1
        //        * 4 - View Analysis 2 (Red, Green, Blue)
        //        * 5 - Sunlight Hours
        //        * 6 - ecotect
        //        * 7 - thermal Comfort Percentage
        //        * 8 - thermal Comfort Colors
        //        * 9 - thermal Comfort Colors(UTCI)
        //        * 10 - Hot Hours
        //        * 11 - Cold Hours
        //        * 12 - Shade Benefit/Harm
        //        * 13 - thermal Comfort Colors v2(UTCI)
        //        * 14 - Shade Harm
        //        * 15 - Shade Benefit
        //        * 16 - Black to White
        //        * 17 - CFD Colors 1
        //        * 18 - CFD Colors 2
        //        * 19 - Energy Balance
        //        * 20 - THERM
        //        * 21 - Cloud Cover
      

        //0
        private static List<Color> _0 => new List<Color>()
        {
            new Color(75,107,169),
            new Color(115,147,202),
            new Color(170,200,247),
            new Color(193,213,208),
            new Color(245,239,103),
            new Color(252,230,74),
            new Color(239,156,21),
            new Color(234,123,0),
            new Color(234,74,0),
            new Color(234,38,0)
        };
        //1
        private static List<Color> _1 => new List<Color>()
        {
            new Color(49, 54, 149), new Color(69, 117, 180), new Color(116, 173, 209), new Color(171, 217, 233),
            new Color(224, 243, 248), new Color(255, 255, 191), new Color(254, 224, 144), new Color(253, 174, 97),
            new Color(244, 109, 67), new Color(215, 48, 39), new Color(165, 0, 38)
        };
        //2
        private static List<Color> _2 => _multicolored;
        //3
        private static List<Color> _3 => new List<Color>()
        {
            new Color(0, 0, 255), new Color(53, 0, 202), new Color(107, 0, 148), new Color(160, 0, 95), new Color(214, 0, 41),
            new Color(255, 12, 0), new Color(255, 66, 0), new Color(255, 119, 0), new Color(255, 173, 0), new Color(255, 226, 0),
            new Color(255, 255, 0)
        };

        //4
        private static List<Color> _4 => new List<Color>()
        {
            new Color(255, 20, 147), new Color(240, 47, 145), new Color(203, 117, 139), new Color(160, 196, 133),
            new Color(132, 248, 129), new Color(124, 253, 132), new Color(96, 239, 160), new Color(53, 217, 203),
            new Color(15, 198, 240), new Color(0, 191, 255)
        };

        //5
        private static List<Color> _5 => new List<Color>()
        {
            new Color(55, 55, 55), new Color(235, 235, 235)
        };

        //6
        private static List<Color> _6 => new List<Color>()
        {
            new Color(156, 217, 255), new Color(255, 243, 77), new Color(255, 115, 0), new Color(255, 0, 0), new Color(0, 0, 0)
        };

        //7
        private static List<Color> _7 => new List<Color>()
        {
            new Color(0, 0, 0), new Color(110, 0, 153), new Color(255, 0, 0), new Color(255, 255, 102), new Color(255, 255, 255)
        };

        //8
        private static List<Color> _8 => _thermalcomfort;

        //9
        private static List<Color> _9 => new List<Color>()
        {
            new Color(255, 251, 0), new Color(255, 0, 0), new Color(148, 24, 24), new Color(135, 178, 224),
            new Color(255, 175, 46), new Color(255, 242, 140), new Color(204, 204, 204)
        };

        //10
        private static List<Color> _10 => _thermalcomfort.Skip(2).ToList();

        //11
        private static List<Color> _11 => _thermalcomfort.Take(3).ToList();

        //12
        private static List<Color> _12 => _benefitharm;

        //13
        private static List<Color> _13 => _benefitharm.Skip(1).ToList();
        //14
        private static List<Color> _14 => _benefitharm.Take(2).ToList();
        //15
        private static List<Color> _15 => _shadebenefitharm.Take(2).ToList();
        //16
        private static List<Color> _16 => _shadebenefitharm.Skip(5).ToList();

        //17
        private static List<Color> _17 => _shadebenefitharm.Take(6).ToList();

        //18
        private static List<Color> _18 {
            get {
                var c = _multicolored.ToList();
                c.Reverse();
                return c;
            }
        }

        //19
        private static List<Color> _19
        {
            get
            {
                var c = _18.ToList();
                c.Add(new Color(128, 102, 64));
                return c;
            }
        }

        //20
        private static List<Color> _20 => new List<Color>()
        {
            new Color(0, 0, 0), new Color(137, 0, 139), new Color(218, 0, 218), new Color(196, 0, 255), new Color(0, 92, 255),
            new Color(0, 198, 252), new Color(0, 244, 215), new Color(0, 220, 101), new Color(7, 193, 0), new Color(115, 220, 0),
            new Color(249, 251, 0), new Color(254, 178, 0), new Color(253, 77, 0), new Color(255, 15, 15),
            new Color(255, 135, 135), new Color(255, 255, 255)
        };


        //21
        private static List<Color> _21 => new List<Color>()
        {
            new Color(0, 251, 255), new Color(255, 255, 255), new Color(217, 217, 217), new Color(83, 114, 115)
        };

        //22
        private static List<Color> _22 => new List<Color>()
        {
            new Color(0, 0, 0), new Color(255, 255, 255)
        };

        //23
        private static List<Color> _23 => new List<Color>()
        {
            new Color(0, 0, 255), new Color(0, 255, 100), new Color(255, 0, 0)
        };

        //24
        private static List<Color> _24 => new List<Color>()
        {
            new Color(0, 16, 120), new Color(38, 70, 160), new Color(5, 180, 222), new Color(16, 180, 109),
            new Color(59, 183, 35), new Color(143, 209, 19), new Color(228, 215, 29), new Color(246, 147, 17),
            new Color(243, 74, 0), new Color(255, 0, 0)
        };

        //25
        private static List<Color> _25 => new List<Color>()
        {
            new Color(69, 92, 166), new Color(66, 128, 167), new Color(62, 176, 168), new Color(78, 181, 137),
            new Color(120, 188, 59), new Color(139, 184, 46), new Color(197, 157, 54), new Color(220, 144, 57),
            new Color(228, 100, 59), new Color(233, 68, 60)
        };

        //26
        private static List<Color> _26 => new List<Color>()
        {
            new Color(230, 180, 60), new Color(230, 215, 150), new Color(165, 82, 0),
            new Color(128, 20, 20), new Color(255, 128, 128), new Color(64, 128, 128),
            new Color(128, 128, 128), new Color(255, 128, 128), new Color(128, 64, 0),
            new Color(64, 180, 255), new Color(160, 150, 100), new Color(120, 75, 190), new Color(255, 255, 200),
            new Color(0, 128, 0)
        };


        #region base colors

        private static List<Color> _multicolored => new List<Color>()
        {
            new Color(4, 25, 145), new Color(7, 48, 224), new Color(7, 88, 255), new Color(1, 232, 255),
            new Color(97, 246, 156), new Color(166, 249, 86), new Color(254, 244, 1), new Color(255, 121, 0),
            new Color(239, 39, 0), new Color(138, 17, 0)
        };

        private static List<Color> _thermalcomfort => new List<Color>()
        {
            new Color(0, 136, 255), new Color(200, 225, 255), new Color(255, 255, 255),
            new Color(255, 230, 230), new Color(255, 0, 0)
        };

        private static List<Color> _benefitharm => new List<Color>()
        {
            new Color(0, 191, 48), new Color(255, 238, 184), new Color(255, 0, 0)
        };

        private static List<Color> _shadebenefitharm => new List<Color>()
        {
            new Color(5, 48, 97), new Color(33, 102, 172), new Color(67, 147, 195), new Color(146, 197, 222),

            new Color(209, 229, 240), new Color(255, 255, 255), new Color(253, 219, 199),

            new Color(244, 165, 130), new Color(214, 96, 77), new Color(178, 24, 43), new Color(103, 0, 31)
        };

        #endregion


    }
}

