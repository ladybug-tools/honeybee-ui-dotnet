using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class ModifierSetManagerViewModel : ManagerBaseViewModel<ModifierSetViewData>
    {
        private HB.ModelRadianceProperties _modelRadianceProperties { get; set; }
     
        public ModifierSetManagerViewModel(HB.ModelRadianceProperties libSource, Control control = default):base(control)
        {
            _modelRadianceProperties = libSource;

            this._userData = libSource.ModifierSetList.OfType<ModifierSetAbridged>().Select(_ => new ModifierSetViewData(_)).ToList();
            //TODO: wait until a ModifierSet library is added
            this._systemData = new List<ModifierSetViewData>();
            this._allData = _userData.Concat(_systemData).ToList();

         
            ResetDataCollection();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.ModifierSet);
            this._modelRadianceProperties.ModifierSets.Clear();
            this._modelRadianceProperties.AddModifierSets(newItems);
        }

        public List<HB.Radiance.IBuildingModifierSet> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Radiance.IBuildingModifierSet>();

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
                //    this._modelEnergyProperties.AddSchedules(d.ModifierSet);
                //}

                itemsToReturn.Add(d.ModifierSet);
            }
            else
            {
                itemsToReturn = this._modelRadianceProperties.ModifierSetList.ToList();
            }
            return itemsToReturn;
        }


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var name = $"New_ModifierSet_{Guid.NewGuid().ToString().Substring(0, 5)}";
            var newSet = new ModifierSetAbridged(name, displayName: name);
            var dialog = new Honeybee.UI.Dialog_ModifierSet(this._modelRadianceProperties, newSet);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjName(dialog_rc);
            this._userData.Insert(0, new ModifierSetViewData(newItem));
            this._allData = _userData.Concat(_systemData).ToList();
            ResetDataCollection();
        });


        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to duplicate!");
                return;
            }


            var dup = selected.ModifierSet.Duplicate() as ModifierSetAbridged;
            var name = $"{dup.DisplayName ?? dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;
            var dialog = new Honeybee.UI.Dialog_ModifierSet(this._modelRadianceProperties, dup);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjName(dialog_rc);
            this._userData.Insert(0, new ModifierSetViewData(newItem));
            this._allData = _userData.Concat(_systemData).ToList();
            ResetDataCollection();

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

            var dup = selected.ModifierSet.Duplicate() as ModifierSetAbridged;
            var dialog = new Honeybee.UI.Dialog_ModifierSet(this._modelRadianceProperties, dup, selected.Locked);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjName(dialog_rc, selected.Name);
            var index = _userData.IndexOf(selected);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ModifierSetViewData(newItem));
            this._allData = _userData.Concat(_systemData).ToList();
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
                this._userData.Remove(selected);
                this._allData = _userData.Concat(_systemData).ToList();
                ResetDataCollection();
            }
        });

    }



    internal class ModifierSetViewData: ManagerViewDataBase
    {
        public bool HasWallSet { get; }
        public bool HasApertureSet { get; }
        public bool HasAirBoundaryModifier { get; }
        public bool HasDoorSet { get; }
        public bool HasFloorSet { get; }
        public bool HasRoofCeilingSet { get; }
        public bool HasShadeSet { get; }
        public string Source { get; }
        public bool Locked { get; }
        public HB.ModifierSetAbridged ModifierSet { get; }

        public static List<ScheduleTypeLimit> TypeLimits;
        //private static IEnumerable<string> NRELLibraryIds =
        //    HB.Helper.EnergyLibrary.StandardsOpaqueModifierSets.Keys
        //    .Concat(HB.Helper.EnergyLibrary.StandardsWindowModifierSets.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelRadianceProperties.Default.ModifierSetList.Select(_ => _.Identifier);

        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds;

        public ModifierSetViewData(HB.ModifierSetAbridged c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
          
            this.ModifierSet = c;
            this.HasWallSet = c.WallSet != null;
            this.HasApertureSet = c.ApertureSet != null;
            this.HasAirBoundaryModifier = c.AirBoundaryModifier != null;
            this.HasDoorSet = c.DoorSet != null;
            this.HasFloorSet = c.FloorSet != null;
            this.HasRoofCeilingSet = c.RoofCeilingSet != null;
            this.HasShadeSet = c.ShadeSet != null;


            this.SearchableText = $"{this.Name}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            //else if (NRELLibraryIds.Contains(this.Name)) this.Source = "DoE NREL";
        }


    }

}
