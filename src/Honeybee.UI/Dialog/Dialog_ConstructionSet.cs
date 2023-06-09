using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using HoneybeeSchema;
using HoneybeeSchema.Radiance;

namespace Honeybee.UI
{
    public class Dialog_ConstructionSet: Dialog_ResourceEditor<HB.ConstructionSetAbridged>
    {

        private ConstructionSetViewModel _vm;
        public Dialog_ConstructionSet(ref HB.ModelEnergyProperties libSource, HB.ConstructionSetAbridged constructionSet, bool lockedMode = false)
        {
            try
            {
                _vm = new ConstructionSetViewModel(this, ref libSource, constructionSet);

                Padding = new Padding(5);
                Resizable = true;
                Title = $"Construction Set - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 600);
                this.Icon = DialogHelper.HoneybeeIcon;


                var hbObjType = constructionSet.GetType();

                // Identifier
                var idPanel = DialogHelper.MakeIDEditor(_vm.Identifier, _vm, _ => _.Identifier);
                var idLabel = new Label() { Text = "ID" };
                var idDescription = HoneybeeSchema.SummaryAttribute.GetSummary(hbObjType, nameof(constructionSet.Identifier));
                idLabel.ToolTip = Utility.NiceDescription(idDescription);
        
                var nameTbx = new TextBox() { Enabled = !lockedMode };
                nameTbx.TextBinding.Bind(_vm, _ => _.Name);

                var panelName = new DynamicLayout();
                panelName.DefaultSpacing = new Size(0, 5);
                panelName.AddRow(idLabel, idPanel);
                panelName.AddRow("Name", nameTbx);


                var wallGroup = GenWallSetPanel(lockedMode);
                var floorGroup = GenFloorSetPanel(lockedMode);
                var roofGroup = GenRoofSetPanel(lockedMode);
                var apertureGroup = GenApertureSetPanel(lockedMode);
                var doorGroup = GenDoorSetPanel(lockedMode);
                var shadeGroup = GenShadeSetPanel(lockedMode);
                var airBdGroup = GenAirBoundarySetPanel(lockedMode);
            

                //Left panel
                var panelLeft = new DynamicLayout();
                panelLeft.DefaultSpacing = new Size(0, 5);
            
                panelLeft.BeginScrollable(BorderType.None);
                panelLeft.Height = 600;

                panelLeft.AddRow(panelName);
                panelLeft.AddRow(wallGroup);
                panelLeft.AddRow(floorGroup);
                panelLeft.AddRow(roofGroup);
                panelLeft.AddRow(apertureGroup);
                panelLeft.AddRow(doorGroup);
                panelLeft.AddRow(shadeGroup);
                panelLeft.AddRow(airBdGroup);

                var locked = new CheckBox() { Text = "Locked", Enabled = false };
                locked.Checked = lockedMode;

                var OkButton = new Button { Text = "OK" , Enabled = !lockedMode };
                OkButton.Click += (sender, e) => 
                {
                    var obj = _vm.GetHBObject();
                    OkCommand.Execute(obj);
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var hbData = new Button { Text = "Schema Data" };
                hbData.Command = _vm.HBDataBtnClick;

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(locked, null, OkButton, this.AbortButton, null, hbData) }
                };


                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    panelLeft,
                    new TableRow(buttons),
                    null
                }
                };

               
            }
            catch (Exception e)
            {
                Dialog_Message.ShowFullMessage(e.ToString());
                //throw e;
            }


        }

   
        private GroupBox GenWallSetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode};

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var ext = new Button();
            ext.Command = vm.WallExtSet.SelectConstrucitonCommand;
            ext.TextBinding.Bind(vm, _ => _.WallExtSet.BtnName);
            ext.Bind(_ => _.Enabled, vm, _ => _.WallExtSet.IsBtnEnabled);
            var extByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            extByGlobal.CheckedBinding.Bind(vm, _ => _.WallExtSet.IsCheckboxChecked);
            layout.AddSeparateRow("Exterior", null, extByGlobal);
            layout.AddRow(ext);

            var gnd = new Button();
            gnd.Command = vm.WallGndSet.SelectConstrucitonCommand;
            gnd.TextBinding.Bind(vm, _ => _.WallGndSet.BtnName);
            gnd.Bind(_ => _.Enabled, vm, _ => _.WallGndSet.IsBtnEnabled);
            var gndByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            gndByGlobal.CheckedBinding.Bind(vm.WallGndSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Ground", null, gndByGlobal);
            layout.AddRow(gnd);

            var itr = new Button();
            itr.Command = vm.WallIntSet.SelectConstrucitonCommand;
            itr.TextBinding.Bind(vm, _ => _.WallIntSet.BtnName);
            itr.Bind(_ => _.Enabled, vm, _ => _.WallIntSet.IsBtnEnabled);
            var itrByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            itrByGlobal.CheckedBinding.Bind(vm, _ => _.WallIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Interior", null, itrByGlobal);
            layout.AddRow(itr);

            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Wall Construction Set" };
            gp.Content = layout;

            return gp;
        }

        private GroupBox GenFloorSetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var ext = new Button();
            ext.Command = vm.FloorExtSet.SelectConstrucitonCommand;
            ext.TextBinding.Bind(vm, _ => _.FloorExtSet.BtnName);
            ext.Bind(_ => _.Enabled, vm, _ => _.FloorExtSet.IsBtnEnabled);
            var extByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            extByGlobal.CheckedBinding.Bind(vm, _ => _.FloorExtSet.IsCheckboxChecked);
            layout.AddSeparateRow("Exterior", null, extByGlobal);
            layout.AddRow(ext);

            var gnd = new Button();
            gnd.Command = vm.FloorGndSet.SelectConstrucitonCommand;
            gnd.TextBinding.Bind(vm, _ => _.FloorGndSet.BtnName);
            gnd.Bind(_ => _.Enabled, vm, _ => _.FloorGndSet.IsBtnEnabled);
            var gndByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            gndByGlobal.CheckedBinding.Bind(vm.FloorGndSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Ground", null, gndByGlobal);
            layout.AddRow(gnd);

            var itr = new Button();
            itr.Command = vm.FloorIntSet.SelectConstrucitonCommand;
            itr.TextBinding.Bind(vm, _ => _.FloorIntSet.BtnName);
            itr.Bind(_ => _.Enabled, vm, _ => _.FloorIntSet.IsBtnEnabled);
            var itrByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            itrByGlobal.CheckedBinding.Bind(vm, _ => _.FloorIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Interior", null, itrByGlobal);
            layout.AddRow(itr);

            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Floor Construction Set" };
            gp.Content = layout;

            return gp;
        }

        private GroupBox GenRoofSetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var ext = new Button();
            ext.Command = vm.RoofExtSet.SelectConstrucitonCommand;
            ext.TextBinding.Bind(vm, _ => _.RoofExtSet.BtnName);
            ext.Bind(_ => _.Enabled, vm, _ => _.RoofExtSet.IsBtnEnabled);
            var extByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            extByGlobal.CheckedBinding.Bind(vm, _ => _.RoofExtSet.IsCheckboxChecked);
            layout.AddSeparateRow("Exterior", null, extByGlobal);
            layout.AddRow(ext);

            var gnd = new Button();
            gnd.Command = vm.RoofGndSet.SelectConstrucitonCommand;
            gnd.TextBinding.Bind(vm, _ => _.RoofGndSet.BtnName);
            gnd.Bind(_ => _.Enabled, vm, _ => _.RoofGndSet.IsBtnEnabled);
            var gndByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            gndByGlobal.CheckedBinding.Bind(vm.RoofGndSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Ground", null, gndByGlobal);
            layout.AddRow(gnd);

            var itr = new Button();
            itr.Command = vm.RoofIntSet.SelectConstrucitonCommand;
            itr.TextBinding.Bind(vm, _ => _.RoofIntSet.BtnName);
            itr.Bind(_ => _.Enabled, vm, _ => _.RoofIntSet.IsBtnEnabled);
            var itrByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            itrByGlobal.CheckedBinding.Bind(vm, _ => _.RoofIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Interior", null, itrByGlobal);
            layout.AddRow(itr);

            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Roof Construction Set" };
            gp.Content = layout;

            return gp;
        }

        private GroupBox GenApertureSetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var ext = new Button();
            ext.Command = vm.ApertureExtSet.SelectConstrucitonCommand;
            ext.TextBinding.Bind(vm, _ => _.ApertureExtSet.BtnName);
            ext.Bind(_ => _.Enabled, vm, _ => _.ApertureExtSet.IsBtnEnabled);
            var extByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            extByGlobal.CheckedBinding.Bind(vm, _ => _.ApertureExtSet.IsCheckboxChecked);
            layout.AddSeparateRow("Exterior", null, extByGlobal);
            layout.AddRow(ext);

            var itr = new Button();
            itr.Command = vm.ApertureIntSet.SelectConstrucitonCommand;
            itr.TextBinding.Bind(vm, _ => _.ApertureIntSet.BtnName);
            itr.Bind(_ => _.Enabled, vm, _ => _.ApertureIntSet.IsBtnEnabled);
            var itrByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            itrByGlobal.CheckedBinding.Bind(vm, _ => _.ApertureIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Interior", null, itrByGlobal);
            layout.AddRow(itr);

            var gnd = new Button();
            gnd.Command = vm.ApertureOptSet.SelectConstrucitonCommand;
            gnd.TextBinding.Bind(vm, _ => _.ApertureOptSet.BtnName);
            gnd.Bind(_ => _.Enabled, vm, _ => _.ApertureOptSet.IsBtnEnabled);
            var gndByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            gndByGlobal.CheckedBinding.Bind(vm.ApertureOptSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Operable", null, gndByGlobal);
            layout.AddRow(gnd);

            var sky = new Button();
            sky.Command = vm.ApertureSkySet.SelectConstrucitonCommand;
            sky.TextBinding.Bind(vm, _ => _.ApertureSkySet.BtnName);
            sky.Bind(_ => _.Enabled, vm, _ => _.ApertureSkySet.IsBtnEnabled);
            var skyByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            skyByGlobal.CheckedBinding.Bind(vm.ApertureSkySet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Skylight", null, skyByGlobal);
            layout.AddRow(sky);


            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Aperture Construction Set" };
            gp.Content = layout;

            return gp;
        }

        private GroupBox GenDoorSetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var ext = new Button();
            ext.Command = vm.DoorExtSet.SelectConstrucitonCommand;
            ext.TextBinding.Bind(vm, _ => _.DoorExtSet.BtnName);
            ext.Bind(_ => _.Enabled, vm, _ => _.DoorExtSet.IsBtnEnabled);
            var extByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            extByGlobal.CheckedBinding.Bind(vm, _ => _.DoorExtSet.IsCheckboxChecked);
            layout.AddSeparateRow("Exterior", null, extByGlobal);
            layout.AddRow(ext);

            var itr = new Button();
            itr.Command = vm.DoorIntSet.SelectConstrucitonCommand;
            itr.TextBinding.Bind(vm, _ => _.DoorIntSet.BtnName);
            itr.Bind(_ => _.Enabled, vm, _ => _.DoorIntSet.IsBtnEnabled);
            var itrByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            itrByGlobal.CheckedBinding.Bind(vm, _ => _.DoorIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Interior", null, itrByGlobal);
            layout.AddRow(itr);

            var gnd = new Button();
            gnd.Command = vm.DoorExtGlassSet.SelectConstrucitonCommand;
            gnd.TextBinding.Bind(vm, _ => _.DoorExtGlassSet.BtnName);
            gnd.Bind(_ => _.Enabled, vm, _ => _.DoorExtGlassSet.IsBtnEnabled);
            var gndByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            gndByGlobal.CheckedBinding.Bind(vm.DoorExtGlassSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Exterior Glass", null, gndByGlobal);
            layout.AddRow(gnd);

            var IntGlass = new Button();
            IntGlass.Command = vm.DoorIntGlassSet.SelectConstrucitonCommand;
            IntGlass.TextBinding.Bind(vm, _ => _.DoorIntGlassSet.BtnName);
            IntGlass.Bind(_ => _.Enabled, vm, _ => _.DoorIntGlassSet.IsBtnEnabled);
            var IntGlassByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            IntGlassByGlobal.CheckedBinding.Bind(vm.DoorIntGlassSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Interior Glass", null, IntGlassByGlobal);
            layout.AddRow(IntGlass);

            var overhead = new Button();
            overhead.Command = vm.DoorOverheadSet.SelectConstrucitonCommand;
            overhead.TextBinding.Bind(vm, _ => _.DoorOverheadSet.BtnName);
            overhead.Bind(_ => _.Enabled, vm, _ => _.DoorOverheadSet.IsBtnEnabled);
            var overheadByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            overheadByGlobal.CheckedBinding.Bind(vm.DoorOverheadSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Overhead", null, overheadByGlobal);
            layout.AddRow(overhead);

            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Door Construction Set" };
            gp.Content = layout;

            return gp;
        }

        private GroupBox GenShadeSetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var ext = new Button();
            ext.Command = vm.ShadeSet.SelectConstrucitonCommand;
            ext.TextBinding.Bind(vm, _ => _.ShadeSet.BtnName);
            ext.Bind(_ => _.Enabled, vm, _ => _.ShadeSet.IsBtnEnabled);
            var extByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            extByGlobal.CheckedBinding.Bind(vm, _ => _.ShadeSet.IsCheckboxChecked);
            layout.AddSeparateRow("Shade", null, extByGlobal);
            layout.AddRow(ext);

            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Shade Construction" };
            gp.Content = layout;

            return gp;
        }
        
        private GroupBox GenAirBoundarySetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode };

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var ext = new Button();
            ext.Command = vm.AirBoundarySet.SelectConstrucitonCommand;
            ext.TextBinding.Bind(vm, _ => _.AirBoundarySet.BtnName);
            ext.Bind(_ => _.Enabled, vm, _ => _.AirBoundarySet.IsBtnEnabled);
            var extByGlobal = new CheckBox() { Text = ReservedText.ByGlobalConstructionSet };
            extByGlobal.CheckedBinding.Bind(vm, _ => _.AirBoundarySet.IsCheckboxChecked);
            layout.AddSeparateRow("Air Boundary", null, extByGlobal);
            layout.AddRow(ext);

            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Air Boundary Construction" };
            gp.Content = layout;

            return gp;
        }


    }
}
