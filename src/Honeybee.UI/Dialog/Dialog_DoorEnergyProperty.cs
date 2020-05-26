using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;

namespace Honeybee.UI
{
    public class Dialog_DoorEnergyProperty: Dialog<HB.DoorEnergyPropertiesAbridged>
    {
     
        public Dialog_DoorEnergyProperty(HB.DoorEnergyPropertiesAbridged energyProp)
        {
            try
            {
                var EnergyProp = energyProp ?? new HB.DoorEnergyPropertiesAbridged();

                Padding = new Padding(5);
                Resizable = true;
                Title = "Door Energy Properties - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);
                this.Icon = DialogHelper.HoneybeeIcon;

                //Get constructions
                var constructionSetDP = DialogHelper.MakeDropDown(EnergyProp.Construction, (v) => EnergyProp.Construction = v?.Identifier,
                    EnergyLibrary.StandardsWindowConstructions.Values, "By Room Construction Set ---------------------");


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(EnergyProp);

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
                    new Label() { Text = "Face Construction:" }, constructionSetDP,
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
