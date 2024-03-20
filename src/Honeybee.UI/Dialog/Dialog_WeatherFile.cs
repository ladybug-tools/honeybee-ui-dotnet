// Ignore Spelling: Epw

using Eto.Drawing;
using Eto.Forms;

namespace Honeybee.UI
{
    internal class Dialog_WeatherFile : Dialog<string>
    {
        public Dialog_WeatherFile(string url)
        {

            var _hbobj = url;


            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(3, 3);
            layout.DefaultPadding = new Padding(5);

            Resizable = false;
            Title = $"Weather Url/File - {DialogHelper.PluginName}";
            Width = 400;
            this.Icon = DialogHelper.HoneybeeIcon;

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e)
                => Close(_hbobj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            // Building type
            var effStdItems = new TextBox();
            effStdItems.Text = _hbobj;
            effStdItems.TextChanged += (s, e) => _hbobj = effStdItems.Text;

            var docLink = new LinkButton();
            docLink.Text = "EPW Map...";
            docLink.ToolTip = @"https://www.ladybug.tools/epwmap";
            docLink.Click += (s, e) =>
            {
                var epw = "https://www.ladybug.tools/epwmap";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(epw) { UseShellExecute = true });
            };

            layout.AddRow("Weather Url/File Path:");
            layout.AddRow(effStdItems);
            layout.AddRow(docLink);
            layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
            layout.AddRow(null);

            Content = layout;


        }
    }
}
