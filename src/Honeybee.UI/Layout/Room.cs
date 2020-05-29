using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public static partial class PanelHelper
    {
        
        ///// <summary>
        ///// Create Eto panel based on Honeybee geomerty. 
        ///// If input HoneybeeObj is a duplicated object, geometryReset action must be provided, 
        ///// otherwise no changes would be applied to original honeybee object.
        ///// </summary>
        ///// <param name="HoneybeeObj"></param>
        ///// <param name="geometryReset"></param>
        ///// <returns></returns>
        //public static Panel GenRoomPanel(Room HoneybeeObj, Action<string> geometryReset = default)
        //{

        //    var room = HoneybeeObj;
        //    var objID = room.Identifier;
        //    geometryReset = geometryReset ?? delegate (string m) { }; //Do nothing if geometryReset is null

        //    var layout = new DynamicLayout { };
        //    layout.Spacing = new Size(5, 5);
        //    layout.Padding = new Padding(10);
        //    layout.DefaultSpacing = new Size(2, 2);


        //    layout.AddSeparateRow(new Label { Text = $"ID: {room.Identifier}" });

        //    layout.AddSeparateRow(new Label { Text = "Name:" });
        //    var nameTB = new TextBox() { };
        //    room.DisplayName = room.DisplayName ?? string.Empty;
        //    nameTB.TextBinding.Bind(room, m => m.DisplayName);
        //    nameTB.LostFocus += (s, e) => { geometryReset($"Set Room Name {room.DisplayName}"); };
        //    layout.AddSeparateRow(nameTB);


        //    layout.AddSeparateRow(new Label { Text = "Properties:" });
        //    var rmPropBtn = new Button { Text = "Room Energy Properties" };
        //    rmPropBtn.Click += (s, e) =>
        //    {
        //        var energyProp = room.Properties.Energy ?? new RoomEnergyPropertiesAbridged();
        //        energyProp = RoomEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
        //        var dialog = new Dialog_RoomEnergyProperty(energyProp);
        //        var dialog_rc = dialog.ShowModal();
        //        if (dialog_rc != null)
        //        {
        //            room.Properties.Energy = dialog_rc;
        //            geometryReset($"Set {objID} Energy Properties ");
        //        }
                    
        //    };
        //    layout.AddSeparateRow(rmPropBtn);

            
        //    var facesListBox = new ListBox();
        //    facesListBox.Height = 120;
        //    var faces = room.Faces;
        //    if (faces != null)
        //    {
        //        var faceItems = faces.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
        //        facesListBox.Items.AddRange(faceItems);
        //    }
        //    layout.AddSeparateRow(new Label { Text = $"Faces: (total: {room.Faces.Count})" });
        //    layout.AddSeparateRow(facesListBox);


        //    layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
        //    var inShadesListBox = new ListBox();
        //    inShadesListBox.Height = 50;
        //    var inShds = room.IndoorShades;
        //    if (inShds != null)
        //    {
        //        var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
        //        inShadesListBox.Items.AddRange(idShds);
        //    }
        //    layout.AddSeparateRow(inShadesListBox);

        //    layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
        //    var outShadesListBox = new ListBox();
        //    outShadesListBox.Height = 50;
        //    var outShds = room.OutdoorShades;
        //    if (outShds != null)
        //    {
        //        var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
        //        outShadesListBox.Items.AddRange(outShdItems);
        //    }
        //    layout.AddSeparateRow(outShadesListBox);


        //    layout.AddSeparateRow(new Label { Text = "Multiplier:" });
        //    var multiplier_NS = new NumericStepper() { MaximumDecimalPlaces = 0, MinValue = 0};
        //    multiplier_NS.ValueBinding.Bind(room, m => m.Multiplier);
        //    multiplier_NS.LostFocus += (s, e) => { geometryReset($"Set Multiplier {room.Multiplier}"); };
  
        //    layout.AddSeparateRow(multiplier_NS);


        //    layout.Add(null);
        //    var data_button = new Button { Text = "Honeybee Data" };
        //    data_button.Click += (sender, e) => MessageBox.Show(room.ToJson(), "Honeybee Data");
        //    layout.AddSeparateRow(data_button, null);

        //    return layout;

        //}

        
        private static Panel _roomPanel;
        public static Panel UpdateRoomPanel(Room HoneybeeObj, Action<string> geometryReset = default, Action<string> redrawDisplay = default)
        {
            var vm = RoomViewModel.Instance;
            vm.Update(HoneybeeObj, geometryReset, redrawDisplay);
            if (_roomPanel == null)
            {
                _roomPanel = GenRoomPanel();
            }
            return _roomPanel;
        }
  
        private static Panel GenRoomPanel()
        {

            var vm = RoomViewModel.Instance;

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
            nameTB.TextBinding.BindDataContext((RoomViewModel m) =>  m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var rmPropBtn = new Button { Text = "Room Energy Properties" };
            rmPropBtn.Click += (s, e) =>
            {
                var energyProp = vm.HoneybeeObject.Properties.Energy ?? new RoomEnergyPropertiesAbridged();
                energyProp = RoomEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_RoomEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal(Helper.Owner);
                if (dialog_rc != null)
                {
                    vm.HoneybeeObject.Properties.Energy = dialog_rc;
                    vm.ActionWhenChanged($"Set {vm.HoneybeeObject.Identifier} Energy Properties ");
                }

            };
            layout.AddSeparateRow(rmPropBtn);


            var faceTitle = new Label();
            faceTitle.TextBinding.BindDataContext((RoomViewModel m) => $"Total: {m.FaceCount}" );
            layout.AddSeparateRow("Faces: ", null, faceTitle);
            var facesGridView = new GridView() { ShowHeader = false, AllowMultipleSelection = false};
            facesGridView.Height = 120;
            facesGridView.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.Faces);
            // add columns 
            var faceName = new TextBoxCell { Binding = Binding.Delegate<Face, string>(r => r.DisplayName ?? r.Identifier) };
            facesGridView.Columns.Add(new GridColumn { DataCell = faceName, HeaderText = "Name" });
            var faceTypeName = new TextBoxCell { Binding = Binding.Delegate<Face, string>(r => r.FaceType.ToString()) };
            facesGridView.Columns.Add(new GridColumn { DataCell = faceTypeName, HeaderText = "FaceType" });
            var faceBCName = new TextBoxCell { Binding = Binding.Delegate<Face, string>(r => r.BoundaryCondition.Obj.GetType().Name) };
            facesGridView.Columns.Add(new GridColumn { DataCell = faceBCName, HeaderText = "BC" });

            facesGridView.SelectedItemsChanged += (s, e) =>
            {
                var sel = facesGridView.SelectedItem as Face;
                if (sel != null)
                    vm.Redraw(sel.Identifier);
            };
            facesGridView.LostFocus += (s, e) =>
            {
                vm.Redraw(null);
            };

            facesGridView.MouseDoubleClick += (s, e) =>
            {
                var sel = facesGridView.SelectedItem as Face;
                if (sel == null)
                    return;

                var dialog = new Dialog_Face(sel);
                var dialog_rc = dialog.ShowModal(Helper.Owner);
                if (dialog_rc != null)
                {
                    //MessageBox.Show(dialog_rc.ToJson());
                    var faces = vm.HoneybeeObject.Faces;
                    var index = faces.FindIndex(_ => _.Identifier == dialog_rc.Identifier);
                    vm.HoneybeeObject.Faces[index] = dialog_rc;

                    vm.ActionWhenChanged($"Set {dialog_rc.Identifier} Properties");
                }
                //MessageBox.Show(sel.ToJson());

            };
            layout.AddSeparateRow(facesGridView);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.Height = 50;
            inShadesListBox.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(outShadesListBox);


            layout.AddSeparateRow(new Label { Text = "Multiplier:" });
            var multiplier_NS = new NumericStepper() { MaximumDecimalPlaces = 0, MinValue = 0 };
            //multiplier_NS.ValueBinding.Bind(room, m => m.Multiplier);
            multiplier_NS.ValueBinding.BindDataContext<RoomViewModel>(m => m.HoneybeeObject.Multiplier);
            multiplier_NS.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Multiplier {vm.HoneybeeObject.Multiplier}"); };
            layout.AddSeparateRow(multiplier_NS);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, vm.HoneybeeObject.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }

    }



}
