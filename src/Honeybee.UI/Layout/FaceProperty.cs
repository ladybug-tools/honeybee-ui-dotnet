using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

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
            layout.Width = 400;

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
            layout.AddRow(tb);


            layout.Add(null);
            var data_button = new Button { Text = "Schema Data" };
            data_button.Command = vm.HBDataBtnClick;
            layout.AddSeparateRow(data_button, null);

            this.Content = layout;
        }

        private DynamicLayout GenGeneralPanel()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(_vm, (_) => _.Identifier);
            id.Bind(_ => _.ToolTip, _vm, _ => _.Identifier);
            layout.AddRow("ID:", id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, (_) => _.DisplayName);
            layout.AddRow("Name:", nameTB);
          


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

            layout.AddRow("Modifier:", cByRoom);
            layout.AddRow(null, c);

            var mb = new Button();
            mb.Bind(_ => _.Enabled, _vm, v => v.ModifierBlk.IsBtnEnabled);
            mb.TextBinding.Bind(_vm, _ => _.ModifierBlk.BtnName);
            mb.Command = this._vm.ModifierBlkCommand;
            var mbByRoom = new CheckBox() { Text = ReservedText.ByParentSetting };
            mbByRoom.CheckedBinding.Bind(_vm, _ => _.ModifierBlk.IsCheckboxChecked);
            layout.AddRow("Modifier Blk:", mbByRoom);
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

            //AFN
            var afnByProgram = new CheckBox() { Text = ReservedText.ByParentSetting };
            afnByProgram.CheckedBinding.Bind(_vm, _ => _.AFNCrack.IsCheckboxChecked);
            layout.AddRow("AFNCrack:", afnByProgram);
            var afn = GenAFNPanel();

            var masterlayout = new DynamicLayout();
            masterlayout.AddRow(layout);
            masterlayout.AddRow(afn);

            gp.Content = masterlayout;
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

        private DynamicLayout GenAFNPanel()
        {
            var vm = this._vm;

            var layout = new DynamicLayout();
            layout.Bind((t) => t.Enabled, vm, v => v.AFNCrack.IsPanelEnabled);
            //layout.Bind((t) => t.Visible, vm, v => v.AFNCrack.IsPanelEnabled);

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var wPerArea = new DoubleText();
            wPerArea.Width = 250;
            wPerArea.ReservedText = ReservedText.Varies;
            wPerArea.SetDefault(_vm.AFNCrack.Default.FlowCoefficient);
            wPerArea.TextBinding.Bind(vm, _ => _.AFNCrack.FlowCoefficient.NumberText);
            layout.AddRow("Flow Coefficient:", wPerArea);

       
            var radFraction = new DoubleText();
            radFraction.ReservedText = ReservedText.Varies;
            radFraction.SetDefault(_vm.AFNCrack.Default.FlowExponent);
            radFraction.TextBinding.Bind(vm, _ => _.AFNCrack.FlowExponent.NumberText);
            layout.AddRow("Flow Exponent:", radFraction);

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
            gd.Height = 460;
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
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4)};

            return gp;
        }

    }

}
