using Eto.Drawing;
using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class Dialog_OpsHVACSelector : Dialog<HoneybeeSchema.Energy.IHvac>
    {
        private OpsHVACSelectorViewModel _vm;
        public Dialog_OpsHVACSelector()
        {

            this.Padding = new Padding(10);
            Title = $"From OpenStudio HVAC library - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 550;
            this.Icon = DialogHelper.HoneybeeIcon;

            _vm = new OpsHVACSelectorViewModel(this);

            var layout = new DynamicLayout() { DataContext = _vm };
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(5);


            // UI for system groups
            var hvacGroups = new DropDown();

            hvacGroups.BindDataContext(c => c.DataStore, (OpsHVACSelectorViewModel m) => m.HvacGroups);
            hvacGroups.SelectedKeyBinding.BindDataContext((OpsHVACSelectorViewModel m) => m.HvacGroup);

            layout.AddRow("HVAC Groups:");
            layout.AddRow(hvacGroups);

            var hvacTypes = new DropDown();
            hvacTypes.BindDataContext(c => c.DataStore, (OpsHVACSelectorViewModel m) => m.HvacTypes);
            hvacTypes.ItemTextBinding = Binding.Delegate<Type, string>(t => _vm.HVACTypesDic[t]);
            hvacTypes.SelectedValueBinding.BindDataContext((OpsHVACSelectorViewModel m) => m.HvacType);

            var hvacSummary = new TextArea() { Height = 100};
            hvacSummary.Bind(c => c.Text, _vm, _ => _.HvacTypeSummary);


            layout.AddRow("HVAC Types:");
            layout.AddRow(hvacTypes);
            layout.AddRow(hvacSummary);
            layout.AddRow(null);


            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) =>
            {
                try
                {
                    this.Close(_vm.GreateHvac());
                }
                catch (Exception ex)
                {
                    Dialog_Message.Show(this, ex);
                }
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            layout.AddSeparateRow(null, OKButton, this.AbortButton, null);
            //layout.AddRow(null);
            Content = layout;


        }

    }
}
