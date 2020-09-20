using Eto.Drawing;
using Eto.Forms;
using System.Reflection;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class Dialog_Modifier<T> : Dialog<T> where T : HB.ModifierBase
    {
        public Dialog_Modifier(T modifier)
        {
            var _hbObj = modifier;

            Padding = new Padding(10);
            Resizable = true;
            Title = "Modifier - Honeybee";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;
            
            var OkButton = new Button { Text = "OK" };
            OkButton.Click += (sender, e) => Close(_hbObj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(10, 5);

            var textArea = new RichTextArea();
            textArea.Height = 300;
            textArea.Text = _hbObj.ToJson();
        
            layout.AddRow(textArea);

            var isValid = new Label() { Text = "Valid input text" };
            layout.AddRow(isValid);


            textArea.TextChanged += (s, e) =>
            {
                var changedText = textArea.Text.Trim();
                //var newObj = HB.ModifierBase.FromJson(changedText);
                var newObj = typeof(T).GetMethod("FromJson", BindingFlags.Static).Invoke(null, new[] { changedText }) as T;
                if (newObj == null)
                {
                    isValid.Text = "Invalid input text";
                    OkButton.Enabled = false;
                    return;
                }

                OkButton.Enabled = true;
                _hbObj = newObj;
            };

            layout.AddRow(null);
            layout.AddSeparateRow(null, OkButton, AbortButton, null);

            Content = layout;

        }

    }
}
