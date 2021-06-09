using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    [Obsolete("This is deprecated", true)]
    public class Dialog_FaceEnergyProperty: Dialog<FaceEnergyPropertiesAbridged>
    {
        private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_FaceEnergyProperty(ModelEnergyProperties libSource, FaceEnergyPropertiesAbridged faceEnergyProperties)
        {
            try
            {
                this.ModelEnergyProperties = libSource;
                var EnergyProp = faceEnergyProperties ?? new FaceEnergyPropertiesAbridged();

                Padding = new Padding(5);
                Resizable = true;
                Title = $"Face Energy Properties - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);
                this.Icon = DialogHelper.HoneybeeIcon;

                //Get constructions
                var cons = this.ModelEnergyProperties.Constructions.OfType<OpaqueConstructionAbridged>();
                var constructionSetDP = DialogHelper.MakeDropDown(EnergyProp.Construction, (v) => EnergyProp.Construction = v?.Identifier,
                    cons, "By Room ConstructionSet---------------------");


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
