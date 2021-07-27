using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{

    public class Dialog_Construction_AirBoundary : Dialog<HB.AirBoundaryConstructionAbridged>
    {
        public Dialog_Construction_AirBoundary(HB.AirBoundaryConstructionAbridged airBoundaryConst)
        {
            var _hbObj = airBoundaryConst;

            Padding = new Padding(10);
            Resizable = true;
            Title = $"Construction - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
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


            //AirMixingPerArea
            var airMx = new MaskedTextBox();
            airMx.Provider = new NumericMaskedTextProvider() { AllowDecimal = true };
            airMx.TextBinding.Bind(() => _hbObj.AirMixingPerArea.ToString(), (v) => _hbObj.AirMixingPerArea = double.Parse(v));
            layout.AddRow(nameof(_hbObj.AirMixingPerArea));
            layout.AddRow(airMx);

            //AirMixingSchedule
            var airMxSch = new TextBox() { Enabled = false};
            airMxSch.TextBinding.Bind(() => _hbObj.AirMixingSchedule, (v) => _hbObj.AirMixingSchedule = v);
            layout.AddRow(nameof(_hbObj.AirMixingSchedule));
            layout.AddRow(airMxSch);

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
