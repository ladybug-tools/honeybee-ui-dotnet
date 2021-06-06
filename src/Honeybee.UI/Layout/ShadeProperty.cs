using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{
    
    public class ShadeProperty : Panel
    {
        private ShadePropertyViewModel _vm { get; set; }
        private static ShadeProperty _instance;
        public static ShadeProperty Instance
        {
            get
            {
                _instance = _instance ?? new ShadeProperty();
                return _instance;
            }
        }

        private ShadeProperty()
        {
            this._vm = new ShadePropertyViewModel(this);
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, List<HB.Shade> objs)
        {
            this._vm.Update(libSource, objs);
        }
        public List<HB.Shade> GetShades()
        {
            return this._vm.GetShades();

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
            id.TextBinding.Bind(_vm, _ => _.Identifier);
            layout.AddRow("ID:", id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow("Name:", nameTB);

            var IsDetached = new CheckBox();
            IsDetached.CheckedBinding.Bind(_vm, _ => _.IsDetached.IsChecked);
            layout.AddRow("Is Site Context:", IsDetached);
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


            var sch = new Button();
            sch.Bind(_ => _.Enabled, _vm, v => v.TransmittanceSchedule.IsBtnEnabled);
            sch.TextBinding.Bind(_vm, _ => _.TransmittanceSchedule.BtnName);
            sch.Command = this._vm.ScheduleCommand;
            var noSch = new CheckBox() { Text = _vm.NoSchedule };
            noSch.CheckedBinding.Bind(_vm, _ => _.TransmittanceSchedule.IsCheckboxChecked);

            layout.AddRow("Transmittance Schedule:", noSch);
            layout.AddRow(null, sch);


            gp.Content = layout;
            return gp;
        }


    }

}
