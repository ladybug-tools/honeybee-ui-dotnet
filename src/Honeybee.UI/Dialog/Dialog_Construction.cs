using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using HoneybeeSchema;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;

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

        private static IEnumerable<HB.Energy.IMaterial> _opaqueMaterials;

        public IEnumerable<HB.Energy.IMaterial> OpaqueMaterials
        {
            get {
                if (_opaqueMaterials == null)
                {
                    _opaqueMaterials = HB.Helper.EnergyLibrary.StandardsOpaqueMaterials;
                }
                return _opaqueMaterials; 
            }
        }

        private static IEnumerable<HB.Energy.IMaterial> _windowMaterials;
        public IEnumerable<HB.Energy.IMaterial> WindowMaterials
        {
            get
            {
                if (_windowMaterials == null)
                {
                    _windowMaterials = HB.Helper.EnergyLibrary.StandardsWindowMaterials;
                }
                return _windowMaterials;
            }
        }

        public Dialog_Construction()
        {
            try
            {
                _hbObj = HB.ModelEnergyProperties.Default.Constructions.First(_ => _.Obj is HB.OpaqueConstructionAbridged).Obj as HB.OpaqueConstructionAbridged;


                Padding = new Padding(5);
                Resizable = true;
                Title = "Construction - Honeybee";
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
                    Dialog_Message.Show(this, _hbObj.ToJson());
                };
              

                leftLayout.AddRow(null);
                leftLayout.AddRow(buttonSource);
                leftLayout.AddRow(null);




                //Right panel
                var rightGroup = new GroupBox();
                rightGroup.Text = "Library";
                var groupPanel = new DynamicLayout();


                var materialType = new DropDown();
                materialType.Items.Add(new ListItem() { Key = "Opaque", Text = "Opaque Material" });
                materialType.Items.Add(new ListItem() { Key = "Window", Text = "Window Material" });
                //constructionTypes.Items.Add(new ListItem() { Key = "Shade Material" });
                //constructionTypes.Items.Add(new ListItem() { Key = "AirBoundary Material" });
                materialType.SelectedIndex = 0;
                groupPanel.AddRow(materialType);

                //Search tbox
                var searchTBox = new TextBox() { PlaceholderText = "Search" };
                groupPanel.AddRow(searchTBox);

                // Library
                var lib = new ListBox();
                lib.Height = 200;
                groupPanel.AddRow(lib);
                var allMaterials = OpaqueMaterials;

                // material details
                var detailPanel = new DynamicLayout();
                var materialDetail = new ListBox();
                materialDetail.Height = 150;
                materialDetail.Items.Add(new ListItem() { Text = "Material Details" });
                //groupPanel.AddRow(materialDetail);

                var rightSplit = new Splitter();
                rightSplit.Panel1 = groupPanel;
                rightSplit.Panel2 = materialDetail;
                rightSplit.Panel1MinimumSize = 300;
                rightSplit.Orientation = Orientation.Vertical;

                rightGroup.Content = rightSplit;


                materialType.SelectedIndexChanged += (sender, e) =>
                {
                    var selectedType = materialType.SelectedKey;

                    if (selectedType == "Window")
                    {
                        allMaterials = this.WindowMaterials;
                    }
                    else
                    {
                        allMaterials = this.OpaqueMaterials;
                    }
                    searchTBox.Text = null;
                    lib.Items.Clear();

                    var filteredItems = allMaterials.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                    lib.Items.AddRange(filteredItems);

                };


                
                var allMaterialItems = allMaterials.Select(_ => new ListItem() { Text = _.DisplayName??_.Identifier, Key = _.DisplayName ?? _.Identifier, Tag = _ });
                lib.Items.AddRange(allMaterialItems);
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

                lib.SelectedIndexChanged += (s, e) => 
                {
                    if (lib.SelectedIndex == -1)
                    {
                        materialDetail.Items.Clear();
                        return;
                    }

                    var selectedItem = (lib.Items[lib.SelectedIndex] as ListItem).Tag as HB.HoneybeeObject;
                    var layers = new List<string>();
                    
                    var layersItems = selectedItem.ToString(true).Split('\n').Select(_ => new ListItem() { Text = _ });
                    materialDetail.Items.Clear();
                    materialDetail.Items.AddRange(layersItems);



                };

               
                searchTBox.TextChanged += (sender, e) =>
                {
                    var input = searchTBox.Text;
                    materialDetail.Items.Clear();
                    lib.Items.Clear();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        lib.Items.AddRange(allMaterialItems);
                        return;
                    }
                    var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                    var filtered = allMaterials.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null ? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
                    var filteredItems = filtered.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Key = _.DisplayName ?? _.Identifier, Tag = _ });
                    lib.Items.AddRange(filteredItems);

                };

               



                var split = new Splitter();
                split.Orientation = Orientation.Horizontal;
                split.Panel1 = leftLayout;
                split.Panel2 = rightGroup;

                //var layout = new DynamicLayout();
                //layout.AddRow(leftLayout, lib);
           
                

                //Create layout
                Content = split;

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
