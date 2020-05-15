using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Compression;
using HoneybeeSchema;

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


                Content = new Panel_Schedule(sch);

                //var layout = new DynamicLayout();
                //layout.Height = 500;
                //var drawable = new Drawable();
                //drawable.Size = new Size(600, 1500);
                ////drawable.BackgroundColor = Colors.Blue;

                //drawable.Paint += Drawable_FillBackground;
                //drawable.Paint += Drawable_FillForeground;
                //drawable.Paint += Drawable_Grid;
                //drawable.Paint += Drawable_Label;



                //layout.BeginScrollable();
                //layout.AddSeparateRow(drawable);

                //var cal = new Calendar();
                //var date = new DateTime(2017, 1, 1);
                //cal.MinDate = date;
                //cal.MaxDate = new DateTime(2017, 12, 31);
                //cal.Mode = CalendarMode.Range;


                //var canvas = new DynamicLayout();
                //canvas.AddSeparateRow(layout);
                //var btn = new Button();
                //btn.Click += (s, e) =>
                //{
                //    clicked = !clicked;
                //    drawable.Update(new Rectangle(new Point(30, 0), new Size(175, 1500)));
                //};

                //canvas.AddSeparateRow(btn);
                //Content = canvas;

            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }

        bool clicked = false;
        private SizeF _dayCellSize = new SizeF(25, 15);
        private PointF _calendarBasePoint = new PointF(30, 10);

        private List<(RectangleF rec, DateTime date)> _dayRectangles;

        public List<(RectangleF rec, DateTime date)> DayRectangles
        {
            get
            {
                if (_dayRectangles == null)
                {
                    _dayRectangles = GenDayPositons(_calendarBasePoint, _dayCellSize);
                }
                return _dayRectangles;
            }
        }

        private List<RectangleF> _monthlyBounds;

        public List<RectangleF> MonthlyBounds
        {
            get
            {
                if (_monthlyBounds == null)
                {
                    var grids = GenGrids(_calendarBasePoint, _dayCellSize);
                    _gridLines = grids.GridLines;
                    _monthlyBounds = grids.MonthlyRecs;
                }
                return _monthlyBounds; }
        }

        private List<RectangleF> _voidBounds;

        public List<RectangleF> VoidBounds
        {
            get
            {
                if (_voidBounds == null)
                {
                    _voidBounds = GenMonthlyVoids();
                }
                return _voidBounds;
            }
        }

        private List<(PointF s, PointF e)> _gridLines;

        public List<(PointF s, PointF e)> GridLines
        {
            get
            {
                if (_gridLines == null)
                {
                    var grids = GenGrids(_calendarBasePoint, _dayCellSize);
                    _gridLines = grids.GridLines;
                    _monthlyBounds = grids.MonthlyRecs;
                }
                return _gridLines;
            }
            set { _gridLines = value; }
        }

        private void Drawable_Grid(object sender, PaintEventArgs e)
        {
            var graphic = e.Graphics;
            var grids = this.GridLines;
            foreach (var item in grids)
            {
                graphic.DrawLine(Colors.Black, item.s, item.e);
            }
        
        }


    
        private void Drawable_FillBackground(object sender, PaintEventArgs e)
        {
            var graphic = e.Graphics;

            var color = Colors.Blue;
            var monthlyBounds = this.MonthlyBounds;
            graphic.FillRectangles(color, monthlyBounds);

            //Void 
            var voidBounds = this.VoidBounds;
            graphic.FillRectangles(Colors.White, voidBounds);

        }

        private void Drawable_FillForeground(object sender, PaintEventArgs e)
        {
            var graphic = e.Graphics;

            var color = Colors.Blue;
            if (clicked)
            {
                var selectedDays = DayRectangles.Where(_ => _.date.DayOfWeek == DayOfWeek.Friday);
                var recs = selectedDays.Select(_ => _.rec);
                graphic.FillRectangles(Colors.Red, recs);
            }
            //else
            //{
            //    var recs = DayRectangles.Select(_ => _.rec);
            //    graphic.FillRectangles(color, recs);
            //}

        }

        private void Drawable_Label(object sender, PaintEventArgs e)
        {
            
            var graphic = e.Graphics;

            var recs = DayRectangles.Select(_ => _.rec);
            var font = Fonts.Sans(8);
            foreach (var item in DayRectangles)
            {
                var cel = item.rec;
                graphic.DrawText(font, Colors.Black, cel.X, cel.Y, item.date.Day.ToString());
            }

        }


        private (List<(PointF s, PointF e)> GridLines, List<RectangleF> MonthlyRecs) GenGrids(PointF basePoint, SizeF daySize)
        {
            var left = basePoint.X;
            var top = basePoint.Y;

            var dayW = daySize.Width;
            var dayH = daySize.Height;
            var monthW = dayW * 7;
            var monthH = dayH * 6;
            var monthTop = 3 * dayH;

            var lines = new List<(PointF s, PointF e)>();
            var monthlyRecs = new List<RectangleF>();

            // draw monthly
            for (int i = 0; i < 12; i++)
            {
                var x = left;
                var y = i * (monthH + monthTop);
                GenMonthlyGrid(x, y);
                var monthBound = new RectangleF(new PointF(x, y), new SizeF(monthW, monthH));
                monthlyRecs.Add(monthBound);
            }

            return (lines, monthlyRecs);

            void GenMonthlyGrid(float baseX, float baseY)
            {
                // vertical lines
                for (int i = 0; i <= 7; i++)
                {
                    var x = i * dayW + baseX;
                    var s = new PointF(x, baseY);
                    var e = new PointF(x, monthH + baseY);
                    lines.Add((s, e));
                }
                // horizontal lines
                for (int i = 0; i <= 6; i++)
                {
                    var y = i * dayH + baseY;
                    var s = new PointF(baseX, y);
                    var e = new PointF(monthW + baseX, y);
                    lines.Add((s, e));
                }


            }
        }
        private List<(RectangleF rec, DateTime date)> GenDayPositons(PointF basePoint, SizeF daySize)
        {
            var left = basePoint.X;
            var top = basePoint.Y;

            var dayW = daySize.Width;
            var dayH = daySize.Height;
            var monthW = dayW * 7;
            var monthH = dayH * 6;
            var monthTop = 3 * dayH;

            var recs = new List<(RectangleF rec, DateTime date)>();

            // draw monthly
            for (int i = 0; i < 12; i++)
            {
                var startDay = new DateTime(2017, i + 1, 1);
                var x = left;
                var y = i * (monthH + monthTop);

                DrawMonthFill(x, y, startDay);
            }

            void DrawMonthFill(float baseX, float baseY, DateTime firstDayOfMonth)
            {
                var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
                var month = firstDayOfMonth.Month;
                var daysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
                var startPt = new PointF(baseX + firstDayOfWeek * dayW, baseY);
               
                for (int i = 0; i < daysInMonth; i++)
                {
                    if (startPt.X >= monthW + baseX)
                    {
                        startPt.X = baseX;
                        startPt.Y += dayH;
                    }

                    var rec = new RectangleF(startPt, startPt + daySize);
                    recs.Add((rec, new DateTime(2017, month, i+1)));
                    startPt.X += dayW;
                }
            }

            return recs;
        }
    
        private List<RectangleF> GenMonthlyVoids()
        {
            var monthlyRecs = this.MonthlyBounds;
            var dayCells = this.DayRectangles;


            var voids = new List<RectangleF>();
            for (int i = 0; i < 12; i++)
            {
                var startDay = new DateTime(2017, i + 1, 1);
                var daysInMonth = DateTime.DaysInMonth(2017, i + 1);
                var endDay = new DateTime(2017, i + 1, daysInMonth);

                var startDOY = startDay.DayOfYear;
                var endDOY = endDay.DayOfYear;

                var startCell = dayCells[startDOY -1].rec;
                var endCell = dayCells[endDOY- 1].rec;
                var cellHeight = startCell.Height;
                var monthBound = monthlyRecs[i];
                var left = monthBound.X;

              
                var voidRecTop = new RectangleF(monthBound.TopLeft, startCell.InnerBottomLeft);
                var voidRecBtm1 = new RectangleF(endCell.InnerTopRight, monthBound.BottomRight);
                var voidRecBtm2 = new RectangleF(endCell.InnerBottomRight, monthBound.BottomLeft);
                voids.Add(voidRecTop);
                voids.Add(voidRecBtm1);
                voids.Add(voidRecBtm2);
            }
            voids = voids.SkipWhile(_ => _.Width == 0 || _.Height == 0).ToList();
            return voids;

       
        }
    
    }
}
