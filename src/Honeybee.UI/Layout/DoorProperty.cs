using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{
    
    public class DoorProperty : Panel
    {
        private DoorPropertyViewModel _vm { get; set; }
        private static DoorProperty _instance;
        public static DoorProperty Instance
        {
            get
            {
                _instance = _instance ?? new DoorProperty();
                return _instance;
            }
        }
        public Button SchemaDataBtn;

        private static Type _HBObjType = typeof(HB.Door);
        private static HB.Door _dummy = new HB.Door("test", new HB.Face3D(new List<List<double>>()), new HB.Outdoors(), new HB.DoorPropertiesAbridged());

        private DoorProperty()
        {
            this._vm = new DoorPropertyViewModel(this);
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, List<HB.Door> objs)
        {
            this._vm.Update(libSource, objs);
        }
        public List<HB.Door> GetDoors()
        {
            return this._vm.GetDoors();

        }

        private void Initialize()
        {
            var vm = this._vm;
            var layout = new DynamicLayout();
            layout.Width = 400;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var tb = new TabControl();
            tb.Bind(_ => _.SelectedIndex, vm, _ => _.TabIndex);
            var basis = GenGeneralTab();
            tb.Pages.Add(new TabPage(basis) { Text = "General" });

            var loads = GenVentPanel();
            tb.Pages.Add(new TabPage(loads) { Text = "Ventilation" });

            var userData = GenUserDataPanel();
            tb.Pages.Add(new TabPage(userData) { Text = "User Data" });

            layout.AddRow(tb);


            layout.Add(null);
            SchemaDataBtn = new Button { Text = "Data" };
            SchemaDataBtn.Command = vm.HBDataBtnClick;
            //layout.AddSeparateRow(data_button, null);

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

            var idLabel = new Label() { Text = "ID:" };
            idLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.Identifier)));

            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(_vm, _ => _.Identifier);
            id.Bind(_ => _.ToolTip, _vm, _ => _.Identifier);
            layout.AddRow(idLabel, id);
            layout.AddRow(null, new Label() { Visible = false }); // add space


            var NameLabel = new Label() { Text = "Name:" };
            NameLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.DisplayName)));

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow(NameLabel, nameTB);

            var isOperableLabel = new Label() { Text = "Is Glass:" };
            isOperableLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.IsGlass)));

            var isOperable = new CheckBox();
            isOperable.CheckedBinding.Bind(_vm, _ => _.IsGlass.IsChecked);
            layout.AddRow(isOperableLabel, isOperable);

            return layout;
        }

        private GroupBox GenRadiancePanel()
        {
            var gp = new GroupBox() { Text = "Radiance" };

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var c = new Button();
            c.Width = 250;
            c.Bind(_ => _.Enabled, _vm, v => v.Modifier.IsBtnEnabled);
            c.TextBinding.Bind(_vm, _ => _.Modifier.BtnName);
            c.Command = this._vm.ModifierCommand;
            var cByRoom = new CheckBox() { Text = ReservedText.ByGlobalSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Modifier.IsCheckboxChecked);

            var ModifierLabel = new Label() { Text = "Modifier:" };
            ModifierLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.PropertiesBaseAbridged), nameof(_dummy.Properties.Radiance.Modifier)));

            layout.AddRow(ModifierLabel, cByRoom);
            layout.AddRow(null, c);

            var mb = new Button();
            mb.Bind(_ => _.Enabled, _vm, v => v.ModifierBlk.IsBtnEnabled);
            mb.TextBinding.Bind(_vm, _ => _.ModifierBlk.BtnName);
            mb.Command = this._vm.ModifierBlkCommand;
            var mbByRoom = new CheckBox() { Text = ReservedText.ByGlobalSetting };
            mbByRoom.CheckedBinding.Bind(_vm, _ => _.ModifierBlk.IsCheckboxChecked);

            var ModifierBlkLabel = new Label() { Text = "Modifier Blk:" };
            ModifierBlkLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DoorRadiancePropertiesAbridged), nameof(_dummy.Properties.Radiance.ModifierBlk)));

            layout.AddRow(ModifierBlkLabel, mbByRoom);
            layout.AddRow(null, mb);


            var DynamicGroupIdentifierLabel = new Label() { Text = "Dynamic Group ID:" };
            DynamicGroupIdentifierLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DoorRadiancePropertiesAbridged), nameof(_dummy.Properties.Radiance.DynamicGroupIdentifier)));

            var dynamicGroup = new StringText();
            dynamicGroup.TextBinding.Bind(_vm, _ => _.DynamicGroupIdentifier);
            layout.AddRow(DynamicGroupIdentifierLabel, dynamicGroup);

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
            var cByRoom = new CheckBox() { Text = ReservedText.ByGlobalSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Construction.IsCheckboxChecked);

            var ConstructionLabel = new Label() { Text = "Construction:" };
            ConstructionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.DoorEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.Construction)));

            layout.AddRow(ConstructionLabel, cByRoom);
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

            var bcLabel = new Label() { Text = "Boundary Condition:" };
            bcLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.BoundaryCondition)));


            var typeDp = new DynamicLayout();
            typeDp.AddRow(bcText);
            typeDp.AddRow(bcDP);
            layout.AddRow(bcLabel, typeDp);


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
            vFactor.ReservedText = ReservedText.Varies;
            vFactor.SetDefault(0);
            vFactor.TextBinding.Bind(_vm, _ => _.BCOutdoor.ViewFactor.NumberText);
            vFactor.Bind(_ => _.Enabled, _vm, _ => _.BCOutdoor.IsViewFactorInputEnabled);
            var autosize = new CheckBox() { Text = "Autocalculate" };
            autosize.Bind(_ => _.Checked, _vm, _ => _.BCOutdoor.IsViewFactorAutocalculate);
            layout.AddRow(autosize);
            layout.AddRow(vFactor);

            var outdoorDummy = new HB.Outdoors();
            var vFactorLabel = new Label() { Text = "View Factor:" };
            vFactorLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(outdoorDummy.GetType(), nameof(outdoorDummy.ViewFactor)));


            layout.AddRow(vFactorLabel);
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

            var wPerAreaLabel = new Label() { Text = "Fraction Area Operable:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationOpening), nameof(_dummy.Properties.Energy.VentOpening.FractionAreaOperable)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.VentilationOpening.Default.FractionAreaOperable);
            wPerArea.TextBinding.Bind(vm, _ => _.VentilationOpening.FractionAreaOperable.NumberText);
            layout.AddRow(wPerAreaLabel);
            layout.AddRow(wPerArea);

            var radFractionLabel = new Label() { Text = "Fraction Height Operable:" };
            radFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationOpening), nameof(_dummy.Properties.Energy.VentOpening.FractionHeightOperable)));

            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.VentilationOpening.Default.FractionHeightOperable);
            radFraction.TextBinding.Bind(vm, _ => _.VentilationOpening.FractionHeightOperable.NumberText);
            layout.AddRow(radFractionLabel);
            layout.AddRow(radFraction);

            var visFractionLabel = new Label() { Text = "Discharge Coefficient:" };
            visFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationOpening), nameof(_dummy.Properties.Energy.VentOpening.DischargeCoefficient)));

            var visFraction = new DoubleText();
            visFraction.ReservedText = ReservedText.Varies;
            visFraction.SetDefault(_vm.VentilationOpening.Default.DischargeCoefficient);
            visFraction.TextBinding.Bind(vm, _ => _.VentilationOpening.DischargeCoefficient.NumberText);
            layout.AddRow(visFractionLabel);
            layout.AddRow(visFraction);

            var autosizeLabel = new Label() { Text = "WindCrossVent:" };
            autosizeLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationOpening), nameof(_dummy.Properties.Energy.VentOpening.WindCrossVent)));

            var autosize = new CheckBox();
            autosize.CheckedBinding.Bind( _vm, _ => _.VentilationOpening.WindCrossVent.IsChecked);
            layout.AddSeparateRow(autosizeLabel, autosize);


            var airFractionLabel = new Label() { Text = "Flow Coefficient Closed:" };
            airFractionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationOpening), nameof(_dummy.Properties.Energy.VentOpening.FlowCoefficientClosed)));

            var airFraction = new DoubleText();
            airFraction.ReservedText = ReservedText.Varies;
            airFraction.SetDefault(_vm.VentilationOpening.Default.FlowCoefficientClosed);
            airFraction.TextBinding.Bind(vm, _ => _.VentilationOpening.FlowCoefficientClosed.NumberText);
            layout.AddRow(airFractionLabel);
            layout.AddRow(airFraction);

            var deltaLabel = new Label() { Text = "Flow Exponent Closed:" };
            deltaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationOpening), nameof(_dummy.Properties.Energy.VentOpening.FlowExponentClosed)));

            var delta = new DoubleText();
            delta.ReservedText = ReservedText.Varies;
            delta.SetDefault(_vm.VentilationOpening.Default.FlowExponentClosed);
            delta.TextBinding.Bind(vm, _ => _.VentilationOpening.FlowExponentClosed.NumberText);
            layout.AddRow(deltaLabel);
            layout.AddRow(delta);

            var twoWayLabel = new Label() { Text = "Two Way Threshold:" };
            twoWayLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.VentilationOpening), nameof(_dummy.Properties.Energy.VentOpening.TwoWayThreshold)));

            var twoWay = new DoubleText();
            twoWay.ReservedText = ReservedText.Varies;
            twoWay.SetDefault(_vm.VentilationOpening.Default.TwoWayThreshold);
            twoWay.TextBinding.Bind(vm, _ => _.VentilationOpening.TwoWayThreshold.NumberText);
            layout.AddRow(twoWayLabel);
            layout.AddRow(twoWay);

            layout.AddRow(null);


            var ltnByProgram = new CheckBox() { Text = ReservedText.NoControl };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.VentilationOpening.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "Ventilation Opening", Height = 470 };
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

            var gp = new GroupBox() { Text = "User Data", Height = 470 };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }



    }

}
