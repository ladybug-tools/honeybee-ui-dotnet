using System;
using System.Collections.Generic;

namespace Honeybee.UI
{
    class StringComparer : IComparer<string>
    {
        bool _isNumber = false;
        public StringComparer(bool isNumber)
        {
            _isNumber = isNumber;

        }

        public int Compare(string x, string y)
        {
            if (!_isNumber) return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);

            var n = _isNumber;
            n &= double.TryParse(x, out double nx);
            n &= double.TryParse(y, out double ny);
            var result = n ? nx.CompareTo(ny) : string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
            return result;
        }
    }
}
