using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class ProcessLoadViewModel : CheckboxPanelViewModel
    {
        private ProcessAbridged _refHBObj => this.refObjProperty as ProcessAbridged;
        // double watts, string schedule, FuelTypes fuelType, object userData = null, string endUseCategory = "Process", double radiantFraction = 0, double latentFraction = 0, double lostFraction = 0

        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            set
            {
                _isDisplayNameVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName));
            }
        }

        // fuel type 
        public FuelTypes FuelType
        {
            get => _refHBObj.FuelType;
            set
            {
                FuelTypeText = value.ToString();
                this.Set(() => _refHBObj.FuelType = value, nameof(FuelType));
            }
        }

        private bool _isFuelTypeVaries;
        private string _fuelTypeText;
        public string FuelTypeText
        {
            get => _fuelTypeText;
            set
            {
                _isFuelTypeVaries = value == ReservedText.Varies;
                this.Set(() => _fuelTypeText = value, nameof(FuelTypeText));
            }
        }


        // endUseCategory
        private bool _isEndUseCategoryVaries;
        public string EndUseCategory
        {
            get => _refHBObj.EndUseCategory;
            set
            {
                _isEndUseCategoryVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.EndUseCategory = value, nameof(EndUseCategory));
            }
        }

        // Watts
        private DoubleViewModel _watts;

        public DoubleViewModel Watts
        {
            get => _watts;
            set
            {
                this.Set(() => _watts = value, nameof(Watts));
            }
        }


        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // RadiantFraction
        private DoubleViewModel _radiantFraction;

        public DoubleViewModel RadiantFraction
        {
            get => _radiantFraction;
            set { this.Set(() => _radiantFraction = value, nameof(RadiantFraction)); }
        }

        // latentFraction
        private DoubleViewModel _latentFraction;

        public DoubleViewModel LatentFraction
        {
            get => _latentFraction;
            set { this.Set(() => _latentFraction = value, nameof(LatentFraction)); }
        }

        // LostFraction
        private DoubleViewModel _lostFraction;

        public DoubleViewModel LostFraction
        {
            get => _lostFraction;
            set { this.Set(() => _lostFraction = value, nameof(LostFraction)); }
        }

        public ProcessAbridged Default { get; private set; }
        public ProcessLoadViewModel(ModelProperties libSource, List<ProcessAbridged> loads, Action<IIDdBase> setAction) : base(libSource, setAction)
        {
            this.Default = new ProcessAbridged(Guid.NewGuid().ToString(), 0, ReservedText.None, FuelTypes.Electricity);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateProcessAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateProcessAbridged();


            if (loads.Distinct().Count() == 1) 
                this.IsCheckboxChecked = loads.FirstOrDefault() == null;
            else
                this.IsCheckboxVaries();

            //DisplayName
            if (loads.Select(_ => _?.DisplayName).Distinct().Count() > 1)
                this.DisplayName = ReservedText.Varies;
            else
                this.DisplayName = this._refHBObj.DisplayName;

            // EndUseCategory
            if (loads.Select(_ => _?.EndUseCategory).Distinct().Count() > 1)
                this.EndUseCategory = ReservedText.Varies;
            else
                this.EndUseCategory = this._refHBObj.EndUseCategory;


            // FuelType
            if (loads.Select(_ => _?.FuelType).Distinct().Count() > 1)
                this.FuelTypeText = ReservedText.Varies;
            else
                this.FuelType = this._refHBObj.FuelType;


            //Watts
            this.Watts = new DoubleViewModel((n) => _refHBObj.Watts = n);
            this.Watts.SetUnits(Units.PowerUnit.Watt, Units.UnitType.Power);
            if (loads.Select(_ => _?.Watts).Distinct().Count() > 1)
                this.Watts.SetNumberText(ReservedText.Varies);
            else
                this.Watts.SetBaseUnitNumber(_refHBObj.Watts);


            //Schedule
            var sch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            sch = sch ?? GetDummyScheduleObj(_refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(ReservedText.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //RadiantFraction
            this.RadiantFraction = new DoubleViewModel((n) => _refHBObj.RadiantFraction = n);
            if (loads.Select(_ => _?.RadiantFraction).Distinct().Count() > 1)
                this.RadiantFraction.SetNumberText(ReservedText.Varies);
            else
                this.RadiantFraction.SetNumberText(_refHBObj.RadiantFraction.ToString());


            //LatentFraction
            this.LatentFraction = new DoubleViewModel((n) => _refHBObj.LatentFraction = n);
            if (loads.Select(_ => _?.LatentFraction).Distinct().Count() > 1)
                this.LatentFraction.SetNumberText(ReservedText.Varies);
            else
                this.LatentFraction.SetNumberText(_refHBObj.LatentFraction.ToString());


            //LostFraction
            this.LostFraction = new DoubleViewModel((n) => _refHBObj.LostFraction = n);
            if (loads.Select(_ => _?.LostFraction).Distinct().Count() > 1)
                this.LostFraction.SetNumberText(ReservedText.Varies);
            else
                this.LostFraction.SetNumberText(_refHBObj.LostFraction.ToString());
        }

        public ProcessAbridged MatchObj(ProcessAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked.GetValueOrDefault())
                return null;

            if (this.IsVaries)
                return obj?.DuplicateProcessAbridged();

            obj = obj?.DuplicateProcessAbridged() ?? new ProcessAbridged(Guid.NewGuid().ToString(), 0, ReservedText.NotSet, FuelTypes.Electricity);


            if (!this._isDisplayNameVaries)
                obj.DisplayName = this._refHBObj.DisplayName;

            if (!this._isEndUseCategoryVaries)
                obj.EndUseCategory = this._refHBObj.EndUseCategory;

            if (!this._isFuelTypeVaries)
                obj.FuelType = this._refHBObj.FuelType;

            if (!this.Watts.IsVaries)
                obj.Watts = this._refHBObj.Watts;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing a required process load schedule!");
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
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Schedule.SetPropetyObj(dialog_rc[0]);
            }
        });

       

    }

}
