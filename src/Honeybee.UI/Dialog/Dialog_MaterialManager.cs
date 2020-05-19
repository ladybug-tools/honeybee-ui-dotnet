﻿using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
//using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_MaterialManager : Dialog<HB.Energy.IMaterial>
    {
     
        public Dialog_MaterialManager(HB.Model model)
        {
            try
            {
                var md = model;
                var materialsInModel = md.Properties.Energy.Materials.Select(_ => _.Obj as HB.Energy.IMaterial);


                Padding = new Padding(5);
                Resizable = true;
                Title = "Materials Library - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 300);
                this.Icon = DialogHelper.HoneybeeIcon;

                var materials = materialsInModel;


                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                var addNew = new Button { Text = "Add" };
                var duplicate = new Button { Text = "Duplicate" };
                var edit = new Button { Text = "Edit" };
                var remove = new Button { Text = "Remove" };

                layout.AddSeparateRow("Constructions:", null, addNew, duplicate, edit, remove);

                var gd = GenGridView(materials);
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
                    var newConstrucion = new OpaqueConstructionAbridged(id, new List<string>(), $"New Opaque Construction {id.Substring(0, 5)}");
                    
                    var dialog = new Honeybee.UI.Dialog_Construction(newConstrucion);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
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

                    var dup = DuplicateConstruction(gd.SelectedItem as HB.Energy.IConstruction);

                    dup.Identifier = id;
                    dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
                    var dialog = new Honeybee.UI.Dialog_Construction(dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
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

                    var dup = DuplicateConstruction(gd.SelectedItem as HB.Energy.IConstruction);

                    var dialog = new Honeybee.UI.Dialog_Construction(dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var index = gd.SelectedRow;
                        var newDataStore = gd.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
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
                    if (doubleClk is HB.Energy.IMaterial selectedObj)
                    {
                        //var dup = DuplicateConstruction(selectedPType);
                        var dialog = new Honeybee.UI.Dialog_Material(selectedObj);
                        var dialog_rc = dialog.ShowModal(this);

                        if (dialog_rc != null)
                        {
                            var index = gd.SelectedRow;
                            var newDataStore = gd.DataStore.Select(_ => _ as HB.Energy.IConstruction).ToList();
                            //newDataStore.RemoveAt(index);
                            //newDataStore.Insert(index, dialog_rc);
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

        private HB.Energy.IConstruction DuplicateConstruction(HB.Energy.IConstruction obj)
        {
            HB.Energy.IConstruction dup = null;
            if (obj is OpaqueConstructionAbridged opq)
            {
                dup = OpaqueConstructionAbridged.FromJson((obj as OpaqueConstructionAbridged).ToJson());
            }
            else if (obj is WindowConstructionAbridged win)
            {
                dup = WindowConstructionAbridged.FromJson((obj as WindowConstructionAbridged).ToJson());
            }
            else
            {
                throw new ArgumentException($"{obj.GetType().Name} cannot be duplicated yet");
            }

            return dup;
        }

        private GridView GenGridView(IEnumerable<object> items)
        {
            var gd = new GridView() { DataStore = items };
            gd.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IMaterial, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IMaterial, string>(r => r.GetType().Name)
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type" });
            return gd;
        }





    }
}
