using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;

namespace Honeybee.UI
{

    public class Dialog_Material : Dialog_ResourceEditor<HB.Energy.IMaterial>
    {
        private Label _r_label;
        private Label _u_label;
        private TextBox _r_value;
        private TextBox _u_value;

        const string _r_si = "R Value [m2·K/W]";
        const string _r_ip = "R Value [ft2·F·h/BTU]";
        const string _u_si = "U Value [W/m2·K]";
        const string _u_ip = "U Value [BTU/ft2·F·h]";

        private bool _showIP = false;
        public Dialog_Material(HB.Energy.IMaterial material, bool lockedMode = false)
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

                var locked = new CheckBox() { Text = "Locked", Enabled = false };
                locked.Checked = lockedMode;

                var OkButton = new Button { Text = "OK", Enabled = !lockedMode };
                OkButton.Click += (sender, e) => OkCommand.Execute(_hbObj);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                //var panel = new Panel_Schedule(sch);
                //panel.AddSeparateRow(null, OkButton, AbortButton, null);
                //Content = panel;

                var layout = new DynamicLayout();
                layout.DefaultSpacing = new Size(5, 5);
                layout.DefaultPadding = new Padding(5);

                // unit switchs
                var unit = new RadioButtonList();
                unit.Items.Add("Metric");
                unit.Items.Add("Imperial");
                unit.SelectedIndex = 0;
                unit.Spacing = new Size(5, 0);
                unit.SelectedIndexChanged += (s, e) => CalRValue(_hbObj, unit.SelectedIndex == 1);

                var buttonSource = new Button { Text = "Schema Data" };
                buttonSource.Click += (s, e) =>
                {
                    Dialog_Message.Show(this, _hbObj.ToJson(true));
                };

                var materialPanel = GenParmPanel(_hbObj);
                layout.AddRow(materialPanel);
                layout.AddSeparateRow( null, "R/U value unit:", unit);
                layout.AddSeparateRow(locked, null, OkButton, AbortButton, null, buttonSource);
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


        private Panel GenParmPanel(HB.Energy.IMaterial material)
        {
            var hbObj = material;

            var panel = new DynamicLayout();
            panel.DefaultSpacing = new Size(5, 5);
            panel.DefaultPadding = new Padding(5);

            var properties = hbObj.GetType().GetProperties().Where(_ => _.CanWrite);
            if (properties.Count() > 15)
            {
                panel.Height = 300;
                panel.BeginScrollable();
            }

            var name = new TextBox();
            hbObj.DisplayName = hbObj.DisplayName ?? hbObj.Identifier;
            name.TextBinding.Bind(() => hbObj.DisplayName, (v) => hbObj.DisplayName = v);

            panel.AddRow("Name", name);

            _r_label = new Label() {Text = _r_si };
            _u_label = new Label() { Text = _u_si };

            _r_value = new TextBox() { Enabled = false, PlaceholderText = "Not available" };
            _u_value = new TextBox() { Enabled = false, PlaceholderText = "Not available" };
            panel.AddRow(_r_label, _r_value);
            panel.AddRow(_u_label, _u_value);
            CalRValue(hbObj, false);

            foreach (var item in properties)
            {
                if (item.Name == "Identifier" || item.Name == "Type" || item.Name == "DisplayName")
                    continue;

                var value = item.GetValue(hbObj);
                var type = item.PropertyType;
                if (value is string stringvalue)
                {
                    var textBox = new TextBox();
                    //textBox.Text = stringvalue;
                    textBox.TextBinding.Bind(() => stringvalue, (v) => item.SetValue(hbObj, v));
                    panel.AddRow(item.Name, textBox);
                }
                else if (value is double numberValue)
                {
                    var numberTB = new MaskedTextBox();
                    numberTB.Provider = new NumericMaskedTextProvider() { AllowDecimal = true };
                    numberTB.TextBinding.Bind(
                        () => numberValue.ToString(), 
                        (v) => {
                            v = v.StartsWith(".") ? $"{0}{v}" : v;
                            item.SetValue(hbObj, Convert.ChangeType(v, type));
                            CalRValue(hbObj, _showIP);
                            }
                        );
                    panel.AddRow(item.Name, numberTB);
                }
                else if (value is int intValue)
                {
                    var numberTB = new NumericStepper();
                    numberTB.DecimalPlaces = 0;
                    numberTB.ValueBinding.Bind(
                        () => intValue, 
                        (v) => { 
                            item.SetValue(hbObj, v);
                            CalRValue(hbObj, _showIP);
                        }
                    );
                    panel.AddRow(item.Name, numberTB);
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

                    var currentValue = item.GetValue(hbObj, null).ToString();
                    dropdown.SelectedKeyBinding.Bind(
                        () => currentValue,
                        (v) => item.SetValue(hbObj, Enum.Parse(enumType, v))
                        );

                    panel.AddRow(item.Name, dropdown);


                }
                else if (type.IsEnum)
                {
                    var values = Enum.GetNames(type).ToList();
                    var dropdown = new DropDown();
                    var dropDownItems = values.Select(_ => new ListItem() { Text = _, Key = _ });
                    dropdown.Items.AddRange(dropDownItems);

                    var currentValue = item.GetValue(hbObj, null).ToString();
                    dropdown.SelectedKeyBinding.Bind(
                        () => currentValue,
                        (v) => item.SetValue(hbObj, Enum.Parse(type, v))
                        );

                    panel.AddRow(item.Name, dropdown);

                }
            }


            panel.AddRow(null, null);
            return panel;

           
        }

        void CalRValue(HB.Energy.IMaterial tc, bool ShowIPUnit)
        {
            _showIP = ShowIPUnit;

            var r = ShowIPUnit ? Math.Round(tc.RValue * 5.678263337, 5) : Math.Round(tc.RValue, 5);
            var u = ShowIPUnit ? Math.Round(tc.UValue / 5.678263337, 5) : Math.Round(tc.UValue, 5);

            _r_value.Text = r < 0 ? "Skylight only" : r.ToString();
            _u_value.Text = u < 0 ? "Skylight only" : u.ToString();
            _r_label.Text = ShowIPUnit ? _r_ip : _r_si;
            _u_label.Text = ShowIPUnit ? _u_ip : _u_si;
        }
    }
}
