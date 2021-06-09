using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_MaterialManager : Dialog<List<HB.Energy.IMaterial>>
    {
        private bool _returnSelectedOnly;
        private GridView _gd { get; set; }
        private ModelEnergyProperties _modelEnergyProperties { get; set; }

        private Dialog_MaterialManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Materials Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(650, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        [Obsolete("This is deprecated", true)]
        public Dialog_MaterialManager(ModelEnergyProperties libSource, List<HB.Energy.IMaterial> materials) :this()
        {
            this._modelEnergyProperties = libSource;
            Content = Init(materials);
        }

        public Dialog_MaterialManager(ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this._modelEnergyProperties = libSource;
            var materials = libSource.Materials
                  .OfType<HB.Energy.IMaterial>()
                  .ToList();

            Content = Init(materials);
        }

        public DynamicLayout Init(List<HB.Energy.IMaterial> materials)
        {
            var materialsInModel = materials;

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

            layout.AddSeparateRow("Materials:", null, addNew, duplicate, edit, remove);

            var gd = GenGridView(materialsInModel);
            gd.Height = 250;
            layout.AddRow(gd);
            this._gd = gd;

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



        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
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
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
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

        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
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
        });

        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var allItems = gd.DataStore.OfType<HB.Energy.IMaterial>().ToList();
            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as HB.Energy.IMaterial;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<HB.Energy.IMaterial>() { d };
            }

            this._modelEnergyProperties.Materials.Clear();
            this._modelEnergyProperties.AddMaterials(allItems);
            Close(itemsToReturn);
        });


    }
}