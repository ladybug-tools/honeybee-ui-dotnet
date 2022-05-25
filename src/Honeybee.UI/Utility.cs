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
            var plane = lbg.Plane;
            var projected = lbg.BoundaryPoints.Select(_ => plane.XYZtoXY(_)).ToArray();
            var b = new Ladybug.Geometry.Polygon2D(projected);
            return b.Area;
        }

        public static Ladybug.Geometry.Face3D ToLBG(this HoneybeeSchema.Face3D face)
        {
            return new Ladybug.Geometry.Face3D(face.Boundary, face.Plane?.ToLBG(), face.Holes);
        }

        public static Ladybug.Geometry.Plane ToLBG(this HoneybeeSchema.Plane geo)
        {
            return new Ladybug.Geometry.Plane(geo.N, geo.O, geo.X);
        }

    }

}
