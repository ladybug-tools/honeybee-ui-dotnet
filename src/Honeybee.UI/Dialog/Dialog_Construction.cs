using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using HoneybeeSchema;
using Newtonsoft.Json.Converters;

namespace Honeybee.UI
{
    public class Dialog_Construction : Dialog
    {
        //private List<string> _layers = new List<string>();
        private OpaqueConstructionAbridged _hbObj;
        private DynamicLayout _layersPanel;
  

        public List<string> _layers
        {
            get { return _hbObj.Layers; }
            set { _hbObj.Layers = value; }
        }

        public Dialog_Construction()
        {
            try
            {
                _hbObj = HB.ModelEnergyProperties.Default.Constructions.First(_ => _.Obj is HB.OpaqueConstructionAbridged).Obj as HB.OpaqueConstructionAbridged;


                Padding = new Padding(5);
                Resizable = true;
                Title = "Schedule - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 650);
                this.Icon = DialogHelper.HoneybeeIcon;

                var OkButton = new Button { Text = "OK" };
                OkButton.Click += (sender, e) => Close();

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                //var panel = new Panel_Schedule(sch);
                //panel.AddSeparateRow(null, OkButton, AbortButton, null);
                //Content = panel;

                var leftLayout = new DynamicLayout();
                leftLayout.DefaultSpacing = new Size(5, 5);
                leftLayout.DefaultPadding = new Padding(10, 5);

                leftLayout.AddRow("Name");
                var name = new TextBox();
                name.Text = _hbObj.DisplayName ?? _hbObj.Identifier;
                leftLayout.AddRow(name);

                //_layers = _hbObj.Layers;
                var i = 0;
                var font = Fonts.Sans(8);
                var brush = Brushes.Black;


                //var group = new GroupBox();
                var groupContent = new DynamicLayout();
                groupContent.DefaultSpacing = new Size(5, 5);
                Action<int, string> actionWhenItemChanged = (int layerIndex, string newValue) => {
                    _layers.RemoveAt(layerIndex);
                    _layers.Insert(layerIndex, newValue);
                };

                _layersPanel = new DynamicLayout();
                _layersPanel.DefaultSpacing = new Size(5, 5);
                GenMaterialLayersPanel(_layersPanel, actionWhenItemChanged);
                var fromLibDropin = GenAddMoreDropInArea(
                  (newValue) =>
                  {
                      var newIndex = _layers.Count;
                      _layers.Add(newValue);
                      GenMaterialLayersPanel(_layersPanel, actionWhenItemChanged);
                      //var newPanel = GenMaterialLayersPanel(_layers, actionWhenItemChanged);
                      //var newLayer = GenDropInArea(newIndex, newValue, actionWhenItemChanged, (index) => { _layers.RemoveAt(index); groupPanel.Create(); });
                      ////groupPanel.Content = newPanel;
                      //_layersPanel.AddRow(newLayer);
                      _layersPanel.Create();

                  }
                  );
                groupContent.AddSeparateRow(null, "Outside", null);
                groupContent.AddRow(_layersPanel);
                groupContent.AddRow(fromLibDropin);
                groupContent.AddSeparateRow(null, "Inside", null);
                //group.Content = groupContent;
                leftLayout.AddRow(groupContent);

                var buttonSource = new Button { Text = "HBData" };
                buttonSource.Click += (s, e) =>
                {
                    //var c = construction;
                    //c.Layers = _layers;
                    Dialog_Message.Show(this, _hbObj.ToJson());

                };
                buttonSource.Click += (s, e) =>
                {
                    ////var newgroupPanel = GenMaterialLayersPanel(actionWhenItemChanged);
                    //var ctrl = groupPanel.Controls.First();
                    //////groupPanel.RemoveAll();
                    //groupPanel.Clear();
                    ////groupPanel.Create();
                    //groupPanel.AddRow(ctrl);
                    ////var newCtrl = GenDropInArea(1, "ddddd", null, null);
                    ////foreach (var item in newgroupPanel.Controls)
                    ////{
                     
                    ////    groupPanel.AddRow(item);
                    ////}
                    
                   
                    //groupPanel.Create();

                };

                //buttonSource.MouseDown += (sender, e) =>
                //{
                //    if (e.Buttons != MouseButtons.None)
                //    {
                //        var selected = "dddddd";
                //        var data = new DataObject();
                //        data.SetString(selected, "Material");
                //        buttonSource.DoDragDrop(data, DragEffects.Move);
                //        e.Handled = true;
                //    }
                //};
                //buttonSource.Click += (s, e) => 
                //{
                //    var box = new TextBox();
                //    leftLayout.AddRow(box);
                //    leftLayout.Create();
                //};
                //buttonSource.Click += (s, e) =>
                //{
                //    //var box = new TextBox();
                //    //leftLayout.AddRow(box);
                //    //leftLayout.Create();
                //};
                leftLayout.AddRow(null);

                var lib = new ListBox();
                lib.Height = 200;
                var materialLibrary = HB.Helper.EnergyLibrary.StandardsOpaqueMaterials;
                var materialItems = materialLibrary.Select(_ => new ListItem() { Text = _.DisplayName??_.Identifier });
                lib.Items.AddRange(materialItems);
                var bound = lib.Bounds;
                lib.MouseMove += (sender, e) =>
                {
                    var dragableArea = lib.Bounds;
                    dragableArea.Width -= 20;
                    dragableArea.Height -= 20;
                    var iscontained = e.Location.Y < dragableArea.Height  && e.Location.X < dragableArea.Width;
                    //name.Text = $"{dragableArea.Width}x{dragableArea.Height}, {new Point(e.Location).X}:{new Point(e.Location).Y}, {dragableArea.Contains(new Point(e.Location))}";
                    if (!iscontained)
                        return; 

                    if (e.Buttons == MouseButtons.Primary && lib.SelectedIndex != -1)
                    {
                        var selected = lib.SelectedKey;
                        var data = new DataObject();
                        data.SetString(selected, "Material");
                        lib.DoDragDrop(data, DragEffects.Move);
                        e.Handled = true;
                    }
                };

                leftLayout.AddRow(buttonSource);
                leftLayout.AddRow(null);
                var layout = new DynamicLayout();
                layout.AddRow(leftLayout, lib);
           
                

                //Create layout
                Content = layout;

            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }

        private void GenMaterialLayersPanel(DynamicLayout layout, Action<int, string> actionAfterChanged)
        {
            _layersPanel.Clear();
            var index = 0;
            foreach (var item in _layers)
            {
                var id = Guid.NewGuid();
                var dropin = GenDropInArea(index, item, actionAfterChanged,
                    (i) => {
                        _layers.RemoveAt(i);
                        ////ctrls.Detach();
                        //ctrls.Clear();
                        GenMaterialLayersPanel(layout, actionAfterChanged);
                        //foreach (var c in newPanel.Controls)
                        //{
                        //    ctrlsLayers.AddRow(c);
                        //}

                        //ctrls.AddRow(new TextBox());
                        _layersPanel.Create();

                    });
                _layersPanel.AddRow(dropin);

                index++;
            }
            //return ctrls;

            //GenItems();
            ////return ctrls;

            //void GenItems()
            //{
            //    _layersPanel.Clear();
            //    var index = 0;
            //    foreach (var item in _layers)
            //    {
            //        var id = Guid.NewGuid();
            //        var dropin = GenDropInArea(index, item, actionAfterChanged,
            //            (i) => {
            //                _layers.RemoveAt(i);
            //                ////ctrls.Detach();
            //                //ctrls.Clear();
            //                GenMaterialLayersPanel(layout, actionAfterChanged);
            //                //foreach (var c in newPanel.Controls)
            //                //{
            //                //    ctrlsLayers.AddRow(c);
            //                //}

            //                //ctrls.AddRow(new TextBox());
            //                ctrls.Create();

            //            });
            //        ctrls.AddRow(dropin);

            //        index++;
            //    }
            //    //return ctrls;
            //}
        
            //ctrls.EndGroup();
        }
        private Control GenDropInArea(int layerIndex, string text, Action<int, string> actionAfterChanged, Action<int> deleteAction)
        {
            var width = 300;
            var height = 36;

            var layerPanel = new PixelLayout();

            var backgroundLayer = new Label();
            backgroundLayer.Width = width;
            backgroundLayer.Height = height;
            backgroundLayer.BackgroundColor = Colors.White;


            var dropInValue = new Label();
            dropInValue.Text = text ?? "Drag from library";
            dropInValue.TextAlignment = TextAlignment.Center;
            dropInValue.VerticalAlignment = VerticalAlignment.Center;

            dropInValue.Width = width-50;
            dropInValue.Height = height;
            var backGround = string.IsNullOrEmpty(text) ? Color.FromArgb(230, 230, 230) : Colors.White;
            dropInValue.BackgroundColor = Colors.Transparent;

            



            var dropIn = new Drawable();
            dropIn.AllowDrop = true;
            dropIn.Width = width;
            dropIn.Height = height;
            dropIn.BackgroundColor = Colors.Transparent;

            var deleteBtn = new Button();
            deleteBtn.Text = "✕";
            deleteBtn.Width = 20;
            deleteBtn.Height = 20;
            deleteBtn.Click += (s, e) => deleteAction(layerIndex);
            //deleteBtn.Visible = false;

            //dropIn.MouseMove += (sender, e) =>
            //{
            //    deleteBtn.Visible = true;
            //    //dropInValue.BackgroundColor = Colors.White;
            //};
            //dropIn.MouseLeave += (sender, e) =>
            //{
            //    deleteBtn.Visible = false;
            //};
            //dropIn.Paint += (s, e) =>
            //{
            //    e.Graphics.DrawText(font, brush, new PointF(0, 0), item);
            //};
            //dropIn.DragEnter += (sender, e) =>
            //{
            //    //if (e.Effects != DragEffects.None)
            //    dropIn.BackgroundColor = Colors.Green;
            //};
            dropIn.DragLeave += (sender, e) =>
            {
                backgroundLayer.BackgroundColor = Colors.White;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                backgroundLayer.BackgroundColor = Color.FromArgb(230, 230, 230);

            };
            dropIn.DragDrop += (sender, e) =>
            {
                //if (e.Effects == DragEffects.None)
                //    return;
                //e.Effects = DragEffects.All;
                //dropIn.BackgroundColor = Colors.Red;
                var newValue = e.Data.GetString("Material");
                dropInValue.Text = newValue;
                actionAfterChanged(layerIndex, newValue);

            };
            layerPanel.Add(backgroundLayer, 0, 0);
            layerPanel.Add(dropInValue, 15, 0);
            layerPanel.Add(dropIn, 0, 0);
            layerPanel.Add(deleteBtn, width - 26, 8);
            return layerPanel;
        }
        private Control GenAddMoreDropInArea(Action<string> actionAfterNewDroppedIn)
        {
            var width = 300;
            var height = 60;

            var layerPanel = new PixelLayout();
            var dropInValue = new Label();
            dropInValue.Text = "Drag a new material from library";
            dropInValue.TextColor = Colors.DimGray;
            dropInValue.TextAlignment = TextAlignment.Center;
            dropInValue.VerticalAlignment = VerticalAlignment.Center;

            dropInValue.Width = width;
            dropInValue.Height = height;
            var backGround =  Color.FromArgb(230, 230, 230);
            dropInValue.BackgroundColor = backGround;

            var dropIn = new Drawable();
            dropIn.AllowDrop = true;
            dropIn.Width = width;
            dropIn.Height = height;
            dropIn.BackgroundColor = Colors.Transparent;

            dropIn.DragLeave += (sender, e) =>
            {

                dropInValue.BackgroundColor = backGround;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                dropInValue.BackgroundColor = Colors.Yellow;
            };
            dropIn.DragDrop += (sender, e) =>
            {
              
                var newValue = e.Data.GetString("Material");
                //dropInValue.Text = newValue;
                actionAfterNewDroppedIn(newValue);

            };
            layerPanel.Add(dropInValue, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            return layerPanel;
        }

        private GridView GenGridView(IEnumerable<object> items)
        {
            var gd = new GridView() { DataStore = items };
            gd.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ScheduleRulesetAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ScheduleRulesetAbridged, string>(r => r.ScheduleTypeLimit)
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type" });
            return gd;
        }

    }
}
