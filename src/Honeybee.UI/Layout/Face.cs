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
    public class Face: Panel
    {
        private FaceViewModel ViewModel { get; set; }
        private static Lazy<Face> _instance = new Lazy<Face>(() => new Face());
        public static Face Instance => _instance.Value;

        private Face()
        {
            this.ViewModel = new FaceViewModel();
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, HB.Face HoneybeeObj, System.Action<string> geometryReset = default)
        {
            this.ViewModel.Update(libSource, HoneybeeObj, geometryReset);
        }

        private void Initialize()
        {
            var vm = this.ViewModel;
            //var face = vm.HoneybeeObject;
            //var objID = face.Identifier;

            var layout = new DynamicLayout { DataContext = vm };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((FaceViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow("ID: ", id);

            layout.AddSeparateRow("Name:");
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((FaceViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Face Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow("Face Type:");
            var faceTypeDP = new EnumDropDown<HB.FaceType>();
            faceTypeDP.SelectedValueBinding.BindDataContext((FaceViewModel m) => m.HoneybeeObject.FaceType);
            faceTypeDP.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Face Type: {vm.HoneybeeObject.FaceType}"); };
            layout.AddSeparateRow(faceTypeDP);


            layout.AddSeparateRow("Properties:");
            var faceRadPropBtn = new Button { Text = "Radiance Properties" };
            faceRadPropBtn.Command = this.ViewModel.FaceRadiancePropertyBtnClick;
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Command = this.ViewModel.FaceEnergyPropertyBtnClick;
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow("Boundary Condition:");
            var bcDP = new DropDown();
            bcDP.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.Bcs);
            bcDP.ItemTextBinding = Binding.Delegate<HB.AnyOf, string>(m => m.Obj.GetType().Name);
            bcDP.SelectedIndexBinding.BindDataContext((FaceViewModel m) => m.SelectedIndex);
            layout.AddSeparateRow(bcDP);

            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.BindDataContext(c => c.Enabled, (FaceViewModel m) => m.IsOutdoor, DualBindingMode.OneWay);
            bcBtn.Command = this.ViewModel.EditFaceBoundaryConditionBtnClick;
            layout.AddSeparateRow(bcBtn);


            var aptCount = new Label();
            aptCount.TextBinding.BindDataContext((FaceViewModel m) => m.ApertureCount);
            layout.AddSeparateRow("Apertures:", null, $"Total: ", aptCount);
            var apertureLBox = new ListBox();
            apertureLBox.Height = 100;
            apertureLBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.Apertures);
            apertureLBox.ItemTextBinding = Binding.Delegate<HB.Aperture, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(apertureLBox);


            layout.AddSeparateRow("Doors:");
            var doorLBox = new ListBox();
            doorLBox.Height = 50;
            doorLBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.Doors);
            doorLBox.ItemTextBinding = Binding.Delegate<HB.Door, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(doorLBox);


            layout.AddSeparateRow("IndoorShades:");
            var inShadesListBox = new ListBox();
            inShadesListBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<HB.Shade, string>(m => m.DisplayName ?? m.Identifier);
            inShadesListBox.Height = 50;
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow("OutdoorShades:");
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<HB.Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Schema Data" };
            data_button.Click += (sender, e) => Dialog_Message.Show(Config.Owner, vm.HoneybeeObject.ToJson(true), "Schema Data");
            layout.AddSeparateRow(data_button, null);

            this.Content = layout;
        }
    }
    
}
