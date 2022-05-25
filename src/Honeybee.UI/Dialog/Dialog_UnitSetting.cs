using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System;

namespace Honeybee.UI
{
    public class Dialog_UnitSetting : Dialog
    {
        public Dialog_UnitSetting(Dictionary<Units.UnitType, Enum> hardcodedUnits = default)
        {
            var vm = new UnitSettingViewModel(hardcodedUnits);

            Padding = new Padding(5);
            Title = "Display Unit Setting";
            WindowStyle = WindowStyle.Default;
            Width = 420;
            this.Icon = Honeybee.UI.DialogHelper.HoneybeeIcon;
            var layout = new DynamicLayout();

            this.DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => 
            { 
                vm.ApplySetting(); 
                Close(); 
            };


            this.AbortButton = new Button { Text = "Close" };
            AbortButton.Click += (sender, e) => Close();


            // all controls
            var allToggle = new Button() { Text = "Presets", Width = 50 };
            allToggle.Command = vm.PresetCommand;
            //layout.AddRow(allToggle);

            // General
            //layout.BeginGroup("General");

            var length = new EnumDropDown<Units.LengthUnit>() { Width = 170 };
            length.SelectedValueBinding.Bind(vm, m => m.Length);
            length.Bind(_=>_.Enabled, vm, m => m.LengthEnabled);
            var lengthAbbr = new Label();
            lengthAbbr.TextBinding.Bind(vm, m => m.LengthAbbv);
            layout.AddSeparateRow("Length:", lengthAbbr, null, length);

            var area = new EnumDropDown<Units.AreaUnit>() { Width = 170 };
            area.SelectedValueBinding.Bind(vm, m => m.Area);
            area.Bind(_ => _.Enabled, vm, m => m.AreaEnabled);
            var areaAbbr = new Label();
            areaAbbr.TextBinding.Bind(vm, m => m.AreaAbbv);
            layout.AddSeparateRow("Area:", areaAbbr, null, area);
            
            var volume = new EnumDropDown<Units.VolumeUnit>() { Width = 170 };
            volume.SelectedValueBinding.Bind(vm, m => m.Volume);
            volume.Bind(_ => _.Enabled, vm, m => m.VolumeEnabled);
            var volumeAbbr = new Label();
            volumeAbbr.TextBinding.Bind(vm, m => m.VolumeAbbv);
            layout.AddSeparateRow("Volume:", volumeAbbr, null, volume);

            var temperature = new EnumDropDown<Units.TemperatureUnit>() { Width = 170 };
            temperature.SelectedValueBinding.Bind(vm, m => m.Temperature);
            temperature.Bind(_ => _.Enabled, vm, m => m.TemperatureEnabled);
            var temperatureAbbr = new Label();
            temperatureAbbr.TextBinding.Bind(vm, m => m.TemperatureAbbv);
            layout.AddSeparateRow("Temperature:", temperatureAbbr, null, temperature);

            var temperatureDelta = new EnumDropDown<Units.TemperatureDeltaUnit>() { Width = 170 };
            temperatureDelta.SelectedValueBinding.Bind(vm, m => m.TemperatureDelta);
            temperatureDelta.Bind(_ => _.Enabled, vm, m => m.TemperatureDeltaEnabled);
            var temperatureDeltaAbbr = new Label();
            temperatureDeltaAbbr.TextBinding.Bind(vm, m => m.TemperatureDeltaAbbv);
            layout.AddSeparateRow("Temperature Delta:", temperatureDeltaAbbr, null, temperatureDelta);

            var power = new EnumDropDown<Units.PowerUnit>() { Width = 170 };
            power.SelectedValueBinding.Bind(vm, m => m.Power);
            power.Bind(_ => _.Enabled, vm, m => m.PowerEnabled);
            var powerAbbr = new Label();
            powerAbbr.TextBinding.Bind(vm, m => m.PowerAbbv);
            layout.AddSeparateRow("Power:", powerAbbr, null, power);

            var energy = new EnumDropDown<Units.EnergyUnit>() { Width = 170 };
            energy.SelectedValueBinding.Bind(vm, m => m.Energy);
            energy.Bind(_ => _.Enabled, vm, m => m.EnergyEnabled);
            var energyAbbr = new Label();
            energyAbbr.TextBinding.Bind(vm, m => m.EnergyAbbv);
            layout.AddSeparateRow("Energy:", energyAbbr, null, energy);

            var powerDensity = new EnumDropDown<Units.HeatFluxUnit>() { Width = 170 };
            powerDensity.SelectedValueBinding.Bind(vm, m => m.PowerDensity);
            powerDensity.Bind(_ => _.Enabled, vm, m => m.PowerDensityEnabled);
            var powerDensityAbbr = new Label();
            powerDensityAbbr.TextBinding.Bind(vm, m => m.PowerDensityAbbv);
            layout.AddSeparateRow("Power Density:", powerDensityAbbr, null, powerDensity);


            var PeopleDensity = new EnumDropDown<Units.ReciprocalAreaUnit>() { Width = 170 };
            PeopleDensity.SelectedValueBinding.Bind(vm, m => m.PeopleDensity);
            PeopleDensity.Bind(_ => _.Enabled, vm, m => m.PeopleDensityEnabled);
            var PeopleDensityAbbr = new Label();
            PeopleDensityAbbr.TextBinding.Bind(vm, m => m.PeopleDensityAbbv);
            layout.AddSeparateRow("People Density:", PeopleDensityAbbr, null, PeopleDensity);


            var AirFlowRate = new EnumDropDown<Units.VolumeFlowUnit>() { Width = 170 };
            AirFlowRate.SelectedValueBinding.Bind(vm, m => m.AirFlowRate);
            AirFlowRate.Bind(_ => _.Enabled, vm, m => m.AirFlowRateEnabled);
            var AirFlowRateAbbr = new Label();
            AirFlowRateAbbr.TextBinding.Bind(vm, m => m.AirFlowRateAbbv);
            layout.AddSeparateRow("Flow Rate:", AirFlowRateAbbr, null, AirFlowRate);

            var AirFlowRateArea = new EnumDropDown<Units.VolumeFlowPerAreaUnit>() { Width = 170 };
            AirFlowRateArea.SelectedValueBinding.Bind(vm, m => m.AirFlowRateArea);
            AirFlowRateArea.Bind(_ => _.Enabled, vm, m => m.AirFlowRateAreaEnabled);
            var AirFlowRateAreaAbbr = new Label();
            AirFlowRateAreaAbbr.TextBinding.Bind(vm, m => m.AirFlowRateAreaAbbv);
            layout.AddSeparateRow("Flow Rate/Area:", AirFlowRateAreaAbbr, null, AirFlowRateArea);

            var Speed = new EnumDropDown<Units.SpeedUnit>() { Width = 170 };
            Speed.SelectedValueBinding.Bind(vm, m => m.Speed);
            Speed.Bind(_ => _.Enabled, vm, m => m.SpeedEnabled);
            var SpeedAbbr = new Label();
            SpeedAbbr.TextBinding.Bind(vm, m => m.SpeedAbbv);
            layout.AddSeparateRow("Speed:", SpeedAbbr, null, Speed);

            var Illuminance = new EnumDropDown<Units.IlluminanceUnit>() { Width = 170 };
            Illuminance.SelectedValueBinding.Bind(vm, m => m.Illuminance);
            Illuminance.Bind(_ => _.Enabled, vm, m => m.IlluminanceEnabled);
            var IlluminanceAbbr = new Label();
            IlluminanceAbbr.TextBinding.Bind(vm, m => m.IlluminanceAbbv);
            layout.AddSeparateRow("Illuminance:", IlluminanceAbbr, null, Illuminance);

            var Conductivity = new EnumDropDown<Units.ThermalConductivityUnit>() { Width = 170 };
            Conductivity.SelectedValueBinding.Bind(vm, m => m.Conductivity);
            Conductivity.Bind(_ => _.Enabled, vm, m => m.ConductivityEnabled);
            var ConductivityAbbr = new Label();
            ConductivityAbbr.TextBinding.Bind(vm, m => m.ConductivityAbbv);
            layout.AddSeparateRow("Conductivity:", ConductivityAbbr, null, Conductivity);

            var Resistance = new EnumDropDown<Units.ThermalResistanceUnit>() { Width = 170 };
            Resistance.SelectedValueBinding.Bind(vm, m => m.Resistance);
            Resistance.Bind(_ => _.Enabled, vm, m => m.ResistanceEnabled);
            var ResistanceAbbr = new Label();
            ResistanceAbbr.TextBinding.Bind(vm, m => m.ResistanceAbbv);
            layout.AddSeparateRow("Resistance:", ResistanceAbbr, null, Resistance);

            var UValue = new EnumDropDown<Units.HeatTransferCoefficientUnit>() { Width = 170 };
            UValue.SelectedValueBinding.Bind(vm, m => m.UValue);
            UValue.Bind(_ => _.Enabled, vm, m => m.UValueEnabled);
            var UValueAbbr = new Label();
            UValueAbbr.TextBinding.Bind(vm, m => m.UValueAbbv);
            layout.AddSeparateRow("UValue:", UValueAbbr, null, UValue);

            var Density = new EnumDropDown<Units.DensityUnit>() { Width = 170 };
            Density.SelectedValueBinding.Bind(vm, m => m.Density);
            Density.Bind(_ => _.Enabled, vm, m => m.DensityEnabled);
            var DensityAbbr = new Label();
            DensityAbbr.TextBinding.Bind(vm, m => m.DensityAbbv);
            layout.AddSeparateRow("Density:", DensityAbbr, null, Density);

            var SpecificHeat = new EnumDropDown<Units.SpecificEntropyUnit>() { Width = 170 };
            SpecificHeat.SelectedValueBinding.Bind(vm, m => m.SpecificHeat);
            SpecificHeat.Bind(_ => _.Enabled, vm, m => m.SpecificHeatEnabled);
            var SpecificHeatAbbr = new Label();
            SpecificHeatAbbr.TextBinding.Bind(vm, m => m.SpecificHeatAbbv);
            layout.AddSeparateRow("SpecificHeat:", SpecificHeatAbbr, null, SpecificHeat);

            //layout.EndGroup();
            //layout.DefaultPadding = new Padding(10);
            layout.DefaultSpacing = new Size(4, 4);
            layout.Padding = new Padding(8);

            //layout.AddRow("Note: units here are only for display purpose and any changes won't affect any under-hood simulation engines' units.");

            layout.AddSeparateRow(allToggle, null, DefaultButton, AbortButton, null);
            layout.AddRow(null);

            Content = layout;

        }



    }

}
