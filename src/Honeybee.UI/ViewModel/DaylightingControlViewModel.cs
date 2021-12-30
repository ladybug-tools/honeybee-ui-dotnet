using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class DaylightingControlViewModel : CheckboxPanelViewModel<DaylightingControl>
    {
        private DaylightingControl _refHBObj => this.refObjProperty;
        // List<double> sensorPosition, double illuminanceSetpoint = 300, double controlFraction = 1, double minPowerInput = 0.3, double minLightOutput = 0.2, bool offAtMinimum = false

        // illuminanceSetpoint
        private DoubleViewModel _illuminanceSetpoint;

        public DoubleViewModel IlluminanceSetpoint
        {
            get => _illuminanceSetpoint;
            private set {
                this.Set(() => _illuminanceSetpoint = value, nameof(IlluminanceSetpoint)); 
            }
        }


        // sensorPosition
        private ButtonViewModel<List<double>> _sensorPosition;

        public ButtonViewModel<List<double>> SensorPosition
        {
            get => _sensorPosition;
            private set { this.Set(() => _sensorPosition = value, nameof(SensorPosition)); }
        }

        // controlFraction
        private DoubleViewModel _controlFraction;

        public DoubleViewModel ControlFraction
        {
            get => _controlFraction;
            private set { this.Set(() => _controlFraction = value, nameof(ControlFraction)); }
        }

        // minPowerInput
        private DoubleViewModel _minPowerInput;

        public DoubleViewModel MinPowerInput
        {
            get => _minPowerInput;
            private set { this.Set(() => _minPowerInput = value, nameof(MinPowerInput)); }
        }

        // minLightOutput
        private DoubleViewModel _minLightOutput;

        public DoubleViewModel MinLightOutput
        {
            get => _minLightOutput;
            private set { this.Set(() => _minLightOutput = value, nameof(MinLightOutput)); }
        }

        // OffAtMinimum
        private CheckboxViewModel _offAtMinimum;

        public CheckboxViewModel OffAtMinimum
        {
            get => _offAtMinimum;
            private set => this.Set(() => _offAtMinimum = value, nameof(OffAtMinimum)); 
        }

        public Func<List<double>> SensorPositionPicker { get; set; }
      

        public bool EnableSensorPositionPicker
        {
            get => SensorPositionPicker != null;
            set => this.RefreshControl(nameof(EnableSensorPositionPicker));
        }
        public DaylightingControl Default { get; private set; }
      

        public DaylightingControlViewModel(ModelProperties libSource, List<DaylightingControl> loads, Action<DaylightingControl> setAction) :base(libSource, setAction)
        {
            this.Default = new DaylightingControl(new List<double>());
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateDaylightingControl();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateDaylightingControl();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //IlluminanceSetpoint
            this.IlluminanceSetpoint = new DoubleViewModel((n) => _refHBObj.IlluminanceSetpoint = n);
            this.IlluminanceSetpoint.SetUnits(Units.IlluminanceUnit.Lux, Units.UnitType.Illuminance);
            if (loads.Select(_ => _?.IlluminanceSetpoint).Distinct().Count() > 1)
                this.IlluminanceSetpoint.SetNumberText(this.Varies);
            else
                this.IlluminanceSetpoint.SetBaseUnitNumber(_refHBObj.IlluminanceSetpoint);


            //SensorPosition
            this.SensorPosition = new ButtonViewModel<List<double>>((n) => _refHBObj.SensorPosition = n);
            if (loads.Select(_ => _?.SensorPosition).Distinct().Count() > 1)
                this.SensorPosition.SetBtnName(this.Varies);
            else
                this.SensorPosition.SetPropetyObj(_refHBObj.SensorPosition);


            //ControlFraction
            this.ControlFraction = new DoubleViewModel((n) => _refHBObj.ControlFraction = n);
            if (loads.Select(_ => _?.ControlFraction).Distinct().Count() > 1)
                this.ControlFraction.SetNumberText(this.Varies);
            else
                this.ControlFraction.SetNumberText(_refHBObj.ControlFraction.ToString());


            //MinPowerInput
            this.MinPowerInput = new DoubleViewModel((n) => _refHBObj.MinPowerInput = n);
            this.MinPowerInput.SetUnits(Units.PowerUnit.Watt, Units.UnitType.Power);
            if (loads.Select(_ => _?.MinPowerInput).Distinct().Count() > 1)
                this.MinPowerInput.SetNumberText(this.Varies);
            else
                this.MinPowerInput.SetBaseUnitNumber(_refHBObj.MinPowerInput);


            //MinLightOutput
            this.MinLightOutput = new DoubleViewModel((n) => _refHBObj.MinLightOutput = n);
            if (loads.Select(_ => _?.MinLightOutput).Distinct().Count() > 1)
                this.MinLightOutput.SetNumberText(this.Varies);
            else
                this.MinLightOutput.SetNumberText(_refHBObj.MinLightOutput.ToString());


            //OffAtMinimum
            this.OffAtMinimum = new CheckboxViewModel(_ => _refHBObj.OffAtMinimum = _);
            if (loads.Select(_ => _?.OffAtMinimum).Distinct().Count() > 1)
                this.OffAtMinimum.SetCheckboxVaries();
            else
                this.OffAtMinimum.SetCheckboxChecked(_refHBObj.OffAtMinimum);
        }

        public DaylightingControl MatchObj(DaylightingControl obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateDaylightingControl() ?? new DaylightingControl(new List<double>());

            if (!this.IlluminanceSetpoint.IsVaries)
                obj.IlluminanceSetpoint = this._refHBObj.IlluminanceSetpoint;
            if (!this.SensorPosition.IsVaries)
            {
                if (this._refHBObj.SensorPosition == null || this._refHBObj.SensorPosition.Count != 3)
                    throw new ArgumentException("Missing required DaylightingControl sensor position!");
                obj.SensorPosition = this._refHBObj.SensorPosition;
            }
            if (!this.ControlFraction.IsVaries)
                obj.ControlFraction = this._refHBObj.ControlFraction;

            if (!this.MinPowerInput.IsVaries)
                obj.MinPowerInput = this._refHBObj.MinPowerInput;
            if (!this.MinLightOutput.IsVaries)
                obj.MinLightOutput = this._refHBObj.MinLightOutput;
            if (!this.OffAtMinimum.IsVaries)
                obj.OffAtMinimum = this._refHBObj.OffAtMinimum;
            return obj;
        }
      
        public RelayCommand SensorPositionCommand => new RelayCommand(() =>
        {
            var pts = this.SensorPositionPicker?.Invoke();
            if (pts != null && pts.Count == 3)
            {
                this.SensorPosition.SetPropetyObj(pts);
            }
            
        });

    }

   


}
