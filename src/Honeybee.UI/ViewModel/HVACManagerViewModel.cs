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
    public class HVACManagerViewModel : ViewModelBase
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


        private DataStoreCollection<HVACViewData> _gridViewDataCollection = new DataStoreCollection<HVACViewData>();
        internal DataStoreCollection<HVACViewData> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        private List<HVACViewData> _userData { get; set; }
        private List<HVACViewData> _systemData { get; set; }
        private List<HVACViewData> _allData { get; set; }
        internal HVACViewData SelectedData { get; set; }

        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private Control _control;
    
        public HVACManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default)
        {
            _control = control;
            _modelEnergyProperties = libSource;

            this._userData = libSource.HVACList.Select(_ => new HVACViewData(_)).ToList();
            this._systemData = new List<HVACViewData>();
            this._allData = _userData.Concat(_systemData).ToList();


            ResetDataCollection();

        }

        private void AddUserData(HB.Energy.IHvac item)
        {
            var newItem = CheckObjName(item);
            this._userData.Insert(0, new HVACViewData(newItem));
            this._allData = _userData.Concat(_systemData).ToList();
        }
        private void ReplaceUserData(HVACViewData oldObj, HB.Energy.IHvac newObj)
        {
            var newItem = CheckObjName(newObj);
            var index = _userData.IndexOf(oldObj);
            _userData.RemoveAt(index);
            _userData.Insert(index, new HVACViewData(newItem));
            this._allData = _userData.Concat(_systemData).ToList();
        }
        private void DeleteUserData(HVACViewData item)
        {
            this._userData.Remove(item);
            this._allData = _userData.Concat(_systemData).ToList();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.HVAC);
            this._modelEnergyProperties.Hvacs.Clear();
            this._modelEnergyProperties.AddHVACs(newItems);
        }

        public List<HB.Energy.IHvac> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Energy.IHvac>();

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
                    this._modelEnergyProperties.AddHVAC(d.HVAC);
                }

                itemsToReturn.Add(d.HVAC);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.HVACList.ToList();
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

        internal void SortList(Func<HVACViewData, string> sortFunc, bool isNumber, bool descend = false)
        {
            var c = new StringComparer(isNumber);
            var newOrder = descend ? _allData.OrderByDescending(sortFunc, c) : _allData.OrderBy(sortFunc, c);
            _allData = newOrder.ToList();
            ResetDataCollection();
        }

        internal HB.Energy.IHvac CheckObjName(HB.Energy.IHvac obj)
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


        private void ShowHVACDialog(HB.Energy.IHvac HVAC)
        {
            HB.Energy.IHvac dialog_rc = null;
            if (HVAC is IdealAirSystemAbridged obj)
            {
                var dialog = new Dialog_IdealAirLoad(obj);
                dialog_rc = dialog.ShowModal(_control);
            }
            else
            {
                var dialog = new Dialog_OpsHVACs(HVAC);
                dialog_rc = dialog.ShowModal(_control);
            }

            if (dialog_rc != null)
            {
                AddUserData(dialog_rc);
                ResetDataCollection();
            }

        }
    
 
        public ICommand AddIdealAirLoadCommand => new RelayCommand<HoneybeeSchema.IdealAirSystemAbridged>((obj) => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Ideal Air System {id.Substring(0, 5)}";
            obj = obj ?? new IdealAirSystemAbridged(name, displayName: name);
            ShowHVACDialog(obj);
        });
        public ICommand AddOpsHVACCommand => new RelayCommand<HoneybeeSchema.Energy.IHvac>((obj) => {
            ShowHVACDialog(obj);
        });

     
        public RelayCommand AddCommand => new RelayCommand(() =>
        {

            var contextMenu = new ContextMenu();

            var AddHvacsDic = new Dictionary<string, ICommand>()
                {
                    { "Ideal Air Load", AddIdealAirLoadCommand},
                    { "From OpenStudio library", AddOpsHVACCommand}
                };

            foreach (var item in AddHvacsDic)
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
           
            var dup = selected.HVAC.Duplicate() as HB.Energy.IHvac;
            var name = $"{dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;

            ShowHVACDialog(dup);

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

            var selectedObj = selected.HVAC;
            var dup = selectedObj.Duplicate() as HB.Energy.IHvac;
            HB.Energy.IHvac dialog_rc = null;
            if (dup is IdealAirSystemAbridged obj)
            {
                var dialog = new Dialog_IdealAirLoad(obj);
                dialog_rc = dialog.ShowModal(_control);
            }
            else
            {
                var dialog = new Dialog_OpsHVACs(dup);
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

    }



    internal class HVACViewData: IEquatable<HVACViewData>
    {
        public string Name { get; }
        public string CType { get; }
        public string Source { get; }
        public bool Locked { get; }
        public HB.Energy.IHvac HVAC { get; }
        public string SearchableText { get; }


        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.HVACList.Select(_ => _.Identifier);

        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds;

        public HVACViewData(HB.Energy.IHvac c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            var type = c.GetType().GetProperty("EquipmentType")?.GetValue(c)?.ToString() ?? c.GetType().Name;
            this.CType = OpsHVACsViewModel.HVACUserFriendlyNamesDic.TryGetValue(type, out var userFriendly) ? userFriendly : type;
            this.HVAC = c;

            this.SearchableText = $"{this.Name}_{this.CType}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            //else if (NRELLibraryIds.Contains(this.Name)) this.Source = "DoE NREL";
        }

        public bool Equals(HVACViewData other)
        {
            return other?.Name == this?.Name;
        }

    }

}
