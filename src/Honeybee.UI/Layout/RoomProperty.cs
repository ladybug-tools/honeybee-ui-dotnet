using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{

    public class RoomPropertyDialog : Dialog<List<HB.Room>>
    {
        public RoomPropertyDialog(HB.ModelProperties libSource, List<HB.Room> rooms)
        {
            try
            {
                var p = new DynamicLayout();
                p.DefaultSpacing = new Size(4, 4);
                p.DefaultPadding = new Padding(4);

                p.AddRow(RoomProperty.Instance);
                RoomProperty.Instance.UpdatePanel(libSource, rooms);

                var OKButton = new Button() { Text = "OK" };
                OKButton.Click += (s, e) =>
                {
                    try
                    {
                        this.Close(RoomProperty.Instance.GetRooms());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.Message);
                        //throw;
                    }
                 
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                p.AddSeparateRow(null, OKButton, this.AbortButton, null);
                this.Content = p;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //throw;
            }

        }
    }

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
            //layout.AddSeparateRow(loads);
            tb.Pages.Add(new TabPage(loads) { Text= "Room Loads"});

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
            id.Width = 250;
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
            var vm = this._vm;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);
            layout.BeginScrollable( BorderType.None);

            //Get lighting
            var ltn = GenLightingPanel();
            layout.AddRow(ltn);

            ////Get People
            //var pplDP = new Button();
            //pplDP.Bind((t) => t.Enabled, vm, v => v.People.IsBtnEnabled);
            //pplDP.TextBinding.Bind(vm, _ => _.People.BtnName);
            //pplDP.Command = vm.RoomHVACCommand;
            //var pplByProgram = new CheckBox() { Text = vm.ByProgramType };
            //pplByProgram.CheckedBinding.Bind(vm, _ => _.People.IsCheckboxChecked);

            //layout.AddRow("People:", pplByProgram);
            //layout.AddRow(null, pplDP);


            ////Get ElecEquipment
            //var eqpDP = new Button();
            //eqpDP.Bind((t) => t.Enabled, vm, v => v.ElecEquipment.IsBtnEnabled);
            //eqpDP.TextBinding.Bind(vm, _ => _.ElecEquipment.BtnName);
            //eqpDP.Command = vm.RoomHVACCommand;
            //var eqpByProgram = new CheckBox() { Text = vm.ByProgramType };
            //eqpByProgram.CheckedBinding.Bind(vm, _ => _.ElecEquipment.IsCheckboxChecked);

            //layout.AddRow("ElecEquipment:", eqpByProgram);
            //layout.AddRow(null, eqpDP);


            ////Get gas Equipment
            //var gasDP = new Button();
            //gasDP.Bind((t) => t.Enabled, vm, v => v.Gas.IsBtnEnabled);
            //gasDP.TextBinding.Bind(vm, _ => _.Gas.BtnName);
            //gasDP.Command = vm.RoomHVACCommand;
            //var gasByProgram = new CheckBox() { Text = vm.ByProgramType };
            //gasByProgram.CheckedBinding.Bind(vm, _ => _.Gas.IsCheckboxChecked);

            //layout.AddRow("GasEquipment:", gasByProgram);
            //layout.AddRow(null, gasDP);


            ////Get Ventilation
            //var ventDP = new Button();
            //ventDP.Bind((t) => t.Enabled, vm, v => v.Ventilation.IsBtnEnabled);
            //ventDP.TextBinding.Bind(vm, _ => _.Ventilation.BtnName);
            //ventDP.Command = vm.RoomHVACCommand;
            //var ventByProgram = new CheckBox() { Text = vm.ByProgramType };
            //ventByProgram.CheckedBinding.Bind(vm, _ => _.Ventilation.IsCheckboxChecked);

            //layout.AddRow("Ventilation:", ventByProgram);
            //layout.AddRow(null, ventDP);


            ////Get Infiltration
            //var infDP = new Button();
            //infDP.Bind((t) => t.Enabled, vm, v => v.Infiltration.IsBtnEnabled);
            //infDP.TextBinding.Bind(vm, _ => _.Infiltration.BtnName);
            //infDP.Command = vm.RoomHVACCommand;
            //var infByProgram = new CheckBox() { Text = vm.ByProgramType };
            //infByProgram.CheckedBinding.Bind(vm, _ => _.Infiltration.IsCheckboxChecked);

            //layout.AddRow("Infiltration:", infByProgram);
            //layout.AddRow(null, infDP);


            ////Get Setpoint
            //var stpDP = new Button();
            //stpDP.Bind((t) => t.Enabled, vm, v => v.Setpoint.IsBtnEnabled);
            //stpDP.TextBinding.Bind(vm, _ => _.Setpoint.BtnName);
            //stpDP.Command = vm.RoomHVACCommand;
            //var stpByProgram = new CheckBox() { Text = vm.ByProgramType };
            //stpByProgram.CheckedBinding.Bind(vm, _ => _.Setpoint.IsCheckboxChecked);

            //layout.AddRow("Setpoint:", stpByProgram);
            //layout.AddRow(null, stpDP);


            ////Get ServiceHotWater
            //var shwDP = new Button();
            //shwDP.Bind((t) => t.Enabled, vm, v => v.ServiceHotWater.IsBtnEnabled);
            //shwDP.TextBinding.Bind(vm, _ => _.ServiceHotWater.BtnName);
            //shwDP.Command = vm.RoomHVACCommand;
            //var shwByProgram = new CheckBox() { Text = vm.ByProgramType };
            //shwByProgram.CheckedBinding.Bind(vm, _ => _.ServiceHotWater.IsCheckboxChecked);

            //layout.AddRow("ServiceHotWater:", shwByProgram);
            //layout.AddRow(null, shwDP);

            layout.AddRow(null);

            layout.EndScrollable();
            return layout;
        }
    
        private GroupBox GenLightingPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.Lighting.IsPanelEnabled);

         
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.TextBinding.Bind(vm, _ => _.Lighting.WattsPerArea.NumberText);
            layout.AddRow("Watts/m2:", wPerArea);

            var sch = new Button();
            sch.TextBinding.Bind(vm, _ => _.Lighting.Schedule.BtnName);
            sch.Bind(_=>_.Command, vm, _ => _.Lighting.ScheduleCommand);
            layout.AddRow("Schedule:", sch);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.TextBinding.Bind(vm, _ => _.Lighting.RadiantFraction.NumberText);
            layout.AddRow("Radiant Fraction:", radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.TextBinding.Bind(vm, _ => _.Lighting.VisibleFraction.NumberText);
            layout.AddRow("Visible Fraction:", visFraction);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.TextBinding.Bind(vm, _ => _.Lighting.ReturnAirFraction.NumberText);
            layout.AddRow("Return Air Fraction:", airFraction);

            var baseline = new DoubleText();
            baseline.ReservedText = _vm.Varies;
            baseline.TextBinding.Bind(vm, _ => _.Lighting.BaselineWattsPerArea.NumberText);
            layout.AddRow("Baseline Watts/m2:", baseline);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.ByProgramType };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.Lighting.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Lighting" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }
    
    
    }


}
