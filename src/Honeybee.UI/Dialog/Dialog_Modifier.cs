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
            Title = $"Modifier - {DialogHelper.PluginName}";
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

            var isValid = new Label();
            isValid.Height = 30;
            layout.AddRow(isValid);

            var docLink = new LinkButton();
            docLink.Text = "View help documents...";
            docLink.ToolTip = @"https://www.ladybug.tools/honeybee-schema/model.html";
            docLink.Click += (s, e) =>
            {
                var url = $"https://www.ladybug.tools/honeybee-schema/model.html#tag/{typeof(T).Name.ToLower()}_model";
                System.Diagnostics.Process.Start(url);
            };
            layout.AddRow(docLink);

            textArea.TextChanged += (s, e) =>
            {
                var changedText = textArea.Text.Trim();

                object newObj = null;
                try
                {
                    newObj = ValidateJsonText<T>(changedText);
                }
                catch (System.Reflection.TargetInvocationException err)
                {
                    var error = err.InnerException;
                    isValid.Text = error.Message;
                    OkButton.Enabled = false;
                    return;
                }

                if (newObj == null)
                {
                    isValid.Text = "Invalid input text";
                    OkButton.Enabled = false;
                    return;
                }
                OkButton.Enabled = true;
                isValid.Text = $"Valid {typeof(T).Name} object";
                _hbObj = newObj as T;
            };

            layout.AddRow(null);
            layout.AddSeparateRow(null, OkButton, AbortButton, null);
            layout.AddRow(null);
            Content = layout;

        }

        private object ValidateJsonText<T>(string userInput)
        {
            var changedText = userInput;
            var hbType = typeof(T);
            var fromJsonMethod = hbType.GetMethod("FromJson");

            //object newObj = null;
            var newObj = fromJsonMethod.Invoke(null, new[] { changedText });
          
            return newObj;

        }

    }
}
