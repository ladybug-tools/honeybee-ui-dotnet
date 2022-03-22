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
                //Resizable = true;
                Title = $"Edit Surface Boundary Condition - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                //MinimumSize = new Size(450, 300);
                Width = 600;

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

                var note = "A list of up to 3 object identifiers that are adjacent to this one. "+
                    "The first object is always the one that is immediately adjacent and is of " +
                    "the same object type (Face, Aperture, Door). When this boundary condition " +
                    "is applied to a Face, the second object in the tuple will be the parent " +
                    "Room of the adjacent object. When the boundary condition is applied to a " +
                    "sub-face (Door or Aperture), the second object will be the parent Face " +
                    "of the adjacent sub-face and the third object will be the parent Room " +
                    "of the adjacent sub-face.\n\nIn most cases you should not be editing these values manually and should use the PO_SolveAdjacency command instead!";
                var label = new Label();
                label.Text = note;
                label.Wrap = WrapMode.Word;


                DefaultButton.Click += (sender, e) => 
                {
                    var text = textArea.Text;
                    var items = text
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_=>_.Trim())
                    .Where(_=> !string.IsNullOrEmpty(_))
                    .ToList();

                    if (items.Count>3 || items.Count<2)
                    {
                        MessageBox.Show(this, "A valid surface boundary condition must be a list that consists of 2 or 3 identifiers");
                        return;
                    }
                    Close(items);
                };

                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                    { 
                        label,
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
