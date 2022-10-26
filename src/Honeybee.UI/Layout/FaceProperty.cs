using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;
using System;

namespace Honeybee.UI.View
{

    public class FaceProperty : Panel
    {
        private FacePropertyViewModel _vm { get; set; }
        private static FaceProperty _instance;
        public static FaceProperty Instance
        {
            get
            {
                _instance = _instance ?? new FaceProperty();
                return _instance;
            }
        }
        public Button SchemaDataBtn;

        private static Type _HBObjType = typeof(HB.Face);
        private static HB.Face _dummy = new HB.Face("test", new HB.Face3D(new List<List<double>>()), HB.FaceType.Wall, new HB.Outdoors(), new HB.FacePropertiesAbridged());

        private FaceProperty()
        {
            this._vm = new FacePropertyViewModel(this);
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, List<HB.Face> objs)
        {
            this._vm.Update(libSource, objs);
        }
        public List<HB.Face> GetFaces()
        {
            return this._vm.GetFaces();

        }

        private void Initialize()
        {
            var vm = this._vm;
            var layout = new DynamicLayout();
            layout.Width = 420;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var tb = new TabControl();
            tb.Bind(_ => _.SelectedIndex, vm, _ => _.TabIndex);
            var general = new DynamicLayout();
            general.DefaultSpacing = new Size(4, 4);
            general.DefaultPadding = new Padding(4);

            general.AddRow(GenGeneralPanel());
            general.AddRow(GenRadiancePanel());
            general.AddRow(GenEnergyPanel());
            tb.Pages.Add(new TabPage(general) { Text = "General" });

            var userData = GenUserDataPanel();
            tb.Pages.Add(new TabPage(userData) { Text = "User Data" });
            //tb.
            layout.AddRow(tb);
            //layout.Add(tb, true, false);


            layout.Add(null);
            SchemaDataBtn = new Button { Text = "Data" };
            SchemaDataBtn.Command = vm.HBDataBtnClick;
            //layout.AddSeparateRow(data_button, null);

            this.Content = layout;
        }

        private DynamicLayout GenGeneralPanel()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var idLabel = new Label() { Text = "ID:" };
            idLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.Identifier)));


            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(_vm, (_) => _.Identifier);
            id.Bind(_ => _.ToolTip, _vm, _ => _.Identifier);
            layout.AddRow(idLabel, id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var NameLabel = new Label() { Text = "Name:" };
            NameLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(_HBObjType, nameof(_dummy.DisplayName)));

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, (_) => _.DisplayName);
            layout.AddRow(NameLabel, nameTB);


            var faceTypeText = new TextBox();
            faceTypeText.Bind(_ => _.Text, _vm, _ => _.FaceTypeText);
            var faceTypeDP = new EnumDropDown<HB.FaceType>() { Height = 24};
            faceTypeDP.SelectedValueBinding.Bind(_vm, (_) => _.FaceType);
            faceTypeDP.Visible = false;
            faceTypeText.MouseDown += (s, e) => {
                faceTypeText.Visible = false;
                faceTypeDP.Visible = true;
            };
            faceTypeDP.LostFocus += (s, e) => { 
                faceTypeText.Visible = true;
                faceTypeDP.Visible = false;
            };
            var typeDp = new DynamicLayout();
            typeDp.AddRow(faceTypeText);
            typeDp.AddRow(faceTypeDP);
            layout.AddRow("Face Type:", typeDp);

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
            var cByRoom = new CheckBox() { Text = ReservedText.ByParentSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Modifier.IsCheckboxChecked);

            var ModifierLabel = new Label() { Text = "Modifier:" };
            ModifierLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.PropertiesBaseAbridged), nameof(_dummy.Properties.Radiance.Modifier)));

            layout.AddRow(ModifierLabel, cByRoom);
            layout.AddRow(null, c);

            var ModifierBlkLabel = new Label() { Text = "Modifier Blk:" };
            ModifierBlkLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.FaceRadiancePropertiesAbridged), nameof(_dummy.Properties.Radiance.ModifierBlk)));

            var mb = new Button();
            mb.Bind(_ => _.Enabled, _vm, v => v.ModifierBlk.IsBtnEnabled);
            mb.TextBinding.Bind(_vm, _ => _.ModifierBlk.BtnName);
            mb.Command = this._vm.ModifierBlkCommand;
            var mbByRoom = new CheckBox() { Text = ReservedText.ByParentSetting };
            mbByRoom.CheckedBinding.Bind(_vm, _ => _.ModifierBlk.IsCheckboxChecked);
            layout.AddRow(ModifierBlkLabel, mbByRoom);
            layout.AddRow(null, mb);

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
            var cByRoom = new CheckBox() { Text = ReservedText.ByParentSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Construction.IsCheckboxChecked);

            var ConstructionLabel = new Label() { Text = "Construction:" };
            ConstructionLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.FaceEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.Construction)));

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

            // bc
            var outdoorBc = CreateOutdoorLayout();
            var surfaceBc = CreateSurfaceLayout();
            var otherSideBc = CreateOtherSideTemperatureLayout();

            //AFN
            var afn = GenAFNPanel();

            var masterlayout = new DynamicLayout() { Height = 245};
            masterlayout.AddRow(layout);
            masterlayout.AddRow(outdoorBc);
            masterlayout.AddRow(surfaceBc);
            masterlayout.AddRow(otherSideBc);
            masterlayout.AddRow(afn);

            gp.Content = masterlayout;
            return gp;
        }

        private DynamicLayout CreateOutdoorLayout()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            layout.Bind(_ => _.Enabled, _vm, _ => _.IsOutdoorBoundary);
            layout.Bind(_ => _.Visible, _vm, _ => _.IsOutdoorBoundary);

            var sun_CB = new CheckBox() { Text = "Sun Exposure" };
            sun_CB.CheckedBinding.Bind(_vm, _ => _.BCOutdoor.SunExposure.IsChecked);
       
            var wind_CB = new CheckBox() { Text = "Wind Exposure" };
            wind_CB.CheckedBinding.Bind(_vm, _ => _.BCOutdoor.WindExposure.IsChecked);
            var exposureLayout = new DynamicLayout() { Width = 250};
            exposureLayout.AddRow(sun_CB, wind_CB);
            layout.AddRow(null, exposureLayout);

            var vFactor = new DoubleText();
            vFactor.ReservedText = ReservedText.Varies;
            vFactor.SetDefault(0);
            vFactor.TextBinding.Bind(_vm, _ => _.BCOutdoor.ViewFactor.NumberText);
            vFactor.Bind(_ => _.Enabled, _vm, _ => _.BCOutdoor.IsViewFactorInputEnabled);
            var autosize = new CheckBox() { Text = "Autocalculate" };
            autosize.Bind(_ => _.Checked, _vm, _ => _.BCOutdoor.IsViewFactorAutocalculate);

            var outdoorDummy = new HB.Outdoors();
            var vFactorLabel = new Label() { Text = "View Factor:" };
            vFactorLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(outdoorDummy.GetType(), nameof(outdoorDummy.ViewFactor)));


            layout.AddRow(vFactorLabel, autosize);
            layout.AddRow(null, vFactor);
            layout.AddRow(null);

            return layout;
        }
        private DynamicLayout CreateSurfaceLayout()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            layout.Bind(_ => _.Enabled, _vm, _ => _.IsSurfaceBoundary);
            layout.Bind(_ => _.Visible, _vm, _ => _.IsSurfaceBoundary);

            var adjBtn = new Button() { Text = "Adjacent Surface", Width = 250 };
            adjBtn.Command = _vm.SurfaceBCCommand;
            layout.AddRow(null, adjBtn);

            return layout;
        }

        private DynamicLayout CreateOtherSideTemperatureLayout()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            layout.Bind(_ => _.Enabled, _vm, _ => _.IsOtherSideTemperatureBoundary);
            layout.Bind(_ => _.Visible, _vm, _ => _.IsOtherSideTemperatureBoundary);
            //layout.AddRow("Other Side Temperature:");

            var dummy = new HB.OtherSideTemperature();
            var wPerAreaLabel = new Label() { Text = "Heat Transfer Coeff:" };
            wPerAreaLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.OtherSideTemperature), nameof(dummy.HeatTransferCoefficient)));

            var wPerArea = new DoubleText();
            wPerArea.Width = 200;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(0);
            wPerArea.TextBinding.Bind(_vm, _ => _.BCOtherSideTemperature.HeatTransferCoefficient.NumberText);
            var unit = new Label();
            unit.TextBinding.Bind(_vm, _ => _.BCOtherSideTemperature.HeatTransferCoefficient.DisplayUnitAbbreviation);
            var wPerAreaLayout = new DynamicLayout() { Width = 250};
            wPerAreaLayout.AddRow(wPerArea, unit);
            layout.AddRow(wPerAreaLabel, wPerAreaLayout);
            //layout.AddRow(wPerArea);


            var vFactorLabel = new Label() { Text = "Temperature:" };
            vFactorLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.OtherSideTemperature), nameof(dummy.Temperature)));

            var vFactor = new DoubleText();
            vFactor.Width = 200;
            vFactor.ReservedText = ReservedText.Varies;
            vFactor.SetDefault(0);
            vFactor.TextBinding.Bind(_vm, _ => _.BCOtherSideTemperature.Temperature.NumberText);
            vFactor.Bind(_ => _.Enabled, _vm, _ => _.BCOtherSideTemperature.IsTemperatureInputEnabled);
            var autosize = new CheckBox() { Text = "Autocalculate" };
            autosize.Bind(_ => _.Checked, _vm, _ => _.BCOtherSideTemperature.IsTemperatureAutocalculate);

            var unitT = new Label();
            unitT.TextBinding.Bind(_vm, _ => _.BCOtherSideTemperature.Temperature.DisplayUnitAbbreviation);
            layout.AddRow(vFactorLabel, autosize);
            var vFactorLayout = new DynamicLayout();
            vFactorLayout.AddRow(vFactor, unitT);
            layout.AddRow(null, vFactorLayout);
            layout.AddRow(null);

            return layout;
        }

        private DynamicLayout GenAFNPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            //layout.Bind((t) => t.Enabled, vm, v => v.AFNCrack.IsPanelEnabled);
            //layout.Bind((t) => t.Visible, vm, v => v.AFNCrack.IsPanelEnabled);

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);
            
            var afnByProgram = new CheckBox() { Text = ReservedText.ByParentSetting };
            afnByProgram.CheckedBinding.Bind(_vm, _ => _.AFNCrack.IsCheckboxChecked);
            layout.AddRow("AFNCrack:", afnByProgram);

          
            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.AFNCrack.Default.FlowCoefficient);
            wPerArea.TextBinding.Bind(vm, _ => _.AFNCrack.FlowCoefficient.NumberText);
            var flowLabel = new Label() { Text = "Flow Coefficient:" };
            flowLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.AFNCrack), nameof(_dummy.Properties.Energy.VentCrack.FlowCoefficient)));
            flowLabel.Bind((t) => t.Enabled, vm, v => v.AFNCrack.IsPanelEnabled);
            wPerArea.Bind((t) => t.Enabled, vm, v => v.AFNCrack.IsPanelEnabled);
            layout.AddRow(flowLabel, wPerArea);

       
            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.AFNCrack.Default.FlowExponent);
            radFraction.TextBinding.Bind(vm, _ => _.AFNCrack.FlowExponent.NumberText); 
            var exponentLabel = new Label() { Text = "Flow Exponent:" };
            exponentLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.AFNCrack), nameof(_dummy.Properties.Energy.VentCrack.FlowExponent)));
            exponentLabel.Bind((t) => t.Enabled, vm, v => v.AFNCrack.IsPanelEnabled);
            radFraction.Bind((t) => t.Enabled, vm, v => v.AFNCrack.IsPanelEnabled);
            layout.AddRow(exponentLabel, radFraction);

            layout.AddRow(null);

            return layout;
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
            gd.Height = 400;
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

            add.Bind(_ => _.Command, vm, _ => _.UserData.AddDataCommand);
            edit.Bind(_ => _.Command, vm, _ => _.UserData.EditDataCommand);
            remove.Bind(_ => _.Command, vm, _ => _.UserData.RemoveDataCommand);

            var ltnByProgram = new CheckBox() { Text = ReservedText.NoUserData };
            ltnByProgram.CheckedBinding.Bind(vm, _ => _.UserData.IsCheckboxChecked);

            var gp = new GroupBox() { Text = "User Data" };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4)};

            return gp;
        }

    }

}
