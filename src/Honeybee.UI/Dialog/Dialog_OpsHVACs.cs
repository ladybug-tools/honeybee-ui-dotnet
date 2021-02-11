﻿using Eto.Drawing;
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
        public Dialog_OpsHVACs()
        {
            //this.ModelEnergyProperties = libSource;
            var vm = new OpsHVACsViewModel();

            Padding = new Padding(5);
            Title = "OpenStudio Standards - Honeybee";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout() { DataContext = vm };
            layout.Spacing = new Size(8, 8);
            layout.Padding = new Padding(15);



            var hvacGroups = new DropDown();
            var hvacTypes = new DropDown();
            var hvacSystems = new DropDown();
            var year = new DropDown();
            var nameText = new TextBox();
            var economizer = new DropDown();
            var sensibleAutosize = new CheckBox() { Text = "Autosize"};
            var sensible = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2 };
            var latentAutosize = new CheckBox() { Text = "Autosize" };
            var latent = new NumericStepper() { MinValue = 0, MaxValue = 1, MaximumDecimalPlaces = 2 };


            year.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Vintages);
            year.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Vintage);

            hvacGroups.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacGroups);
            hvacGroups.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacGroup);

            hvacTypes.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacTypes);
            hvacTypes.ItemTextBinding = Binding.Delegate<Type, string>(t => vm.HVACTypesDic[t]);
            hvacTypes.SelectedValueBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacType);

            hvacSystems.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.HvacSystems);
            hvacSystems.ItemTextBinding = Binding.Delegate<string, string>(t => vm.HVACsDic[t]);
            hvacSystems.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.HvacSystem);

            nameText.TextBinding.BindDataContext((OpsHVACsViewModel m) => m.Name);

            var economizerTitle = new Label() { Text = "Economizer:" };
            economizer.BindDataContext(c => c.DataStore, (OpsHVACsViewModel m) => m.Economizers);
            economizer.SelectedKeyBinding.BindDataContext((OpsHVACsViewModel m) => m.Economizer);
            economizer.BindDataContext(c => c.Enabled, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizer.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);
            economizerTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.EconomizerVisable);

            var sensibleTitle = new Label() { Text = "Sensible Heat Recovery:" };
            sensibleAutosize.BindDataContext(c => c.Checked, (OpsHVACsViewModel m) => m.SensibleHRAutosized);
            sensibleAutosize.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
            sensible.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.SensibleHR);
            sensible.BindDataContext(c => c.Enabled, (OpsHVACsViewModel m) => m.SensibleHRInputEnabled);
            sensible.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
            sensibleTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.SensibleHRVisable);
      

            var latentTitle = new Label() { Text = "Latent Heat Recovery:" };
            latentAutosize.BindDataContext(c => c.Checked, (OpsHVACsViewModel m) => m.LatentHRAutosized);
            latentAutosize.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);
            latent.BindDataContext(c => c.Value, (OpsHVACsViewModel m) => m.LatentHR);
            latent.BindDataContext(c => c.Enabled, (OpsHVACsViewModel m) => m.LatentHRInputEnabled);
            latent.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);
            latentTitle.BindDataContext(c => c.Visible, (OpsHVACsViewModel m) => m.LatentHRVisable);

            layout.AddRow("HVAC Groups:");
            layout.AddRow(hvacGroups);
            layout.AddRow("HVAC Types:");
            layout.AddRow(hvacTypes);
            layout.AddRow("HVAC Systems:");
            layout.AddRow(hvacSystems);
            layout.AddRow(null);

            layout.AddRow("Name:");
            layout.AddRow(nameText);
            layout.AddRow("Vintage:");
            layout.AddRow(year);
            layout.AddRow(economizerTitle);
            layout.AddRow(economizer);
            layout.AddSeparateRow(sensibleTitle, null, sensibleAutosize);
            layout.AddRow(sensible);
            layout.AddSeparateRow(latentTitle, null, latentAutosize);
            layout.AddRow(latent);
            //layout.AddSeparateRow(cloneBtn);

            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => {
                var obj = vm.GreateHvac();
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
