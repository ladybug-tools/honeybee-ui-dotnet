using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public abstract class ValidableText : TextBox
    {
        public abstract bool IsTextValid(string text);
        public string ReservedText { get; set; }
        private string _defaultText { get; set; }

        public ValidableText()
        {
            this._defaultTextColor = this.TextColor;
        }
        public virtual void SetDefault(object value)
        {
            this._defaultText = value?.ToString();
        }
        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            this.TextColor = GetTextColor();
            this._defaultBackground = this.BackgroundColor;
        }
        //protected override void OnShown(EventArgs e)
        //{
        //    base.OnShown(e);
        //}
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.Text == ReservedText)
                return;

            if (!this.Loaded)
                return;

            this.TextColor = GetTextColor();
            this.BackgroundColor = IsTextValid(this.Text) ? _defaultBackground : _red;

        }

        private Eto.Drawing.Color _defaultBackground;
        private Eto.Drawing.Color _defaultTextColor;
        private Eto.Drawing.Color _blk = Eto.Drawing.Color.FromArgb(0, 0, 0);
        private Eto.Drawing.Color _disabledGry = Eto.Drawing.Color.FromArgb(200, 200, 200);
        private Eto.Drawing.Color _gry = Eto.Drawing.Color.FromArgb(150, 150, 150);
        private Eto.Drawing.Color _red = Eto.Drawing.Color.FromArgb(255, 100, 100);
        private Eto.Drawing.Color _editedColor = Eto.Drawing.Color.FromArgb(0, 100, 255);
        
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.TextColor = GetTextColor();
        }

        private Eto.Drawing.Color GetTextColor()
        {
            var defalutColor = _defaultText == this.Text ?  _defaultTextColor: _editedColor;
            return this.Enabled ? defalutColor : _disabledGry;
        }
    }

}
