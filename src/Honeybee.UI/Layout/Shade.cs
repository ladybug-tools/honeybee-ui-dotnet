using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;

namespace Honeybee.UI.View
{
    public class Shade : Panel
    {
        private ShadeViewModel ViewModel { get; set; }
        private static Lazy<Shade> _instance = new Lazy<Shade>(() => new Shade());
        public static Shade Instance => _instance.Value;

        private Shade()
        {
            this.ViewModel = new ShadeViewModel();
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, HB.Shade HoneybeeObj, System.Action<string> geometryReset = default)
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
            layout.DefaultSpacing = new Size(5, 5);

            var id = new Label();
            id.TextBinding.BindDataContext((ShadeViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow("ID: ", id);


            layout.AddRow("Name:");
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((ShadeViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Shade Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddRow(nameTB);

            layout.AddRow("IsSiteContext:");
            var isContextTB = new CheckBox() { };
            isContextTB.CheckedBinding.BindDataContext((ShadeViewModel m) => m.HoneybeeObject.IsDetached);
            isContextTB.LostFocus += (s, e) => { vm.ActionWhenChanged?.Invoke($"Set Shade as site context {vm.HoneybeeObject.IsDetached}"); };
            layout.AddRow(isContextTB);

            layout.AddRow("Properties:");
            var faceRadPropBtn = new Button { Text = "Radiance Properties" };
            faceRadPropBtn.Command = vm.ShadeRadiancePropertyBtnClick;
            layout.AddRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Command = vm.ShadeEnergyPropertyBtnClick;
            layout.AddRow(faceEngPropBtn);


            layout.Add(null);
            var data_button = new Button { Text = "Schema Data" };
            data_button.Click += (sender, e) => Dialog_Message.Show(Config.Owner, vm.HoneybeeObject.ToJson(), "Schema Data");
            layout.AddSeparateRow(data_button, null);

            this.Content = layout;

        }
    }
   

}
