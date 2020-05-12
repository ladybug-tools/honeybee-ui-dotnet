using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class Panel_Schedule: DynamicLayout
    {
        private double _intervalMinutes = 60; // hourly = 60, 15 minutes = 15, etc

        public Panel_Schedule()
        {
            //var layout = new DynamicLayout();
            this.DefaultPadding = new Padding(10);
            this.DefaultSpacing = new Size(5, 5);

            (double start, double end) scheduleTypeLimits = (0, 1);
            var dayValues = new List<double>() { 0.16, 0.5, 0 };
            var dayTimes = new List<(int hour, int minute)>() { (0, 0), (6, 0), (18, 0) };
            var hours = dayTimes.Select(_ => _.hour).ToArray();


            var mouseHoverValue_TB = new NumericMaskedTextBox<double>() { PlaceholderText = "Mouse over lines" };
            var label = new Label() { Text = "" };
            this.AddSeparateRow(null, label, mouseHoverValue_TB);


            var drawable = new Drawable(true) { };
            drawable.Size = new Size(600, 400);
            var location = new Point(0, 0);
            var canvas = drawable.Bounds;
            canvas.Size = canvas.Size - 30;
            canvas.TopLeft = new Point(30, 15);


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
                        var preRec = mouseHoveredRanges.First().rectangle;
                        mouseHoveredRanges.Clear();
                        drawable.Update(new Rectangle(preRec));
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
                            beforeTime = dayTimes[beforeIndex];
                        }
                        var beforeTime2 = beforeTime.Item1 + beforeTime.Item2 / 60;
                        // get maximum movable right bound; 
                        var nextTime = (24, 0);
                        var nextIndex = hovered.valueIndex + 1;
                        if (nextIndex < dayTimes.Count)
                        {
                            nextTime = dayTimes[nextIndex];
                        }
                        var nextTime2 = nextTime.Item1 + nextTime.Item2 / 60;

                        
                        if (newTime < nextTime2 && newTime > beforeTime2)
                        {
                            dayTimes[hovered.valueIndex] = (newHour, newMinute);
                            drawable.Update(canvas);
                        }


                    }
                    else
                    {
                        //TODO: need to do conversion based on schedule's max and min. For now, it just calculated %.
                        //var mappedValue = (canvas.Height - (mouseLoc - canvas.Location).Y) / canvas.Height;
                        var mappedValue = (canvas.Bottom - mouseLoc.Y) / canvas.Height;
                        var pecent = Math.Round(mappedValue, 3);

                        //label.Text = $"{canvas.Bottom} - {mouseLoc.Y} / {canvas.Height} = {pecent}";
                        dayValues[valueIndex] = pecent;
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
                        dayTimes.RemoveAt(hovered.valueIndex);
                        dayValues.RemoveAt(hovered.valueIndex);
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
                        beforeTime = dayTimes[beforeIndex];
                    var beforeTime2 = beforeTime.Item1 + beforeTime.Item2 / 60;
                    // get maximum movable right bound; 
                    var nextTime = (24, 0);
                    var nextIndex = hovered.valueIndex + 1;
                    if (nextIndex < dayTimes.Count)
                        nextTime = dayTimes[nextIndex];
                    var nextTime2 = nextTime.Item1 + nextTime.Item2 / 60;

                    //var newDateTimes = dayTimes.ToList();
                    if (mappedTimeNormalized < nextTime2 && mappedTimeNormalized > beforeTime2)
                    {
                        var insertIndex = hovered.valueIndex;
                        dayTimes.Insert(insertIndex+1, (hour, minute));
                        var addValue = dayValues[insertIndex];
                        dayValues.Insert(insertIndex+1, addValue);
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

                var hourPts = GenHourPts(dayValues, dayTimes, canvas);
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
                        var time = dayTimes[hoveredRec.valueIndex];
                        valueToDisplay = TimeSpan.Parse($"{time.hour}:{time.minute}").ToString(@"hh\:mm");
                        valueToDisplay = $" {valueToDisplay}";
                    }
                    else
                    {
                        valueToDisplay = dayValues[hoveredRec.valueIndex].ToString();
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

                        var hoveredValue = dayValues[hoveredRec.valueIndex];
                        if (mouseHoverValue_TB.SelectedText != hoveredValue.ToString())
                        {
                            mouseHoverValue_TB.Value = hoveredValue;
                            mouseHoverValue_TB.SelectAll();
                        }
                    }
                    
                }

                foreach (var pt in allPts)
                {
                    graphics.FillRectangle(Colors.Black, new RectangleF(pt.X - 3, pt.Y - 3, 6, 6));
                }

                var pen = new Pen(Colors.Black, 2);
                graphics.DrawLines(pen, allPts);


                graphics.DrawRectangle(Colors.Black, canvas);

                //Draw horizontal ticks
                var markCount = 6;
                var hourInterval = 24 / markCount;
                var hourStartPt = canvas.BottomLeft;
                var hourEdPt = canvas.BottomRight;

                var widthPerInterval = (hourEdPt.X- hourStartPt.X) / markCount;
                var bottom = canvas.Bottom;
                var left = canvas.Left;
              
                var tickLength = 8;
                var tickfont = Fonts.Sans(8);
                //var font = Fonts.Sans(10);
                for (int i = 0; i <= markCount; i++)
                {
                    var p1 = new PointF(left + i * widthPerInterval, bottom);
                    var p2 = new PointF(left + i * widthPerInterval, bottom + tickLength);
                    graphics.DrawLine(Colors.Black, p1, p2 );

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
                    graphics.DrawText(tickfont, Colors.Black, p2.X - textSize.Width -2, p2.Y - textSize.Height / 2, tickText);
                }

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
                        var oldValue = dayValues[valueIndex];
                  
                        //preRec.Top = (float) Math.Max(newUserInput, oldValue);
                        //preRec.Bottom = (float)Math.Min(newUserInput, oldValue);
                        dayValues[valueIndex] = newUserInput;

                        drawable.Update(canvas);
                    }


                }

            };


            this.AddRow(drawable);
            var btn = new Button() { Text = "HBData" };
            btn.Click += (s, e) =>
            {
                MessageBox.Show(this, string.Join<double>(", ", dayValues));
            };
            
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
        private List<PointF> GenHourPts(List<double> dayValues, List<(int hour, int minute)> dayTimes, RectangleF frameBound)
        {
            var canv = frameBound;
            var widthPerHour = canv.Width / 24;
            var heightPerHour = canv.Height;
            var count = dayValues.Count();
            var hourPts = new List<PointF>();
            for (int i = 0; i < count; i++)
            {
                var checkedMinute = NormalizeMinute(dayTimes[i].minute);
                var time = (double)dayTimes[i].hour + checkedMinute / 60;
                var hourX = canv.Left + time * widthPerHour;
                var hourY = (canv.Bottom - (float)dayValues[i] * heightPerHour);
                hourPts.Add(new PointF((float)hourX, hourY));
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
