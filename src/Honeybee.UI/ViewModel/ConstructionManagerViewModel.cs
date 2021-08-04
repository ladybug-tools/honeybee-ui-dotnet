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
    public class ConstructionManagerViewModel : ViewModelBase
    {
        private string _filterKey;
        public string FilterKey
        {
            get => _filterKey;
            set {
                this.Set(() => _filterKey = value, nameof(FilterKey));
                ApplyFilter();
                this.Counts = this.GridViewDataCollection.Count.ToString();
            }
        }

        private string _counts;
        public string Counts
        {
            get => $"Count: {_counts}";
            set => this.Set(() => _counts = value, nameof(Counts));
        }

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

        private DataStoreCollection<ConstructionViewData> _gridViewDataCollection = new DataStoreCollection<ConstructionViewData>();
        internal DataStoreCollection<ConstructionViewData> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        private List<ConstructionViewData> _userData { get; set; }
        private List<ConstructionViewData> _systemData { get; set; }
        private List<ConstructionViewData> _allData { get; set; }
        internal ConstructionViewData SelectedData { get; set; }

        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private Control _control;
    
        public ConstructionManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default)
        {
            
            _control = control;
            _modelEnergyProperties = libSource;

            // for calculating R/U values 
            ConstructionViewData.LibSource = libSource.DuplicateModelEnergyProperties();
            ConstructionViewData.LibSource.AddMaterials(HB.Helper.EnergyLibrary.StandardsOpaqueMaterials.Values);
            ConstructionViewData.LibSource.AddMaterials(HB.Helper.EnergyLibrary.StandardsWindowMaterials.Values);

            this._userData = libSource.ConstructionList.Select(_ => new ConstructionViewData(_, ShowIPUnit: false)).ToList();
            this._systemData = 
                HB.Helper.EnergyLibrary.StandardsOpaqueConstructions.Select(_ => new ConstructionViewData(_.Value, ShowIPUnit: false))
                .Concat(HB.Helper.EnergyLibrary.StandardsWindowConstructions.Select(_ => new ConstructionViewData(_.Value, ShowIPUnit: false)))
                .ToList();
            this._allData = _userData.Concat(_systemData).ToList();

            ResetDataCollection();
        }

        private void AddUserData(HB.Energy.IConstruction item)
        {
            var newItem = CheckObjName(item);
            this._userData.Insert(0, new ConstructionViewData(newItem, this.UseIPUnit));
            this._allData = _userData.Concat(_systemData).ToList();
        }
        private void ReplaceUserData(ConstructionViewData oldObj, HB.Energy.IConstruction newObj)
        {
            var newItem = CheckObjName(newObj);
            var index = _userData.IndexOf(oldObj);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ConstructionViewData(newItem, this.UseIPUnit));
            this._allData = _userData.Concat(_systemData).ToList();
        }
        private void DeleteUserData(ConstructionViewData item)
        {
            this._userData.Remove(item);
            this._allData = _userData.Concat(_systemData).ToList();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.Construction);
            this._modelEnergyProperties.Constructions.Clear();
            this._modelEnergyProperties.AddConstructions(newItems);
        }

        public List<HB.Energy.IConstruction> GetUserItems(bool selectedOnly) 
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Energy.IConstruction>();

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
                    var mats = d.GetMaterials();
                    this._modelEnergyProperties.AddMaterials(mats);
                    this._modelEnergyProperties.AddConstruction(d.Construction);
                }
               
                itemsToReturn.Add(d.Construction);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.ConstructionList.ToList();
            }
            return itemsToReturn;
        }

        private void ResetDataCollection()
        {
            if (!string.IsNullOrEmpty(this.FilterKey))
                this.FilterKey = string.Empty;

            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(_allData);
            this.Counts = this.GridViewDataCollection.Count.ToString();

        }

        internal void SortList(Func<ConstructionViewData, string> sortFunc, bool isNumber, bool descend = false)
        {
            var c = new StringComparer(isNumber);
            var newOrder = descend ? _allData.OrderByDescending(sortFunc, c) : _allData.OrderBy(sortFunc, c);
            this._allData = newOrder.ToList();
            ResetDataCollection();
        }

        internal HB.Energy.IConstruction CheckObjName(HB.Energy.IConstruction obj)
        {
            var name = obj.DisplayName ?? obj.Identifier;

            if (_allData.Any(_=>_.Name == name))
            {
                name = $"{name} {Guid.NewGuid().ToString().Substring(0, 5)}";
                MessageBox.Show(_control, $"Name [{obj.DisplayName}] is conflicting with an existing item, and now it is changed to [{name}].");
            }
            obj.Identifier = name;
            obj.DisplayName = name;
            return obj;
        }

        private void ShowConstructionDialog(HB.Energy.IConstruction c)
        {
            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, c);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                AddUserData(dialog_rc);
                ResetDataCollection();
            }

        }
      

        public ICommand AddOpaqueConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Opaque Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.OpaqueConstructionAbridged(name, new List<string>(), name);

            ShowConstructionDialog(newConstrucion);
        });



        public ICommand AddWindowConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Window Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.WindowConstructionAbridged(name, new List<string>(), name);

            ShowConstructionDialog(newConstrucion);
        });
        public ICommand AddShadeConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Shade Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.ShadeConstruction(name, name);

            ShowConstructionDialog(newConstrucion);
        });

        public ICommand AddAirBoundaryConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New AirBoundary Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.AirBoundaryConstructionAbridged(name, name);

            ShowConstructionDialog(newConstrucion);
        });

        public ICommand AddWindowConstructionShadeCommand => new RelayCommand(() => {
            //var id = Guid.NewGuid().ToString();
            //var name = $"New AirBoundary Construction {id.Substring(0, 5)}";
            //var newConstrucion = new HB.WindowConstructionShadeAbridged(name, name);

            //var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(newConstrucion);
            //var dialog_rc = dialog.ShowModal(_control);
            //if (dialog_rc != null)
            //{
            //    GridViewDataCollection.Add(new ConstructionViewData(dialog_rc));
            //}
        });


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var menuDic = new Dictionary<string, ICommand>()
            {
                { "Opaque", AddOpaqueConstructionCommand},
                { "Window", AddWindowConstructionCommand},
                { "Shade",AddShadeConstructionCommand },
                { "AirBoundary", AddAirBoundaryConstructionCommand}
            };

            var contextMenu = new ContextMenu();
            foreach (var item in menuDic)
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
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to duplicate!");
                return;
            }
           
            var dup = selected.Construction.Duplicate() as HB.Energy.IConstruction;
            var name = $"{dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;
            ShowConstructionDialog(dup);
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
                MessageBox.Show(_control, "You cannot edit an item of system library!");
                return;
            }

            var selectedObj = selected.Construction;
            HB.Energy.IConstruction dialog_rc;
            if (selectedObj is HB.ShadeConstruction shd)
            {
                var dup = shd.DuplicateShadeConstruction();
                var dialog = new Honeybee.UI.Dialog_Construction_Shade(dup);
                dialog_rc = dialog.ShowModal(_control);
            }
            else if (selectedObj is HB.AirBoundaryConstructionAbridged airBoundary)
            {
                var dup = airBoundary.DuplicateAirBoundaryConstructionAbridged();
                var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(dup);
                dialog_rc = dialog.ShowModal(_control);
            }
            else
            {
                // Opaque Construction or Window Construciton
                var dup = selectedObj.Duplicate() as HB.Energy.IConstruction;
                var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, dup);
                dialog_rc = dialog.ShowModal(_control);
            }

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

        private void ChangeUnit(bool IPUnit)
        {
            this._userData = this._userData.Select(_ => new ConstructionViewData(_.Construction, IPUnit)).ToList();
            this._systemData = this._systemData.Select(_ => new ConstructionViewData(_.Construction, IPUnit)).ToList();
            this._allData = _userData.Concat(_systemData).ToList();
            ResetDataCollection();
        }
    }



    internal class ConstructionViewData: IEquatable<ConstructionViewData>
    {
        public string Name { get; }
        public string CType { get; }
        public string RValue { get; }
        public string UValue { get; }
        public string UFactor { get; }
        public bool Locked { get; }
        public string Source { get; }
        public HB.Energy.IConstruction Construction { get; }
        public string SearchableText { get; }

        public static HB.ModelEnergyProperties LibSource { get; set; }

        private static IEnumerable<string> NRELLibraryIds =
         HB.Helper.EnergyLibrary.StandardsOpaqueConstructions.Keys
         .Concat(HB.Helper.EnergyLibrary.StandardsWindowConstructions.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.ConstructionList.Select(_ => _.Identifier);

        private static IEnumerable<string> SystemLibraryIds = LBTLibraryIds.Concat(NRELLibraryIds);

        public ConstructionViewData(HB.Energy.IConstruction c, bool ShowIPUnit)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            this.CType = c.GetType().Name.Replace("Abridged", "").Replace("Construction", "");
            if (c is HB.Energy.IThermalConstruction tc)
            {
                tc.CalThermalValues(LibSource);
                this.RValue = ShowIPUnit ? Math.Round(tc.RValue * 5.678263337, 5).ToString(): Math.Round(tc.RValue, 5).ToString();
                this.UFactor = ShowIPUnit ? Math.Round(tc.UFactor / 5.678263337, 5).ToString(): Math.Round(tc.UFactor, 5).ToString();
                this.UValue = ShowIPUnit ? Math.Round(tc.UValue / 5.678263337, 5).ToString() : Math.Round(tc.UValue, 5).ToString();
            }
            this.Construction = c;

            this.SearchableText = $"{this.Name}_{this.CType}";

            //check if system library
            this.Locked = SystemLibraryIds.Contains(c.Identifier);
            if (LBTLibraryIds.Contains(this.Name)) this.Source = "LBT";
            else if (NRELLibraryIds.Contains(this.Name)) this.Source = "DoE NREL";
        }

        public bool Equals(ConstructionViewData other)
        {
            return other?.Name == this?.Name;
        }

        public List<HB.Energy.IMaterial> GetMaterials()
        {
            var layers = new List<string>();
            if (this.Construction is HB.OpaqueConstructionAbridged obj)
            {
                layers =  obj.Materials;
            }
            else if (this.Construction is HB.WindowConstructionAbridged win)
            {
                layers = win.Materials;
            }

            var materials = new List<HB.Energy.IMaterial>();
            foreach (var layer in layers)
            {
                var m = LibSource.MaterialList.FirstOrDefault(_ => _.Identifier == layer);
                var dup = m.Duplicate() as HB.Energy.IMaterial;
                dup.DisplayName = m.DisplayName ?? m.Identifier;
                materials.Add(dup);
            }
            return materials;
        }
    }

}
