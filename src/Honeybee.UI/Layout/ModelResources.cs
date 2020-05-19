using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HoneybeeSchema;
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

        private static Model _model;
        //private static SimulationParameter _simulationParameter;

        public Panel_Model(Model model)
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
                var dialog = new Dialog_MaterialManager(_model);
                var dialog_rc = dialog.ShowModal(this);

            };
            constrcutionBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var dialog = new Dialog_ConstructionManager(_model);
                var dialog_rc = dialog.ShowModal(this);
               
            };
            constrSetbtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var dialog = new Dialog_ConstructionSetManager(_model);
                var dialog_rc = dialog.ShowModal(this);
                //MessageBox.Show(this, "Working in progress");
            };
            scheduleBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var dialog = new Dialog_ScheduleRulesetManager(_model);
                var dialog_rc = dialog.ShowModal(this);
                //MessageBox.Show(this, "Working in progress");
            };
            programTypeBtn.Click += (s, e) =>
            {
                if (_model == null)
                {
                    MessageBox.Show(this, "Invalid model");
                    return;
                }
                var dialog = new Dialog_ProgramTypeManager(_model);
                var dialog_rc = dialog.ShowModal(this);
                //MessageBox.Show(this, "Working in progress");
            };

            //simParamBtn.Click += (s, e) =>
            //{
            //    var dialog = new Dialog_SimulationParameter(_simulationParameter);
            //    var dialog_rc = dialog.ShowModal(this);
            //    //MessageBox.Show(this, "Working in progress");
            //};
           
        }

        //public static void UpdatePanel()
        //{
           
        //}
      

        private static Panel GenDoorPanel()
        {
            var vm = DoorViewModel.Instance;

            var layout = new DynamicLayout { DataContext = vm};
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow(new Label { Text = "ID: " }, id);


            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Room Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            //layout.AddSeparateRow(new Label { Text = "Glass:" });
            var isGlassCBox = new CheckBox();
            isGlassCBox.CheckedBinding.BindDataContext((DoorViewModel m) => m.HoneybeeObject.IsGlass);
            isGlassCBox.CheckedChanged += (s, e) => { vm.ActionWhenChanged($"Set Glass Door: {vm.HoneybeeObject.IsGlass}"); };
            layout.AddSeparateRow(new Label { Text = "Glass:" }, isGlassCBox);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show(Helper.Owner, "Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = vm.HoneybeeObject.Properties.Energy ?? new DoorEnergyPropertiesAbridged();
                energyProp = DoorEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_DoorEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal(Helper.Owner);
                if (dialog_rc != null)
                {
                    vm.HoneybeeObject.Properties.Energy = dialog_rc;
                    vm.ActionWhenChanged($"Set Door Energy Properties");
                }

            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcDP = new DropDown();
            bcDP.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.Bcs);
            bcDP.ItemTextBinding = Binding.Delegate<AnyOf, string>(m => m.Obj.GetType().Name);
            bcDP.SelectedIndexBinding.BindDataContext((DoorViewModel m) => m.SelectedIndex);
            layout.AddSeparateRow(bcDP);

            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.BindDataContext(c => c.Enabled, (DoorViewModel m) => m.IsOutdoor, DualBindingMode.OneWay);
            bcBtn.Click += (s, e) =>
            {
                if (vm.HoneybeeObject.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal(Helper.Owner);
                    if (dialog_rc != null)
                    {
                        vm.HoneybeeObject.BoundaryCondition = dialog_rc;
                        vm.ActionWhenChanged($"Set Aperture Boundary Condition");
                    }
                }
                else
                {
                    MessageBox.Show(Helper.Owner, "Only Outdoors type has additional properties to edit!");
                }
            };
            layout.AddSeparateRow(bcBtn);


            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            inShadesListBox.Height = 50;
            layout.AddSeparateRow(inShadesListBox);


            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (DoorViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, vm.HoneybeeObject.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }



    }
}
