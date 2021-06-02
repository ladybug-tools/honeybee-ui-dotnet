using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class IntText : TextBox
    {
        public string ReservedText { get; set; }
        public IntText()
        {
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Text == ReservedText)
                return;
            if (int.TryParse(this.Text, out var value))
                this.TextColor = _blk;
            else
                this.TextColor = _red;

        }

        private Eto.Drawing.Color _blk = Eto.Drawing.Color.FromArgb(0, 0, 0);
        private Eto.Drawing.Color _gry = Eto.Drawing.Color.FromArgb(150, 150, 150);
        private Eto.Drawing.Color _red = Eto.Drawing.Color.FromArgb(255, 0, 0);

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.TextColor = this.Enabled ? _blk : _gry;
        }

    }
    public class DoubleText : TextBox
    {
        public string ReservedText { get; set; }
        public DoubleText()
        {
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Text == ReservedText)
                return;
            if (double.TryParse(this.Text, out var value))
                this.TextColor = _blk;
            else
                this.TextColor = _red;

        }

        private Eto.Drawing.Color _blk = Eto.Drawing.Color.FromArgb(0, 0, 0);
        private Eto.Drawing.Color _gry = Eto.Drawing.Color.FromArgb(150, 150, 150);
        private Eto.Drawing.Color _red = Eto.Drawing.Color.FromArgb(255, 0, 0);

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.TextColor = this.Enabled ? _blk : _gry;
        }

    }


}
