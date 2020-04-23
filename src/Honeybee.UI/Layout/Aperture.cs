using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HoneybeeSchema;
using System.Collections.Generic;


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
        public static Panel GenAperturePanel(Aperture HoneybeeObj, System.Action<string> geometryReset = default)
        {
            var apt = HoneybeeObj;
            var objID = apt.Identifier;
            geometryReset = geometryReset ?? delegate (string m) { }; //Do nothing if geometryReset is null

            var layout = new DynamicLayout { };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {apt.Identifier}" });


            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTBox = new TextBox() { };
            apt.DisplayName = apt.DisplayName ?? string.Empty;
            nameTBox.TextBinding.Bind(apt, m => m.DisplayName);
            nameTBox.LostFocus += (s, e) => { geometryReset($"Set Aperture Name: {apt.DisplayName}"); };
            layout.AddSeparateRow(nameTBox);


            layout.AddSeparateRow(new Label { Text = "Operable:" });
            var operableCBox = new CheckBox();
            operableCBox.CheckedBinding.Bind(apt, v => v.IsOperable);
            operableCBox.CheckedChanged += (s, e) => { geometryReset($"Set Aperture Operable: {apt.IsOperable}"); };
            layout.AddSeparateRow(operableCBox);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = apt.Properties.Energy ?? new ApertureEnergyPropertiesAbridged();
                energyProp = ApertureEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_ApertureEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    apt.Properties.Energy = dialog_rc;
                    geometryReset($"Set Aperture Energy Properties");
                }
                    
            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.Enabled = apt.BoundaryCondition.Obj is Outdoors;
            bcBtn.Click += (s, e) =>
            {
                if (apt.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        apt.BoundaryCondition = dialog_rc;
                        geometryReset($"Set Boundary Condition Properties");
                    }
                    
                }
            };

            var bcs = new List<AnyOf<Outdoors, Surface>>() { new Outdoors(), new Surface(new List<string>()) };
            var bcDP = DialogHelper.MakeDropDownForAnyOf(apt.BoundaryCondition.Obj.GetType().Name, v => apt.BoundaryCondition = v, bcs);
            bcDP.SelectedIndexChanged += (s, e) =>
            {
                bcBtn.Enabled = false;
                if (bcDP.SelectedKey == nameof(Outdoors))
                    bcBtn.Enabled = true;
            };

            layout.AddSeparateRow(bcDP);
            layout.AddSeparateRow(bcBtn);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.Height = 50;
            var inShds = apt.IndoorShades;
            if (inShds != null)
            {
                var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                inShadesListBox.Items.AddRange(idShds);
            }
            layout.AddSeparateRow(inShadesListBox);

            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            var outShds = apt.OutdoorShades;
            if (outShds != null)
            {
                var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                outShadesListBox.Items.AddRange(outShdItems);
            }
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(apt.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }

        private static Panel _aperturePanel;
        public static Panel UpdateAperturePanel(Aperture HoneybeeObj, System.Action<string> geometryReset = default)
        {
            var vm = ApertureViewModel.Instance;
            vm.Update(HoneybeeObj, geometryReset);
            if (_aperturePanel == null)
                _aperturePanel = GenAperturePanel();
            return _aperturePanel;
        }

        private static Panel GenAperturePanel()
        {
            var vm = ApertureViewModel.Instance;

            var layout = new DynamicLayout { DataContext = vm };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((ApertureViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow(new Label { Text = "ID: " }, id);


            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((ApertureViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            //layout.AddSeparateRow(new Label { Text = "Operable:" });
            var operableCBox = new CheckBox();
            operableCBox.CheckedBinding.BindDataContext((ApertureViewModel m) => m.HoneybeeObject.IsOperable);
            operableCBox.CheckedChanged += (s, e) => { vm.ActionWhenChanged($"Set Aperture Operable: {vm.HoneybeeObject.IsOperable}"); };
            layout.AddSeparateRow(new Label { Text = "Operable:" }, operableCBox);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = vm.HoneybeeObject.Properties.Energy ?? new ApertureEnergyPropertiesAbridged();
                energyProp = ApertureEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_ApertureEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    vm.HoneybeeObject.Properties.Energy = dialog_rc;
                    vm.ActionWhenChanged($"Set Aperture Energy Properties");
                }

            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcDP = new DropDown();
            bcDP.BindDataContext(c => c.DataStore, (ApertureViewModel m) => m.Bcs);
            bcDP.ItemTextBinding = Binding.Delegate<AnyOf, string>(m => m.Obj.GetType().Name);
            bcDP.SelectedIndexBinding.BindDataContext((ApertureViewModel m) => m.SelectedIndex);
            layout.AddSeparateRow(bcDP);

            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.BindDataContext(c => c.Enabled, (ApertureViewModel m) => m.IsOutdoor, DualBindingMode.OneWay);
            bcBtn.Click += (s, e) =>
            {
                if (vm.HoneybeeObject.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        vm.HoneybeeObject.BoundaryCondition = dialog_rc;
                        vm.ActionWhenChanged($"Set Aperture Boundary Condition");
                    }
                }
                else
                {
                    MessageBox.Show("Only Outdoors type has additional properties to edit!");
                }
            };
            layout.AddSeparateRow(bcBtn);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.BindDataContext(c => c.DataStore, (ApertureViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            inShadesListBox.Height = 50;
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (ApertureViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(vm.HoneybeeObject.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }


    }

}
