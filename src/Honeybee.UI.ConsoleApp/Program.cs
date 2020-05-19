using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using HB = HoneybeeSchema;

namespace Honeybee.UI.ConsoleApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Eto.Forms.Application().Run(new MyForm());
            Console.ReadLine();
        }

        public class MyForm : Form
        {
            public MyForm()
            {
                ClientSize = new Eto.Drawing.Size(400, 300);
                Title = "Eto.Forms";

                var panel = new DynamicLayout();
                var btn = new Button() { Text="Energy Property"};
                btn.Click += (s, e) =>
                {
                    var energyProp = new HoneybeeSchema.RoomEnergyPropertiesAbridged();
                    var dialog = new Honeybee.UI.Dialog_RoomEnergyProperty(energyProp);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        Console.WriteLine(dialog_rc.ToJson());
                    }
                };

                var Messagebtn = new Button() { Text = "message text" };
                Messagebtn.Click += (s, e) =>
                {
                    var energyProp = new HoneybeeSchema.RoomEnergyPropertiesAbridged();
                    Dialog_Message.Show(this, energyProp.ToJson());
                    
                };

                var cSetbtn = new Button() { Text = "ConstructionSet" };
                cSetbtn.Click += (s, e) =>
                {
                    var cSet = new HoneybeeSchema.ConstructionSetAbridged(identifier: Guid.NewGuid().ToString());
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(cSet);
                    dialog.ShowModal(this);

                };

                var pTypebtn = new Button() { Text = "ProgramType" };
                pTypebtn.Click += (s, e) =>
                {
                    var pType = new HoneybeeSchema.ProgramTypeAbridged(identifier: Guid.NewGuid().ToString());
                    var dialog = new Honeybee.UI.Dialog_ProgramType(pType);
                    dialog.ShowModal(this);

                };

                var pTypeMngbtn = new Button() { Text = "ProgramTypeManager" };
                pTypeMngbtn.Click += (s, e) =>
                {
                    var md = new HB.Model("", new HB.ModelProperties(HB.ModelEnergyProperties.Default));
                    var pTypeInModel = md.Properties.Energy.ProgramTypes
                        .Where(_ => _.Obj is HB.ProgramTypeAbridged)
                        .Select(_ => _.Obj as HB.ProgramTypeAbridged)
                        .ToList();

                    var dialog = new Honeybee.UI.Dialog_ProgramTypeManager(pTypeInModel);
                    var dialog_rc =dialog.ShowModal(this);
                  
                };

                var schbtn = new Button() { Text = "ScheduleRulesetManager" };
                schbtn.Click += (s, e) =>
                {
                    var md = new HB.Model("", new HB.ModelProperties(HB.ModelEnergyProperties.Default));

                    var allSches = md.Properties.Energy.Schedules
                       .Where(_ => _.Obj is HB.ScheduleRulesetAbridged)
                       .Select(_ => _.Obj as HB.ScheduleRulesetAbridged)
                       .ToList();
                    var schTypes = md.Properties.Energy.ScheduleTypeLimits.Select(_ => _).ToList();

                    var dialog = new Honeybee.UI.Dialog_ScheduleRulesetManager(allSches, schTypes);
                    dialog.ShowModal(this);

                };

                var conbtn = new Button() { Text = "ConstructionManager" };
                conbtn.Click += (s, e) =>
                {
                 

                    var md = new HB.Model("", new HB.ModelProperties(HB.ModelEnergyProperties.Default));
                    var constrcutionsInModel = md.Properties.Energy.Constructions
                        .Where(_ => _.Obj.GetType().Name.Contains("Abridged"))
                        .Select(_ => _.Obj as HB.Energy.IConstruction)
                        .ToList();
                    var dialog = new Honeybee.UI.Dialog_ConstructionManager(constrcutionsInModel);
                    dialog.ShowModal(this);

                };

                var cSetManager = new Button() { Text = "ConstructionSet Manager" };
                cSetManager.Click += (s, e) =>
                {
                    var md = new HB.Model("", new HB.ModelProperties(HB.ModelEnergyProperties.Default));
                    var cSets = md.Properties.Energy.ConstructionSets;
                    var constrcutionSetsInModel = md.Properties.Energy.ConstructionSets
                      .Where(_ => _.Obj is HB.ConstructionSetAbridged)
                      .Select(_ => _.Obj as HB.Energy.IBuildingConstructionset)
                      .ToList();
                    var globalCSet = md.Properties.Energy.GlobalConstructionSet;

                    var dialog = new Dialog_ConstructionSetManager(constrcutionSetsInModel, (id) => id == globalCSet);
                    dialog.ShowModal(this);

                };

                var simuParam = new Button() { Text = "Simulation Parameter" };
                simuParam.Click += (s, e) =>
                {
                    var sP = new HB.SimulationParameter();
                    var dialog = new Honeybee.UI.Dialog_SimulationParameter(sP);
                    dialog.ShowModal(this);

                };

                var modelManager = new Button() { Text = "Model Resource" };
                modelManager.Click += (s, e) =>
                {
                    var md = new HB.Model("", new HB.ModelProperties(HB.ModelEnergyProperties.Default));
                    var dialog = new Honeybee.UI.Dialog_ModelResources(md);
                    dialog.ShowModal(this);

                };

                var materialBtn = new Button() { Text = "Material Manager" };
                materialBtn.Click += (s, e) =>
                {
                    var md = new HB.Model("", new HB.ModelProperties(HB.ModelEnergyProperties.Default));
                    //var obj = ModelEnergyProperties.Default.Materials.First(_ => _.Obj is EnergyMaterial).Obj as EnergyMaterial;
                    var dialog = new Honeybee.UI.Dialog_MaterialManager(md);
                    dialog.ShowModal(this);

                };

                panel.AddSeparateRow(btn);
                panel.AddSeparateRow(Messagebtn);
                panel.AddSeparateRow(cSetbtn);
                panel.AddSeparateRow(pTypebtn);
                panel.AddSeparateRow(pTypeMngbtn);
                panel.AddSeparateRow(schbtn);
                panel.AddSeparateRow(conbtn);
                panel.AddSeparateRow(cSetManager);
                panel.AddSeparateRow(simuParam);
                panel.AddSeparateRow(modelManager);
                panel.AddSeparateRow(materialBtn);
                panel.AddSeparateRow(null);
                Content = panel;
            }
        }
    }
}
