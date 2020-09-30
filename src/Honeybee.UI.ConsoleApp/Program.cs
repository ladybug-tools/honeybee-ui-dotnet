using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using HB = HoneybeeSchema;
using HoneybeeSchema;

namespace Honeybee.UI.ConsoleApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = new Application();
            app.Run(new MyForm());
            Console.ReadLine();
        }

        public class MyForm : Form
        {
            public MyForm()
            {
                //ClientSize = new Eto.Drawing.Size(400, 300);
                Title = "Eto.Forms";
                Width = 400;

                var panel = new DynamicLayout();
                var btn = new Button() { Text="Room Energy Property"};
                btn.Click += (s, e) =>
                {
                    var energyProp = new HoneybeeSchema.RoomEnergyPropertiesAbridged();
                    //var dialog = new Honeybee.UI.Dialog_RoomEnergyProperty(energyProp, HB.ModelEnergyProperties.Default);
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
                    var dialog = new Dialog_ConstructionSetManager(constrcutionSetsInModel);
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
                    var materialsInModel = md.Properties.Energy.Materials.Select(_ => _.Obj as HB.Energy.IMaterial).ToList();

                    var dialog = new Honeybee.UI.Dialog_MaterialManager(materialsInModel);
                    dialog.ShowModal(this);

                };

                var stndBtn = new Button() { Text = "Standards" };
                stndBtn.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_OpsProgramTypes();
                    dialog.ShowModal(this);

                };

                var modifierBtn = new Button() { Text = "Modifier Manager" };
                modifierBtn.Click += (s, e) =>
                {
                    var hbModel = new HB.Model("", new HB.ModelProperties(radiance: HB.ModelRadianceProperties.Default));
                    var existingItems = hbModel.Properties.Radiance.Modifiers
                        .OfType<HoneybeeSchema.ModifierBase>()
                        .ToList();

                    var dup = existingItems.Select(_ => _.Duplicate()).OfType<HoneybeeSchema.ModifierBase>().ToList();
                    var dialog = new Honeybee.UI.Dialog_ModifierManager(dup);

                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        hbModel.Properties.Radiance.Modifiers.Clear();
                        hbModel.AddModifiers(dialog_rc);

                    }

                };

                var modifierSetBtn = new Button() { Text = "ModifierSet Manager" };
                modifierSetBtn.Click += (s, e) =>
                {
                    var hbModel = new HB.Model("", new HB.ModelProperties(radiance: HB.ModelRadianceProperties.Default));
                    var existingItems = hbModel.Properties.Radiance.ModifierSets
                    .OfType<HoneybeeSchema.ModifierSetAbridged>()
                    .ToList();

                    var dup = existingItems.Select(_ => _.Duplicate()).OfType<HoneybeeSchema.ModifierSetAbridged>().ToList();
                    var dialog = new Honeybee.UI.Dialog_ModifierSetManager(dup);

                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        hbModel.Properties.Radiance.ModifierSets.Clear();
                        hbModel.AddModifierSets(dialog_rc.OfType<IDdRadianceBaseModel>().ToList());

                    }

                };

                var outputs = new Button() { Text = "EPOutputs" };
                outputs.Click += (s, e) =>
                {
                    var epoutput = new HB.SimulationOutput();
                    var dialog = new Honeybee.UI.Dialog_EPOutputs(epoutput);
                    dialog.ShowModal(this);

                };

                var opsProgramType = new Button() { Text = "OpenStudioProgramType" };
                opsProgramType.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_OpsProgramTypes();
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
                panel.AddSeparateRow(stndBtn);
                panel.AddSeparateRow(modifierBtn);
                panel.AddSeparateRow(modifierSetBtn);
                panel.AddSeparateRow(outputs);
                panel.AddSeparateRow(opsProgramType);
                panel.AddSeparateRow(null);
                Content = panel;
            }
        }
    }
}
