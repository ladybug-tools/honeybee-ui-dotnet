using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Honeybee.UI
{
    public class Dialog_IdealAirLoad : Dialog<HoneybeeSchema.IdealAirSystemAbridged>
    {
        //private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_IdealAirLoad(IdealAirSystemAbridged hvac = default)
        {
            var sys = hvac ?? new IdealAirSystemAbridged($"IdealAirSystem_{Guid.NewGuid().ToString().Substring(0, 8)}");
            var vm = new IdealAirLoadViewModel(sys);

            Padding = new Padding(5);
            Title = "Ideal Air Load - Honeybee";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout() { DataContext = vm };
            layout.DefaultSpacing = new Size(8, 8);
            layout.Padding = new Padding(15);

           
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

            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => {
                var obj = vm.GreateHvac(hvac);
                Close(obj);
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            layout.AddSeparateRow(null, OKButton, this.AbortButton, null);
            layout.AddRow(null);
            Content = layout;


        }
        
     

    }
}
