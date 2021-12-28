using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class ProgramTypeManagerViewModel : ManagerBaseViewModel<ProgramTypeViewData>
    {
        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
    
        public ProgramTypeManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default):base(control)
        {
            _modelEnergyProperties = libSource;

            this._userData = libSource.ProgramTypeList.OfType<ProgramTypeAbridged>().Select(_ => new ProgramTypeViewData(_)).ToList();
            this._systemData = SystemEnergyLib.ProgramTypeList.OfType<ProgramTypeAbridged>().Select(_ => new ProgramTypeViewData(_)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(new ManagerItemComparer<ProgramTypeViewData>()).ToList();

            ResetDataCollection();

        }

        private void AddUserData(ProgramTypeAbridged item)
        {
            var newItem = CheckObjName(item);
            var newViewData = new ProgramTypeViewData(newItem);
            if (!this._userData.Contains(newViewData))
            {
                // add resources to model EnergyProperties
                var engLib = newViewData.CheckResources(SystemEnergyLib);
                this._modelEnergyProperties.MergeWith(engLib);
            }
            this._userData.Insert(0, newViewData);
            this._allData = _userData.Concat(_systemData).ToList();
        }
        private void ReplaceUserData(ProgramTypeViewData oldObj, ProgramTypeAbridged newObj)
        {
            var newItem = CheckObjName(newObj, oldObj.Name);
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
                else if (!this._userData.Contains(d))
                {
                    // user selected an item from system library, now add it to model EnergyProperties
                    var engLib = d.CheckResources(SystemEnergyLib);
                    this._modelEnergyProperties.MergeWith(engLib);
                }

                itemsToReturn.Add(d.ProgramType);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.ProgramTypeList.ToList();
            }
            return itemsToReturn;
        }

        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var dialog = new Honeybee.UI.Dialog_OpsProgramTypes(this._modelEnergyProperties);
            var dialog_rc = dialog.ShowModal(_control);

            var type = dialog_rc.programType?.DuplicateProgramTypeAbridged();
            var sches = dialog_rc.schedules?.Select(_=>_.DuplicateScheduleRulesetAbridged());
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

            //if (selected.Locked)
            //{
            //    MessageBox.Show(_control, "You cannot edit an item of system library! Try to duplicate it first!");
            //    return;
            //}

            var selectedObj = selected.ProgramType;
            var dup = selectedObj.Duplicate() as ProgramTypeAbridged;
        
            var dialog = new Honeybee.UI.Dialog_ProgramType(this._modelEnergyProperties, dup, selected.Locked);
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

        private static IEnumerable<string> NRELLibraryIds = ModelEnergyProperties.StandardLib.ProgramTypeList.Select(_ => _.Identifier);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.ProgramTypeList.Select(_ => _.Identifier);

        private static IEnumerable<string> UserLibIds = HB.Helper.EnergyLibrary.UserProgramtypes.Select(_ => _.Identifier);
        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(UserLibIds).Concat(NRELLibraryIds);

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

            eng.AddSchedules(sches);
            eng.AddScheduleTypeLimits(libSource.ScheduleTypeLimits);
            return eng.DuplicateModelEnergyProperties();

        }
    }

}
