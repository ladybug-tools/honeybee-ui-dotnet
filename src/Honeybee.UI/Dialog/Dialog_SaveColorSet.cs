using Eto.Forms;

namespace Honeybee.UI
{
    public class Dialog_SaveColorSet : Eto.Forms.Dialog
    {
        public Dialog_SaveColorSet(System.Collections.Generic.List<LadybugDisplaySchema.Color> colors)
        {
            this.Title = "Save as a preset";
            this.Width = 300;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Eto.Drawing.Size(5, 5);
            layout.DefaultPadding = new Eto.Drawing.Padding(5);

            var name = new TextBox() { PlaceholderText = "Input a name for this preset" };
            layout.AddSeparateRow("Name:", name);

            var OkBtn = new Eto.Forms.Button() { Text = "OK" };
            OkBtn.Click += (s, e) => {
                var n = name.Text;
                if (LegendColorSet.SaveUserColorSet(n, colors))
                    this.Close();
            };
            this.AbortButton = new Eto.Forms.Button() { Text = "Cancel" };
            this.AbortButton.Click += (s, e) => { this.Close(); };

            layout.AddSeparateRow(null, OkBtn, this.AbortButton, null);
            layout.AddSeparateRow(null);
            this.Content = layout;
        }
    }

}
