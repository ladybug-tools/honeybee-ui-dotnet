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
        private ModelRadianceProperties ModelRadianceProperties { get; set; }
        public Dialog_ModifierSetManager(ModelRadianceProperties libSource, List<ModifierSetAbridged> modifierSets)
        {
            this.ModelRadianceProperties = libSource;

            Padding = new Padding(5);
            Resizable = true;
            Title = "ModifierSet Manager - Honeybee";
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

            layout.AddSeparateRow("Construction Sets:", null, addNew, duplicate, edit, remove);

            var gd = GenGridView(modifierSets);
            gd.Height = 250;
            layout.AddRow(gd);


            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) =>
            {
                var d = gd.DataStore.OfType<ModifierSetAbridged>().ToList();
                Close(d);
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, OKButton, AbortButton, null);


            addNew.Click += (s, e) =>
            {
                var dialog = new Honeybee.UI.Dialog_ModifierSet(this.ModelRadianceProperties, null);
                var dialog_rc = dialog.ShowModal(this);

                if (dialog_rc == null) return;
                var d = gd.DataStore.OfType<ModifierSetAbridged>().ToList();
                d.Add(dialog_rc);
                gd.DataStore = d;
            };

            duplicate.Click += (s, e) =>
            {
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
            };

            Action editAction = () => {
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
            };
            edit.Click += (s, e) =>
            {
                editAction();
            };
            remove.Click += (s, e) =>
            {
                var selected = gd.SelectedItem as ModifierSetAbridged;
                if (selected == null)
                {
                    MessageBox.Show(this, "Nothing is selected to edit!");
                    return;
                }

                var index = gd.SelectedRow;
                if (selected.Identifier.StartsWith("Generic_"))
                {
                    MessageBox.Show(this, $"{selected.DisplayName ?? selected.Identifier } cannot be removed, because it is Honeybee default modifier set.");
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


        private GridView GenGridView(IEnumerable<object> items)
        {
            var gd = new GridView() { DataStore = items };
  
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<ModifierSetAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            return gd;
        }





    }
}
