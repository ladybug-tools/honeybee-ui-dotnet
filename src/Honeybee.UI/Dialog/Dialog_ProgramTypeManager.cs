using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ProgramTypeManager : Dialog<List<HB.Energy.IProgramtype>>
    {
     
        public Dialog_ProgramTypeManager(List<HB.ProgramTypeAbridged> programTypes)
        {
            try
            {
                //var md = model;
                var pTypes = programTypes;

                Padding = new Padding(5);
                Resizable = true;
                Title = "Program Type Manager - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(900, 400);
                this.Icon = DialogHelper.HoneybeeIcon;

                var layout = new DynamicLayout();
                layout.DefaultSpacing = new Size(5, 5);
                layout.DefaultPadding = new Padding(10, 5);

                var addNew = new Button { Text = "Add" };
                //var import = new Button { Text = "Import" };
                var duplicate = new Button { Text = "Duplicate" };
                var edit = new Button { Text = "Edit" };
                var remove = new Button { Text = "Remove" };


                layout.AddSeparateRow("Program Types:", null, addNew, duplicate, edit, remove);
                var gd = GenProgramType_GV(pTypes);
                layout.AddSeparateRow(gd);

                addNew.Click += (s, e) =>
                {
                    var dialog = new Honeybee.UI.Dialog_OpsProgramTypes();
                    var dialog_rc = dialog.ShowModal(this);

                    var type = dialog_rc.programType;
                    var sches = dialog_rc.schedules;
                    if (type != null)
                    {
                        // add schedules
                        var existingScheduleIds = HB.Helper.EnergyLibrary.InModelEnergyProperties.Schedules.Select(_ => (_.Obj as HB.IDdEnergyBaseModel).Identifier);
                        foreach (var sch in sches)
                        {
                            if (existingScheduleIds.Any(_ => _ == sch.Identifier))
                                continue;
                            HB.Helper.EnergyLibrary.InModelEnergyProperties.Schedules.Add(sch);
                        }

                        // add program type
                        var d = gd.DataStore.Select(_ => _ as ProgramTypeAbridged).ToList();
                        d.Add(type);
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
                    var newPType = ProgramTypeAbridged.FromJson((gd.SelectedItem as ProgramTypeAbridged).ToJson());
                    newPType.Identifier = id;
                    newPType.DisplayName = string.IsNullOrEmpty( newPType.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}": $"{newPType.DisplayName}_dup";
                    var dialog = new Honeybee.UI.Dialog_ProgramType(newPType);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var d = gd.DataStore.Select(_=>_ as ProgramTypeAbridged).ToList();
                        d.Add(dialog_rc);
                        gd.DataStore = d;

                    }
                };
                Action editAction = () =>
                {
                    var selected = gd.SelectedItem;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }

                    var newPType = ProgramTypeAbridged.FromJson((selected as ProgramTypeAbridged).ToJson());
                    var dialog = new Honeybee.UI.Dialog_ProgramType(newPType);
                    var dialog_rc = dialog.ShowModal(this);
                    if (dialog_rc != null)
                    {
                        var index = gd.SelectedRow;
                        var newDataStore = gd.DataStore.Select(_ => _ as ProgramTypeAbridged).ToList();
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
                    var selected = gd.SelectedItem as ProgramTypeAbridged;
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

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => 
                {
                    var d = gd.DataStore.OfType<HB.Energy.IProgramtype>().ToList();
                    Close(d);
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                layout.AddSeparateRow(null);
                layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
                

                //Create layout
                Content = layout;


            }
            catch (Exception e)
            {

                throw e;
            }
            
            
        }

        private GridView GenProgramType_GV(IEnumerable<object> items)
        {
            var pType_GD = new GridView() { DataStore = items };
            pType_GD.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var peopleTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>( r => r.People == null ? "No Load" : r.People.DisplayName ?? r.People.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = peopleTB, HeaderText = "People", Sortable = true });

            var lightingTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.Lighting == null ? "No Load" : r.Lighting.DisplayName ?? r.Lighting.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = lightingTB, HeaderText = "Lighting", Sortable = true });

            var equipTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.ElectricEquipment == null ? "No Load" : r.ElectricEquipment.DisplayName ?? r.ElectricEquipment.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = equipTB, HeaderText = "ElecEquip", Sortable = true });

            var gasTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.GasEquipment == null ? "No Load": r.GasEquipment.DisplayName ?? r.GasEquipment.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = gasTB, HeaderText = "GasEquip", Sortable = true });

            var infTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.Infiltration == null ? "No Load": r.Infiltration.DisplayName ?? r.Infiltration.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = infTB, HeaderText = "Infiltration", Sortable = true });

            var ventTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.Ventilation == null ? "No Load" : r.Ventilation.DisplayName ?? r.Ventilation.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = ventTB, HeaderText = "Ventilation", Sortable = true });

            var setpointTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ProgramTypeAbridged, string>(r => r.Setpoint == null ? "No Load" : r.Setpoint.DisplayName ?? r.Setpoint.Identifier)
            };
            pType_GD.Columns.Add(new GridColumn { DataCell = setpointTB, HeaderText = "Setpoint", Sortable = true });



            return pType_GD;
        }





    }
}
