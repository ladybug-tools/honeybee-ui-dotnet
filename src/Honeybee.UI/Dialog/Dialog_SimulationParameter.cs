using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using HB = HoneybeeSchema;


namespace Honeybee.UI
{
    public class Dialog_SimulationParameter : Dialog<HB.SimulationParameter>
    {
        
        public Dialog_SimulationParameter(HB.SimulationParameter simulationParameter)
        {
            try
            {

                var param = simulationParameter;

                var layout = new DynamicLayout();
                //layout.DefaultSpacing = new Size(3, 15);
                layout.DefaultPadding = new Padding(5);

                Resizable = false;
                Title = $"Simulation Parameter - {DialogHelper.PluginName}";
                //WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 620);
                this.Icon = DialogHelper.HoneybeeIcon;
                

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e)
                    => Close(param);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();


                //NorthAngle
                var northNum = new NumericStepper() { };
                northNum.ValueBinding.Bind( Binding.Delegate(() => param.NorthAngle, v => param.NorthAngle = v));
                northNum.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationParameter), nameof(param.NorthAngle)));


                //TerrainType
                var terrainTypeDP = new EnumDropDown<HB.TerrianTypes>();
                //param.TerrainType = param.TerrainType;
                terrainTypeDP.SelectedValueBinding.Bind(Binding.Delegate(() => param.TerrainType, v => param.TerrainType = v));
                terrainTypeDP.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationParameter), nameof(param.TerrainType)));

                // RunPeriod
                var year = 2017;
                var runP = param.RunPeriod ?? new HB.RunPeriod() { StartDate = new List<int>() { 1, 1 }, EndDate = new List<int>() { 12, 31 } };
                var d1 = new DateTimePicker();
                d1.ValueBinding.Bind(
                    () => new DateTime(year, runP.StartDate[0], runP.StartDate[1]), 
                    (d) => param.RunPeriod.StartDate = new List<int>() { d.Value.Month, d.Value.Day });
                d1.Width = 100;
                d1.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.RunPeriod), nameof(runP.StartDate)));

                var d2 = new DateTimePicker();
                d2.ValueBinding.Bind(
                    () => new DateTime(year, runP.EndDate[0], runP.EndDate[1]),
                    (d) => param.RunPeriod.EndDate = new List<int>() { d.Value.Month, d.Value.Day });
                d2.Width = 100;
                d2.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.RunPeriod), nameof(runP.EndDate)));

                param.RunPeriod = runP;

                //var runPeriod = new TableLayout
                //{
                //    Spacing = new Size(5, 5),
                //    Rows = { new TableRow(new Label() { Text = "Run Period:", Width = 150 }, d1, new Label() { Text = "To:" }, d2, null) }
                //};
               



                // TimeSteps
                var timeSteps_NS = new NumericStepper();
                timeSteps_NS.DecimalPlaces = 0;
                timeSteps_NS.MinValue = 1;
                timeSteps_NS.ValueBinding.Bind(
                    () => param.Timestep,
                    (d) => param.Timestep = (int)d
                    );
                timeSteps_NS.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationParameter), nameof(param.Timestep)));


                //var sizingParam_GB = new GroupBox();
                //sizingParam_GB.Text = "Sizing Parameter";
                var topPanel = new DynamicLayout();
                topPanel.DefaultSpacing = new Size(5, 5);
                //topPanel.DefaultPadding = new Padding(5);
                //var subLayout = new DynamicLayout();
                //subLayout.Spacing = new Size(5, 5);
                topPanel.AddSeparateRow(new Label() { Text = "North:", Width = 150 }, northNum);
                topPanel.AddSeparateRow(new Label() { Text = "Terrain Type:", Width = 150 }, terrainTypeDP);
                topPanel.AddSeparateRow(new Label() { Text = "Run Period:", Width = 150 }, d1, new Label() { Text = "To:" }, d2, null);
                topPanel.AddSeparateRow(new Label() { Text = "Timesteps per Hour:", Width = 150 }, timeSteps_NS);
                layout.AddSeparateRow(topPanel);


                // SizingParameter
                param.SizingParameter = param.SizingParameter ?? new HB.SizingParameter();
                // CoolingFactor
                var cooling_NS = new NumericStepper();
                cooling_NS.DecimalPlaces = 2;
                cooling_NS.MinValue = 0;
                cooling_NS.ValueBinding.Bind(
                    () => param.SizingParameter.CoolingFactor,
                    (d) => param.SizingParameter.CoolingFactor = d
                    );
                cooling_NS.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SizingParameter), nameof(param.SizingParameter.CoolingFactor)));

                // HeatingFactor
                var heating_NS = new NumericStepper();
                heating_NS.DecimalPlaces = 2;
                heating_NS.MinValue = 0;
                heating_NS.ValueBinding.Bind(
                    () => param.SizingParameter.HeatingFactor,
                    (d) => param.SizingParameter.HeatingFactor = d
                    );
                heating_NS.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SizingParameter), nameof(param.SizingParameter.HeatingFactor)));

                var effStdDP = new EnumDropDown<HB.EfficiencyStandards>();
                effStdDP.SelectedValueBinding.Bind(Binding.Delegate(() => param.SizingParameter.EfficiencyStandard, v => param.SizingParameter.EfficiencyStandard = v));
                effStdDP.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SizingParameter), nameof(param.SizingParameter.EfficiencyStandard)));


                var bldnType = new TextBox();
                bldnType.TextBinding.Bind(Binding.Delegate(() => param.SizingParameter.BuildingType, v => param.SizingParameter.BuildingType = v));
                bldnType.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SizingParameter), nameof(param.SizingParameter.BuildingType)));

                var climateZone = new EnumDropDown<HB.ClimateZones>();
                climateZone.SelectedValueBinding.Bind(Binding.Delegate(() => param.SizingParameter.ClimateZone, v => param.SizingParameter.ClimateZone = v));
                climateZone.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SizingParameter), nameof(param.SizingParameter.CoolingFactor)));

                var sizing_GB = new GroupBox();
                sizing_GB.Font = new Font(sizing_GB.Font.Family, sizing_GB.Font.Size, FontStyle.Bold);
                sizing_GB.Text = "Sizing Parameter";

                var sizingLayout = new DynamicLayout();
                sizingLayout.Spacing = new Size(5, 5);
                sizingLayout.AddRow(new Label() { Text = "Cooling Sizing Factor:", Width = 145 }, cooling_NS);
                sizingLayout.AddRow("Heating Sizing Factor:", heating_NS);
                sizingLayout.AddRow("Efficiency Standards:", effStdDP);
                sizingLayout.AddRow("Bilding Type:", bldnType);
                sizingLayout.AddRow("Climate Zone:", climateZone);
                sizing_GB.Content = sizingLayout;
                layout.AddSeparateRow(sizing_GB);


                // ShadowCalculation
                var shadowCal = param.ShadowCalculation ?? new HB.ShadowCalculation();
                //shadowCal.CalculationFrequency = 1;
                var freq_NS = new NumericStepper();
                freq_NS.DecimalPlaces = 0;
                freq_NS.MinValue = 1;
                freq_NS.MaxValue = 60;

                freq_NS.ValueBinding.Bind(
                    () => shadowCal.CalculationFrequency,
                    (d) => shadowCal.CalculationFrequency = (int)d
                    );
                freq_NS.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.ShadowCalculation), nameof(shadowCal.CalculationFrequency)));

                //shadowCal.SolarDistribution
                var solarDist_DP = new EnumDropDown<HB.SolarDistribution>();
                solarDist_DP.SelectedValueBinding.Bind(() => shadowCal.SolarDistribution, v => shadowCal.SolarDistribution = v);
                solarDist_DP.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.ShadowCalculation), nameof(shadowCal.SolarDistribution)));

                //shadowCal.CalculationMethodEnum
                var CalMethod_DP = new EnumDropDown<HB.CalculationMethod>();
                CalMethod_DP.SelectedValueBinding.Bind(() => shadowCal.CalculationMethod, v => shadowCal.CalculationMethod = v);
                CalMethod_DP.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.ShadowCalculation), nameof(shadowCal.CalculationMethod)));

                // MaximumFigures
                var maxFigure_NS = new NumericStepper();
                maxFigure_NS.DecimalPlaces = 0;
                maxFigure_NS.ValueBinding.Bind(
                    () => shadowCal.MaximumFigures,
                    (d) => shadowCal.MaximumFigures = (int)d
                    );
                maxFigure_NS.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.ShadowCalculation), nameof(shadowCal.MaximumFigures)));



                param.ShadowCalculation = shadowCal;

               
                var shadowCal_GB = new GroupBox();
                shadowCal_GB.Font = new Font(shadowCal_GB.Font.Family, shadowCal_GB.Font.Size, FontStyle.Bold);
                shadowCal_GB.Text = "Shadow Calculation";
                var shadowCalLayout = new DynamicLayout();
                shadowCalLayout.Spacing = new Size(5, 5);
                shadowCalLayout.AddRow(new Label() { Text = "Calculation Frequency: ", Width = 145 }, freq_NS);
                shadowCalLayout.AddRow("Maximum Figures:", maxFigure_NS);
                shadowCalLayout.AddRow("Solar Distribution: ", solarDist_DP);
                shadowCalLayout.AddRow("Calculation Method: ", CalMethod_DP);
                shadowCal_GB.Content = shadowCalLayout;
                layout.AddSeparateRow(shadowCal_GB);



                // SimulationControl
                var simuCtrl = param.SimulationControl ?? new HB.SimulationControl();
                var plantSz_CB = new CheckBox();
                plantSz_CB.CheckedBinding.Bind(simuCtrl, v => v.DoPlantSizing);
                plantSz_CB.Text = "Do Plant Sizing";
                plantSz_CB.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationControl), nameof(simuCtrl.DoPlantSizing)));


                var zoneSz_CB = new CheckBox();
                zoneSz_CB.CheckedBinding.Bind(simuCtrl, v => v.DoZoneSizing);
                zoneSz_CB.Text = "Do Zone Sizing";
                zoneSz_CB.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationControl), nameof(simuCtrl.DoZoneSizing)));

                var sysSz_CB = new CheckBox();
                sysSz_CB.CheckedBinding.Bind(simuCtrl, v => v.DoSystemSizing);
                sysSz_CB.Text = "Do System Sizing";
                sysSz_CB.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationControl), nameof(simuCtrl.DoSystemSizing)));

                var runPeriods_CB = new CheckBox();
                runPeriods_CB.CheckedBinding.Bind(simuCtrl.RunForRunPeriods, v => v);
                runPeriods_CB.Text = "Run For Weather File Run Periods";
                runPeriods_CB.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationControl), nameof(simuCtrl.RunForRunPeriods)));

                var runForSizing_CB = new CheckBox();
                runForSizing_CB.CheckedBinding.Bind(simuCtrl, v => v.RunForSizingPeriods);
                runForSizing_CB.Text = "Run For Sizing Periods";
                runForSizing_CB.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationControl), nameof(simuCtrl.RunForSizingPeriods)));
                //var plantSz_TB = new ToggleButton();
                //plantSz_TB.TextBinding.Bind(() => simuCtrl.DoPlantSizing.ToString(), v => simuCtrl.DoPlantSizing = bool.Parse(v));
                //plantSz_TB.Bind(v => simuCtrl.DoPlantSizing, simuCtrl.DoPlantSizing, v => simuCtrl.DoPlantSizing);
                param.SimulationControl = simuCtrl;

                //hbModel.DisplayName = hbModel.DisplayName ?? string.Empty;
                //var modelNameTextBox = new TextBox() { };
                //modelNameTextBox.TextBinding.Bind(hbModel, m => m.DisplayName);
                var simulationControl_GB = new GroupBox();
                simulationControl_GB.Font = new Font(simulationControl_GB.Font.Family, simulationControl_GB.Font.Size, FontStyle.Bold);
                simulationControl_GB.Text = "Simulation Control";
                var simCtrlLayout = new DynamicLayout();
                simCtrlLayout.Spacing = new Size(5, 5);
                simCtrlLayout.AddRow(zoneSz_CB);
                simCtrlLayout.AddRow(plantSz_CB);
                simCtrlLayout.AddRow(sysSz_CB);
                simCtrlLayout.AddRow(runPeriods_CB);
                simCtrlLayout.AddRow(runForSizing_CB);
                simulationControl_GB.Content = simCtrlLayout;
                layout.AddSeparateRow(simulationControl_GB);


                // SimulationOutput
                param.Output = param.Output ?? new HB.SimulationOutput();

                var simOutput_GB = new GroupBox();
                simOutput_GB.Font = new Font(simOutput_GB.Font.Family, simOutput_GB.Font.Size, FontStyle.Bold);
                simOutput_GB.Text = "Simulation Output";
                var simuOutputLayout = new DynamicLayout();
                simuOutputLayout.Spacing = new Size(5, 5);
                //simuOutputLayout.AddRow("EnergyPlus Output Names:");
                var outputNamesBtn = new Button { Text = "EnergyPlus Output/Summary Report" };
                outputNamesBtn.Click += (s, e) =>
                {
                    var simuOutput = param.Output ?? new HB.SimulationOutput();
                    simuOutput = simuOutput.DuplicateSimulationOutput();
                    var dialog = new UI.Dialog_EPOutputs(simuOutput);
                    var outputRs = dialog.ShowModal(Config.Owner);
                    if (outputRs != null)
                        param.Output = outputRs;
                };
                outputNamesBtn.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationParameter), nameof(param.Output)));
                simuOutputLayout.AddRow(outputNamesBtn);
                //simuOutputLayout.AddRow(outputListBox);
                //simuOutputLayout.AddRow("EnergyPlus Summary Report:");
                //simuOutputLayout.AddRow(sumReportsListBox);
                simOutput_GB.Content = simuOutputLayout;
                layout.AddSeparateRow(simOutput_GB);


                layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
                layout.AddRow(null);

                Content = layout;
               
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
                //Rhino.RhinoApp.WriteLine(e.Message);
            }


        }

    
    }
}
