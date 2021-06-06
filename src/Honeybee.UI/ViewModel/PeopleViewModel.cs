﻿using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class PeopleViewModel : CheckboxPanelViewModel
    {
        private PeopleAbridged _refHBObj => this.refObjProperty as PeopleAbridged;
        // double peoplePerArea, string occupancySchedule, string activitySchedule, string displayName = null, double radiantFraction = 0.3, AnyOf<Autocalculate, double> latentFraction = null

        // WattsPerArea
        private DoubleViewModel _peoplePerArea;

        public DoubleViewModel PeoplePerArea
        {
            get => _peoplePerArea;
            private set {
                this.Set(() => _peoplePerArea = value, nameof(PeoplePerArea)); 
            }
        }


        // OccupancySchedule
        private ButtonViewModel _occupancySchedule;

        public ButtonViewModel OccupancySchedule
        {
            get => _occupancySchedule;
            private set { this.Set(() => _occupancySchedule = value, nameof(OccupancySchedule)); }
        }

        // activitySchedule
        private ButtonViewModel _activitySchedule;

        public ButtonViewModel ActivitySchedule
        {
            get => _activitySchedule;
            private set { this.Set(() => _activitySchedule = value, nameof(ActivitySchedule)); }
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

        private bool _isLatentFractionAutocalculate;

        public bool IsLatentFractionAutocalculate
        {
            get => _isLatentFractionAutocalculate;
            private set {

                if (value)
                    this._refHBObj.LatentFraction = new Autocalculate();
                else
                    this.LatentFraction.SetNumberText(this.LatentFraction.NumberText);

                IsLatenFractionInputEnabled = !value;
                this.Set(() => _isLatentFractionAutocalculate = value, nameof(IsLatentFractionAutocalculate));
            }
        }

        private bool _isLatenFractionInputEnabled;

        public bool IsLatenFractionInputEnabled
        {
            get => _isLatenFractionInputEnabled && this.IsPanelEnabled;
            private set { this.Set(() => _isLatenFractionInputEnabled = value, nameof(IsLatenFractionInputEnabled)); }
        }



        public PeopleAbridged Default { get; private set; }
        public PeopleViewModel(ModelProperties libSource, List<PeopleAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new PeopleAbridged(Guid.NewGuid().ToString(), 0, "", "");
            this.refObjProperty = loads.FirstOrDefault()?.DuplicatePeopleAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicatePeopleAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.PeoplePerArea = new DoubleViewModel((n) => _refHBObj.PeoplePerArea = n);
            if (loads.Select(_ => _?.PeoplePerArea).Distinct().Count() > 1)
                this.PeoplePerArea.SetNumberText(this.Varies);
            else
                this.PeoplePerArea.SetNumberText(_refHBObj.PeoplePerArea.ToString());


            //Schedule
            var sch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.OccupancySchedule);
            this.OccupancySchedule = new ButtonViewModel((n) => _refHBObj.OccupancySchedule = n?.Identifier);
            if (loads.Select(_ => _?.OccupancySchedule).Distinct().Count() > 1)
                this.OccupancySchedule.SetBtnName(this.Varies);
            else
                this.OccupancySchedule.SetPropetyObj(sch);


            //ActivitySchedule
            var actSch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.ActivitySchedule);
            this.ActivitySchedule = new ButtonViewModel((n) => _refHBObj.ActivitySchedule = n?.Identifier);
            if (loads.Select(_ => _?.ActivitySchedule).Distinct().Count() > 1)
                this.ActivitySchedule.SetBtnName(this.Varies);
            else
                this.ActivitySchedule.SetPropetyObj(actSch);



            //RadiantFraction
            this.RadiantFraction = new DoubleViewModel((n) => _refHBObj.RadiantFraction = n);
            if (loads.Select(_ => _?.RadiantFraction).Distinct().Count() > 1)
                this.RadiantFraction.SetNumberText(this.Varies);
            else
                this.RadiantFraction.SetNumberText(_refHBObj.RadiantFraction.ToString());


            //LatentFraction
            this.LatentFraction = new DoubleViewModel((n) => _refHBObj.LatentFraction = n);
            var latFractions = loads.Select(_ => _?.LatentFraction).Distinct();
            if (latFractions.Count() > 1)
            {
                this.IsLatentFractionAutocalculate = false;
                this.LatentFraction.SetNumberText(this.Varies);
            }
            else
            {
                this.LatentFraction.SetNumberText("0");
                this.IsLatentFractionAutocalculate = latFractions?.FirstOrDefault(_ => _?.Obj is Autocalculate) != null;
                if (!IsLatentFractionAutocalculate)
                {
                    this.LatentFraction.SetNumberText(_refHBObj.LatentFraction.ToString());
                }
            }

        }

        public PeopleAbridged MatchObj(PeopleAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicatePeopleAbridged() ?? new PeopleAbridged(Guid.NewGuid().ToString(), 0, "", "");

            if (!this.PeoplePerArea.IsVaries)
                obj.PeoplePerArea = this._refHBObj.PeoplePerArea;
            if (!this.OccupancySchedule.IsVaries)
            {
                if (this._refHBObj.OccupancySchedule == null)
                    throw new ArgumentException("Missing required occupancy schedule of the people load!");
                obj.OccupancySchedule = this._refHBObj.OccupancySchedule;
            }
            if (!this.ActivitySchedule.IsVaries)
                obj.ActivitySchedule = this._refHBObj.ActivitySchedule;
            if (!this.RadiantFraction.IsVaries)
                obj.RadiantFraction = this._refHBObj.RadiantFraction;

            if (!this.LatentFraction.IsVaries)
                obj.LatentFraction = this._refHBObj.LatentFraction;
   
            return obj;
        }

        public RelayCommand ScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.OccupancySchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand ActivityScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.ActivitySchedule.SetPropetyObj(dialog_rc[0]);
            }
        });
      
    }


}
