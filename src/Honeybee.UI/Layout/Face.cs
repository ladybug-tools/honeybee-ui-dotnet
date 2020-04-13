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
        public static Panel GenFacePanel(Face HoneybeeObj, System.Action<string> geometryReset = default)
        {

            var face = HoneybeeObj;
            var objID = face.Identifier;
            geometryReset = geometryReset ?? delegate (string m) { }; //Do nothing if geometryReset is null

            var layout = new DynamicLayout { };
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {face.Identifier}" });
            
            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTBox = new TextBox() { };
            face.DisplayName = face.DisplayName ?? string.Empty;
            nameTBox.TextBinding.Bind(face, m => m.DisplayName );
            nameTBox.LostFocus += (s, e) => { geometryReset($"Set Face Name: {face.DisplayName}"); };
            layout.AddSeparateRow(nameTBox);

            
            layout.AddSeparateRow(new Label { Text = "Face Type:" });
            var faceTypeDP = new EnumDropDown<Face.FaceTypeEnum>();
            faceTypeDP.SelectedValueBinding.Bind(Binding.Delegate(() => face.FaceType, v => face.FaceType = v));
            faceTypeDP.LostFocus += (s, e) => { geometryReset($"Set Face Type: {face.FaceType}"); };
            layout.AddSeparateRow(faceTypeDP);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = face.Properties.Energy ?? new FaceEnergyPropertiesAbridged();
                energyProp = FaceEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_FaceEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    face.Properties.Energy = dialog_rc;
                    geometryReset($"Set Face Energy Properties");
                }
            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.Enabled = face.BoundaryCondition.Obj is Outdoors;
            bcBtn.Click += (s, e) => 
            {
                if (face.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        face.BoundaryCondition = dialog_rc;
                        geometryReset($"Set Face Boundary Condition");
                    }
                }
                //Dialogs.ShowMessage("Work in progress", "Honeybee");
            }; 
       

            var bcs = new List<AnyOf<Ground, Outdoors, Adiabatic, Surface>>() { new Ground(), new Outdoors(), new Adiabatic(), new Surface(new List<string>()) };
            var bcDP = DialogHelper.MakeDropDownForAnyOf(face.BoundaryCondition.Obj.GetType().Name, v => face.BoundaryCondition = v, bcs);
            bcDP.SelectedIndexChanged += (s, e) =>
            {
                bcBtn.Enabled = false;
                if (bcDP.SelectedKey == nameof(Outdoors))
                    bcBtn.Enabled = true;

            };

            layout.AddSeparateRow(bcDP);
            layout.AddSeparateRow(bcBtn);


            var apertureLBox = new ListBox();
            apertureLBox.Height = 100;
            var apertures = face.Apertures ?? new List<Aperture>();
            var faceCount = 0;
            if (apertures.Any())
            {
                var validApertures = apertures;
                faceCount = validApertures.Count();
                //Check if displace name is null, in case this hb object is not created from rhino. 
                var faceItems = validApertures.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                apertureLBox.Items.AddRange(faceItems);
               
            }
            layout.AddSeparateRow(new Label { Text = $"Apertures: (total: {faceCount})" });
            layout.AddSeparateRow(apertureLBox);


            var doorLBox = new ListBox();
            doorLBox.Height = 50;
            var doors = face.Doors ?? new List<Door>();
            if (doors.Any())
            {
                //Check if displace name is null, in case this hb object is not created from rhino. 
                var faceItems = doors.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                doorLBox.Items.AddRange(faceItems);
            }
            layout.AddSeparateRow(new Label { Text = $"Doors:" });
            layout.AddSeparateRow(doorLBox);

            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.Height = 50;
            var inShds = face.IndoorShades;
            if (inShds != null)
            {
                var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                inShadesListBox.Items.AddRange(idShds);
            }
            layout.AddSeparateRow(inShadesListBox);

            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            var outShds = face.OutdoorShades;
            if (outShds != null)
            {
                var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                outShadesListBox.Items.AddRange(outShdItems);
            }
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(face.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;
            //this.Content = layout;
            //layout.up

            //void FacePropBtn_Click(FaceEntity ent)
            //{
            //    var energyProp = ent.HBObject.Properties.Energy ?? new FaceEnergyPropertiesAbridged();
            //    energyProp = FaceEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
            //    var dialog = new UI.Dialog_FaceEnergyProperty(energyProp);
            //    dialog.RestorePosition();
            //    var dialog_rc = dialog.ShowModal(RhinoEtoApp.MainWindow);
            //    dialog.SavePosition();
            //    if (dialog_rc != null)
            //    {
            //        //replace brep in order to add an undo history
            //        var undo = Rhino.RhinoDoc.ActiveDoc.BeginUndoRecord("Set Honeybee face energy properties");

            //        //Dup entire room for replacement
            //        var dupRoomHost = ent.RoomHostObjRef.Brep().DuplicateBrep();
            //        //get face entity with subsurface component index
            //        var dupEnt = dupRoomHost.Faces[ent.ComponentIndex.Index].TryGetFaceEntity();
            //        //update properties
            //        dupEnt.HBObject.Properties.Energy = dialog_rc;
            //        Rhino.RhinoDoc.ActiveDoc.Objects.Replace(ent.RoomHostObjRef.ObjectId, dupRoomHost);

            //        Rhino.RhinoDoc.ActiveDoc.EndUndoRecord(undo);

            //    }

            //}


        }
       
    }
}
