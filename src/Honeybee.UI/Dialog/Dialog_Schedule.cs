using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;

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


                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                //var datePicker = new DateTimePicker();
                //layout.AddRow(datePicker);
                //layout.AddRow(null);


                var drawable = new Drawable(true)
                {
                    Size = new Size(600, 600)
                    //BackgroundColor = Colors.Blue
                };
                var location = new Point(100, 100);
                //var drawableTarget = new DrawableTarget(drawable) { UseOffScreenBitmap = true };
                EventHandler<MouseEventArgs> mouseHandler = (s, e) => {
                    location = new Point(e.Location);
                    ((Control)s).Invalidate();
                    e.Handled = true;
                };
                //drawable.MouseMove += mouseHandler;
                //drawable.MouseDoubleClick += mouseHandler;

                var dayValues = new List<double>() { 0, 0.5, 0 };
                var dayTimes = new List<(int hour,int minute)>() { (0,0), (6, 0), (18, 0) };
                var hours = dayTimes.Select(_ => _.hour).ToArray();

                drawable.MouseDoubleClick += (s, e) =>
                {
                    dayValues = new List<double>() { 0, 0.5, 1 ,0.5,0 };
                    dayTimes = new List<(int hour, int minute)>() { (0, 0), (6, 0), (7, 0), (14, 0), (18, 0) };
                    drawable.Update(drawable.Bounds);
                };

                drawable.Paint += (s, e) =>
                {
                    //var path = new GraphicsPath();
                    //path.AddEllipse(25, 25, 50, 50);
                    //path.AddRectangle(125, 25, 50, 50);
                    //path.AddLines(new PointF(225, 25), new PointF(225, 75), new PointF(275, 50));
                    //path.CloseFigure();

                    var graphics = e.Graphics;

                    //Draw schedule
                    var canv = graphics.ClipBounds;
                    canv.TopLeft = new PointF(10, 10);
                    canv.BottomRight = new PointF(canv.Width - 10, canv.Height - 10);
                    var hourPts = GenHourPts(dayValues, dayTimes, canv);
                    var allPts = GenPts(hourPts, canv);


                    graphics.DrawRectangle(Colors.Black, canv);

                    //foreach (var item in linePts)
                    //{
                    //    graphics.DrawRectangle(Colors.Blue, new RectangleF(item.start.X-1, item.start.Y-1, 2, 2));
                    //    graphics.DrawRectangle(Colors.Blue, new RectangleF(item.end.X-1, item.end.Y-1, 2, 2));
                    //    graphics.DrawLine(Colors.Black, item.start, item.end);
                    //}

                    foreach (var pt in allPts)
                    {
                        graphics.DrawRectangle(Colors.Blue, new RectangleF(pt.X - 3, pt.Y - 3, 6, 6));
                    }
                    graphics.DrawLines(Colors.Black, allPts);

                    //graphics.FillRectangle(Colors.Black, new Rectangle((int)hourX, (int)hourY, 2, 2));

                    //var loc = location;
                    //loc.Restrict(new Rectangle(image.Size));

                    //graphics.FillRectangle(Colors.Black, new Rectangle(loc.X - 50, loc.Y - 50, 100, 100));
                    //drawableTarget.EndDraw(e);
                };

                //drawable.Paint += (s, e) => {

                //    var graphics = e.Graphics;

                //    var loc = location;
                //    //loc.Restrict(new Rectangle(image.Size));
                //    graphics.FillRectangle(Colors.Black, new Rectangle(loc.X-50, loc.Y-50, 100, 100));
                //    //drawableTarget.EndDraw(e);
                //};

                //control.Paint += delegate (object sender, PaintEventArgs pe) {
                //    pe.Graphics.FillRectangle(Colors.Black, new Rectangle(150, 150, 100, 100));
                //    var inc = 400;
                //    for (int i = 0; i <= control.Size.Width / inc; i++)
                //    {
                //        var pos = i * inc;
                //        pe.Graphics.DrawLine(Colors.White, new Point(pos, 0), new Point(pos + control.Size.Width, control.Size.Height));
                //        pe.Graphics.DrawLine(Colors.White, new Point(pos, 0), new Point(pos - control.Size.Width, control.Size.Height));
                //    }
                //    var lpos = 100;
                //    pe.Graphics.DrawLine(Colors.White, new Point(0, lpos), new Point(control.Size.Width, lpos));
                //    pe.Graphics.DrawLine(Colors.White, new Point(lpos, 0), new Point(lpos, control.Size.Height));

                //};
                layout.AddRow(drawable);





                Content = layout;

            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }

        private List<PointF> GenHourPts(List<double> dayValues, List<(int hour, int minute)> dayTimes, RectangleF frameBound)
        {
            var canv = frameBound;
            var widthPerHour = canv.Width / 24;
            var heightPerHour = canv.Height;
            var count = dayValues.Count();
            var hourPts = new List<PointF>();
            for (int i = 0; i < count; i++)
            {
                var hourX = canv.Left + (float)dayTimes[i].hour * widthPerHour;
                var hourY = (canv.Bottom - (float)dayValues[i] * heightPerHour);
                hourPts.Add(new PointF(hourX, hourY));
            }

            return hourPts;
        }

        private List<PointF> GenPts(List<PointF> hourPts, RectangleF frameBound)
        {
            var count = hourPts.Count;
            var pts = new List<PointF>();
            var linePts = new List<(PointF start, PointF end)>();
            for (int i = 0; i < count; i++)
            {
                var pt = hourPts[i];

                if (i == 0)
                {
                    if (pt.Y != frameBound.Bottom)
                    {
                        //Begining of the schedule 
                        pts.Add(new PointF(frameBound.BottomLeft.X, frameBound.Height));
                        pts.Add(new PointF(pt.X, frameBound.Height));

                    }
                    else
                    {
                        pts.Add(pt);
                    }

                }
                else if (i == count - 1)
                {
                    var prePt = hourPts[i - 1];

                    pts.Add(new PointF(pt.X, prePt.Y));
                    pts.Add(pt);
                    pts.Add(new PointF(frameBound.Right, pt.Y));
                }
                else
                {
                    pts.Add(new PointF(pt.X, hourPts[i - 1].Y));
                    pts.Add(pt);

                }
            }

            return pts;
        }

     
        
     

    }
}
