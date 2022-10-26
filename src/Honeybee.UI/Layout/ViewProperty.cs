using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI.View
{
    public class ViewProperty : Panel
    {
        private ViewPropertyViewModel _vm { get; set; }
        private static ViewProperty _instance;
        public static ViewProperty Instance => _instance ?? (_instance = new ViewProperty());
        public Button SchemaDataBtn;
        private ViewProperty()
        {
            this._vm = new ViewPropertyViewModel(this);
            Initialize();
        }

        public void UpdatePanel(List<HB.View> objs)
        {
            this._vm.Update(objs);
        }
        public List<HB.View> GetViews()
        {
            return this._vm.GetViews();

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
            var _dummy = new HB.View("test", new List<double>(), new List<double>(), new List<double>());

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

            var id = new Label() { Width = 255 };
            id.TextBinding.Bind(_vm, _ => _.Identifier);
            id.Bind(_ => _.ToolTip, _vm, _ => _.Identifier);
            layout.AddRow("ID:", id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTBLabel = new Label() { Text = "Name:" };
            nameTBLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.DisplayName)));

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow(nameTBLabel, nameTB);

            var pxLabel = new Label() { Text = "Position:" };
            pxLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.Position)));

            var px = new DoubleText() {Width = 70 };
            px.ReservedText = ReservedText.Varies;
            px.SetDefault(_vm.Default.Position[0]);
            px.TextBinding.Bind(_vm, _ => _.Px.NumberText);
            var py = new DoubleText() { Width = 70 };
            py.ReservedText = ReservedText.Varies;
            py.SetDefault(_vm.Default.Position[1]);
            py.TextBinding.Bind(_vm, _ => _.Py.NumberText);
            var pz = new DoubleText() { Width = 70 };
            pz.ReservedText = ReservedText.Varies;
            pz.SetDefault(_vm.Default.Position[2]);
            pz.TextBinding.Bind(_vm, _ => _.Pz.NumberText);
            var position = new DynamicLayout() { DefaultSpacing = new Size(4, 4) };
            position.AddRow("X", px, "Y", py, "Z", pz);
            layout.AddRow(pxLabel, position);

            // direction

            var vxLabel = new Label() { Text = "Direction:" };
            vxLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.Direction)));

            var vx = new DoubleText() { Width = 70 };
            vx.ReservedText = ReservedText.Varies;
            vx.SetDefault(_vm.Default.Direction[0]);
            vx.TextBinding.Bind(_vm, _ => _.Vx.NumberText);
            var vy = new DoubleText() { Width = 70 };
            vy.ReservedText = ReservedText.Varies;
            vy.SetDefault(_vm.Default.Direction[1]);
            vy.TextBinding.Bind(_vm, _ => _.Vy.NumberText);
            var vz = new DoubleText() { Width = 70 };
            vz.ReservedText = ReservedText.Varies;
            vz.SetDefault(_vm.Default.Direction[2]);
            vz.TextBinding.Bind(_vm, _ => _.Vz.NumberText);
            var direction = new DynamicLayout() { DefaultSpacing = new Size(4, 4) };
            direction.AddRow("X", vx, "Y", vy, "Z", vz);
            layout.AddRow(vxLabel, direction);

            // up vector
            var UxLabel = new Label() { Text = "Up Vector:" };
            UxLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.UpVector)));

            var Ux = new DoubleText() { Width = 70 };
            Ux.ReservedText = ReservedText.Varies;
            Ux.SetDefault(_vm.Default.UpVector[0]);
            Ux.TextBinding.Bind(_vm, _ => _.Ux.NumberText);
            var Uy = new DoubleText() { Width = 70 };
            Uy.ReservedText = ReservedText.Varies;
            Uy.SetDefault(_vm.Default.UpVector[1]);
            Uy.TextBinding.Bind(_vm, _ => _.Uy.NumberText);
            var Uz = new DoubleText() { Width = 70 };
            Uz.ReservedText = ReservedText.Varies;
            Uz.SetDefault(_vm.Default.UpVector[2]);
            Uz.TextBinding.Bind(_vm, _ => _.Uz.NumberText);
            var upvec = new DynamicLayout() { DefaultSpacing = new Size(4, 4) };
            upvec.AddRow("X", Ux, "Y", Uy, "Z", Uz);
            layout.AddRow(UxLabel, upvec);

            // view type
            var viewTypeLabel = new Label() { Text = "View type:" };
            viewTypeLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.ViewType)));

            var viewTypeText = new TextBox();
            viewTypeText.Bind(_ => _.Text, _vm, _ => _.ViewTypeText);
            var viewTypeDP = new DropDown() { Height = 24 };
            viewTypeDP.DataStore = System.Enum.GetValues(typeof(HB.ViewType)).OfType<object>().ToList(); ;
            viewTypeDP.ItemTextBinding = Binding.Delegate((HB.ViewType vmbd) => ViewPropertyViewModel.ViewTypeNames[vmbd]);
            viewTypeDP.SelectedValueBinding.Bind(_vm, (_) => _.ViewType); 
            viewTypeDP.Visible = false;
            viewTypeText.MouseDown += (s, e) => {
                viewTypeText.Visible = false;
                viewTypeDP.Visible = true;
            };
            viewTypeDP.LostFocus += (s, e) => {
                viewTypeText.Visible = true;
                viewTypeDP.Visible = false;
            };
            var typeDp = new DynamicLayout();
            typeDp.AddRow(viewTypeText);
            typeDp.AddRow(viewTypeDP);
            layout.AddRow(viewTypeLabel, typeDp);

            var hSizeLabel = new Label() { Text = "Horizontal angle:" };
            hSizeLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.HSize)));

            var hSize = new DoubleText();
            hSize.ReservedText = ReservedText.Varies;
            hSize.SetDefault(_vm.Default.HSize);
            hSize.TextBinding.Bind(_vm, _ => _.HSize.NumberText);
            layout.AddRow(hSizeLabel, hSize);

            var vSizeLabel = new Label() { Text = "Vertical angle:" };
            vSizeLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.VSize)));

            var vSize = new DoubleText();
            vSize.ReservedText = ReservedText.Varies;
            vSize.SetDefault(_vm.Default.VSize);
            vSize.TextBinding.Bind(_vm, _ => _.VSize.NumberText);
            layout.AddRow(vSizeLabel, vSize);

            var shiftLabel = new Label() { Text = "Shift:" };
            shiftLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.Shift)));

            var shift = new DoubleText();
            shift.ReservedText = ReservedText.Varies;
            shift.SetDefault(_vm.Default.Shift);
            shift.TextBinding.Bind(_vm, _ => _.Shift.NumberText);
            layout.AddRow(shiftLabel, shift);

            var liftLabel = new Label() { Text = "Lift:" };
            liftLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.Lift)));

            var lift = new DoubleText();
            lift.ReservedText = ReservedText.Varies;
            lift.SetDefault(_vm.Default.Lift);
            lift.TextBinding.Bind(_vm, _ => _.Lift.NumberText);
            layout.AddRow(liftLabel, lift);

            var foreClipLabel = new Label() { Text = "ForeClip:" };
            foreClipLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.ForeClip)));

            var foreClip = new DoubleText();
            foreClip.ReservedText = ReservedText.Varies; 
            foreClip.SetDefault(_vm.Default.ForeClip);
            foreClip.TextBinding.Bind(_vm, _ => _.ForeClip.NumberText);
            layout.AddRow(foreClipLabel, foreClip);

            var AfterClipLabel = new Label() { Text = "AfterClip:" };
            AfterClipLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.AftClip)));

            var afterClip = new DoubleText();
            afterClip.ReservedText = ReservedText.Varies;
            afterClip.SetDefault(_vm.Default.AftClip);
            afterClip.TextBinding.Bind(_vm, _ => _.AftClip.NumberText);
            layout.AddRow(AfterClipLabel, afterClip);

            var groupIDLabel = new Label() { Text = "Group ID:" };
            groupIDLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.GroupIdentifier)));

            var groupID = new StringText();
            groupID.TextBinding.Bind(_vm, _ => _.GroupID);
            layout.AddRow(groupIDLabel, groupID);

            var roomIDLabel = new Label() { Text = "Room ID:" };
            roomIDLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(HB.View), nameof(_dummy.RoomIdentifier)));

            var roomID = new StringText();
            roomID.TextBinding.Bind(_vm, _ => _.RoomID);
            layout.AddRow(roomIDLabel, roomID);

            return layout;
        }

   
    }

}
