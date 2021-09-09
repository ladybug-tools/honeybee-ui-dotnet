using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class ConstructionSetManagerViewModel : ManagerBaseViewModel<ConstructionSetViewData>
    {
        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
     
        public ConstructionSetManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default):base(control)
        {
            _modelEnergyProperties = libSource;

            this._userData = libSource.ConstructionSetList.OfType<ConstructionSetAbridged>().Select(_ => new ConstructionSetViewData(_)).ToList();
            this._systemData = HB.Helper.EnergyLibrary.UserConstructionSets.OfType<ConstructionSetAbridged>().Select(_ => new ConstructionSetViewData(_))
                .Concat(ModelEnergyProperties.Default.ConstructionSets.OfType<ConstructionSetAbridged>().Select(_ => new ConstructionSetViewData(_)))
                .ToList();
            this._allData = _userData.Concat(_systemData).Distinct(new ManagerItemComparer<ConstructionSetViewData>()).ToList();

         
            ResetDataCollection();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.ConstructionSet);
            this._modelEnergyProperties.ConstructionSets.Clear();
            this._modelEnergyProperties.AddConstructionSets(newItems);
        }

        public List<HB.Energy.IBuildingConstructionset> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Energy.IBuildingConstructionset>();

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
                //    this._modelEnergyProperties.AddSchedules(d.ConstructionSet);
                //}

                itemsToReturn.Add(d.ConstructionSet);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.ConstructionSetList.ToList();
            }
            return itemsToReturn;
        }


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var dialog = new Honeybee.UI.Dialog_OpsConstructionSet(this._modelEnergyProperties);
            var dialog_rc = dialog.ShowModal(_control);

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
                var newItem = CheckObjName(cSet);
                this._userData.Insert(0, new ConstructionSetViewData(newItem));
                this._allData = _userData.Concat(_systemData).ToList();
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


            var dup = selected.ConstructionSet.Duplicate() as ConstructionSetAbridged;
            var name = $"{dup.DisplayName ?? dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;
            var dialog = new Honeybee.UI.Dialog_ConstructionSet(this._modelEnergyProperties, dup);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjName(dialog_rc);
            this._userData.Insert(0, new ConstructionSetViewData(newItem));
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

            var dup = selected.ConstructionSet.Duplicate() as ConstructionSetAbridged;
            var dialog = new Honeybee.UI.Dialog_ConstructionSet(this._modelEnergyProperties, dup, selected.Locked);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjName(dialog_rc, selected.Name);
            var index = _userData.IndexOf(selected);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ConstructionSetViewData(newItem));
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



    internal class ConstructionSetViewData: ManagerViewDataBase
    {
        public bool HasWallSet { get; }
        public bool HasApertureSet { get; }
        public bool HasAirBoundaryConstruction { get; }
        public bool HasDoorSet { get; }
        public bool HasFloorSet { get; }
        public bool HasRoofCeilingSet { get; }
        public bool HasShadeSet { get; }
        public string Source { get; }
        public bool Locked { get; }
        public HB.ConstructionSetAbridged ConstructionSet { get; }

        public static List<ScheduleTypeLimit> TypeLimits;
        //private static IEnumerable<string> NRELLibraryIds =
        //    HB.Helper.EnergyLibrary.StandardsOpaqueConstructionSets.Keys
        //    .Concat(HB.Helper.EnergyLibrary.StandardsWindowConstructionSets.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.ConstructionSetList.Select(_ => _.Identifier);

        private static IEnumerable<string> UserLibIds = HB.Helper.EnergyLibrary.UserConstructionSets.Select(_ => _.Identifier);
        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(UserLibIds);

        public ConstructionSetViewData(HB.ConstructionSetAbridged c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
          
            this.ConstructionSet = c;
            this.HasWallSet = c.WallSet != null;
            this.HasApertureSet = c.ApertureSet != null;
            this.HasAirBoundaryConstruction = c.AirBoundaryConstruction != null;
            this.HasDoorSet = c.DoorSet != null;
            this.HasFloorSet = c.FloorSet != null;
            this.HasRoofCeilingSet = c.RoofCeilingSet != null;
            this.HasShadeSet = c.ShadeConstruction != null;


            this.SearchableText = $"{this.Name}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            else if (UserLibIds.Contains(c.Identifier)) this.Source = "User";
        }


    }

}
