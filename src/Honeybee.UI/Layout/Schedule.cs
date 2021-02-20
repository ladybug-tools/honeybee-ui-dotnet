using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class Panel_Schedule : DynamicLayout
    {
        private static ScheduleRulesetViewModel _vm;

        private double _intervalMinutes = 60; // hourly = 60, 15 minutes = 15, etc
     

        private double _scheduleTypelength => Math.Abs(_vm.UpperLimit - _vm.LowerLimit);
        //public List<double> DayValues => _vmSelectedDay.Values;
        public List<double> DayValuesFraction => _vm.SchDayValues.Select(_ => Math.Abs(_ - _vm.LowerLimit) / _scheduleTypelength).ToList();
        //public List<List<int>> DayTimes => _vmSelectedDay.Times;

        private DynamicLayout _layoutSchRule;
        private Drawable _scheduleDaydrawable;
        private Drawable _calendarPanel;
        private Color _defaultColor = Color.FromArgb(184, 229, 255);
        private List<(Color color, HB.ScheduleRuleAbridged schRule)> colorsetPerRuleset = new List<(Color, HB.ScheduleRuleAbridged)>();
        private Size _calendarSize = new Size(200, 1550);

        public Panel_Schedule(HB.ScheduleRuleset scheduleRuleset)
        {
            _vm = new ScheduleRulesetViewModel(scheduleRuleset);
          

            this.DefaultPadding = new Padding(10);
            this.DefaultSpacing = new Size(5, 5);

            #region Panel for Schedule RuleSet rules
            var rightPanel = new DynamicLayout();
            var rules = _vm.ScheduleRules;
            var rulesPanel = CreateScheduleRulesPanel(rules);
            rightPanel.AddRow(rulesPanel);

            // add new rule
            var new_btn = new Button() { Text = "Add" };
            new_btn.Command = AddRuleCommand;
            new_btn.CommandParameter = rulesPanel;
            rightPanel.AddRow(new_btn);

            var hbData_btn = new Button() { Text = "HBData" };
            hbData_btn.Command = HBDataCommand;

            rightPanel.AddRow(hbData_btn);
            rightPanel.AddRow(null);
            #endregion


            var schName_Tb = new TextBox() { };
            _vm.SchRuleset_hbObj.DisplayName = _vm.SchRuleset_hbObj.DisplayName ?? _vm.SchRuleset_hbObj.Identifier;
            schName_Tb.TextBinding.Bind(_vm, c => c.DisplayName);

            var dayName_Tb = new TextBox() { Width = 300 };
            //_vmSelectedDay.hbObj.DisplayName = _vmSelectedDay.hbObj.DisplayName ?? _vmSelectedDay.hbObj.Identifier;
            dayName_Tb.TextBinding.Bind(_vm, c => c.SchDayName);


            var lowerLimit_TB = new MaskedTextBox() { };
            var lowNumMask = new NumericMaskedTextProvider() { AllowDecimal = true, AllowSign = true };
            lowerLimit_TB.Provider = lowNumMask;
            lowerLimit_TB.TextBinding.Bind(() => _vm.LowerLimit.ToString(), v => _vm.LowerLimit = double.Parse(v));

            var higherLimit_TB = new MaskedTextBox() { };
            var highNumMask = new NumericMaskedTextProvider() { AllowDecimal = true, AllowSign = true };
            higherLimit_TB.Provider = highNumMask;
            higherLimit_TB.TextBinding.Bind(() => _vm.UpperLimit.ToString(), v => _vm.UpperLimit = double.Parse(v));

            //var label = new Label() { Text = "Mouse over lines" };
            var mouseHoverValue_TB = new NumericMaskedTextStepper<double>() { Height = 20, Font = Fonts.Sans(8) };
            mouseHoverValue_TB.ShowStepper = false;
            mouseHoverValue_TB.PlaceholderText = "Mouse over lines";


            #region LeftPanel

            _scheduleDaydrawable = new Drawable(true) { };
            _scheduleDaydrawable.Size = new Size(600, 400);
            //drawable.BackgroundColor = Colors.Blue;
            var location = new Point(0, 0);
            var canvas = _scheduleDaydrawable.Bounds;
            canvas.Size = canvas.Size - 20;
            canvas.TopLeft = new Point(50, 15);


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
                        var mappedTime = (mouseLoc.X - canvas.Left) / canvas.Width * 24;
                        var timeNormalized = NormalizeHourMinute(mappedTime);
                        var newHour = timeNormalized.hour;
                        var newMinute = timeNormalized.minute;
                        var newTime = newHour + newMinute / 60;


                        // get minimum movable left bound; 
                        var beforeTime = 0;
                        var beforeIndex = hovered.valueIndex - 1;
                        if (beforeIndex >= 0)
                        {
                            var timeValue = _vm.SchDayTimes[beforeIndex];
                            beforeTime = timeValue[0] + timeValue[1] / 60;
                        }


                        // get maximum movable right bound; 
                        var nextTime = 24;
                        var nextIndex = hovered.valueIndex + 1;
                        if (nextIndex < _vm.SchDayTimes.Count)
                        {
                            var timeValue = _vm.SchDayTimes[nextIndex];
                            nextTime = timeValue[0] + timeValue[1] / 60;
                        }


                        if (newTime < nextTime && newTime > beforeTime)
                        {
                            _vm.SchDayTimes[valueIndex] = new List<int>() { newHour, newMinute };
                            //DayTimes[valueIndex] = (newHour, newMinute);
                            //mouseHoverValue_TB.Text = TimeSpan.Parse($"{DayTimes[valueIndex].hour}:{DayTimes[valueIndex].minute}").ToString(@"hh\:mm");
                            _scheduleDaydrawable.Update(canvas);
                        }


                    }
                    else
                    {
                        var mappedPercent = (canvas.Bottom - mouseLoc.Y) / canvas.Height;
                        mappedPercent = Math.Min(1, mappedPercent);
                        mappedPercent = Math.Max(0, mappedPercent);

                        var decimalPlaces = (this._scheduleTypelength / 100).ToString().Split('.').Last().Length;
                        var checkedValue = mappedPercent * this._scheduleTypelength + _vm.LowerLimit;
                        _vm.SchDayValues[valueIndex] = Math.Round(checkedValue, decimalPlaces);
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
                        _vm.SchDayTimes.RemoveAt(hovered.valueIndex);
                        _vm.SchDayValues.RemoveAt(hovered.valueIndex);
                        _scheduleDaydrawable.Update(canvas);

                        return;
                    }

                    //var mouseLoc = e.Location;
                    var mappedTimeRaw = (mouseLoc.X - canvas.Left) / canvas.Width * 24;

                    var timeNormalized = NormalizeHourMinute(mappedTimeRaw);
                    var hour = timeNormalized.hour;
                    var minute = timeNormalized.minute;
                    var mappedTimeNormalized = hour + (double)(minute / 60.0);

                    // get minimum movable left bound; 
                    var beforeTime = new[] { 0, 0 }.ToList();
                    var beforeIndex = hovered.valueIndex;
                    if (beforeIndex >= 0)
                        beforeTime = _vm.SchDayTimes[beforeIndex];
                    var beforeTime2 = beforeTime[0] + beforeTime[1] / 60;
                    // get maximum movable right bound; 
                    var nextTime = new[] { 24, 0 }.ToList();
                    var nextIndex = hovered.valueIndex + 1;
                    if (nextIndex < _vm.SchDayTimes.Count)
                        nextTime = _vm.SchDayTimes[nextIndex];
                    var nextTime2 = nextTime[0] + nextTime[1] / 60;

                    //var newDateTimes = dayTimes.ToList();
                    if (mappedTimeNormalized < nextTime2 && mappedTimeNormalized > beforeTime2)
                    {
                        var insertIndex = hovered.valueIndex;
                        _vm.SchDayTimes.Insert(insertIndex + 1, new List<int> { hour, minute });
                        var addValue = _vm.SchDayValues[insertIndex];
                        _vm.SchDayValues.Insert(insertIndex + 1, addValue);
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

                    //if (startDragging && hoveredRec.isVertical)
                    //{
                    //    mouseHoverValue_TB.Value = mouseLoc.X;
                    //    mouseHoverValue_TB.Enabled = false;
                    //    return;
                    //}

                    var valueToDisplay = string.Empty;
                    if (hoveredRec.isVertical)
                    {
                        var time = _vm.SchDayTimes[hoveredRec.valueIndex];
                        valueToDisplay = TimeSpan.Parse($"{time[0]}:{time[1]}").ToString(@"hh\:mm");
                        valueToDisplay = $" {valueToDisplay}";
                    }
                    else
                    {
                        valueToDisplay = _vm.SchDayValues[hoveredRec.valueIndex].ToString();
                    }
                    var font = Fonts.Sans(8);
                    var textSize = font.MeasureString(valueToDisplay);
                    graphics.DrawText(font, Colors.Black, textLoc.X - textSize.Width / 2, textLoc.Y - textSize.Height / 2 - 8, valueToDisplay);

                    // Show input textBox for users to type in a new value
                    if (hoveredRec.isVertical)
                    {
                        mouseHoverValue_TB.Enabled = false;
                        mouseHoverValue_TB.Text = valueToDisplay;
                    }
                    else
                    {
                        mouseHoverValue_TB.Enabled = true;
                        hoveredValueIndex = hoveredRec.valueIndex;
                        if (!mouseHoverValue_TB.HasFocus)
                            mouseHoverValue_TB.Focus();

                        var hoveredValue = _vm.SchDayValues[hoveredRec.valueIndex];
                        if (mouseHoverValue_TB.SelectedText != hoveredValue.ToString())
                        {
                            mouseHoverValue_TB.Text = hoveredValue.ToString();
                            mouseHoverValue_TB.SelectAll();
                        }
                    }

                }
                else
                {
                    if (!startDragging)
                    {
                        mouseHoverValue_TB.Text = null;
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
                DrawTicks(canvas, graphics);

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
                        var oldValue = _vm.SchDayValues[valueIndex];

                        //preRec.Top = (float) Math.Max(newUserInput, oldValue);
                        //preRec.Bottom = (float)Math.Min(newUserInput, oldValue);
                        _vm.SchDayValues[valueIndex] = newUserInput;

                        _scheduleDaydrawable.Update(canvas);
                    }


                }

            };

            lowerLimit_TB.TextChanged += (s, e) =>
            {
                //this._selectedScheduleTypeLimits.LowerLimit = lowerLimit_TB.Value;
                _scheduleDaydrawable.Update(canvas);
            };

            higherLimit_TB.TextChanged += (s, e) =>
            {
                //_vm.ScheduleTypeLimit.UpperLimit = higherLimit_TB.Value;
                _scheduleDaydrawable.Update(canvas);
            };



            var layoutLeft = new DynamicLayout();
            layoutLeft.DefaultPadding = new Padding(5);

            var layoutLeftTop = new DynamicLayout();
            #region Layout Left Top

            layoutLeftTop.DefaultPadding = new Padding(25, 0, 10, 0);

            layoutLeftTop.DefaultSpacing = new Size(3, 3);
            layoutLeftTop.AddSeparateRow(new Label { Text = "Schedule Name:", Width = 90 }, schName_Tb);

            //layoutLeft.DataContext = _vmSelectedRule;
            layoutLeftTop.AddSeparateRow(new Label { Text = "Schedule Day:", Width = 90 }, dayName_Tb, null);


            _layoutSchRule = new DynamicLayout();
            _layoutSchRule.DefaultSpacing = new Size(3, 3);
            //_layoutSchRule.DefaultPadding = new Padding(25, 0, 0, 0);
            var dateRanges = ApplyDateRangeControls().ToList();
            dateRanges.Insert(0, new Label { Text = "Date Range:", Width = 90 });
            dateRanges.Add(null);
            _layoutSchRule.AddSeparateRow(dateRanges.ToArray());

            _applyToDayWeekBtns = ApplyDayOfWeekControls();
            var dayOfweek = _applyToDayWeekBtns.ToList();
            dayOfweek.Insert(0, new Label { Text = "Apply To:", Width = 90 });
            dayOfweek.Add(null);
            _layoutSchRule.AddSeparateRow(dayOfweek.ToArray());

            layoutLeftTop.AddSeparateRow(_layoutSchRule);
            layoutLeftTop.AddSeparateRow(new Label { Text = "Lower Limit:", Width = 90 }, lowerLimit_TB, "Upper Limit:", higherLimit_TB, null, mouseHoverValue_TB);
            UpdateScheduleRulePanel(true);
            #endregion

            //this.AddSeparateRow(null, label, mouseHoverValue_TB);
            var layoutDayDrawable = new DynamicLayout();
            #region Layout Left Bottom
            //layoutDayDrawable.DefaultPadding = new Padding(5);
            layoutDayDrawable.DefaultSpacing = new Size(5, 5);
            layoutDayDrawable.AddSeparateRow(_scheduleDaydrawable);
            layoutDayDrawable.AddSeparateRow(this.AddIntervalButtons());
            #endregion


            layoutLeft.AddSeparateRow(layoutLeftTop);
            layoutLeft.AddSeparateRow(layoutDayDrawable);

            layoutLeft.AddSeparateRow(null);
            #endregion

            #region RightPanel

            //Background color
            var calendarBackground = new Drawable(true);
            calendarBackground.Size = _calendarSize;
            calendarBackground.Paint += Drawable_FillBackground;

            //Middle layer
            _calendarPanel = new Drawable(true);
            _calendarPanel.Size = _calendarSize;

            //_calendarPanel.Paint += Drawable_FillBackground;
            _calendarPanel.Paint += Drawable_FillForeground;
            //_calendarPanel.Paint += Drawable_Grid;
            //_calendarPanel.Paint += Drawable_Label;

            //Foreground text/grids
            var calendarForeground = new Drawable(true);
            calendarForeground.Size = _calendarSize;
            calendarForeground.Paint += Drawable_Grid;
            calendarForeground.Paint += Drawable_Label;

            // combine layers
            var layoutLayers = new PixelLayout();
            //layoutLayers.Height = 600;
            layoutLayers.Add(calendarBackground, 0, 0);
            layoutLayers.Add(_calendarPanel, 0, 0);
            layoutLayers.Add(calendarForeground, 0, 0);


            var layoutRight = new DynamicLayout();
            layoutRight.Height = 600;
            layoutRight.BeginScrollable();
            layoutRight.AddRow(layoutLayers);
            layoutRight.EndScrollable();
            #endregion


            this.AddRow(rightPanel, layoutLeft, layoutRight, null);

        }

        private DynamicLayout CreateScheduleRulesPanel(List<HB.ScheduleRuleAbridged> rules)
        {
            var layout = new DynamicLayout();
            layout.Width = 200;
            //rulesPanel.Height = 600;


            var summerbtn = new Button() { Text = "Summer Design Day", Enabled = false };
            var winterbtn = new Button() { Text = "Winter Design Day", Enabled = false };
            var holidaybtn = new Button() { Text = "Holiday", Enabled = false };
            layout.AddRow("Special Day Profiles:");
            layout.AddRow(summerbtn);
            layout.AddRow(winterbtn);
            layout.AddRow(holidaybtn);
            layout.AddRow("Day Profiles:");


            var random = new Random();
            foreach (var item in rules)
            {
                if (string.IsNullOrEmpty(item.ScheduleDay))
                    continue;

                var topBorder = new Label() { Height = 1, BackgroundColor = Colors.Black };
                layout.AddSeparateRow(topBorder);

                var radColor = Color.FromArgb((int)(0xFF000000 + (random.Next(0xFFFFFF) & 0x7F7F7F)));
                colorsetPerRuleset.Add((radColor, item));
                var ruleDay = _vm.DaySchedules.First(_ => _.Identifier == item.ScheduleDay);
                var ruleBtn = GenScheduleRuleBtn(item.ScheduleDay, radColor,
                    () =>
                    {
                        _vm.SchDay_hbObj = ruleDay;
                        _vm.SchRule_hbObj = item;
                        UpdateScheduleRulePanel(false);
                        UpdateApplyDayWeekBtns();
                    }
                    );

                layout.AddSeparateRow(ruleBtn);

            }
            var defaultBorder = new Label() { Height = 1, BackgroundColor = Colors.Black };
            layout.AddSeparateRow(defaultBorder);
            var defaultScheduleBtn = GenScheduleRuleBtn("Default Day", _defaultColor,
                () =>
                {
                    _vm.SchDay_hbObj = _vm.DefaultDaySchedule;
                    UpdateScheduleRulePanel(true);
                }
            );
            layout.AddSeparateRow(defaultScheduleBtn);
            //var bottomBorder = new Label() { Height = 1, BackgroundColor = Colors.Black };
            //layout.AddSeparateRow(bottomBorder);
            //layout.AddRow(null);

    

            //layout.AddRow(null);

            return layout;
        }

        private Control[] _applyToDayWeekBtns;

        // btn background color
        private static Color _btnColor = new Button().BackgroundColor;
        private static Color BtnColor => _btnColor;
        private static Color _textColor = new Label().TextColor;
        private static Color TextColor => _textColor;

        private Button GenScheduleRuleBtn(string btnName, Color color, Action setAction)
        {
            //var left = new Label() { Width = 1, BackgroundColor= Colors.Black };
            //var middle = new Label() { Width = 1, BackgroundColor = Colors.Black };
            //var right = new Label() { Width = 1, BackgroundColor = Colors.Black };

            //var labelColor = new Label() { Width = 10 };
            //labelColor.BackgroundColor = color;


            //var ruleDayBtn = new Label() { Text = btnName, Height = 25, TextColor = TextColor };
            var ruleDayBtn = new Button() { Text = btnName};
            ruleDayBtn.BackgroundColor = color;
            ruleDayBtn.ToolTip = btnName;
            //ruleDayBtn.TextAlignment = TextAlignment.Left;
            //ruleDayBtn.Wrap = WrapMode.None;

            //var defaultBackgroundColor = BtnColor;
            //var pressColor = Color.FromArgb(200, 200, 200);
            //ruleDayBtn.BackgroundColor = defaultBackgroundColor;
            //ruleDayBtn.MouseMove += (s, e) =>
            //{
            //    ruleDayBtn.BackgroundColor = pressColor;
            //};
            //ruleDayBtn.MouseLeave += (s, e) =>
            //{
            //    ruleDayBtn.BackgroundColor = defaultBackgroundColor;
            //};

            ruleDayBtn.Click += (s, e) =>
            {
                setAction();
                _scheduleDaydrawable.Update(new Rectangle(new Size(600, 400)));
            };
             
            //ruleDayBtn.MouseDown += (s, e) =>
            //{
            //    ruleDayBtn.BackgroundColor = pressColor;
            //    //var rad = new Random();
            //    //var dayValues = new List<double>() { 0, rad.NextDouble(), rad.NextDouble() };
            //    //var dayTimes = new List<List<int>>() { new List<int> { 0, 0 }, new List<int> { 6, 0 }, new List<int> { 18, 0 } };
            //    //var newday = new ScheduleDay("ddd", dayValues, null, dayTimes) { };

            //    ////this.DayValues = new List<double>() { 0, 0.5, 0.1 };
            //    ////var dayValueFraction = dayValues.Select(_ => _ / scheduleTypelength);
            //    ////this.DayTimes = new List<(int hour, int minute)>() { (0, 0), (6, 0), (18, 0) };
            //    //this._selectedScheduleDay = newday;
            //    setAction();
            //    _scheduleDaydrawable.Update(new Rectangle(new Size(600,400)));


            //};

            return ruleDayBtn;


        }


        private Control[] ApplyDateRangeControls()
        {
            var sDate = new DateTimePicker();
            sDate.Mode = DateTimePickerMode.Date;
            sDate.MinDate = new DateTime(2017, 1, 1);
            sDate.MaxDate = new DateTime(2017, 12, 31);
            sDate.Value = sDate.MinDate;
            sDate.ValueBinding.Bind(_vm, m => m.StartDate);
            sDate.ValueChanged += (s, e) => {  RefreshCalendar(); };
          

            var eDate = new DateTimePicker();
            eDate.Mode = DateTimePickerMode.Date;
            eDate.MinDate = new DateTime(2017, 1, 1);
            eDate.MaxDate = new DateTime(2017, 12, 31);
            eDate.Value = eDate.MaxDate;
            eDate.ValueBinding.Bind(_vm, m => m.EndDate);
            eDate.ValueChanged += (s, e) => { RefreshCalendar(); };
            return new Control[] {sDate, eDate };
        }

        Color btnDefaultColor;
        Color btnEnabledColor;
        private Control[] ApplyDayOfWeekControls()
        {
            //var nonDefaultSchedulrRulePanel = new DynamicLayout();
            var width = 30;
            var btnSU = new Button() { Text = "S", Width = width };
            var btnMO = new Button() { Text = "M", Width = width };
            var btnTU = new Button() { Text = "T", Width = width };
            var btnWE = new Button() { Text = "W", Width = width };
            var btnTH = new Button() { Text = "T", Width = width };
            var btnFI = new Button() { Text = "F", Width = width };
            var btnSA = new Button() { Text = "S", Width = width };
            btnEnabledColor = Color.FromArgb(191,229,249);
            btnDefaultColor = btnSA.BackgroundColor;
           
            btnSU.Click += (s, e) => {
                _vm.ApplySunday = !_vm.ApplySunday;
                btnSU.BackgroundColor = _vm.ApplySunday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnMO.Click += (s, e) => {
                _vm.ApplyMonday = !_vm.ApplyMonday;
                btnMO.BackgroundColor = _vm.ApplyMonday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnTU.Click += (s, e) => {
                _vm.ApplyTuesday = !_vm.ApplyTuesday;
                btnTU.BackgroundColor = _vm.ApplyTuesday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnWE.Click += (s, e) => {
                _vm.ApplyWednesday = !_vm.ApplyWednesday;
                btnWE.BackgroundColor = _vm.ApplyWednesday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnTH.Click += (s, e) => {
                _vm.ApplyThursday = !_vm.ApplyThursday;
                btnTH.BackgroundColor = _vm.ApplyThursday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnFI.Click += (s, e) => {
                _vm.ApplyFriday = !_vm.ApplyFriday;
                btnFI.BackgroundColor = _vm.ApplyFriday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            btnSA.Click += (s, e) => {
                _vm.ApplySaturday = !_vm.ApplySaturday;
                btnSA.BackgroundColor = _vm.ApplySaturday ? btnEnabledColor : btnDefaultColor;
                RefreshCalendar();
            };
            //nonDefaultSchedulrRulePanel.AddRow(null, "Apply to", btnSU, btnMO, btnTU, btnWE, btnTH, btnFI, btnSA, null);
            return new Control[] {btnSU, btnMO, btnTU, btnWE, btnTH, btnFI, btnSA };
        }
        private void UpdateApplyDayWeekBtns()
        {
            if (_applyToDayWeekBtns == null)
                return;


            var bools = new[] {
                _vm.ApplySunday, _vm.ApplyMonday, _vm.ApplyTuesday, _vm.ApplyWednesday,
                _vm.ApplyThursday,_vm.ApplyFriday, _vm.ApplySaturday};

            for (int i = 0; i < 7; i++)
            {
                var btn = _applyToDayWeekBtns[i] as Button;
                btn.BackgroundColor = bools[i] ? btnEnabledColor : btnDefaultColor;

            }

        }

        private void UpdateScheduleRulePanel(bool isDefault)
        {
            if (isDefault)
            {
                _layoutSchRule.Enabled = false;
            }
            else
            {
                _layoutSchRule.Enabled = true;
            }
        }
        //private void DrawCalendarGrids(Graphics graphic)
        //{
        //    //var graphic = e.Graphics;
        //    var dayW = 25;
        //    var dayH = 15;
        //    var monthW = dayW * 7;
        //    var monthH = dayH * 5;
        //    // vertical lines
            

        //    void DrawMonthGrid()
        //    {
        //        for (int i = 0; i <= 7; i++)
        //        {
        //            var x = i * dayW;
        //            graphic.DrawLine(Colors.Black, x, 0, x, monthH);
        //        }
        //        // horizontal lines
        //        for (int i = 0; i <= 5; i++)
        //        {
        //            var y = i * dayH;
        //            graphic.DrawLine(Colors.Black, 0, y, monthW, y);

        //        }
        //    }
        //}



        //private void RedrawMonth(int startDay, Graphics graphic)
        //{
        //    var daysPerMonth = new[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        //    var weekDays = new[] { "S", "M", "T", "W", "T", "F", "S" };
        //    var dayW = 25;
        //    var dayH = 15;
        //    var defaultColor = Colors.Aqua;

        //    var monthRec = new Rectangle(0, 0, dayW * 7, dayH * 5);
        //    //// draw grid
        //    //// fill day cell color
        //    //for (int j = 0; j < 35; j++)
        //    //{

        //    //}
        //    var row = 0;
        //    var indexInRow = 0;

        //    var font = Fonts.Sans(8);
        //    var DOY = 0;

        //    var pt = new Point();
        //    var coloum = 1;
        //    var month = 0;

        //    var setColor = defaultColor;

        //    foreach (var days in daysPerMonth)
        //    {
        //        if (month > 0)
        //        {
        //            pt.Y = pt.Y + dayH * 2;
        //        }

        //        var monthPt = new Point(0, pt.Y);
        //        graphic.DrawText(font, Colors.Black, monthPt.X, monthPt.Y, (month + 1).ToString());
        //        pt.Y = pt.Y + dayH;
        //        monthPt.Y = monthPt.Y + dayH;
        //        for (int i = 0; i < 7; i++)
        //        {
        //            var rec = new Rectangle(i * dayW, monthPt.Y, dayW, dayH);

        //            var date = weekDays[i];
        //            var size = font.MeasureString(date);


        //            graphic.DrawText(font, Colors.Black, rec.Center.X - size.Width / 2, rec.Center.Y - size.Height / 2, date);
        //            monthPt.X = monthPt.X + dayW;
        //        }


        //        pt.Y = pt.Y + dayH;
        //        for (int i = 0; i < days; i++)
        //        {
        //            var weekOfMonth = (int)Math.Floor((double)i / 7);
        //            var dayOfWeek = (i - row * 7);
                    
                    
        //            if (pt.X + dayW >= monthRec.Width * coloum)
        //            {
        //                pt.Y = pt.Y + dayH;
        //                pt.X = monthRec.Width * (coloum - 1);
        //            }
        //            else if (DOY != 0)
        //            {
        //                pt.X = pt.X + dayW;
        //            }

        //            dayOfWeek = (int)(pt.X / dayW);
        //            if (_vmSelectedRule.ApplySaturday && dayOfWeek == 6)
        //            {
        //                setColor = Colors.Red;
        //            }
        //            else
        //            {
        //                setColor = defaultColor;
        //            }

        //            var rec = new Rectangle(pt.X, pt.Y, dayW, dayH);
        //            graphic.FillRectangle(setColor, rec);
        //            graphic.DrawRectangle(Colors.Black, rec);
        //            var date = (i + 1).ToString();
        //            var size = font.MeasureString(date);

        //            graphic.DrawText(font, Colors.Black, rec.Center.X - size.Width / 2, rec.Center.Y - size.Height / 2, date);
        //            //row = i == daysPerMonth[month]-1 ? row + 1 : row;
        //            DOY++;
        //        }

        //        month++;
        //    }
        //}


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

        private static void DrawTicks(Rectangle canvas, Graphics graphics)
        {
            double start = _vm.LowerLimit;
            double end =  _vm.UpperLimit;

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
                graphics.DrawLine(TextColor, p1, p2);

                var tickText = $"{i * hourInterval}:00";
                var textSize = tickfont.MeasureString(tickText);
                graphics.DrawText(tickfont, TextColor, p2.X - textSize.Width / 2, p2.Y - textSize.Height / 2 + 8, tickText);
            }

            //Draw vertical value ticks
            var valueMarkCount = 5;
            var valueStartPt = canvas.BottomLeft;
            var valueEndPt = canvas.TopLeft;
            var heightPerInterval = (valueStartPt.Y - valueEndPt.Y) / valueMarkCount;
            var valueInterval = Math.Abs(start - end) / valueMarkCount;
            for (int i = 0; i <= valueMarkCount; i++)
            {
                var p1 = new PointF(left, bottom - i * heightPerInterval);
                var p2 = new PointF(left - tickLength, bottom - i * heightPerInterval);
                graphics.DrawLine(TextColor, p1, p2);

                var tickText = $"{start + i * valueInterval}";
                var textSize = tickfont.MeasureString(tickText);
                graphics.DrawText(tickfont, TextColor, p2.X - textSize.Width - 2, p2.Y - textSize.Height / 2, tickText);
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
            var dayValues = _vm.SchDayValues;
            var dayValuesFraction = this.DayValuesFraction;
            var dayTimes = _vm.SchDayTimes;

            var widthPerHour = canv.Width / 24;
            var heightPerHour = canv.Height;
            var count = dayValues.Count();
            var hourPts = new List<PointF>();
            for (int i = 0; i < count; i++)
            {
                var checkedMinute = NormalizeMinute(dayTimes[i][1]);
                var time = (double)dayTimes[i][0] + checkedMinute / 60;
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
            _calendarPanel.Update(new Rectangle(_calendarSize));
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

            var colorSet = colorsetPerRuleset;
            var total = colorSet.Count;
            for (int i = 0; i < total; i++)
            {
                var item = colorSet[total - 1 - i];
                var rule = item.schRule;
                //var color = Colors.Blue;
                var appliedDays = new List<DayOfWeek> { };

                if (rule.ApplySunday)
                    appliedDays.Add(DayOfWeek.Sunday);
                if (rule.ApplyMonday)
                    appliedDays.Add(DayOfWeek.Monday);
                if (rule.ApplyTuesday)
                    appliedDays.Add(DayOfWeek.Tuesday);
                if (rule.ApplyWednesday)
                    appliedDays.Add(DayOfWeek.Wednesday);
                if (rule.ApplyThursday)
                    appliedDays.Add(DayOfWeek.Thursday);
                if (rule.ApplyFriday)
                    appliedDays.Add(DayOfWeek.Friday);
                if (rule.ApplySaturday)
                    appliedDays.Add(DayOfWeek.Saturday);

                var startDate = new DateTime(2017, rule.StartDate[0], rule.StartDate[1]);
                var startDOY = startDate.DayOfYear;

                var endDate = new DateTime(2017, rule.EndDate[0], rule.EndDate[1]);
                var endDOY = endDate.DayOfYear;


                var selectedDays = DayRectangles.Skip(startDOY - 1).Take(endDOY - startDOY + 1).Where(_ => appliedDays.Contains(_.date.DayOfWeek));
                var recs = selectedDays.Select(_ => _.rec);
                graphic.FillRectangles(item.color, recs);
            }
       

        }

        private void Drawable_Label(object sender, PaintEventArgs e)
        {

            var graphic = e.Graphics;

            var font = Fonts.Sans(8);
            var brush = Brushes.Cached(TextColor);
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

        public ICommand HBDataCommand => new RelayCommand(() => {
            Dialog_Message.Show(this, _vm.SchRuleset_hbObj.ToJson());
        });

        public ICommand AddRuleCommand => new RelayCommand<DynamicLayout>((layout) => {

            var rules = _vm.ScheduleRules ?? new List<HB.ScheduleRuleAbridged>();
            var defaultDay = _vm.DefaultDaySchedule;
            var newRule = new HB.ScheduleRuleAbridged(defaultDay.Identifier);
            rules.Add(newRule);
            _vm.ScheduleRules = rules;

            var ruleBtn = GenScheduleRuleBtn(defaultDay.DisplayName, _defaultColor,
                () =>
                {
                    _vm.SchDay_hbObj = defaultDay;
                    _vm.SchRule_hbObj = newRule;
                    UpdateScheduleRulePanel(false);
                    UpdateApplyDayWeekBtns();
                }
                );

            layout.AddRow(ruleBtn);
            layout.Create();

        });

    }
}
