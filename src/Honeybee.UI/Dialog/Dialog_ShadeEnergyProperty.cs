using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;

namespace Honeybee.UI
{
    public class Dialog_ShadeEnergyProperty: Dialog<HB.ShadeEnergyPropertiesAbridged>
    {
     
        public Dialog_ShadeEnergyProperty(HB.ShadeEnergyPropertiesAbridged energyProp)
        {
            try
            {
                var EnergyProp = energyProp ?? new HB.ShadeEnergyPropertiesAbridged();

                Padding = new Padding(5);
                Resizable = true;
                Title = "Shade Energy Properties - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);
                this.Icon = DialogHelper.HoneybeeIcon;

                //Get constructions
                var constructionSetDP = DialogHelper.MakeDropDown(EnergyProp.Construction, (v) => EnergyProp.Construction = v?.Identifier,
                    EnergyLibrary.StandardsWindowConstructions.Values, "By Global Construction Set ---------------------");

                //Placeholder 
                var bcTBox = new TextBox() { };
                bcTBox.Enabled = false;
                EnergyProp.TransmittanceSchedule = EnergyProp.TransmittanceSchedule ?? string.Empty;
                bcTBox.TextBinding.Bind(EnergyProp, m => m.TransmittanceSchedule);
         

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
                    new Label() { Text = "Transmittance Schedule:" }, bcTBox,
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
