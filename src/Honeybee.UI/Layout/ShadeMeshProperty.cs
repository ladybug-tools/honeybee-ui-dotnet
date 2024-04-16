using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{

    public class ShadeMeshProperty : Panel
    {
        private ShadeMeshPropertyViewModel _vm { get; set; }
        private static ShadeMeshProperty _instance;
        public static ShadeMeshProperty Instance
        {
            get
            {
                _instance = _instance?? new ShadeMeshProperty();
                return _instance;
            }
        }
        public Button SchemaDataBtn;

        private static HB.ShadeMesh _dummy = new HB.ShadeMesh("test", new HB.Mesh3D(new List<List<double>>(), new List<List<int>>()), new HB.ShadeMeshPropertiesAbridged());
        private ShadeMeshProperty()
        {
            this._vm = new ShadeMeshPropertyViewModel(this);
            Initialize();
        }

        public void UpdatePanel(HB.ModelProperties libSource, List<HB.ShadeMesh> objs)
        {
            this._vm.Update(libSource, objs);
        }
        public List<HB.ShadeMesh> GetShadeMeshs()
        {
            return this._vm.GetShadeMeshs();

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
            idLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.Shade), nameof(_dummy.Identifier)));

            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(_vm, _ => _.Identifier);
            id.Bind(_ => _.ToolTip, _vm, _ => _.Identifier);
            layout.AddRow(idLabel, id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTBLabel = new Label() { Text = "Name:" };
            nameTBLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.Shade), nameof(_dummy.DisplayName)));

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow(nameTBLabel, nameTB);

            var IsDetachedLabel = new Label() { Text = "Is Site Context:" };
            IsDetachedLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.Shade), nameof(_dummy.IsDetached)));

            var IsDetached = new CheckBox();
            IsDetached.CheckedBinding.Bind(_vm, _ => _.IsDetached.IsChecked);
            layout.AddRow(IsDetachedLabel, IsDetached);
            return layout;
        }

        private GroupBox GenRadiancePanel()
        {
            var gp = new GroupBox() { Text = "Radiance" };

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var cLabel = new Label() { Text = "Modifier:" };
            cLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ShadeRadiancePropertiesAbridged), nameof(_dummy.Properties.Radiance.Modifier)));

            var c = new Button();
            c.Width = 250;
            c.Bind(_ => _.Enabled, _vm, v => v.Modifier.IsBtnEnabled);
            c.TextBinding.Bind(_vm, _ => _.Modifier.BtnName);
            c.Command = this._vm.ModifierCommand;
            var cByRoom = new CheckBox() { Text = ReservedText.ByGlobalSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Modifier.IsCheckboxChecked);

            layout.AddRow(cLabel, cByRoom);
            layout.AddRow(null, c);

            var mbLabel = new Label() { Text = "Modifier Blk:" };
            mbLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ShadeRadiancePropertiesAbridged), nameof(_dummy.Properties.Radiance.ModifierBlk)));

            var mb = new Button();
            mb.Bind(_ => _.Enabled, _vm, v => v.ModifierBlk.IsBtnEnabled);
            mb.TextBinding.Bind(_vm, _ => _.ModifierBlk.BtnName);
            mb.Command = this._vm.ModifierBlkCommand;
            var mbByRoom = new CheckBox() { Text = ReservedText.ByGlobalSetting };
            mbByRoom.CheckedBinding.Bind(_vm, _ => _.ModifierBlk.IsCheckboxChecked);
            layout.AddRow(mbLabel, mbByRoom);
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


            var cLabel = new Label() { Text = "Construction:" };
            cLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ShadeEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.Construction)));

            var c = new Button();
            c.Width = 250;
            c.Bind(_ => _.Enabled, _vm, v => v.Construction.IsBtnEnabled);
            c.TextBinding.Bind(_vm, _ => _.Construction.BtnName);
            c.Command = this._vm.ConstructionCommand;
            var cByRoom = new CheckBox() { Text = ReservedText.ByGlobalSetting };
            cByRoom.CheckedBinding.Bind(_vm, _ => _.Construction.IsCheckboxChecked);

            layout.AddRow(cLabel, cByRoom);
            layout.AddRow(null, c);


            var schLabel = new Label() { Text = "Transmittance Schedule:" };
            schLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.ShadeEnergyPropertiesAbridged), nameof(_dummy.Properties.Energy.TransmittanceSchedule)));

            var sch = new Button();
            sch.Bind(_ => _.Enabled, _vm, v => v.TransmittanceSchedule.IsBtnEnabled);
            sch.TextBinding.Bind(_vm, _ => _.TransmittanceSchedule.BtnName);
            sch.Command = this._vm.ScheduleCommand;
            var noSch = new CheckBox() { Text = ReservedText.NoSchedule };
            noSch.CheckedBinding.Bind(_vm, _ => _.TransmittanceSchedule.IsCheckboxChecked);

            layout.AddRow(schLabel, noSch);
            layout.AddRow(null, sch);


            gp.Content = layout;
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
            gd.Height = 250;
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

            var gp = new GroupBox() { Text = "User Data", Height = 350 };
            gp.Content = new StackLayout(ltnByProgram, layout) { Spacing = 4, Padding = new Padding(4) };

            return gp;
        }

    }

}
