﻿using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HB = HoneybeeSchema;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{

    public class RoomProperty : Panel
    {
        private RoomPropertyViewModel _vm { get; set; }
        private static RoomProperty _instance;
        public static RoomProperty Instance
        {
            get
            {
                _instance = _instance ?? new RoomProperty();
                return _instance;
            }
        }

        public RoomProperty()
        {
            this._vm = new RoomPropertyViewModel(this);
            Initialize();
        }


        public void UpdatePanel(HB.ModelProperties libSource, List<HB.Room> HoneybeeObjs)
        {
            this._vm.Update(libSource, HoneybeeObjs);
        }

        public List<HB.Room> GetRooms()
        {
            return this._vm.GetRooms();
           
        }

        private void Initialize()
        {
            var vm = this._vm;
            var layout = new DynamicLayout();
            layout.Width = 400;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var tb = new TabControl();
            
            var basis = GenBasisPanel();
            tb.Pages.Add(new TabPage(basis) { Text = "Basis" });

            var loads = GenLoadsPanel();
            tb.Pages.Add(new TabPage(loads) { Text= "Room Loads"});

            var ctrls = GenControlPanel();
            tb.Pages.Add(new TabPage(ctrls) { Text = "Room Controls" });

            layout.AddRow(tb);

            layout.AddRow(null);
            var data_button = new Button { Text = "Schema Data" };
            data_button.Command = this._vm.HBDataBtnClick;
            layout.AddSeparateRow(data_button, null);


            this.Content = layout;
        }

        private DynamicLayout GenBasisPanel()
        {
            var layout = new DynamicLayout() { Height = 350 };
            var vm = this._vm;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var id = new Label();
            id.TextBinding.Bind(vm, (_) => _.Identifier);
            layout.AddRow("ID: ", id);

            var nameTB = new TextBox() { };
            nameTB.TextBinding.Bind(vm, (_) => _.DisplayName);
            layout.AddRow("Name:", nameTB);

            var storyTB = new TextBox() { };
            storyTB.TextBinding.Bind(vm, (_) => _.Story);
            layout.AddRow("Story:", storyTB);

            var multiplier_NS = new IntText();
            multiplier_NS.ReservedText = _vm.Varies;
            multiplier_NS.TextBinding.Bind(vm, _ => _.MultiplierText);
            layout.AddRow("Multiplier:", multiplier_NS);

            layout.AddSpace();

            //Get modifierSet
            var modifierSetDP = new Button();
            modifierSetDP.Bind((t) => t.Enabled, vm, v => v.ModifierSet.IsBtnEnabled);
            modifierSetDP.TextBinding.Bind(vm, _ => _.ModifierSet.BtnName);
            modifierSetDP.Command = vm.ModifierSetCommand;
            var modifierSetByProgram = new CheckBox() { Text = vm.ByGlobalModifierSet };
            modifierSetByProgram.CheckedBinding.Bind(vm, _ => _.ModifierSet.IsCheckboxChecked);

            layout.AddRow("Modifier Set:", modifierSetByProgram);
            layout.AddRow(null, modifierSetDP);

            layout.AddSpace();

            //Get constructions
            var constructionSetDP = new Button();
            constructionSetDP.Bind(_ => _.Enabled, vm, _ => _.ConstructionSet.IsBtnEnabled);
            constructionSetDP.TextBinding.Bind(vm, _ => _.ConstructionSet.BtnName);
            constructionSetDP.Command = vm.RoomConstructionSetCommand;
            var cSetByProgram = new CheckBox() { Text = vm.ByGlobalConstructionSet };
            cSetByProgram.CheckedBinding.Bind(vm, _ => _.ConstructionSet.IsCheckboxChecked);

            layout.AddRow("Construction Set:", cSetByProgram);
            layout.AddRow(null, constructionSetDP);


            //Get programs
            var programTypesDP = new Button();
            programTypesDP.Bind((t) => t.Enabled, vm, v => v.PropgramType.IsBtnEnabled);
            programTypesDP.TextBinding.Bind(vm, _ => _.PropgramType.BtnName);
            programTypesDP.Command = vm.RoomProgramTypeCommand;
            var pTypeByProgram = new CheckBox() { Text = vm.Unoccupied };
            pTypeByProgram.CheckedBinding.Bind(vm, _ => _.PropgramType.IsCheckboxChecked);

            layout.AddRow("Program Type:", pTypeByProgram);
            layout.AddRow(null, programTypesDP);


            //Get HVACs
            var hvacDP = new Button();
            hvacDP.Bind((t) => t.Enabled, vm, v => v.HVAC.IsBtnEnabled);
            hvacDP.TextBinding.Bind(vm, _ => _.HVAC.BtnName);
            hvacDP.Command = vm.RoomHVACCommand;
            var hvacByProgram = new CheckBox() { Text = vm.Unconditioned };
            hvacByProgram.CheckedBinding.Bind(vm, _ => _.HVAC.IsCheckboxChecked);

            layout.AddRow("HVAC:", hvacByProgram);
            layout.AddRow(null, hvacDP);

            layout.AddRow(null);
            return layout;
        }

        private DynamicLayout GenLoadsPanel()
        {
            var layout = new DynamicLayout() { Height = 350 };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);
            layout.BeginScrollable( BorderType.None);

            //Get lighting
            layout.AddRow(GenLightingPanel());

            //Get People
            layout.AddRow(GenPplPanel());

            //Get ElecEquipment
            layout.AddRow(GenElecEqpPanel());


            //Get gas Equipment
            layout.AddRow(GenGasEqpPanel());

            //Get Ventilation
            layout.AddRow(GenVentPanel());

            //Get Infiltration
            layout.AddRow(GenInfPanel());
            
            //Get Setpoint
            layout.AddRow(GenStpPanel());

            //Get ServiceHotWater
            layout.AddRow(GenSHWPanel());

            layout.AddRow(null);

            layout.EndScrollable();
            return layout;
        }

        private DynamicLayout GenControlPanel()
        {
            var layout = new DynamicLayout() { Height = 350 };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);
            layout.BeginScrollable(BorderType.None);

            layout.AddRow(GenVentCtrlPanel());
            layout.AddRow(GenDlightCtrlPanel());
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
            wPerArea.TextBinding.Bind(vm, _ => _.Lighting.WattsPerArea.NumberText);
            layout.AddRow("Watts/Area:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Lighting.Schedule.BtnName);
            sch.Bind(_=>_.Command, vm, _ => _.Lighting.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.Lighting.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.Lighting.VisibleFraction.NumberText);
            layout.AddRow("Visible Fraction:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.Lighting.ReturnAirFraction.NumberText);
            layout.AddRow("Return Air Fraction:");
            layout.AddRow(airFraction);

            var baseline = new DoubleText();
            baseline.ReservedText = _vm.Varies;
            baseline.TextBinding.Bind(vm, _ => _.Lighting.BaselineWattsPerArea.NumberText);
            layout.AddRow("Baseline Watts/Area:");
            layout.AddRow(baseline);
            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
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
            wPerArea.TextBinding.Bind(vm, _ => _.ElecEquipment.WattsPerArea.NumberText);
            layout.AddRow("Watts/m2:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.ElecEquipment.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.ElecEquipment.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow( sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.LatentFraction.NumberText);
            layout.AddRow("Latent Fraction:");
            layout.AddRow( visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.LostFraction.NumberText);
            layout.AddRow("Lost Fraction:");
            layout.AddRow( airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
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
            wPerArea.TextBinding.Bind(vm, _ => _.Gas.WattsPerArea.NumberText);
            layout.AddRow("Watts/Area:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Gas.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Gas.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.Gas.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow( radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.Gas.LatentFraction.NumberText);
            layout.AddRow("Latent Fraction:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.Gas.LostFraction.NumberText);
            layout.AddRow("Lost Fraction:");
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
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
            wPerArea.TextBinding.Bind(vm, _ => _.People.PeoplePerArea.NumberText);
            layout.AddRow("People/Area:");
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
            radFraction.TextBinding.Bind(vm, _ => _.People.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.People.LatentFraction.NumberText);
            visFraction.Bind(_ => _.Enabled, vm, _ => _.People.IsLatenFractionInputEnabled);
            var autosize = new CheckBox() { Text = "Autosize" };
            autosize.Bind(_ => _.Checked, vm, _ => _.People.IsLatentFractionAutosize);
            layout.AddRow("Latent Fraction:");
            layout.AddRow(autosize);
            layout.AddRow(visFraction);

            layout.AddRow(null);
        


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
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
            wPerArea.TextBinding.Bind(vm, _ => _.Infiltration.FlowPerExteriorArea.NumberText);
            layout.AddRow("Flow/Area(exterior):");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Infiltration.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Infiltration.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.Infiltration.ConstantCoefficient.NumberText);
            layout.AddRow("Constant Coefficient:");
            layout.AddRow( radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.Infiltration.TemperatureCoefficient.NumberText);
            layout.AddRow("Temperature Coefficient:");
            layout.AddRow( visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.Infiltration.VelocityCoefficient.NumberText);
            layout.AddRow("Velocity Coefficient:");
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
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
            wPerArea.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerArea.NumberText);
            layout.AddRow("Flow/Area:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Ventilation.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Ventilation.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerPerson.NumberText);
            layout.AddRow("Flow/Person:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerZone.NumberText);
            layout.AddRow("Flow/Zone:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.Ventilation.AirChangesPerHour.NumberText);
            layout.AddRow("AirChanges/Hour:");
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
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


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
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
            wPerArea.TextBinding.Bind(vm, _ => _.ServiceHotWater.FlowPerArea.NumberText);
            layout.AddRow("Flow/Area:");
            layout.AddRow(wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.ServiceHotWater.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.ServiceHotWater.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.TargetTemperature.NumberText);
            layout.AddRow("Target Temperature:");
            layout.AddRow(airFraction);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.SensibleFraction.NumberText);
            layout.AddRow("Sensible Fraction:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.LatentFraction.NumberText);
            layout.AddRow("Latent Fraction:");
            layout.AddRow(visFraction);


            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.ServiceHotWater.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Service Hot Water" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }


        private GroupBox GenVentCtrlPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.VentilationControl.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.VentilationControl.IsPanelEnabled);


            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.TextBinding.Bind(vm, _ => _.VentilationControl.MinIndoorTemperature.NumberText);
            layout.AddRow("MinIndoorTemperature:");
            layout.AddRow( wPerArea);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.VentilationControl.MaxIndoorTemperature.NumberText);
            layout.AddRow("MaxIndoorTemperature:");
            layout.AddRow( radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.VentilationControl.MinOutdoorTemperature.NumberText);
            layout.AddRow("MinOutdoorTemperature:");
            layout.AddRow( visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.VentilationControl.MaxOutdoorTemperature.NumberText);
            layout.AddRow("MaxOutdoorTemperature:");
            layout.AddRow(airFraction);

            var delta = new DoubleText();
            delta.ReservedText = _vm.Varies;
            delta.TextBinding.Bind(vm, _ => _.VentilationControl.DeltaTemperature.NumberText);
            layout.AddRow("Delta Temperature:");
            layout.AddRow(delta);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.VentilationControl.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.VentilationControl.ScheduleCommand);
            layout.AddRow("Schedule:");
            layout.AddRow(sch);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.NoControl };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.VentilationControl.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Ventilation Controls" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }


        private GroupBox GenDlightCtrlPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.DaylightingControl.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.DaylightingControl.IsPanelEnabled);


            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.DaylightingControl.SensorPosition.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.DaylightingControl.SensorPositionCommand);
            layout.AddRow("Sensor Position:");
            layout.AddRow(sch);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.TextBinding.Bind(vm, _ => _.DaylightingControl.IlluminanceSetpoint.NumberText);
            layout.AddRow("Illuminance Setpoint:");
            layout.AddRow(wPerArea);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.DaylightingControl.ControlFraction.NumberText);
            layout.AddRow("Control Fraction:");
            layout.AddRow(visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.DaylightingControl.MinPowerInput.NumberText);
            layout.AddRow("MinPower Input:");
            layout.AddRow(airFraction);

            var delta = new DoubleText();
            delta.ReservedText = _vm.Varies;
            delta.TextBinding.Bind(vm, _ => _.DaylightingControl.MinLightOutput.NumberText);
            layout.AddRow("MinLight Output:");
            layout.AddRow( delta);

            var offAtMin = new CheckBox();
            offAtMin.CheckedBinding.Bind(vm, _ => _.DaylightingControl.OffAtMinimum);
            layout.AddRow("Off At Minimum:");
            layout.AddRow(offAtMin);
            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.NoControl };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.DaylightingControl.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Daylighting Controls" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

    }


}