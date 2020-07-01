using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_RoomEnergyProperty: Dialog<HB.RoomEnergyPropertiesAbridged>
    {
     
        public Dialog_RoomEnergyProperty(HB.RoomEnergyPropertiesAbridged roomEnergyProperties, bool updateChangesOnly = false)
        {
            try
            {
                var EnergyProp = roomEnergyProperties ?? new HB.RoomEnergyPropertiesAbridged();
                var noChangeEnergyProp = new HB.RoomEnergyPropertiesAbridged(
                    "No Changes", 
                    "No Changes", 
                    "No Changes", 
                    new PeopleAbridged("No Changes",0,"",""), 
                    new LightingAbridged("No Changes",0,""),
                    new ElectricEquipmentAbridged("No Changes", 0,""),
                    new GasEquipmentAbridged("No Changes", 0,""),
                    new InfiltrationAbridged("No Changes", 0,""),
                    new VentilationAbridged("No Changes"),
                    new SetpointAbridged("No Changes", "","")
                    );

                if (updateChangesOnly)
                    EnergyProp = noChangeEnergyProp;


                Padding = new Padding(15);
                Resizable = true;
                Title = "Room Energy Properties - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 620);
                this.Icon = DialogHelper.HoneybeeIcon;

                //Get constructions
                //var cSets = EnergyLibrary.DefaultConstructionSets.ToList();
                var cSets = EnergyLibrary.InModelEnergyProperties.ConstructionSets
                    .Where(_=>_.Obj is ConstructionSetAbridged)
                    .Select(_ => _.Obj as ConstructionSetAbridged).ToList();

                if (updateChangesOnly)
                    cSets.Insert(0, new ConstructionSetAbridged("No Changes"));


                var constructionSetDP = DialogHelper.MakeDropDown(EnergyProp.ConstructionSet, (v) => EnergyProp.ConstructionSet = v?.Identifier,
                    cSets, "Default Generic Construction Set");

                //var cSetBtn = new Button();
                //cSetBtn.Text = "+";
                //cSetBtn.Width = 20;
                //cSetBtn.Height = 20;
                //cSetBtn.Click += (s, e) => 
                //{

                //    var constrcutionSetsInModel = modelProp.ConstructionSets
                //    .Where(_ => _.Obj is HB.ConstructionSetAbridged)
                //    .Select(_ => _.Obj as HB.Energy.IBuildingConstructionset)
                //    .ToList();

                //    var globalCSet = modelProp.GlobalConstructionSet;
                //    var dialog = new Dialog_ConstructionSetManager(constrcutionSetsInModel, (id) => id == globalCSet);
                //    var dialog_rc = dialog.ShowModal(this);
                //    if (dialog_rc != null)
                //    {
                //        var newCSets = dialog_rc.OfType<HB.ConstructionSetAbridged>();
                //        modelProp.ConstructionSets.Clear();

                //        var dpItems = constructionSetDP.Items.Take(1).ToList();
                //        constructionSetDP.Items.Clear();
                //        foreach (var item in newCSets)
                //        {
                //            modelProp.ConstructionSets.Add(item);
                //            var name = item.DisplayName ?? item.Identifier;
                //            dpItems.Add(new ListItem() { Text = name, Key = name });
                //        }
                //        constructionSetDP.Items.AddRange(dpItems);
                //        constructionSetDP.SelectedIndex = 0;

                //    }
                //};

                //Get programs
                //var pTypes = EnergyLibrary.DefaultProgramTypes.ToList();
                var pTypes = EnergyLibrary.InModelEnergyProperties.ProgramTypes
                    .Where(_ => _.Obj is HB.ProgramTypeAbridged)
                    .Select(_ => _.Obj as HB.ProgramTypeAbridged).ToList();

                if (updateChangesOnly)
                    pTypes.Insert(0, new ProgramTypeAbridged("No Changes"));

                var programTypesDP = DialogHelper.MakeDropDown(EnergyProp.ProgramType, (v) => EnergyProp.ProgramType = v?.Identifier,
                   pTypes, "Unoccupied, NoLoads");

                //var pTypeBtn = new Button();
                //pTypeBtn.Text = "+";
                //pTypeBtn.Width = 20;
                //pTypeBtn.Height = 20;
                //pTypeBtn.Click += (s, e) =>
                //{

                //    var pTypeInModel = modelProp.ProgramTypes
                //   .Where(_ => _.Obj is HB.ProgramTypeAbridged)
                //   .Select(_ => _.Obj as HB.ProgramTypeAbridged)
                //   .ToList();


                //    var dialog = new Dialog_ProgramTypeManager(pTypeInModel);
                //    var dialog_rc = dialog.ShowModal(this);
                //    if (dialog_rc != null)
                //    {
                //        modelProp.ProgramTypes.Clear();

                //        var newPTypes = dialog_rc.OfType<HB.ProgramTypeAbridged>();
                //        var dpItems = programTypesDP.Items.Take(1).ToList();
                //        programTypesDP.Items.Clear();
                //        foreach (var item in newPTypes)
                //        {
                //            modelProp.ProgramTypes.Add(item);
                //            var name = item.DisplayName ?? item.Identifier;
                //            dpItems.Add(new ListItem() { Text = name, Key = name });
                //        }
                //        programTypesDP.Items.AddRange(dpItems);
                //        programTypesDP.SelectedIndex = 0;
                //    }
                //};


                //Get HVACs
                //var hvacs = EnergyLibrary.DefaultHVACs.ToList();
                var hvacs = EnergyLibrary.InModelEnergyProperties.Hvacs.ToList();

                if (updateChangesOnly)
                    hvacs.Insert(0, new IdealAirSystemAbridged("No Changes"));

                var hvacDP = DialogHelper.MakeDropDown(EnergyProp.Hvac, (v) => EnergyProp.Hvac = v?.Identifier,
                   hvacs, "Unconditioned");


                var defaultByProgramType = "By Room Program Type";
                //Get people
                var ppls = EnergyLibrary.DefaultPeopleLoads.ToList();
                if (updateChangesOnly)
                    ppls.Insert(0, noChangeEnergyProp.People);
                var peopleDP = DialogHelper.MakeDropDown(EnergyProp.People, (v) => EnergyProp.People = v,
                    ppls, defaultByProgramType);

                //Get lighting
                var lpds = EnergyLibrary.DefaultLightingLoads.ToList();
                if (updateChangesOnly)
                    lpds.Insert(0, noChangeEnergyProp.Lighting);
                var lightingDP = DialogHelper.MakeDropDown(EnergyProp.Lighting, (v) => EnergyProp.Lighting = v,
                    lpds, defaultByProgramType);

                //Get ElecEqp
                var eqps = EnergyLibrary.DefaultElectricEquipmentLoads.ToList();
                if (updateChangesOnly)
                    eqps.Insert(0, noChangeEnergyProp.ElectricEquipment);
                var elecEqpDP = DialogHelper.MakeDropDown(EnergyProp.ElectricEquipment, (v) => EnergyProp.ElectricEquipment = v,
                    eqps, defaultByProgramType);

                //Get gasEqp
                var gas = EnergyLibrary.GasEquipmentLoads.ToList();
                if (updateChangesOnly)
                    gas.Insert(0, noChangeEnergyProp.GasEquipment);
                var gasEqpDP = DialogHelper.MakeDropDown(EnergyProp.GasEquipment, (v) => EnergyProp.GasEquipment = v,
                    gas, defaultByProgramType);

                //Get infiltration
                var inf = EnergyLibrary.DefaultInfiltrationLoads.ToList();
                if (updateChangesOnly)
                    inf.Insert(0, noChangeEnergyProp.Infiltration);
                var infilDP = DialogHelper.MakeDropDown(EnergyProp.Infiltration, (v) => EnergyProp.Infiltration = v,
                    inf, defaultByProgramType);


                //Get ventilation
                var vent = EnergyLibrary.DefaultVentilationLoads.ToList();
                if (updateChangesOnly)
                    vent.Insert(0, noChangeEnergyProp.Ventilation);
                var ventDP = DialogHelper.MakeDropDown(EnergyProp.Ventilation, (v) => EnergyProp.Ventilation = v,
                    vent, defaultByProgramType);

                //Get setpoint
                var spt = EnergyLibrary.DefaultSetpoints.ToList();
                if (updateChangesOnly)
                    spt.Insert(0, noChangeEnergyProp.Setpoint);
                var setPtDP = DialogHelper.MakeDropDown(EnergyProp.Setpoint, (v) => EnergyProp.Setpoint = v,
                    spt, defaultByProgramType);


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => 
                {
                    Close(EnergyProp); 
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };


                var layout = new DynamicLayout();
                //layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                layout.AddSeparateRow("Room ConstructionSet:");
                layout.AddSeparateRow(constructionSetDP);
                layout.AddSeparateRow("Room Program Type:");
                layout.AddSeparateRow(programTypesDP);
                layout.AddSeparateRow("Room HVAC:");
                layout.AddSeparateRow(hvacDP);
                layout.AddSeparateRow("");
                layout.AddSeparateRow("People [ppl/m2]:");
                layout.AddSeparateRow(peopleDP);
                layout.AddSeparateRow("Lighting [W/m2]:");
                layout.AddSeparateRow(lightingDP);
                layout.AddSeparateRow("Electric Equipment [W/m2]:");
                layout.AddSeparateRow(elecEqpDP);
                layout.AddSeparateRow("Gas Equipment [W/m2]:");
                layout.AddSeparateRow(gasEqpDP);
                layout.AddSeparateRow("Infiltration [m3/s per m2 facade @4Pa]:");
                layout.AddSeparateRow(infilDP);
                layout.AddSeparateRow("Ventilation [m3/s.m2]:");
                layout.AddSeparateRow(ventDP);
                layout.AddSeparateRow("Setpoint [C]:");
                layout.AddSeparateRow(setPtDP);
                layout.AddSeparateRow(buttons);
                layout.AddSeparateRow(null);
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
