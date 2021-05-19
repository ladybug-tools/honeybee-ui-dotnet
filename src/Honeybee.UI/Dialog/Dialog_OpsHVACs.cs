using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Honeybee.UI
{
    public class Dialog_OpsHVACs : Dialog<HoneybeeSchema.Energy.IHvac>
    {
        //private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_OpsHVACs(HoneybeeSchema.Energy.IHvac hvac = default)
        {
            //this.ModelEnergyProperties = libSource;
            var vm = new OpsHVACsViewModel(hvac);
            this.Padding = new Padding(10);
            Title = $"From OpenStudio HVAC library - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout() { DataContext = vm };
            layout.DefaultSpacing = new Size(5, 5);
            layout.DefaultPadding = new Padding(5);


            // add a new system from lib
            if (hvac == null)
            {
                var hvacGroups = new DropDown();
                var hvacTypes = new DropDown();
                var hvacEquipments = new DropDown();

                hvacGroups.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacGroups);
                hvacGroups.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacGroup);

                hvacTypes.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacTypes);
                hvacTypes.ItemTextBinding = Binding.Delegate<Type, string>(t => vm.HVACTypesDic[t]);
                hvacTypes.SelectedValueBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacType);

                hvacEquipments.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacEquipmentTypes);
                hvacEquipments.ItemTextBinding = Binding.Delegate<string, string>(t => vm.HVACsDic[t]);
                hvacEquipments.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacEquipmentType);

                layout.AddRow("HVAC Groups:");
                layout.AddRow(hvacGroups);
                layout.AddRow("HVAC Types:");
                layout.AddRow(hvacTypes);
                layout.AddRow("HVAC Equipment Types:");
                layout.AddRow(hvacEquipments);
                layout.AddRow(null);
            }


            var year = new DropDown();
            var nameText = new TextBox();
            var economizer = new DropDown();
            var sensible = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };
            var latent = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2, Increment = 0.1 };


            nameText.TextBinding.BindDataContext((OpsHVACsViewModel m) => m.Name);

            year.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Vintages);
            year.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Vintage);

            var economizerTitle = new Label() { Text = "Economizer:" };
            economizer.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Economizers);
            economizer.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Economizer);
            economizer.BindDataContext(c => c.Enabled, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizer.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizerTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);

            var sensibleTitle = new Label() { Text = "Sensible Heat Recovery:" };
            sensible.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.SensibleHR);
            sensible.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
            sensibleTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
      

            var latentTitle = new Label() { Text = "Latent Heat Recovery:" };
            latent.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.LatentHR);
            latent.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);
            latentTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);

            layout.BeginGroup("HVAC System settings", new Padding(5), new Size(5,5));
            layout.AddRow("Name:");
            layout.AddRow(nameText);
            layout.AddRow("Vintage:");
            layout.AddRow(year);
            layout.AddRow(economizerTitle);
            layout.AddRow(economizer);
            layout.AddRow(sensibleTitle);
            layout.AddRow(sensible);
            layout.AddRow(latentTitle);
            layout.AddRow(latent);
            layout.EndGroup();
            //layout.AddSeparateRow(cloneBtn);

            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => {
                var obj = vm.GreateHvac(hvac);
                Close(obj);
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            layout.AddSeparateRow(null, OKButton, this.AbortButton, null);
            layout.AddRow(null);
            Content = layout;


        }
        
     

    }
}
