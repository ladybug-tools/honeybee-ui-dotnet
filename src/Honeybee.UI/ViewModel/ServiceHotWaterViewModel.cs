using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class ServiceHotWaterViewModel : CheckboxPanelViewModel
    {
        private ServiceHotWaterAbridged _refHBObj => this.refObjProperty as ServiceHotWaterAbridged;
        // double flowPerArea, string schedule, string displayName = null, double targetTemperature = 60, double sensibleFraction = 0.2, double latentFraction = 0.05

        // FlowPerArea
        private DoubleViewModel _isFlowPerArea;

        public DoubleViewModel FlowPerArea
        {
            get => _isFlowPerArea;
            set {
                this.Set(() => _isFlowPerArea = value, nameof(FlowPerArea)); 
            }
        }
        private bool _flowPerAreaEnabled = true;
        public bool FlowPerAreaEnabled
        {
            get => _flowPerAreaEnabled;
            set { this.Set(() => _flowPerAreaEnabled = value, nameof(FlowPerAreaEnabled)); }
        }

        // FlowPerRoom
        private double _totalFlowPerRoom;
        private bool _FlowPerRoomEnabled;
        public bool FlowPerRoomEnabled
        {
            get => _FlowPerRoomEnabled;
            set
            {
                this.Set(() => _FlowPerRoomEnabled = value, nameof(FlowPerRoomEnabled));
                FlowPerAreaEnabled = !value;
            }
        }

        private DoubleViewModel _FlowPerRoom;
        public DoubleViewModel FlowPerRoom
        {
            get => _FlowPerRoom;
            set { this.Set(() => _FlowPerRoom = value, nameof(FlowPerRoom)); }
        }

        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // TargetTemperature
        private DoubleViewModel _isTargetTemperature;

        public DoubleViewModel TargetTemperature
        {
            get => _isTargetTemperature;
            set { this.Set(() => _isTargetTemperature = value, nameof(TargetTemperature)); }
        }

        // latentFraction
        private DoubleViewModel _isLatentFraction;

        public DoubleViewModel LatentFraction
        {
            get => _isLatentFraction;
            set { this.Set(() => _isLatentFraction = value, nameof(LatentFraction)); }
        }

        // SensibleFraction
        private DoubleViewModel _isSensibleFraction;

        public DoubleViewModel SensibleFraction
        {
            get => _isSensibleFraction;
            set { this.Set(() => _isSensibleFraction = value, nameof(SensibleFraction)); }
        }

        public ServiceHotWaterAbridged Default { get; private set; }
        public ServiceHotWaterViewModel(ModelProperties libSource, List<ServiceHotWaterAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new ServiceHotWaterAbridged(Guid.NewGuid().ToString(), 0, ReservedText.None);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateServiceHotWaterAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateServiceHotWaterAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.FlowPerArea = new DoubleViewModel((n) => _refHBObj.FlowPerArea = n);
            //this.FlowPerArea.SetUnits(Units.VolumeFlowPerAreaUnit.CubicMeterPerSecondPerSquareMeter, Units.UnitType.AirFlowRateArea);
            if (loads.Select(_ => _?.FlowPerArea).Distinct().Count() > 1)
                this.FlowPerArea.SetNumberText(ReservedText.Varies);
            else
                this.FlowPerArea.SetBaseUnitNumber(_refHBObj.FlowPerArea);


            //Schedule
            var sch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            sch = sch ?? GetDummyScheduleObj(_refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(ReservedText.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //TargetTemperature
            this.TargetTemperature = new DoubleViewModel((n) => _refHBObj.TargetTemperature = n);
            this.TargetTemperature.SetUnits(Units.TemperatureUnit.DegreeCelsius, Units.UnitType.Temperature);
            if (loads.Select(_ => _?.TargetTemperature).Distinct().Count() > 1)
                this.TargetTemperature.SetNumberText(ReservedText.Varies);
            else
                this.TargetTemperature.SetBaseUnitNumber(_refHBObj.TargetTemperature);


            //LatentFraction
            this.LatentFraction = new DoubleViewModel((n) => _refHBObj.LatentFraction = n);
            if (loads.Select(_ => _?.LatentFraction).Distinct().Count() > 1)
                this.LatentFraction.SetNumberText(ReservedText.Varies);
            else
                this.LatentFraction.SetNumberText(_refHBObj.LatentFraction.ToString());


            //LostFraction
            this.SensibleFraction = new DoubleViewModel((n) => _refHBObj.SensibleFraction = n);
            if (loads.Select(_ => _?.SensibleFraction).Distinct().Count() > 1)
                this.SensibleFraction.SetNumberText(ReservedText.Varies);
            else
                this.SensibleFraction.SetNumberText(_refHBObj.SensibleFraction.ToString());
        }

        public ServiceHotWaterViewModel(
            ModelProperties libSource,
            List<ServiceHotWaterAbridged> loads,
            Action<IIDdBase> setAction,
            IEnumerable<double> areas) : this(libSource, loads, setAction)
        {
            if (areas == default)
                throw new ArgumentNullException(nameof(areas));
            if (areas.Count() != loads.Count)
                throw new ArgumentException($"The area list doesn't have the same length of the load list");


            //FlowPerRoom
            this.FlowPerRoom = new DoubleViewModel((n) => _totalFlowPerRoom = n);
            //this.FlowPerRoom.SetUnits(Units.VolumeFlowUnit.CubicMeterPerSecond, Units.UnitType.Power);
            var FlowPerRooms = loads.Zip(areas, (l, a) => a * (l?.FlowPerArea).GetValueOrDefault());
            if (FlowPerRooms.Distinct().Count() > 1)
                this.FlowPerRoom.SetNumberText(ReservedText.Varies);
            else
                this.FlowPerRoom.SetBaseUnitNumber(FlowPerRooms.FirstOrDefault());

        }

        public ServiceHotWaterAbridged MatchObj(ServiceHotWaterAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateServiceHotWaterAbridged() ?? new ServiceHotWaterAbridged(Guid.NewGuid().ToString(), 0, "Not Set");

            if (!this.FlowPerArea.IsVaries)
                obj.FlowPerArea = this._refHBObj.FlowPerArea;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing a required schedule of the service hot water load!");
                obj.Schedule = this._refHBObj.Schedule;
            }
            if (!this.TargetTemperature.IsVaries)
                obj.TargetTemperature = this._refHBObj.TargetTemperature;

            if (!this.LatentFraction.IsVaries)
                obj.LatentFraction = this._refHBObj.LatentFraction;
            if (!this.SensibleFraction.IsVaries)
                obj.SensibleFraction = this._refHBObj.SensibleFraction;
            return obj;
        }
        public ServiceHotWaterAbridged MatchObj(ServiceHotWaterAbridged obj, Room room)
        {
            var checkedObj = MatchObj(obj);
            if (this.FlowPerAreaEnabled)
                return checkedObj;

            if (this.FlowPerRoom == null || this.FlowPerRoom.IsVaries)
                return checkedObj;

            var area = room.CalArea();
            checkedObj.FlowPerArea = area > 0 ? this._totalFlowPerRoom / area : 0;
            return checkedObj;

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
