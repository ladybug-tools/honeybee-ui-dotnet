using Eto.Forms;
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
            set {
                this.Set(() => _wattsPerArea = value, nameof(WattsPerArea)); 
            }
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
        public ElectricEquipmentAbridged Default { get; private set; }

        public ElecEquipmentViewModel(ModelProperties libSource, List<ElectricEquipmentAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new ElectricEquipmentAbridged(Guid.NewGuid().ToString(), 0, ReservedText.None);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateElectricEquipmentAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateElectricEquipmentAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.WattsPerArea = new DoubleViewModel((n) => _refHBObj.WattsPerArea = n);
            this.WattsPerArea.SetUnits(Units.HeatFluxUnit.WattPerSquareMeter, Units.UnitType.PowerDensity);
            if (loads.Select(_ => _?.WattsPerArea).Distinct().Count() > 1)
                this.WattsPerArea.SetNumberText(ReservedText.Varies);
            else
                this.WattsPerArea.SetBaseUnitNumber(_refHBObj.WattsPerArea);


            //Schedule
            var sch = libSource.Energy.ScheduleList
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
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

        public ElecEquipmentViewModel(
            ModelProperties libSource, 
            List<ElectricEquipmentAbridged> loads, 
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
                    throw new ArgumentException("Missing a required electric equipment schedule!");
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


        public ElectricEquipmentAbridged MatchObj(ElectricEquipmentAbridged obj, Room room)
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
