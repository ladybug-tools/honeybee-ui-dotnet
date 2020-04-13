using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public static partial class PanelHelper
    {
        /// <summary>
        /// Create Eto panel based on Honeybee geomerty. 
        /// If input HoneybeeObj is a duplicated object, geometryReset action must be provided, 
        /// otherwise no changes would be applied to original honeybee object.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel GenShadePanel(Shade HoneybeeObj, System.Action<string> geometryReset = default)
        {

            var shd = HoneybeeObj;
            var objID = shd.Identifier;
            geometryReset = geometryReset ?? delegate (string m) { }; //Do nothing if geometryReset is null

            var layout = new DynamicLayout { };
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {shd.Identifier}" });

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTBox = new TextBox() { };
            shd.DisplayName = shd.DisplayName ?? string.Empty;
            nameTBox.TextBinding.Bind(shd, m => m.DisplayName);
            nameTBox.LostFocus += (s, e) => { geometryReset($"Set Door Name: {shd.DisplayName}"); };
            layout.AddSeparateRow(nameTBox);

            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = shd.Properties.Energy ?? new ShadeEnergyPropertiesAbridged();
                energyProp = ShadeEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_ShadeEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    shd.Properties.Energy = dialog_rc;
                    geometryReset($"Set Door Energy Properties");
                }
            };
            layout.AddSeparateRow(faceEngPropBtn);



            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(shd.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;
            //this.Content = layout;

            //ShadeEnergyPropertiesAbridged PropBtn_Click(ShadeEnergyPropertiesAbridged EnergyProp)
            //{
            //    var energyProp = EnergyProp ?? new ShadeEnergyPropertiesAbridged();
            //    energyProp = ShadeEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
            //    var dialog = new Dialog_ShadeEnergyProperty(energyProp);
            //    var dialog_rc = dialog.ShowModal();
            //    return dialog_rc;

            //}

            
        }


    }
}
