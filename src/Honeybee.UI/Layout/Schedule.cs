using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class Panel_Schedule : DynamicLayout
    {
        private double _intervalMinutes = 60; // hourly = 60, 15 minutes = 15, etc

        private HB.ScheduleTypeLimit _scheduleTypeLimits = new HB.ScheduleTypeLimit(Guid.NewGuid().ToString(), null, 0, 1);
        public (double start, double end) ScheduleTypeLimits { 
            get 
            {
                var limits = (0.0, 1.0);
                if (_scheduleTypeLimits.LowerLimit.Obj is double low)
                    limits.Item1 = low;
                if (_scheduleTypeLimits.UpperLimit.Obj is double up)
                    limits.Item2 = up;
                return limits;
            }
        }

        private double _scheduleTypelength => Math.Abs(ScheduleTypeLimits.end - ScheduleTypeLimits.start);
        public List<double> DayValues { get; set; }
        public List<double> DayValuesFraction => DayValues.Select(_ => _ / _scheduleTypelength).ToList();

        public List<(int hour, int minute)> DayTimes { get; set; }

        public Panel_Schedule(HB.ScheduleRulesetAbridged scheduleRuleset)
        {
            var sch = scheduleRuleset;
            var daySchedule = sch.DaySchedules.First(_ => _.Identifier == sch.DefaultDaySchedule);
            var values = daySchedule.Values;
            //sch.ScheduleTypeLimit

            //(double start, double end) scheduleTypeLimits = (0, 100);
            //var scheduleTypelength = Math.Abs(scheduleTypeLimits.end - scheduleTypeLimits.start);
            this._scheduleTypeLimits = new HB.ScheduleTypeLimit(Guid.NewGuid().ToString(), null, 0, 1);
            this.DayValues = new List<double>() { 0, 0.5, 0.1 };
            //var dayValueFraction = dayValues.Select(_ => _ / scheduleTypelength);
            this.DayTimes = new List<(int hour, int minute)>() { (0, 0), (6, 0), (18, 0) };

            //var DayValues = this.DayValues;

            //var layout = new DynamicLayout();
            this.DefaultPadding = new Padding(10);
            this.DefaultSpacing = new Size(5, 5);

            var name_Tb = new TextBox() { Text = daySchedule.DisplayName ?? daySchedule.Identifier };
            this.AddSeparateRow("Day Schedule Name:", name_Tb);
            var lowerLimit_TB = new NumericMaskedTextStepper<double>() { Value = ScheduleTypeLimits.start, PlaceholderText= "0" };
            var higherLimit_TB = new NumericMaskedTextStepper<double>() { Value = ScheduleTypeLimits.end, PlaceholderText = "0" };
      

            var mouseHoverValue_TB = new NumericMaskedTextBox<double>() { PlaceholderText = "Mouse over lines", Height = 20, Font= Fonts.Sans(8) };
      
            var label = new Label() { Text = "" };
            


            var drawable = new Drawable(true) { };
            drawable.Size = new Size(600, 400);
            //drawable.BackgroundColor = Colors.Blue;
            var location = new Point(0, 0);
            var canvas = drawable.Bounds;
            canvas.Size = canvas.Size - 20;
            canvas.TopLeft = new Point(50, 10);


            //var drawableTarget = new DrawableTarget(drawable) { UseOffScreenBitmap = true };
            EventHandler<MouseEventArgs> mouseHandler = (s, e) => {
                location = new Point(e.Location);
                ((Control)s).Invalidate();
                e.Handled = true;
            };
            //drawable.MouseMove += mouseHandler;
            //drawable.MouseDoubleClick += mouseHandler;

            var allMouseHoverRanges = new List<(bool isVertical, RectangleF rectangle, int valueIndex)>();
            var mouseHoveredRanges = new List<(bool isVertical, RectangleF rectangle, int valueIndex)>();
            drawable.MouseMove += (s, e) =>
            {
                var mouseLoc = e.Location;
                //label.Text = $"{Math.Round(mouseLoc.X)},{Math.Round(mouseLoc.Y)}";
                //Draw mouse hover over ranges
                var hovered = allMouseHoverRanges.Where(_ => _.rectangle.Contains(mouseLoc));
                if (hovered.Any())
                {
                    mouseHoveredRanges = hovered.ToList();
                    drawable.Update(canvas);
                }
                else
                {
                    if (mouseHoveredRanges.Any())
                    {
                        ////var preRec = mouseHoveredRanges.First().rectangle;
                        mouseHoveredRanges.Clear();
                        //drawable.Update(new Rectangle(preRec));
                        drawable.Update(canvas);
                    }

                }

            };

            var startDragging = false;
            var mouseHoveredRangesForDragging = new List<(bool isVertical, RectangleF rectangle, int valueIndex)>();
            drawable.MouseDown += (s, e) =>
            {
                if (mouseHoveredRanges.Any())
                {
                    startDragging = e.Buttons == MouseButtons.Primary;
                    mouseHoveredRangesForDragging = mouseHoveredRanges;
                    //label.Text = startDragging.ToString();
                }

            };
            drawable.MouseUp += (s, e) =>
            {
                if (startDragging)
                {
                    mouseHoveredRangesForDragging.Clear();
                    startDragging = false;
                    drawable.Update(canvas);
                }

            };

            drawable.LostFocus += (s, e) =>
            {
                if (startDragging)
                {
                    mouseHoveredRangesForDragging.Clear();
                    startDragging = false;
                    drawable.Update(canvas);
                }


            };

            // mouse move for dragging

            drawable.MouseMove += (s, e) =>
            {
                if (!startDragging)
                    return;

                var mouseLoc = e.Location;
                if (canvas.Contains(new Point(mouseLoc)) && mouseHoveredRangesForDragging.Any())
                {
                    var hovered = mouseHoveredRangesForDragging.First();
                    var valueIndex = hovered.valueIndex;

                    if (hovered.isVertical)
                    {
                        var mappedTime = (mouseLoc.X - canvas.Left) / canvas.Width*24;
                        var timeNormalized = NormalizeHourMinute(mappedTime);
                        var newHour = timeNormalized.hour;
                        var newMinute = timeNormalized.minute;
                        var newTime = newHour + newMinute / 60;

                        // get minimum movable left bound; 
                        var beforeTime = (0, 0);
                        var beforeIndex = hovered.valueIndex - 1;
                        if (beforeIndex >= 0)
                        {
                            beforeTime = DayTimes[beforeIndex];
                        }
                        var beforeTime2 = beforeTime.Item1 + beforeTime.Item2 / 60;
                        // get maximum movable right bound; 
                        var nextTime = (24, 0);
                        var nextIndex = hovered.valueIndex + 1;
                        if (nextIndex < DayTimes.Count)
                        {
                            nextTime = DayTimes[nextIndex];
                        }
                        var nextTime2 = nextTime.Item1 + nextTime.Item2 / 60;

                        
                        if (newTime < nextTime2 && newTime > beforeTime2)
                        {
                            DayTimes[hovered.valueIndex] = (newHour, newMinute);
                            drawable.Update(canvas);
                        }


                    }
                    else
                    {
                        var mappedValue = (canvas.Bottom - mouseLoc.Y) / canvas.Height;
                        var decimalPlaces = (this._scheduleTypelength/100).ToString().Split('.').Last().Length;
                        var pecent = Math.Round(mappedValue, decimalPlaces);

                        DayValues[valueIndex] = pecent * this._scheduleTypelength;
                        drawable.Update(canvas);
                    }
                    
                   
                }
                else
                {
                    mouseHoveredRangesForDragging.Clear();
                    startDragging = false;
                }



            };


            drawable.MouseDoubleClick += (s, e) =>
            {
                var mouseLoc = e.Location;
                var doubleClickedRanges = allMouseHoverRanges.Where(_ => _.rectangle.Contains(mouseLoc));
               
                if (doubleClickedRanges.Any())
                {
                    var hovered = doubleClickedRanges.First();
                    if (hovered.isVertical)
                    {
                        DayTimes.RemoveAt(hovered.valueIndex);
                        DayValues.RemoveAt(hovered.valueIndex);
                        drawable.Update(canvas);

                        return;
                    }

                    //var mouseLoc = e.Location;
                    //TODO: need to do conversion based on schedule's max and min. For now, it just calculated %.
                    var mappedTimeRaw = (mouseLoc.X - canvas.Left) / canvas.Width * 24;
                
                    var timeNormalized = NormalizeHourMinute(mappedTimeRaw);
                    var hour = timeNormalized.hour;
                    var minute = timeNormalized.minute;
                    var mappedTimeNormalized = hour + (double)(minute / 60.0);

                    // get minimum movable left bound; 
                    var beforeTime = (0, 0);
                    var beforeIndex = hovered.valueIndex;
                    if (beforeIndex >= 0)
                        beforeTime = DayTimes[beforeIndex];
                    var beforeTime2 = beforeTime.Item1 + beforeTime.Item2 / 60;
                    // get maximum movable right bound; 
                    var nextTime = (24, 0);
                    var nextIndex = hovered.valueIndex + 1;
                    if (nextIndex < DayTimes.Count)
                        nextTime = DayTimes[nextIndex];
                    var nextTime2 = nextTime.Item1 + nextTime.Item2 / 60;

                    //var newDateTimes = dayTimes.ToList();
                    if (mappedTimeNormalized < nextTime2 && mappedTimeNormalized > beforeTime2)
                    {
                        var insertIndex = hovered.valueIndex;
                        DayTimes.Insert(insertIndex+1, (hour, minute));
                        var addValue = DayValues[insertIndex];
                        DayValues.Insert(insertIndex + 1, addValue);
                        //newDateTimes.Add((hour, minute));
                        drawable.Update(new Rectangle(hovered.rectangle));
                    }

              

                    
                }

            };

            var hoveredValueIndex = 0;
            drawable.Paint += (s, e) =>
            {
                //TODO: this is need when I start working on resizable charts 
                //canvas = drawable.Bounds;
                //canvas.Size = canvas.Size - 30;
                //canvas.TopLeft = new Point(30, 15);
                //canvas.Size = new Size(canvas.Width -100, 300);

                var mouseLoc = location;

                var graphics = e.Graphics;

                //Draw schedule
                graphics.FillRectangle(Colors.White, canvas);

                var hourPts = GenHourPts(canvas);
                var graphElements = GenPts(hourPts, canvas);
                var allPts = graphElements.points;
                allMouseHoverRanges = graphElements.ranges;


                //Draw mouse hover over ranges
                //var hovered = allMouseHoverRanges.Where(_ => _.rectangle.Contains(mouseLoc));

                var hovered = mouseHoveredRanges;
                if (hovered.Any())
                {
                    //mouseHoverValue_TB.Text = null;
                    //label.Text = null;

                    var hoveredRec = hovered.First();
                    var rec = hoveredRec.rectangle;
                    //draw hover rec
                    graphics.FillRectangle(Color.FromArgb(200, 200, 200), rec);
                    //draw text
                    var textLoc = hoveredRec.rectangle.Center;

                    var valueToDisplay = string.Empty;
                    if (hoveredRec.isVertical)
                    {
                        var time = DayTimes[hoveredRec.valueIndex];
                        valueToDisplay = TimeSpan.Parse($"{time.hour}:{time.minute}").ToString(@"hh\:mm");
                        valueToDisplay = $" {valueToDisplay}";
                    }
                    else
                    {
                        valueToDisplay = DayValues[hoveredRec.valueIndex].ToString();
                    }
                    var font = Fonts.Sans(8);
                    var textSize = font.MeasureString(valueToDisplay);
                    graphics.DrawText(font, Colors.Black, textLoc.X - textSize.Width / 2, textLoc.Y - textSize.Height / 2 - 8, valueToDisplay);

                    // Show input textBox for users to type in a new value
                    if (hoveredRec.isVertical)
                    {
                        mouseHoverValue_TB.Enabled = false;
                    }
                    else
                    {
                        mouseHoverValue_TB.Enabled = true;
                        hoveredValueIndex = hoveredRec.valueIndex;
                        if (!mouseHoverValue_TB.HasFocus)
                            mouseHoverValue_TB.Focus();

                        var hoveredValue = DayValues[hoveredRec.valueIndex];
                        if (mouseHoverValue_TB.SelectedText != hoveredValue.ToString())
                        {
                            mouseHoverValue_TB.Value = hoveredValue;
                            mouseHoverValue_TB.SelectAll();
                        }
                    }

                }
                else
                {
                    if (!startDragging)
                    {
                        mouseHoverValue_TB.Value = 0;
                        mouseHoverValue_TB.Enabled = false;
                    }
                    
                }

                foreach (var pt in allPts)
                {
                    graphics.FillRectangle(Colors.Black, new RectangleF(pt.X - 3, pt.Y - 3, 6, 6));
                }

                var pen = new Pen(Colors.Black, 2);
                graphics.DrawLines(pen, allPts);

                // Draw canvas border
                graphics.DrawRectangle(Colors.Black, canvas);
                // Draw chart axis ticks
                DrawTicks(this.ScheduleTypeLimits, canvas, graphics);
                
            };


            mouseHoverValue_TB.KeyDown += (s, e) =>
            {
                if (e.Key == Keys.Enter)
                {
                    if (mouseHoveredRanges.Any())
                    {
                        var preRec = mouseHoveredRanges.First().rectangle;
                        mouseHoveredRanges.Clear();

                        var valueIndex = hoveredValueIndex;
                        var newUserInput = mouseHoverValue_TB.Value;
                        var oldValue = DayValues[valueIndex];
                  
                        //preRec.Top = (float) Math.Max(newUserInput, oldValue);
                        //preRec.Bottom = (float)Math.Min(newUserInput, oldValue);
                        DayValues[valueIndex] = newUserInput;

                        drawable.Update(canvas);
                    }


                }

            };


            lowerLimit_TB.ValueChanged += (s, e) => 
            { 
                this._scheduleTypeLimits.LowerLimit = lowerLimit_TB.Value;
                drawable.Update(canvas);
            };
            //lowerLimit_TB.KeyDown += (s, e) =>
            //{
            //    if (e.Key == Keys.Enter)
            //    {
            //        this._scheduleTypeLimits.LowerLimit = lowerLimit_TB.Value;
            //        drawable.Update(canvas);
            //    }
            //};
            higherLimit_TB.TextChanged += (s, e) => 
            { 
                this._scheduleTypeLimits.UpperLimit = higherLimit_TB.Value;
                drawable.Update(canvas);
            };
            //higherLimit_TB.KeyDown += (s, e) =>
            //{
            //    this._scheduleTypeLimits.UpperLimit = higherLimit_TB.Value;
            //    drawable.Update(canvas);
            //};
            this.AddSeparateRow("Lower Limit:", lowerLimit_TB, "Upper Limit:", higherLimit_TB, null, label, mouseHoverValue_TB);
            //this.AddSeparateRow(null, label, mouseHoverValue_TB);
            this.AddRow(drawable);
            this.AddIntervalButtons();


            var btn = new Button() { Text = "HBData" };
            btn.Click += (s, e) =>
            {
                Dialog_Message.Show(this, this._scheduleTypeLimits.ToJson());
                //MessageBox.Show(this, string.Join<double>(", ", dayValues));
            };
            this.AddRow(btn);

        }

        private void AddIntervalButtons()
        {
            var btn1 = new Button() { Text = "Hourly", Enabled = false };
            var btn2 = new Button() { Text = "15 Minutes" };
            var btn3 = new Button() { Text = "1 Minute" };

            btn1.Click += (s, e) =>
            {
                _intervalMinutes = 60;
                btn1.Enabled = false;
                btn2.Enabled = true;
                btn3.Enabled = true;
            };
            btn2.Click += (s, e) =>
            {
                _intervalMinutes = 15;
                btn2.Enabled = false;
                btn1.Enabled = true;
                btn3.Enabled = true;
            };
            btn3.Click += (s, e) =>
            {
                _intervalMinutes = 1;
                btn3.Enabled = false;
                btn1.Enabled = true;
                btn2.Enabled = true;
            };
            this.AddSeparateRow(null, btn1, btn2, btn3, null);
        }


        private static void DrawTicks((double start, double end) scheduleTypeLimits, Rectangle canvas, Graphics graphics)
        {
            //Draw horizontal ticks
            var markCount = 6;
            var hourInterval = 24 / markCount;
            var hourStartPt = canvas.BottomLeft;
            var hourEdPt = canvas.BottomRight;

            var widthPerInterval = (hourEdPt.X - hourStartPt.X) / markCount;
            var bottom = canvas.Bottom;
            var left = canvas.Left;

            var tickLength = 8;
            var tickfont = Fonts.Sans(8);
            //var font = Fonts.Sans(10);
            for (int i = 0; i <= markCount; i++)
            {
                var p1 = new PointF(left + i * widthPerInterval, bottom);
                var p2 = new PointF(left + i * widthPerInterval, bottom + tickLength);
                graphics.DrawLine(Colors.Black, p1, p2);

                var tickText = $"{i * hourInterval}:00";
                var textSize = tickfont.MeasureString(tickText);
                graphics.DrawText(tickfont, Colors.Black, p2.X - textSize.Width / 2, p2.Y - textSize.Height / 2 + 8, tickText);
            }

            //Draw vertical value ticks
            var valueMarkCount = 5;
            var valueStartPt = canvas.BottomLeft;
            var valueEndPt = canvas.TopLeft;
            var heightPerInterval = (valueStartPt.Y - valueEndPt.Y) / valueMarkCount;
            var valueInterval = Math.Abs(scheduleTypeLimits.start - scheduleTypeLimits.end) / valueMarkCount;
            for (int i = 0; i <= valueMarkCount; i++)
            {
                var p1 = new PointF(left, bottom - i * heightPerInterval);
                var p2 = new PointF(left - tickLength, bottom - i * heightPerInterval);
                graphics.DrawLine(Colors.Black, p1, p2);

                var tickText = $"{scheduleTypeLimits.start + i * valueInterval}";
                var textSize = tickfont.MeasureString(tickText);
                graphics.DrawText(tickfont, Colors.Black, p2.X - textSize.Width - 2, p2.Y - textSize.Height / 2, tickText);
            }

        }





        /// <summary>
        /// Convert 20 (minute) to 15 (minute) based on _intervalMinutes
        /// </summary>
        /// <param name="oldMinute"></param>
        /// <returns></returns>
        private double NormalizeMinute(int oldMinute)
        {
            var checkedMinute = Math.Round(oldMinute / _intervalMinutes) * _intervalMinutes;
            return checkedMinute;
        }

        /// <summary>
        /// Convert 11.20 (11:12 AM) to 11.25 (11:15 AM) based on _intervalMinutes
        /// </summary>
        /// <param name="oldHourMinute"></param>
        /// <returns></returns>
        private double NormalizeHour(double oldHourMinute)
        {
            var time = NormalizeHourMinute(oldHourMinute);
            var mappedTimeNormalized = time.hour + (double)(time.minute / 60.0);
            return mappedTimeNormalized;
        }
        private (int hour, int minute) NormalizeHourMinute(double oldHourMinute)
        {
            var hour = (int)Math.Floor(oldHourMinute);
            var mappedMinute = (int)NormalizeMinute((int)(Math.Abs(oldHourMinute - hour) * 60));
            if (mappedMinute == 60)
            {
                hour++;
                mappedMinute = 0;
            }
            return (hour, mappedMinute);
        }
        private List<PointF> GenHourPts(RectangleF frameBound)
        {
            var canv = frameBound;
            var dayValues = this.DayValues;
            var dayValuesFraction = this.DayValuesFraction;
            var dayTimes = this.DayTimes;

            var widthPerHour = canv.Width / 24;
            var heightPerHour = canv.Height;
            var count = dayValues.Count();
            var hourPts = new List<PointF>();
            for (int i = 0; i < count; i++)
            {
                var checkedMinute = NormalizeMinute(dayTimes[i].minute);
                var time = (double)dayTimes[i].hour + checkedMinute / 60;
                float hourX = canv.Left + (float)time * widthPerHour;
                float hourY = (canv.Bottom - (float)dayValuesFraction[i] * heightPerHour);
                hourPts.Add(new PointF(hourX, hourY));
            }

            return hourPts;
        }

        private (List<PointF> points, List<(bool isVertical, RectangleF rectangle, int valueIndex)> ranges) GenPts(List<PointF> hourPts, RectangleF frameBound)
        {
            var count = hourPts.Count;
            var pts = new List<PointF>();
            var recs = new List<(bool, RectangleF, int)>();
            for (int i = 0; i < count; i++)
            {
                var pt = hourPts[i];

                if (i == 0)
                {
                    //TODO: need to add a check if pt.X is 0
                    if (pt.Y != frameBound.Bottom)
                    {
                        // | p1 ------ p2
                        // |                  
                        // |                   
                        // |_0__________________24_

                        //Begining of the schedule 
                        var p1 = new PointF(frameBound.BottomLeft.X, pt.Y);
                        var p2 = new PointF(pt.X, pt.Y);
                        pts.Add(p1);
                        pts.Add(p2);

                        var rec = new RectangleF() { Location = p1, EndLocation = p2 };
                        rec.Top = rec.Top - 10;
                        rec.Bottom = rec.Bottom + 10;
                        recs.Add((false, rec, i));
                    }
                    else
                    {
                        // | 
                        // |                  
                        // |                   
                        // |_pt____________________

                        pts.Add(pt);
                    }

                    if (count == 1)
                    {
                        // | pt ----------------pt2
                        // |                  
                        // |                   
                        // |_0__________________24_
                        var pt2 = new PointF(frameBound.Right, pt.Y);
                        pts.Add(pt);
                        pts.Add(pt2);
                        // Horizontal 
                        var rec2 = new RectangleF() { Location = pt, EndLocation = pt2 };
                        rec2.Top = rec2.Top - 10;
                        rec2.Bottom = rec2.Bottom + 10;
                        recs.Add((false, rec2, i));


                    }

                }
                else if (i == count - 1)
                {
                    //  prePt0 ------ prePt1
                    //                   |
                    //                   |
                    //                   pt------ pt2(hour == 24)

                    var prePt0 = hourPts[i - 1];
                    var prePt1 = new PointF(pt.X, prePt0.Y);
                    pts.Add(prePt1);
                    // Horizontal 
                    var rec = new RectangleF() { Location = prePt0, EndLocation = prePt1 };
                    rec.Top = rec.Top - 10;
                    rec.Bottom = rec.Bottom + 10;
                    recs.Add((false, rec, i - 1));

                    // Vertical
                    var lowerPt = new PointF(prePt1.X, Math.Min(prePt1.Y, pt.Y));
                    var higherPt = new PointF(prePt1.X, Math.Max(prePt1.Y, pt.Y));
                    var rec1 = new RectangleF() { Location = lowerPt, EndLocation = higherPt };
                    rec1.Left = rec1.Left - 10;
                    rec1.Right = rec1.Right + 10;
                    rec1.Top = rec1.Top - 20;
                    rec1.Bottom = rec1.Bottom + 20;
                    //rec1.TopLeft = new PointF(rec1.TopLeft.X - 10, rec1.TopLeft.Y - 10);
                    //rec1.BottomRight = new PointF(rec1.BottomRight.X + 10, rec1.BottomRight.Y + 10);
                    recs.Add((true, rec1, i));


                    var pt2 = new PointF(frameBound.Right, pt.Y);
                    pts.Add(pt);
                    pts.Add(pt2);
                    // Horizontal 
                    var rec2 = new RectangleF() { Location = pt, EndLocation = pt2 };
                    rec2.Top = rec2.Top - 10;
                    rec2.Bottom = rec2.Bottom + 10;
                    recs.Add((false, rec2, i));

                }
                else
                {
                    //              (OR) pt
                    //                  |
                    //                  |
                    //  prePt0------ prePt1
                    //                  |
                    //                  |
                    //                  pt

                    var prePt0 = hourPts[i - 1];
                    var prePt1 = new PointF(pt.X, prePt0.Y);
                    pts.Add(prePt1);
                    pts.Add(pt);

                    // Horizontal 
                    var rec = new RectangleF() { Location = prePt0, EndLocation = prePt1 };
                    rec.Top = rec.Top - 10;
                    rec.Bottom = rec.Bottom + 10;
                    recs.Add((false, rec, i - 1));

                    // Vertical
                    var lowerPt = new PointF(prePt1.X, Math.Min(prePt1.Y, pt.Y));
                    var higherPt = new PointF(prePt1.X, Math.Max(prePt1.Y, pt.Y));
                    var rec1 = new RectangleF() { Location = lowerPt, EndLocation = higherPt };
                    rec1.Left = rec1.Left - 10;
                    rec1.Right = rec1.Right + 10;
                    rec1.Top = rec1.Top - 20;
                    rec1.Bottom = rec1.Bottom + 20;
                    //rec1.BottomRight = new PointF(rec1.BottomRight.X + 10, rec1.BottomRight.Y + 10);
                    recs.Add((true, rec1, i));


                }
            }

            return (pts, recs);
        }



    }
}
