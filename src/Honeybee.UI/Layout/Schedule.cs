using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Panel_Schedule : DynamicLayout
    {
        private ScheduleRulesetAbridged _scheduleRuleset;
        private ScheduleRuleAbridged _selectedScheduleRule = new ScheduleRuleAbridged("");
        private HB.ScheduleTypeLimit _selectedScheduleTypeLimits = new HB.ScheduleTypeLimit(Guid.NewGuid().ToString(), null, 0, 1);

        private ScheduleDay _selectedScheduleDay;
        private ScheduleDay _defaultScheduleDay => _scheduleRuleset.DaySchedules.First(_ => _.Identifier == _scheduleRuleset.DefaultDaySchedule);
        private double _intervalMinutes = 60; // hourly = 60, 15 minutes = 15, etc

        public (double start, double end) ScheduleTypeLimits { 
            get 
            {
                var limits = (0.0, 1.0);
                if (_selectedScheduleTypeLimits.LowerLimit.Obj is double low)
                    limits.Item1 = low;
                if (_selectedScheduleTypeLimits.UpperLimit.Obj is double up)
                    limits.Item2 = up;
                return limits;
            }
        }


        private double _scheduleTypelength => Math.Abs(ScheduleTypeLimits.end - ScheduleTypeLimits.start);
        public List<double> DayValues => _selectedScheduleDay.Values;
        public List<double> DayValuesFraction => DayValues.Select(_ => _ / _scheduleTypelength).ToList();
        public List<(int hour, int minute)> DayTimes => _selectedScheduleDay.Times.Select(_ => (_[0], _[1])).ToList();

        private Drawable _scheduleDaydrawable;
        private Drawable _calendarPanel;
        private Color _defaultColor = Color.FromArgb(184, 229, 255);

        public Panel_Schedule(HB.ScheduleRulesetAbridged scheduleRuleset)
        {
            var sch = scheduleRuleset;
            _scheduleRuleset = sch;
            _selectedScheduleDay = _defaultScheduleDay;
       
            //sch.ScheduleTypeLimit

            //(double start, double end) scheduleTypeLimits = (0, 100);
            //var scheduleTypelength = Math.Abs(scheduleTypeLimits.end - scheduleTypeLimits.start);
            this._selectedScheduleTypeLimits = new HB.ScheduleTypeLimit(Guid.NewGuid().ToString(), null, 0, 1);
            //this.DayValues = new List<double>() { 0, 0.5, 0.1 };
            //var dayValueFraction = dayValues.Select(_ => _ / scheduleTypelength);
            //this.DayTimes = new List<(int hour, int minute)>() { (0, 0), (6, 0), (18, 0) };

            //var DayValues = this.DayValues;

            //var layout = new DynamicLayout();
            this.DefaultPadding = new Padding(10);
            this.DefaultSpacing = new Size(5, 5);

            #region Panel for Schedule RuleSet rules

            var rulesPanel = new DynamicLayout();
            rulesPanel.Width = 200;
           
            var rules = _scheduleRuleset.ScheduleRules;
            var summerbtn = new Button() { Text = "Summer Design Day"};
            var winterbtn = new Button() { Text = "Winter Design Day" };
            var holidaybtn = new Button() { Text = "Holiday" };
            rulesPanel.AddRow("Special Day Profiles:");
            rulesPanel.AddRow(summerbtn);
            rulesPanel.AddRow(winterbtn);
            rulesPanel.AddRow(holidaybtn);
            rulesPanel.AddRow("Day Profiles:");
            var random = new Random();
            foreach (var item in rules)
            {
                var topBorder = new Label() { Height = 1, BackgroundColor = Colors.Black };
                rulesPanel.AddSeparateRow(topBorder);

                var radColor = Color.FromArgb((int)(0xFF000000 + (random.Next(0xFFFFFF) & 0x7F7F7F)));
                var ruleDay = sch.DaySchedules.First(_ => _.Identifier == item.ScheduleDay); 
                var ruleBtn = GenScheduleRuleBtn(item.ScheduleDay, radColor, ()=> _selectedScheduleDay = ruleDay);
                
                rulesPanel.AddSeparateRow(ruleBtn);

            }
            var defaultBorder = new Label() { Height = 1, BackgroundColor = Colors.Black };
            rulesPanel.AddSeparateRow(defaultBorder);
            var defaultScheduleBtn = GenScheduleRuleBtn("Default Day", _defaultColor, () => _selectedScheduleDay = _defaultScheduleDay);
            rulesPanel.AddSeparateRow(defaultScheduleBtn);
            var bottomBorder = new Label() { Height = 1, BackgroundColor = Colors.Black };
            rulesPanel.AddSeparateRow(bottomBorder);
            rulesPanel.AddSeparateRow(null);
            #endregion




            var schName_Tb = new TextBox() { Text = _scheduleRuleset.DisplayName ?? _scheduleRuleset.Identifier };
            var dayName_Tb = new TextBox() { Text = _selectedScheduleDay.DisplayName ?? _selectedScheduleDay.Identifier, Width = 300 };
            var lowerLimit_TB = new NumericMaskedTextStepper<double>() { Value = ScheduleTypeLimits.start, PlaceholderText= "0" };
            var higherLimit_TB = new NumericMaskedTextStepper<double>() { Value = ScheduleTypeLimits.end, PlaceholderText = "0" };
      

            var mouseHoverValue_TB = new NumericMaskedTextBox<double>() { PlaceholderText = "Mouse over lines", Height = 20, Font= Fonts.Sans(8) };
      
            var label = new Label() { Text = "" };


            #region LeftPanel

            _scheduleDaydrawable = new Drawable(true) { };
            _scheduleDaydrawable.Size = new Size(600, 400);
            //drawable.BackgroundColor = Colors.Blue;
            var location = new Point(0, 0);
            var canvas = _scheduleDaydrawable.Bounds;
            canvas.Size = canvas.Size - 20;
            canvas.TopLeft = new Point(50, 10);


            #region MouseEvent
            var allMouseHoverRanges = new List<(bool isVertical, RectangleF rectangle, int valueIndex)>();
            var mouseHoveredRanges = new List<(bool isVertical, RectangleF rectangle, int valueIndex)>();
            _scheduleDaydrawable.MouseMove += (s, e) =>
            {
                var mouseLoc = e.Location;
                //label.Text = $"{Math.Round(mouseLoc.X)},{Math.Round(mouseLoc.Y)}";
                //Draw mouse hover over ranges
                var hovered = allMouseHoverRanges.Where(_ => _.rectangle.Contains(mouseLoc));
                if (hovered.Any())
                {
                    mouseHoveredRanges = hovered.ToList();
                    _scheduleDaydrawable.Update(canvas);
                }
                else
                {
                    if (mouseHoveredRanges.Any())
                    {
                        ////var preRec = mouseHoveredRanges.First().rectangle;
                        mouseHoveredRanges.Clear();
                        //drawable.Update(new Rectangle(preRec));
                        _scheduleDaydrawable.Update(canvas);
                    }

                }

            };

            var startDragging = false;
            var mouseHoveredRangesForDragging = new List<(bool isVertical, RectangleF rectangle, int valueIndex)>();
            _scheduleDaydrawable.MouseDown += (s, e) =>
            {
                if (mouseHoveredRanges.Any())
                {
                    startDragging = e.Buttons == MouseButtons.Primary;
                    mouseHoveredRangesForDragging = mouseHoveredRanges;
                    //label.Text = startDragging.ToString();
                }

            };
            _scheduleDaydrawable.MouseUp += (s, e) =>
            {
                if (startDragging)
                {
                    mouseHoveredRangesForDragging.Clear();
                    startDragging = false;
                    _scheduleDaydrawable.Update(canvas);
                }

            };

            _scheduleDaydrawable.LostFocus += (s, e) =>
            {
                if (startDragging)
                {
                    mouseHoveredRangesForDragging.Clear();
                    startDragging = false;
                    _scheduleDaydrawable.Update(canvas);
                }


            };

            // mouse move for dragging
            _scheduleDaydrawable.MouseMove += (s, e) =>
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
                            _scheduleDaydrawable.Update(canvas);
                        }


                    }
                    else
                    {
                        var mappedValue = (canvas.Bottom - mouseLoc.Y) / canvas.Height;
                        var decimalPlaces = (this._scheduleTypelength/100).ToString().Split('.').Last().Length;
                        var pecent = Math.Round(mappedValue, decimalPlaces);

                        DayValues[valueIndex] = pecent * this._scheduleTypelength;
                        _scheduleDaydrawable.Update(canvas);
                    }
                    
                   
                }
                else
                {
                    mouseHoveredRangesForDragging.Clear();
                    startDragging = false;
                }



            };


            _scheduleDaydrawable.MouseDoubleClick += (s, e) =>
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
                        _scheduleDaydrawable.Update(canvas);

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
                        _scheduleDaydrawable.Update(new Rectangle(hovered.rectangle));
                    }

              

                    
                }

            };
            #endregion

            var hoveredValueIndex = 0;
            _scheduleDaydrawable.Paint += (s, e) =>
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

                        _scheduleDaydrawable.Update(canvas);
                    }


                }

            };


            lowerLimit_TB.ValueChanged += (s, e) => 
            { 
                this._selectedScheduleTypeLimits.LowerLimit = lowerLimit_TB.Value;
                _scheduleDaydrawable.Update(canvas);
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
                this._selectedScheduleTypeLimits.UpperLimit = higherLimit_TB.Value;
                _scheduleDaydrawable.Update(canvas);
            };

            var btn = new Button() { Text = "HBData" };
            btn.Click += (s, e) =>
            {
                Dialog_Message.Show(this, this._selectedScheduleTypeLimits.ToJson());
                //MessageBox.Show(this, string.Join<double>(", ", dayValues));
            };

            var layoutLeft = new DynamicLayout();
            layoutLeft.DefaultPadding = new Padding(5);
            layoutLeft.DefaultSpacing = new Size(3, 3);
            layoutLeft.AddSeparateRow("Schedule Name:", schName_Tb);

            var layoutRule = new DynamicLayout();
            layoutRule.DefaultPadding = new Padding(25, 0, 0, 0);
            layoutRule.DefaultSpacing = new Size(3, 3);
            layoutRule.AddSeparateRow(new Label { Text = "Rule Name:", Width = 90 }, dayName_Tb, null);
            var dateRanges = ApplyDateRangeControls().ToList();
            dateRanges.Insert(0, new Label { Text = "Date Range:", Width = 90 });
            dateRanges.Add(null);
            layoutRule.AddSeparateRow(dateRanges.ToArray());
            var dayOfweek = ApplyDayOfWeekControls().ToList();
            dayOfweek.Insert(0, new Label { Text = "Apply To:", Width = 90 });
            dayOfweek.Add(null);
            layoutRule.AddSeparateRow(dayOfweek.ToArray());

            layoutLeft.AddSeparateRow(layoutRule);
            layoutLeft.AddSeparateRow(new Label {Width = 47 }, new Label { Text = "Lower Limit:", Width = 90 }, lowerLimit_TB, "Upper Limit:", higherLimit_TB, null, label, mouseHoverValue_TB);
            //this.AddSeparateRow(null, label, mouseHoverValue_TB);
            layoutLeft.AddSeparateRow(_scheduleDaydrawable);
            layoutLeft.AddSeparateRow(this.AddIntervalButtons());
            layoutLeft.AddRow(btn);
            #endregion

            #region RightPanel
            var calendarSize = new Size(200, 1550);
      
            //Background color
            var calendarBackground = new Drawable(true);
            calendarBackground.Size = calendarSize;
            calendarBackground.Paint += Drawable_FillBackground;

            //Middle layer
            _calendarPanel = new Drawable(true);
            _calendarPanel.Size = calendarSize;

            //_calendarPanel.Paint += Drawable_FillBackground;
            _calendarPanel.Paint += Drawable_FillForeground;
            //_calendarPanel.Paint += Drawable_Grid;
            //_calendarPanel.Paint += Drawable_Label;

            //Foreground text/grids
            var calendarForeground = new Drawable(true);
            calendarForeground.Size = calendarSize;
            calendarForeground.Paint += Drawable_Grid;
            calendarForeground.Paint += Drawable_Label;

            // combine layers
            var layoutLayers = new PixelLayout();
            layoutLayers.Height = 600;
            layoutLayers.Add(calendarBackground, 0, 0);
            layoutLayers.Add(_calendarPanel, 0, 0);
            layoutLayers.Add(calendarForeground, 0, 0);


            var layoutRight = new DynamicLayout();
            layoutRight.Height = 600;
            layoutRight.BeginScrollable();
            layoutRight.AddRow(layoutLayers);

            #endregion


            this.AddRow(rulesPanel, layoutLeft, layoutRight, null);

        }
        private Control[] GenScheduleRuleBtn(string btnName, Color color, Action setAction)
        {
            var left = new Label() { Width = 1, BackgroundColor= Colors.Black };
            var middle = new Label() { Width = 1, BackgroundColor = Colors.Black };
            var right = new Label() { Width = 1, BackgroundColor = Colors.Black };

            var labelColor = new Label() { Width = 10 };
            labelColor.BackgroundColor = color;


            var defaultDaybtn = new Label() { Text = btnName, Height = 25 };
            defaultDaybtn.TextAlignment = TextAlignment.Left;
            defaultDaybtn.Wrap = WrapMode.None;

            var defaultBackgroundColor = Color.FromArgb(235, 235, 235);
            var pressColor = Color.FromArgb(200, 200, 200);
            defaultDaybtn.BackgroundColor = defaultBackgroundColor;
            defaultDaybtn.MouseMove += (s, e) =>
            {
                defaultDaybtn.BackgroundColor = pressColor;
            };
            defaultDaybtn.MouseLeave += (s, e) =>
            {
                defaultDaybtn.BackgroundColor = defaultBackgroundColor;
            };
          
            defaultDaybtn.MouseDown += (s, e) =>
            {
                defaultDaybtn.BackgroundColor = pressColor;
                //var rad = new Random();
                //var dayValues = new List<double>() { 0, rad.NextDouble(), rad.NextDouble() };
                //var dayTimes = new List<List<int>>() { new List<int> { 0, 0 }, new List<int> { 6, 0 }, new List<int> { 18, 0 } };
                //var newday = new ScheduleDay("ddd", dayValues, null, dayTimes) { };

                ////this.DayValues = new List<double>() { 0, 0.5, 0.1 };
                ////var dayValueFraction = dayValues.Select(_ => _ / scheduleTypelength);
                ////this.DayTimes = new List<(int hour, int minute)>() { (0, 0), (6, 0), (18, 0) };
                //this._selectedScheduleDay = newday;
                setAction();
                _scheduleDaydrawable.Update(new Rectangle(new Size(600,400)));


            };

            return new Control[] { left, labelColor, middle, defaultDaybtn };


        }


        private Control[] ApplyDateRangeControls()
        {
            var sDate = new DateTimePicker();
            sDate.Mode = DateTimePickerMode.Date;
            sDate.MinDate = new DateTime(2017, 1, 1);
            sDate.MaxDate = new DateTime(2017, 12, 31);
            sDate.Value = sDate.MinDate;
            sDate.ValueChanged += (s, e) => {
                var selected = sDate.Value.Value;
                _selectedScheduleRule.StartDate = new List<int> { selected.Month, selected.Day };
                RefreshCalendar();
            };
          

            var eDate = new DateTimePicker();
            eDate.Mode = DateTimePickerMode.Date;
            eDate.MinDate = new DateTime(2017, 1, 1);
            eDate.MaxDate = new DateTime(2017, 12, 31);
            eDate.Value = eDate.MaxDate;
            eDate.ValueChanged += (s, e) => {
                var selected = eDate.Value.Value;
                _selectedScheduleRule.EndDate = new List<int> { selected.Month, selected.Day };
                RefreshCalendar();
            };
            return new Control[] {sDate, eDate };
        }
        private Control[] ApplyDayOfWeekControls()
        {
            var scheduleRule = _selectedScheduleRule;
            //var nonDefaultSchedulrRulePanel = new DynamicLayout();
            var width = 30;
            var btnSU = new Button() { Text = "S", Width = width };
            var btnMO = new Button() { Text = "M", Width = width };
            var btnTU = new Button() { Text = "T", Width = width };
            var btnWE = new Button() { Text = "W", Width = width };
            var btnTH = new Button() { Text = "T", Width = width };
            var btnFI = new Button() { Text = "F", Width = width };
            var btnSA = new Button() { Text = "S", Width = width };
            var btnEnabledColor = Color.FromArgb(191,229,249);
            var btnDefaultColor = btnSA.BackgroundColor;

            btnSU.Click += (s, e) => {
                scheduleRule.ApplySunday = !scheduleRule.ApplySunday;
                btnSU.BackgroundColor = scheduleRule.ApplySunday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnMO.Click += (s, e) => {
                scheduleRule.ApplyMonday = !scheduleRule.ApplyMonday;
                btnMO.BackgroundColor = scheduleRule.ApplyMonday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnTU.Click += (s, e) => {
                scheduleRule.ApplyTuesday = !scheduleRule.ApplyTuesday;
                btnTU.BackgroundColor = scheduleRule.ApplyTuesday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnWE.Click += (s, e) => {
                scheduleRule.ApplyWednesday = !scheduleRule.ApplyWednesday;
                btnWE.BackgroundColor = scheduleRule.ApplyWednesday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnTH.Click += (s, e) => {
                scheduleRule.ApplyThursday = !scheduleRule.ApplyThursday;
                btnTH.BackgroundColor = scheduleRule.ApplyThursday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnFI.Click += (s, e) => {
                scheduleRule.ApplyFriday = !scheduleRule.ApplyFriday;
                btnFI.BackgroundColor = scheduleRule.ApplyFriday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnSA.Click += (s, e) => {
                scheduleRule.ApplySaturday = !scheduleRule.ApplySaturday;
                btnSA.BackgroundColor = scheduleRule.ApplySaturday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            //nonDefaultSchedulrRulePanel.AddRow(null, "Apply to", btnSU, btnMO, btnTU, btnWE, btnTH, btnFI, btnSA, null);
            return new Control[] {btnSU, btnMO, btnTU, btnWE, btnTH, btnFI, btnSA };
        }


        private void DrawCalendarGrids(Graphics graphic)
        {
            //var graphic = e.Graphics;
            var dayW = 25;
            var dayH = 15;
            var monthW = dayW * 7;
            var monthH = dayH * 5;
            // vertical lines
            

            void DrawMonthGrid()
            {
                for (int i = 0; i <= 7; i++)
                {
                    var x = i * dayW;
                    graphic.DrawLine(Colors.Black, x, 0, x, monthH);
                }
                // horizontal lines
                for (int i = 0; i <= 5; i++)
                {
                    var y = i * dayH;
                    graphic.DrawLine(Colors.Black, 0, y, monthW, y);

                }
            }
        }



        private void RedrawMonth(int startDay, Graphics graphic)
        {
            var daysPerMonth = new[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            var weekDays = new[] { "S", "M", "T", "W", "T", "F", "S" };
            var dayW = 25;
            var dayH = 15;
            var defaultColor = Colors.Aqua;

            var monthRec = new Rectangle(0, 0, dayW * 7, dayH * 5);
            //// draw grid
            //// fill day cell color
            //for (int j = 0; j < 35; j++)
            //{

            //}
            var row = 0;
            var indexInRow = 0;

            var font = Fonts.Sans(8);
            var DOY = 0;

            var pt = new Point();
            var coloum = 1;
            var month = 0;

            var setColor = defaultColor;

            foreach (var days in daysPerMonth)
            {
                if (month > 0)
                {
                    pt.Y = pt.Y + dayH * 2;
                }

                var monthPt = new Point(0, pt.Y);
                graphic.DrawText(font, Colors.Black, monthPt.X, monthPt.Y, (month + 1).ToString());
                pt.Y = pt.Y + dayH;
                monthPt.Y = monthPt.Y + dayH;
                for (int i = 0; i < 7; i++)
                {
                    var rec = new Rectangle(i * dayW, monthPt.Y, dayW, dayH);

                    var date = weekDays[i];
                    var size = font.MeasureString(date);


                    graphic.DrawText(font, Colors.Black, rec.Center.X - size.Width / 2, rec.Center.Y - size.Height / 2, date);
                    monthPt.X = monthPt.X + dayW;
                }


                pt.Y = pt.Y + dayH;
                for (int i = 0; i < days; i++)
                {
                    var weekOfMonth = (int)Math.Floor((double)i / 7);
                    var dayOfWeek = (i - row * 7);
                    
                    
                    if (pt.X + dayW >= monthRec.Width * coloum)
                    {
                        pt.Y = pt.Y + dayH;
                        pt.X = monthRec.Width * (coloum - 1);
                    }
                    else if (DOY != 0)
                    {
                        pt.X = pt.X + dayW;
                    }

                    dayOfWeek = (int)(pt.X / dayW);
                    if (_selectedScheduleRule.ApplySaturday && dayOfWeek == 6)
                    {
                        setColor = Colors.Red;
                    }
                    else
                    {
                        setColor = defaultColor;
                    }

                    var rec = new Rectangle(pt.X, pt.Y, dayW, dayH);
                    graphic.FillRectangle(setColor, rec);
                    graphic.DrawRectangle(Colors.Black, rec);
                    var date = (i + 1).ToString();
                    var size = font.MeasureString(date);

                    graphic.DrawText(font, Colors.Black, rec.Center.X - size.Width / 2, rec.Center.Y - size.Height / 2, date);
                    //row = i == daysPerMonth[month]-1 ? row + 1 : row;
                    DOY++;
                }

                month++;
            }
        }


        private Control[] AddIntervalButtons()
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
            return new[] { null, btn1, btn2, btn3, null };
          
        }

        #region Left DaySchedule Methods

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


        #endregion

        #region Right Calendar Methods

        private SizeF _dayCellSize = new SizeF(25, 15);
        private PointF _calendarBasePoint = new PointF(5, 5);

        private static List<(RectangleF rec, DateTime date)> _dayRectangles;

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

        private static List<RectangleF> _monthlyBounds;

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
                return _monthlyBounds;
            }
        }

        private static List<RectangleF> _voidBounds;

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

        private static List<(PointF s, PointF e)> _gridLines;

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


        private void RefreshCalendar()
        {
            _calendarPanel.Update(new Rectangle(new Size(200, 1500)));
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

            var color = _defaultColor;
            var monthlyBounds = this.MonthlyBounds;
            graphic.FillRectangles(color, monthlyBounds);

            //Void 
            var voidBounds = this.VoidBounds;
            graphic.FillRectangles(Color.FromArgb(235,235,235), voidBounds);

        }

        private void Drawable_FillForeground(object sender, PaintEventArgs e)
        {
            var graphic = e.Graphics;

            var color = Colors.Blue;
            var appliedDays = new List<DayOfWeek> { };

            if (_selectedScheduleRule.ApplySunday)
                appliedDays.Add(DayOfWeek.Sunday); 
            if (_selectedScheduleRule.ApplyMonday)
                appliedDays.Add(DayOfWeek.Monday);
            if (_selectedScheduleRule.ApplyTuesday)
                appliedDays.Add(DayOfWeek.Tuesday);
            if (_selectedScheduleRule.ApplyWednesday)
                appliedDays.Add(DayOfWeek.Wednesday);
            if (_selectedScheduleRule.ApplyThursday)
                appliedDays.Add(DayOfWeek.Thursday);
            if (_selectedScheduleRule.ApplyFriday)
                appliedDays.Add(DayOfWeek.Friday);
            if (_selectedScheduleRule.ApplySaturday)
                appliedDays.Add(DayOfWeek.Saturday);

            var s = _selectedScheduleRule.StartDate ?? new List<int> { 1, 1 };
            var startDate = new DateTime(2017, s[0], s[1]);
            var startDOY = startDate.DayOfYear;

            var end = _selectedScheduleRule.EndDate ?? new List<int> { 12, 31 };
            var endDate = new DateTime(2017, end[0], end[1]);
            var endDOY = endDate.DayOfYear;


            var selectedDays = DayRectangles.Skip(startDOY -1).Take(endDOY - startDOY+1).Where(_ => appliedDays.Contains(_.date.DayOfWeek));
            var recs = selectedDays.Select(_ => _.rec);
            graphic.FillRectangles(Colors.Red, recs);


        }

        private void Drawable_Label(object sender, PaintEventArgs e)
        {

            var graphic = e.Graphics;

            var font = Fonts.Sans(8);
            var brush = Brushes.Black;
            var brush2 = Brushes.Gray;
            foreach (var item in DayRectangles)
            {
                // Day
                var cel = item.rec;
                var day = item.date.Day.ToString();
                graphic.DrawText(font, brush, cel, day, FormattedTextWrapMode.Word, FormattedTextAlignment.Center);
            }

            var monthRecs = MonthlyBounds;
            var cellSize = _dayCellSize;
            var weekDays = new[] { "S", "M", "T", "W", "T", "F", "S" };
            var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            var m = 0;
            foreach (var item in monthRecs)
            {
                var loc = item.Location;
                loc.Y -= cellSize.Height;
                // Weekday
                for (int i = 0; i < 7; i++)
                {
                    var text = weekDays[i];
                    var cel = new RectangleF(loc, cellSize);
                    graphic.DrawText(font, brush2, cel, text, FormattedTextWrapMode.Word, FormattedTextAlignment.Center);
                    loc.X += cellSize.Width;
                }

                // Month
                var mText = months[m].ToUpper();
                var mLoc = item.Location;
                mLoc.Y -= cellSize.Height *2;
                //mLoc.X += cellSize.Width * 3;
                var mCel = new RectangleF(mLoc, cellSize);
                graphic.DrawText(font, brush, mCel, mText, FormattedTextWrapMode.Word, FormattedTextAlignment.Center);
                m++;
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
            var monthTop = dayH * 2.5F;

            var lines = new List<(PointF s, PointF e)>();
            var monthlyRecs = new List<RectangleF>();

            // draw monthly
            for (int i = 0; i < 12; i++)
            {
                var x = left;
                var y = i * (monthH + monthTop) + monthTop + top;
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
            var monthTop = dayH * 2.5F;

            var recs = new List<(RectangleF rec, DateTime date)>();

            // draw monthly
            for (int i = 0; i < 12; i++)
            {
                var startDay = new DateTime(2017, i + 1, 1);
                var x = left;
                var y = i * (monthH + monthTop) + monthTop + top;

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
                    recs.Add((rec, new DateTime(2017, month, i + 1)));
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

                var startCell = dayCells[startDOY - 1].rec;
                var endCell = dayCells[endDOY - 1].rec;
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

        #endregion

    }
}
