using Eto.Drawing;
using Eto.Forms;
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
                layout.DefaultPadding = new Padding(10);
                //layout.BeginScrollable();


                Padding = new Padding(5);
                Resizable = true;
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
                

                //TerrainType
                var terrainTypeDP = new EnumDropDown<HB.TerrianTypes>();
                param.TerrainType = param.TerrainType;
                terrainTypeDP.SelectedValueBinding.Bind(Binding.Delegate(() => param.TerrainType, v => param.TerrainType = v));
                

                // RunPeriod
                var year = 2017;
                var runP = param.RunPeriod ?? new HB.RunPeriod() { StartDate = new List<int>() { 1, 1 }, EndDate = new List<int>() { 12, 31 } };
                var d1 = new DateTimePicker();
                d1.ValueBinding.Bind(
                    () => new DateTime(year, runP.StartDate[0], runP.StartDate[1]), 
                    (d) => param.RunPeriod.StartDate = new List<int>() { d.Value.Month, d.Value.Day });
                d1.Width = 100;
               
                var d2 = new DateTimePicker();
                d2.ValueBinding.Bind(
                    () => new DateTime(year, runP.EndDate[0], runP.EndDate[1]),
                    (d) => param.RunPeriod.EndDate = new List<int>() { d.Value.Month, d.Value.Day });
                d2.Width = 100;

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

                // HeatingFactor
                var heating_NS = new NumericStepper();
                heating_NS.DecimalPlaces = 2;
                heating_NS.MinValue = 0;
                heating_NS.ValueBinding.Bind(
                    () => param.SizingParameter.HeatingFactor,
                    (d) => param.SizingParameter.HeatingFactor = d
                    );

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
                topPanel.AddSeparateRow(new Label() { Text = "Cooling Sizing Factor:", Width = 150 }, cooling_NS);
                topPanel.AddSeparateRow(new Label() { Text = "Heating Sizing Factor:", Width = 150 }, heating_NS);
                layout.AddSeparateRow(topPanel);
                //sizingParam_GB.Content = sizingParamLayout;
                //layout.AddRow(subLayout);


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

                //shadowCal.SolarDistribution
                shadowCal.SolarDistribution = shadowCal.SolarDistribution;
                var solarDist_DP = new EnumDropDown<HB.SolarDistribution>();
                solarDist_DP.SelectedValueBinding.Bind(() => shadowCal.SolarDistribution, v => shadowCal.SolarDistribution = v);

                //shadowCal.CalculationMethodEnum
                shadowCal.CalculationMethod = shadowCal.CalculationMethod;
                var CalMethod_DP = new EnumDropDown<HB.CalculationMethod>();
                CalMethod_DP.SelectedValueBinding.Bind(() => shadowCal.CalculationMethod, v => shadowCal.CalculationMethod = v);

                // MaximumFigures
                var maxFigure_NS = new NumericStepper();
                maxFigure_NS.DecimalPlaces = 0;
                maxFigure_NS.ValueBinding.Bind(
                    () => shadowCal.MaximumFigures,
                    (d) => shadowCal.MaximumFigures = (int)d
                    );


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
         
                var zoneSz_CB = new CheckBox();
                zoneSz_CB.CheckedBinding.Bind(simuCtrl, v => v.DoZoneSizing);
                zoneSz_CB.Text = "Do Zone Sizing";

                var sysSz_CB = new CheckBox();
                sysSz_CB.CheckedBinding.Bind(simuCtrl, v => v.DoSystemSizing);
                sysSz_CB.Text = "Do System Sizing";

                var runPeriods_CB = new CheckBox();
                runPeriods_CB.CheckedBinding.Bind(simuCtrl.RunForRunPeriods, v => v);
                runPeriods_CB.Text = "Run For Weather File Run Periods";

                var runForSizing_CB = new CheckBox();
                runForSizing_CB.CheckedBinding.Bind(simuCtrl, v => v.RunForSizingPeriods);
                runForSizing_CB.Text = "Run For Sizing Periods";
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
                    var outputRs = OutputNamesBtn_Click(param.Output);
                    if (outputRs != null)
                        param.Output.Outputs = outputRs.Outputs;
                };
                simuOutputLayout.AddRow(outputNamesBtn);
                //simuOutputLayout.AddRow(outputListBox);
                //simuOutputLayout.AddRow("EnergyPlus Summary Report:");
                //simuOutputLayout.AddRow(sumReportsListBox);
                simOutput_GB.Content = simuOutputLayout;
                layout.AddSeparateRow(simOutput_GB);


                layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
                layout.AddRow(null);
                //layout.EndScrollable();
         
                //Content = layout;

                //var g = new GroupBox();
                //g.Content = layout;
                Content = layout;
               
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
                //Rhino.RhinoApp.WriteLine(e.Message);
            }

            HB.SimulationOutput OutputNamesBtn_Click(HB.SimulationOutput simulationOutput)
            {
                var simuOutput = simulationOutput ?? new HB.SimulationOutput();
                simuOutput = simulationOutput.DuplicateSimulationOutput();
                var dialog = new UI.Dialog_EPOutputs(simuOutput);
                var dialog_rc = dialog.ShowModal(Config.Owner);
         
                return dialog_rc;
               
            }

        }

    
    }
}
