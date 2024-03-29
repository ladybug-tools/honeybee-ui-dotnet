﻿using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
namespace Honeybee.UI
{
    public class Dialog_EPOutputs : Dialog<HB.SimulationOutput>
    {
     
        public Dialog_EPOutputs(HB.SimulationOutput simulationOutput)
        {
            try
            {
                var output = simulationOutput;

                Padding = new Padding(5);
                Title = $"EnergyPlus Output Names - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 500);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout();
                layout.Spacing = new Size(5, 5);
                layout.Padding = new Padding(5);

                var rept_DP = new EnumDropDown<HB.ReportingFrequency>();
                rept_DP.SelectedValueBinding.Bind(() => output.ReportingFrequency, v => output.ReportingFrequency = v);
                rept_DP.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationOutput), nameof(output.ReportingFrequency)));
                layout.AddRow("Reporting Frequency:");
                layout.AddRow(rept_DP);

                //Outputs
                var outputListBox = new RichTextArea();
                outputListBox.TextBinding.Bind(() =>
                {
                    output.Outputs = output.Outputs ?? new List<string>() { "Zone Electric Equipment Electric Energy", "Zone Lights Electric Energy" };
                    var s = string.Join(Environment.NewLine, output.Outputs);
                    return s;
                }, 
                v =>
                {
                    output.Outputs = v.Split(new[] { Environment.NewLine, "\n" },StringSplitOptions.RemoveEmptyEntries).Select(_=>_.Trim()).ToList();
                }
                );
                outputListBox.Height = 200;
                outputListBox.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationOutput), nameof(output.Outputs)));

                layout.AddRow("EnergyPlus Output Names:");
                layout.AddRow(outputListBox);

                //SummaryReports
                var sumReportsListBox = new RichTextArea();
                sumReportsListBox.Height = 100;
                sumReportsListBox.TextBinding.Bind(
                () =>
                {
                    output.SummaryReports = output.SummaryReports ?? new List<string>() { "AllSummary" };
                    var s = string.Join(Environment.NewLine, output.SummaryReports);
                    return s;
                },
                v =>
                {
                    output.SummaryReports = v.Split(new[] { Environment.NewLine, "\n"}, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToList();
                }
                );
                sumReportsListBox.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.SimulationOutput), nameof(output.SummaryReports)));
                layout.AddRow("EnergyPlus Summary Report:");
                layout.AddSeparateRow(sumReportsListBox);


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => {
                    Close(output);
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };

                layout.AddSeparateRow(buttons);
                layout.AddRow(null);
                //Create layout
                Content = layout;
              
            }
            catch (Exception e)
            {
                throw e;
            }
            
            
        }
        
     

    }
}
