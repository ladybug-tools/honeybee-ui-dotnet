using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Honeybee.UI
{
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
        public List<HoneybeeSchema.Color> Colors { get; set; }

        public List<HoneybeeSchema.Color> ColorsReverse { get; }
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

        [DataMember(Name = "type")]
        public string Type { get; } = nameof(LegendParameter);

        public bool StringValues { get; set; }
        public LegendParameter()
        {
            initDefault();
            Colors = new List<HoneybeeSchema.Color>();
        }

        public LegendParameter(int x = 50, int y = 100)
        {
            initDefault();
            X = x;
            Y = y;

            Colors = LegendColorSet.Presets.First().Value.ToList();
        }
        public LegendParameter(double min, double max, int numSegs, List<HoneybeeSchema.Color> colors = default)
        {
            initDefault();
            Min = min;
            Max = max;
            NumSegment = numSegs;

            var c = colors ?? _defaultColorSet.ToList();
            Colors = numSegs >1 ? c : new List<HoneybeeSchema.Color>() { c[0], c[0] };
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
            DecimalPlaces = 1;

            Colors = _defaultColorSet.ToList();
        }

        private List<HoneybeeSchema.Color> _defaultColorSet = LegendColorSet.Presets.First().Value.ToList();

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

        private List<double> _colorDomains;
        private List<double> ColorDomains()
        {
            if (_colorDomains != null && _colorDomains.Count == this.Colors.Count)
                return _colorDomains;

            if (this.Colors.Count < 2)
                throw new System.ArgumentException("Need at least 2 colors");

            var cs = this.Colors;
            double factor = 1.0 / (cs.Count - 1);
            var bounds = cs.Select((_, i) => i * factor).ToList();
            _colorDomains = bounds;
            return bounds;
        }
        /// <summary>
        /// Blend between two colors based on input value.
        /// </summary>
        /// <param name="value"></param>
        public HoneybeeSchema.Color CalColor(double value, ref Dictionary<double, HoneybeeSchema.Color> cache)
        {
            if (cache.TryGetValue(value, out var c))
                return c;
            var newColor = CalColor(value);
            cache.Add(value, newColor);
            return newColor;
        }

        public HoneybeeSchema.Color CalColor(double value)
        {
         
            var colors = this.Colors.ToList();

            var colorStart = colors.First();
            var colorEnd = colors.Last();
            if (value <= this.Min)
                return colorStart;
            if (value >= this.Max)
                return colorEnd;

            var range_p = this.Max - this.Min;
            var factor = range_p == 0 ? 0 : (value - this.Min) / range_p;

            var colorDomains = ColorDomains();
            var segFactor = colorDomains[1];
            var colorFactor = 0.0;
            for (int i = 1; i < colorDomains.Count; i++)
            {
                var cFactorBefore = colorDomains[i - 1];
                var cFactor = colorDomains[i];
                if (factor < cFactor && factor >= cFactorBefore)
                {
                    colorStart = colors[i - 1];
                    colorEnd = colors[i];
                    colorFactor = (factor - cFactorBefore) / segFactor;
                }
                else
                    continue;
            }

            var newColor = BlendColors(colorFactor, colorStart, colorEnd);
        
            return newColor;
        }

        private HoneybeeSchema.Color BlendColors(double factor, HoneybeeSchema.Color c1, HoneybeeSchema.Color c2)
        {
            var red = (int)(factor * (c2.R - c1.R) + c1.R);
            var green = (int)(factor * (c2.G - c1.G) + c1.G);
            var blue = (int)(factor * (c2.B - c1.B) + c1.B);

            return new HoneybeeSchema.Color(red, green, blue);
        }

       

    }
}
