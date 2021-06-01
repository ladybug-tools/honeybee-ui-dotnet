using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
//using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ConstructionSetSelector : Dialog<HB.Energy.IBuildingConstructionset>
    {
        private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_ConstructionSetSelector(ModelEnergyProperties libSource)
        {
            try
            {
                this.ModelEnergyProperties = libSource;
               
                Padding = new Padding(5);
                Resizable = true;
                Title = $"ConstructionSet Manager - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 300);
                this.Icon = DialogHelper.HoneybeeIcon;

                var constrSets = libSource.ConstructionSets
               .OfType<HoneybeeSchema.Energy.IBuildingConstructionset>()
               .ToList();

                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                var addNew = new Button { Text = "Add" };
                var duplicate = new Button { Text = "Duplicate" };
                var edit = new Button { Text = "Edit" };
                var remove = new Button { Text = "Remove" };

                layout.AddSeparateRow("Construction Sets:", null, addNew, duplicate, edit, remove);

                var gd = GenGridView(constrSets);
                gd.Height = 250;
                layout.AddRow(gd);
                

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => 
                {
                    var d = gd.SelectedItem as HB.Energy.IBuildingConstructionset;
                    if (d == null)
                    {
                        MessageBox.Show(this, "Nothing is selected!");
                        return;
                    }
                    Close(d);
                    
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                layout.AddSeparateRow(null, DefaultButton, AbortButton, null);


                addNew.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_OpsConstructionSet(this.ModelEnergyProperties);
                    var dialog_rc = dialog.ShowModal(this);

                    var cSet = dialog_rc.constructionSet;
                    var contrs = dialog_rc.constructions;
                    var mats = dialog_rc.materials;
                    if (cSet != null)
                    {
                        
                        var existingConstructionIds =  this.ModelEnergyProperties.Constructions.Select(_ => (_.Obj as HB.IDdEnergyBaseModel).Identifier);
                        var existingMaterialIds =  this.ModelEnergyProperties.Materials.Select(_ => (_.Obj as HB.IDdEnergyBaseModel).Identifier);

                        // add constructions
                        var newConstrs = contrs.Where(_ => !existingConstructionIds.Any(c => c == _.Identifier)).ToList();
                         this.ModelEnergyProperties.AddConstructions(newConstrs);

                        // add materials
                        var newMats = mats.Where(_ => !existingMaterialIds.Any(m => m == _.Identifier)).ToList();
                         this.ModelEnergyProperties.AddMaterials(newMats);

                        // add program type
                        var d = gd.DataStore.Select(_ => _ as ConstructionSetAbridged).ToList();
                        d.Add(cSet);
                        gd.DataStore = d;

                    }

                };
                duplicate.Click += (s, e) =>
                {
                    if (gd.SelectedItem == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to duplicate!");
                        return;
                    }

                    var id = Guid.NewGuid().ToString();

                    var dup = (gd.SelectedItem as HB.ConstructionSetAbridged).DuplicateConstructionSetAbridged();

                    dup.Identifier = id;
                    dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(this.ModelEnergyProperties, dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.OfType<ConstructionSetAbridged>().ToList();
                        d.Add(dialog_rc);
                        gd.DataStore = d;

                    }
                };

                Action editAction = () => {
                    var selected = gd.SelectedItem as HB.ConstructionSetAbridged;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var dup = (gd.SelectedItem as HB.ConstructionSetAbridged).DuplicateConstructionSetAbridged();

                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(this.ModelEnergyProperties, dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var index = gd.SelectedRow;
                        var newDataStore = gd.DataStore.OfType<ConstructionSetAbridged>().ToList();
                        newDataStore.RemoveAt(index);
                        newDataStore.Insert(index, dialog_rc);
                        gd.DataStore = newDataStore;

                    }
                };
                edit.Click += (s, e) =>
                {
                    editAction();
                };
                remove.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.Energy.IBuildingConstructionset;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var index = gd.SelectedRow;
                    if ( selected.Identifier.Equals("Default Generic Construction Set"))
                    {
                        MessageBox.Show(this, $"{selected.DisplayName ?? selected.Identifier } cannot be removed, because it is set to default global construction set.");
                        return;
                    }

                    var res = MessageBox.Show(this, $"Are you sure you want to delete:\n {selected.DisplayName ?? selected.Identifier }", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                    {
                        var newDataStore = gd.DataStore.ToList();
                        newDataStore.RemoveAt(index);
                        gd.DataStore = newDataStore;
                    }

                };

                gd.CellDoubleClick += (s, e) =>
                {
                    editAction();
                };

                //Create layout
                Content = layout;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
                //Rhino.RhinoApp.WriteLine(e.Message);
            }
            
            
        }


        private GridView GenGridView(IEnumerable<object> items)
        {
            var gd = new GridView() { DataStore = items };
  
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ConstructionSetAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            return gd;
        }





    }
}
