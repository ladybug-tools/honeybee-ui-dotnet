using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Honeybee.UI
{
    [System.Obsolete("Use the new LadybugDisplaySchema.LegendParameters instead", true)]
    [DataContract]
    public class LegendParameter
    {
      
        /// <summary>
        /// X position of the legend
        /// </summary>
        [DataMember(Name = "x")]
        public int X { get; set; }

        /// <summary>
        /// Y position of the legend
        /// </summary>
        [DataMember(Name = "y")]
        public int Y { get; set; }

        /// <summary>
        /// Width of the legend
        /// </summary>
        [DataMember(Name = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Height of the legend
        /// </summary>
        [DataMember(Name = "height")]
        public int Height { get; set; }


        /// <summary>
        /// Colors
        /// </summary>
        [DataMember(Name = "colors")]
        public List<LadybugDisplaySchema.Color> Colors { get; set; }

        public List<LadybugDisplaySchema.Color> ColorsReverse { get; }
        /// <summary>s
        /// Font size of the legend labels
        /// </summary>
        [DataMember(Name = "fontHeight")]
        public int FontHeight { get; set; }

        [DataMember(Name = "fontColor")]
        public HoneybeeSchema.Color FontColor { get; set; }
        /// <summary>
        /// Min label
        /// </summary>
        [DataMember(Name = "min")]
        public double Min { get; set; }

        /// <summary>
        /// Max label
        /// </summary>
        [DataMember(Name = "max")]
        public double Max { get; set; }

        /// <summary>
        /// Number of segments
        /// </summary>
        [DataMember(Name = "numSegment")]
        public int NumSegment { get; set; }

        [DataMember(Name = "decimalPlaces")]
        public int DecimalPlaces { get; set; }

        [DataMember(Name = "continuous")]
        public bool Continuous { get; set; }
        [DataMember(Name = "horizontal")]
        public bool Horizontal { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "unit")]
        public string Unit { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; } = nameof(LegendParameter);

        public bool StringValues { get; set; }
        public LegendParameter()
        {
            initDefault();
            Colors = new List<LadybugDisplaySchema.Color>();
        }

        public LegendParameter(int x = 50, int y = 100)
        {
            initDefault();
            X = x;
            Y = y;

            Colors = LegendColorSet.Presets.First().Value.ToList();
        }
        public LegendParameter(double min, double max, int numSegs, List<LadybugDisplaySchema.Color> colors = default)
        {
            initDefault();
            Min = min;
            Max = max;
            NumSegment = numSegs;

            var c = colors ?? _defaultColorSet.ToList();
            Colors = numSegs >1 ? c : new List<LadybugDisplaySchema.Color>() { c[0], c[0] };
        }

        private void initDefault()
        {
            X = 10;
            Y = 100;
            Width = 25;
            Height = 250;

            FontHeight = 12;
            FontColor = new HoneybeeSchema.Color(0, 0, 0);
            Min = 0;
            Max = 100;
            NumSegment = 3;
            DecimalPlaces = 2;

            Colors = _defaultColorSet.ToList();
        }

        private List<LadybugDisplaySchema.Color> _defaultColorSet = LegendColorSet.Presets.First().Value.ToList();

        public System.Drawing.Rectangle GetBoundary => new System.Drawing.Rectangle(this.X, this.Y, this.Width, this.Height);
  

        /// <summary>
        /// It converts a JSON string to a LegendScreen object.
        /// </summary>
        /// <param name="json">JSON string.</param>
        /// <example>For example:
        /// <code>
        /// var json = @"{
        ///     'type': 'LegendScreen',
        ///     'x': 50,
        ///     'y': 50,
        ///     'height': 600,
        ///     'width': 25,
        ///     'min': 0,
        ///     'max': 100,
        ///     'num': 8,
        ///     'font': 17,
        ///     'colors': [{'r': 255, 'g': 0, 'b': 0 },
        ///         {'r': 0, 'g': 255, 'b': 255 }, 
        ///         { 'r': 12, 'g': 123, 'b': 255 }, 
        ///         { 'r': 0, 'g': 255, 'b': 0 }]
        ///     }";
        /// var legend = LegendScreen.FromJson(json);
        /// </code>
        /// where <c>legend</c> is a new LegendParameter object.
        /// </example>
        /// <returns>LegendParameter object.</returns>
        public static LegendParameter FromJson(string json)
        {
            var legend = JsonConvert.DeserializeObject<LegendParameter>(json);
            return legend;
        }

        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(this);
            return json;
        }

        public LegendParameter Duplicate()
        {
            var json = this.ToJson();
            return FromJson(json);
        }


    }
}
