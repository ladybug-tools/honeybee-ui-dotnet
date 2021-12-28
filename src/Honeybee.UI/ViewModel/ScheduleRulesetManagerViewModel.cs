﻿using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class ScheduleRulesetManagerViewModel : ManagerBaseViewModel<ScheduleRulesetViewData>
    {
      
        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private List<ScheduleTypeLimit> _typeLimits;
    

        public ScheduleRulesetManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default):base(control)
        {
            _modelEnergyProperties = libSource;

            libSource.AddScheduleTypeLimits(HB.Helper.EnergyLibrary.UserScheduleTypeLimits); 
            libSource.AddScheduleTypeLimits(ModelEnergyProperties.Default.ScheduleTypeLimits);
            var schTypes = libSource.ScheduleTypeLimits;
            _typeLimits = schTypes.Select(_ => _.DuplicateScheduleTypeLimit()).ToList();
            


            this._userData = libSource.ScheduleList.OfType<ScheduleRulesetAbridged>().Select(_ => new ScheduleRulesetViewData(_, ref _typeLimits)).ToList();
            this._systemData = HB.Helper.EnergyLibrary.UserSchedules.OfType<ScheduleRulesetAbridged>().Select(_ => new ScheduleRulesetViewData(_, ref _typeLimits))
                .Concat(ModelEnergyProperties.Default.Schedules.OfType<ScheduleRulesetAbridged>().Select(_ => new ScheduleRulesetViewData(_, ref _typeLimits))).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(new ManagerItemComparer<ScheduleRulesetViewData>()).ToList();

         
            ResetDataCollection();

        }

        private ScheduleRuleset AbridgedToReal(ScheduleRulesetAbridged obj)
        {
            var typeLimit = _typeLimits.FirstOrDefault(_ => _.Identifier == obj.ScheduleTypeLimit);

            var realObj = new ScheduleRuleset(obj.Identifier, obj.DaySchedules, obj.DefaultDaySchedule, obj.DisplayName, obj.UserData,
               obj.ScheduleRules, obj.HolidaySchedule, obj.SummerDesigndaySchedule, obj.WinterDesigndaySchedule, typeLimit);

            return realObj;
        }
        //private ScheduleRulesetAbridged ToAbridged(ScheduleRuleset obj)
        //{
        //    var abridged = new ScheduleRulesetAbridged(obj.Identifier, obj.DaySchedules, obj.DefaultDaySchedule, obj.DisplayName, obj.UserData,
        //       obj.ScheduleRules, obj.HolidaySchedule, obj.SummerDesigndaySchedule, obj.WinterDesigndaySchedule, obj.ScheduleTypeLimit?.Identifier);
        //    _typeLimits.Add(obj.ScheduleTypeLimit);
        //    return abridged;
        //}

        public void UpdateLibSource()
        {
            var typeLimits = this._userData.Select(_ => _.TypeLimitObj).Distinct();
            this._modelEnergyProperties.ScheduleTypeLimits.Clear();
            this._modelEnergyProperties.AddScheduleTypeLimits(_typeLimits);
            this._modelEnergyProperties.AddScheduleTypeLimits(typeLimits);

            var newItems = this._userData.Select(_ => _.ScheduleRuleset);

            this._modelEnergyProperties.Schedules.Clear();
            this._modelEnergyProperties.AddSchedules(newItems);
        }

        public List<HB.Energy.ISchedule> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Energy.ISchedule>();

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
                    var engLib = d.CheckResources();
                    this._modelEnergyProperties.MergeWith(engLib);
                }

                itemsToReturn.Add(d.ScheduleRuleset);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.ScheduleList.ToList();
            }
            return itemsToReturn;
        }


 

        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var dayId = Guid.NewGuid().ToString();
            var dayName = $"New Schedule Day {dayId.Substring(0, 5)}";
            var newDay = new ScheduleDay(
                dayId,
                new List<double> { 0.3 },
                dayName,
                new List<List<int>>() { new List<int> { 0, 0 } }
                );

            var id = Guid.NewGuid().ToString();
            var newSch = new ScheduleRuleset(
                id,
                new List<ScheduleDay> { newDay },
                dayId,
                $"New Schedule Ruleset {id.Substring(0, 5)}"
                );
            
            //TODO: needs to create type limit library.
            newSch.ScheduleTypeLimit = new ScheduleTypeLimit("Fractional", "", 0, 1);
            var dialog = new Honeybee.UI.Dialog_Schedule(newSch);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;

            var newItem = CheckObjName(dialog_rc);
            
            var newViewdata = new ScheduleRulesetViewData(newItem);
            if (!this._userData.Contains(newViewdata))
            {
                // add resources to model EnergyProperties
                var engLib = newViewdata.CheckResources();
                this._modelEnergyProperties.MergeWith(engLib);
            }
            this._userData.Insert(0, newViewdata);
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


            var dup = selected.ScheduleRuleset.Duplicate() as ScheduleRulesetAbridged;
            var name = $"{dup.DisplayName ?? dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;
            var realObj = AbridgedToReal(dup);
            var dialog = new Honeybee.UI.Dialog_Schedule(realObj);
            var dialog_rc = dialog.ShowModal(_control);

            if (dialog_rc == null) return;

            var newItem = CheckObjName(dialog_rc);
            var newViewdata = new ScheduleRulesetViewData(newItem);
            if (!this._userData.Contains(newViewdata))
            {
                // add resources to model EnergyProperties
                var engLib = newViewdata.CheckResources();
                this._modelEnergyProperties.MergeWith(engLib);
            }

            this._userData.Insert(0, newViewdata);
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

            var dup = selected.ScheduleRuleset.Duplicate() as ScheduleRulesetAbridged;
            var realObj = AbridgedToReal(dup);
            var dialog = new Honeybee.UI.Dialog_Schedule(realObj, selected.Locked);
            var dialog_rc = dialog.ShowModal(_control);
       
            if (dialog_rc == null) return;
            var newItem = CheckObjName(dialog_rc, selected.Name);
            var index = _userData.IndexOf(selected);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ScheduleRulesetViewData(newItem));
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



    internal class ScheduleRulesetViewData: ManagerViewDataBase
    {
        public string TypeLimit { get; }
        public string Source { get; } = "Model";
        public bool Locked { get; }
        public HB.Energy.ISchedule ScheduleRuleset { get; }


        public ScheduleTypeLimit TypeLimitObj { get; }
        //public static List<ScheduleTypeLimit> TypeLimits;
        //private static IEnumerable<string> NRELLibraryIds =
        //    HB.Helper.EnergyLibrary.StandardsOpaqueScheduleRulesets.Keys
        //    .Concat(HB.Helper.EnergyLibrary.StandardsWindowScheduleRulesets.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelEnergyProperties.Default.ScheduleList.Select(_ => _.Identifier);

        private static IEnumerable<string> UserLibIds = HB.Helper.EnergyLibrary.UserSchedules.Select(_ => _.Identifier);
        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(UserLibIds);

        public ScheduleRulesetViewData(HB.ScheduleRulesetAbridged c, ref List<ScheduleTypeLimit> libSource)
        {
            this.Name = c.DisplayName ?? c.Identifier;
          
            this.ScheduleRuleset = c;

            var typeLimit = libSource.FirstOrDefault(_ => _.Identifier == c.ScheduleTypeLimit);
            this.TypeLimit =  typeLimit?.DisplayName ?? typeLimit?.Identifier;
            this.TypeLimitObj = typeLimit;
            this.SearchableText = $"{this.Name}_{this.TypeLimit}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            else if (UserLibIds.Contains(c.Identifier)) this.Source = "User";
        }

        public ScheduleRulesetViewData(HB.ScheduleRuleset c)
        {
            this.Name = c.DisplayName ?? c.Identifier;

            this.ScheduleRuleset = c;

            this.TypeLimitObj = c.ScheduleTypeLimit;
            this.TypeLimit = TypeLimitObj?.DisplayName ?? TypeLimitObj?.Identifier;
        
            this.SearchableText = $"{this.Name}_{this.TypeLimit}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            else if (UserLibIds.Contains(c.Identifier)) this.Source = "User";
        }

        internal HB.ModelEnergyProperties CheckResources()
        {
            
            var eng = new ModelEnergyProperties();
            eng.AddSchedule(this.ScheduleRuleset);
            eng.AddScheduleTypeLimit(this.TypeLimitObj);
            return eng.DuplicateModelEnergyProperties();

        }
    }

}
