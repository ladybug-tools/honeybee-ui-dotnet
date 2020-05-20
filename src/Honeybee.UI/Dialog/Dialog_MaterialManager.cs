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
    public class Dialog_MaterialManager : Dialog<List<HB.Energy.IMaterial>>
    {

        public Dialog_MaterialManager(List<HB.Energy.IMaterial> materials)
        {
            try
            {
                var materialsInModel = materials;


                Padding = new Padding(5);
                Resizable = true;
                Title = "Materials Manager - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(650, 300);
                this.Icon = DialogHelper.HoneybeeIcon;



                var layout = new DynamicLayout();
                layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                var addNew = new Button { Text = "Add" };
                var duplicate = new Button { Text = "Duplicate" };
                var edit = new Button { Text = "Edit" };
                var remove = new Button { Text = "Remove" };

                layout.AddSeparateRow("Materials:", null, addNew, duplicate, edit, remove);

                var gd = GenGridView(materialsInModel);
                gd.Height = 250;
                layout.AddRow(gd);


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) =>
                {
                    var d = gd.DataStore.OfType<HB.Energy.IMaterial>().ToList();
                    Close(d);
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                layout.AddSeparateRow(null, DefaultButton, AbortButton, null);


                Action editAction = () =>
                {
                    var selected = gd.SelectedItem;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var dup = (gd.SelectedItem as HB.Energy.IMaterial).Duplicate() as HB.Energy.IMaterial;

                    var dialog = new Honeybee.UI.Dialog_Material(dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var index = gd.SelectedRow;
                        var newDataStore = gd.DataStore.Select(_ => _ as HB.Energy.IMaterial).ToList();
                        newDataStore.RemoveAt(index);
                        newDataStore.Insert(index, dialog_rc);
                        gd.DataStore = newDataStore;

                    }
                };

                addNew.Click += (s, e) =>
                {
                    var id = Guid.NewGuid().ToString();
                    // R10
                    var newObj = new EnergyMaterialNoMass(id, 0.35, $"New No Mass Material {id.Substring(0, 5)}");

                    var dialog = new Honeybee.UI.Dialog_Material(newObj);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.Select(_ => _ as HB.Energy.IMaterial).ToList();
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

                    var dup = (gd.SelectedItem as HB.Energy.IMaterial).Duplicate() as HB.Energy.IMaterial;

                    dup.Identifier = id;
                    dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
                    var dialog = new Honeybee.UI.Dialog_Material(dup);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.Select(_ => _ as HB.Energy.IMaterial).ToList();
                        d.Add(dialog_rc);
                        gd.DataStore = d;

                    }
                };
                edit.Click += (s, e) =>
                {
                    editAction();
                };
                remove.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.Energy.IMaterial;
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