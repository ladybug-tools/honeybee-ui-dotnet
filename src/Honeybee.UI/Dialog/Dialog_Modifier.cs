using Eto.Drawing;
using Eto.Forms;
using System;
using System.Linq;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class Dialog_Modifier<T> : Dialog_ResourceEditor<T> where T : HB.ModifierBase
    {
        public Dialog_Modifier(T modifier, bool lockedMode = false)
        {
            var _hbObj = modifier;
            Title = $"Modifier - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OkButton = new Button { Text = "OK", Enabled = !lockedMode };
            OkButton.Click += (sender, e) => OkCommand.Execute(_hbObj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(5);

            var paramPanel = GenParmPanel(_hbObj);
            layout.AddRow(paramPanel);

            var docLink = new LinkButton();
            docLink.Text = "View help documents...";
            docLink.ToolTip = @"https://www.ladybug.tools/honeybee-schema/model.html";
            docLink.Click += (s, e) =>
            {
                var url = $"https://www.ladybug.tools/honeybee-schema/model.html#tag/{typeof(T).Name.ToLower()}_model";
                System.Diagnostics.Process.Start(url);
            };

            var buttonSource = new Button { Text = "Schema Data" };
            buttonSource.Click += (s, e) =>
            {
                Dialog_Message.ShowFullMessage(this, _hbObj.ToJson(true));
            };

            layout.AddRow(docLink);

            layout.AddSeparateRow(locked, null, OkButton, AbortButton, null, buttonSource) ;
            layout.AddRow(null);
            Content = layout;

        }

        private Panel GenParmPanel(HB.ModifierBase modifier)
        {
            var hbObj = modifier;

            var panel = new DynamicLayout();
            panel.DefaultSpacing = new Size(5, 5);
            panel.DefaultPadding = new Padding(5);

            var properties = hbObj.GetType().GetProperties().Where(_ => _.CanWrite);
            if (properties.Count() > 15)
            {
                panel.Height = 360;
                panel.BeginScrollable();
            }

            var name = new TextBox();
            hbObj.DisplayName = hbObj.DisplayName ?? hbObj.Identifier;
            name.TextBinding.Bind(() => hbObj.DisplayName, (v) => hbObj.DisplayName = v);

            panel.AddRow("Name", name);

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
                    numberTB.TextBinding.Bind(() => numberValue.ToString(), (v) => item.SetValue(hbObj, Convert.ChangeType(v, type)));
                    panel.AddRow(item.Name, numberTB);
                }
                else if (value is int intValue)
                {
                    var numberTB = new NumericStepper();
                    numberTB.DecimalPlaces = 0;
                    numberTB.Value = intValue;
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

    }


}
