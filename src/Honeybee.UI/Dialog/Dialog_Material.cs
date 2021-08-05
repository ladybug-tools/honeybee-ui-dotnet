using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;

namespace Honeybee.UI
{

    public class Dialog_Material : Dialog_ResourceEditor<HB.Energy.IMaterial>
    {
        private DynamicLayout _materialPanel;


        public Dialog_Material(HB.Energy.IMaterial material)
        {
            try
            {
                //_hbObj = HB.ModelEnergyProperties.Default.Materials.First(_ => _.Obj is HB.EnergyWindowMaterialGas).Obj as HB.EnergyWindowMaterialGas;
                var _hbObj = material;

                Padding = new Padding(5);
                Resizable = true;
                Title = $"Material - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);
                this.Icon = DialogHelper.HoneybeeIcon;

                var OkButton = new Button { Text = "OK" };
                OkButton.Click += (sender, e) => OkCommand.Execute(_hbObj);

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
                    _materialPanel.Height = 360;
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
                _materialPanel.AddRow(null, null);

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
                MessageBox.Show(e.Message);
                //throw e;
            }
        }



    }
}
