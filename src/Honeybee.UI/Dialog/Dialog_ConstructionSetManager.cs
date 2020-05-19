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
    public class Dialog_ConstructionSetManager : Dialog<HB.ConstructionSetAbridged>
    {
     
        public Dialog_ConstructionSetManager(HB.Model model)
        {
            try
            {
                var md = model;
                var constrcutionSetsInModel = md.Properties.Energy.ConstructionSets.Where(_ => _.Obj is ConstructionSetAbridged);


                Padding = new Padding(5);
                Resizable = true;
                Title = "ConstructionSets - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 300);
                this.Icon = DialogHelper.HoneybeeIcon;

                var constrSets = constrcutionSetsInModel.Select(_=>_.Obj);


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
                DefaultButton.Click += (sender, e) => Close();

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                layout.AddSeparateRow(null, DefaultButton, AbortButton, null);


                addNew.Click += (s, e) =>
                {
                    var id = Guid.NewGuid().ToString();
                    var newConstrucionSet = new ConstructionSetAbridged(id, $"New ConstructionSet {id.Substring(0, 5)}");
                    
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(newConstrucionSet);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.OfType<ConstructionSetAbridged>().ToList();
                        d.Add(dialog_rc);
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

                    var dup = DuplicateConstructionSet(gd.SelectedItem as HB.ConstructionSetAbridged);

                    dup.Identifier = id;
                    dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.OfType<ConstructionSetAbridged>().ToList();
                        d.Add(dialog_rc);
                        gd.DataStore = d;

                    }
                };
                edit.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var dup = DuplicateConstructionSet(gd.SelectedItem as HB.ConstructionSetAbridged);

                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(dup);
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
                remove.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.Energy.IConstruction;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var index = gd.SelectedRow;
                    if (selected.Identifier == md.Properties.Energy.GlobalConstructionSet)
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
                    var doubleClk = e.Item;
                    if (doubleClk is HB.ConstructionSetAbridged selectedObj)
                    {
                        var dup = DuplicateConstructionSet(selectedObj);
                        var dialog = new Honeybee.UI.Dialog_ConstructionSet(dup);
                        var dialog_rc = dialog.ShowModal(this);

                        if (dialog_rc != null)
                        {
                            var index = gd.SelectedRow;
                            var newDataStore = gd.DataStore.OfType<ConstructionSetAbridged>().ToList();
                            newDataStore.RemoveAt(index);
                            newDataStore.Insert(index, dialog_rc);
                            gd.DataStore = newDataStore;


                        }
                    }

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

        private HB.ConstructionSetAbridged DuplicateConstructionSet(HB.ConstructionSetAbridged obj)
        {
            return ConstructionSetAbridged.FromJson((obj as ConstructionSetAbridged).ToJson());
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
