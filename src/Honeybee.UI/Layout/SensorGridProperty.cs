using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{
    public class SensorGridProperty : Panel
    {
        private SensorGridPropertyViewModel _vm { get; set; }
        private static SensorGridProperty _instance;
        public static SensorGridProperty Instance => _instance ?? (_instance = new SensorGridProperty());
        public Button SchemaDataBtn;
        private SensorGridProperty()
        {
            this._vm = new SensorGridPropertyViewModel(this);
            Initialize();
        }

        public void UpdatePanel(List<HB.SensorGrid> objs)
        {
            this._vm.Update(objs);
        }
        public List<HB.SensorGrid> GetSensorGrids()
        {
            return this._vm.GetSensorGrids();

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
            var pg = new TabPage(basis) { Text = "General" };
            tb.Pages.Add(pg);

            tb.SelectedPage = pg;
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
            layout.Add(null);
            return layout;
        }

        private DynamicLayout GenGeneralPanel()
        {
            var dummy = new HB.SensorGrid("test", new List<HB.Sensor>());

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var idLabel = new Label() { Text = "ID:" };
            idLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SensorGrid), nameof(dummy.Identifier)));

            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(_vm, _ => _.Identifier);
            id.Bind(_ => _.ToolTip, _vm, _ => _.Identifier);
            layout.AddRow(idLabel, id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTBLabel = new Label() { Text = "Name:" };
            nameTBLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SensorGrid), nameof(dummy.DisplayName)));

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow(nameTBLabel, nameTB);

            var groupIDLabel = new Label() { Text = "Group ID:" };
            groupIDLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SensorGrid), nameof(dummy.GroupIdentifier)));

            var groupID = new StringText();
            groupID.TextBinding.Bind(_vm, _ => _.GroupID);
            layout.AddRow(groupIDLabel, groupID);


            var roomIDLabel = new Label() { Text = "Room ID:" };
            roomIDLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.SensorGrid), nameof(dummy.RoomIdentifier)));

            var roomID = new StringText();
            roomID.TextBinding.Bind(_vm, _ => _.RoomID);
            layout.AddRow(roomIDLabel, roomID);

            return layout;
        }


    }

}
