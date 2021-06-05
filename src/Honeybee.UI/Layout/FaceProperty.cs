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

            layout.AddRow(GenGeneralPanel());
            layout.AddRow(GenRadiancePanel());
            layout.AddRow(GenEnergyPanel());

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
            layout.AddRow("ID:", id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTB = new TextBox();
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
            var cByRoom = new CheckBox() { Text = _vm.ByRoomConstructionSet };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Modifier.IsCheckboxChecked);

            layout.AddRow("Modifier:", cByRoom);
            layout.AddRow(null, c);

            var mb = new Button();
            mb.Bind(_ => _.Enabled, _vm, v => v.ModifierBlk.IsBtnEnabled);
            mb.TextBinding.Bind(_vm, _ => _.ModifierBlk.BtnName);
            mb.Command = this._vm.ModifierBlkCommand;
            var mbByRoom = new CheckBox() { Text = _vm.ByRoomConstructionSet };
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
            var cByRoom = new CheckBox() { Text = _vm.ByRoomConstructionSet };
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

            gp.Content = layout;
            return gp;
        }

        public DynamicLayout CreateOutdoorLayout()
        {
            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);

            layout.Bind(_ => _.Enabled, _vm, _ => _.IsOutdoorBoundary);

            var sun_CB = new CheckBox() { Text = "Sun Exposure" };
            sun_CB.CheckedBinding.Bind(_vm, _ => _.BCOutdoor.SunExposure);
       
            var wind_CB = new CheckBox() { Text = "Wind Exposure" };
            wind_CB.CheckedBinding.Bind(_vm, _ => _.BCOutdoor.WindExposure);

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
    }
    
}
