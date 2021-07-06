using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{
    
    public class ApertureProperty : Panel
    {
        private AperturePropertyViewModel _vm { get; set; }
        private static ApertureProperty _instance;
        public static ApertureProperty Instance
        {
            get
            {
                _instance = _instance ?? new ApertureProperty();
                return _instance;
            }
        }

        private ApertureProperty()
        {
            this._vm = new AperturePropertyViewModel(this);
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, List<HB.Aperture> objs)
        {
            this._vm.Update(libSource, objs);
        }
        public List<HB.Aperture> GetApertures()
        {
            return this._vm.GetApertures();

        }

        private void Initialize()
        {
            var vm = this._vm;
            var layout = new DynamicLayout();
            layout.Width = 400;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var tb = new TabControl();

            var basis = GenGeneralTab();
            var pg = new TabPage(basis) { Text = "General" };
            tb.Pages.Add(pg);

            var loads = GenVentPanel();
            tb.Pages.Add(new TabPage(loads) { Text = "Ventilation" });
            tb.SelectedPage = pg;
            layout.AddRow(tb);


            layout.Add(null);
            var data_button = new Button { Text = "Schema Data" };
            data_button.Command = vm.HBDataBtnClick;
            layout.AddSeparateRow(data_button, null);

            this.Content = layout;
        }
        private DynamicLayout GenGeneralTab()
        {
            var layout = new DynamicLayout();
            layout.Width = 400;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);


            layout.AddRow(GenGeneralPanel());
            layout.AddRow(GenRadiancePanel());
            layout.AddRow(GenEnergyPanel());
            layout.Add(null);
            return layout;
        }

        private DynamicLayout GenGeneralPanel()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(_vm, _ => _.Identifier);
            layout.AddRow("ID:", id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow("Name:", nameTB);

            var isOperable = new CheckBox();
            isOperable.CheckedBinding.Bind(_vm, _ => _.IsOperable.IsChecked);
            layout.AddRow("Is Operable:", isOperable);
            return layout;
        }

        private GroupBox GenRadiancePanel()
        {
            var gp = new GroupBox() { Text = "Radiance" };

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var c = new ValidateButton();
            c.Width = 250;
            c.Bind(_ => _.Enabled, _vm, v => v.Modifier.IsBtnEnabled);
            c.TextBinding.Bind(_vm, _ => _.Modifier.BtnName);
            c.Command = this._vm.ModifierCommand;
            var cByRoom = new CheckBox() { Text = _vm.ByGlobalSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Modifier.IsCheckboxChecked);

            layout.AddRow("Modifier:", cByRoom);
            layout.AddRow(null, c);

            var mb = new Button();
            mb.Bind(_ => _.Enabled, _vm, v => v.ModifierBlk.IsBtnEnabled);
            mb.TextBinding.Bind(_vm, _ => _.ModifierBlk.BtnName);
            mb.Command = this._vm.ModifierBlkCommand;
            var mbByRoom = new CheckBox() { Text = _vm.ByGlobalSetting };
            mbByRoom.CheckedBinding.Bind(_vm, _ => _.ModifierBlk.IsCheckboxChecked);
            layout.AddRow("Modifier Blk:", mbByRoom);
            layout.AddRow(null, mb);

            var dynamicGroup = new StringText();
            dynamicGroup.TextBinding.Bind(_vm, _ => _.DynamicGroupIdentifier);
            layout.AddRow("Dynamic Group ID:", dynamicGroup);

            gp.Content = layout;
            return gp;
        }

        private GroupBox GenEnergyPanel()
        {
            var gp = new GroupBox() { Text = "Energy" };

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);


            var c = new Button();
            c.Width = 250;
            c.Bind(_ => _.Enabled, _vm, v => v.Construction.IsBtnEnabled);
            c.TextBinding.Bind(_vm, _ => _.Construction.BtnName);
            c.Command = this._vm.ConstructionCommand;
            var cByRoom = new CheckBox() { Text = _vm.ByGlobalSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Construction.IsCheckboxChecked);

            layout.AddRow("Construction:", cByRoom);
            layout.AddRow(null, c);


            // boundary condition
            var bcText = new TextBox();
            bcText.TextBinding.Bind( _vm, _ => _.BoundaryConditionText);

            var bcDP = new DropDown() { Height = 24, DataStore = _vm.BoundaryConditionTexts };
            bcDP.SelectedValueBinding.Bind(_vm, _ => _.BoundaryConditionText);
            bcDP.Visible = false;
            bcText.MouseDown += (s, e) => {
                bcText.Visible = false;
                bcDP.Visible = true;
            };
            bcDP.LostFocus += (s, e) => {
                bcText.Visible = true;
                bcDP.Visible = false;
            };

            var typeDp = new DynamicLayout();
            typeDp.AddRow(bcText);
            typeDp.AddRow(bcDP);
            layout.AddRow("Boundary Condition:", typeDp);


            // outdoor bc
            var outdoorBc = CreateOutdoorLayout();
            layout.AddRow(null, outdoorBc);
            var surfaceBc = CreateSurfaceLayout();
            layout.AddRow(null, surfaceBc);

            gp.Content = layout;
            return gp;
        }

        private DynamicLayout CreateOutdoorLayout()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);

            layout.Bind(_ => _.Enabled, _vm, _ => _.IsOutdoorBoundary);

            var sun_CB = new CheckBox() { Text = "Sun Exposure" };
            sun_CB.CheckedBinding.Bind(_vm, _ => _.BCOutdoor.SunExposure.IsChecked);
       
            var wind_CB = new CheckBox() { Text = "Wind Exposure" };
            wind_CB.CheckedBinding.Bind(_vm, _ => _.BCOutdoor.WindExposure.IsChecked);

            layout.AddRow(sun_CB);
            layout.AddRow(wind_CB);


            var vFactor = new DoubleText();
            vFactor.ReservedText = _vm.Varies;
            vFactor.SetDefault(0);
            vFactor.TextBinding.Bind(_vm, _ => _.BCOutdoor.ViewFactor.NumberText);
            vFactor.Bind(_ => _.Enabled, _vm, _ => _.BCOutdoor.IsViewFactorInputEnabled);
            var autosize = new CheckBox() { Text = "Autocalculate" };
            autosize.Bind(_ => _.Checked, _vm, _ => _.BCOutdoor.IsViewFactorAutocalculate);
            layout.AddRow(autosize);
            layout.AddRow(vFactor);

            layout.AddRow("View Factor:");
            layout.AddRow(autosize);
            layout.AddRow(vFactor);

            return layout;
        }

        private DynamicLayout CreateSurfaceLayout()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);

            layout.Bind(_ => _.Enabled, _vm, _ => _.IsSurfaceBoundary);

            var adjBtn = new Button() { Text = "Adjacent Surface" };
            adjBtn.Command = _vm.SurfaceBCCommand;
            layout.AddRow(adjBtn);
            return layout;
        }

        private GroupBox GenVentPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.VentilationOpening.IsPanelEnabled);
            //layout.Bind((t) => t.Visible, vm, v => v.VentilationOpening.IsPanelEnabled);

            // double fractionAreaOperable = 0.5, double fractionHeightOperable = 1, double dischargeCoefficient = 0.45,
            // bool windCrossVent = false, double flowCoefficientClosed = 0, double flowExponentClosed = 0.65, double twoWayThreshold = 0.0001

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = _vm.Varies;
            wPerArea.SetDefault(_vm.VentilationOpening.Default.FractionAreaOperable);
            wPerArea.TextBinding.Bind(vm, _ => _.VentilationOpening.FractionAreaOperable.NumberText);
            layout.AddRow("Fraction Area Operable:");
            layout.AddRow(wPerArea);

            var radFraction = new DoubleText();
            radFraction.ReservedText = _vm.Varies;
            radFraction.SetDefault(_vm.VentilationOpening.Default.FractionHeightOperable);
            radFraction.TextBinding.Bind(vm, _ => _.VentilationOpening.FractionHeightOperable.NumberText);
            layout.AddRow("Fraction Height Operable:");
            layout.AddRow(radFraction);

            var visFraction = new DoubleText();
            visFraction.ReservedText = _vm.Varies;
            visFraction.SetDefault(_vm.VentilationOpening.Default.DischargeCoefficient);
            visFraction.TextBinding.Bind(vm, _ => _.VentilationOpening.DischargeCoefficient.NumberText);
            layout.AddRow("Discharge Coefficient:");
            layout.AddRow(visFraction);

            var autosize = new CheckBox() { Text = "WindCrossVent" };
            autosize.CheckedBinding.Bind( _vm, _ => _.VentilationOpening.WindCrossVent.IsChecked);
            layout.AddRow(autosize);

            var airFraction = new DoubleText();
            airFraction.ReservedText = _vm.Varies;
            airFraction.SetDefault(_vm.VentilationOpening.Default.FlowCoefficientClosed);
            airFraction.TextBinding.Bind(vm, _ => _.VentilationOpening.FlowCoefficientClosed.NumberText);
            layout.AddRow("Flow Coefficient Closed:");
            layout.AddRow(airFraction);

            var delta = new DoubleText();
            delta.ReservedText = _vm.Varies;
            delta.SetDefault(_vm.VentilationOpening.Default.FlowExponentClosed);
            delta.TextBinding.Bind(vm, _ => _.VentilationOpening.FlowExponentClosed.NumberText);
            layout.AddRow("Flow Exponent Closed:");
            layout.AddRow(delta);

            var twoWay = new DoubleText();
            twoWay.ReservedText = _vm.Varies;
            twoWay.SetDefault(_vm.VentilationOpening.Default.TwoWayThreshold);
            twoWay.TextBinding.Bind(vm, _ => _.VentilationOpening.TwoWayThreshold.NumberText);
            layout.AddRow("Two Way Threshold:");
            layout.AddRow(twoWay);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = vm.NoControl };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.VentilationOpening.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Ventilation Opening", Height = 500 };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };
            return gp;
        }





    }

}
