using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_Schedule : Dialog_ResourceEditor<ScheduleRuleset>
    {
        
        public Dialog_Schedule(ScheduleRuleset scheduleRuleset)
        {
            Padding = new Padding(5);
            Title = $"Schedule - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 1100;
            this.Icon = DialogHelper.HoneybeeIcon;

            var OkButton = new Button { Text = "OK" };
            OkButton.Click += (sender, e) => OkCommand.Execute(scheduleRuleset);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            var panel = new Panel_Schedule(scheduleRuleset);
            panel.AddSeparateRow(null, OkButton, AbortButton, null);
            Content = panel;
        }

    }
}
