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
                this.Set(() => _filterKey = value, nameof(_filterKey));
                ApplyFilter();
            }
        }

        private DataStoreCollection<ConstructionViewData> _gridViewDataCollection = new DataStoreCollection<ConstructionViewData>();
        internal DataStoreCollection<ConstructionViewData> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        private List<ConstructionViewData> _allData { get; set; }
        internal ConstructionViewData SelectedData { get; set; }

        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private Control _control;
    
        public ConstructionManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default)
        {
            ConstructionViewData.LibSource = libSource;
            _control = control;
            _modelEnergyProperties = libSource;

            this._allData = libSource.ConstructionList.Select(_ => new ConstructionViewData(_)).ToList();
            GridViewDataCollection = new DataStoreCollection<ConstructionViewData>(this._allData);

        }

        public void UpdateLibSource(List<HB.Energy.IConstruction> newItems)
        {
            this._modelEnergyProperties.Constructions.Clear();
            this._modelEnergyProperties.AddConstructions(newItems);
        }

        private void ResetDataCollection()
        {
            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(_allData);
        }

        internal void SortList(Func<ConstructionViewData, string> sortFunc, bool isNumber, bool descend = false)
        {
            var c = new StringComparer(isNumber);
            var newOrder = descend ? _allData.OrderByDescending(sortFunc, c) : _allData.OrderBy(sortFunc, c);
            _allData = newOrder.ToList();
            ResetDataCollection();
        }

        internal HB.Energy.IConstruction CheckObjName(HB.Energy.IConstruction obj)
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

        public ICommand AddOpaqueConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Opaque Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.OpaqueConstructionAbridged(name, new List<string>(), name);

            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                var newItem = CheckObjName(dialog_rc);
                _allData.Add(new ConstructionViewData(newItem));
                ResetDataCollection();
            }
        });



        public ICommand AddWindowConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Window Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.WindowConstructionAbridged(name, new List<string>(), name);

            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                var newItem = CheckObjName(dialog_rc);
                _allData.Add(new ConstructionViewData(newItem));
                ResetDataCollection();
            }
        });
        public ICommand AddShadeConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Shade Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.ShadeConstruction(name, name);

            var dialog = new Honeybee.UI.Dialog_Construction_Shade(newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                var newItem = CheckObjName(dialog_rc);
                _allData.Add(new ConstructionViewData(newItem));
                ResetDataCollection();
            }
        });

        public ICommand AddAirBoundaryConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New AirBoundary Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.AirBoundaryConstructionAbridged(name, name);

            var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                var newItem = CheckObjName(dialog_rc);
                _allData.Add(new ConstructionViewData(newItem));
                ResetDataCollection();
            }
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
            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, dup);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                var newItem = CheckObjName(dialog_rc);
                _allData.Add(new ConstructionViewData(newItem));
                ResetDataCollection();
            }
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }

            if (selected.IsSystemLibrary)
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
            var newItem = new ConstructionViewData(CheckObjName(dialog_rc));
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

            if (selected.IsSystemLibrary)
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
    }



    internal class ConstructionViewData: IEquatable<ConstructionViewData>
    {
        public string Name { get; }
        public string CType { get; }
        public string RValue { get; }
        public string RValueIP { get; }
        public string UFactor { get; }
        public string UFactorIP { get; }
        public bool IsSystemLibrary { get; }
        public HB.Energy.IConstruction Construction { get; }
        public string SearchableText { get; }

        public static HB.ModelEnergyProperties LibSource { get; set; }
        private static IEnumerable<string> SystemLibraryIds = HB.ModelEnergyProperties.Default.ConstructionList.Select(_ => _.Identifier);
        public ConstructionViewData(HB.Energy.IConstruction c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            this.CType = c.GetType().Name.Replace("Abridged", "").Replace("Construction", "");
            if (c is HB.Energy.IThermalConstruction tc)
            {
                tc.CalThermalValues(LibSource);
                this.RValue = Math.Round(tc.RValue, 5).ToString();
                this.UFactor = Math.Round(tc.UFactor, 5).ToString();

                this.RValueIP = Math.Round(tc.RValue * 5.678263337, 5).ToString();
                this.UFactorIP = Math.Round(tc.UFactor / 5.678263337, 5).ToString();
            }
            this.Construction = c;

            this.SearchableText = $"{this.Name}_{this.CType}";

            //check if system library
            this.IsSystemLibrary = SystemLibraryIds.Contains(c.Identifier);
        }

        public bool Equals(ConstructionViewData other)
        {
            return other?.Name == this?.Name;
        }
    }

}
