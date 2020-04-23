using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HoneybeeSchema;
using System.Collections.Generic;
using System;
using System.Threading;

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
            geometryReset = geometryReset ?? delegate (string m) { }; //Do nothing if geometryReset is null

            var layout = new DynamicLayout { };
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {room.Identifier}" });

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            room.DisplayName = room.DisplayName ?? string.Empty;
            nameTB.TextBinding.Bind(room, m => m.DisplayName);
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

        }

        private static Panel _roomPanel;
        public static Panel UpdateRoomPanel(Room HoneybeeObj, Action<string> geometryReset = default)
        {
            var vm = RoomViewModel.Instance;
            vm.Update(HoneybeeObj, geometryReset);
            if (_roomPanel == null)
            {
                _roomPanel = GenRoomPanel2();
            }
            return _roomPanel;
        }
  
        //this is only for testing
        private static Panel GenRoomPanel_()
        {
            var vm = RoomViewModel.Instance;
            var room = vm.HoneybeeObject;
            //var objID = room.Identifier;

            var layout = new DynamicLayout { };
            layout.MinimumSize = new Size(100, 200);

            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((RoomViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow(id);

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((RoomViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => vm.ActionWhenChanged($"Set Room Name {room.DisplayName}");
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var rmPropBtn = new Button { Text = "Room Energy Properties" };
            layout.AddSeparateRow(rmPropBtn);


            layout.AddSeparateRow(new Label { Text = "Multiplier:" });
            var multiplier_NS = new NumericStepper() { MaximumDecimalPlaces = 0, MinValue = 0 };
            multiplier_NS.ValueBinding.BindDataContext((RoomViewModel m) => m.HoneybeeObject.Multiplier);
            //multiplier_NS.LostFocus += (s, e) => { geometryReset($"Set Multiplier {room.Multiplier}"); };

            layout.AddSeparateRow(multiplier_NS);


            layout.Add(null);

            layout.DataContext = RoomViewModel.Instance;
            return layout;

        }


        private static Panel GenRoomPanel2()
        {

            var vm = RoomViewModel.Instance;
            //var room = vm.HoneybeeObject;
            //var objID = room.Identifier;

            var layout = new DynamicLayout() { DataContext = vm };
            layout.MinimumSize = new Size(100, 200);
            
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);
         
            var id = new Label();
            id.TextBinding.BindDataContext((RoomViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow(new Label { Text = "ID: " }, id);

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            //room.DisplayName = room.DisplayName ?? string.Empty;
            nameTB.TextBinding.BindDataContext((RoomViewModel m) =>  m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var rmPropBtn = new Button { Text = "Room Energy Properties" };
            //rmPropBtn.Click += (s, e) => MessageBox.Show(vm.HoneybeeObject.Identifier);
            rmPropBtn.Click += (s, e) =>
            {
                var energyProp = vm.HoneybeeObject.Properties.Energy ?? new RoomEnergyPropertiesAbridged();
                energyProp = RoomEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_RoomEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    vm.HoneybeeObject.Properties.Energy = dialog_rc;
                    vm.ActionWhenChanged($"Set {vm.HoneybeeObject.Identifier} Energy Properties ");
                }

            };
            layout.AddSeparateRow(rmPropBtn);

            //var dp = new DropDown();
            ////dp.DataStore = RoomViewModel.Instance.HoneybeeObject.Faces.Select(_ => _.DisplayName ?? _.Identifier);
            //dp.BindDataContext(c => c.DataStore, (RoomViewModel m) => room.Faces.Select(_ => _.DisplayName ?? _.Identifier));

            var facesListBox = new ListBox();
            facesListBox.Height = 120;
            //facesListBox.DataContext = room.Faces;
            //facesListBox.ItemTextBinding = Binding.Delegate<Face, string>(m => m.DisplayName);
            //facesListBox.BindDataContext(Binding.Delegate<RoomViewModel, string>(m => m.HoneybeeObject.DisplayName),);
            facesListBox.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.Faces);
            facesListBox.ItemTextBinding = Binding.Delegate<Face, string>(m => m.DisplayName ?? m.Identifier); 
            //var faces = room.Faces;
            //if (faces != null)
            //{
            //    var faceItems = faces.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
            //    facesListBox.Items.AddRange(faceItems);
            //}
            var faceTitle = new Label();
            //faceTitle.BindDataContext(c=>c.DataContext, (RoomViewModel m) => m.FaceCount);
            //faceTitle.TextBinding.Bind(Binding.Delegate<string>( ()=> RoomViewModel.Instance.FaceCount.ToString()));
            //faceTitle.TextBinding.BindDataContext<RoomViewModel>(r => r.FaceCount.ToString());
            
            faceTitle.TextBinding.BindDataContext((RoomViewModel m) => m.FaceCount);
            layout.AddSeparateRow(new Label { Text = "Faces: " }, null, new Label { Text = "Total: " }, faceTitle);
            layout.AddSeparateRow(facesListBox);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.Height = 50;
            inShadesListBox.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            //var inShds = room.IndoorShades;
            //if (inShds != null)
            //{
            //    var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
            //    inShadesListBox.Items.AddRange(idShds);
            //}
            layout.AddSeparateRow(inShadesListBox);

            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            //var outShds = room.OutdoorShades;
            //if (outShds != null)
            //{
            //    var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
            //    outShadesListBox.Items.AddRange(outShdItems);
            //}
            layout.AddSeparateRow(outShadesListBox);


            layout.AddSeparateRow(new Label { Text = "Multiplier:" });
            var multiplier_NS = new NumericStepper() { MaximumDecimalPlaces = 0, MinValue = 0 };
            //multiplier_NS.ValueBinding.Bind(room, m => m.Multiplier);
            multiplier_NS.ValueBinding.BindDataContext<RoomViewModel>(m => m.HoneybeeObject.Multiplier);
            multiplier_NS.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Multiplier {vm.HoneybeeObject.Multiplier}"); };

            layout.AddSeparateRow(multiplier_NS);


            layout.Add(null);
            //var data_button = new Button { Text = "Honeybee Data" };
            //data_button.Click += (sender, e) => MessageBox.Show(room.ToJson(), "Honeybee Data");
            //layout.AddSeparateRow(data_button, null);

            return layout;

        }

    }



}
