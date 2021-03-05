using Eto.Drawing;
using Eto.Forms;
using System;
using System.Linq;
using System.Collections.Generic;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_OpsConstructionSet : Dialog<(ConstructionSetAbridged constructionSet, IEnumerable<HoneybeeSchema.Energy.IConstruction> constructions, IEnumerable<HoneybeeSchema.Energy.IMaterial> materials)>
    {
        private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_OpsConstructionSet(ModelEnergyProperties libSource)
        {
            try
            {
                this.ModelEnergyProperties = libSource;
                //var output = simulationOutput;
                var vm = OpsConstructionSetsViewModel.Instance;

                Padding = new Padding(5);
                Resizable = true;
                Title = $"OpenStudio Construction Set - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 300);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout() { DataContext = vm};
                layout.Spacing = new Size(8, 8);
                layout.Padding = new Padding(15);


                //"2004::ClimateZone1::WoodFramed"
                var vintage = new DropDown();
                var climateZone = new DropDown();
                var constructionType = new DropDown();
                var cloneBtn = new Button() { Text = "Clone and Edit" };

                vintage.DataStore = vm.VintageNames;
                vintage.SelectedKeyBinding.BindDataContext((OpsConstructionSetsViewModel m) => m.Vintage);

                climateZone.BindDataContext(c => c.DataStore, (OpsConstructionSetsViewModel m) => m.ClimateZones);
                climateZone.SelectedKeyBinding.BindDataContext((OpsConstructionSetsViewModel m) => m.ClimateZone);

                constructionType.BindDataContext(c => c.DataStore, (OpsConstructionSetsViewModel m) => m.ConstructionSetTypes);
                constructionType.SelectedKeyBinding.BindDataContext((OpsConstructionSetsViewModel m) => m.ConstructionSetType);


                cloneBtn.Click += (s, e) => {
                    var cSet = vm.ConstructionWithMats.ConstructionSet;
                    var consts = vm.ConstructionWithMats.constructions;
                    var mats = vm.ConstructionWithMats.materials;
                    var id = Guid.NewGuid().ToString();

                    cSet.DisplayName =  $"{vm.FullConstructionSet}_dup";
                    cSet.Identifier = id;
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(this.ModelEnergyProperties, cSet);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        Close((dialog_rc, consts, mats));
                    }

                };

                layout.AddRow("Vintage:");
                layout.AddRow(vintage);
                layout.AddRow("Climate Zone:");
                layout.AddRow(climateZone);
                layout.AddRow("Construction Type:");
                layout.AddRow(constructionType);
                //layout.AddRow(textArea);
                layout.AddSeparateRow(cloneBtn);

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => {
                    var cSet = vm.ConstructionWithMats.ConstructionSet;
                    var consts = vm.ConstructionWithMats.constructions;
                    var mats = vm.ConstructionWithMats.materials;

                    cSet.DisplayName = vm.FullConstructionSet;
                    cSet.Identifier = Guid.NewGuid().ToString();

                    Close((cSet, consts, mats));
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
