using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ScheduleTypeLimit : Dialog<ScheduleTypeLimit>
    {
        public Dialog_ScheduleTypeLimit()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Schedule type limits - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 350;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(5);

            var dropDowns = new DropDown();

            foreach (var item in _typeLimitLib)
            {
                var dropItem = new ListItem();
                dropItem.Text = item.Key;
                dropItem.Tag = item.Value;
                dropDowns.Items.Add(dropItem);
            }
            dropDowns.SelectedIndex = 0;
            layout.Add("Select a schedule type limit:");
            layout.Add(dropDowns);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) =>
            {
                var sel = (dropDowns.SelectedValue as ListItem)?.Tag as ScheduleTypeLimit;
                if (sel == null)
                {
                    MessageBox.Show("Failed to select a ScheduleTypeLimit");
                    return;
                }
                Close(sel);
            };


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
            layout.AddRow(null);

            this.Content = layout;

        }

        private static NoLimit _noLimit = new NoLimit();
        private static readonly Dictionary<string, ScheduleTypeLimit> _typeLimitLib = new Dictionary<string, ScheduleTypeLimit>()
            {
                { "Dimensionless 1 [NoLimit ~ NoLimit]",
                    new ScheduleTypeLimit("Dimensionless 1","Dimensionless 1", _noLimit, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.Dimensionless)},
                { "Dimensionless 2 [-1 ~ 1]",
                    new ScheduleTypeLimit("Dimensionless 2","Dimensionless 2", -1, 1, ScheduleNumericType.Continuous, ScheduleUnitType.Dimensionless)},
                { "Dimensionless 3 [0 ~ NoLimit]",
                    new ScheduleTypeLimit("Dimensionless 3","Dimensionless 3", 0, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.Dimensionless)},
                { "Dimensionless 4 [0.1 ~ 1000]",
                    new ScheduleTypeLimit("Dimensionless 4","Dimensionless 4", 0.1, 1000, ScheduleNumericType.Continuous, ScheduleUnitType.Dimensionless)},
                { "Dimensionless 5 [0 ~ 1]",
                    new ScheduleTypeLimit("Dimensionless 5","Dimensionless 5", 0, 1, ScheduleNumericType.Continuous, ScheduleUnitType.Dimensionless)},
                { "Fractional [0 ~ 1]",
                    new ScheduleTypeLimit("Fractional","Fractional", 0, 1, ScheduleNumericType.Continuous, ScheduleUnitType.Dimensionless)},
                { "Activity Level [W/person]",
                    new ScheduleTypeLimit("Activity Level","Activity Level", 0, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.ActivityLevel)},
                { "Angle [degree]",
                    new ScheduleTypeLimit("Angle","Angle", 0, 180, ScheduleNumericType.Continuous, ScheduleUnitType.Angle)},
                { "Control Level [0 , NoLimit]:Discrete",
                    new ScheduleTypeLimit("Control Level","Control Level", 0, _noLimit, ScheduleNumericType.Discrete, ScheduleUnitType.Control)},
                { "Capacity [W]",
                    new ScheduleTypeLimit("Capacity","Capacity", 0, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.Capacity)},
                { "Clothing [clo]",
                    new ScheduleTypeLimit("Clothing","Clothing", 0, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.Dimensionless)},
                { "Delta Temperature",
                    new ScheduleTypeLimit("Delta Temperature","Delta Temperature", _noLimit, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.DeltaTemperature)},
                { "Humidity [0 ~ 100]",
                    new ScheduleTypeLimit("Humidity","Humidity", 0, 100, ScheduleNumericType.Continuous, ScheduleUnitType.Percent)},
                { "On-Off [0 , 1]:Discrete",
                    new ScheduleTypeLimit("On-Off","On-Off", 0, 1, ScheduleNumericType.Discrete, ScheduleUnitType.Dimensionless)},
                { "Power [NoLimit ~ NoLimit]",
                    new ScheduleTypeLimit("Power","Power", _noLimit, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.Power)},
                { "Temperature [-273.15 ~ NoLimit]",
                    new ScheduleTypeLimit("Temperature","Temperature", -273.15, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.Temperature)},
                { "Temperature [0 ~ 100]",
                    new ScheduleTypeLimit("Temperature 2","Temperature 2", 0, 100, ScheduleNumericType.Continuous, ScheduleUnitType.Temperature)},
                { "Percent [%]",
                    new ScheduleTypeLimit("Percent","Percent", 0, 100, ScheduleNumericType.Continuous, ScheduleUnitType.Percent)},
                { "Velocity",
                    new ScheduleTypeLimit("Velocity","Velocity", 0, _noLimit, ScheduleNumericType.Continuous, ScheduleUnitType.Velocity)},
            };



    }
}
