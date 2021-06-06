namespace Honeybee.UI
{
    public class IntText : ValidableText
    {
        public IntText()
        {
        }

        public override bool IsTextValid(string text) => int.TryParse(this.Text, out var value);
        public override void SetDefault(object value)
        {
            if (value == null)
                base.SetDefault(0);
            else
                base.SetDefault(value);
        }
    }

}
