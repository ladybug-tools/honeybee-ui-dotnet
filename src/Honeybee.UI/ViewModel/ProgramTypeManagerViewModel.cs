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
    public class ProgramTypeManagerViewModel : ViewModelBase
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


        private DataStoreCollection<ProgramTypeViewData> _gridViewDataCollection = new DataStoreCollection<ProgramTypeViewData>();
        internal DataStoreCollection<ProgramTypeViewData> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        private List<ProgramTypeViewData> _userData { get; set; }
        private List<ProgramTypeViewData> _systemData { get; set; }
        private List<ProgramTypeViewData> _allData { get; set; }
        internal ProgramTypeViewData SelectedData { get; set; }

        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private Control _control;
    
        public ProgramTypeManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default)
        {
            _control = control;
            _modelEnergyProperties = libSource;

            this._userData = libSource.ProgramTypeList.OfType<ProgramTypeAbridged>().Select(_ => new ProgramTypeViewData(_)).ToList();
            this._systemData = new List<ProgramTypeViewData>();
            this._allData = _userData.Concat(_systemData).ToList();


            ResetDataCollection();

        }

        private void AddUserData(ProgramTypeAbridged item)
        {
            var newItem = CheckObjName(item);
            this._userData.Insert(0, new ProgramTypeViewData(newItem));
            this._allData = _userData.Concat(_systemData).ToList();
        }
        private void ReplaceUserData(ProgramTypeViewData oldObj, ProgramTypeAbridged newObj)
        {
            var newItem = CheckObjName(newObj);
            var index = _userData.IndexOf(oldObj);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ProgramTypeViewData(newItem));
            this._allData = _userData.Concat(_systemData).ToList();
        }
        private void DeleteUserData(ProgramTypeViewData item)
        {
            this._userData.Remove(item);
            this._allData = _userData.Concat(_systemData).ToList();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.ProgramType);
            this._modelEnergyProperties.ProgramTypes.Clear();
            this._modelEnergyProperties.AddProgramTypes(newItems);
        }

        public List<HB.Energy.IProgramtype> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Energy.IProgramtype>();

            if (selectedOnly)
            {
                var d = SelectedData;
                if (d == null)
                {
                    MessageBox.Show(_control, "Nothing is selected!");
                    return null;
                }
                //else if (!this._userData.Contains(d))
                //{
                //    // user selected an item from system library, now add it to model EnergyProperties
                //    this._modelEnergyProperties.AddProgramType(d.ProgramType);
                //}

                itemsToReturn.Add(d.ProgramType);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.ProgramTypeList.ToList();
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

        internal void SortList(Func<ProgramTypeViewData, string> sortFunc, bool isNumber, bool descend = false)
        {
            var c = new StringComparer(isNumber);
            var newOrder = descend ? _allData.OrderByDescending(sortFunc, c) : _allData.OrderBy(sortFunc, c);
            _allData = newOrder.ToList();
            ResetDataCollection();
        }

        internal ProgramTypeAbridged CheckObjName(ProgramTypeAbridged obj)
        {
            var name = obj.DisplayName ?? obj.Identifier;
            obj = obj.DuplicateProgramTypeAbridged();
            if (_allData.Any(_=>_.Name == name))
            {
                name = $"{name} {Guid.NewGuid().ToString().Substring(0, 5)}";
                MessageBox.Show(_control, $"Name [{obj.DisplayName}] is conflicting with an existing item, and now it is changed to [{name}].");
            }
            obj.Identifier = name;
            obj.DisplayName = name;
            return obj;
        }


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var dialog = new Honeybee.UI.Dialog_OpsProgramTypes(this._modelEnergyProperties);
            var dialog_rc = dialog.ShowModal(_control);

            var type = dialog_rc.programType;
            var sches = dialog_rc.schedules;
            if (type != null)
            {
                // add schedules
                var existingScheduleIds = this._modelEnergyProperties.ScheduleList.Select(_ => _.Identifier);
                foreach (var sch in sches)
                {
                    if (existingScheduleIds.Any(_ => _ == sch.Identifier))
                        continue;
                    this._modelEnergyProperties.AddSchedule(sch);
                }

                this._modelEnergyProperties.AddProgramType(type);

                AddUserData(type);
                ResetDataCollection();
            }
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to duplicate!");
                return;
            }
           
            var dup = selected.ProgramType.Duplicate() as HB.ProgramTypeAbridged;
            var name = $"{dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;

            var dialog = new Honeybee.UI.Dialog_ProgramType(this._modelEnergyProperties, dup);
            var dialog_rc = dialog.ShowModal(_control);
          
            if (dialog_rc != null)
            {
                AddUserData(dialog_rc);
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

            if (selected.Locked)
            {
                MessageBox.Show(_control, "You cannot edit an item of system library! Try to duplicate it first!");
                return;
            }

            var selectedObj = selected.ProgramType;
            var dup = selectedObj.Duplicate() as ProgramTypeAbridged;
        
            var dialog = new Honeybee.UI.Dialog_ProgramType(this._modelEnergyProperties, dup);
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



    internal class ProgramTypeViewData: IEquatable<ProgramTypeViewData>
    {
        public string Name { get; }
        public bool HasPeople { get; }
        public bool HasLighting { get; }
        public bool HasElecEquip { get; }
        public bool HasGasEquip { get; }
        public bool HasInfiltration { get; }
        public bool HasVentilation { get; }
        public bool HasSetpoint { get; }
        public bool HasServiceHotWater { get; }
        public string Source { get; }
        public bool Locked { get; }
        public HB.Energy.IProgramtype ProgramType { get; }
        public string SearchableText { get; }


        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.ProgramTypeList.Select(_ => _.Identifier);

        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds;

        public ProgramTypeViewData(HB.ProgramTypeAbridged c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            //this.CType = c.GetType().Name;
            this.ProgramType = c;
            this.HasPeople = c.People != null;
            this.HasLighting = c.Lighting != null;
            this.HasElecEquip = c.ElectricEquipment != null;
            this.HasGasEquip = c.GasEquipment != null;
            this.HasInfiltration = c.Infiltration != null;
            this.HasVentilation = c.Ventilation != null;
            this.HasSetpoint = c.Setpoint != null;
            this.HasServiceHotWater = c.ServiceHotWater != null;


            this.SearchableText = $"{this.Name}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            //else if (NRELLibraryIds.Contains(this.Name)) this.Source = "DoE NREL";
        }

        public bool Equals(ProgramTypeViewData other)
        {
            return other?.Name == this?.Name;
        }

    }

}
