using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using Honeybee.UI.View;

namespace Honeybee.UI
{
    public class Dialog_SensorGridProperty : Dialog<List<HB.SensorGrid>>
    {
        public Dialog_SensorGridProperty(List<HB.SensorGrid> sensorGrids)
        {
            try
            {
                Title = $"Sensor Grid Properties - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                this.Icon = DialogHelper.HoneybeeIcon;

                var p = new DynamicLayout();
                p.DefaultSpacing = new Size(4, 4);
                p.DefaultPadding = new Padding(4);


                var panel = SensorGridProperty.Instance;
                p.AddRow(panel);
                panel.UpdatePanel(sensorGrids);

                var OKButton = new Button() { Text = "OK" };
                OKButton.Click += (s, e) =>
                {
                    try
                    {
                        this.Close(panel.GetSensorGrids());
                    }
                    catch (Exception er)
                    {
                        Dialog_Message.Show(this, er);
                        //throw;
                    }

                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                p.AddSeparateRow(null, null, OKButton, this.AbortButton, null, panel.SchemaDataBtn);
                p.Add(null);
                this.Content = p;
            }
            catch (Exception e)
            {
                Dialog_Message.Show(this, e);
                //throw;
            }

        }
    }

}
