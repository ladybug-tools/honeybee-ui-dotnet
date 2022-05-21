using Eto.Forms;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Honeybee.UI
{
    public class Dialog_Error : Eto.Forms.Form
    {
        private ErrorViewModel _vm;
        private static Dialog_Error _instance;
        private TreeGridView _grid;

        public static Dialog_Error Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = new Dialog_Error(null);
                return _instance; 
            }
        }

        public void Update(
            HoneybeeSchema.ValidationReport report, 
            Action<IEnumerable<HoneybeeSchema.ValidationError>, bool> showAction = default,
            Action reRunValidation = default)
        {
            this._vm.Update(report, showAction, reRunValidation);
            this._vm.UILoaded();
            this._grid?.ReloadData();
        }

        private Dialog_Error(HoneybeeSchema.ValidationReport report)
        {
            this.Title = $"Validation Report - {DialogHelper.PluginName}";
            this.Width = 800;
            this.Icon = DialogHelper.HoneybeeIcon;

            _vm = new ErrorViewModel(report, this);

            _grid = GenGridView();
            var nextBtn = new Button() { Text = ">>"};
            var currentErrorIndex = new Label();
            var preBtn = new Button() { Text = "<<" };
            var showBtn = new Button() { Text = "Show" };
            var showParentBtn = new Button() { Text = "Show Parent" };
            var moreInfoBtn = new Button() { Text = "More Info" };
            
            var message = new TextArea() { Height = 84 };


            message.TextBinding.Bind(_vm, _=>_.CurrentErrorMessage);
            currentErrorIndex.TextBinding.Bind(_vm, _=>_.CurrentErrorIndex);
            nextBtn.Bind(_ => _.Enabled, _vm, _ => _.NextBtnEnabled);
            preBtn.Bind(_ => _.Enabled, _vm, _ => _.PreBtnEnabled);
            moreInfoBtn.Bind(_ => _.Enabled, _vm, _ => _.MoreBtnEnabled);

            showBtn.Bind(_ => _.Enabled, _vm, _ => _.ShowBtnEnabled);
            showParentBtn.Bind(_ => _.Enabled, _vm, _ => _.ShowParentBtnEnabled);

            nextBtn.Command = _vm.NextBtnCommand;
            preBtn.Command = _vm.PreBtnCommand;
            moreInfoBtn.Command = _vm.ErrorLinkCommand;
            showBtn.Command = _vm.ShowCommand;
            showParentBtn.Command = _vm.ShowParentCommand;
            //errorLink.Command = _vm.ErrorLinkCommand;


            var group = new GroupBox();
            group.Bind(_ => _.Text, _vm, _ => _.TotalErrorMessage);

            group.Size = new Eto.Drawing.Size(-1, -1);
            var groupLayout = new DynamicLayout();
            groupLayout.DefaultSpacing = new Eto.Drawing.Size(5, 5);
            groupLayout.DefaultPadding = new Eto.Drawing.Padding(5);
            groupLayout.AddSeparateRow(_grid);
            groupLayout.AddSeparateRow(preBtn, currentErrorIndex, nextBtn, null, showBtn, showParentBtn, moreInfoBtn);
            groupLayout.AddSeparateRow(message);
            group.Content = groupLayout;

            var validateBtn = new Eto.Forms.Button() { Text = "Re-Validate" };
            validateBtn.Bind(_=>_.Visible, _vm, _ => _.ValidateBtnEnabled);
            validateBtn.Command = _vm.ValidateCommand;
           
            var abortButton = new Eto.Forms.Button() { Text = "Close", Height = 24 };
            abortButton.Click += (s, e) => { this.Close(); };

            var layout = new Eto.Forms.DynamicLayout();
            layout.DefaultSpacing = new Eto.Drawing.Size(5, 2);
            layout.DefaultPadding = new Eto.Drawing.Padding(4);
            layout.AddSeparateRow(controls: new[] { group }, xscale: true, yscale: true);
            layout.AddSeparateRow(null, validateBtn, abortButton);
            this.Content = layout;

            _vm.UILoaded();

        }

        public void ShowModal(Eto.Forms.Window owner)
        {
            if (!this.Loaded)
            {
                this.Owner = owner;

                var c = owner.Bounds.Center;
                c.X = c.X - this.Width / 2;
                c.Y = c.Y - 200;
                this.Location = c;
            }
            
            this.Show();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance?.Dispose();
            _instance = null;
        }

        private TreeGridView GenGridView()
        {
            var gd = new TreeGridView();
            gd.AllowMultipleSelection = true;
            gd.Bind(_ => _.DataStore, _vm, _ => _.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedErrors = gd.SelectedItems?.OfType<ErrorData>()?.ToList();
            };

            gd.ShowHeader = false;
            gd.Height = 250;
            
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ErrorData, string>(r => r.DisplayMessage) },
                HeaderText = "Error Message"
                
            });

            gd.CellDoubleClick += (s, e) =>
            {
                var data = e.Item as ErrorData;
                if (data != null && data.IsParent)
                {
                    data.Expanded = !data.Expanded;
                    gd.ReloadData();
                }

            };


            return gd;
        }

        public void ReloadGrid()
        {
            this._grid.ReloadData();
        }

        public void SelectGridItem(ErrorData error)
        {
            this._grid.SelectedItem = error;
        }

        public void MoveToNext()
        {
            try
            {
                var current = _grid.SelectedRow;
                var currentItem = _grid.SelectedItem as ErrorData;
                if (current == -1 || currentItem == null)
                {
                    current = 0;
                    currentItem = _vm.GridViewDataCollection.FirstOrDefault() as ErrorData;
                }

                if (currentItem.IsParent && currentItem.Expandable)
                {
                    if (!currentItem.Expanded)
                    {
                        currentItem.Expanded = true;
                        _grid.ReloadData();
                    }
                }

                _grid.UnselectAll();
                var newRow = current + 1;
                _grid.SelectRow(newRow);
                _grid.ScrollToRow(newRow);

            }
            catch (Exception)
            {
                //throw;
            }
         
        }

        public void MoveToPrevious()
        {
            try
            {
                var current = _grid.SelectedRow;
                var currentItem = _grid.SelectedItem as ErrorData;
                if (current <= 0 || currentItem == null)
                {
                    return;
                }

                //get previous item
                _grid.UnselectAll();
                var newRow = current - 1;
                _grid.SelectRow(newRow);
                var preItem = _grid.SelectedItem as ErrorData;
                if (preItem.IsParent && preItem.Expandable)
                {
                    if (!preItem.Expanded)
                    {
                        preItem.Expanded = true;
                        _grid.ReloadData();
                        _grid.SelectedItem = preItem.Children.Last();
                    }

                }

                if (_grid.SelectedRow >= 0)
                {
                    _grid.ScrollToRow(_grid.SelectedRow);
                }
                //_grid.ScrollToRow(newRow);

            }
            catch (Exception)
            {
                //throw;
            }
        }
    }

}
