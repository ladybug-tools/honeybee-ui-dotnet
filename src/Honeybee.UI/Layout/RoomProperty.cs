using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HB = HoneybeeSchema;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;
using System;
using HoneybeeSchema;
using HoneybeeSchema.Energy;

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

        public Button SchemaDataBtn;
        private static Type _HBObjType = typeof(HB.Room);
        private static HB.Room _dummy = new HB.Room("test", new List<HB.Face>(), new HB.RoomPropertiesAbridged());

        public RoomProperty()
        {
            this._vm = new RoomPropertyViewModel(this);
            Init();
        }


        public void UpdatePanel(HB.ModelProperties libSource, List<HB.Room> HoneybeeObjs)
        {
            this._vm.Update(libSource, HoneybeeObjs);
        }

        public List<HB.Room> GetRooms()
        {
            return this._vm.GetRooms();
           
        }

        public void SetSensorPositionPicker(Func<List<double>> SensorPositionPicker)
        {
            this._vm.DaylightingControl.SensorPositionPicker = SensorPositionPicker;
            this._vm.DaylightingControl.EnableSensorPositionPicker = SensorPositionPicker != null;
        }
        public void SetInternalMassPicker(Func<double> internalMassAreaPicker)
        {
            this._vm.InternalMass.InternalMassAreaPicker = internalMassAreaPicker;
            this._vm.InternalMass.EnableInternalMassAreaPicker = internalMassAreaPicker != null;
        }

        public void SetAmbientCoffConditionRoomPicker(Func<string> RoomIDPicker)
        {
            this._vm.AmbientCoffConditionRoomPicker = RoomIDPicker;
        }

        public void SetDetailedHVACChecker(Func<DetailedHVAC, IHvac> systemChecker)
        {
            this._vm.DetailedHvacChecker = systemChecker;
        }
        private void Init()
        {
            var vm = this._vm;
            var layout = new DynamicLayout();
            layout.Width = 400;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var tb = new TabControl() { Height = 480 };
            tb.Bind(_ => _.SelectedIndex, vm, _ => _.TabIndex);
            var basis = GenGeneralTab();
            tb.Pages.Add(new TabPage(basis) { Text = "General" });

            var loads = GenLoadsPanel();
            tb.Pages.Add(new TabPage(loads) { Text = "Loads" });

            var ctrls = GenControlPanel();
            tb.Pages.Add(new TabPage(ctrls) { Text = "Controls" });

            var userData = GenUserDataPanel();
            tb.Pages.Add(new TabPage(userData) { Text = "User Data" });
            layout.AddRow(tb);

            layout.AddRow(null);
            SchemaDataBtn = new Button { Text = "Data" };
            SchemaDataBtn.Command = this._vm.HBDataBtnClick;
            //layout.AddSeparateRow(data_button, null);


            this.Content = layout;
        }

        private DynamicLayout GenGeneralTab()
        {
            var layout = new DynamicLayout();

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            //layout.BeginScrollable(BorderType.None);
            layout.AddRow(GenGeneralPanel());
            layout.AddRow(GenRadiancePanel());
            layout.AddRow(GenEnergyPanel());
            layout.Add(null);
            //layout.EndScrollable();
            return layout;
        }

        private DynamicLayout GenGeneralPanel()
        {
            var layout = new DynamicLayout();
            var vm = this._vm;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var idLabel = new Label() { Text = "ID:" };
            idLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.Identifier)));

            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(vm, (_) => _.Identifier);
            id.Bind(_ => _.ToolTip, vm, _ => _.Identifier);
            layout.AddRow("ID: ", id);

            var NameLabel = new Label() { Text = "Name:" };
            NameLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.DisplayName)));

            var nameTB = new StringText() { };
            nameTB.TextBinding.Bind(vm, (_) => _.DisplayName);
            layout.AddRow("Name:", nameTB);

            var storyLabel = new Label() { Text = "Story:" };
            storyLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.Story)));

            var storyTB = new StringText() { };
            storyTB.TextBinding.Bind(vm, (_) => _.Story);
            layout.AddRow(storyLabel, storyTB);


            var multiplierLabel = new Label() { Text = "Multiplier:" };
            multiplierLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.Multiplier)));

            var multiplier_NS = new IntText();
            multiplier_NS.ReservedText = ReservedText.Varies;
            multiplier_NS.SetDefault(vm.Default.Multiplier);
            multiplier_NS.TextBinding.Bind(vm, _ => _.MultiplierText);
            layout.AddRow(multiplierLabel, multiplier_NS);

            var excludeFloorLabel = new Label() { Text = "Exclude Floor:" };
            excludeFloorLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.ExcludeFloorArea)));

            var excludeFloor = new CheckBox();
            excludeFloor.CheckedBinding.Bind(_vm, _ => _.IsExcludeFloor.IsChecked);
            layout.AddRow(excludeFloorLabel, excludeFloor);


            layout.AddRow(null);
            return layout;
        }

        private GroupBox GenRadiancePanel()
        {
            var gp = new GroupBox() { Text = "Radiance" };
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var modifierSetLabel = new Label() { Text = "Modifier Set:" };
            modifierSetLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.RoomRadiancePropertiesAbridged), nameof(_dummy.Properties.Radiance.ModifierSet)));

            //Get modifierSet
            var modifierSetDP = new Button();
            modifierSetDP.Width = 250;
            modifierSetDP.Bind((t) => t.Enabled, vm, v => v.ModifierSet.IsBtnEnabled);
            modifierSetDP.TextBinding.Bind(vm, _ => _.ModifierSet.BtnName);
            modifierSetDP.Command = vm.ModifierSetCommand;
            var modifierSetByProgram = new CheckBox() { Text = ReservedText.ByGlobalModifierSet };
            modifierSetByProgram.CheckedBinding.Bind(vm, _ => _.ModifierSet.IsCheckboxChecked);

            layout.AddRow(modifierSetLabel, modifierSetByProgram);
            layout.AddRow(null, modifierSetDP);

            gp.Content = layout;
            return gp;
        }

        private GroupBox GenEnergyPanel()
        {
            var gp = new GroupBox() { Text = "Energy" };
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);



            //Get constructions
            var constructionSetLabel = new Label() { Text = "Construction Set:" };
            constructionSetLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.RoomEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.ConstructionSet)));

            var constructionSetDP = new Button();
            constructionSetDP.Width = 250;
            constructionSetDP.Bind(_ => _.Enabled, vm, _ => _.ConstructionSet.IsBtnEnabled);
            constructionSetDP.TextBinding.Bind(vm, _ => _.ConstructionSet.BtnName);
            constructionSetDP.Command = vm.RoomConstructionSetCommand;
            var cSetByProgram = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            cSetByProgram.CheckedBinding.Bind(vm, _ => _.ConstructionSet.IsCheckboxChecked);

            layout.AddRow(constructionSetLabel, cSetByProgram);
            layout.AddRow(null, constructionSetDP);


            //Get programs
            var programTypesLabel = new Label() { Text = "Program Type:" };
            programTypesLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.RoomEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.ProgramType)));

            var programTypesDP = new Button();
            programTypesDP.Bind((t) => t.Enabled, vm, v => v.PropgramType.IsBtnEnabled);
            programTypesDP.TextBinding.Bind(vm, _ => _.PropgramType.BtnName);
            programTypesDP.Command = vm.RoomProgramTypeCommand;
            var pTypeByProgram = new CheckBox() { Text = ReservedText.Unoccupied };
            pTypeByProgram.CheckedBinding.Bind(vm, _ => _.PropgramType.IsCheckboxChecked);

            layout.AddRow(programTypesLabel, pTypeByProgram);
            layout.AddRow(null, programTypesDP);

            // Thermal Zone
            var zoneLabel = new Label() { Text = "Zone:" };
            zoneLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.Zone)));

            var zoneTB = new StringText() { };
            zoneTB.TextBinding.Bind(vm, (_) => _.Zone);
            layout.AddRow(zoneLabel, zoneTB);

            //Get HVACs
            var hvacLabel = new Label() { Text = "HVAC:" };
            hvacLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.RoomEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.Hvac)));

            var hvacDP = new Button();
            hvacDP.Bind((t) => t.Enabled, vm, v => v.HVAC.IsBtnEnabled);
            hvacDP.TextBinding.Bind(vm, _ => _.HVAC.BtnName);
            hvacDP.Command = vm.RoomHVACCommand;
            var hvacByProgram = new CheckBox() { Text = ReservedText.Unconditioned };
            hvacByProgram.CheckedBinding.Bind(vm, _ => _.HVAC.IsCheckboxChecked);

            layout.AddRow(hvacLabel, hvacByProgram);
            layout.AddRow(null, hvacDP);

            //Get SHW
            var shwLabel = new Label() { Text = "Service Hot Water:" };
            shwLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.RoomEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.Shw)));

            var shwDP = new Button();
            shwDP.Bind((t) => t.Enabled, vm, v => v.SHW.IsBtnEnabled);
            shwDP.TextBinding.Bind(vm, _ => _.SHW.BtnName);
            shwDP.Command = vm.RoomSHWCommand;
            var shwByProgram = new CheckBox() { Text = ReservedText.NoSystem };
            shwByProgram.CheckedBinding.Bind(vm, _ => _.SHW.IsCheckboxChecked);

            layout.AddRow(shwLabel, shwByProgram);
            layout.AddRow(null, shwDP);

            gp.Content = layout;
            return gp;
        }


        private DynamicLayout GenLoadsPanel()
        {
            var layout = new DynamicLayout();

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

            //Get InternalMass
            layout.AddRow(GenInternalMassPanel());

            //Get process load
            layout.AddRow(GenProcessLoadPanel());

            layout.AddRow(null);

            layout.EndScrollable();
            return layout;
        }

        private DynamicLayout GenControlPanel()
        {
            var layout = new DynamicLayout();

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);
            layout.BeginScrollable(BorderType.None);

            layout.AddRow(GenVentCtrlPanel("Window Ventilation Controls", _vm));
            layout.AddRow(GenDlightCtrlPanel(_vm));
            layout.AddRow(GenVentFanPanel(_vm));
            layout.AddRow(null);

            layout.EndScrollable();
            return layout;
        }

        private GroupBox GenLightingPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.Lighting.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.Lighting.IsPanelVisible);

            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(0);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.Lighting.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "Watts/Area:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.LightingAbridged), nameof(_dummy.Properties.Energy.Lighting.WattsPerArea)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.Lighting.Default.WattsPerArea);
            wPerArea.Bind(_ => _.Enabled, _vm, _ => _.Lighting.WattsPerAreaEnabled);
            wPerArea.TextBinding.Bind(vm, _ => _.Lighting.WattsPerArea.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.Lighting.WattsPerArea.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);

            var wPerRoomCheckbox = new CheckBox() { Text = "Watts/Room:" };
            wPerRoomCheckbox.Bind(_ => _.Checked, _vm, _ => _.Lighting.WattsPerRoomEnabled);


            var wPerRoom = new DoubleText();
            wPerRoom.Width = 250;
            wPerRoom.ReservedText = ReservedText.Varies;
            wPerRoom.SetDefault(0);
            wPerRoom.Bind(_ => _.Enabled, _vm, _ => _.Lighting.WattsPerRoomEnabled);
            wPerRoom.TextBinding.Bind(vm, _ => _.Lighting.WattsPerRoom.NumberText);
            layout.AddRow(wPerRoomCheckbox);
            var wPerRoomUnit = new Label();
            wPerRoomUnit.TextBinding.Bind(vm, _ => _.Lighting.WattsPerRoom.DisplayUnitAbbreviation);
            layout.AddSeparateRow( wPerRoom, wPerRoomUnit);

            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.LightingAbridged), nameof(_dummy.Properties.Energy.Lighting.Schedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Lighting.Schedule.BtnName);
            sch.Bind(_=>_.Command, vm, _ => _.Lighting.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            var radFractionLabel = new Label() { Text = "Radiant Fraction:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.LightingAbridged), nameof(_dummy.Properties.Energy.Lighting.RadiantFraction)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.Lighting.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.Lighting.RadiantFraction.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow(radFraction);

            var visFractionLabel = new Label() { Text = "Visible Fraction:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.LightingAbridged), nameof(_dummy.Properties.Energy.Lighting.VisibleFraction)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.Lighting.Default.VisibleFraction);
            visFraction.TextBinding.Bind(vm, _ => _.Lighting.VisibleFraction.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow(visFraction);


            var airFractionLabel = new Label() { Text = "Return Air Fraction:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.LightingAbridged), nameof(_dummy.Properties.Energy.Lighting.ReturnAirFraction)));

            var airFraction = new DoubleText();
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.Lighting.Default.ReturnAirFraction);
            airFraction.TextBinding.Bind(vm, _ => _.Lighting.ReturnAirFraction.NumberText);
            layout.AddRow(airFractionLabel);
            layout.AddRow(airFraction);


            var baselineLabel = new Label() { Text = "Baseline Watts/Area:" };
            baselineLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.LightingAbridged), nameof(_dummy.Properties.Energy.Lighting.BaselineWattsPerArea)));

            var baseline = new DoubleText();
            baseline.Width = 250;
            baseline.ReservedText = ReservedText.Varies;
            baseline.SetDefault(_vm.Lighting.Default.BaselineWattsPerArea);
            baseline.TextBinding.Bind(vm, _ => _.Lighting.BaselineWattsPerArea.NumberText);
            var unit2 = new Label();
            unit2.TextBinding.Bind(vm, _ => _.Lighting.BaselineWattsPerArea.DisplayUnitAbbreviation);
            layout.AddRow(baselineLabel);
            layout.AddSeparateRow(baseline, unit2);
            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
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
            layout.Bind((t) => t.Visible, vm, v => v.ElecEquipment.IsPanelVisible);

            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.ElecEquipment.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "Watts/Area:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.ElectricEquipment.WattsPerArea)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.ElecEquipment.Default.WattsPerArea);
            wPerArea.Bind(_ => _.Enabled, _vm, _ => _.ElecEquipment.WattsPerAreaEnabled);
            wPerArea.TextBinding.Bind(vm, _ => _.ElecEquipment.WattsPerArea.NumberText);
            layout.AddRow("Watts/Area:");
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.ElecEquipment.WattsPerArea.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);


            var wPerRoomCheckbox = new CheckBox() { Text = "Watts/Room:" };
            wPerRoomCheckbox.Bind(_ => _.Checked, _vm, _ => _.ElecEquipment.WattsPerRoomEnabled);

            var wPerRoom = new DoubleText(); 
            wPerRoom.Width = 250;
            wPerRoom.ReservedText = ReservedText.Varies;
            wPerRoom.SetDefault(0);
            wPerRoom.Bind(_ => _.Enabled, _vm, _ => _.ElecEquipment.WattsPerRoomEnabled);
            wPerRoom.TextBinding.Bind(vm, _ => _.ElecEquipment.WattsPerRoom.NumberText);
            layout.AddRow(wPerRoomCheckbox);
            var wPerRoomUnit = new Label();
            wPerRoomUnit.TextBinding.Bind(vm, _ => _.ElecEquipment.WattsPerRoom.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerRoom, wPerRoomUnit);

            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.ElectricEquipment.Schedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.ElecEquipment.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.ElecEquipment.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow( sch);

            var radFractionLabel = new Label() { Text = "Radiant Fraction:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.ElectricEquipment.RadiantFraction)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.ElecEquipment.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.RadiantFraction.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow(radFraction);

            var visFractionLabel = new Label() { Text = "Latent Fraction:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.ElectricEquipment.LatentFraction)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.ElecEquipment.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.LatentFraction.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow( visFraction);

            var airFractionLabel = new Label() { Text = "Lost Fraction:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.ElectricEquipment.LostFraction)));

            var airFraction = new DoubleText();
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.ElecEquipment.Default.LostFraction);
            airFraction.TextBinding.Bind(vm, _ => _.ElecEquipment.LostFraction.NumberText);
            layout.AddRow(airFractionLabel);
            layout.AddRow( airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
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
            layout.Bind((t) => t.Visible, vm, v => v.Gas.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.Gas.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "Watts/Area:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.GasEquipment.WattsPerArea)));


            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.Gas.Default.WattsPerArea);
            wPerArea.Bind(_ => _.Enabled, _vm, _ => _.Gas.WattsPerAreaEnabled);
            wPerArea.TextBinding.Bind(vm, _ => _.Gas.WattsPerArea.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.Gas.WattsPerArea.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);


            var wPerRoomCheckbox = new CheckBox() { Text = "Watts/Room:" };
            wPerRoomCheckbox.Bind(_ => _.Checked, _vm, _ => _.Gas.WattsPerRoomEnabled);

            var wPerRoom = new DoubleText();
            wPerRoom.Width = 250;
            wPerRoom.ReservedText = ReservedText.Varies;
            wPerRoom.SetDefault(0);
            wPerRoom.Bind(_ => _.Enabled, _vm, _ => _.Gas.WattsPerRoomEnabled);
            wPerRoom.TextBinding.Bind(vm, _ => _.Gas.WattsPerRoom.NumberText);
            layout.AddRow(wPerRoomCheckbox);
            var wPerRoomUnit = new Label();
            wPerRoomUnit.TextBinding.Bind(vm, _ => _.Gas.WattsPerRoom.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerRoom, wPerRoomUnit);

            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.GasEquipment.Schedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Gas.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Gas.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            var radFractionLabel = new Label() { Text = "Radiant Fraction:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.GasEquipment.RadiantFraction)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.Gas.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.Gas.RadiantFraction.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow( radFraction);

            var visFractionLabel = new Label() { Text = "Latent Fraction:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.GasEquipment.LatentFraction)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.Gas.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.Gas.LatentFraction.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow(visFraction);

            var airFractionLabel = new Label() { Text = "Lost Fraction:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.EquipmentBase), nameof(_dummy.Properties.Energy.GasEquipment.LostFraction)));

            var airFraction = new DoubleText();
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.Gas.Default.LostFraction);
            airFraction.TextBinding.Bind(vm, _ => _.Gas.LostFraction.NumberText);
            layout.AddRow(airFractionLabel);
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
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
            layout.Bind((t) => t.Visible, vm, v => v.People.IsPanelVisible);

            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.People.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "People/Area:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.PeopleAbridged), nameof(_dummy.Properties.Energy.People.PeoplePerArea)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.People.Default.PeoplePerArea);
            wPerArea.Bind(_ => _.Enabled, _vm, _ => _.People.PeoplePerAreaEnabled);
            wPerArea.TextBinding.Bind(vm, _ => _.People.PeoplePerArea.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.People.PeoplePerArea.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);


            var wPerRoomCheckbox = new CheckBox() { Text = "People/Room:" };
            wPerRoomCheckbox.Bind(_ => _.Checked, _vm, _ => _.People.PeoplePerRoomEnabled);

            var wPerRoom = new DoubleText();
            wPerRoom.Width = 250;
            wPerRoom.ReservedText = ReservedText.Varies;
            wPerRoom.SetDefault(0);
            wPerRoom.Bind(_ => _.Enabled, _vm, _ => _.People.PeoplePerRoomEnabled);
            wPerRoom.TextBinding.Bind(vm, _ => _.People.PeoplePerRoom.NumberText);
            layout.AddRow(wPerRoomCheckbox);
            layout.AddSeparateRow(wPerRoom, "people");

            var schLabel = new Label() { Text = "Occupancy Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.PeopleAbridged), nameof(_dummy.Properties.Energy.People.OccupancySchedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.People.OccupancySchedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.People.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            var sch2Label = new Label() { Text = "Activity Schedule:" };
            sch2Label.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.PeopleAbridged), nameof(_dummy.Properties.Energy.People.ActivitySchedule)));

            var sch2 = new OptionalButton();
            sch2.TextBinding.Bind(vm, _ => _.People.ActivitySchedule.BtnName);
            sch2.Bind(_ => _.Command, vm, _ => _.People.ActivityScheduleCommand);
            sch2.Bind(_ => _.RemoveCommand, vm, _ => _.People.RemoveActivityScheduleCommand);
            sch2.Bind(_ => _.IsRemoveVisable, vm, _ => _.People.ActivitySchedule.IsRemoveVisable);
            layout.AddRow("Activity Schedule:");
            layout.AddRow(sch2);

            var radFractionLabel = new Label() { Text = "Radiant Fraction:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.PeopleAbridged), nameof(_dummy.Properties.Energy.People.RadiantFraction)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.People.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.People.RadiantFraction.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow(radFraction);

            var visFractionLabel = new Label() { Text = "Latent Fraction:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.PeopleAbridged), nameof(_dummy.Properties.Energy.People.LatentFraction)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.People.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.People.LatentFraction.NumberText);
            visFraction.Bind(_ => _.Enabled, vm, _ => _.People.IsLatenFractionInputEnabled);
            var autosize = new CheckBox() { Text = "Autocalculate" };
            autosize.Bind(_ => _.Checked, vm, _ => _.People.IsLatentFractionAutocalculate);
            layout.AddRow(visFractionLabel);
            layout.AddRow(autosize);
            layout.AddRow(visFraction);

            layout.AddRow(null);
        


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
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
            layout.Bind((t) => t.Visible, vm, v => v.Infiltration.IsPanelVisible);

            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.Infiltration.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "Flow/Area (exterior area):" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.InfiltrationAbridged), nameof(_dummy.Properties.Energy.Infiltration.FlowPerExteriorArea)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.Infiltration.Default.FlowPerExteriorArea);
            wPerArea.TextBinding.Bind(vm, _ => _.Infiltration.FlowPerExteriorArea.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.Infiltration.FlowPerExteriorArea.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);


            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.InfiltrationAbridged), nameof(_dummy.Properties.Energy.Infiltration.Schedule)));


            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Infiltration.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Infiltration.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);


            var radFractionLabel = new Label() { Text = "Constant Coefficient:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.InfiltrationAbridged), nameof(_dummy.Properties.Energy.Infiltration.ConstantCoefficient)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.Infiltration.Default.ConstantCoefficient);
            radFraction.TextBinding.Bind(vm, _ => _.Infiltration.ConstantCoefficient.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow( radFraction);


            var visFractionLabel = new Label() { Text = "Temperature Coefficient:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.InfiltrationAbridged), nameof(_dummy.Properties.Energy.Infiltration.TemperatureCoefficient)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.Infiltration.Default.TemperatureCoefficient);
            visFraction.TextBinding.Bind(vm, _ => _.Infiltration.TemperatureCoefficient.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow( visFraction);


            var airFractionLabel = new Label() { Text = "Velocity Coefficient:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.InfiltrationAbridged), nameof(_dummy.Properties.Energy.Infiltration.VelocityCoefficient)));

            var airFraction = new DoubleText();
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.Infiltration.Default.VelocityCoefficient);
            airFraction.TextBinding.Bind(vm, _ => _.Infiltration.VelocityCoefficient.NumberText);
            layout.AddRow(airFractionLabel);
            layout.AddRow(airFraction);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
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
            layout.Bind((t) => t.Visible, vm, v => v.Ventilation.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);


            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.Ventilation.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "Flow/Area:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationAbridged), nameof(_dummy.Properties.Energy.Ventilation.FlowPerArea)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.Ventilation.Default.FlowPerArea);
            wPerArea.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerArea.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerArea.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);

            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationAbridged), nameof(_dummy.Properties.Energy.Ventilation.Schedule)));

            var sch = new OptionalButton();
            sch.TextBinding.Bind(vm, _ => _.Ventilation.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Ventilation.ScheduleCommand);
            sch.Bind(_ => _.RemoveCommand, vm, _ => _.Ventilation.RemoveScheduleCommand);
            sch.Bind(_ => _.IsRemoveVisable, vm, _ => _.Ventilation.Schedule.IsRemoveVisable);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            var radFractionLabel = new Label() { Text = "Flow/Person:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationAbridged), nameof(_dummy.Properties.Energy.Ventilation.FlowPerPerson)));

            var radFraction = new DoubleText() { Width = 250 };
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.Ventilation.Default.FlowPerPerson);
            radFraction.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerPerson.NumberText);
            layout.AddRow(radFractionLabel);
            var unit2 = new Label();
            unit2.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerPerson.DisplayUnitAbbreviation);
            layout.AddSeparateRow(radFraction, unit2);


            var visFractionLabel = new Label() { Text = "Flow/Zone:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationAbridged), nameof(_dummy.Properties.Energy.Ventilation.FlowPerZone)));

            var visFraction = new DoubleText() { Width = 250 };
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.Ventilation.Default.FlowPerZone);
            visFraction.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerZone.NumberText);
            layout.AddRow(visFractionLabel);
            var unit3 = new Label();
            unit3.TextBinding.Bind(vm, _ => _.Ventilation.FlowPerZone.DisplayUnitAbbreviation);
            layout.AddSeparateRow(visFraction, unit3);


            var airFractionLabel = new Label() { Text = "AirChanges/Hour:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationAbridged), nameof(_dummy.Properties.Energy.Ventilation.AirChangesPerHour)));

            var airFraction = new DoubleText() { Width = 250 };
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.Ventilation.Default.AirChangesPerHour);
            airFraction.TextBinding.Bind(vm, _ => _.Ventilation.AirChangesPerHour.NumberText);
            layout.AddRow(airFractionLabel);
            var unit4 = new Label();
            unit4.TextBinding.Bind(vm, _ => _.Ventilation.AirChangesPerHour.DisplayUnitAbbreviation);
            layout.AddSeparateRow(airFraction, unit4);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
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
            layout.Bind((t) => t.Visible, vm, v => v.Setpoint.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);


            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.Setpoint.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var schLabel = new Label() { Text = "Cooling Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SetpointAbridged), nameof(_dummy.Properties.Energy.Setpoint.CoolingSchedule)));

            var sch = new Button();
            sch.Width = 250;
            sch.TextBinding.Bind(vm, _ => _.Setpoint.CoolingSchedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.Setpoint.CoolingScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            var sch2Label = new Label() { Text = "Heating Schedule:" };
            sch2Label.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SetpointAbridged), nameof(_dummy.Properties.Energy.Setpoint.HeatingSchedule)));

            var sch2 = new Button();
            sch2.TextBinding.Bind(vm, _ => _.Setpoint.HeatingSchedule.BtnName);
            sch2.Bind(_ => _.Command, vm, _ => _.Setpoint.HeatingScheduleCommand);
            layout.AddRow(sch2Label);
            layout.AddRow(sch2);

            var sch3Label = new Label() { Text = "Humidifying Schedule:" };
            sch3Label.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SetpointAbridged), nameof(_dummy.Properties.Energy.Setpoint.HumidifyingSchedule)));

            var sch3 = new OptionalButton();
            sch3.TextBinding.Bind(vm, _ => _.Setpoint.HumidifyingSchedule.BtnName);
            sch3.Bind(_ => _.Command, vm, _ => _.Setpoint.HumidifyingScheduleCommand);
            sch3.Bind(_ => _.RemoveCommand, vm, _ => _.Setpoint.RemoveHumidifyingScheduleCommand);
            sch3.Bind(_ => _.IsRemoveVisable, vm, _ => _.Setpoint.HumidifyingSchedule.IsRemoveVisable);
            layout.AddRow(sch3Label);
            layout.AddRow(sch3);

            var sch4Label = new Label() { Text = "Dehumidifying Schedule:" };
            sch4Label.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SetpointAbridged), nameof(_dummy.Properties.Energy.Setpoint.DehumidifyingSchedule)));

            var sch4 = new OptionalButton();
            sch4.TextBinding.Bind(vm, _ => _.Setpoint.DehumidifyingSchedule.BtnName);
            sch4.Bind(_ => _.Command, vm, _ => _.Setpoint.DehumidifyingScheduleCommand);
            sch4.Bind(_ => _.RemoveCommand, vm, _ => _.Setpoint.RemoveDehumidifyingScheduleCommand);
            sch4.Bind(_ => _.IsRemoveVisable, vm, _ => _.Setpoint.DehumidifyingSchedule.IsRemoveVisable);
            layout.AddRow(sch4Label);
            layout.AddRow(sch4);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
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
            layout.Bind((t) => t.Visible, vm, v => v.ServiceHotWater.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.ServiceHotWater.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "Flow/Area:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ServiceHotWaterAbridged), nameof(_dummy.Properties.Energy.ServiceHotWater.FlowPerArea)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.ServiceHotWater.Default.FlowPerArea);
            wPerArea.Bind(_ => _.Enabled, _vm, _ => _.ServiceHotWater.FlowPerAreaEnabled);
            wPerArea.TextBinding.Bind(vm, _ => _.ServiceHotWater.FlowPerArea.NumberText);
            layout.AddRow(wPerAreaLabel);
            //var unit = new Label();
            //unit.TextBinding.Bind(vm, _ => _.ServiceHotWater.FlowPerArea.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, "L/h-m2");


            var wPerRoomCheckbox = new CheckBox() { Text = "Flow/Room:" };
            wPerRoomCheckbox.Bind(_ => _.Checked, _vm, _ => _.ServiceHotWater.FlowPerRoomEnabled);

            var wPerRoom = new DoubleText();
            wPerRoom.Width = 250;
            wPerRoom.ReservedText = ReservedText.Varies;
            wPerRoom.SetDefault(0);
            wPerRoom.Bind(_ => _.Enabled, _vm, _ => _.ServiceHotWater.FlowPerRoomEnabled);
            wPerRoom.TextBinding.Bind(vm, _ => _.ServiceHotWater.FlowPerRoom.NumberText);
            layout.AddRow(wPerRoomCheckbox);
            layout.AddSeparateRow(wPerRoom, "L/h");

            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ServiceHotWaterAbridged), nameof(_dummy.Properties.Energy.ServiceHotWater.Schedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.ServiceHotWater.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.ServiceHotWater.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);


            var airFractionLabel = new Label() { Text = "Target Temperature:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ServiceHotWaterAbridged), nameof(_dummy.Properties.Energy.ServiceHotWater.TargetTemperature)));

            var airFraction = new DoubleText() { Width = 250 };
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.ServiceHotWater.Default.TargetTemperature);
            airFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.TargetTemperature.NumberText);
            layout.AddRow(airFractionLabel);
            var unit2 = new Label();
            unit2.TextBinding.Bind(vm, _ => _.ServiceHotWater.TargetTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(airFraction, unit2);


            var radFractionLabel = new Label() { Text = "Sensible Fraction:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ServiceHotWaterAbridged), nameof(_dummy.Properties.Energy.ServiceHotWater.SensibleFraction)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.ServiceHotWater.Default.SensibleFraction);
            radFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.SensibleFraction.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow(radFraction);


            var visFractionLabel = new Label() { Text = "Latent Fraction:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ServiceHotWaterAbridged), nameof(_dummy.Properties.Energy.ServiceHotWater.LatentFraction)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.ServiceHotWater.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.ServiceHotWater.LatentFraction.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow(visFraction);


            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.ByProgramType };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.ServiceHotWater.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Service Hot Water" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }


        private GroupBox GenInternalMassPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.InternalMass.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.InternalMass.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.InternalMass.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var wPerAreaLabel = new Label() { Text = "Area:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.InternalMassAbridged), "Area"));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.InternalMass.Default.Area);
            wPerArea.TextBinding.Bind(vm, _ => _.InternalMass.Area.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.InternalMass.Area.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);

            var btn = new Button() { Text = "Get area from geometry"};
            btn.Bind(_ => _.Command, vm, _ => _.InternalMass.InternalMassAreaCommand);
            btn.Bind(_ => _.Enabled, vm, _ => _.InternalMass.EnableInternalMassAreaPicker);
            layout.AddRow(btn);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.InternalMass.Construction.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.InternalMass.ConstructionCommand);
            layout.AddRow("Construction:");
            layout.AddRow(sch);
             

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.InternalMass.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Internal Mass" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private GroupBox GenProcessLoadPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.ProcessLoad.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.ProcessLoad.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            // DisplayName
            var NameLabel = new Label() { Text = "Name:" };
            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.ProcessLoad.DisplayName);
            layout.AddRow(NameLabel);
            layout.AddRow(nameTB);

            var dummy = new HB.ProcessAbridged("test", 1, "sch", HB.FuelTypes.Electricity);

            var wattsLabel = new Label() { Text = "Watts:" };
            wattsLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ProcessAbridged), nameof(dummy.Watts)));

            var watts = new DoubleText();
            watts.Width = 250;
            watts.ReservedText = ReservedText.Varies;
            watts.SetDefault(_vm.ProcessLoad.Default.Watts);
            watts.TextBinding.Bind(vm, _ => _.ProcessLoad.Watts.NumberText);
            layout.AddRow(wattsLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.ProcessLoad.Watts.DisplayUnitAbbreviation);
            layout.AddSeparateRow(watts, unit);

            var fuelTypeTextLabel = new Label() { Text = "Fuel Type:" };
            fuelTypeTextLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ProcessAbridged), nameof(dummy.FuelType)));

            var fuelTypeText = new TextBox();
            fuelTypeText.Bind(_ => _.Text, _vm, _ => _.ProcessLoad.FuelTypeText);
            var fuelTypeDP = new EnumDropDown<HB.FuelTypes>() { Height = 24 };
            fuelTypeDP.SelectedValueBinding.Bind(_vm, (_) => _.ProcessLoad.FuelType);
            fuelTypeDP.Visible = false;
            fuelTypeText.MouseDown += (s, e) => {
                fuelTypeText.Visible = false;
                fuelTypeDP.Visible = true;
            };
            fuelTypeDP.LostFocus += (s, e) => {
                fuelTypeText.Visible = true;
                fuelTypeDP.Visible = false;
            };
            var typeDp = new DynamicLayout();
            typeDp.AddRow(fuelTypeText);
            typeDp.AddRow(fuelTypeDP);
            layout.AddRow(fuelTypeTextLabel);
            layout.AddRow(typeDp);


            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ProcessAbridged), nameof(dummy.Schedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.ProcessLoad.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.ProcessLoad.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            var radFractionLabel = new Label() { Text = "Radiant Fraction:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ProcessAbridged), nameof(dummy.RadiantFraction)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.ProcessLoad.Default.RadiantFraction);
            radFraction.TextBinding.Bind(vm, _ => _.ProcessLoad.RadiantFraction.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow(radFraction);

            var visFractionLabel = new Label() { Text = "Latent Fraction:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ProcessAbridged), nameof(dummy.LatentFraction)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.ProcessLoad.Default.LatentFraction);
            visFraction.TextBinding.Bind(vm, _ => _.ProcessLoad.LatentFraction.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow(visFraction);

            var airFractionLabel = new Label() { Text = "Lost Fraction:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ProcessAbridged), nameof(dummy.LostFraction)));

            var airFraction = new DoubleText();
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.ProcessLoad.Default.LostFraction);
            airFraction.TextBinding.Bind(vm, _ => _.ProcessLoad.LostFraction.NumberText);
            layout.AddRow(airFractionLabel);
            layout.AddRow(airFraction);

            var endUseLabel = new Label() { Text = "End Use Category:" };
            endUseLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ProcessAbridged), nameof(dummy.EndUseCategory)));

            var endUse = new StringText();
            endUse.TextBinding.Bind(_vm, (_) => _.ProcessLoad.EndUseCategory);
            layout.AddRow(endUseLabel);
            layout.AddRow(endUse);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.Noload };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.ProcessLoad.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Process Load" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private static GroupBox GenVentCtrlPanel(string gpTitle, RoomPropertyViewModel vm)
        {

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.VentilationControl.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.VentilationControl.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            var wPerAreaLabel = new Label() { Text = "MinIndoorTemperature:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MinIndoorTemperature)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(vm.VentilationControl.Default.MinIndoorTemperature);
            wPerArea.TextBinding.Bind(vm, _ => _.VentilationControl.MinIndoorTemperature.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.VentilationControl.MinIndoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);

            var radFractionLabel = new Label() { Text = "MaxIndoorTemperature:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MaxIndoorTemperature)));

            var radFraction = new DoubleText() { Width = 250 };
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(vm.VentilationControl.Default.MaxIndoorTemperature);
            radFraction.TextBinding.Bind(vm, _ => _.VentilationControl.MaxIndoorTemperature.NumberText);
            layout.AddRow(radFractionLabel);
            var unit2 = new Label();
            unit2.TextBinding.Bind(vm, _ => _.VentilationControl.MaxIndoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(radFraction, unit2);

            var visFractionLabel = new Label() { Text = "MinOutdoorTemperature:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MinOutdoorTemperature)));

            var visFraction = new DoubleText() { Width = 250 };
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(vm.VentilationControl.Default.MinOutdoorTemperature);
            visFraction.TextBinding.Bind(vm, _ => _.VentilationControl.MinOutdoorTemperature.NumberText);
            layout.AddRow(visFractionLabel);
            var unit3 = new Label();
            unit3.TextBinding.Bind(vm, _ => _.VentilationControl.MinOutdoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(visFraction, unit3);

            var airFractionLabel = new Label() { Text = "MaxOutdoorTemperature:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MaxOutdoorTemperature)));

            var airFraction = new DoubleText() { Width = 250 };
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(vm.VentilationControl.Default.MaxOutdoorTemperature);
            airFraction.TextBinding.Bind(vm, _ => _.VentilationControl.MaxOutdoorTemperature.NumberText);
            layout.AddRow(airFractionLabel);
            var unit4 = new Label();
            unit4.TextBinding.Bind(vm, _ => _.VentilationControl.MaxOutdoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(airFraction, unit4);

            var deltaLabel = new Label() { Text = "Delta Temperature:" };
            deltaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.DeltaTemperature)));

            var delta = new DoubleText() { Width = 250 };
            delta.ReservedText = ReservedText.Varies;
            delta.SetDefault(vm.VentilationControl.Default.DeltaTemperature);
            delta.TextBinding.Bind(vm, _ => _.VentilationControl.DeltaTemperature.NumberText);
            layout.AddRow(deltaLabel);
            var unit5 = new Label();
            unit5.TextBinding.Bind(vm, _ => _.VentilationControl.DeltaTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(delta, unit5);

            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.Schedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.VentilationControl.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.VentilationControl.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            layout.AddRow(null);

            var ltnByProgram = new CheckBox() { Text = ReservedText.NoControl };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.VentilationControl.IsCheckboxChecked);

            var gp = new GroupBox() { Text = gpTitle };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }


        private static GroupBox GenDlightCtrlPanel(RoomPropertyViewModel vm)
        {
            //var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.DaylightingControl.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.DaylightingControl.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            var schLabel = new Label() { Text = "Sensor Position:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DaylightingControl), nameof(_dummy.Properties.Energy.DaylightingControl.SensorPosition)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.DaylightingControl.SensorPosition.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.DaylightingControl.SensorPositionCommand);
            sch.Bind(_ => _.Enabled, vm, _ => _.DaylightingControl.EnableSensorPositionPicker);
            layout.AddRow(schLabel);
            layout.AddRow(sch);


            var wPerAreaLabel = new Label() { Text = "Illuminance Setpoint:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DaylightingControl), nameof(_dummy.Properties.Energy.DaylightingControl.IlluminanceSetpoint)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(vm.DaylightingControl.Default.IlluminanceSetpoint);
            wPerArea.TextBinding.Bind(vm, _ => _.DaylightingControl.IlluminanceSetpoint.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.DaylightingControl.IlluminanceSetpoint.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);

            var visFractionLabel = new Label() { Text = "Control Fraction:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DaylightingControl), nameof(_dummy.Properties.Energy.DaylightingControl.ControlFraction)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(vm.DaylightingControl.Default.ControlFraction);
            visFraction.TextBinding.Bind(vm, _ => _.DaylightingControl.ControlFraction.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow(visFraction);

            var airFractionLabel = new Label() { Text = "MinPower Input:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DaylightingControl), nameof(_dummy.Properties.Energy.DaylightingControl.MinPowerInput)));

            var airFraction = new DoubleText() { Width = 250 };
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(vm.DaylightingControl.Default.MinPowerInput);
            airFraction.TextBinding.Bind(vm, _ => _.DaylightingControl.MinPowerInput.NumberText);
            layout.AddRow(airFractionLabel);
            var unit2 = new Label();
            unit2.TextBinding.Bind(vm, _ => _.DaylightingControl.MinPowerInput.DisplayUnitAbbreviation);
            layout.AddSeparateRow(airFraction, unit2);

            var deltaLabel = new Label() { Text = "MinLight Output:" };
            deltaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DaylightingControl), nameof(_dummy.Properties.Energy.DaylightingControl.MinLightOutput)));

            var delta = new DoubleText();
            delta.ReservedText = ReservedText.Varies;
            delta.SetDefault(vm.DaylightingControl.Default.MinLightOutput);
            delta.TextBinding.Bind(vm, _ => _.DaylightingControl.MinLightOutput.NumberText);
            layout.AddRow(deltaLabel);
            layout.AddRow( delta);

            var offAtMinLabel = new Label() { Text = "Off At Minimum:" };
            offAtMinLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DaylightingControl), nameof(_dummy.Properties.Energy.DaylightingControl.OffAtMinimum)));

            var offAtMin = new CheckBox();
            offAtMin.CheckedBinding.Bind(vm, _ => _.DaylightingControl.OffAtMinimum.IsChecked);
            layout.AddRow(offAtMinLabel);
            layout.AddRow(offAtMin);
            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.NoControl };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.DaylightingControl.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Daylighting Controls" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }


        private static GroupBox GenVentFanPanel(RoomPropertyViewModel vm)
        {
            //var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.VentilationFan.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.VentilationFan.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);


            var ventTypeLabel = new Label() { Text = "Ventilation Type:" };
            ventTypeLabel.ToolTip = vm.VentilationFan.VentilationTypeDescription;

            var VentilationTypeDP = new EnumDropDown<HB.VentilationType>() { Height = 24 };
            VentilationTypeDP.SelectedValueBinding.Bind(vm, (_) => _.VentilationFan.VentilationType);
            layout.AddRow(ventTypeLabel);
            layout.AddRow(VentilationTypeDP);


            var wPerAreaLabel = new Label() { Text = "Flow Rate:" };
            wPerAreaLabel.ToolTip = vm.VentilationFan.FlowRateDescription;

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(vm.VentilationFan.Default.FlowRate);
            wPerArea.TextBinding.Bind(vm, _ => _.VentilationFan.FlowRate.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.VentilationFan.FlowRate.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);


            var radFractionLabel = new Label() { Text = "Pressure Rise:" };
            radFractionLabel.ToolTip = vm.VentilationFan.PressureRiseDescription;

            var radFraction = new DoubleText() { Width = 250 };
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(vm.VentilationFan.Default.PressureRise);
            radFraction.TextBinding.Bind(vm, _ => _.VentilationFan.PressureRise.NumberText);
            layout.AddRow(radFractionLabel);
            var unit2 = new Label();
            unit2.TextBinding.Bind(vm, _ => _.VentilationFan.PressureRise.DisplayUnitAbbreviation);
            layout.AddSeparateRow(radFraction, unit2);


            var visFractionLabel = new Label() { Text = "Efficiency:" };
            visFractionLabel.ToolTip = vm.VentilationFan.EfficiencyDescription;

            var visFraction = new DoubleText() { Width = 250 };
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(vm.VentilationFan.Default.Efficiency);
            visFraction.TextBinding.Bind(vm, _ => _.VentilationFan.Efficiency.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow(visFraction);

            var controlGp = GenVentFanCtrlPanel("Controls", vm);
            layout.AddRow(controlGp);


            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.NoControl };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.VentilationFan.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Fan Ventilation Controls" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

        private static GroupBox GenVentFanCtrlPanel(string gpTitle, RoomPropertyViewModel vm)
        {

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.VentilationFan.Control.IsPanelEnabled);
            layout.Bind((t) => t.Visible, vm, v => v.VentilationFan.Control.IsPanelVisible);


            layout.DefaultSpacing = new Size(4, 4);
            //layout.DefaultPadding = new Padding(4);

            var wPerAreaLabel = new Label() { Text = "MinIndoorTemperature:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MinIndoorTemperature)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(vm.VentilationFan.Control.Default.MinIndoorTemperature);
            wPerArea.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MinIndoorTemperature.NumberText);
            layout.AddRow(wPerAreaLabel);
            var unit = new Label();
            unit.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MinIndoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(wPerArea, unit);

            var radFractionLabel = new Label() { Text = "MaxIndoorTemperature:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MaxIndoorTemperature)));

            var radFraction = new DoubleText() { Width = 250 };
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(vm.VentilationFan.Control.Default.MaxIndoorTemperature);
            radFraction.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MaxIndoorTemperature.NumberText);
            layout.AddRow(radFractionLabel);
            var unit2 = new Label();
            unit2.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MaxIndoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(radFraction, unit2);

            var visFractionLabel = new Label() { Text = "MinOutdoorTemperature:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MinOutdoorTemperature)));

            var visFraction = new DoubleText() { Width = 250 };
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(vm.VentilationFan.Control.Default.MinOutdoorTemperature);
            visFraction.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MinOutdoorTemperature.NumberText);
            layout.AddRow(visFractionLabel);
            var unit3 = new Label();
            unit3.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MinOutdoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(visFraction, unit3);

            var airFractionLabel = new Label() { Text = "MaxOutdoorTemperature:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.MaxOutdoorTemperature)));

            var airFraction = new DoubleText() { Width = 250 };
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(vm.VentilationFan.Control.Default.MaxOutdoorTemperature);
            airFraction.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MaxOutdoorTemperature.NumberText);
            layout.AddRow(airFractionLabel);
            var unit4 = new Label();
            unit4.TextBinding.Bind(vm, _ => _.VentilationFan.Control.MaxOutdoorTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(airFraction, unit4);

            var deltaLabel = new Label() { Text = "Delta Temperature:" };
            deltaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.DeltaTemperature)));

            var delta = new DoubleText() { Width = 250 };
            delta.ReservedText = ReservedText.Varies;
            delta.SetDefault(vm.VentilationFan.Control.Default.DeltaTemperature);
            delta.TextBinding.Bind(vm, _ => _.VentilationFan.Control.DeltaTemperature.NumberText);
            layout.AddRow(deltaLabel);
            var unit5 = new Label();
            unit5.TextBinding.Bind(vm, _ => _.VentilationFan.Control.DeltaTemperature.DisplayUnitAbbreviation);
            layout.AddSeparateRow(delta, unit5);

            var schLabel = new Label() { Text = "Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationControlAbridged), nameof(_dummy.Properties.Energy.WindowVentControl.Schedule)));

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.VentilationFan.Control.Schedule.BtnName);
            sch.Bind(_ => _.Command, vm, _ => _.VentilationFan.Control.ScheduleCommand);
            layout.AddRow(schLabel);
            layout.AddRow(sch);

            layout.AddRow(null);

            var ltnByProgram = new CheckBox() { Text = ReservedText.AlwaysOn };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.VentilationFan.Control.IsCheckboxChecked);

            var gp = new GroupBox() { Text = gpTitle };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }



        private GroupBox GenUserDataPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.UserData.IsPanelEnabled);

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var add = new Button() { Text = "Add" };
            var edit = new Button() { Text = "Edit" };
            var remove = new Button() { Text = "Remove" };
            layout.AddSeparateRow(null, add, edit, remove);

            var gd = new GridView();
            gd.Width = 350;
            gd.Height = 360;
            gd.Bind(_ => _.DataStore, _vm, _ => _.UserData.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => 
            {
                _vm.UserData.SelectedItem = gd.SelectedItem as UserDataItem;
            };

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<UserDataItem, string>(r => r.Key) },
                HeaderText = "Key",
                Width = 100
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<UserDataItem, string>(r => r.Value) },
                HeaderText = "Value",
                Width = 250
            });

            layout.AddRow(gd);
            layout.AddRow(null);

            add.Bind(_ => _.Command, vm, _ => _.UserData.AddDataCommand);
            edit.Bind(_ => _.Command, vm, _ => _.UserData.EditDataCommand);
            remove.Bind(_ => _.Command, vm, _ => _.UserData.RemoveDataCommand);

            var ltnByProgram = new CheckBox() { Text = ReservedText.NoUserData };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.UserData.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "User Data" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4), Height = 430 };

            return gp;
        }
    }


}
