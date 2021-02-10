using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HoneybeeSchema;

namespace Honeybee.UI
{

    public class Dialog_Construction : Dialog<HB.Energy.IConstruction>
    {
        //private List<string> _layers = new List<string>();
        private HB.Energy.IConstruction _hbObj;
        private DynamicLayout _layersPanel;

        public List<string> _layers
        {
            get {
                if (_hbObj is HB.OpaqueConstructionAbridged obj)
                {
                    return obj.Layers;
                }
                else if (_hbObj is HB.WindowConstructionAbridged win)
                {
                    return win.Layers;
                }
                return new List<string>();
              
            }
            set 
            {
                if (_hbObj is HB.OpaqueConstructionAbridged obj)
                {
                    obj.Layers = value;
                }
                else if (_hbObj is HB.WindowConstructionAbridged win)
                {
                    win.Layers = value;
                }
                
            }
        }

        private string MakeUserReadable(string materialId)
        {
            IEnumerable<HB.Energy.IMaterial> searchBase = OpaqueMaterials;

            if (_hbObj is HB.OpaqueConstructionAbridged obj)
            {
                // do nothing..
            }
            else if (_hbObj is HB.WindowConstructionAbridged win)
            {
                searchBase = WindowMaterials;
            }

            var found = searchBase.FirstOrDefault(_ => _.Identifier == materialId);
            if (found == null)
                return materialId;
            return found.DisplayName ?? found.Identifier;
        }

       
        private static IEnumerable<HB.Energy.IMaterial> _opaqueMaterials;

        public IEnumerable<HB.Energy.IMaterial> OpaqueMaterials
        {
            get {

                var libObjs = HB.Helper.EnergyLibrary.StandardsOpaqueMaterials.Values.ToList();
                var inModelObjs =  this.ModelEnergyProperties.Materials
                    .Where(_ => !_.Obj.GetType().Name.Contains("EnergyWindow"))
                    .Select(_=>_.Obj as HB.Energy.IMaterial);

                libObjs.AddRange(inModelObjs);
                _opaqueMaterials = libObjs;

                return _opaqueMaterials; 
            }
        }

        private static IEnumerable<HB.Energy.IMaterial> _windowMaterials;
        public IEnumerable<HB.Energy.IMaterial> WindowMaterials
        {
            get
            {
                var libObjs = HB.Helper.EnergyLibrary.StandardsWindowMaterials.Values.ToList();
                var inModelObjs =  this.ModelEnergyProperties.Materials
                    .Where(_ => _.Obj.GetType().Name.Contains("EnergyWindow"))
                    .Select(_ => _.Obj as HB.Energy.IMaterial);

                libObjs.AddRange(inModelObjs);
                _windowMaterials = libObjs;

                return _windowMaterials;
            }
        }

        //private bool _primMousePressed = false;
        private HB.ModelEnergyProperties ModelEnergyProperties { get; set; }

        public Dialog_Construction(HB.ModelEnergyProperties libSource, HB.Energy.IConstruction construction)
        {
            try
            {
                this.ModelEnergyProperties = libSource;

                _hbObj = construction;

                Padding = new Padding(5);
                Resizable = true;
                Title = "Construction - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 500);
                this.Icon = DialogHelper.HoneybeeIcon;

                var OkButton = new Button { Text = "OK" };
                OkButton.Click += (sender, e) =>
                {
                    // add materials to the lib
                    foreach (var layer in _layers)
                    {
                        var mat = _opaqueMaterials.FirstOrDefault(_ => _.Identifier == layer);
                        mat = mat ?? _windowMaterials.FirstOrDefault(_ => _.Identifier == layer);
                        mat.Identifier = Guid.NewGuid().ToString();
                        libSource.AddMaterial(mat);
                    }
                    Close(_hbObj);
                };

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
                _hbObj.DisplayName = _hbObj.DisplayName ?? _hbObj.Identifier;
                name.TextBinding.Bind(() => _hbObj.DisplayName, v => _hbObj.DisplayName = v);
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
                      // add new material to library source
                      HB.Energy.IMaterial m = null;
                      if (_hbObj is HB.OpaqueConstructionAbridged)
                          m = _opaqueMaterials.FirstOrDefault(_ => _.Identifier == newValue);
                      else if (_hbObj is HB.WindowConstructionAbridged)
                          m = _windowMaterials.FirstOrDefault(_ => _.Identifier == newValue);
                      if (null != m)
                          libSource.AddMaterial(m);


                      GenMaterialLayersPanel(_layersPanel, actionWhenItemChanged);
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

                var allMaterials = OpaqueMaterials;

                // Library
                var lib = new GridView();
                lib.Height = 300;
                groupPanel.AddRow(lib);

                lib.ShowHeader = false;
                var nameCol = new GridColumn() { DataCell = new TextBoxCell(0) };
                lib.Columns.Add(nameCol);

                // Library items
                lib.DataStore = allMaterials;

               
                var idCell = new TextBoxCell
                {
                    Binding = Binding.Delegate<HB.Energy.IIDdEnergyBaseModel, string>(r => r.DisplayName ?? r.Identifier)
                };
                lib.Columns.Add(new GridColumn() { DataCell = idCell });


                

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
                    //lib.Items.Clear();

                    lib.DataStore = allMaterials;
                    //var filteredItems = allMaterials.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                    //lib.Items.AddRange(filteredItems);

                };



                //// need this to make drag drop work on Mac
                //lib.MouseDown += (sender, e) => {
                //    _primMousePressed = e.Buttons == MouseButtons.Primary;
                //};
                //lib.MouseUp += (sender, e) => {
                //    _primMousePressed = false;

                //};
                
                lib.MouseMove += (sender, e) =>
                {
                    if (e.Buttons != MouseButtons.Primary)
                        return;

                    var dragableArea = lib.Bounds;
                    dragableArea.Width -= 20;
                    dragableArea.Height -= 20;
                    var iscontained = e.Location.Y < dragableArea.Height  && e.Location.X < dragableArea.Width;
                    //name.Text = $"{dragableArea.Width}x{dragableArea.Height}, {new Point(e.Location).X}:{new Point(e.Location).Y}, {dragableArea.Contains(new Point(e.Location))}";
                    if (!iscontained)
                        return;


                    var cell = lib.GetCellAt(e.Location);
                    if (cell.RowIndex == -1 || cell.ColumnIndex == -1)
                        return;

                    var selected = (lib.SelectedItem as HB.Energy.IMaterial).Identifier;
                    var data = new DataObject();
                    data.SetString(selected, "HBObj");
                    lib.DoDragDrop(data, DragEffects.Move);
                    e.Handled = true;
                };

                lib.SelectedItemsChanged += (s, e) => 
                {
                    //Clear preview first
                    materialDetail.Items.Clear();

                    //Check current selected item from library
                    var selItem = lib.SelectedItem as HB.HoneybeeObject;
                    if (selItem == null)
                        return;

                    //Update Preview
                    var layersItems = selItem.ToString(true).Split('\n').Select(_ => new ListItem() { Text = _ });
                    materialDetail.Items.AddRange(layersItems);

                };

               
                searchTBox.TextChanged += (sender, e) =>
                {
                    var input = searchTBox.Text;
                    materialDetail.Items.Clear();
                  
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        lib.DataStore = allMaterials;
                        return;
                    }
                    var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                    var filtered = allMaterials.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null ? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
               
                    lib.DataStore = filtered;

                };

               



                //var split = new Splitter();
                //split.Orientation = Orientation.Horizontal;
                //split.Panel1 = leftLayout;
                //split.Panel2 = rightGroup;

                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(5);
                layout.AddSeparateRow(leftLayout, rightGroup);
                layout.AddSeparateRow(null, OkButton, AbortButton, null);
                layout.AddRow(null);

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

        private int _leftControlsWith = 300;
        private Control GenDropInArea(int layerIndex, string text, Action<int, string> actionAfterChanged, Action<int> deleteAction)
        {
            var width = _leftControlsWith;
            var height = 36;

            var layerPanel = new PixelLayout();


            var dropInValue = new TextBox();
            var materialDisplayText = MakeUserReadable(text);
            dropInValue.Text = materialDisplayText ?? "Drag from library";
            dropInValue.TextAlignment = TextAlignment.Center;
            dropInValue.Width = width;
            dropInValue.Height = height;
            dropInValue.Enabled = false;
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
                dropInValue.BackgroundColor = Colors.Transparent;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                dropInValue.BackgroundColor = Colors.LightGrey;

            };
            dropIn.DragDrop += (sender, e) =>
            {
                //if (e.Effects == DragEffects.None)
                //    return;
                //e.Effects = DragEffects.All;
                //dropIn.BackgroundColor = Colors.Red;
                var newValue = e.Data.GetString("HBObj");
                dropInValue.Text = MakeUserReadable(newValue); ;
                actionAfterChanged(layerIndex, newValue);

            };
            //layerPanel.Add(backgroundLayer, 0, 0);
            layerPanel.Add(dropInValue, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            layerPanel.Add(deleteBtn, width - 26, 8);
            return layerPanel;
        }
        private Control GenAddMoreDropInArea(Action<string> actionAfterNewDroppedIn)
        {
            var width = _leftControlsWith;
            var height = 60;

            var layerPanel = new PixelLayout();
            var dropInValue = new Label();
            dropInValue.Text = "Drag a new material from library";
            dropInValue.TextColor = Colors.DimGray;
            dropInValue.TextAlignment = TextAlignment.Center;
            dropInValue.VerticalAlignment = VerticalAlignment.Center;

            dropInValue.Width = width;
            dropInValue.Height = height;
            var backGround =  Eto.Drawing.Color.FromArgb(230, 230, 230);
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
                dropInValue.BackgroundColor = Colors.LightGrey;
            };
            dropIn.DragDrop += (sender, e) =>
            {
              
                var newValue = e.Data.GetString("HBObj");
                //dropInValue.Text = newValue;
                actionAfterNewDroppedIn(newValue);

            };
            layerPanel.Add(dropInValue, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            return layerPanel;
        }

       

    }
}
