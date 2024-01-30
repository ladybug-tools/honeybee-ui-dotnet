// Ignore Spelling: Epw

using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class Dialog_EfficiencyStandards : Dialog<HB.EfficiencyStandards>
    {
        public Dialog_EfficiencyStandards()
        {

            var _hbobj = (HB.EfficiencyStandards)0;


            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(3, 3);
            layout.DefaultPadding = new Padding(5);

            Resizable = false;
            Title = $"Efficiency Standards - {DialogHelper.PluginName}";
            //WindowStyle = WindowStyle.Default;
            //MinimumSize = new Size(450, 620);
            Width = 400;
            this.Icon = DialogHelper.HoneybeeIcon;

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e)
                => Close(_hbobj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            // Building type
            var effStdItems = Enum.GetValues(typeof(HB.EfficiencyStandards)).Cast<HB.EfficiencyStandards>().Select(_ => _.ToString()).ToList();
            effStdItems.Insert(0, "<None>");
            var effStdDP = new DropDown();
            effStdDP.DataStore = effStdItems;
            effStdDP.SelectedValueBinding.Bind(
                Binding.Delegate(() =>
                {
                    var o = _hbobj.ToString();
                    o = o == "0" ? "<None>" : o;
                    return (object)o;
                },
                v =>
                {
                    Enum.TryParse<HB.EfficiencyStandards>(v?.ToString(), out var cz);
                    _hbobj = cz;
                }));

            layout.AddRow("Efficiency Standards:");
            layout.AddRow(effStdDP);
            layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
            layout.AddRow(null);

            Content = layout;


        }
    }
}
