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

        public Func<string> AmbientCoffConditionRoomPicker { get; set; }
        public SHWManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default, Func<string> roomIDPicker = default) :base(control)
        {
            AmbientCoffConditionRoomPicker = roomIDPicker;
            _modelEnergyProperties = libSource;

            this._userData = libSource.Shws.Select(_ => new SHWViewData(_)).ToList();
            this._systemData = SystemEnergyLib.Shws.Select(_ => new SHWViewData(_)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();

            ResetDataCollection();
        }

        private void AddUserData(HB.SHWSystem item)
        {
            var newItem = CheckObjID(item);
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
            var newItem = CheckObjID(newObj, oldObj.Identifier);
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
            this._modelEnergyProperties.Shws.Clear();
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

    

        private void ShowSHWDialog(HB.SHWSystem system, Action<SHWSystem> doneAction = default, bool editID = false)
        {

            var dialog = new Dialog_SHW(system, roomIDPicker: AmbientCoffConditionRoomPicker, editID: editID );
            System.Action<SHWSystem> f = (SHWSystem s) =>
            {
                if (s != null)
                {
                    if (doneAction != default)
                        doneAction(s); //edit object
                    else
                        AddUserData(s); // add/duplicate object
                    ResetDataCollection();
                }
            };


            dialog.ShowModal(_control, f);

        }
    
        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            ShowSHWDialog(null, editID: true);
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
            dup.Identifier = Guid.NewGuid().ToString().Substring(0, 5);
            dup.DisplayName = name;

            ShowSHWDialog(dup, editID: true);

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

            var selectedObj = selected.System;
            var dup = selectedObj.Duplicate() as HB.SHWSystem;

            System.Action<SHWSystem> f = (SHWSystem s) =>
            {
                if (s == null) return;
                ReplaceUserData(selected, s);
            };

            ShowSHWDialog(dup, f, editID: false);


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
            this.Identifier = c.Identifier;
            this.Name = c.DisplayName ?? c.Identifier;
            this.EType = c.EquipmentType.ToString();
            if (c.HeaterEfficiency?.Obj is Autocalculate)
                this.HeaterEfficiency = "Autocalculate";
            else
                this.HeaterEfficiency = c.HeaterEfficiency?.Obj?.ToString();

            this.Condition = c.AmbientCondition?.Obj?.ToString();
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
