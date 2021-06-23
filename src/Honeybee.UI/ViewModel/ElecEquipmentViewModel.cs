﻿using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class ElecEquipmentViewModel : CheckboxPanelViewModel
    {
        private ElectricEquipmentAbridged _refHBObj => this.refObjProperty as ElectricEquipmentAbridged;
        // double wattsPerArea, string schedule, double radiantFraction = 0, double latentFraction = 0, double lostFraction = 0

        // WattsPerArea
        private DoubleViewModel _wattsPerArea;

        public DoubleViewModel WattsPerArea
        {
            get => _wattsPerArea;
            private set {
                this.Set(() => _wattsPerArea = value, nameof(WattsPerArea)); 
            }
        }
        

        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            private set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // RadiantFraction
        private DoubleViewModel _radiantFraction;

        public DoubleViewModel RadiantFraction
        {
            get => _radiantFraction;
            private set { this.Set(() => _radiantFraction = value, nameof(RadiantFraction)); }
        }

        // latentFraction
        private DoubleViewModel _latentFraction;

        public DoubleViewModel LatentFraction
        {
            get => _latentFraction;
            private set { this.Set(() => _latentFraction = value, nameof(LatentFraction)); }
        }

        // LostFraction
        private DoubleViewModel _lostFraction;

        public DoubleViewModel LostFraction
        {
            get => _lostFraction;
            private set { this.Set(() => _lostFraction = value, nameof(LostFraction)); }
        }
        public ElectricEquipmentAbridged Default { get; private set; }

        public ElecEquipmentViewModel(ModelProperties libSource, List<ElectricEquipmentAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new ElectricEquipmentAbridged(Guid.NewGuid().ToString(), 0, "Not Set");
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateElectricEquipmentAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateElectricEquipmentAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.WattsPerArea = new DoubleViewModel((n) => _refHBObj.WattsPerArea = n);
            if (loads.Select(_ => _?.WattsPerArea).Distinct().Count() > 1)
                this.WattsPerArea.SetNumberText(this.Varies);
            else
                this.WattsPerArea.SetNumberText(_refHBObj.WattsPerArea.ToString());


            //Schedule
            var sch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(this.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //RadiantFraction
            this.RadiantFraction = new DoubleViewModel((n) => _refHBObj.RadiantFraction = n);
            if (loads.Select(_ => _?.RadiantFraction).Distinct().Count() > 1)
                this.RadiantFraction.SetNumberText(this.Varies);
            else
                this.RadiantFraction.SetNumberText(_refHBObj.RadiantFraction.ToString());


            //LatentFraction
            this.LatentFraction = new DoubleViewModel((n) => _refHBObj.LatentFraction = n);
            if (loads.Select(_ => _?.LatentFraction).Distinct().Count() > 1)
                this.LatentFraction.SetNumberText(this.Varies);
            else
                this.LatentFraction.SetNumberText(_refHBObj.LatentFraction.ToString());


            //LostFraction
            this.LostFraction = new DoubleViewModel((n) => _refHBObj.LostFraction = n);
            if (loads.Select(_ => _?.LostFraction).Distinct().Count() > 1)
                this.LostFraction.SetNumberText(this.Varies);
            else
                this.LostFraction.SetNumberText(_refHBObj.LostFraction.ToString());
        }

        public ElectricEquipmentAbridged MatchObj(ElectricEquipmentAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateElectricEquipmentAbridged() ?? new ElectricEquipmentAbridged(Guid.NewGuid().ToString(), 0, "Not Set");

            if (!this.WattsPerArea.IsVaries)
                obj.WattsPerArea = this._refHBObj.WattsPerArea;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing required electric equipment schedule!");
                obj.Schedule = this._refHBObj.Schedule;
            }
            if (!this.RadiantFraction.IsVaries)
                obj.RadiantFraction = this._refHBObj.RadiantFraction;

            if (!this.LatentFraction.IsVaries)
                obj.LatentFraction = this._refHBObj.LatentFraction;
            if (!this.LostFraction.IsVaries)
                obj.LostFraction = this._refHBObj.LostFraction;
            return obj;
        }

        public RelayCommand ScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Schedule.SetPropetyObj(dialog_rc[0]);
            }
        });

    }


}
