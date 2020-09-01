using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;

namespace Honeybee.UI.View
{
    /// <summary>
    /// Create Eto panel based on Honeybee geomerty. 
    /// If input HoneybeeObj is a duplicated object, geometryReset action must be provided, 
    /// otherwise no changes would be applied to original honeybee object.
    /// </summary>
    public class Door: Panel
    {
        private DoorViewModel ViewModel { get; set; }
        private static Lazy<Door> _instance = new Lazy<Door>(() => new Door());
        public static Door Instance => _instance.Value;

        private Door()
        {
            this.ViewModel = new DoorViewModel();
            Initialize();
        }

        public void UpdatePanel(HB.Door HoneybeeObj, System.Action<string> geometryReset = default)
        {
            this.ViewModel.Update(HoneybeeObj, geometryReset);
        }

        private void Initialize() 
        {
            var vm = this.ViewModel;

            var layout = new DynamicLayout { DataContext = vm };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow("ID: ", id);


            layout.AddSeparateRow("Name:");
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set door name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            var isGlassCBox = new CheckBox();
            isGlassCBox.CheckedBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.IsGlass);
            isGlassCBox.CheckedChanged += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Glass Door: {vm.HoneybeeObject.IsGlass}"); };
            layout.AddSeparateRow("Glass:", isGlassCBox);


            layout.AddSeparateRow("Properties:");
            var faceRadPropBtn = new Button { Text = "Radiance Properties" };
            faceRadPropBtn.Command = vm.FaceRadiancePropertyBtnClick;
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Command = vm.FaceEnergyPropertyBtnClick;
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow("Boundary Condition:");
            var bcDP = new DropDown();
            bcDP.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.Bcs);
            bcDP.ItemTextBinding = Binding.Delegate<HB.AnyOf, string>(m => m.Obj.GetType().Name);
            bcDP.SelectedIndexBinding.BindDataContext((DoorViewModel m) => m.SelectedIndex);
            layout.AddSeparateRow(bcDP);

            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.BindDataContext(c => c.Enabled, (DoorViewModel m) => m.IsOutdoor, DualBindingMode.OneWay);
            bcBtn.Command = vm.EditBoundaryConditionBtnClick;
            layout.AddSeparateRow(bcBtn);


            layout.AddSeparateRow("IndoorShades:");
            var inShadesListBox = new ListBox();
            inShadesListBox.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<HB.Shade, string>(m => m.DisplayName ?? m.Identifier);
            inShadesListBox.Height = 50;
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow("OutdoorShades:");
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<HB.Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, vm.HoneybeeObject.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            this.Content = layout;
        }
    }
 
}
