using Eto.Drawing;
using Eto.Forms;
using System;
using HoneybeeSchema;
using Honeybee.UI.ViewModel;

namespace Honeybee.UI
{
    public class Dialog_RoomEnergyProperty: Dialog<RoomEnergyPropertiesAbridged>
    {
        private RoomEnergyPropertyViewModel ViewModel { get; set; }
        public Dialog_RoomEnergyProperty(ModelEnergyProperties libSource, RoomEnergyPropertiesAbridged roomEnergyProperties, bool updateChangesOnly = false)
        {
            try
            {

                this.ViewModel = new RoomEnergyPropertyViewModel(this, libSource, roomEnergyProperties, updateChangesOnly); 

                Padding = new Padding(15);
                Title = "Room Energy Properties - Honeybee";
                WindowStyle = WindowStyle.Default;
                Width = 450;
                this.Icon = DialogHelper.HoneybeeIcon;

                //Get constructions
                var constructionSetDP = new DropDown();
                constructionSetDP.Bind((t) => t.DataStore, this.ViewModel, v => v.ConstructionSets);
                constructionSetDP.ItemTextBinding = Binding.Delegate<ConstructionSetAbridged, string>(g => g.DisplayName ?? g.Identifier);
                constructionSetDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.ConstructionSet);
                var addCSetBtn = new LinkButton() { Text = "Add", ToolTip = "Add a new one from OpenStudio library"};
                addCSetBtn.Command = ViewModel.AddNewConstructionSet;


                //Get programs
                var programTypesDP = new DropDown();
                programTypesDP.Bind((t) => t.DataStore, this.ViewModel, v => v.ProgramTypes);
                programTypesDP.ItemTextBinding = Binding.Delegate<ProgramTypeAbridged, string>(g => g.DisplayName ?? g.Identifier);
                programTypesDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.ProgramType);
                var addPTypeBtn = new LinkButton() { Text = "Add", ToolTip = "Add a new one from OpenStudio library" };
                addPTypeBtn.Command = ViewModel.AddNewProgramType;

                //Get HVACs
                var hvacDP = new DropDown();
                hvacDP.Bind((t) => t.DataStore, this.ViewModel, v => v.Hvacs);
                hvacDP.ItemTextBinding = Binding.Delegate<HoneybeeSchema.Energy.IHvac, string>(g => g.DisplayName ?? g.Identifier);
                hvacDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.HVAC);
                var addHvacBtn = new LinkButton() { Text = "Add", ToolTip = "Add a new one from OpenStudio library", Enabled = false};
                //addHvacBtn.Command = ViewModel.AddNewConstructionSet;


                //Get people
                var peopleDP = new DropDown();
                peopleDP.Bind((t) => t.DataStore, this.ViewModel, v => v.Peoples);
                peopleDP.ItemTextBinding = Binding.Delegate<PeopleAbridged, string>(g => g.DisplayName ?? g.Identifier);
                peopleDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.People);
                var addpplBtn = new LinkButton() { Text = "Add", ToolTip = "Override with a new load", Enabled = false };

                //Get lighting
                var lightingDP = new DropDown();
                lightingDP.Bind((t) => t.DataStore, this.ViewModel, v => v.Lightings);
                lightingDP.ItemTextBinding = Binding.Delegate <LightingAbridged, string>(g => g.DisplayName ?? g.Identifier);
                lightingDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.Lighting);
                var addLpdBtn = new LinkButton() { Text = "Add", ToolTip = "Override with a new load", Enabled = false };

                //Get ElecEqp
                var elecEqpDP = new DropDown();
                elecEqpDP.Bind((t) => t.DataStore, this.ViewModel, v => v.ElectricEquipments);
                elecEqpDP.ItemTextBinding = Binding.Delegate<ElectricEquipmentAbridged, string>(g => g.DisplayName ?? g.Identifier);
                elecEqpDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.ElectricEquipment);
                var addEqpBtn = new LinkButton() { Text = "Add", ToolTip = "Override with a new load", Enabled = false };

                //Get gasEqp
                var gasEqpDP = new DropDown();
                gasEqpDP.Bind((t) => t.DataStore, this.ViewModel, v => v.GasEquipments);
                gasEqpDP.ItemTextBinding = Binding.Delegate<GasEquipmentAbridged, string>(g => g.DisplayName ?? g.Identifier);
                gasEqpDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.GasEquipment);
                var addGasBtn = new LinkButton() { Text = "Add", ToolTip = "Override with a new load", Enabled = false };

                //Get infiltration
                var infilDP = new DropDown();
                infilDP.Bind((t) => t.DataStore, this.ViewModel, v => v.Infiltrations);
                infilDP.ItemTextBinding = Binding.Delegate<InfiltrationAbridged, string>(g => g.DisplayName ?? g.Identifier);
                infilDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.Infiltration);
                var addInfilBtn = new LinkButton() { Text = "Add", ToolTip = "Override with a new load", Enabled = false };

                //Get ventilation
                var ventDP = new DropDown();
                ventDP.Bind((t) => t.DataStore, this.ViewModel, v => v.Ventilations);
                ventDP.ItemTextBinding = Binding.Delegate<VentilationAbridged, string>(g => g.DisplayName ?? g.Identifier);
                ventDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.Ventilation);
                var addVentBtn = new LinkButton() { Text = "Add", ToolTip = "Override with a new load", Enabled = false };

                //Get setpoint
                var setPtDP = new DropDown();
                setPtDP.Bind((t) => t.DataStore, this.ViewModel, v => v.Setpoints);
                setPtDP.ItemTextBinding = Binding.Delegate<SetpointAbridged, string>(g => g.DisplayName ?? g.Identifier);
                setPtDP.Bind((t) => t.SelectedValue, this.ViewModel, v => v.Setpoint);
                var addSptBtn = new LinkButton() { Text = "Add", ToolTip = "Override with a new load", Enabled = false };


                var OK = new Button { Text = "OK" };
                OK.Click += (sender, e) => 
                {
                    Close(this.ViewModel.HoneybeeObject); 
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

              
                var layout = new DynamicLayout();
                //layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                layout.AddSeparateRow("Room ConstructionSet:", null, addCSetBtn);
                layout.AddSeparateRow(constructionSetDP);
                layout.AddSeparateRow("Room Program Type:", null, addPTypeBtn);
                layout.AddSeparateRow(programTypesDP);
                layout.AddSeparateRow("Room HVAC:", null, addHvacBtn);
                layout.AddSeparateRow(hvacDP);
                layout.AddSeparateRow("");
                layout.AddSeparateRow("People [ppl/m2]:", null, addpplBtn);
                layout.AddSeparateRow(peopleDP);
                layout.AddSeparateRow("Lighting [W/m2]:", null, addLpdBtn);
                layout.AddSeparateRow(lightingDP);
                layout.AddSeparateRow("Electric Equipment [W/m2]:", null, addEqpBtn);
                layout.AddSeparateRow(elecEqpDP);
                layout.AddSeparateRow("Gas Equipment [W/m2]:", null, addGasBtn);
                layout.AddSeparateRow(gasEqpDP);
                layout.AddSeparateRow("Infiltration [m3/s per m2 facade @4Pa]:", null, addInfilBtn);
                layout.AddSeparateRow(infilDP);
                layout.AddSeparateRow("Ventilation [m3/s.m2]:", null, addVentBtn);
                layout.AddSeparateRow(ventDP);
                layout.AddSeparateRow("Setpoint [C]:", null, addSptBtn);
                layout.AddSeparateRow(setPtDP);
                layout.AddSeparateRow("");
                layout.AddSeparateRow(null, OK, this.AbortButton, null);
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
