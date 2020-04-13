using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;

namespace Honeybee.UI
{
    public class LibraryDialog_Constructions : Dialog<HB.OpaqueConstructionAbridged>
    {
     
        public LibraryDialog_Constructions()
        {
            try
            {
               

                Padding = new Padding(5);
                Resizable = true;
                Title = "Construction Library - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);

                var constrs = EnergyLibrary.StandardsOpaqueConstructions;
                var constrLBox = new ListBox();
                constrLBox.Height = 100;
                HB.OpaqueConstructionAbridged selectedConstr = null;
                foreach (var item in constrs)
                {
                    constrLBox.Items.Add(new ListItem() { Text = item.Identifier, Tag = item });
                }
                constrLBox.SelectedKeyChanged += (s, e) => selectedConstr = (constrLBox.Items[constrLBox.SelectedIndex] as ListItem).Tag as HB.OpaqueConstructionAbridged;

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(selectedConstr);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };


                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    new Label() { Text = "Opaque Constructions:" }, constrLBox,
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
