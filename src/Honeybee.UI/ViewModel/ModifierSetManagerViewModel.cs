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
        private static ManagerItemComparer<ModifierSetViewData> _viewDataComparer = new ManagerItemComparer<ModifierSetViewData>();

        public ModifierSetManagerViewModel(HB.ModelRadianceProperties libSource, Control control = default):base(control)
        {
            _modelRadianceProperties = libSource;

            this._userData = libSource.ModifierSetList.OfType<ModifierSetAbridged>().Select(_ => new ModifierSetViewData(_)).ToList();
            this._systemData = SystemRadianceLib.ModifierSetList.OfType<ModifierSetAbridged>().Select(_ => new ModifierSetViewData(_)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();

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
                else if (!this._userData.Contains(d))
                {
                    // user selected an item from system library, now add it to model EnergyProperties
                    var engLib = d.CheckResources(SystemRadianceLib);
                    this._modelRadianceProperties.MergeWith(engLib);
                }

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
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New_ModifierSet_{id}";
            var newSet = new ModifierSetAbridged(id, displayName: name);
            var lib = this._modelRadianceProperties;
            var dialog = new Honeybee.UI.Dialog_ModifierSet(ref lib, newSet, editID: true);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjID(dialog_rc);
            this._userData.Insert(0, new ModifierSetViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
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
            dup.Identifier = Guid.NewGuid().ToString().Substring(0, 5);
            dup.DisplayName = name;
            var lib = this._modelRadianceProperties;
            var dialog = new Honeybee.UI.Dialog_ModifierSet(ref lib, dup, editID: true);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjID(dialog_rc);
            this._userData.Insert(0, new ModifierSetViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
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
            var lib = this._modelRadianceProperties;
            var dialog = new Honeybee.UI.Dialog_ModifierSet(ref lib, dup, selected.Locked);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;
            var newItem = CheckObjID(dialog_rc, selected.Identifier);
            var index = _userData.IndexOf(selected);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ModifierSetViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
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
                this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
                ResetDataCollection();
            }
        });

        public RelayCommand ExportCommand => new RelayCommand(() =>
        {
            try
            {
                var inModelData = this._userData.Where(_ => _.IsInModelUserlib).Select(_ => _.ModifierSet).ToList();
                if (!inModelData.Any())
                    throw new ArgumentException("There is no user's custom data found!");
                var container = new HB.ModelRadianceProperties();
                container.AddModifierSets(inModelData);

                var json = container.ToJson();

                var fd = new Eto.Forms.SaveFileDialog();
                fd.FileName = $"custom_{System.Guid.NewGuid().ToString().Substring(0, 5)}";
                fd.Filters.Add(new FileFilter("JSON", new[] { "json" }));
                var rs = fd.ShowDialog(_control);
                if (rs != DialogResult.Ok)
                    return;
                var path = fd.FileName;
                path = System.IO.Path.ChangeExtension(path, "json");

                System.IO.File.WriteAllText(path, json);

                Dialog_Message.Show(_control, $"{inModelData.Count} custom data were exported!");
            }
            catch (Exception ex)
            {
                Dialog_Message.Show(_control, ex);
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
        public string Source { get; } = "Model";
        public bool Locked { get; }
        public HB.ModifierSetAbridged ModifierSet { get; }
        public bool IsInModelUserlib => this.Source == "Model";

        public static List<ScheduleTypeLimit> TypeLimits;
        //private static IEnumerable<string> NRELLibraryIds =
        //    HB.Helper.EnergyLibrary.StandardsOpaqueModifierSets.Keys
        //    .Concat(HB.Helper.EnergyLibrary.StandardsWindowModifierSets.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelRadianceProperties.Default.ModifierSetList.Select(_ => _.Identifier);

        private static IEnumerable<string> UserLibIds = HB.Helper.EnergyLibrary.UserModifierSets.Select(_ => _.Identifier);
        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(UserLibIds);

        public ModifierSetViewData(HB.ModifierSetAbridged c)
        {
            this.Identifier = c.Identifier;
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
            else if (UserLibIds.Contains(c.Identifier)) this.Source = "User";
        }

        internal HB.ModelRadianceProperties CheckResources(HB.ModelRadianceProperties libSource)
        {
            var eng = new ModelRadianceProperties();
            eng.AddModifierSet(this.ModifierSet);


            var names = this.ModifierSet.GetAllModifiers();
            var mods = names.Select(_ => libSource.ModifierList.FirstOrDefault(c => c.Identifier == _)).Where(_ => _ != null);
            eng.AddModifiers(mods);

            return eng.DuplicateModelRadianceProperties();
        }
    }

}
