using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class IntText : ValidableText
    {
        public IntText()
        {
        }

        public override bool IsTextValid(string text) => int.TryParse(this.Text, out var value);

    }


    public class DoubleText : ValidableText
    {
        public DoubleText()
        {
        }

        public override bool IsTextValid(string text) => double.TryParse(this.Text, out var value);
    }


    public abstract class ValidableText : TextBox
    {
        public abstract bool IsTextValid(string text);
        public string ReservedText { get; set; }
        private string _defaultText { get; set; }

        public ValidableText()
        {
            this._defaultBackground = this.BackgroundColor;
            this._defaultTextColor = this.TextColor;
        }
        public void SetDefault(object value)
        {
            this._defaultText = value?.ToString();
           
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.TextColor = _defaultText == this.Text ? _gry : _defaultTextColor;
        }
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Text == ReservedText)
                return;

            this.TextColor = _defaultText == this.Text ? _gry : _defaultTextColor;
            this.BackgroundColor = IsTextValid(this.Text) ? _defaultBackground : _red;

        }

        private Eto.Drawing.Color _defaultBackground;
        private Eto.Drawing.Color _defaultTextColor;
        private Eto.Drawing.Color _blk = Eto.Drawing.Color.FromArgb(0, 0, 0);
        private Eto.Drawing.Color _gry = Eto.Drawing.Color.FromArgb(150, 150, 150);
        private Eto.Drawing.Color _red = Eto.Drawing.Color.FromArgb(255, 100, 100);

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.TextColor = this.Enabled ? _blk : _gry;
        }
    }

    public class ValidateButton: Button
    {
        public string ReservedText { get; set; }
        private string _defaultText { get; set; }
        public void SetDefault(object value)
        {
            this._defaultText = value?.ToString();
        }
        public ValidateButton()
        {
            this._defaultBackground = this.BackgroundColor;
            this._defaultTextColor = this.TextColor;
        }
        public bool IsTextValid(string text) => !string.IsNullOrEmpty(text);
        // this doesn't work for the button
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Text == ReservedText)
                return;

            this.TextColor = _defaultText == this.Text ? _gry : _defaultTextColor;
            this.BackgroundColor = IsTextValid(this.Text) ? _defaultBackground : _red;
        }

        private Eto.Drawing.Color _defaultBackground;
        private Eto.Drawing.Color _defaultTextColor;
        private Eto.Drawing.Color _blk = Eto.Drawing.Color.FromArgb(0, 0, 0);
        private Eto.Drawing.Color _gry = Eto.Drawing.Color.FromArgb(150, 150, 150);
        private Eto.Drawing.Color _red = Eto.Drawing.Color.FromArgb(255, 100, 100);

    }

}
