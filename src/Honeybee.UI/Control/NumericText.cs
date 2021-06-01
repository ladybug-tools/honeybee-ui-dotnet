using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class NumericText : TextBox
    {
        public string ReservedText { get; set; }
        public NumericText()
        {
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Text == ReservedText)
                return;
            if (int.TryParse(this.Text, out var value))
                this.TextColor = Eto.Drawing.Color.FromArgb(0, 0, 0);
            else
                this.TextColor = Eto.Drawing.Color.FromArgb(255, 0, 0);

        }

    }



}
