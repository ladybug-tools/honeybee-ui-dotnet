using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class MaterialManagerViewModel : ManagerBaseViewModel<MaterialViewData>
    {
        private bool _useIPUnit;
        public bool UseIPUnit
        {
            get => _useIPUnit;
            set
            {
                if (_useIPUnit != value)
                    ChangeUnit(value);
                _useIPUnit = value;

            }
        }

       
        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }

        private static ManagerItemComparer<MaterialViewData> _viewDataComparer = new ManagerItemComparer<MaterialViewData>();

        public MaterialManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default):base(control)
        {
            _modelEnergyProperties = libSource;

            this._userData = libSource.MaterialList.Select(_ => new MaterialViewData(_, ShowIPUnit: false)).ToList();
            this._systemData = SystemEnergyLib.MaterialList.Select(_ => new MaterialViewData(_, ShowIPUnit: false)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();


            ResetDataCollection();

        }

        private void AddUserData(HB.Energy.IMaterial item)
        {
            var newItem = CheckObjName(item);
            this._userData.Insert(0, new MaterialViewData(newItem, this.UseIPUnit));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void ReplaceUserData(MaterialViewData oldObj, HB.Energy.IMaterial newObj)
        {
            var newItem = CheckObjName(newObj, oldObj.Name);
            var index = _userData.IndexOf(oldObj);
            _userData.RemoveAt(index);
            _userData.Insert(index, new MaterialViewData(newItem, this.UseIPUnit));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void DeleteUserData(MaterialViewData item)
        {
            this._userData.Remove(item);
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.Material);
            this._modelEnergyProperties.Materials.Clear();
            this._modelEnergyProperties.AddMaterials(newItems);
        }

        public List<HB.Energy.IMaterial> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Energy.IMaterial>();

            if (selectedOnly)
            {
                var d = SelectedData;
                if (d == null)
                {
                    MessageBox.Show(_control, "Nothing is selected!");
                    return null;
                }
                else if (!this._userData.Contains(d))
                {
                    // user selected an item from system library, now add it to model EnergyProperties
                    this._modelEnergyProperties.AddMaterial(d.Material);
                }

                itemsToReturn.Add(d.Material);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.MaterialList.ToList();
            }
            return itemsToReturn;
        }

    

        private void ShowMaterialDialog(HB.Energy.IMaterial material)
        {
            var dialog = new Honeybee.UI.Dialog_Material(material);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                AddUserData(dialog_rc);
                ResetDataCollection();
            }

        }
        public ICommand AddNoMassMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Opaque (No Mass) {id}";
            // R10
            var newObj = new EnergyMaterialNoMass(id, 0.35, name);
            ShowMaterialDialog(newObj);

        });


        public ICommand AddOpaqueMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Opaque (Concrete) {id}";
            var newObj = new EnergyMaterial(id, 0.1016, 2.3085, 2322.0053, 831.4635, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddGlassMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Glass {id}";
            var newObj = new EnergyWindowMaterialGlazing(id, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowGapMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Window Gap {id}";
            var newObj = new EnergyWindowMaterialGas(id, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowGapCustomMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Window Gap (Custom) {id}";
            var newObj = new EnergyWindowMaterialGasCustom(id, 0, 0, 0, 0, 20, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowShadeMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Window Shade {id}";
            var newObj = new EnergyWindowMaterialShade(id, displayName: name);
            ShowMaterialDialog(newObj);
        });

        public ICommand AddWindowBlindMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Window Blind {id}";
            var newObj = new EnergyWindowMaterialBlind(id, displayName: name);
            ShowMaterialDialog(newObj);
        });


        public ICommand AddSimpleWindowMaterialCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Simple Window {id}";
            var newObj = new EnergyWindowMaterialSimpleGlazSys(id, 2, 0.55, displayName: name);
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
            dup.Identifier = Guid.NewGuid().ToString().Substring(0, 5);
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

            //if (selected.Locked)
            //{
            //    MessageBox.Show(_control, "You cannot edit an item of system library! Try to duplicate it first!");
            //    return;
            //}

            var selectedObj = selected.Material;
            var dup = selectedObj.Duplicate() as HB.Energy.IMaterial;
            var dialog = new Honeybee.UI.Dialog_Material(dup, selected.Locked);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;

            ReplaceUserData(selected, dialog_rc);
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
                DeleteUserData(selected);
                ResetDataCollection();
            }
        });

    

        private void ChangeUnit(bool IPUnit)
        {
            this._userData = this._userData.Select(_ => new MaterialViewData(_.Material, IPUnit)).ToList();
            this._systemData = this._systemData.Select(_ => new MaterialViewData(_.Material, IPUnit)).ToList();
            this._allData = _userData.Concat(_systemData).ToList();
            ResetDataCollection();
        }
    }

    internal class MaterialViewData: ManagerViewDataBase
    {
        public string CType { get; }
        public string RValue { get; }
        public string UValue { get; }
        public string UFactor { get; }
        public string TSolar { get; }
        public string TVis { get; }
        public string SHGC { get; }
        public string Source { get; } = "Model";
        public bool Locked { get; }
        public HB.Energy.IMaterial Material { get; }

        private static IEnumerable<string> NRELLibraryIds =
            HB.Helper.EnergyLibrary.StandardsOpaqueMaterials.Keys
            .Concat(HB.Helper.EnergyLibrary.StandardsWindowMaterials.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.MaterialList.Select(_ => _.Identifier);

        private static IEnumerable<string> UserLibIds = HB.Helper.EnergyLibrary.UserMaterials.Select(_ => _.Identifier);

        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(NRELLibraryIds).Concat(UserLibIds);

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
                var r = ShowIPUnit? Math.Round(tc.RValue * 5.678263337, 5): Math.Round(tc.RValue, 5);
                var u = ShowIPUnit ? Math.Round(tc.UValue / 5.678263337, 5) : Math.Round(tc.UValue, 5);

                this.RValue = r < 0 ? "Skylight only" : r.ToString();
                this.UValue = u < 0 ? "Skylight only" : u.ToString();
            }
            
            if (c is HB.EnergyWindowMaterialSimpleGlazSys win)
            {
                this.UFactor = ShowIPUnit ? Math.Round(win.UFactor / 5.678263337, 5).ToString() : Math.Round(win.UFactor, 5).ToString();
                this.SHGC = Math.Round(win.Shgc, 5).ToString();
                this.TSolar = Math.Round(win.SolarTransmittance, 5).ToString();
                this.TVis = Math.Round(win.Vt, 5).ToString();
            }
            else if (c is HB.EnergyWindowMaterialGlazing glz)
            {
                this.TSolar = Math.Round(glz.SolarTransmittance, 5).ToString();
                this.TVis = Math.Round(glz.VisibleTransmittance, 5).ToString();
            }
            this.Material = c;

            this.SearchableText = $"{this.Name}_{this.CType}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            else if (NRELLibraryIds.Contains(c.Identifier)) this.Source = "DoE NREL";
            else if (UserLibIds.Contains(c.Identifier)) this.Source = "User";
        }

    

    }

}
