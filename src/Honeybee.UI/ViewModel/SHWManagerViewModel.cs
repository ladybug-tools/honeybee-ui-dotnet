using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class SHWManagerViewModel : ManagerBaseViewModel<SHWViewData>
    {
        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private static ManagerItemComparer<SHWViewData> _viewDataComparer = new ManagerItemComparer<SHWViewData>();

        public SHWManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default):base(control)
        {
            _modelEnergyProperties = libSource;

            this._userData = libSource.Shws.Select(_ => new SHWViewData(_)).ToList();
            this._systemData = SystemEnergyLib.Shws.Select(_ => new SHWViewData(_)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();

            ResetDataCollection();
        }

        private void AddUserData(HB.SHWSystem item)
        {
            var newItem = CheckObjName(item);
            var newDataView = new SHWViewData(newItem);
            if (!this._userData.Contains(newDataView))
            {
                // user selected an item from system library, now add it to model EnergyProperties
                this._modelEnergyProperties.AddSHW(newDataView.System);
            }
            this._userData.Insert(0, newDataView);
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void ReplaceUserData(SHWViewData oldObj, HB.SHWSystem newObj)
        {
            var newItem = CheckObjName(newObj, oldObj.Name);
            var index = _userData.IndexOf(oldObj);
            _userData.RemoveAt(index);
            _userData.Insert(index, new SHWViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void DeleteUserData(SHWViewData item)
        {
            this._userData.Remove(item);
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }

        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.System);
            this._modelEnergyProperties.Hvacs.Clear();
            this._modelEnergyProperties.AddSHWs(newItems);
        }

        public List<HB.SHWSystem> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.SHWSystem>();

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
                    this._modelEnergyProperties.AddSHW(d.System);
                }

                itemsToReturn.Add(d.System);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.Shws;
            }
            return itemsToReturn;
        }

    

        private void ShowHVACDialog(HB.SHWSystem system)
        {
            var dialog = new Dialog_SHW(system);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc != null)
            {
                AddUserData(dialog_rc);
                ResetDataCollection();
            }

        }
    
        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            ShowHVACDialog(null);
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to duplicate!");
                return;
            }
           
            var dup = selected.System.Duplicate() as HB.SHWSystem;
            var name = $"{dup.DisplayName ?? dup.Identifier}_dup";
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

            //if (selected.Locked)
            //{
            //    MessageBox.Show(_control, "You cannot edit an item of system library! Try to duplicate it first!");
            //    return;
            //}

            var selectedObj = selected.System;
            var dup = selectedObj.Duplicate() as HB.SHWSystem;
            HB.SHWSystem dialog_rc = null;

            var lib = _modelEnergyProperties;
            var dialog = new Dialog_SHW(dup, selected.Locked);
            dialog_rc = dialog.ShowModal(_control);

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



    internal class SHWViewData: ManagerViewDataBase
    {
        public string EType { get; }
        public string Condition { get; }
        public string LossCoeff { get; }
        public string HeaterEfficiency { get; }
        
        public string Source { get; } = "Model";
        public bool Locked { get; }
        public HB.SHWSystem System { get; }


        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.Shws.Select(_ => _.Identifier);

        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds;

        public SHWViewData(HB.SHWSystem c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            this.EType = c.EquipmentType.ToString();
            if (c.HeaterEfficiency?.Obj is Autocalculate)
                this.HeaterEfficiency = "Autocalculate";
            else
                this.HeaterEfficiency = c.HeaterEfficiency?.Obj?.ToString();

            this.Condition = c.AmbientCondition.Obj?.ToString();
            this.LossCoeff = c.AmbientLossCoefficient.ToString();
            this.System = c;

            this.SearchableText = $"{this.Name}_{this.EType}_{HeaterEfficiency}_{Condition}_{LossCoeff}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            //else if (NRELLibraryIds.Contains(this.Name)) this.Source = "DoE NREL";
        }

    }

}
