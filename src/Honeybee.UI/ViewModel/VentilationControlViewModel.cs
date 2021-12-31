using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class VentilationControlViewModel : CheckboxPanelViewModel<VentilationControlAbridged>
    {
        private VentilationControlAbridged _refHBObj => this.refObjProperty;
        // double minIndoorTemperature = -100, double maxIndoorTemperature = 100, double minOutdoorTemperature = -100, double maxOutdoorTemperature = 100, double deltaTemperature = -100, string schedule = null

        // MinIndoorTemperature
        private DoubleViewModel _minIndoorTemperature;

        public DoubleViewModel MinIndoorTemperature
        {
            get => _minIndoorTemperature;
            private set {
                this.Set(() => _minIndoorTemperature = value, nameof(MinIndoorTemperature)); 
            }
        }
        

        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            private set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // MaxIndoorTemperature
        private DoubleViewModel _maxIndoorTemperature;

        public DoubleViewModel MaxIndoorTemperature
        {
            get => _maxIndoorTemperature;
            private set { this.Set(() => _maxIndoorTemperature = value, nameof(MaxIndoorTemperature)); }
        }

        // minOutdoorTemperature
        private DoubleViewModel _minOutdoorTemperature;

        public DoubleViewModel MinOutdoorTemperature
        {
            get => _minOutdoorTemperature;
            private set { this.Set(() => _minOutdoorTemperature = value, nameof(MinOutdoorTemperature)); }
        }

        // maxOutdoorTemperature
        private DoubleViewModel _maxOutdoorTemperature;

        public DoubleViewModel MaxOutdoorTemperature
        {
            get => _maxOutdoorTemperature;
            private set { this.Set(() => _maxOutdoorTemperature = value, nameof(MaxOutdoorTemperature)); }
        }

        // DeltaTemperature
        private DoubleViewModel _deltaTemperature;

        public DoubleViewModel DeltaTemperature
        {
            get => _deltaTemperature;
            private set { this.Set(() => _deltaTemperature = value, nameof(DeltaTemperature)); }
        }

        public VentilationControlAbridged Default { get; private set; }
        public VentilationControlViewModel(ModelProperties libSource, List<VentilationControlAbridged> loads, Action<VentilationControlAbridged> setAction):base(libSource, setAction)
        {
            this.Default = new VentilationControlAbridged();
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateVentilationControlAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateVentilationControlAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //MinIndoorTemperature
            this.MinIndoorTemperature = new DoubleViewModel((n) => _refHBObj.MinIndoorTemperature = n);
            this.MinIndoorTemperature.SetUnits(Units.TemperatureUnit.DegreeCelsius, Units.UnitType.Temperature);
            if (loads.Select(_ => _?.MinIndoorTemperature).Distinct().Count() > 1)
                this.MinIndoorTemperature.SetNumberText(this.Varies);
            else
                this.MinIndoorTemperature.SetBaseUnitNumber(_refHBObj.MinIndoorTemperature);


            //Schedule
            var sch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(this.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //MaxIndoorTemperature
            this.MaxIndoorTemperature = new DoubleViewModel((n) => _refHBObj.MaxIndoorTemperature = n);
            this.MaxIndoorTemperature.SetUnits(Units.TemperatureUnit.DegreeCelsius, Units.UnitType.Temperature);
            if (loads.Select(_ => _?.MaxIndoorTemperature).Distinct().Count() > 1)
                this.MaxIndoorTemperature.SetNumberText(this.Varies);
            else
                this.MaxIndoorTemperature.SetBaseUnitNumber(_refHBObj.MaxIndoorTemperature);


            //MinOutdoorTemperature
            this.MinOutdoorTemperature = new DoubleViewModel((n) => _refHBObj.MinOutdoorTemperature = n);
            this.MinOutdoorTemperature.SetUnits(Units.TemperatureUnit.DegreeCelsius, Units.UnitType.Temperature);
            if (loads.Select(_ => _?.MinOutdoorTemperature).Distinct().Count() > 1)
                this.MinOutdoorTemperature.SetNumberText(this.Varies);
            else
                this.MinOutdoorTemperature.SetBaseUnitNumber(_refHBObj.MinOutdoorTemperature);


            //MaxOutdoorTemperature
            this.MaxOutdoorTemperature = new DoubleViewModel((n) => _refHBObj.MaxOutdoorTemperature = n);
            this.MaxOutdoorTemperature.SetUnits(Units.TemperatureUnit.DegreeCelsius, Units.UnitType.Temperature);
            if (loads.Select(_ => _?.MaxOutdoorTemperature).Distinct().Count() > 1)
                this.MaxOutdoorTemperature.SetNumberText(this.Varies);
            else
                this.MaxOutdoorTemperature.SetBaseUnitNumber(_refHBObj.MaxOutdoorTemperature);


            //DeltaTemperature
            this.DeltaTemperature = new DoubleViewModel((n) => _refHBObj.DeltaTemperature = n);
            this.DeltaTemperature.SetUnits(Units.TemperatureDeltaUnit.DegreeCelsius, Units.UnitType.TemperatureDelta);
            if (loads.Select(_ => _?.DeltaTemperature).Distinct().Count() > 1)
                this.DeltaTemperature.SetNumberText(this.Varies);
            else
                this.DeltaTemperature.SetBaseUnitNumber(_refHBObj.DeltaTemperature);
        }

        public VentilationControlAbridged MatchObj(VentilationControlAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateVentilationControlAbridged() ?? new VentilationControlAbridged();

            if (!this.MinIndoorTemperature.IsVaries)
                obj.MinIndoorTemperature = this._refHBObj.MinIndoorTemperature;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing required VentilationControl schedule!");
                obj.Schedule = this._refHBObj.Schedule;
            }
            if (!this.MaxIndoorTemperature.IsVaries)
                obj.MaxIndoorTemperature = this._refHBObj.MaxIndoorTemperature;

            if (!this.MinOutdoorTemperature.IsVaries)
                obj.MinOutdoorTemperature = this._refHBObj.MinOutdoorTemperature;
            if (!this.MaxOutdoorTemperature.IsVaries)
                obj.MaxOutdoorTemperature = this._refHBObj.MaxOutdoorTemperature;
            if (!this.DeltaTemperature.IsVaries)
                obj.DeltaTemperature = this._refHBObj.DeltaTemperature;
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
