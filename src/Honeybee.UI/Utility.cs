using System;
using System.Linq;

namespace Honeybee.UI
{
    public static class Utility
    {
        public static bool TryParse(string text, out double value)
        {
            value = -999;
            text = text?.Trim();
            if (string.IsNullOrEmpty(text))
                return false;

            text = text.StartsWith(".") ? $"0{text}" : text;
            return double.TryParse(text, out value);
        }

        public static double ToNumber(this string text)
        {
            TryParse(text, out var value);
            return value;
        }

        public static double CalArea(this HoneybeeSchema.Room room)
        {
            var areas = room.Faces.Where(_ => _.FaceType == HoneybeeSchema.FaceType.Floor).Select(_ => _.CalArea());
            return areas.Sum();
        }

        public static double CalArea(this HoneybeeSchema.Face face)
        {
            return face.Geometry.CalArea();
        }

        public static double CalArea(this HoneybeeSchema.Face3D face)
        {
            var lbg = face.ToLBG();
            return lbg.Area;
        }

        public static LadybugDisplaySchema.Face3D ToLBG(this HoneybeeSchema.Face3D face)
        {
            return new LadybugDisplaySchema.Face3D(face.Boundary, face.Holes, face.Plane?.ToLBG());
        }

        public static LadybugDisplaySchema.Plane ToLBG(this HoneybeeSchema.Plane geo)
        {
            return new LadybugDisplaySchema.Plane(geo.N, geo.O, geo.X);
        }

        public static string NiceDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return description;

            var charCount = 0;
            var maxLineLength = 50;
            var lines = description
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => (charCount += w.Length + 1) / maxLineLength)
                .Select(g => string.Join(" ", g));
            return string.Join(Environment.NewLine, lines);
        }

    }

}
