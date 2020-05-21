using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class Dialog_ModelResources : Dialog
    {
        //public EventHandler<EventArgs> ModelPropertyButtonClicked;
        //public EventHandler<EventArgs> RoomsButtonClicked;
        //public EventHandler<EventArgs> RunSimulationButtonClicked;
        public Dialog_ModelResources(HB.Model honeybeeObj)
        {
            try
            {
                //var dup = HB.Face.FromJson(honeybeeObj.ToJson());


                Padding = new Padding(5);
                Resizable = true;
                Title = "Model Resources - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 650);
                this.Icon = DialogHelper.HoneybeeIcon;

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close();

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                //var layout = new DynamicLayout();
                ////layout.DefaultPadding = new Padding(10);
                ////layout.DefaultSpacing = new Size(5,5);


                //Create layout
                var panel = new Panel_ModelResources(honeybeeObj);
                //panel.ModelPropertyBtn.Click += (s, e) => ModelPropertyButtonClicked.Invoke(s, e);
                //panel.RoomsBtn.Click += (s, e) => RoomsButtonClicked.Invoke(s, e);
                //panel.RunSimulationBtn.Click += (s, e) => RunSimulationButtonClicked.Invoke(s, e);

                panel.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
                panel.AddSeparateRow(null);


                //Create layout
                Content = panel;



            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }

     
        
     

    }
}
