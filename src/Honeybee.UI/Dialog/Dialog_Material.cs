using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Honeybee.UI
{

    public class Dialog_Material : Dialog_ResourceEditor<HB.Energy.IMaterial>
    {
        private TextBox _r_value;
        private TextBox _u_value;

        //const string _r_si = "RValue";
        //const string _u_si = "UValue";

        private bool _isIDEditable;
        public Dialog_Material(HB.Energy.IMaterial material, bool lockedMode = false, bool editID = false)
        {
            try
            {
                //_hbObj = HB.ModelEnergyProperties.Default.Materials.First(_ => _.Obj is HB.EnergyWindowMaterialGas).Obj as HB.EnergyWindowMaterialGas;
                var _hbObj = material;
                _isIDEditable = editID;

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

                var buttonSource = new Button { Text = "Schema Data" };
                buttonSource.Click += (s, e) =>
                {
                    Dialog_Message.ShowFullMessage(this, _hbObj.ToJson(true));
                };

                var materialPanel = GenParmPanel(_hbObj);
                layout.AddRow(materialPanel);
                //layout.AddSeparateRow( null, "R/U value unit:", unit);
                layout.AddSeparateRow(locked, null, OkButton, AbortButton, null, buttonSource);
                layout.AddRow(null);

                //Create layout
                Content = layout;

            }
            catch (Exception e)
            {
                Dialog_Message.Show(this, e);
                //throw e;
            }
        }


        private Panel GenParmPanel(HB.Energy.IMaterial material)
        {
            var hbObj = material;

            var panel = new DynamicLayout();
            panel.DefaultSpacing = new Size(5, 5);
            panel.DefaultPadding = new Padding(5);

            var hbObjType = hbObj.GetType();
            var properties = hbObjType.GetProperties().Where(_ => _.CanWrite);
            if (properties.Count() > 15)
            {
                panel.Height = 300;
                panel.BeginScrollable();
            }

            // Identifier
            var idPanel = DialogHelper.MakeIDEditor(hbObj, _isIDEditable);
            var idLabel = new Label() { Text = "ID" };
            var idDescription = HoneybeeSchema.SummaryAttribute.GetSummary(hbObjType, nameof(hbObj.Identifier));
            idLabel.ToolTip = Utility.NiceDescription(idDescription);
            panel.AddRow(idLabel, idPanel);


            var name = new TextBox();
            hbObj.DisplayName = hbObj.DisplayName ?? hbObj.Identifier;
            name.TextBinding.Bind(() => hbObj.DisplayName, (v) => hbObj.DisplayName = v);
            var nameLabel = new Label() { Text = "Name" };
            var nameDescription = HoneybeeSchema.SummaryAttribute.GetSummary(hbObjType, nameof(hbObj.DisplayName));
            nameLabel.ToolTip = Utility.NiceDescription(nameDescription);
            panel.AddRow(nameLabel, name);

            var rUnit = _RValueUnit.displayUnit.ToUnitsNetEnum().GetAbbreviation();
            var uUnit = _UValueUnit.displayUnit.ToUnitsNetEnum().GetAbbreviation();

            var rLabel = new Label() { Text = $"RValue [{rUnit}]" };
            var uLabel = new Label() { Text = $"UValue [{uUnit}]" };
            _r_value = new TextBox() { Enabled = false, PlaceholderText = "Not available" };
            _u_value = new TextBox() { Enabled = false, PlaceholderText = "Not available" };
            rLabel.ToolTip = Utility.NiceDescription("R-value of the material. Note that R-values do NOT include the resistance of air films on either side of the material."); ;
            uLabel.ToolTip = Utility.NiceDescription("U-value of the material. Note that U-values do NOT include the resistance of air films on either side of the material."); ;
            panel.AddRow(rLabel, _r_value);
            panel.AddRow(uLabel, _u_value);
            CalRValue(hbObj);

            foreach (var item in properties)
            {
                if (item.Name == "Identifier" || item.Name == "Type" || item.Name == "DisplayName")
                    continue;

                var value = item.GetValue(hbObj);
                var type = item.PropertyType;

                var label = new Label() { Text = item.Name };
                var description = HoneybeeSchema.SummaryAttribute.GetSummary(hbObjType, item.Name);
                label.ToolTip = Utility.NiceDescription(description);
                if (value is string stringvalue)
                {
                    var textBox = new TextBox();
                    textBox.TextBinding.Bind(() => stringvalue, (v) => item.SetValue(hbObj, v));

                    panel.AddRow(label, textBox);
                }
                else if (value is double numberValue)
                {
                    var numberTB = new MaskedTextBox();
                    numberTB.Provider = new NumericMaskedTextProvider() { AllowDecimal = true };
                    label.ToolTip = Utility.NiceDescription(description);

                    var hasUnits = _propertyUnits.TryGetValue(item.Name, out var units);
                    if (hasUnits)
                    {
                        var baseUnit = units.baseUnit.ToUnitsNetEnum();
                        var displayUnit = units.displayUnit.ToUnitsNetEnum();

                        numberTB.TextBinding.Bind(
                        () =>
                        {
                            var displayValue = Units.ConvertValueWithUnits(numberValue, baseUnit, displayUnit);
                            return displayValue.ToString();
                        },
                        (v) =>
                        {
                            Utility.TryParse(v, out var numValue);
                            var baseValue = Units.ConvertValueWithUnits(numValue, displayUnit, baseUnit);
                            item.SetValue(hbObj, baseValue);
                            CalRValue(hbObj);
                        }
                        );
                        var abb = displayUnit.GetAbbreviation();
                        label.Text = $"{item.Name} [{abb}]";
                        panel.AddRow(label, numberTB);
                    }
                    else // unitless property
                    {
                        numberTB.TextBinding.Bind(
                        () => numberValue.ToString(),
                        (v) =>
                        {
                            Utility.TryParse(v, out var numValue);
                            item.SetValue(hbObj, numValue);
                            CalRValue(hbObj);
                        }
                        );
                        panel.AddRow(label, numberTB);
                    }

                }
                else if (value is int intValue)
                {
                    var numberTB = new NumericStepper();
                    numberTB.DecimalPlaces = 0;
                    numberTB.ValueBinding.Bind(
                        () => intValue,
                        (v) =>
                        {
                            item.SetValue(hbObj, v);
                            CalRValue(hbObj);
                        }
                    );
                    label.ToolTip = Utility.NiceDescription(description);
                    panel.AddRow(label, numberTB);
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

                    label.ToolTip = Utility.NiceDescription(description);

                    panel.AddRow(label, dropdown);


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

                    label.ToolTip = Utility.NiceDescription(description);
                    panel.AddRow(label, dropdown);

                }
            }


            panel.AddRow(null, null);
            return panel;

        }



        private static (Enum baseUnit, Enum displayUnit) _RValueUnit => _propertyUnits["RValue"];
        private static (Enum baseUnit, Enum displayUnit) _UValueUnit => _propertyUnits["UValue"];
        private static Dictionary<string, (Enum baseUnit, Enum displayUnit)> _propertyUnits => new Dictionary<string, (Enum, Enum)>()
        {
            { "Thickness", (Units.LengthUnit.Meter, Units.CustomUnitSettings[Units.UnitType.Length])},
            { "Conductivity", (Units.ThermalConductivityUnit.WattPerMeterKelvin, Units.CustomUnitSettings[Units.UnitType.Conductivity])},
            { "UFactor", (Units.HeatTransferCoefficientUnit.WattPerSquareMeterKelvin, Units.CustomUnitSettings[Units.UnitType.UValue])},
            { "UValue", (Units.HeatTransferCoefficientUnit.WattPerSquareMeterKelvin, Units.CustomUnitSettings[Units.UnitType.UValue])},
            { "RValue", (Units.ThermalResistanceUnit.SquareMeterDegreeCelsiusPerWatt, Units.CustomUnitSettings[Units.UnitType.Resistance])},
            { "Density", (Units.DensityUnit.KilogramPerCubicMeter, Units.CustomUnitSettings[Units.UnitType.Density])},
            { "SpecificHeat", (Units.SpecificEntropyUnit.JoulePerKilogramKelvin, Units.CustomUnitSettings[Units.UnitType.SpecificEntropy])},

        };

        void CalRValue(HB.Energy.IMaterial tc)
        {
            try
            {
                var r = tc.RValue;
                var rUnit = _RValueUnit;
                var baseUnit = rUnit.baseUnit.ToUnitsNetEnum();
                var displayUnit = rUnit.displayUnit.ToUnitsNetEnum();
                r = Units.ConvertValueWithUnits(r, baseUnit, displayUnit);

                var u = 1 / r;

                r = Math.Round(r, 5);
                u = Math.Round(u, 5);

                _r_value.Text = r < 0 ? "Skylight only" : r.ToString();
                _u_value.Text = u < 0 ? "Skylight only" : u.ToString();
            }
            catch (Exception)
            {
                //throw;
            }

        }
    }
}
