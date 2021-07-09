using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_ConstructionSetManager : Dialog<List<HB.Energy.IBuildingConstructionset>>
    {
        private GridView _gd;
        private bool _returnSelectedOnly;
        private ModelEnergyProperties _modelEnergyProperties { get; set; }

        private Dialog_ConstructionSetManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"ConstructionSet Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(650, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }

        public Dialog_ConstructionSetManager(ref ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            this._modelEnergyProperties = libSource;

            var constrSets = libSource.ConstructionSetList;
            Content = Init(constrSets);
        }


        private DynamicLayout Init(IEnumerable<HB.Energy.IBuildingConstructionset> constrSets)
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

            var gd = GenGridView(constrSets);
            _gd = gd;
            gd.Height = 250;
            layout.AddRow(gd);


            gd.CellDoubleClick += (s, e) => EditCommand.Execute(null);

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => OkCommand.Execute(null);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
            return layout;

        }

        private GridView GenGridView(IEnumerable<object> items)
        {
            items = items ?? new List<HB.Energy.IBuildingConstructionset>();
            var gd = new GridView() { DataStore = items };
  
            var nameTB = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.ConstructionSetAbridged, string>(r => r.DisplayName ?? r.Identifier)
            };
            gd.Columns.Add(new GridColumn { DataCell = nameTB, HeaderText = "Name" });

            return gd;
        }



        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var dialog = new Honeybee.UI.Dialog_OpsConstructionSet(this._modelEnergyProperties);
            var dialog_rc = dialog.ShowModal(this);

            var cSet = dialog_rc.constructionSet;
            var contrs = dialog_rc.constructions;
            var mats = dialog_rc.materials;
            if (cSet != null)
            {

                var existingConstructionIds = this._modelEnergyProperties.Constructions.Select(_ => (_.Obj as HB.IDdEnergyBaseModel).Identifier);
                var existingMaterialIds = this._modelEnergyProperties.Materials.Select(_ => (_.Obj as HB.IDdEnergyBaseModel).Identifier);

                // add constructions
                var newConstrs = contrs.Where(_ => !existingConstructionIds.Any(c => c == _.Identifier)).ToList();
                this._modelEnergyProperties.AddConstructions(newConstrs);

                // add materials
                var newMats = mats.Where(_ => !existingMaterialIds.Any(m => m == _.Identifier)).ToList();
                this._modelEnergyProperties.AddMaterials(newMats);

                // add program type
                var d = gd.DataStore.Select(_ => _ as ConstructionSetAbridged).ToList();
                d.Add(cSet);
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

            var dup = (gd.SelectedItem as HB.ConstructionSetAbridged).DuplicateConstructionSetAbridged();

            dup.Identifier = id;
            dup.DisplayName = string.IsNullOrEmpty(dup.DisplayName) ? $"New Duplicate {id.Substring(0, 5)}" : $"{dup.DisplayName}_dup";
            var dialog = new Honeybee.UI.Dialog_ConstructionSet(this._modelEnergyProperties, dup);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var d = gd.DataStore.OfType<ConstructionSetAbridged>().ToList();
                d.Add(dialog_rc);
                gd.DataStore = d;

            }
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as HB.ConstructionSetAbridged;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            var dup = (gd.SelectedItem as HB.ConstructionSetAbridged).DuplicateConstructionSetAbridged();

            var dialog = new Honeybee.UI.Dialog_ConstructionSet(this._modelEnergyProperties, dup);
            var dialog_rc = dialog.ShowModal(this);
            if (dialog_rc != null)
            {
                var index = gd.SelectedRow;
                var newDataStore = gd.DataStore.OfType<ConstructionSetAbridged>().ToList();
                newDataStore.RemoveAt(index);
                newDataStore.Insert(index, dialog_rc);
                gd.DataStore = newDataStore;

            }
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var gd = this._gd;
            var selected = gd.SelectedItem as HB.Energy.IBuildingConstructionset;
            if (selected == null)
            {
                MessageBox.Show(this, "Nothing is selected to edit!");
                return;
            }

            if (selected.Identifier.Equals("Default Generic Construction Set"))
            {
                MessageBox.Show(this, $"{selected.DisplayName ?? selected.Identifier } cannot be removed, because it is set to default global construction set.");
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
            var allItems = gd.DataStore.Select(_ => _ as HB.Energy.IBuildingConstructionset).ToList();
            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = gd.SelectedItem as HB.Energy.IBuildingConstructionset;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<HB.Energy.IBuildingConstructionset>() { d };
            }

            this._modelEnergyProperties.ConstructionSets.Clear();
            this._modelEnergyProperties.AddConstructionSets(allItems);
            Close(itemsToReturn);
        });


    }
}
