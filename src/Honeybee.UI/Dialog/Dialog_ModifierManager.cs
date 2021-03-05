using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
//using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class Dialog_ModifierManager : Dialog<List<HB.ModifierBase>>
    {
        
        private GridView _gridView { get; set; }

        public Dialog_ModifierManager(List<HB.ModifierBase> modifiers)
        {
            try
            {
                //var md = model;
                var constrcutionsInModel = modifiers;


                Padding = new Padding(5);
                Resizable = true;
                Title = $"Modifiers Manager - {DialogHelper.PluginName}";
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

                layout.AddSeparateRow("Modifiers:", null, addNew, duplicate, edit, remove);

                this._gridView = GenGridView(constrcutionsInModel);
                this._gridView.Height = 250;
                layout.AddRow(this._gridView);

                var gd = this._gridView;



                var modifierDic = new Dictionary<string, ICommand>()
                {
                    { "Plastic", AddPlasticCommand},
                    { "Glass", AddGlassCommand},
                    { "Trans",AddTransCommand },
                    { "Metal", AddMetalCommand},
                    { "Mirror", AddMirrorCommand},
                    { "Glow",AddGlowCommand },
                    { "Light", AddLightCommand},
                    { "BSDF", AddBSDFCommand}
                };

                addNew.Click += (s, e) =>
                {
                    var contextMenu = new ContextMenu();

                    foreach (var item in modifierDic)
                    {
                        contextMenu.Items.Add(
                          new Eto.Forms.ButtonMenuItem()
                          {
                              Text = item.Key,
                              Command = item.Value
                          });
                    }
                    contextMenu.Show();
                };
                duplicate.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.ModifierBase;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to duplicate!");
                        return;
                    }

                   
                    var dup = selected.DuplicateModifierBase();
                    var id = Guid.NewGuid().ToString();
                    dup.Identifier = id;
                    dup.DisplayName = string.IsNullOrEmpty(selected.DisplayName) ? $"{selected.GetType()} {id.Substring(0, 5)}" : $"{selected.DisplayName}_dup";
                    switch (dup)
                    {
                        case Plastic obj:
                            AddPlasticCommand.Execute(obj);
                            break;
                        case Glass obj:
                            AddGlassCommand.Execute(obj);
                            break;
                        case Trans obj:
                            AddTransCommand.Execute(obj);
                            break;
                        case Metal obj:
                            AddMetalCommand.Execute(obj);
                            break;
                        case Mirror obj:
                            AddMirrorCommand.Execute(obj);
                            break;
                        case Glow obj:
                            AddGlowCommand.Execute(obj);
                            break;
                        case Light obj:
                            AddLightCommand.Execute(obj);
                            break;
                        case BSDF obj:
                            AddBSDFCommand.Execute(obj);
                            break;
                        default:
                            break;
                    }

                };

                Action editAction = () =>{
                    var selected = gd.SelectedItem as HB.ModifierBase;
                    if (selected == null)
                    {
                        MessageBox.Show(this, "Nothing is selected to edit!");
                        return;
                    }
                    var dup = selected.DuplicateModifierBase();
                    HB.ModifierBase dialog_rc = null;
                    switch (dup)
                    {
                        case Plastic obj:
                            dialog_rc = new Dialog_Modifier<Plastic>(obj).ShowModal(this);
                            break;
                        case Glass obj:
                            dialog_rc = new Dialog_Modifier<Glass>(obj).ShowModal(this);
                            break;
                        case Trans obj:
                            dialog_rc = new Dialog_Modifier<Trans>(obj).ShowModal(this);
                            break;
                        case Metal obj:
                            dialog_rc = new Dialog_Modifier<Metal>(obj).ShowModal(this);
                            break;
                        case Mirror obj:
                            dialog_rc = new Dialog_Modifier<Mirror>(obj).ShowModal(this);
                            break;
                        case Glow obj:
                            dialog_rc = new Dialog_Modifier<Glow>(obj).ShowModal(this);
                            break;
                        case Light obj:
                            dialog_rc = new Dialog_Modifier<Light>(obj).ShowModal(this);
                            break;
                        case BSDF obj:
                            dialog_rc = new Dialog_Modifier<BSDF>(obj).ShowModal(this);
                            break;
                        default:
                            break;
                    }
                   
                    if (dialog_rc == null) return;
                    var index = gd.SelectedRow;
                    var newDataStore = gd.DataStore.OfType<HB.ModifierBase>().ToList();
                    newDataStore.RemoveAt(index);
                    newDataStore.Insert(index, dialog_rc);
                    gd.DataStore = newDataStore;

                };
                edit.Click += (s, e) =>
                {
                    editAction();
                };
                remove.Click += (s, e) =>
                {
                    var selected = gd.SelectedItem as HB.ModifierBase;
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
                DefaultButton.Click += (sender, e) => {
                    var d = gd.DataStore.Select(_ => _ as HB.ModifierBase).ToList();
                    Close(d);
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();
                layout.AddSeparateRow(null, DefaultButton, AbortButton, null);

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
                Binding = Binding.Delegate<HB.ModifierBase, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ModifierBase, string>(r => r.GetType().Name)
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type" });
            return gd;
        }

        #region AddModifier
        public ICommand AddPlasticCommand => new RelayCommand<HB.ModifierBase>((obj) => {

            var id = Guid.NewGuid().ToString();
            var newModifier = obj as Plastic ?? new Plastic(id, $"Plastic {id.Substring(0, 5)}", new HB.Void() );

            var dialog = new Honeybee.UI.Dialog_Modifier<Plastic>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        public ICommand AddGlassCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newModifier = new Glass(id, $"Glass {id.Substring(0, 5)}", new HB.Void());

            var dialog = new Honeybee.UI.Dialog_Modifier<Glass>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        public ICommand AddBSDFCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newModifier = new BSDF(id, "Replace with your BSDF data", $"BSDF {id.Substring(0, 5)}", new HB.Void());;

            var dialog = new Honeybee.UI.Dialog_Modifier<BSDF>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        public ICommand AddGlowCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newModifier = new Glow(id, $"Glow {id.Substring(0, 5)}", new HB.Void()); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Glow>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        public ICommand AddLightCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newModifier = new Light(id, $"Light {id.Substring(0, 5)}", new HB.Void()); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Light>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        public ICommand AddTransCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newModifier = new Trans(id, $"Trans {id.Substring(0, 5)}", new HB.Void()); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Trans>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        public ICommand AddMetalCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newModifier = new Metal(id, $"Trans {id.Substring(0, 5)}", new HB.Void()); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Metal>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        public ICommand AddMirrorCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var newModifier = new Mirror(id, $"Mirror {id.Substring(0, 5)}", new HB.Void()); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Mirror>(newModifier);
            var dialog_rc = dialog.ShowModal(this);
            AddModifier(dialog_rc);
        });
        private void AddModifier(HB.ModifierBase newModifier)
        {
            if (newModifier == null) return;
            var d = this._gridView.DataStore.OfType<HB.ModifierBase>().ToList();
            d.Add(newModifier);
            this._gridView.DataStore = d;
        }

        #endregion

    }
}
