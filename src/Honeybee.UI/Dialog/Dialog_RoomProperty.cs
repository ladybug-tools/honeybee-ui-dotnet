using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using Honeybee.UI.View;

namespace Honeybee.UI
{
    public class Dialog_RoomProperty : Dialog<List<HB.Room>>
    {
        public Dialog_RoomProperty(HB.ModelProperties libSource, List<HB.Room> rooms)
        {
            try
            {
                libSource.FillNulls();

                Title = $"Room Properties - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                this.Icon = DialogHelper.HoneybeeIcon;

                var p = new DynamicLayout();
                p.DefaultSpacing = new Size(4, 4);
                p.DefaultPadding = new Padding(4);

                var panel = RoomProperty.Instance;
                p.AddRow(panel);
                panel.UpdatePanel(libSource, rooms);

                var OKButton = new Button() { Text = "OK" };
                OKButton.Click += (s, e) =>
                {
                    try
                    {
                        this.Close(panel.GetRooms());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(this, er.Message);
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
                MessageBox.Show(e.Message);
                //throw;
            }

        }

        public void SetSensorPositionPicker(Func<List<double>> SensorPositionPicker)
        {
            RoomProperty.Instance.SetSensorPositionPicker(SensorPositionPicker);
        }
        public void SetInternalMassPicker(Func<double> internalMassAreaPicker)
        {
            RoomProperty.Instance.SetInternalMassPicker(internalMassAreaPicker);
        }
        public void SetAmbientCoffConditionRoomPicker(Func<string> RoomIDPicker)
        {
            RoomProperty.Instance.SetAmbientCoffConditionRoomPicker(RoomIDPicker);
        }

    }


}
