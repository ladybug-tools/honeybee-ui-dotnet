using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
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
                var md = new Model("id", new ModelProperties(new ModelEnergyProperties(), new ModelRadianceProperties()) );

                var panel = new DynamicLayout();

                var ltn = new LightingAbridged("lnt", 12, "Always On");
                var ltn2 = new LightingAbridged("lnt", 15, "Always On");
                var dlightCtrl = new DaylightingControl(new List<double>() { 0.5, 0.1, 0.5 }, offAtMinimum: true);
                var ppl = new PeopleAbridged("ppl", 0.1, "Always On", "Always On", latentFraction: new Autocalculate());
                var RoomPropertybtn = new Button() { Text = "2 Rooms Property" };
                var rm1 = new Room("id1", new List<Face>(), new RoomPropertiesAbridged(new RoomEnergyPropertiesAbridged("aaa", lighting: ltn)), "name1", multiplier: 1, story: "11");
                var rm2 = new Room($"Room_{Guid.NewGuid()}", new List<Face>(), new RoomPropertiesAbridged(new RoomEnergyPropertiesAbridged("bbb", lighting: ltn2, people:ppl, daylightingControl: dlightCtrl)), "name2", multiplier: 2, story: "22");
                rm2.UserData = new { RhinoLayer = "layer 01", Color = "Red" };
                rm2 = rm2.DuplicateRoom();

                var rms = new List<Room>() { rm1, rm2 };
                RoomPropertybtn.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_RoomProperty(md.Properties ,rms);
                    //dialog.SetSensorPositionPicker(() => { return new List<double>(); });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }
                        rms = dialog_rc;
                      
                    }
                };


                var RoomPropertybtn2 = new Button() { Text = "1 Room Property" };

                RoomPropertybtn2.Click += (s, e) =>
                {
                    //md.Properties.Energy?.Hvacs = null;
                    var dialog = new Honeybee.UI.Dialog_RoomProperty(md.Properties, new List<Room>() { rm2 });
                    //dialog.SetSensorPositionPicker(() => { return new List<double>(); });
                    dialog.SetInternalMassPicker(() => 22);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        rm2 = dialog_rc.FirstOrDefault();
                        Console.WriteLine(rm2.ToJson(true));
                        
                    }
                };

                var facePropertybtn = new Button() { Text = "2 Faces Property" };
                var face = new Face("faceId", new Face3D(new List<List<double>>()), FaceType.Floor, new Ground(), new FacePropertiesAbridged(new FaceEnergyPropertiesAbridged("aa"), new FaceRadiancePropertiesAbridged("bb", "cc")));
                var face2 = new Face($"Face_{Guid.NewGuid()}", new Face3D(new List<List<double>>()), FaceType.Wall, new Outdoors(), new FacePropertiesAbridged(new FaceEnergyPropertiesAbridged("Generic Exterior Wall")), "Face name");
                facePropertybtn.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_FaceProperty(md.Properties, new List<Face>() { face, face2 });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }

                    }
                };

                var facePropertybtn2 = new Button() { Text = "1 Face Property" };
                facePropertybtn2.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_FaceProperty(md.Properties, new List<Face>() { face2 });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }

                    }
                };


                var aptPropertybtn = new Button() { Text = "1 Apt Property" };
                var apt = new Aperture("aptId", new Face3D(new List<List<double>>()), new Outdoors(), new AperturePropertiesAbridged(new ApertureEnergyPropertiesAbridged("aa"), new ApertureRadiancePropertiesAbridged("bb", "cc")));
                var apt2 = new Aperture($"Aperture_{Guid.NewGuid()}", new Face3D(new List<List<double>>()), new Outdoors(), new AperturePropertiesAbridged(new ApertureEnergyPropertiesAbridged("Generic Exterior Wall")), "apt name", isOperable: true);
                aptPropertybtn.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_ApertureProperty(md.Properties, new List<Aperture>() { apt2 });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }

                    }
                };
                var aptPropertybtn2 = new Button() { Text = "2 Apts Property" };
                aptPropertybtn2.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_ApertureProperty(md.Properties, new List<Aperture>() { apt, apt2 });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }

                    }
                };

                var doorPropertybtn = new Button() { Text = "1 Door Property" };
                var door = new Door("aptId", new Face3D(new List<List<double>>()), new Outdoors(), new DoorPropertiesAbridged());
                doorPropertybtn.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_DoorProperty(md.Properties, new List<Door>() { door });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }

                    }
                };

                var shdPropertybtn = new Button() { Text = "1 Shade Property" };
                var shd = new Shade("shdId", new Face3D(new List<List<double>>()),  new ShadePropertiesAbridged(new ShadeEnergyPropertiesAbridged("aa"), new ShadeRadiancePropertiesAbridged("bb", "cc")));
                var shd2 = new Shade($"Shade_{Guid.NewGuid()}", new Face3D(new List<List<double>>()), new ShadePropertiesAbridged(new ShadeEnergyPropertiesAbridged("Generic Exterior Wall")), "shd name", isDetached: true);
                shdPropertybtn.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_ShadeProperty(md.Properties, new List<Shade>() { shd2 });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }
                    }
                };
                var shdPropertybtn2 = new Button() { Text = "2 Shade Property" };
                shdPropertybtn2.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_ShadeProperty(md.Properties, new List<Shade>() { shd, shd2 });
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        foreach (var item in dialog_rc)
                        {
                            Console.WriteLine(item.ToJson(true));
                        }
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
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(md.Properties.Energy, cSet);
                    dialog.ShowModal(this);

                };

                var pTypebtn = new Button() { Text = "ProgramType" };
                pTypebtn.Click += (s, e) =>
                {
                    var pType = new HoneybeeSchema.ProgramTypeAbridged(identifier: Guid.NewGuid().ToString());
                    var dialog = new Honeybee.UI.Dialog_ProgramType(md.Properties.Energy, pType);
                    dialog.ShowModal(this);

                };

                var pTypeMngbtn = new Button() { Text = "ProgramTypeManager" };
                pTypeMngbtn.Click += (s, e) =>
                {
                    var lib = md.Properties.Energy;
                    var dialog = new Honeybee.UI.Dialog_ProgramTypeManager(ref lib);
                    var dialog_rc =dialog.ShowModal(this);
                  
                };

                var schbtn = new Button() { Text = "ScheduleRulesetManager" };
                schbtn.Click += (s, e) =>
                {
                    var lib = md.Properties.Energy;
                    var dialog = new Honeybee.UI.Dialog_ScheduleRulesetManager(ref lib);
                    dialog.ShowModal(this);

                };

                var conbtn = new Button() { Text = "ConstructionManager" };
                conbtn.Click += (s, e) =>
                {
                    var lib = md.Properties.Energy;
                    var dialog = new Honeybee.UI.Dialog_ConstructionManager(ref lib);
                    dialog.ShowModal(this);

                };

                var cSetManager = new Button() { Text = "ConstructionSet Manager" };
                cSetManager.Click += (s, e) =>
                {
                    var lib = md.Properties.Energy;
                    var dialog = new Dialog_ConstructionSetManager(ref lib);
                    dialog.ShowModal(this);

                };

                var simuParam = new Button() { Text = "Simulation Parameter" };
                simuParam.Click += (s, e) =>
                {
                    var sP = new SimulationParameter();
                    var dialog = new Honeybee.UI.Dialog_SimulationParameter(sP);
                    dialog.ShowModal(this);

                };

                var modelManager = new Button() { Text = "Model Resource" };
                modelManager.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_ModelResources(md);
                    dialog.ShowModal(this);

                };

                var materialBtn = new Button() { Text = "Material Manager" };
                materialBtn.Click += (s, e) =>
                {
                    var lib = md.Properties.Energy;
                    var dialog = new Honeybee.UI.Dialog_MaterialManager(ref lib);
                    var newMaterials = dialog.ShowModal(this);
                    if (newMaterials != null)
                    {
                        md.Properties.Energy.Materials.Clear();
                        md.AddMaterials(newMaterials);
                    }

                };

                var stndBtn = new Button() { Text = "Standards" };
                stndBtn.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_OpsProgramTypes(md.Properties.Energy);
                    dialog.ShowModal(this);

                };

                var modifierBtn = new Button() { Text = "Modifier Manager" };
                modifierBtn.Click += (s, e) =>
                {
                    var lib = md.Properties.Radiance;
                    var dialog = new Honeybee.UI.Dialog_ModifierManager(ref lib);

                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        md.Properties.Radiance.Modifiers.Clear();
                        md.AddModifiers(dialog_rc);

                    }

                };

                var modifierSetMngBtn = new Button() { Text = "ModifierSet Manager" };
                modifierSetMngBtn.Click += (s, e) =>
                {
                    var lib = md.Properties.Radiance;
                    var dialog = new Honeybee.UI.Dialog_ModifierSetManager(ref lib);

                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        md.Properties.Radiance.ModifierSets.Clear();
                        md.AddModifierSets(dialog_rc);

                    }
                };

                var modifierSetBtn = new Button() { Text = "ModifierSet Editor" };
                modifierSetBtn.Click += (s, e) =>
                {
                    var dup = new ModifierSetAbridged("NewModifierSet");
                    var dialog = new Honeybee.UI.Dialog_ModifierSet(md.Properties.Radiance, dup);

                    var dialog_rc = dialog.ShowModal(this);
                };

                var outputs = new Button() { Text = "EPOutputs" };
                outputs.Click += (s, e) =>
                {
                    var epoutput = new SimulationOutput();
                    var dialog = new Honeybee.UI.Dialog_EPOutputs(epoutput);
                    dialog.ShowModal(this);

                };

                var opsProgramType = new Button() { Text = "OpenStudioProgramType" };
                opsProgramType.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_OpsProgramTypes(md.Properties.Energy);
                    dialog.ShowModal(this);
                };

                var opsHVACs = new Button() { Text = "OpenStudioHVACs" };
                opsHVACs.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_OpsHVACs();
                    dialog.ShowModal(this);
                };

                var HVACManager = new Button() { Text = "HVACsManager" };
                HVACManager.Click += (s, e) =>
                {
                    var lib = md.Properties.Energy;
                    var dialog = new Honeybee.UI.Dialog_HVACManager(ref lib);
                    dialog.ShowModal(this);
                };

                var cSetSel_btn = new Button() { Text = "CSetSelector" };
                cSetSel_btn.Click += (s, e) =>
                {
                    var lib = md.Properties.Energy;
                    var cSetSel = new Dialog_ConstructionSetManager(ref lib, true);
                    var cSetSel_rc = cSetSel.ShowModal(Config.Owner);
                    if (cSetSel_rc != null)
                    {
                        foreach (var item in md.Properties.Energy.ConstructionSets)
                        {
                            Console.WriteLine(item.Obj);
                        }
                    }
                };

                var matchRooms_btn = new Button() { Text = "MatchRooms" };
                matchRooms_btn.Click += (s, e) =>
                {
                    var source = new Room("id", new List<Face>(), new RoomPropertiesAbridged(energy: new RoomEnergyPropertiesAbridged("my construction")), displayName: "SourceName");
                    var target = new Room("id", new List<Face>(), new RoomPropertiesAbridged());
                    var dia = new Dialog_MatchRoomProperties(source, new List<Room>() { target });
                    var rc = dia.ShowModal(Config.Owner);
                    if (rc != null)
                    {
                        foreach (var item in rc)
                        {
                            Console.WriteLine(item.ToJson());
                        }
                    }
                };


                panel.AddSeparateRow(RoomPropertybtn, RoomPropertybtn2, null);
                panel.AddSeparateRow(facePropertybtn, facePropertybtn2, null);
                panel.AddSeparateRow(aptPropertybtn, aptPropertybtn2, null);
                panel.AddSeparateRow(doorPropertybtn, null);
                panel.AddSeparateRow(shdPropertybtn, shdPropertybtn2, null);
                panel.AddSeparateRow(Messagebtn);
                panel.AddSeparateRow(conbtn, cSetbtn, cSetManager, cSetSel_btn, null);
                panel.AddSeparateRow(pTypebtn, pTypeMngbtn, null);
                panel.AddSeparateRow(schbtn);
                panel.AddSeparateRow(simuParam);
                panel.AddSeparateRow(modelManager);
                panel.AddSeparateRow(materialBtn);
                panel.AddSeparateRow(stndBtn);
                panel.AddSeparateRow(modifierBtn, modifierSetMngBtn, modifierBtn, null);
                panel.AddSeparateRow(outputs);
                panel.AddSeparateRow(opsProgramType);
                panel.AddSeparateRow(opsHVACs);
                panel.AddSeparateRow(HVACManager);
                panel.AddSeparateRow(matchRooms_btn);
                panel.AddSeparateRow(null);
                Content = panel;
            }
        }
    }
}
