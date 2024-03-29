﻿using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class HVACManagerViewModel : ManagerBaseViewModel<HVACViewData>
    {
        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private static ManagerItemComparer<HVACViewData> _viewDataComparer = new ManagerItemComparer<HVACViewData>();

        public HVACManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default):base(control)
        {
            _modelEnergyProperties = libSource;

            this._userData = libSource.HVACList.Select(_ => new HVACViewData(_)).ToList();
            this._systemData = SystemEnergyLib.HVACList.Select(_ => new HVACViewData(_)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();

            ResetDataCollection();
        }

        private void AddUserData(HB.Energy.IHvac item)
        {
            var newItem = CheckObjID(item);
            var newDataView = new HVACViewData(newItem);
            if (!this._userData.Contains(newDataView))
            {
                // user selected an item from system library, now add it to model EnergyProperties
                this._modelEnergyProperties.AddHVAC(newDataView.HVAC);
            }
            this._userData.Insert(0, newDataView);
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void ReplaceUserData(HVACViewData oldObj, HB.Energy.IHvac newObj)
        {
            var newItem = CheckObjID(newObj, oldObj.Identifier);
            var index = _userData.IndexOf(oldObj);
            _userData.RemoveAt(index);
            _userData.Insert(index, new HVACViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
        }
        private void DeleteUserData(HVACViewData item)
        {
            this._userData.Remove(item);
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
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

    

        private void ShowHVACDialog(HB.Energy.IHvac hvac)
        {
            try
            {
                HB.Energy.IHvac dialog_rc = null;
                var lib = _modelEnergyProperties;
                if (hvac is IdealAirSystemAbridged obj)
                {
                    var dialog = new Dialog_IdealAirLoad(ref lib, obj);
                    dialog_rc = dialog.ShowModal(_control);
                }
                else
                {
                    var dialog = new Dialog_OpsHVACs(ref lib, hvac);
                    dialog_rc = dialog.ShowModal(_control);
                }

                if (dialog_rc != null)
                {
                    AddUserData(dialog_rc);
                    ResetDataCollection();
                }
            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
                //throw;
            }
         

        }
    
 
        public ICommand AddIdealAirLoadCommand => new RelayCommand<HoneybeeSchema.IdealAirSystemAbridged>((obj) => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"New Ideal Air System {id}";
            obj = obj ?? new IdealAirSystemAbridged(id, displayName: name);
            ShowHVACDialog(obj);
        });
        public ICommand AddOpsHVACCommand => new RelayCommand(
            () => {
                var dialog = new Dialog_OpsHVACSelector();
                var sysType = dialog.ShowModal(_control);
                if (sysType == null)
                    return;
          
                ShowHVACDialog(sysType);
            }
            );
      

        public RelayCommand AddCommand => new RelayCommand(() =>
        {

            var contextMenu = new ContextMenu();

            var AddHvacsDic = new Dictionary<string, ICommand>()
                {
                    { "Ideal Air Load", AddIdealAirLoadCommand},
                    { "From OpenStudio library", AddOpsHVACCommand},
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

            if (GetDetailedHVACItems != null)
            {
                Action endOfMenuClick = () =>
                {
                    (_control as Dialog)?.Close();
                };
                var tps = GetDetailedHVACItems(endOfMenuClick);
       
                contextMenu.Items.Add(tps);
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

            if (selected.HVAC is DetailedHVAC dt)
            {
                if (ValidatePlaceholder(dt))
                {
                    (_control as Dialog)?.Close();
                    DuplicateIBDoc?.Invoke(dt);
                }else
                {
                    Dialog_Message.Show(_control, $"Non-Ironbug DetailedHVAC [{dt.Identifier}]!");
                }

                return;
            }


            var dup = selected.HVAC.Duplicate() as HB.Energy.IHvac;
            var name = $"{dup.DisplayName ?? dup.Identifier}_dup";
            dup.Identifier = Guid.NewGuid().ToString().Substring(0, 5);
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


            var selectedObj = selected.HVAC;
            var dup = selectedObj.Duplicate() as HB.Energy.IHvac;
            HB.Energy.IHvac dialog_rc = null;

            var lib = _modelEnergyProperties;
            if (dup is IdealAirSystemAbridged obj)
            {
                var dialog = new Dialog_IdealAirLoad(ref lib, obj, selected.Locked);
                dialog_rc = dialog.ShowModal(_control);
            }
            else if (dup is DetailedHVAC dt)
            {
                if (ValidatePlaceholder(dt))
                {
                    dialog_rc = EditDetailedHVAC?.Invoke(dt);
                }
                else
                {
                    Dialog_Message.Show(_control, $"Non-Ironbug DetailedHVAC [{dt.Identifier}]!");
                    return;
                }
            }
            else
            {
                var dialog = new Dialog_OpsHVACs(ref lib, dup, selected.Locked);
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
            if (res != DialogResult.Yes)
                return;

            DeleteUserData(selected);
            ResetDataCollection();

        });


        public RelayCommand EditIBSystemCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }

            var selectedObj = selected.HVAC;
            var dup = selectedObj.Duplicate() as HB.Energy.IHvac;

            var lib = _modelEnergyProperties;
            if (dup is DetailedHVAC dt)
            {
                if (ValidatePlaceholder(dt))
                {
                    (_control as Dialog)?.Close();
                    EditIBDoc?.Invoke(dt);
                    return;
                }
            }

            Dialog_Message.Show(_control, $"Non-Ironbug DetailedHVAC [{selectedObj.DisplayName ?? selectedObj.Identifier}]!");

        });

        
        public static Func<Action,ButtonMenuItem> GetDetailedHVACItems { get; set; }
        public static Func<DetailedHVAC, bool> ValidatePlaceholder { get; set; }
        public static Func<DetailedHVAC, DetailedHVAC> EditDetailedHVAC { get; set; }
        public static Action<DetailedHVAC> EditIBDoc { get; set; }
        public static Action<DetailedHVAC> DuplicateIBDoc { get; set; }
    }



    internal class HVACViewData: ManagerViewDataBase
    {
        public string CType { get; }
        public bool HasIB { get; }
        public string Source { get; } = "Model";
        public bool Locked { get; }
        public HB.Energy.IHvac HVAC { get; }


        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.HVACList.Select(_ => _.Identifier);

        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds;

        public HVACViewData(HB.Energy.IHvac c)
        {
            this.Identifier = c.Identifier;
            this.Name = c.DisplayName ?? c.Identifier;
            var type = c.GetType().GetProperty("EquipmentType")?.GetValue(c)?.ToString() ?? c.GetType().Name;
            this.CType = OpsHVACsViewModel.HVACUserFriendlyNamesDic.TryGetValue(type, out var userFriendly) ? userFriendly : type;
            this.HVAC = c;

            this.HasIB = false;
            if (c is HB.DetailedHVAC dHVAC )
            {
                if (HVACManagerViewModel.ValidatePlaceholder != null)
                    this.HasIB = HVACManagerViewModel.ValidatePlaceholder.Invoke(dHVAC);
            }

            this.SearchableText = $"{this.Name}_{this.CType}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            //else if (NRELLibraryIds.Contains(this.Name)) this.Source = "DoE NREL";
        }

    }

}
