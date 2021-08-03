using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HoneybeeSchema;
using System.Text.RegularExpressions;

namespace Honeybee.UI
{
    public class MaterialManagerViewModel : ViewModelBase
    {
        private string _filterKey;
        public string FilterKey
        {
            get => _filterKey;
            set {
                this.Set(() => _filterKey = value, nameof(_filterKey));
                ApplyFilter();
            }
        }

        private DataStoreCollection<MaterialViewData> _gridViewDataCollection = new DataStoreCollection<MaterialViewData>();
        internal DataStoreCollection<MaterialViewData> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        private List<MaterialViewData> _allData { get; set; }
        internal MaterialViewData SelectedData { get; set; }

        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private Control _control;
    
        public MaterialManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default)
        {
            _control = control;
            _modelEnergyProperties = libSource;

            this._allData = libSource.MaterialList.Select(_ => new MaterialViewData(_))
                .Concat(HB.Helper.EnergyLibrary.StandardsOpaqueMaterials.Select(_ => new MaterialViewData(_.Value)))
                .Concat(HB.Helper.EnergyLibrary.StandardsWindowMaterials.Select(_ => new MaterialViewData(_.Value)))
                .ToList();
      
            GridViewDataCollection = new DataStoreCollection<MaterialViewData>(this._allData);

        }

        public void UpdateLibSource(List<HB.Energy.IMaterial> newItems)
        {
            this._modelEnergyProperties.Materials.Clear();
            this._modelEnergyProperties.AddMaterials(newItems);
        }

        private void ResetDataCollection()
        {
            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(_allData);
        }

        internal void SortList(Func<MaterialViewData, string> sortFunc, bool isNumber, bool descend = false)
        {
            var c = new StringComparer(isNumber);
            var newOrder = descend ? _allData.OrderByDescending(sortFunc, c) : _allData.OrderBy(sortFunc, c);
            _allData = newOrder.ToList();
            ResetDataCollection();
        }

        internal HB.Energy.IMaterial CheckObjName(HB.Energy.IMaterial obj)
        {
            var name = obj.DisplayName;
            
            if (_allData.Any(_=>_.Name == name))
            {
                name = $"{name} {Guid.NewGuid().ToString().Substring(0, 5)}";
                MessageBox.Show(_control, $"Name [{obj.DisplayName}] is conflicting with an existing item, and now it is changed to [{name}].");
            }
            obj.Identifier = name;
            obj.DisplayName = name;
            return obj;
        }


        private void ShowMaterialDialog(HB.Energy.IMaterial material)
        {
            var dialog = new Honeybee.UI.Dialog_Material(material);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                var newItem = CheckObjName(dialog_rc);
                _allData.Add(new MaterialViewData(newItem));
                ResetDataCollection();
            }

        }
        public ICommand AddNoMassMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Opaque (No Mass) {id.Substring(0, 5)}";
            // R10
            var newObj = new EnergyMaterialNoMass(name, 0.35, name);
            ShowMaterialDialog(newObj);

        });


        public ICommand AddOpaqueMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Opaque (Concrete) {id.Substring(0, 5)}";
            var newObj = new EnergyMaterial(name, 0.1016, 2.3085, 2322.0053, 831.4635, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddGlassMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Glass {id.Substring(0, 5)}";
            var newObj = new EnergyWindowMaterialGlazing(name, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowGapMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Window Gap {id.Substring(0, 5)}";
            var newObj = new EnergyWindowMaterialGas(name, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowGapCustomMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Window Gap (Custom) {id.Substring(0, 5)}";
            var newObj = new EnergyWindowMaterialGasCustom(name, 0, 0, 0, 0, 0, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowShadeMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Window Shade {id.Substring(0, 5)}";
            var newObj = new EnergyWindowMaterialShade(name, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowBlindMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Window Blind {id.Substring(0, 5)}";
            var newObj = new EnergyWindowMaterialBlind(name, displayName: name);
            ShowMaterialDialog(newObj);
        });


        public ICommand AddSimpleWindowMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Simple Window {id.Substring(0, 5)}";
            var newObj = new EnergyWindowMaterialSimpleGlazSys(name, 2, 0.55, displayName: name);
            ShowMaterialDialog(newObj);
        });

  
        public ICommand AddWindowGasMixtureMaterialShadeCommand => new RelayCommand(() => {
            MessageBox.Show(_control, $"Working in progress.");
            //var id = Guid.NewGuid().ToString();
            //var name = $"New Simple Window {id.Substring(0, 5)}";
            //var newObj = new EnergyWindowMaterialGasMixture(name, 2, 0.55, displayName: name);
            //ShowMaterialDialog(newObj);
        });


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var menuDic = new Dictionary<string, ICommand>()
            {
                { "Opaque (No Mass)", AddNoMassMaterialCommand},
                { "Opaque", AddOpaqueMaterialCommand},
                { "Separator_1", null},
                { "Simple Window",AddSimpleWindowMaterialCommand },
                { "Window Glass", AddGlassMaterialCommand},
                { "Window Gap", AddWindowGapMaterialCommand},
                { "Window Shade", AddWindowShadeMaterialCommand},
                { "Separator_2", null},
                { "Window Blind", AddWindowBlindMaterialCommand},
                { "Custom Gap", AddWindowGapCustomMaterialCommand},
                { "Gas Mixture (WIP)", AddWindowGasMixtureMaterialShadeCommand},
            };

            var contextMenu = new ContextMenu();
            foreach (var item in menuDic)
            {
                if (item.Key.StartsWith("Separator"))
                {
                    contextMenu.Items.Add(new Eto.Forms.SeparatorMenuItem());
                }
                else
                {
                    contextMenu.Items.Add(
                    new Eto.Forms.ButtonMenuItem()
                    {
                        Text = item.Key,
                        Command = item.Value
                    });
                }
              
            }
            contextMenu.Show();
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to duplicate!");
                return;
            }
           
            var dup = selected.Material.Duplicate() as HB.Energy.IMaterial;
            var name = $"{dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;

            ShowMaterialDialog(dup);

        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }

            if (selected.Locked)
            {
                MessageBox.Show(_control, "You cannot edit an item of system library! Try to duplicate it first!");
                return;
            }

            var selectedObj = selected.Material;
            var dup = selectedObj.Duplicate() as HB.Energy.IMaterial;
            var dialog = new Honeybee.UI.Dialog_Material(dup);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = new MaterialViewData(CheckObjName(dialog_rc));
            var index = _allData.IndexOf(selected);
            _allData.RemoveAt(index);
            _allData.Insert(index, newItem);

            ResetDataCollection();

        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var selected = SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }

            if (selected.Locked)
            {
                MessageBox.Show(_control, "You cannot remove an item of system library!");
                return;
            }

            var res = MessageBox.Show(_control, $"Are you sure you want to delete:\n {selected.Name}", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                _allData.Remove(selected);
                ResetDataCollection();
            }
        });

        public void ApplyFilter(bool forceRefresh = true)
        {
            var filter = this.FilterKey;
            var allData = this._allData;

            // list is not filtered
            if (string.IsNullOrWhiteSpace(filter))
            {
                if (!forceRefresh) return;
                // reset
                ResetDataCollection();
                return;
            }

            // do nothing if user only type in one key
            if (filter.Length <= 1) return;

            // filter
            var regexPatten = ".*" + filter.Replace(" ", "(.*)") + ".*";
            var filtered = allData.Where(_ => Regex.IsMatch(_.SearchableText, regexPatten, RegexOptions.IgnoreCase));
            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(filtered);
        }

        public void ChangeUnit(bool IPUnit)
        {
            this._allData = this._allData.Select(_ => new MaterialViewData( _.Material, IPUnit)).ToList();
            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(this._allData);
        }
    }



    internal class MaterialViewData: IEquatable<MaterialViewData>
    {
        public string Name { get; }
        public string CType { get; }
        public string RValue { get; }
        public string UValue { get; }
        public string UFactor { get; }
        public string Source { get; }
        public bool Locked { get; }
        public HB.Energy.IMaterial Material { get; }
        public string SearchableText { get; }

        //public static HB.ModelEnergyProperties LibSource { get; set; }
        private static IEnumerable<string> NRELLibraryIds =
            HB.Helper.EnergyLibrary.StandardsOpaqueMaterials.Keys
            .Concat(HB.Helper.EnergyLibrary.StandardsWindowMaterials.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.MaterialList.Select(_ => _.Identifier);

        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(NRELLibraryIds);

        public MaterialViewData(HB.Energy.IMaterial c, bool ShowIPUnit = false)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            this.CType = c.GetType().Name.Replace("EnergyWindowMaterial", "");
            if (c is HB.EnergyMaterial opc)
                this.CType = "Opaque";
            else if (c is HB.EnergyMaterialNoMass noMass)
                this.CType = "Opaque (No Mass)";

            if (c is HB.Energy.IMaterial tc)
            {
                this.RValue = ShowIPUnit? Math.Round(tc.RValue * 5.678263337, 5).ToString(): Math.Round(tc.RValue, 5).ToString();
                this.UValue = ShowIPUnit ? Math.Round(tc.UValue * 5.678263337, 5).ToString() : Math.Round(tc.UValue, 5).ToString();
            }
            
            if (c is HB.EnergyWindowMaterialSimpleGlazSys win)
            {
                this.UFactor = ShowIPUnit ? Math.Round(win.UFactor / 5.678263337, 5).ToString() : Math.Round(win.UFactor, 5).ToString();
            }
            this.Material = c;

            this.SearchableText = $"{this.Name}_{this.CType}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(this.Name)) this.Source = "LBT";
            else if (NRELLibraryIds.Contains(this.Name)) this.Source = "DoE NREL";
        }

        public bool Equals(MaterialViewData other)
        {
            return other?.Name == this?.Name;
        }

    }

}
