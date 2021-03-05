using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class Dialog_Face: Dialog<Face>
    {
        public ModelProperties ModelProperties { get; set; }
        public Dialog_Face(ModelProperties libSource , Face honeybeeObj)
        {
            try
            {
                this.ModelProperties = libSource;
                var dup = Face.FromJson(honeybeeObj.ToJson());


                Padding = new Padding(5);
                Resizable = true;
                Title = $"Door Energy Properties - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 650);
                this.Icon = DialogHelper.HoneybeeIcon;

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(dup);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                
                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };

                //Create layout
                var panel = Honeybee.UI.View.Face.Instance;
                panel.UpdatePanel(libSource, dup);

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

                throw e;
            }
            
            
        }

     
        
     

    }
}
