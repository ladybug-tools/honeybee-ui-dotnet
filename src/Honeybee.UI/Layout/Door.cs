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
            var bcDP = DialogHelper.MakeDropDownForAnyOf(door.BoundaryCondition.Obj.GetType().Name, v => door.BoundaryCondition = v, bcs);
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



    }
}
