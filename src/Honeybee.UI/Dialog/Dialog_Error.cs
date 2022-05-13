using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class Dialog_Error : Eto.Forms.Dialog
    {
        private ErrorViewModel _vm;

        public Dialog_Error(HoneybeeSchema.ValidationReport report, Action<HoneybeeSchema.ValidationError> showAction = default)
        {
            this.Title = $"Error - {DialogHelper.PluginName}";
            this.Width = 600;
            this.Icon = DialogHelper.HoneybeeIcon;

            _vm = new ErrorViewModel(report, this);

            var message = new TextArea() { Height = 80 };
            var nextBtn = new Button() { Text = ">>"};
            var preBtn = new Button() { Text = "<<" };
            var showBtn = new Button() { Text = "Show" };
            var moreInfoBtn = new Button() { Text = "More Info" };

            var totalErrorCount = new Label();
            var currentErrorIndex = new Label();

            message.TextBinding.Bind(_vm, _=>_.CurrentErrorMessage);
            totalErrorCount.TextBinding.Bind(_vm, _=>_.TotalErrorCount);
            currentErrorIndex.TextBinding.Bind(_vm, _=>_.CurrentErrorIndex);
            nextBtn.Bind(_ => _.Enabled, _vm, _ => _.NextBtnEnabled);
            preBtn.Bind(_ => _.Enabled, _vm, _ => _.PreBtnEnabled);
            nextBtn.Command = _vm.NextBtnCommand;
            preBtn.Command = _vm.PreBtnCommand;
            moreInfoBtn.Command = _vm.MoreInfoCommand;
            showBtn.Command = _vm.ShowCommand;

            preBtn.Enabled = false;
            showBtn.Enabled = showAction != null;

          
            var group = new GroupBox() { Text = $"Found {_vm.TotalErrorCount} Error(s)" };
            var groupLayout = new DynamicLayout();
            groupLayout.DefaultSpacing = new Eto.Drawing.Size(5, 5);
            groupLayout.DefaultPadding = new Eto.Drawing.Padding(5);
            groupLayout.AddSeparateRow(message);
            groupLayout.AddSeparateRow(preBtn, currentErrorIndex, nextBtn, null, showBtn, moreInfoBtn);
            group.Content = groupLayout;

            var OkBtn = new Eto.Forms.Button() { Text = "OK" };
            OkBtn.Click += (s, e) => {
                //if (_vm.Validate())
                //{
                //    var lg = _vm.GetLegend();
                //    this.Close(lg);
                //}
               
            };
            this.AbortButton = new Eto.Forms.Button() { Text = "Close" };
            this.AbortButton.Click += (s, e) => { this.Close(); };


            var layout = new Eto.Forms.DynamicLayout();
            layout.DefaultSpacing = new Eto.Drawing.Size(5, 2);
            layout.DefaultPadding = new Eto.Drawing.Padding(4);
            layout.AddSeparateRow(group);
      
            layout.AddSeparateRow(null, this.AbortButton);
            layout.AddRow(null);
            this.Content = layout;

        }



    }

}
