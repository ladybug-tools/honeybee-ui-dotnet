using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class LightingViewModel : CheckboxPanelViewModel
    {
        private LightingAbridged _refHBObj => this.refObjProperty as LightingAbridged;

        // WattsPerArea
        private bool _wattsPerAreaEnabled = true;
        public bool WattsPerAreaEnabled
        {
            get => _wattsPerAreaEnabled;
            set { this.Set(() => _wattsPerAreaEnabled = value, nameof(WattsPerAreaEnabled)); }
        }

        private DoubleViewModel _wattsPerArea;
        public DoubleViewModel WattsPerArea
        {
            get => _wattsPerArea;
            set { this.Set(() => _wattsPerArea = value, nameof(WattsPerArea)); }
        }

        // WattsPerRoom
        private double _totalWattsPerRoom;
        private bool _wattsPerRoomEnabled;
        public bool WattsPerRoomEnabled
        {
            get => _wattsPerRoomEnabled;
            set 
            { 
                this.Set(() => _wattsPerRoomEnabled = value, nameof(WattsPerRoomEnabled));
                WattsPerAreaEnabled = !value;
            }
        }

        private DoubleViewModel _wattsPerRoom;
        public DoubleViewModel WattsPerRoom
        {
            get => _wattsPerRoom;
            set { this.Set(() => _wattsPerRoom = value, nameof(WattsPerRoom)); }
        }


        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // RadiantFractionText
        private DoubleViewModel _radiantFraction;

        public DoubleViewModel RadiantFraction
        {
            get => _radiantFraction;
            set { this.Set(() => _radiantFraction = value, nameof(RadiantFraction)); }
        }

        // VisibleFraction
        private DoubleViewModel _visibleFraction;

        public DoubleViewModel VisibleFraction
        {
            get => _visibleFraction;
            set { this.Set(() => _visibleFraction = value, nameof(VisibleFraction)); }
        }

        // BaselineWattsPerArea
        private DoubleViewModel _baselineWattsPerArea;

        public DoubleViewModel BaselineWattsPerArea
        {
            get => _baselineWattsPerArea;
            set { this.Set(() => _baselineWattsPerArea = value, nameof(BaselineWattsPerArea)); }
        }

        // ReturnAirFraction
        private DoubleViewModel _returnAirFraction;

        public DoubleViewModel ReturnAirFraction
        {
            get => _returnAirFraction;
            set { this.Set(() => _returnAirFraction = value, nameof(ReturnAirFraction)); }
        }

        public LightingAbridged Default { get; private set; }

        public LightingViewModel(ModelProperties libSource, List<LightingAbridged> lights, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new LightingAbridged(Guid.NewGuid().ToString(), 0, ReservedText.None);
            this.refObjProperty = lights.FirstOrDefault()?.DuplicateLightingAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateLightingAbridged();



            if (lights.Distinct().Count() == 1 && lights.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.WattsPerArea = new DoubleViewModel((n) => _refHBObj.WattsPerArea = n);
            this.WattsPerArea.SetUnits(Units.HeatFluxUnit.WattPerSquareMeter, Units.UnitType.PowerDensity);
            if (lights.Select(_ => _?.WattsPerArea).Distinct().Count() > 1)
                this.WattsPerArea.SetNumberText(ReservedText.Varies);
            else
                this.WattsPerArea.SetBaseUnitNumber(_refHBObj.WattsPerArea);
            

            //Schedule
            var sch = libSource.Energy.ScheduleList
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            sch = sch ?? GetDummyScheduleObj(_refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (lights.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(ReservedText.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //RadiantFraction
            this.RadiantFraction = new DoubleViewModel((n) => _refHBObj.RadiantFraction = n);
            if (lights.Select(_ => _?.RadiantFraction).Distinct().Count() > 1)
                this.RadiantFraction.SetNumberText(ReservedText.Varies);
            else
                this.RadiantFraction.SetNumberText(_refHBObj.RadiantFraction.ToString());


            //VisibleFraction
            this.VisibleFraction = new DoubleViewModel((n) => _refHBObj.VisibleFraction = n);
            if (lights.Select(_ => _?.VisibleFraction).Distinct().Count() > 1)
                this.VisibleFraction.SetNumberText(ReservedText.Varies);
            else
                this.VisibleFraction.SetNumberText(_refHBObj.VisibleFraction.ToString());


            //ReturnAirFraction
            this.ReturnAirFraction = new DoubleViewModel((n) => _refHBObj.ReturnAirFraction = n);
            if (lights.Select(_ => _?.ReturnAirFraction).Distinct().Count() > 1)
                this.ReturnAirFraction.SetNumberText(ReservedText.Varies);
            else
                this.ReturnAirFraction.SetNumberText(_refHBObj.ReturnAirFraction.ToString());


            //BaselineWattsPerArea
            this.BaselineWattsPerArea = new DoubleViewModel((n) => _refHBObj.BaselineWattsPerArea = n);
            this.BaselineWattsPerArea.SetUnits(Units.HeatFluxUnit.WattPerSquareMeter, Units.UnitType.PowerDensity);
            if (lights.Select(_ => _?.BaselineWattsPerArea).Distinct().Count() > 1)
                this.BaselineWattsPerArea.SetNumberText(ReservedText.Varies);
            else
                this.BaselineWattsPerArea.SetBaseUnitNumber(_refHBObj.BaselineWattsPerArea);
        }


        public LightingViewModel(
            ModelProperties libSource, 
            List<LightingAbridged> loads, 
            Action<IIDdBase> setAction, 
            IEnumerable<double> areas) : this(libSource, loads, setAction)
        {
            if (areas == default)
                throw new ArgumentNullException(nameof(areas));
            if (areas.Count() != loads.Count)
                throw new ArgumentException($"The area list doesn't have the same length of the load list");


            //WattsPerRoom
            this.WattsPerRoom = new DoubleViewModel((n) => _totalWattsPerRoom = n);
            this.WattsPerRoom.SetUnits(Units.PowerUnit.Watt, Units.UnitType.Power);
            var wattsPerRooms = loads.Zip(areas, (l, a) => a * (l?.WattsPerArea).GetValueOrDefault());
            if (wattsPerRooms.Distinct().Count() > 1)
                this.WattsPerRoom.SetNumberText(ReservedText.Varies);
            else
                this.WattsPerRoom.SetBaseUnitNumber(wattsPerRooms.FirstOrDefault());

        }

        public LightingAbridged MatchObj(LightingAbridged obj)
        {
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateLightingAbridged() ?? new LightingAbridged(Guid.NewGuid().ToString(), 0, "Not Set");

            if (!this.WattsPerArea.IsVaries)
                obj.WattsPerArea = this._refHBObj.WattsPerArea;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing a required schedule of the lighting load!");
                obj.Schedule = this._refHBObj.Schedule;
            }
            if (!this.RadiantFraction.IsVaries)
                obj.RadiantFraction = this._refHBObj.RadiantFraction;

            if (!this.VisibleFraction.IsVaries)
                obj.VisibleFraction = this._refHBObj.VisibleFraction;
            if (!this.ReturnAirFraction.IsVaries)
                obj.ReturnAirFraction = this._refHBObj.ReturnAirFraction;
            if (!this.BaselineWattsPerArea.IsVaries)
                obj.BaselineWattsPerArea = this._refHBObj.BaselineWattsPerArea;
            return obj;
        }


        public LightingAbridged MatchObj(LightingAbridged obj, Room room)
        {
            var checkedObj = MatchObj(obj);
            if (this.WattsPerAreaEnabled)
                return checkedObj;

            if (this.WattsPerRoom == null || this.WattsPerRoom.IsVaries)
                return checkedObj;

            var area = room.CalArea();
            checkedObj.WattsPerArea = area > 0 ? this._totalWattsPerRoom / area : 0;
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
