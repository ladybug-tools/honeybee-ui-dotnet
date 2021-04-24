using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;

namespace Honeybee.UI.View
{
    public class Room: Panel
    {
        private RoomViewModel ViewModel { get; set; }
        private static Lazy<Room> _instance = new Lazy<Room>(() => new Room());
        public static Room Instance => _instance.Value;

        public GridView FacesGridView { get; private set; }
        private Room()
        {            
            this.ViewModel = new RoomViewModel(this);
            Initialize();
            
        }

        public void UpdatePanel(HB.ModelProperties libSource, HB.Room HoneybeeObj, Action<string> geometryReset = default, Action<HB.Face> subGeometryReset = default, Action<string> subGeometryDisplay = default)
        {
            this.ViewModel.Update(libSource, HoneybeeObj, geometryReset, subGeometryReset, subGeometryDisplay);
        }

        private void Initialize()
        {
            var vm = this.ViewModel;

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
            nameTB.TextBinding.BindDataContext((RoomViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow(new Label { Text = "Story:" });
            var storyTB = new TextBox() { };
            storyTB.TextBinding.BindDataContext((RoomViewModel m) => m.HoneybeeObject.Story);
            storyTB.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Room Story {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(storyTB);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var rmRadPropBtn = new Button { Text = "Room Radiance Properties" };
            rmRadPropBtn.Command = vm.RoomRadiancePropertyBtnClick;
            layout.AddSeparateRow(rmRadPropBtn);

            var rmEngPropBtn = new Button { Text = "Room Energy Properties" };
            rmEngPropBtn.Command = vm.RoomEnergyPropertyBtnClick;
            layout.AddSeparateRow(rmEngPropBtn);


            var faceTitle = new Label();
            faceTitle.TextBinding.BindDataContext((RoomViewModel m) => m.FaceCount);
            layout.AddSeparateRow("Faces: ", null, $"Total: ", faceTitle);
            FacesGridView = new GridView() { ShowHeader = false, AllowMultipleSelection = false };
            FacesGridView.Height = 120;
            FacesGridView.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.Faces);

            // add columns 
            var faceName = new TextBoxCell { Binding = Binding.Delegate<HB.Face, string>(r => r.DisplayName ?? r.Identifier) };
            FacesGridView.Columns.Add(new GridColumn { DataCell = faceName, HeaderText = "Name" });
            var faceTypeName = new TextBoxCell { Binding = Binding.Delegate<HB.Face, string>(r => r.FaceType.ToString()) };
            FacesGridView.Columns.Add(new GridColumn { DataCell = faceTypeName, HeaderText = "FaceType" });
            var faceBCName = new TextBoxCell { Binding = Binding.Delegate<HB.Face, string>(r => r.BoundaryCondition.Obj.GetType().Name) };
            FacesGridView.Columns.Add(new GridColumn { DataCell = faceBCName, HeaderText = "BC" });
            layout.AddSeparateRow(FacesGridView);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.Height = 50;
            inShadesListBox.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<HB.Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (RoomViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<HB.Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(outShadesListBox);


            layout.AddSeparateRow(new Label { Text = "Multiplier:" });
            var multiplier_NS = new NumericStepper() { MaximumDecimalPlaces = 0, MinValue = 0 };
            //multiplier_NS.ValueBinding.Bind(room, m => m.Multiplier);
            multiplier_NS.ValueBinding.BindDataContext<RoomViewModel>(m => m.HoneybeeObject.Multiplier);
            multiplier_NS.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Multiplier {vm.HoneybeeObject.Multiplier}"); };
            layout.AddSeparateRow(multiplier_NS);


            layout.Add(null);
            var data_button = new Button { Text = "Schema Data" };
            data_button.Command = this.ViewModel.HBDataBtnClick;
            layout.AddSeparateRow(data_button, null);


            // hook up events
            FacesGridView.SelectedItemsChanged += this.ViewModel.OnRoomFaceSelected;
            FacesGridView.LostFocus += (s, e) => { vm.Redraw(null); };
            FacesGridView.MouseDoubleClick += this.ViewModel.OnRoomFaceMouseDoubleClick;


            this.Content = layout;
        }
    }

    
}
