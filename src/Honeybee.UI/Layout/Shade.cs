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
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {shd.Identifier}" });

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTBox = new TextBox() { };
            shd.DisplayName = shd.DisplayName ?? string.Empty;
            nameTBox.TextBinding.Bind(shd, m => m.DisplayName);
            nameTBox.LostFocus += (s, e) => { geometryReset($"Set Shade Name: {shd.DisplayName}"); };
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
                    geometryReset($"Set Shade Energy Properties");
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


        private static Panel _shadePanel;
        public static Panel UpdateShadePanel(Shade HoneybeeObj, System.Action<string> geometryReset = default)
        {
            var vm = ShadeViewModel.Instance;
            vm.Update(HoneybeeObj, geometryReset);
            if (_shadePanel == null)
                _shadePanel = GenShadePanel();
            return _shadePanel;
        }

        private static Panel GenShadePanel()
        {
            var vm = ShadeViewModel.Instance;

            var layout = new DynamicLayout { DataContext = vm };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((ShadeViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow(new Label { Text = "ID: " }, id);


            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((ShadeViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show(Helper.Owner, "Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = vm.HoneybeeObject.Properties.Energy ?? new ShadeEnergyPropertiesAbridged();
                energyProp = ShadeEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_ShadeEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal(Helper.Owner);
                if (dialog_rc != null)
                {
                    vm.HoneybeeObject.Properties.Energy = dialog_rc;
                    vm.ActionWhenChanged($"Set Shade Energy Properties");
                }

            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(Helper.Owner, vm.HoneybeeObject.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }


    }
}
