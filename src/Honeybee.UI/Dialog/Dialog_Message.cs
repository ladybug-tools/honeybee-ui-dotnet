using Eto.Drawing;
using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class Dialog_Message : Dialog
    {
        private Dialog_Message _instance;

        private Dialog_Message(string message, string title = default)
        {
            try
            {
               
                Padding = new Padding(5);
                Resizable = true;
                Title = title ?? DialogHelper.PluginName;
                this.Icon = DialogHelper.HoneybeeIcon;
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 400);
                Width = 500;

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };

                var panel = PanelHelper.UpdateMessagePanel(message);
                
                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    panel,
                    new TableRow(buttons),
                    null
                }
                };
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
                //Rhino.RhinoApp.WriteLine(e.Message);
            }
            
        }
        public static void Show(Control owner, string message, string title)
        {
            var dia = new Dialog_Message(message, title);
            dia.ShowModal(owner);
        }

        public static void Show(Control owner, string message)
        {
            var dia = new Dialog_Message(message);
            dia.ShowModal(owner);
        }

        public static void Show(string message)
        {
            var dia = new Dialog_Message(message);
            dia.ShowModal();
        }




    }
}
