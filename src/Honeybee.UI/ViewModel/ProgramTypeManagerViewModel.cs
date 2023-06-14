using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    internal class ProgramTypeManagerViewModel : ManagerBaseViewModel<ProgramTypeViewData>
    {
        private HB.ModelProperties _modelProperties { get; set; }
        private static ManagerItemComparer<ProgramTypeViewData> _viewDataComparer = new ManagerItemComparer<ProgramTypeViewData>();

        public ProgramTypeManagerViewModel(HB.ModelProperties libSource, Control control = default):base(control)
        {
            _modelProperties = libSource;

            this._userData = libSource.Energy.ProgramTypeList.OfType<ProgramTypeAbridged>().Select(_ => new ProgramTypeViewData(_)).ToList();
            this._systemData = SystemEnergyLib.ProgramTypeList.OfType<ProgramTypeAbridged>().Select(_ => new ProgramTypeViewData(_)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();

            ResetDataCollection();

        }

        private void AddUserData(ProgramTypeAbridged item)
        {
            var newItem = CheckObjID(item);
            var newViewData = new ProgramTypeViewData(newItem);
            if (!this._userData.Contains(newViewData))
            {
                // add resources to model EnergyProperties
                var engLib = newViewData.CheckResources(SystemEnergyLib);
                this._modelProperties.Energy.MergeWith(engLib);
            }
            this._userData.Insert(0, newViewData);
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void ReplaceUserData(ProgramTypeViewData oldObj, ProgramTypeAbridged newObj)
        {
            var newItem = CheckObjID(newObj, oldObj.Identifier);
            var index = _userData.IndexOf(oldObj);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ProgramTypeViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void DeleteUserData(ProgramTypeViewData item)
        {
            this._userData.Remove(item);
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.ProgramType);
            this._modelProperties.Energy.ProgramTypes.Clear();
            this._modelProperties.Energy.AddProgramTypes(newItems);
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
                else if (!this._userData.Contains(d))
                {
                    // user selected an item from system library, now add it to model EnergyProperties
                    var engLib = d.CheckResources(SystemEnergyLib);
                    this._modelProperties.Energy.MergeWith(engLib);
                }

                itemsToReturn.Add(d.ProgramType);
            }
            else
            {
                itemsToReturn = this._modelProperties.Energy.ProgramTypeList.ToList();
            }
            return itemsToReturn;
        }

        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var id = System.Guid.NewGuid().ToString().Substring(0, 5);
            var newItem = new ProgramTypeAbridged(id, $"New program type {id}");
            var lib = this._modelProperties;
            var dialog = new Honeybee.UI.Dialog_ProgramType(ref lib, newItem, editID: true);
            var dialog_rc = dialog.ShowModal(_control);
            //var newItem = new ProgramTypeAbridged($"{}");

            var type = dialog_rc?.DuplicateProgramTypeAbridged();
            if (type != null)
            {
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
            var name = $"{dup.DisplayName ?? dup.Identifier}_dup";
            dup.Identifier = System.Guid.NewGuid().ToString().Substring(0, 5);
            dup.DisplayName = name;

            var lib = this._modelProperties;
            var dialog = new Honeybee.UI.Dialog_ProgramType(ref lib, dup, editID: true);
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


            var selectedObj = selected.ProgramType;
            var dup = selectedObj.Duplicate() as ProgramTypeAbridged;

            var lib = this._modelProperties;
            var dialog = new Honeybee.UI.Dialog_ProgramType(ref lib, dup, selected.Locked);
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

        public RelayCommand ExportCommand => new RelayCommand(() =>
        {
            try
            {
                var inModelData = this._userData.Where(_ => _.IsInModelUserlib).Select(_ => _.ProgramType).ToList();
                if (!inModelData.Any())
                    throw new ArgumentException("There is no user's custom data found!");
                var container = new HB.ModelEnergyProperties();
                container.AddProgramTypes(inModelData);

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



    internal class ProgramTypeViewData: ManagerViewDataBase
    {
        public bool HasPeople { get; }
        public bool HasLighting { get; }
        public bool HasElecEquip { get; }
        public bool HasGasEquip { get; }
        public bool HasInfiltration { get; }
        public bool HasVentilation { get; }
        public bool HasSetpoint { get; }
        public bool HasServiceHotWater { get; }
        public string Source { get; } = "Model";
        public bool Locked { get; }
        public HB.Energy.IProgramtype ProgramType { get; }
        public bool IsInModelUserlib => this.Source == "Model";

        private static IEnumerable<string> NRELLibraryIds = ModelEnergyProperties.StandardLib.ProgramTypeList.Select(_ => _.Identifier);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.ProgramTypeList.Select(_ => _.Identifier);

        private static IEnumerable<string> UserLibIds = HB.Helper.EnergyLibrary.UserProgramtypes.Select(_ => _.Identifier);
        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(UserLibIds).Concat(NRELLibraryIds);

        public ProgramTypeViewData(HB.ProgramTypeAbridged c)
        {
            this.Identifier = c.Identifier;
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
            else if (NRELLibraryIds.Contains(c.Identifier)) this.Source = "DoE NREL";
            else if (UserLibIds.Contains(c.Identifier)) this.Source = "User";
        }

        internal HB.ModelEnergyProperties CheckResources(HB.ModelEnergyProperties libSource)
        {
            var eng = new ModelEnergyProperties();
            eng.AddProgramType(this.ProgramType);

            var names = (this.ProgramType as ProgramTypeAbridged).GetAllSchedules();
            var sches = names
                 .Select(_ => libSource.ScheduleList.FirstOrDefault(m => m.Identifier == _)).Where(_ => _ != null);
            var schTypes = libSource.ScheduleTypeLimits.Where(_ => _ != null);

            eng.AddSchedules(sches);
            eng.AddScheduleTypeLimits(schTypes);
            return eng.DuplicateModelEnergyProperties();

        }
    }

}
