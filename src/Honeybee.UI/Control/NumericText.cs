﻿namespace Honeybee.UI
{
    public class DoubleText : ValidableText
    {
        public DoubleText()
        {
        }

        public override bool IsTextValid(string text) => double.TryParse(this.Text, out var value);
        public override void SetDefault(object value)
        {
            if (value == null)
                base.SetDefault(0);
            else
                base.SetDefault(value);
        }
    }

}
