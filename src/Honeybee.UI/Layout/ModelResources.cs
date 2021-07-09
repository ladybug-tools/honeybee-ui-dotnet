using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HB = HoneybeeSchema;
using System.Collections.Generic;

namespace Honeybee.UI
{
    public class Panel_ModelResources: DynamicLayout
    {

        private static HB.Model _model;
        //private static SimulationParameter _simulationParameter;

        public Panel_ModelResources(HB.Model model)
        {
            _model = model;

            var resourceGroup = new GroupBox();
            resourceGroup.Text = "In-Model Resources";
            var groupPanel = new DynamicLayout();
            //groupPanel.DefaultPadding = new Padding(5);
            groupPanel.DefaultSpacing = new Size(5, 5);

            var materialBtn = new Button();
            materialBtn.Text = "Materials";
            groupPanel.AddRow(materialBtn);

            var constrcutionBtn = new Button();
            constrcutionBtn.Text = "Constructions";
            groupPanel.AddRow(constrcutionBtn);

            var constrSetbtn = new Button();
            constrSetbtn.Text = "Construction Sets";
            groupPanel.AddRow(constrSetbtn);

            var scheduleBtn = new Button();
            scheduleBtn.Text = "Schedules";
            groupPanel.AddRow(scheduleBtn);

            var programTypeBtn = new Button();
            programTypeBtn.Text = "Program Types";
            groupPanel.AddRow(programTypeBtn);
            resourceGroup.Content = groupPanel;

            //this.Content = resourceGroup;
            this.AddRow(resourceGroup);


            materialBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var lib = _model.Properties.Energy;
                var dialog = new Dialog_MaterialManager(ref lib);
                var dialog_rc = dialog.ShowModal(this);
                if (dialog_rc != null)
                {
                    _model.Properties.Energy.Materials.Clear();
                    _model.AddMaterials(dialog_rc);

                }

            };
            constrcutionBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
        
                var lib = _model.Properties.Energy;
                var dialog = new Dialog_ConstructionManager(ref lib);
                var dialog_rc = dialog.ShowModal(this);
                if (dialog_rc != null)
                {
                    _model.Properties.Energy.Constructions.Clear();
                    _model.AddConstructions(dialog_rc);

                }
               
            };
            constrSetbtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var lib = _model.Properties.Energy;
                var dialog = new Dialog_ConstructionSetManager(ref lib);
                var dialog_rc = dialog.ShowModal(this);
                if (dialog_rc != null)
                {
                    _model.Properties.Energy.ConstructionSets.Clear();
                    _model.AddConstructionSets(dialog_rc);

                }
                //MessageBox.Show(this, "Working in progress");
            };
            scheduleBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var lib = _model.Properties.Energy;
                var dialog = new Dialog_ScheduleRulesetManager(ref lib);
                var dialog_rc = dialog.ShowModal(this);
                if (dialog_rc != null)
                {
                    // sch list
                }
            };
            programTypeBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }

                var lib = _model.Properties.Energy;
                var dialog = new Dialog_ProgramTypeManager(ref lib);
                var dialog_rc = dialog.ShowModal(this);
                if (dialog_rc != null)
                {
                    _model.Properties.Energy.ProgramTypes.Clear();
                    _model.AddProgramTypes(dialog_rc);

                }
                //MessageBox.Show(this, "Working in progress");
            };

           
        }

       


    }
}
