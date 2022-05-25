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
            id.Bind(_ => _.ToolTip, _vm, _ => _.Identifier);
            layout.AddRow("ID:", id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow("Name:", nameTB);

            var groupID = new StringText();
            groupID.TextBinding.Bind(_vm, _ => _.GroupID);
            layout.AddRow("Group ID:", groupID);

            var roomID = new StringText();
            roomID.TextBinding.Bind(_vm, _ => _.RoomID);
            layout.AddRow("Room ID:", roomID);

            return layout;
        }


    }

}
