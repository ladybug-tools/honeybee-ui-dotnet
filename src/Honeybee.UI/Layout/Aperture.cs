using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;

namespace Honeybee.UI.View
{
    [Obsolete("This is deprecated, please use ApertureProperty instead", true)]
    public class Aperture: Panel
    {
        private ApertureViewModel ViewModel { get; set; }
        private static Lazy<Aperture> _instance = new Lazy<Aperture>(() => new Aperture());
        public static Aperture Instance => _instance.Value;

        private Aperture()
        {
            this.ViewModel = new ApertureViewModel();
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, HB.Aperture HoneybeeObj, System.Action<string> geometryReset = default)
        {
            this.ViewModel.Update(libSource, HoneybeeObj, geometryReset);
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
            id.TextBinding.BindDataContext((ApertureViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow("ID: ", id);


            layout.AddSeparateRow("Name:");
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((ApertureViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            //layout.AddSeparateRow(new Label { Text = "Operable:" });
            var operableCBox = new CheckBox();
            operableCBox.CheckedBinding.BindDataContext((ApertureViewModel m) => m.HoneybeeObject.IsOperable);
            operableCBox.CheckedChanged += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Aperture Operable: {vm.HoneybeeObject.IsOperable}"); };
            layout.AddSeparateRow("Operable:", operableCBox);


            layout.AddSeparateRow("Properties:");
            var faceRadPropBtn = new Button { Text = "Radiance Properties" };
            faceRadPropBtn.Command = vm.ApertureRadiancePropertyBtnClick;
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Command = vm.ApertureEnergyPropertyBtnClick;
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow("Boundary Condition:");
            var bcDP = new DropDown();
            bcDP.BindDataContext(c => c.DataStore, (ApertureViewModel m) => m.Bcs);
            bcDP.ItemTextBinding = Binding.Delegate<HB.AnyOf, string>(m => m.Obj.GetType().Name);
            bcDP.SelectedIndexBinding.BindDataContext((ApertureViewModel m) => m.SelectedIndex);
            layout.AddSeparateRow(bcDP);

            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.BindDataContext(c => c.Enabled, (ApertureViewModel m) => m.IsOutdoor, DualBindingMode.OneWay);
            bcBtn.Command = vm.EditBoundaryConditionBtnClick;
            layout.AddSeparateRow(bcBtn);


            layout.AddSeparateRow("IndoorShades:");
            var inShadesListBox = new ListBox();
            inShadesListBox.BindDataContext(c => c.DataStore, (ApertureViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<HB.Shade, string>(m => m.DisplayName ?? m.Identifier);
            inShadesListBox.Height = 50;
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow("OutdoorShades:");
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (ApertureViewModel m) => m.HoneybeeObject.OutdoorShades);
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
