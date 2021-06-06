﻿using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using Honeybee.UI.View;

namespace Honeybee.UI
{
    public class Dialog_DoorProperty : Dialog<List<HB.Door>>
    {
        public Dialog_DoorProperty(HB.ModelProperties libSource, List<HB.Door> faces)
        {
            try
            {
                Title = $"Door Properties - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                this.Icon = DialogHelper.HoneybeeIcon;

                var p = new DynamicLayout();
                p.DefaultSpacing = new Size(4, 4);
                p.DefaultPadding = new Padding(4);

                p.AddRow(DoorProperty.Instance);
                DoorProperty.Instance.UpdatePanel(libSource, faces);

                var OKButton = new Button() { Text = "OK" };
                OKButton.Click += (s, e) =>
                {
                    try
                    {
                        this.Close(DoorProperty.Instance.GetDoors());
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.Message);
                        //throw;
                    }
                 
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                p.AddSeparateRow(null, OKButton, this.AbortButton, null);
                this.Content = p;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //throw;
            }

        }
    }


}
