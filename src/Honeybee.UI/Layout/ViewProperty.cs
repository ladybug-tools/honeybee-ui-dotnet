﻿using Eto.Drawing;
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
            layout.AddRow("ID:", id);
            layout.AddRow(null, new Label() { Visible = false }); // add space

            var nameTB = new StringText();
            nameTB.TextBinding.Bind(_vm, _ => _.DisplayName);
            layout.AddRow("Name:", nameTB);

            // view type
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
            layout.AddRow("View type:", typeDp);

            var hSize = new DoubleText();
            hSize.ReservedText = ReservedText.Varies;
            hSize.SetDefault(_vm.Default.HSize);
            hSize.TextBinding.Bind(_vm, _ => _.HSize.NumberText);
            layout.AddRow("Horizontal angle:", hSize);

            var vSize = new DoubleText();
            vSize.ReservedText = ReservedText.Varies;
            vSize.SetDefault(_vm.Default.VSize);
            vSize.TextBinding.Bind(_vm, _ => _.VSize.NumberText);
            layout.AddRow("Vertical angle:", vSize);

            var shift = new DoubleText();
            shift.ReservedText = ReservedText.Varies;
            shift.SetDefault(_vm.Default.Shift);
            shift.TextBinding.Bind(_vm, _ => _.Shift.NumberText);
            layout.AddRow("Shift:", shift);

            var lift = new DoubleText();
            lift.ReservedText = ReservedText.Varies;
            lift.SetDefault(_vm.Default.Lift);
            lift.TextBinding.Bind(_vm, _ => _.Lift.NumberText);
            layout.AddRow("Lift:", lift);

            var foreClip = new DoubleText();
            foreClip.ReservedText = ReservedText.Varies;
            foreClip.SetDefault(_vm.Default.ForeClip);
            foreClip.TextBinding.Bind(_vm, _ => _.ForeClip.NumberText);
            layout.AddRow("ForeClip:", foreClip);

            var afterClip = new DoubleText();
            afterClip.ReservedText = ReservedText.Varies;
            afterClip.SetDefault(_vm.Default.AftClip);
            afterClip.TextBinding.Bind(_vm, _ => _.AftClip.NumberText);
            layout.AddRow("AfterClip:", afterClip);

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
