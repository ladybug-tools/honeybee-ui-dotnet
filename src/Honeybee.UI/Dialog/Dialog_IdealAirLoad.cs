using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class Dialog_IdealAirLoad : Dialog_ResourceEditor<IdealAirSystemAbridged>
    {
        //private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_IdealAirLoad(ref HoneybeeSchema.ModelEnergyProperties libSource, IdealAirSystemAbridged hvac = default, bool lockedMode = false)
        {
            var sys = hvac ?? new IdealAirSystemAbridged($"IdealAirSystem_{Guid.NewGuid().ToString().Substring(0, 8)}");
            var vm = new IdealAirLoadViewModel(libSource, sys, this);

            //Padding = new Padding(4);
            Title = $"Ideal Air Load - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout() { DataContext = vm };
            layout.DefaultSpacing = new Size(4, 4);
            layout.Padding = new Padding(10);

           
            // string displayName = null, 
            // EconomizerType economizerType = EconomizerType.DifferentialDryBulb, 
            // bool demandControlledVentilation = false, 
            // double sensibleHeatRecovery = 0, 
            // double latentHeatRecovery = 0, 
            // double heatingAirTemperature = 50, 
            // double coolingAirTemperature = 13, 
            // AnyOf<Autosize, NoLimit, double> heatingLimit = null, 
            // AnyOf<Autosize, NoLimit, double> coolingLimit = null, 
            // string heatingAvailability = null, 
            // string coolingAvailability

            var nameText = new TextBox();
            var economizer = new DropDown();
            var DCV = new CheckBox() { Text = "Demand Controlled Ventilation" };

            var sensible = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };
            var latent = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };

            var heatingAirT = new NumericStepper() { MinValue = -50, MaxValue = 100, MaximumDecimalPlaces = 2 };
            var coolingAirT = new NumericStepper() { MinValue = -50, MaxValue = 100, MaximumDecimalPlaces = 2 };

            var heatingLimitAuto = new RadioButton() { Text = "Autosize" };
            var heatingLimitNoLimit = new RadioButton() { Text = "No Limit" };
            var heatingLimitNumber = new RadioButton();
            var heatingLimit = new NumericStepper();

            var coolingLimitAuto = new RadioButton() { Text = "Autosize" };
            var coolingLimitNoLimit = new RadioButton() { Text = "No Limit" };
            var coolingLimitNumber = new RadioButton();
            var coolingLimit = new NumericStepper();

            var heatingAvailability = new OptionalButton();
            heatingAvailability.TextBinding.Bind(vm, _ => _.HeatingAvaliabilitySchedule.BtnName);
            heatingAvailability.Bind(_ => _.Command, vm, _ => _.HeatingAvaliabilityCommand);
            heatingAvailability.Bind(_ => _.RemoveCommand, vm, _ => _.RemoveHeatingAvaliabilityCommand);
            heatingAvailability.Bind(_ => _.IsRemoveVisable, vm, _ => _.HeatingAvaliabilitySchedule.IsRemoveVisable);

            var coolingAvailability = new OptionalButton();
            coolingAvailability.TextBinding.Bind(vm, _ => _.CoolingAvaliabilitySchedule.BtnName);
            coolingAvailability.Bind(_ => _.Command, vm, _ => _.CoolingAvaliabilityCommand);
            coolingAvailability.Bind(_ => _.RemoveCommand, vm, _ => _.RemoveCoolingAvaliabilityCommand);
            coolingAvailability.Bind(_ => _.IsRemoveVisable, vm, _ => _.CoolingAvaliabilitySchedule.IsRemoveVisable);


            nameText.TextBinding.BindDataContext((IdealAirLoadViewModel m) => m.Name);

            economizer.BindDataContext(c => c.DataStore, (IdealAirLoadViewModel m) => m.Economizers);
            economizer.SelectedKeyBinding.BindDataContext((IdealAirLoadViewModel m) => m.Economizer);

            DCV.BindDataContext(c => c.Checked, (IdealAirLoadViewModel m) => m.DCV);

            sensible.BindDataContext(c => c.Value, (IdealAirLoadViewModel m) => m.SensibleHR);
            latent.BindDataContext(c => c.Value, (IdealAirLoadViewModel m) => m.LatentHR);

            heatingAirT.BindDataContext(c => c.Value, (IdealAirLoadViewModel m) => m.HeatingAirTemperature);
            coolingAirT.BindDataContext(c => c.Value, (IdealAirLoadViewModel m) => m.CoolingAirTemperature);

            heatingLimitAuto.BindDataContext(c => c.Checked, (IdealAirLoadViewModel m) => m.HeatingLimitAutosized);
            heatingLimitNoLimit.BindDataContext(c => c.Checked, (IdealAirLoadViewModel m) => m.HeatingLimitNoLimit);
            heatingLimitNumber.BindDataContext(c => c.Checked, (IdealAirLoadViewModel m) => m.HeatingLimitNumber);
            heatingLimit.BindDataContext(c => c.Value, (IdealAirLoadViewModel m) => m.HeatingLimit);
            heatingLimit.BindDataContext(c => c.Enabled, (IdealAirLoadViewModel m) => m.HeatingLimitNumber);

            coolingLimitAuto.BindDataContext(c => c.Checked, (IdealAirLoadViewModel m) => m.CoolingLimitAutosized);
            coolingLimitNoLimit.BindDataContext(c => c.Checked, (IdealAirLoadViewModel m) => m.CoolingLimitNoLimit);
            coolingLimitNumber.BindDataContext(c => c.Checked, (IdealAirLoadViewModel m) => m.CoolingLimitNumber);
            coolingLimit.BindDataContext(c => c.Value, (IdealAirLoadViewModel m) => m.CoolingLimit);
            coolingLimit.BindDataContext(c => c.Enabled, (IdealAirLoadViewModel m) => m.CoolingLimitNumber);

            layout.AddRow("Name:");
            layout.AddRow(nameText);
            layout.AddRow("Economizer:");
            layout.AddRow(economizer);
            layout.AddSeparateRow(DCV);
            layout.AddRow("Sensible Heat Recovery: [0-1]");
            layout.AddRow(sensible);
            layout.AddRow("Latent Heat Recovery: [0-1]");
            layout.AddRow(latent);
            layout.AddRow("Heating Supply Air Temperature: [C]");
            layout.AddRow(heatingAirT);
            layout.AddRow("Cooling Supply Air Temperature: [C]");
            layout.AddRow(coolingAirT);

            layout.AddRow("Heating Capacity Limit: [Watts]");
            layout.AddRow(heatingLimitAuto);
            layout.AddRow(heatingLimitNoLimit);
            layout.AddSeparateRow(heatingLimitNumber, heatingLimit);

            layout.AddRow("Cooling Capacity Limit: [Watts]");
            layout.AddRow(coolingLimitAuto);
            layout.AddRow(coolingLimitNoLimit);
            layout.AddSeparateRow(coolingLimitNumber, coolingLimit);

            layout.AddRow("Heating Availability");
            layout.AddRow(heatingAvailability);
            layout.AddRow("Cooling Availability");
            layout.AddRow(coolingAvailability);

            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OKButton = new Button { Text = "OK", Enabled = !lockedMode };
            OKButton.Click += (sender, e) => OkCommand.Execute(vm.GreateHvac(hvac));

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            var hbData = new Button { Text = "Schema Data" };
            hbData.Click += (sender, e) => Dialog_Message.Show(this, vm.GreateHvac(hvac).ToJson(true), "Schema Data");

            layout.AddSeparateRow(locked, null, OKButton, this.AbortButton, null, hbData);
            layout.AddRow(null);
            Content = layout;

        }


    }
}
