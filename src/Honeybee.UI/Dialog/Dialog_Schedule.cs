using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;

namespace Honeybee.UI
{
    public class Dialog_Schedule : Dialog
    {
        
        public Dialog_Schedule()
        {
            try
            {
                var sch = HB.ModelEnergyProperties.Default.Schedules.First(_=>_.Obj is HB.ScheduleRulesetAbridged).Obj as HB.ScheduleRulesetAbridged;
                
                Padding = new Padding(5);
                Resizable = true;
                Title = "Schedule - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 650);
                this.Icon = DialogHelper.HoneybeeIcon;

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close();

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var daySchedule = sch.DaySchedules.First(_ => _.Identifier == sch.DefaultDaySchedule);
              

                var layout = new Panel_Schedule(sch);
                //var layout = new DynamicLayout();

                //var drawable = new Drawable();
                //drawable.Size = new Size(600, 400);
                //drawable.BackgroundColor = Colors.Blue;
                //layout.AddSeparateRow(drawable);

                Content = layout;

            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }

    

    }
}
