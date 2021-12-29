using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;

namespace Honeybee.UI
{

    public class Dialog_ProgramType: Dialog_ResourceEditor<HB.ProgramTypeAbridged>
    {
        private ProgramTypeViewModel _vm;

     
        public Dialog_ProgramType(ref HB.ModelProperties libSource, HB.ProgramTypeAbridged programType, bool lockedMode = false)
        {
            try
            {
                libSource.FillNulls();

                _vm = new ProgramTypeViewModel(this, ref libSource, programType);

                Title = $"Program Type - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                Width = 450;
                this.Icon = DialogHelper.HoneybeeIcon;


                //Generate ProgramType Panel

                var nameTbx = new TextBox();
                nameTbx.TextBinding.Bind(_vm, c => c.Name);

                var loadGroup = GenLoadsPanel(lockedMode);


                var locked = new CheckBox() { Text = "Locked", Enabled = false };
                locked.Checked = lockedMode;

                var OkButton = new Button { Text = "OK", Enabled = !lockedMode };
                OkButton.Click += (sender, e) => {
                    try
                    {
                        OkCommand.Execute(_vm.GetHBObject());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.Message);
                        //throw;
                    }

                }; 

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                // Json Data
                var hbData = new Button { Text = "Schema Data" };
                hbData.Command = _vm.HBDataBtnClick;

                //Create layout
                var layout = new DynamicLayout() { DefaultPadding = new Padding(5), DefaultSpacing = new Size(5, 5) };
                layout.BeginScrollable(BorderType.None);

                layout.AddRow("Name");
                layout.AddRow(nameTbx);
                layout.AddRow(loadGroup);
                layout.AddRow(null);
                layout.EndScrollable();
                layout.AddSeparateRow(locked, null, OkButton, AbortButton, null, hbData);
                layout.Add(null);
                //Create layout
                Content = layout;

            }
            catch (Exception e)
            {
                //throw e;
                Dialog_Message.Show(e.ToString());
            }


        }


        private DynamicLayout GenLoadsPanel(bool lockedMode)
        {

            var layout = new DynamicLayout() { Height = 500 };

        
            //layout.DefaultPadding = new Padding(4);
            layout.BeginScrollable(BorderType.None);

            var container = new DynamicLayout() { Enabled = !lockedMode };
            container.DefaultSpacing = new Size(4, 4);

            //Get lighting
            container.AddRow(GenLightingPanel());

            //Get People
            container.AddRow(GenPplPanel());

            //Get ElecEquipment
            container.AddRow(GenElecEqpPanel());


            //Get gas Equipment
            container.AddRow(GenGasEqpPanel());

            //Get Ventilation
            container.AddRow(GenVentPanel());

            //Get Infiltration
            container.AddRow(GenInfPanel());

            //Get Setpoint
            container.AddRow(GenStpPanel());

            //Get ServiceHotWater
            container.AddRow(GenSHWPanel());

            layout.AddRow(container);
            layout.AddRow(null);

            layout.EndScrollable();

            return layout;
        }

        private GroupBox GenLightingPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.Lighting.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.Lighting.IsPanelEnabled);

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.Lighting.Default.WattsPerArea);
            wPerArea.TextBinding.Bind(vm, _ => _.Lighting.WattsPerArea.NumberText);
            layout.AddRow("Watts/m2:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Lighting.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Lighting.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.Lighting.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.Lighting.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.Lighting.Default.VisibleFraction);
            visFraction.TextBinding.Bind(vm, _ => _.Lighting.VisibleFraction.NumberText);
            layout.AddRow("Visible Fraction:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.SetDefault(_vm.Lighting.Default.ReturnAirFraction);
            airFraction.TextBinding.Bind(vm, _ => _.Lighting.ReturnAirFraction.NumberText);
            layout.AddRow("Return Air Fraction:");
            layout.AddRow(airFraction);

            var baseline = new DoubleText();
            baseline.ReservedText = _vm.Varies;
            baseline.SetDefault(_vm.Lighting.Default.BaselineWattsPerArea);
            baseline.TextBinding.Bind(vm, _ => _.Lighting.BaselineWattsPerArea.NumberText);
            layout.AddRow("Baseline Watts/m2:");
            layout.AddRow(baseline);
            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.Lighting.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Lighting" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenElecEqpPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.ElecEquipment.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.ElecEquipment.IsPanelEnabled);

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.ElecEquipment.Default.WattsPerArea);
            wPerArea.TextBinding.Bind(vm, _ => _.ElecEquipment.WattsPerArea.NumberText);
            layout.AddRow("Watts/m2:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.ElecEquipment.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.ElecEquipment.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.ElecEquipment.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.ElecEquipment.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.LatentFraction.NumberText);
            layout.AddRow("Latent Fraction:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.SetDefault(_vm.ElecEquipment.Default.LostFraction);
            airFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.LostFraction.NumberText);
            layout.AddRow("Lost Fraction:");
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.ElecEquipment.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Electric Equipment" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenGasEqpPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.Gas.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.Gas.IsPanelEnabled);


            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.Gas.Default.WattsPerArea);
            wPerArea.TextBinding.Bind(vm, _ => _.Gas.WattsPerArea.NumberText);
            layout.AddRow("Watts/m2:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Gas.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Gas.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.Gas.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.Gas.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.Gas.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.Gas.LatentFraction.NumberText);
            layout.AddRow("Latent Fraction:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.SetDefault(_vm.Gas.Default.LostFraction);
            airFraction.TextBinding.Bind(vm, _ => _.Gas.LostFraction.NumberText);
            layout.AddRow("Lost Fraction:");
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.Gas.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Gas Equipment" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenPplPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.People.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.People.IsPanelEnabled);

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.People.Default.PeoplePerArea);
            wPerArea.TextBinding.Bind(vm, _ => _.People.PeoplePerArea.NumberText);
            layout.AddRow("People/m2:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.People.OccupancySchedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.People.ScheduleCommand);
            layout.AddRow("Occupancy Schedule:");
            layout.AddRow(sch);

            var sch2 = new Button();
            sch2.TextBinding.Bind(vm, _ => _.People.ActivitySchedule.BtnName);
            sch2.Bind(_ => _.Command, vm, _ => _.People.ActivityScheduleCommand);
            layout.AddRow("Activity Schedule:");
            layout.AddRow(sch2);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.People.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.People.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.People.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.People.LatentFraction.NumberText);
            visFraction.Bind(_ => _.Enabled, vm, _ => _.People.IsLatenFractionInputEnabled);
            var autosize = new CheckBox() { Text = "Autocalculate" };
            autosize.Bind(_ => _.Checked, vm, _ => _.People.IsLatentFractionAutocalculate);
            layout.AddRow("Latent Fraction:");
            layout.AddRow(autosize);
            layout.AddRow(visFraction);

            layout.AddRow(null);



            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.People.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "People" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenInfPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.Infiltration.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.Infiltration.IsPanelEnabled);

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.Infiltration.Default.FlowPerExteriorArea);
            wPerArea.TextBinding.Bind(vm, _ => _.Infiltration.FlowPerExteriorArea.NumberText);
            layout.AddRow("Flow/m2 (exterior area):");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Infiltration.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Infiltration.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.Infiltration.Default.ConstantCoefficient);
            radFraction.TextBinding.Bind(vm, _ => _.Infiltration.ConstantCoefficient.NumberText);
            layout.AddRow("Constant Coefficient:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.Infiltration.Default.TemperatureCoefficient);
            visFraction.TextBinding.Bind(vm, _ => _.Infiltration.TemperatureCoefficient.NumberText);
            layout.AddRow("Temperature Coefficient:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.SetDefault(_vm.Infiltration.Default.VelocityCoefficient);
            airFraction.TextBinding.Bind(vm, _ => _.Infiltration.VelocityCoefficient.NumberText);
            layout.AddRow("Velocity Coefficient:");
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.Infiltration.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Infiltration" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenVentPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.Ventilation.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.Ventilation.IsPanelEnabled);


            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.Ventilation.Default.FlowPerArea);
            wPerArea.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerArea.NumberText);
            layout.AddRow("Flow/m2:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Ventilation.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Ventilation.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.Ventilation.Default.FlowPerPerson);
            radFraction.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerPerson.NumberText);
            layout.AddRow("Flow/Person:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.Ventilation.Default.FlowPerZone);
            visFraction.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerZone.NumberText);
            layout.AddRow("Flow/Zone:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.SetDefault(_vm.Ventilation.Default.AirChangesPerHour);
            airFraction.TextBinding.Bind(vm, _ => _.Ventilation.AirChangesPerHour.NumberText);
            layout.AddRow("AirChanges/Hour:");
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.Ventilation.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Ventilation" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenStpPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.Setpoint.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.Setpoint.IsPanelEnabled);


            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);


            var sch = new Button();
            sch.Width = 250;
            sch.TextBinding.Bind(vm, _ => _.Setpoint.CoolingSchedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Setpoint.CoolingScheduleCommand);
            layout.AddRow("Cooling Schedule:");
            layout.AddRow(sch);

            var sch2 = new Button();
            sch2.TextBinding.Bind(vm, _ => _.Setpoint.HeatingSchedule.BtnName);
            sch2.Bind(_ => _.Command, vm, _ => _.Setpoint.HeatingScheduleCommand);
            layout.AddRow("Heating Schedule:");
            layout.AddRow(sch2);

            var sch3 = new Button();
            sch3.TextBinding.Bind(vm, _ => _.Setpoint.HumidifyingSchedule.BtnName);
            sch3.Bind(_ => _.Command, vm, _ => _.Setpoint.HumidifyingScheduleCommand);
            layout.AddRow("Humidifying Schedule:");
            layout.AddRow(sch3);

            var sch4 = new Button();
            sch4.TextBinding.Bind(vm, _ => _.Setpoint.DehumidifyingSchedule.BtnName);
            sch4.Bind(_ => _.Command, vm, _ => _.Setpoint.DehumidifyingScheduleCommand);
            layout.AddRow("Dehumidifying Schedule:");
            layout.AddRow(sch4);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.Setpoint.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Setpoint" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenSHWPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.ServiceHotWater.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.ServiceHotWater.IsPanelEnabled);


            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.ServiceHotWater.Default.FlowPerArea);
            wPerArea.TextBinding.Bind(vm, _ => _.ServiceHotWater.FlowPerArea.NumberText);
            layout.AddRow("Flow/m2:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.ServiceHotWater.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.ServiceHotWater.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.SetDefault(_vm.ServiceHotWater.Default.TargetTemperature);
            airFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.TargetTemperature.NumberText);
            layout.AddRow("Target Temperature:");
            layout.AddRow(airFraction);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.ServiceHotWater.Default.SensibleFraction);
            radFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.SensibleFraction.NumberText);
            layout.AddRow("Sensible Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.ServiceHotWater.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.LatentFraction.NumberText);
            layout.AddRow("Latent Fraction:");
            layout.AddRow(visFraction);


            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.ServiceHotWater.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Service Hot Water" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }


    


    }
}
