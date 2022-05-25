using Eto.Forms;
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
        private bool _peoplePerAreaEnabled = true;
        public bool PeoplePerAreaEnabled
        {
            get => _peoplePerAreaEnabled;
            set { this.Set(() => _peoplePerAreaEnabled = value, nameof(PeoplePerAreaEnabled)); }
        }

        private DoubleViewModel _peoplePerArea;

        public DoubleViewModel PeoplePerArea
        {
            get => _peoplePerArea;
            set {
                this.Set(() => _peoplePerArea = value, nameof(PeoplePerArea)); 
            }
        }
        
        // WattsPerRoom
        private double _totalPeoplePerRoom;
        private bool _peoplePerRoomEnabled;
        public bool PeoplePerRoomEnabled
        {
            get => _peoplePerRoomEnabled;
            set
            {
                this.Set(() => _peoplePerRoomEnabled = value, nameof(PeoplePerRoomEnabled));
                PeoplePerAreaEnabled = !value;
            }
        }

        private DoubleViewModel _peoplePerRoom;
        public DoubleViewModel PeoplePerRoom
        {
            get => _peoplePerRoom;
            set { this.Set(() => _peoplePerRoom = value, nameof(PeoplePerRoom)); }
        }


        // OccupancySchedule
        private ButtonViewModel _occupancySchedule;

        public ButtonViewModel OccupancySchedule
        {
            get => _occupancySchedule;
            set { this.Set(() => _occupancySchedule = value, nameof(OccupancySchedule)); }
        }

        // activitySchedule
        private OptionalButtonViewModel _activitySchedule;

        public OptionalButtonViewModel ActivitySchedule
        {
            get => _activitySchedule;
            set { this.Set(() => _activitySchedule = value, nameof(ActivitySchedule)); }
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

        private bool _isLatentFractionAutocalculate;

        public bool IsLatentFractionAutocalculate
        {
            get => _isLatentFractionAutocalculate;
            set {

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
            set { this.Set(() => _isLatenFractionInputEnabled = value, nameof(IsLatenFractionInputEnabled)); }
        }



        public PeopleAbridged Default { get; private set; }
        public PeopleViewModel(ModelProperties libSource, List<PeopleAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new PeopleAbridged(Guid.NewGuid().ToString(), 0, ReservedText.None);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicatePeopleAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicatePeopleAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.PeoplePerArea = new DoubleViewModel((n) => _refHBObj.PeoplePerArea = n);
            this.PeoplePerArea.SetUnits(Units.ReciprocalAreaUnit.InverseSquareMeter, Units.UnitType.PeopleDensity);
            if (loads.Select(_ => _?.PeoplePerArea).Distinct().Count() > 1)
                this.PeoplePerArea.SetNumberText(ReservedText.Varies);
            else
                this.PeoplePerArea.SetBaseUnitNumber(_refHBObj.PeoplePerArea);


            //Schedule
            var sch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.OccupancySchedule);
            sch = sch ?? GetDummyScheduleObj(_refHBObj.OccupancySchedule);
            this.OccupancySchedule = new ButtonViewModel((n) => _refHBObj.OccupancySchedule = n?.Identifier);
            if (loads.Select(_ => _?.OccupancySchedule).Distinct().Count() > 1)
                this.OccupancySchedule.SetBtnName(ReservedText.Varies);
            else
                this.OccupancySchedule.SetPropetyObj(sch);


            //ActivitySchedule
            var actSch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.ActivitySchedule);
            actSch = actSch ?? GetDummyScheduleObj(_refHBObj.ActivitySchedule);
            this.ActivitySchedule = new OptionalButtonViewModel((n) => _refHBObj.ActivitySchedule = n?.Identifier);
            if (loads.Select(_ => _?.ActivitySchedule).Distinct().Count() > 1)
                this.ActivitySchedule.SetBtnName(ReservedText.Varies);
            else
                this.ActivitySchedule.SetPropetyObj(actSch);



            //RadiantFraction
            this.RadiantFraction = new DoubleViewModel((n) => _refHBObj.RadiantFraction = n);
            if (loads.Select(_ => _?.RadiantFraction).Distinct().Count() > 1)
                this.RadiantFraction.SetNumberText(ReservedText.Varies);
            else
                this.RadiantFraction.SetNumberText(_refHBObj.RadiantFraction.ToString());


            //LatentFraction
            this.LatentFraction = new DoubleViewModel((n) => _refHBObj.LatentFraction = n);
            var latFractions = loads.Select(_ => _?.LatentFraction).Distinct();
            if (latFractions.Count() > 1)
            {
                this.IsLatentFractionAutocalculate = false;
                this.LatentFraction.SetNumberText(ReservedText.Varies);
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

        public PeopleViewModel(
         ModelProperties libSource,
         List<PeopleAbridged> loads,
         Action<IIDdBase> setAction,
         IEnumerable<double> areas) : this(libSource, loads, setAction)
        {
            if (areas == default)
                throw new ArgumentNullException(nameof(areas));
            if (areas.Count() != loads.Count)
                throw new ArgumentException($"The area list doesn't have the same length of the load list");


            //WattsPerRoom
            this.PeoplePerRoom = new DoubleViewModel((n) => _totalPeoplePerRoom = n);
            //this.PeoplePerRoom.SetUnits(Units..PowerUnit.Watt, Units.UnitType.Power);
            var wattsPerRooms = loads.Zip(areas, (l, a) => a * (l?.PeoplePerArea).GetValueOrDefault());
            if (wattsPerRooms.Distinct().Count() > 1)
                this.PeoplePerRoom.SetNumberText(ReservedText.Varies);
            else
                this.PeoplePerRoom.SetBaseUnitNumber(wattsPerRooms.FirstOrDefault());

        }

        public PeopleAbridged MatchObj(PeopleAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicatePeopleAbridged() ?? new PeopleAbridged(Guid.NewGuid().ToString(), 0, "Not Set");

            if (!this.PeoplePerArea.IsVaries)
                obj.PeoplePerArea = this._refHBObj.PeoplePerArea;
            if (!this.OccupancySchedule.IsVaries)
            {
                if (this._refHBObj.OccupancySchedule == null)
                    throw new ArgumentException("Missing a required occupancy schedule of the people load!");
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
        public PeopleAbridged MatchObj(PeopleAbridged obj, Room room)
        {
            var checkedObj = MatchObj(obj);
            if (this.PeoplePerAreaEnabled)
                return checkedObj;

            if (this.PeoplePerRoom == null || this.PeoplePerRoom.IsVaries)
                return checkedObj;

            var area = room.CalArea();
            checkedObj.PeoplePerArea = area > 0 ? this._totalPeoplePerRoom / area : 0;
            return checkedObj;

        }

        public RelayCommand ScheduleCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.OccupancySchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand ActivityScheduleCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.ActivitySchedule.SetPropetyObj(dialog_rc[0]);
            }
        });
        public RelayCommand RemoveActivityScheduleCommand => new RelayCommand(() =>
        {
            this.ActivitySchedule.SetPropetyObj(null);
        });

    }


}
