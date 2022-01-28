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
    }

}
