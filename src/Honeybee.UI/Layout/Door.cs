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
        public static Panel GenDoorPanel(Door HoneybeeObj, System.Action<string> geometryReset = default)
        {

            var door = HoneybeeObj;
            var objID = door.Identifier;
            geometryReset = geometryReset ?? delegate (string m) { }; //Do nothing if geometryReset is null

            var layout = new DynamicLayout { };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {door.Identifier}" });

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTBox = new TextBox() { };
            door.DisplayName = door.DisplayName ?? string.Empty;
            nameTBox.TextBinding.Bind(door, m => m.DisplayName);
            nameTBox.LostFocus += (s, e) => { geometryReset($"Set Door Name: {door.DisplayName}"); };
            layout.AddSeparateRow(nameTBox);


            layout.AddSeparateRow(new Label { Text = "Glass:" });
            var isGlassCBox = new CheckBox();
            isGlassCBox.CheckedBinding.Bind(door, v => v.IsGlass);
            isGlassCBox.CheckedChanged += (s, e) => { geometryReset($"Set Glass Door: {door.IsGlass}"); };
            layout.AddSeparateRow(isGlassCBox);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = door.Properties.Energy ?? new DoorEnergyPropertiesAbridged();
                energyProp = DoorEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_DoorEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    door.Properties.Energy = dialog_rc;
                    geometryReset($"Set Door Energy Properties");
                }
            };
            layout.AddSeparateRow(faceEngPropBtn);

            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.Enabled = door.BoundaryCondition.Obj is Outdoors;
            bcBtn.Click += (s, e) =>
            {
                if (door.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        door.BoundaryCondition = dialog_rc;
                        geometryReset($"Set Door Boundary Condition");
                    }
                        
                }
            };

            var bcs = new List<AnyOf<Outdoors, Surface>>() { new Outdoors(), new Surface(new List<string>()) };
            var bcDP = DialogHelper.MakeDropDownForAnyOfType(door.BoundaryCondition.Obj.GetType().Name, v => door.BoundaryCondition = v, bcs);
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
            var inShds = door.IndoorShades;
            if (inShds != null)
            {
                var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                inShadesListBox.Items.AddRange(idShds);
            }
            layout.AddSeparateRow(inShadesListBox);

            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            var outShds = door.OutdoorShades;
            if (outShds != null)
            {
                var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                outShadesListBox.Items.AddRange(outShdItems);
            }
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(door.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;
            //layout.up

          
        }

        private static Panel _doorPanel;
        public static Panel UpdateDoorPanel(Door HoneybeeObj, System.Action<string> geometryReset = default)
        {
            var vm = DoorViewModel.Instance;
            vm.Update(HoneybeeObj, geometryReset);
            if (_doorPanel == null)
                _doorPanel = GenDoorPanel();
            return _doorPanel;
        }

        private static Panel GenDoorPanel()
        {
            var vm = DoorViewModel.Instance;

            var layout = new DynamicLayout { DataContext = vm};
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow(new Label { Text = "ID: " }, id);


            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            //layout.AddSeparateRow(new Label { Text = "Glass:" });
            var isGlassCBox = new CheckBox();
            isGlassCBox.CheckedBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.IsGlass);
            isGlassCBox.CheckedChanged += (s, e) => { vm.ActionWhenChanged($"Set Glass Door: {vm.HoneybeeObject.IsGlass}"); };
            layout.AddSeparateRow(new Label { Text = "Glass:" }, isGlassCBox);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show(Helper.Owner, "Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = vm.HoneybeeObject.Properties.Energy ?? new DoorEnergyPropertiesAbridged();
                energyProp = DoorEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_DoorEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal(Helper.Owner);
                if (dialog_rc != null)
                {
                    vm.HoneybeeObject.Properties.Energy = dialog_rc;
                    vm.ActionWhenChanged($"Set Door Energy Properties");
                }

            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcDP = new DropDown();
            bcDP.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.Bcs);
            bcDP.ItemTextBinding = Binding.Delegate<AnyOf, string>(m => m.Obj.GetType().Name);
            bcDP.SelectedIndexBinding.BindDataContext((DoorViewModel m) => m.SelectedIndex);
            layout.AddSeparateRow(bcDP);

            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.BindDataContext(c => c.Enabled, (DoorViewModel m) => m.IsOutdoor, DualBindingMode.OneWay);
            bcBtn.Click += (s, e) =>
            {
                if (vm.HoneybeeObject.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal(Helper.Owner);
                    if (dialog_rc != null)
                    {
                        vm.HoneybeeObject.BoundaryCondition = dialog_rc;
                        vm.ActionWhenChanged($"Set Aperture Boundary Condition");
                    }
                }
                else
                {
                    MessageBox.Show(Helper.Owner, "Only Outdoors type has additional properties to edit!");
                }
            };
            layout.AddSeparateRow(bcBtn);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            inShadesListBox.Height = 50;
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, vm.HoneybeeObject.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }



    }
}
