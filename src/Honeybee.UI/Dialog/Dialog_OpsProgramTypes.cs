using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Honeybee.UI
{
    public class Dialog_OpsProgramTypes : Dialog<(HB.ProgramTypeAbridged programType, IEnumerable<HB.ScheduleRulesetAbridged> schedules)>
    {
     
        public Dialog_OpsProgramTypes()
        {
            try
            {
                //var output = simulationOutput;
                var vm = OpsProgramTypesViewModel.Instance;

                Padding = new Padding(5);
                Resizable = true;
                Title = "OpenStudio Standards - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 300);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout() { DataContext = vm};
                layout.Spacing = new Size(8, 8);
                layout.Padding = new Padding(15);

                
                var year = new DropDown();
                var buildingType = new DropDown();
                var programType = new DropDown();
                //var textArea = new RichTextArea() { Height = 200 };
                var cloneBtn = new Button() { Text = "Clone and Edit" };

                year.DataStore = vm.VintageNames;
                year.SelectedKeyBinding.BindDataContext((OpsProgramTypesViewModel m) => m.Vintage);

                buildingType.BindDataContext(c => c.DataStore, (OpsProgramTypesViewModel m) => m.BuildingTypes);
                buildingType.SelectedKeyBinding.BindDataContext((OpsProgramTypesViewModel m) => m.BuildingType);

                programType.BindDataContext(c => c.DataStore, (OpsProgramTypesViewModel m) => m.ProgramTypes);
                programType.SelectedKeyBinding.BindDataContext((OpsProgramTypesViewModel m) => m.ProgramType);


                //textArea.TextBinding.BindDataContext((OpsProgramTypesViewModel m) => m.ProgramTypeJson);
                //textArea.TextBinding.BindDataContext((MessageViewModel m) => m.MessageText);
                //layout.AddSeparateRow(textArea);



                cloneBtn.Click += (s, e) => {
                    var program = vm.ProgramTypeWithSches.programType;
                    var sch = vm.ProgramTypeWithSches.schedules;
                    program.Identifier = Guid.NewGuid().ToString();
                    var dialog = new Honeybee.UI.Dialog_ProgramType(program);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        Close((program, sch));
                    }

                };

                layout.AddRow("Vintage:");
                layout.AddRow(year);
                layout.AddRow("Building Type:");
                layout.AddRow(buildingType);
                layout.AddRow("Program Type:");
                layout.AddRow(programType);
                //layout.AddRow(textArea);
                layout.AddSeparateRow(cloneBtn);

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => {
                    var programT = vm.ProgramTypeWithSches.programType;
                    var sch = vm.ProgramTypeWithSches.schedules;
                    programT.DisplayName = programT.Identifier;
                    programT.Identifier = Guid.NewGuid().ToString();
                    Close((programT, sch));
                }; 

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
                layout.AddRow(null);
                Content = layout;
              
            }
            catch (Exception e)
            {
                throw e;
            }
            
            
        }
        
     

    }
}
