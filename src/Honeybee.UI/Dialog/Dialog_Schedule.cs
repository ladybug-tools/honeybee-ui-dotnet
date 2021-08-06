using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_Schedule : Dialog_ResourceEditor<ScheduleRuleset>
    {
        
        public Dialog_Schedule(ScheduleRuleset scheduleRuleset, bool lockedMode = false)
        {
            Padding = new Padding(10);
            Title = $"Schedule - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 1110;
            this.Icon = DialogHelper.HoneybeeIcon;

            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OkButton = new Button { Text = "OK", Enabled = !lockedMode };
            OkButton.Click += (sender, e) => OkCommand.Execute(scheduleRuleset);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            var sche = new Panel_Schedule(scheduleRuleset);
            var panel = new DynamicLayout() { DefaultSpacing = new Size(5, 5) };
            panel.AddRow(sche);
            panel.AddSeparateRow(locked, OkButton, AbortButton, null);
            Content = panel;
        }

    }
}
