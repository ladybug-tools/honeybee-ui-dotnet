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

                Padding = new Padding(5);
                Resizable = true;
                Title = $"Material - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);
                this.Icon = DialogHelper.HoneybeeIcon;

                var OkButton = new Button { Text = "OK" };
                OkButton.Click += (sender, e) => Close(_hbObj);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                //var panel = new Panel_Schedule(sch);
                //panel.AddSeparateRow(null, OkButton, AbortButton, null);
                //Content = panel;

                var layout = new DynamicLayout();
                layout.DefaultSpacing = new Size(5, 5);
                layout.DefaultPadding = new Padding(5);

        

                //_layers = _hbObj.Layers;
                var i = 0;
                var font = Fonts.Sans(8);
                var brush = Brushes.Black;


                //var group = new GroupBox();
                //var groupContent = new DynamicLayout();
                //groupContent.DefaultSpacing = new Size(5, 5);
           
                _materialPanel = new DynamicLayout();
                _materialPanel.DefaultSpacing = new Size(5, 5);
                _materialPanel.DefaultPadding = new Padding(5);

                var properties = _hbObj.GetType().GetProperties().Where(_=>_.CanWrite);
                if (properties.Count() > 15)
                {
                    _materialPanel.Height = 450;
                    _materialPanel.BeginScrollable();
                }

                var name = new TextBox();
                _hbObj.DisplayName = _hbObj.DisplayName ?? _hbObj.Identifier;
                name.TextBinding.Bind(() => _hbObj.DisplayName, (v) => _hbObj.DisplayName = v);

                _materialPanel.AddRow("Name", name);

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
                        _materialPanel.AddRow(item.Name, textBox);
                    }
                    else if (value is double numberValue)
                    {
                        var numberTB = new MaskedTextBox();
                        numberTB.Provider = new NumericMaskedTextProvider() { AllowDecimal=true };
                        numberTB.TextBinding.Bind(() => numberValue.ToString(), (v)=> item.SetValue(_hbObj, Convert.ChangeType(v, type)));
                        _materialPanel.AddRow(item.Name, numberTB);
                    }
                    else if (value is int intValue)
                    {
                        var numberTB = new NumericStepper();
                        numberTB.DecimalPlaces = 0;
                        numberTB.Value = intValue;
                        _materialPanel.AddRow(item.Name, numberTB);
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

                        _materialPanel.AddRow(item.Name, dropdown);


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

                        _materialPanel.AddRow(item.Name, dropdown);

                    }
                }
              

                var buttonSource = new Button { Text = "Schema Data" };
                buttonSource.Click += (s, e) =>
                {
                    Dialog_Message.Show(this, _hbObj.ToJson(true));
                };
              

                layout.AddRow(_materialPanel);
                layout.AddRow(buttonSource);
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
