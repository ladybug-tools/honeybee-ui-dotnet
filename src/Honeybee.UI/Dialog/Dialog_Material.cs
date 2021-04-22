using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;

namespace Honeybee.UI
{

    public class Dialog_Material : Dialog<HB.Energy.IMaterial>
    {
        //private List<string> _layers = new List<string>();
        private HB.Energy.IMaterial _hbObj;
        private DynamicLayout _materialPanel;


        public Dialog_Material(HB.Energy.IMaterial material)
        {
            try
            {
                //_hbObj = HB.ModelEnergyProperties.Default.Materials.First(_ => _.Obj is HB.EnergyWindowMaterialGas).Obj as HB.EnergyWindowMaterialGas;
                _hbObj = material;

                Padding = new Padding(10);
                Resizable = true;
                Title = $"Material - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 250);
                this.Icon = DialogHelper.HoneybeeIcon;

                var OkButton = new Button { Text = "OK" };
                OkButton.Click += (sender, e) => Close(_hbObj);

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
                name.TextBinding.Bind(() => _hbObj.DisplayName, (v) => _hbObj.DisplayName = v);
                leftLayout.AddRow(name);

                //_layers = _hbObj.Layers;
                var i = 0;
                var font = Fonts.Sans(8);
                var brush = Brushes.Black;


                //var group = new GroupBox();
                //var groupContent = new DynamicLayout();
                //groupContent.DefaultSpacing = new Size(5, 5);
           
                _materialPanel = new DynamicLayout();

                var properties = _hbObj.GetType().GetProperties();
                foreach (var item in properties)
                {
                    if (item.Name == "Identifier" || item.Name == "Type" || item.Name == "DisplayName")
                        continue;

                    var value = item.GetValue(_hbObj);
                    var type = item.PropertyType;
                    if (value is string stringvalue)
                    {
                        var textBox = new TextBox();
                        //textBox.Text = stringvalue;
                        textBox.TextBinding.Bind(() => stringvalue, (v) => item.SetValue(_hbObj, v));
                        _materialPanel.AddRow(item.Name);
                        _materialPanel.AddRow(textBox);
                    }
                    else if (value is double numberValue)
                    {
                        var numberTB = new MaskedTextBox();
                        numberTB.Provider = new NumericMaskedTextProvider() { AllowDecimal=true };
                        numberTB.TextBinding.Bind(() => numberValue.ToString(), (v)=> item.SetValue(_hbObj, Convert.ChangeType(v, type)));
                        _materialPanel.AddRow(item.Name);
                        _materialPanel.AddRow(numberTB);
                    }
                    else if (value is int intValue)
                    {
                        var numberTB = new NumericStepper();
                        numberTB.DecimalPlaces = 0;
                        numberTB.Value = intValue;
                        _materialPanel.AddRow(item.Name);
                        _materialPanel.AddRow(numberTB);
                    }
                    else if (Nullable.GetUnderlyingType(type) != null)
                    {
                        var enumType = Nullable.GetUnderlyingType(type);
                        if (!enumType.IsEnum)
                        {
                            continue;
                        }
                        var values = Enum.GetNames(enumType).ToList();
                        var dropdown = new DropDown();
                        var dropDownItems = values.Select(_ => new ListItem() { Text = _, Key = _ });
                        dropdown.Items.AddRange(dropDownItems);
                       
                        var currentValue = item.GetValue(_hbObj, null).ToString();
                        dropdown.SelectedKeyBinding.Bind(
                            () => currentValue, 
                            (v) => item.SetValue(_hbObj, Enum.Parse(enumType, v))
                            );

                        _materialPanel.AddRow(item.Name);
                        _materialPanel.AddRow(dropdown);


                    }
                    else if (type.IsEnum)
                    {
                        var values = Enum.GetNames(type).ToList();
                        var dropdown = new DropDown();
                        var dropDownItems = values.Select(_ => new ListItem() { Text = _, Key = _ });
                        dropdown.Items.AddRange(dropDownItems);

                        var currentValue = item.GetValue(_hbObj, null).ToString();
                        dropdown.SelectedKeyBinding.Bind(
                            () => currentValue,
                            (v) => item.SetValue(_hbObj, Enum.Parse(type, v))
                            );

                        _materialPanel.AddRow(item.Name);
                        _materialPanel.AddRow(dropdown);

                    }
                }
              
                leftLayout.AddRow(_materialPanel);

                var buttonSource = new Button { Text = "Schema Data" };
                buttonSource.Click += (s, e) =>
                {
                    Dialog_Message.Show(this, _hbObj.ToJson(true));
                };
              

                leftLayout.AddRow(null);
                leftLayout.AddRow(buttonSource);
                leftLayout.AddRow(null);

      

                #region Right Panel
        
                ////Right panel
                //var rightGroup = new GroupBox();
                //rightGroup.Text = "Library";
                //var groupPanel = new DynamicLayout();


                //var materialType = new DropDown();
                //materialType.Items.Add(new ListItem() { Key = "Opaque", Text = "Opaque Material" });
                //materialType.Items.Add(new ListItem() { Key = "Window", Text = "Window Material" });
                ////constructionTypes.Items.Add(new ListItem() { Key = "Shade Material" });
                ////constructionTypes.Items.Add(new ListItem() { Key = "AirBoundary Material" });
                //materialType.SelectedIndex = 0;
                //groupPanel.AddRow(materialType);

                ////Search tbox
                //var searchTBox = new TextBox() { PlaceholderText = "Search" };
                //groupPanel.AddRow(searchTBox);

                //// Library
                //var lib = new ListBox();
                //lib.Height = 300;
                //groupPanel.AddRow(lib);
                //var allMaterials = OpaqueMaterials;

                //// material details
                //var detailPanel = new DynamicLayout();
                //var materialDetail = new ListBox();
                //materialDetail.Height = 150;
                //materialDetail.Items.Add(new ListItem() { Text = "Material Details" });
                ////groupPanel.AddRow(materialDetail);

                //var rightSplit = new Splitter();
                //rightSplit.Panel1 = groupPanel;
                //rightSplit.Panel2 = materialDetail;
                //rightSplit.Panel1MinimumSize = 300;
                //rightSplit.Orientation = Orientation.Vertical;

                //rightGroup.Content = rightSplit;


                //materialType.SelectedIndexChanged += (sender, e) =>
                //{
                //    var selectedType = materialType.SelectedKey;

                //    if (selectedType == "Window")
                //    {
                //        allMaterials = this.WindowMaterials;
                //    }
                //    else
                //    {
                //        allMaterials = this.OpaqueMaterials;
                //    }
                //    searchTBox.Text = null;
                //    lib.Items.Clear();

                //    var filteredItems = allMaterials.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                //    lib.Items.AddRange(filteredItems);

                //};


                
                //var allMaterialItems = allMaterials.Select(_ => new ListItem() { Text = _.DisplayName??_.Identifier, Key = _.DisplayName ?? _.Identifier, Tag = _ });
                //lib.Items.AddRange(allMaterialItems);
                //lib.MouseMove += (sender, e) =>
                //{
                //    var dragableArea = lib.Bounds;
                //    dragableArea.Width -= 20;
                //    dragableArea.Height -= 20;
                //    var iscontained = e.Location.Y < dragableArea.Height  && e.Location.X < dragableArea.Width;
                //    //name.Text = $"{dragableArea.Width}x{dragableArea.Height}, {new Point(e.Location).X}:{new Point(e.Location).Y}, {dragableArea.Contains(new Point(e.Location))}";
                //    if (!iscontained)
                //        return; 

                //    if (e.Buttons == MouseButtons.Primary && lib.SelectedIndex != -1)
                //    {
                //        var selected = lib.SelectedKey;
                //        var data = new DataObject();
                //        data.SetString(selected, "Material");
                //        lib.DoDragDrop(data, DragEffects.Move);
                //        e.Handled = true;
                //    }
                //};

                //lib.SelectedIndexChanged += (s, e) => 
                //{
                //    if (lib.SelectedIndex == -1)
                //    {
                //        materialDetail.Items.Clear();
                //        return;
                //    }

                //    var selectedItem = (lib.Items[lib.SelectedIndex] as ListItem).Tag as HB.HoneybeeObject;
                //    var layers = new List<string>();
                    
                //    var layersItems = selectedItem.ToString(true).Split('\n').Select(_ => new ListItem() { Text = _ });
                //    materialDetail.Items.Clear();
                //    materialDetail.Items.AddRange(layersItems);



                //};

               
                //searchTBox.TextChanged += (sender, e) =>
                //{
                //    var input = searchTBox.Text;
                //    materialDetail.Items.Clear();
                //    lib.Items.Clear();
                //    if (string.IsNullOrWhiteSpace(input))
                //    {
                //        lib.Items.AddRange(allMaterialItems);
                //        return;
                //    }
                //    var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                //    var filtered = allMaterials.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null ? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
                //    var filteredItems = filtered.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Key = _.DisplayName ?? _.Identifier, Tag = _ });
                //    lib.Items.AddRange(filteredItems);

                //};


                #endregion


                //var split = new Splitter();
                //split.Orientation = Orientation.Horizontal;
                //split.Panel1 = leftLayout;
                //split.Panel2 = rightGroup;

                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(5);
                layout.AddRow(leftLayout);
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

        //private void GenMaterialLayersPanel(DynamicLayout layout, Action<int, string> actionAfterChanged)
        //{
        //    _materialPanel.Clear();
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
        //                _materialPanel.Create();

        //            });
        //        _materialPanel.AddRow(dropin);

        //        index++;
        //    }
        //    //return ctrls;

        //    //GenItems();
        //    ////return ctrls;

        //    //void GenItems()
        //    //{
        //    //    _layersPanel.Clear();
        //    //    var index = 0;
        //    //    foreach (var item in _layers)
        //    //    {
        //    //        var id = Guid.NewGuid();
        //    //        var dropin = GenDropInArea(index, item, actionAfterChanged,
        //    //            (i) => {
        //    //                _layers.RemoveAt(i);
        //    //                ////ctrls.Detach();
        //    //                //ctrls.Clear();
        //    //                GenMaterialLayersPanel(layout, actionAfterChanged);
        //    //                //foreach (var c in newPanel.Controls)
        //    //                //{
        //    //                //    ctrlsLayers.AddRow(c);
        //    //                //}

        //    //                //ctrls.AddRow(new TextBox());
        //    //                ctrls.Create();

        //    //            });
        //    //        ctrls.AddRow(dropin);

        //    //        index++;
        //    //    }
        //    //    //return ctrls;
        //    //}
        
        //    //ctrls.EndGroup();
        //}
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

       

    }
}
