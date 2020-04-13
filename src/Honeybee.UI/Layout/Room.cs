using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HoneybeeSchema;
using System.Collections.Generic;
using System;

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
        public static Panel GenRoomPanel(Room HoneybeeObj, Action<string> geometryReset = default)
        {
            
            var room = HoneybeeObj;
            var objID = room.Identifier;
            geometryReset = geometryReset ?? delegate(string m){}; //Do nothing if geometryReset is null

            var layout = new DynamicLayout { };
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {room.Identifier}" });

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            room.DisplayName = room.DisplayName ?? string.Empty;
            nameTB.TextBinding.Bind(room, m => m.DisplayName );
            nameTB.LostFocus += (s, e) => { geometryReset($"Set Room Name {room.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var rmPropBtn = new Button { Text = "Room Energy Properties" };
            rmPropBtn.Click += (s, e) =>
            {
                var energyProp = room.Properties.Energy ?? new RoomEnergyPropertiesAbridged();
                energyProp = RoomEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_RoomEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    room.Properties.Energy = dialog_rc;
                    geometryReset($"Set {objID} Energy Properties ");
                }
                    
            };
            layout.AddSeparateRow(rmPropBtn);

            
            var facesListBox = new ListBox();
            facesListBox.Height = 120;
            var faces = room.Faces;
            if (faces != null)
            {
                var faceItems = faces.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                facesListBox.Items.AddRange(faceItems);
            }
            layout.AddSeparateRow(new Label { Text = $"Faces: (total: {room.Faces.Count})" });
            layout.AddSeparateRow(facesListBox);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.Height = 50;
            var inShds = room.IndoorShades;
            if (inShds != null)
            {
                var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                inShadesListBox.Items.AddRange(idShds);
            }
            layout.AddSeparateRow(inShadesListBox);

            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            var outShds = room.OutdoorShades;
            if (outShds != null)
            {
                var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                outShadesListBox.Items.AddRange(outShdItems);
            }
            layout.AddSeparateRow(outShadesListBox);


            layout.AddSeparateRow(new Label { Text = "Multiplier:" });
            var multiplier_NS = new NumericStepper() { MaximumDecimalPlaces = 0, MinValue = 0};
            multiplier_NS.ValueBinding.Bind(room, m => m.Multiplier);
            multiplier_NS.LostFocus += (s, e) => { geometryReset($"Set Multiplier {room.Multiplier}"); };
  
            layout.AddSeparateRow(multiplier_NS);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(room.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

            //this.Content = layout;
            //layout.up

            //void RmPropBtn_Click(RoomEntity roomEnt)
            //{
            //    var roomEnergyProperties = RoomEnergyPropertiesAbridged.FromJson(roomEnt.GetEnergyProp().ToJson());
            //    var dialog = new UI.Dialog_RoomEnergyProperty(roomEnergyProperties);
            //    dialog.RestorePosition();
            //    var dialog_rc = dialog.ShowModal(RhinoEtoApp.MainWindow);
            //    dialog.SavePosition();
            //    if (dialog_rc != null)
            //    {
            //        //replace brep in order to add an undo history
            //        var undo = Rhino.RhinoDoc.ActiveDoc.BeginUndoRecord("Set Honeybee room energy properties");

            //        var dup = roomEnt.HostObjRef.Brep().DuplicateBrep();
            //        dup.TryGetRoomEntity().HBObject.Properties.Energy = dialog_rc;
            //        Rhino.RhinoDoc.ActiveDoc.Objects.Replace(roomEnt.HostObjRef.ObjectId, dup);

            //        Rhino.RhinoDoc.ActiveDoc.EndUndoRecord(undo);

            //    }

            //}
        }


    }
}
