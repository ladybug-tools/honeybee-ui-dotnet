using Eto.Drawing;
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
                Resizable = true;
                Title = "EnergyPlus Output Names - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 500);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout();
                layout.Spacing = new Size(8, 8);
                layout.Padding = new Padding(15);

                //Outputs
                var outputListBox = new ListBox();
                outputListBox.Height = 200;
                var outputs = output.Outputs;
                if (outputs != null)
                {
                    var outputItems = outputs.Select(_ => new ListItem() { Text = _ });
                    outputListBox.Items.AddRange(outputItems);
                }
                layout.AddRow("EnergyPlus Output Names:");
                layout.AddRow(outputListBox);

                //SummaryReports
                var sumReportsListBox = new ListBox();
                sumReportsListBox.Height = 100;
                var sumReports = output.SummaryReports;
                if (sumReports != null)
                {
                    var outputItems = sumReports.Select(_ => new ListItem() { Text = _ });
                    sumReportsListBox.Items.AddRange(outputItems);
                }
                layout.AddRow("EnergyPlus Summary Report:");
                layout.AddRow(sumReportsListBox);


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(output);

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
