using Eto.Forms;
using System;

namespace Honeybee.UI
{
    public class Dialog_Error : Eto.Forms.Form
    {
        private ErrorViewModel _vm;

        public Dialog_Error(HoneybeeSchema.ValidationReport report, Action<HoneybeeSchema.ValidationError, bool> showAction = default)
        {
            this.Title = $"Error - {DialogHelper.PluginName}";
            this.Width = 800;
            this.Icon = DialogHelper.HoneybeeIcon;

            _vm = new ErrorViewModel(report, this, showAction);

         
            var grid = GenGridView();
            var nextBtn = new Button() { Text = ">>"};
            var preBtn = new Button() { Text = "<<" };
            var showBtn = new Button() { Text = "Show" };
            var showParentBtn = new Button() { Text = "Show Parent" };
            var moreInfoBtn = new Button() { Text = "More Info" };

            var totalErrorCount = new Label();
            var currentErrorIndex = new Label();

            var message = new TextArea() { Height = 84 };

            message.TextBinding.Bind(_vm, _=>_.CurrentErrorMessage);
            totalErrorCount.TextBinding.Bind(_vm, _=>_.TotalErrorCount);
            currentErrorIndex.TextBinding.Bind(_vm, _=>_.CurrentErrorIndex);
            nextBtn.Bind(_ => _.Enabled, _vm, _ => _.NextBtnEnabled);
            preBtn.Bind(_ => _.Enabled, _vm, _ => _.PreBtnEnabled);
            moreInfoBtn.Bind(_ => _.Enabled, _vm, _ => _.MoreBtnEnabled);

            showBtn.Bind(_ => _.Enabled, _vm, _ => _.ShowBtnEnabled);
            showParentBtn.Bind(_ => _.Enabled, _vm, _ => _.ShowParentBtnEnabled);

            nextBtn.Command = _vm.NextBtnCommand;
            preBtn.Command = _vm.PreBtnCommand;
            moreInfoBtn.Command = _vm.MoreInfoCommand;
            showBtn.Command = _vm.ShowCommand;
            showParentBtn.Command = _vm.ShowParentCommand;

            var group = new GroupBox() { Text = $"Found {_vm.TotalErrorCount} Error(s)" };
            group.Size = new Eto.Drawing.Size(-1, -1);
            var groupLayout = new DynamicLayout();
            groupLayout.DefaultSpacing = new Eto.Drawing.Size(5, 5);
            groupLayout.DefaultPadding = new Eto.Drawing.Padding(5);
            groupLayout.AddSeparateRow(grid);
            groupLayout.AddSeparateRow(preBtn, currentErrorIndex, nextBtn, null, showBtn, showParentBtn, moreInfoBtn);
            groupLayout.AddSeparateRow(message);
            group.Content = groupLayout;

            var OkBtn = new Eto.Forms.Button() { Text = "OK" };
            OkBtn.Click += (s, e) => {
                //if (_vm.Validate())
                //{
                //    var lg = _vm.GetLegend();
                //    this.Close(lg);
                //}
               
            };
            var abortButton = new Eto.Forms.Button() { Text = "Close", Height = 24 };
            abortButton.Click += (s, e) => { this.Close(); };

            var layout = new Eto.Forms.DynamicLayout();
            layout.DefaultSpacing = new Eto.Drawing.Size(5, 2);
            layout.DefaultPadding = new Eto.Drawing.Padding(4);
            layout.AddSeparateRow(controls: new[] { group }, xscale: true, yscale: true);
            layout.AddSeparateRow(null, abortButton);
            this.Content = layout;

            _vm.UILoaded();

        }

        public void ShowModal(Eto.Forms.Window owner)
        {

            this.Owner = owner;

            var c = owner.Bounds.Center;
            c.X = c.X - this.Width / 2;
            c.Y = c.Y - 200;
            this.Location = c;
            this.Show();
        }

        private TreeGridView GenGridView()
        {
            var gd = new TreeGridView();
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => { 
                _vm.SelectedError = gd.SelectedItem as ErrorData;
            };

            gd.ShowHeader = false;
            gd.Height = 250;
            
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ErrorData, string>(r => r.DisplayMessage) },
                HeaderText = "Error Message"
                
            });


            return gd;
        }


    }

}
