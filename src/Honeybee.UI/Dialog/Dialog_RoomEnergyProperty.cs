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
        private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_RoomEnergyProperty(ModelEnergyProperties libSource, HB.RoomEnergyPropertiesAbridged roomEnergyProperties, bool updateChangesOnly = false)
        {
            try
            {
                this.ModelEnergyProperties = libSource;
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
                var cSets = this.ModelEnergyProperties.ConstructionSets.OfType<ConstructionSetAbridged>().ToList();

                if (updateChangesOnly)
                    cSets.Insert(0, new ConstructionSetAbridged("No Changes"));


                var constructionSetDP = DialogHelper.MakeDropDown(EnergyProp.ConstructionSet, (v) => EnergyProp.ConstructionSet = v?.Identifier,
                    cSets, "Default Generic Construction Set");



                //Get programs
                //var pTypes = EnergyLibrary.DefaultProgramTypes.ToList();
                var pTypes = this.ModelEnergyProperties.ProgramTypes.OfType<HB.ProgramTypeAbridged>().ToList();
         
                if (updateChangesOnly)
                    pTypes.Insert(0, new ProgramTypeAbridged("No Changes"));

                var programTypesDP = DialogHelper.MakeDropDown(EnergyProp.ProgramType, (v) => EnergyProp.ProgramType = v?.Identifier,
                   pTypes, "Unoccupied, NoLoads");

              

                //Get HVACs
                //var hvacs = EnergyLibrary.DefaultHVACs.ToList();+
                var hvacs = this.ModelEnergyProperties.Hvacs.OfType<HoneybeeSchema.Energy.IHvac>().ToList();

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
                throw new ArgumentException($"Failed to open RoomEnergyProperty dialog:\n{e.Message}");
            }
            
            
        }
    
    
    }
}
