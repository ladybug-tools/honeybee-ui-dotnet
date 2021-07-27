using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class Dialog_Construction_Shade : Dialog<HB.ShadeConstruction>
    {
        public Dialog_Construction_Shade(HB.ShadeConstruction shadeConstruction)
        {
            var _hbObj = shadeConstruction;

            Padding = new Padding(10);
            Resizable = true;
            Title = $"Construction - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            //MinimumSize = new Size(450, 250);
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var OkButton = new Button { Text = "OK" };
            OkButton.Click += (sender, e) => Close(_hbObj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(10, 5);

            layout.AddRow("Name");
            var name = new TextBox();
            _hbObj.DisplayName = _hbObj.DisplayName ?? _hbObj.Identifier;
            name.TextBinding.Bind(() => _hbObj.DisplayName, (v) => _hbObj.DisplayName = v);
            layout.AddRow(name);


            //SolarReflectance
            var solarRef = new MaskedTextBox();
            solarRef.Provider = new NumericMaskedTextProvider() { AllowDecimal = true };
            solarRef.TextBinding.Bind(() => _hbObj.SolarReflectance.ToString(), (v) => _hbObj.SolarReflectance = double.Parse(v));
            layout.AddRow(nameof(_hbObj.SolarReflectance));
            layout.AddRow(solarRef);

            //VisibleReflectance
            var visibleRef = new MaskedTextBox();
            visibleRef.Provider = new NumericMaskedTextProvider() { AllowDecimal = true };
            visibleRef.TextBinding.Bind(() => _hbObj.VisibleReflectance.ToString(), (v) => _hbObj.VisibleReflectance = double.Parse(v));
            layout.AddRow(nameof(_hbObj.VisibleReflectance));
            layout.AddRow(visibleRef);

            // IsSpecular
            var IsSpecular = new CheckBox();
            IsSpecular.CheckedBinding.Bind(() => _hbObj.IsSpecular, (v) => _hbObj.IsSpecular = v.GetValueOrDefault(false));
            layout.AddRow(nameof(_hbObj.IsSpecular));
            layout.AddRow(IsSpecular);

            var buttonSource = new Button { Text = "Schema Data" };
            buttonSource.Click += (s, e) =>
            {
                Dialog_Message.Show(this, _hbObj.ToJson(true));
            };


            layout.AddRow(null);
            layout.AddRow(buttonSource);
            layout.AddRow(null);
            layout.AddSeparateRow(null, OkButton, AbortButton, null);

            Content = layout;

        }

    }
}
