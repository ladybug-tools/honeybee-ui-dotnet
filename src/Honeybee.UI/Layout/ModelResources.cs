using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace Honeybee.UI
{
    public class Panel_Model: DynamicLayout
    {

        //private static Panel_Model _instance;

        //public static Panel_Model Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new Panel_Model();
        //        }
        //        return _instance;
        //    }
        //    set { _instance = value; }
        //}
        //public Button ModelPropertyBtn { get; set; }
        //public Button RoomsBtn { get; set; }
        //public Button RunSimulationBtn { get; set; }

        private static HB.Model _model;
        //private static SimulationParameter _simulationParameter;

        public Panel_Model(HB.Model model)
        {
            _model = model;
            //_simulationParameter = simulationParameter ?? new SimulationParameter();

            //this.DefaultPadding = new Padding(10);
            //this.DefaultSpacing = new Size(5, 5);


            //ModelPropertyBtn = new Button();
            //ModelPropertyBtn.Text = "Model Property";
            //this.AddSeparateRow(ModelPropertyBtn);

            //RoomsBtn = new Button();
            //RoomsBtn.Text = "Rooms";
            //this.AddSeparateRow(RoomsBtn);


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


            //var simParamBtn = new Button();
            //simParamBtn.Text = "Simulation Parameter";
            //this.AddSeparateRow(simParamBtn);

            //RunSimulationBtn = new Button();
            //RunSimulationBtn.Text = "Run Simulation";
            //this.AddSeparateRow(RunSimulationBtn);
            //this.AddSeparateRow(null);


            //RoomsBtn.Click += (s, e) =>
            //{
            //    MessageBox.Show(this, "Working in progress");
            //};
            //ModelPropertyBtn.Click += (s, e) =>
            //{
            //    MessageBox.Show(this, "Working in progress");
            //};
            materialBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var materialsInModel = _model.Properties.Energy.Materials.Select(_ => _.Obj as HB.Energy.IMaterial).ToList();

                var dialog = new Dialog_MaterialManager(materialsInModel);
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
                var constrcutionsInModel = _model.Properties.Energy.Constructions
                .Where(_ => _.Obj.GetType().Name.Contains("Abridged"))
                .Select(_ => _.Obj as HB.Energy.IConstruction)
                .ToList();

                var dialog = new Dialog_ConstructionManager(constrcutionsInModel);
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
                var constrcutionSetsInModel = _model.Properties.Energy.ConstructionSets
                .Where(_ => _.Obj is HB.ConstructionSetAbridged)
                .Select(_ => _.Obj as HB.Energy.IBuildingConstructionset)
                .ToList();
                var globalCSet = _model.Properties.Energy.GlobalConstructionSet;
                var dialog = new Dialog_ConstructionSetManager(constrcutionSetsInModel, (id)=> id == globalCSet);
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
                var allSches = _model.Properties.Energy.Schedules
                .Where(_ => _.Obj is HB.ScheduleRulesetAbridged)
                .Select(_ => _.Obj as HB.ScheduleRulesetAbridged)
                .ToList();

                var schTypes = _model.Properties.Energy.ScheduleTypeLimits.Select(_=>_).ToList();
              
                var dialog = new Dialog_ScheduleRulesetManager(allSches, schTypes);
                var dialog_rc = dialog.ShowModal(this);
                if (dialog_rc.scheduleRulesets != null)
                {
                    var schs = dialog_rc.scheduleRulesets.OfType<HB.IDdEnergyBaseModel>().ToList();
                    _model.Properties.Energy.Schedules.Clear();
                    _model.AddSchedules(schs);
                    _model.Properties.Energy.ScheduleTypeLimits.Clear();
                    _model.AddScheduleTypeLimits(dialog_rc.scheduleTypeLimits);
                }
                //MessageBox.Show(this, "Working in progress");
            };
            programTypeBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }

                var pTypeInModel = _model.Properties.Energy.ProgramTypes
                .Where(_ => _.Obj is HB.ProgramTypeAbridged)
                .Select(_ => _.Obj as HB.ProgramTypeAbridged)
                .ToList();


                var dialog = new Dialog_ProgramTypeManager(pTypeInModel);
                var dialog_rc = dialog.ShowModal(this);
                if (dialog_rc != null)
                {
                    _model.Properties.Energy.ProgramTypes.Clear();
                    _model.AddProgramTypes(dialog_rc);
                }
                //MessageBox.Show(this, "Working in progress");
            };

            //simParamBtn.Click += (s, e) =>
            //{
            //    var dialog = new Dialog_SimulationParameter(_simulationParameter);
            //    var dialog_rc = dialog.ShowModal(this);
            //    //MessageBox.Show(this, "Working in progress");
            //};
           
        }

       


    }
}
