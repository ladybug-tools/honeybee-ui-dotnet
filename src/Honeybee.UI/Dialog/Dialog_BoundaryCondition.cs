using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
namespace Honeybee.UI
{
    public class Dialog_BoundaryCondition_Outdoors : Dialog<HB.Outdoors>
    {
     
        public Dialog_BoundaryCondition_Outdoors(HB.Outdoors boundaryCondition)
        {
            try
            {
           
                Padding = new Padding(5);
                Resizable = true;
                Title = "Energy Boundary Condition - Honeybee Rhino PlugIn";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 300);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout() { Padding = new Padding(15) };

                
                var bcLayout = LayoutHelper.CreateOutdoorLayout(boundaryCondition);
                layout.AddRow(bcLayout);

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(boundaryCondition);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };

                layout.AddSeparateRow(buttons);
                layout.AddRow(null);
                //Create layout
                Content = layout;
              
            }
            catch (Exception e)
            {
                throw e;
            }
            
            
        }

   
    }
    //TODO: finish this later
    public class Dialog_BoundaryCondition_Surface : Dialog<HB.Surface>
    {

        public Dialog_BoundaryCondition_Surface(HB.Surface boundaryCondition)
        {
            try
            {

                Padding = new Padding(5);
                Resizable = true;
                Title = "Energy Boundary Condition - Honeybee Rhino PlugIn";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 500);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout() { Padding = new Padding(15) };

                layout.AddRow("Surface:");
                //var bcLayout = new Layout_BoundaryCondition();
                //layout.AddRow(LayoutHelper.CreateOutdoorLayout());

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(boundaryCondition);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };

                layout.AddSeparateRow(buttons);
                layout.AddRow(null);
                //Create layout
                Content = layout;

            }
            catch (Exception e)
            {
                throw e;
            }


        }


    }

}
