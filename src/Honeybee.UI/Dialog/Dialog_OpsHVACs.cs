using Eto.Drawing;
using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class Dialog_OpsHVACs : Dialog_ResourceEditor<HoneybeeSchema.Energy.IHvac>
    {
        private OpsHVACsViewModel _vm;
        //private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_OpsHVACs(ref HoneybeeSchema.ModelEnergyProperties libSource, HoneybeeSchema.Energy.IHvac hvac = default, bool lockedMode = false)
        {

            this.Padding = new Padding(10);
            Title = $"From OpenStudio HVAC library - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            _vm = new OpsHVACsViewModel(libSource, hvac, this);

            var layout = new DynamicLayout() { DataContext = _vm };
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(5);
      

            var yearTitle = new Label() { Text = "Vintage:" };
            yearTitle.Bind(_ => _.ToolTip, _vm, _ => _.VintageTip);
            var year = new DropDown();
            var nameTitle = new Label() { Text = "Name:" };
            nameTitle.Bind(_ => _.ToolTip, _vm, _ => _.NameTip);
            var nameText = new TextBox();
            var economizer = new DropDown();
            var sensible = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };
            var latent = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };


            nameText.TextBinding.BindDataContext((OpsHVACsViewModel m) => m.Name);

            year.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Vintages);
            year.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Vintage);


            var hvacEquipments = new DropDown();
            hvacEquipments.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacEquipmentTypes);
            hvacEquipments.ItemTextBinding = Binding.Delegate<string, string>(t => _vm.HVACsDic[t]);
            hvacEquipments.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacEquipmentType);


            var economizerTitle = new Label() { Text = "Economizer:" };
            economizer.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Economizers);
            economizer.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Economizer);
            economizer.BindDataContext(c => c.Enabled, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizer.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizerTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizerTitle.Bind(_ => _.ToolTip, _vm, _ => _.EconomizerTip);
       

            var sensibleTitle = new Label() { Text = "Sensible Heat Recovery:" };
            sensible.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.SensibleHR);
            sensible.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
            sensibleTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
            sensibleTitle.Bind(_ => _.ToolTip, _vm, _ => _.SensibleHRTip);

            var latentTitle = new Label() { Text = "Latent Heat Recovery:" };
            latent.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.LatentHR);
            latent.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);
            latentTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);
            latentTitle.Bind(_ => _.ToolTip, _vm, _ => _.LatentHRTip);

            var dcvTitle = new Label() { Text = "Demand Control Ventilation:" };
            var dcv = new CheckBox();
            dcv.Bind(_ => _.Checked, _vm, _ => _.DcvChecked);
            dcv.Bind(c => c.Visible, _vm, _ => _.DcvVisable);
            dcvTitle.Bind(_ => _.ToolTip, _vm, _ => _.DCVTip);
            dcvTitle.Bind(_ => _.Visible, _vm, _ => _.DcvVisable);

            var availabilityTitle = new Label() { Text = "DOAS Availability Schedule:" };
            availabilityTitle.Bind(c => c.Visible, _vm, _ => _.AvaliabilityVisable);
            var availability = new OptionalButton();
            availability.Bind(c => c.Visible, _vm, _ => _.AvaliabilityVisable);
            availability.TextBinding.Bind(_vm, _ => _.AvaliabilitySchedule.BtnName);
            availability.Bind(_ => _.Command, _vm, _ => _.AvaliabilityCommand);
            availability.Bind(_ => _.RemoveCommand, _vm, _ => _.RemoveAvaliabilityCommand);
            availability.Bind(_ => _.IsRemoveVisable, _vm, _ => _.AvaliabilitySchedule.IsRemoveVisable);
            availabilityTitle.Bind(_ => _.ToolTip, _vm, _ => _.AvaliabilityTip);

            var radSettings = GenRadSettingsPanel();

            //var gp = new GroupBox() { Text = "HVAC System settings" };
            //var gpLayout = new DynamicLayout();
            //gpLayout.DefaultPadding = new Padding(5);

            //gpLayout.Height = 250;
            //gpLayout.BeginScrollable(BorderType.None);

            var gpGeneralLayout = new DynamicLayout();
            //gpLayout.BeginGroup("HVAC System settings", new Padding(5), new Size(5, 0));
            gpGeneralLayout.Spacing = new Size(5, 2);

         
            gpGeneralLayout.AddRow(nameTitle);
            gpGeneralLayout.AddRow(nameText);
            gpGeneralLayout.AddRow(yearTitle);
            gpGeneralLayout.AddRow(year);
            gpGeneralLayout.AddRow("HVAC Equipment Types:");
            gpGeneralLayout.AddRow(hvacEquipments);
            gpGeneralLayout.AddRow(economizerTitle);
            gpGeneralLayout.AddRow(economizer);
            gpGeneralLayout.AddRow(sensibleTitle);
            gpGeneralLayout.AddRow(sensible);
            gpGeneralLayout.AddRow(latentTitle);
            gpGeneralLayout.AddRow(latent);

            gpGeneralLayout.AddRow(availabilityTitle);
            gpGeneralLayout.AddRow(availability);
            gpGeneralLayout.AddSeparateRow(dcvTitle, dcv);

            layout.AddRow(gpGeneralLayout);
            layout.AddRow(radSettings);
         
            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OKButton = new Button { Text = "OK", Enabled = !lockedMode };
            OKButton.Click += (sender, e) =>
            {
                try
                {
                    var obj = _vm.GreateHvac(); 
                    OkCommand.Execute(obj);
                }
                catch (Exception ex)
                {
                    Dialog_Message.Show(this, ex);
                }
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            layout.AddSeparateRow(locked, null, OKButton, this.AbortButton, null, null);
            //layout.AddRow(null);
            Content = layout;


        }

        private DynamicLayout GenRadSettingsPanel()
        {
            //radiant system
            var radFaceTitle = new Label() { Text = "Radiant Face Type:" };
            var radFaceType = new EnumDropDown<HoneybeeSchema.RadiantFaceTypes>();
            radFaceType.Bind(_ => _.SelectedValue, _vm, _ => _.RadiantFaceType);
            //radFaceType.Bind(_ => _.Visible, _vm, _ => _.RadiantVisable);
            radFaceTitle.Bind(_ => _.ToolTip, _vm, _ => _.RadiantFaceTip);

            var minOptTitle = new Label() { Text = "Minimum Operation Time:" };
            minOptTitle.Bind(_ => _.ToolTip, _vm, _ => _.MinOptTimeTip);
            var rswitchTimeTitle = new Label() { Text = "Switch Over Time:" };
            rswitchTimeTitle.Bind(_ => _.ToolTip, _vm, _ => _.SwitchTimeTip);
            var minOptTime = new NumericStepper() { MinValue = 0, MaxValue = 24, MaximumDecimalPlaces = 1, Increment = 1 };
            var switchOverTime = new NumericStepper() { MinValue = 0, MaxValue = 24, MaximumDecimalPlaces = 1, Increment = 1 };
            minOptTime.Bind(_ => _.Value, _vm, _ => _.MinOptTime);
            //minOptTime.Bind(_ => _.Visible, _vm, _ => _.RadiantVisable);
            switchOverTime.Bind(_ => _.Value, _vm, _ => _.SwitchTime);
            //switchOverTime.Bind(_ => _.Visible, _vm, _ => _.RadiantVisable);

            var radSettings = new DynamicLayout();
            radSettings.DefaultSpacing = new Size(5, 2);
            radSettings.Bind(_ => _.Visible, _vm, _ => _.RadiantVisable);
            radSettings.AddRow(radFaceTitle);
            radSettings.AddRow(radFaceType);
            radSettings.AddRow(minOptTitle);
            radSettings.AddRow(minOptTime);
            radSettings.AddRow(rswitchTimeTitle);
            radSettings.AddRow(switchOverTime);
            return radSettings;
        }


    }
}
