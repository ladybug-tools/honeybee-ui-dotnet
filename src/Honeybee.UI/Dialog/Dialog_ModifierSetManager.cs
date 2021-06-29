using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ModifierSetManager : Dialog<List<ModifierSetAbridged>>
    {
        private GridView _gd;
        private bool _returnSelectedOnly;
        private ModelRadianceProperties ModelRadianceProperties { get; set; }

        private Dialog_ModifierSetManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"ModifierSet Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(650, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        [Obsolete("This is deprecated", true)]
        public Dialog_ModifierSetManager(ModelRadianceProperties libSource, List<ModifierSetAbridged> modifierSets) : this()
        {
            this.ModelRadianceProperties = libSource;
            Content = Init(modifierSets);
        }

        public Dialog_ModifierSetManager(ModelRadianceProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this.ModelRadianceProperties = libSource;
            var modifierSets = libSource.ModifierSetList;

            Content = Init(modifierSets);
        }

        private DynamicLayout Init(IEnumerable<HoneybeeSchema.Radiance.IBuildingModifierSet> modifierSets)
        {
            var layout = new DynamicLayout();
            layout.DefaultPadding = new Padding(10);
            layout.DefaultSpacing = new Size(5, 5);


            var addNew = new Button { Text = "Add" };
            addNew.Command = AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = RemoveCommand;

            layout.AddSeparateRow("Construction Sets:", null, addNew, duplicate, edit, remove);

            var gd = GenGridView(modifierSets);
            _gd = gd;
            gd.Height = 250;
            layout.AddRow(gd);


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
            items = items ?? new List<HoneybeeSchema.Radiance.IBuildingModifierSet>();
            var gd = new GridView() { DataStore = items };
  
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HoneybeeSchema.Radiance.IBuildingModifierSet, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            return gd;
        }



        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var dialog = new Honeybee.UI.Dialog_ModifierSet(this.ModelRadianceProperties, null);
            var dialog_rc = dialog.ShowModal(this);

            if (dialog_rc == null) return;
            var d = gd.DataStore.OfType<ModifierSetAbridged>().ToList();
            d.Add(dialog_rc);
            gd.DataStore = d;
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            if (gd.SelectedItem == null)
            {
                MessageBox.Show(this, "Nothing is selected to duplicate!");
                return;
            }

            var id = Guid.NewGuid().ToString();

            var dup = (gd.SelectedItem as ModifierSetAbridged).DuplicateModifierSetAbridged();

            dup.Identifier = id;
            dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"ModifierSet {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
            var dialog = new Honeybee.UI.Dialog_ModifierSet(this.ModelRadianceProperties, dup);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc == null) return;
            var d = gd.DataStore.OfType<ModifierSetAbridged>().ToList();
            d.Add(dialog_rc);
            gd.DataStore = d;
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as ModifierSetAbridged;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            var dup = selected.DuplicateModifierSetAbridged();

            var dialog = new Honeybee.UI.Dialog_ModifierSet(this.ModelRadianceProperties, dup);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc == null) return;

            var index = gd.SelectedRow;
            var newDataStore = gd.DataStore.OfType<ModifierSetAbridged>().ToList();
            newDataStore.RemoveAt(index);
            newDataStore.Insert(index, dialog_rc);
            gd.DataStore = newDataStore;
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as ModifierSetAbridged;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            if (selected.Identifier.StartsWith("Generic_"))
            {
                MessageBox.Show(this, $"{selected.DisplayName ?? selected.Identifier } cannot be removed, because it is Honeybee default modifier set.");
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
     
            var allItems = gd.DataStore.OfType<ModifierSetAbridged>().ToList();
            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as ModifierSetAbridged;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<ModifierSetAbridged>() { d };
            }

            this.ModelRadianceProperties.ModifierSets.Clear();
            this.ModelRadianceProperties.AddModifierSets(allItems);
            Close(itemsToReturn);
        });


    }
}
