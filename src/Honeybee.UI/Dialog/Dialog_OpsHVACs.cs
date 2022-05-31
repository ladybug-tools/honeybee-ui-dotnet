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

            var refHvac = hvac ?? new HoneybeeSchema.VAV($"VAV {Guid.NewGuid().ToString().Substring(0, 5)}");
            _vm = new OpsHVACsViewModel(libSource, refHvac, this);
            var layout = new DynamicLayout() { DataContext = _vm};
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(5);


            // add a new system from lib
            if (hvac == null)
            {
                // UI for system groups
                var hvacGroups = new DropDown();
                var hvacTypes = new DropDown();
                var hvacEquipments = new DropDown();

                hvacGroups.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacGroups);
                hvacGroups.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacGroup);

                hvacTypes.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacTypes);
                hvacTypes.ItemTextBinding = Binding.Delegate<Type, string>(t => _vm.HVACTypesDic[t]);
                hvacTypes.SelectedValueBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacType);

                hvacEquipments.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacEquipmentTypes);
                hvacEquipments.ItemTextBinding = Binding.Delegate<string, string>(t => _vm.HVACsDic[t]);
                hvacEquipments.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacEquipmentType);

                layout.AddRow("HVAC Groups:");
                layout.AddRow(hvacGroups);
                layout.AddRow("HVAC Types:");
                layout.AddRow(hvacTypes);
                layout.AddRow("HVAC Equipment Types:");
                layout.AddRow(hvacEquipments);
                layout.AddRow(null);
            }

            

            var year = new DropDown();
            var nameText = new TextBox();
            var economizer = new DropDown();
            var sensible = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };
            var latent = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };


            nameText.TextBinding.BindDataContext((OpsHVACsViewModel m) => m.Name);

            year.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Vintages);
            year.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Vintage);

            var economizerTitle = new Label() { Text = "Economizer:" };
            economizer.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Economizers);
            economizer.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Economizer);
            economizer.BindDataContext(c => c.Enabled, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizer.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizerTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);

            var sensibleTitle = new Label() { Text = "Sensible Heat Recovery:" };
            sensible.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.SensibleHR);
            sensible.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
            sensibleTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
      

            var latentTitle = new Label() { Text = "Latent Heat Recovery:" };
            latent.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.LatentHR);
            latent.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);
            latentTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);

            var dcv = new CheckBox() { Text = "Demand Control Ventilation" };
            dcv.Bind(_ => _.Checked, _vm, _ => _.DcvChecked);
            dcv.Bind(c => c.Visible, _vm, _ => _.DcvVisable);

            var availabilityTitle = new Label() { Text = "Availability Schedule:" };
            availabilityTitle.Bind(c => c.Visible, _vm, _ => _.AvaliabilityVisable);
            var availability = new OptionalButton();
            availability.Bind(c => c.Visible, _vm, _ => _.AvaliabilityVisable);
            availability.TextBinding.Bind(_vm, _ => _.AvaliabilitySchedule.BtnName);
            availability.Bind(_ => _.Command, _vm, _ => _.AvaliabilityCommand);
            availability.Bind(_ => _.RemoveCommand, _vm, _ => _.RemoveAvaliabilityCommand);
            availability.Bind(_ => _.IsRemoveVisable, _vm, _ => _.AvaliabilitySchedule.IsRemoveVisable);

            layout.BeginGroup("HVAC System settings", new Padding(5), new Size(5, 5), yscale: true);
            layout.AddRow("Name:");
            layout.AddRow(nameText);
            layout.AddRow("Vintage:");
            layout.AddRow(year);
            layout.AddRow(economizerTitle);
            layout.AddRow(economizer);
            layout.AddRow(sensibleTitle);
            layout.AddRow(sensible);
            layout.AddRow(latentTitle);
            layout.AddRow(latent);

            layout.AddRow(availabilityTitle);
            layout.AddRow(availability);
            layout.AddRow(dcv);
            layout.EndGroup();
            //layout.AddSeparateRow(cloneBtn);

            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OKButton = new Button { Text = "OK", Enabled = !lockedMode };
            OKButton.Click += (sender, e) => {
                var obj = _vm.GreateHvac(hvac);
                OkCommand.Execute(obj);
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            layout.AddSeparateRow(locked, null, OKButton, this.AbortButton, null, null);
            //layout.AddRow(null);
            Content = layout;


        }
        
     

    }
}
