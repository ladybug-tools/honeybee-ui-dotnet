using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class Dialog_SurfaceBoundaryCondition : Dialog<List<string>>
    {
        
        public Dialog_SurfaceBoundaryCondition(List<string> BCs)
        {
            try
            {
               
                Padding = new Padding(5);
                Resizable = true;
                Title = $"Edit Surface Boundary Condition - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 300);

                DefaultButton = new Button { Text = "OK" };
               

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };

                var textArea = new TextArea();
                textArea.Height = 300;
                textArea.Text = string.Join(Environment.NewLine, BCs);

                
                DefaultButton.Click += (sender, e) => Close(textArea.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList());

                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    textArea,
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
        
    }
}
