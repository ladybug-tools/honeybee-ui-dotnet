using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class Dialog_ModifierManager : Dialog<List<HB.Radiance.IModifier>>
    {
 
        private bool _returnSelectedOnly;
        private GridView _gd { get; set; }
        private ModelRadianceProperties _modelRadianceProperties { get; set; }

        private Dialog_ModifierManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Modifiers Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(650, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        [Obsolete("This is deprecated", true)]
        public Dialog_ModifierManager(ModelRadianceProperties libSource, List<HB.ModifierBase> modifiers) : this()
        {
            this._modelRadianceProperties = libSource;
            Content = Init(modifiers);
        }

        public Dialog_ModifierManager(ModelRadianceProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this._modelRadianceProperties = libSource;
            var modifierSets = libSource.ModifierList;

            Content = Init(modifierSets);
        }
        private DynamicLayout Init(IEnumerable<HB.Radiance.IModifier> modifiers)
        {
            var layout = new DynamicLayout();
            layout.DefaultPadding = new Padding(10);
            layout.DefaultSpacing = new Size(5, 5);
            var constrcutionsInModel = modifiers;


            var addNew = new Button { Text = "Add" };
            addNew.Command = AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = RemoveCommand;

            layout.AddSeparateRow("Modifiers:", null, addNew, duplicate, edit, remove);

            this._gd = GenGridView(constrcutionsInModel);
            this._gd.Height = 250;
            layout.AddRow(this._gd);

            var gd = this._gd;


            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, OKButton, AbortButton, null);


            gd.CellDoubleClick += (s, e) => EditCommand.Execute(null);

            return layout;
        }



        private GridView GenGridView(IEnumerable<object> items)
        {
            items = items ?? new List<HB.Radiance.IModifier>();
            var gd = new GridView() { DataStore = items };
            gd.Height = 250;
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Radiance.IModifier, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            var typeTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Radiance.IModifier, string>(r => r.GetType().Name)
            };
            gd.Columns.Add(new GridColumn { DataCell = typeTB, HeaderText = "Type" });
            return gd;
        }

        #region AddModifier
        public ICommand AddPlasticCommand => new RelayCommand<HB.Radiance.IModifier>((obj) => {

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
        private void AddModifier(HB.Radiance.IModifier newModifier)
        {
            if (newModifier == null) return;
            var d = this._gd.DataStore.OfType<HB.Radiance.IModifier>().ToList();
            d.Add(newModifier);
            this._gd.DataStore = d;
        }


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
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
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as HB.Radiance.IModifier;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to duplicate!");
                return;
            }


            var dup = selected.Duplicate() as HB.Radiance.IModifier;
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
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as HB.Radiance.IModifier;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }
            var dup = selected.Duplicate() as HB.Radiance.IModifier;
            HB.Radiance.IModifier dialog_rc = null;
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
            var newDataStore = gd.DataStore.OfType<HB.Radiance.IModifier>().ToList();
            newDataStore.RemoveAt(index);
            newDataStore.Insert(index, dialog_rc);
            gd.DataStore = newDataStore;
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as HB.Radiance.IModifier;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            var res = MessageBox.Show(this, $"Are you sure you want to delete:\n {selected.DisplayName ?? selected.Identifier }", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                var newDataStore = gd.DataStore.Where(_ => _ != selected).ToList();
                gd.DataStore = newDataStore;
            }
        });

        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var gd = this._gd;

            var allItems = gd.DataStore.Select(_ => _ as HB.Radiance.IModifier).ToList();
            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as HB.Radiance.IModifier;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<HB.Radiance.IModifier>() { d };
            }

            this._modelRadianceProperties.Modifiers.Clear();
            this._modelRadianceProperties.AddModifiers(allItems);
            Close(itemsToReturn);
        });
        #endregion

    }
}
