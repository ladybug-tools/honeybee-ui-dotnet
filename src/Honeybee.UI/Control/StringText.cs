namespace Honeybee.UI
{
    public class StringText : ValidableText
    {
        public StringText()
        {
        }

        public override bool IsTextValid(string text) => true;
    }

}
