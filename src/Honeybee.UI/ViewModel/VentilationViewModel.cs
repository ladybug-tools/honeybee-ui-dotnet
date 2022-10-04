using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class VentilationViewModel : CheckboxPanelViewModel
    {
        private VentilationAbridged _refHBObj => this.refObjProperty as VentilationAbridged;
        // double flowPerPerson = 0, double flowPerArea = 0, double airChangesPerHour = 0, double flowPerZone = 0, string schedule = null

        // FlowPerPerson
        private DoubleViewModel _flowPerPerson;

        public DoubleViewModel FlowPerPerson
        {
            get => _flowPerPerson;
            set {
                this.Set(() => _flowPerPerson = value, nameof(FlowPerPerson)); 
            }
        }
        

        // Schedule
        private OptionalButtonViewModel _schedule;

        public OptionalButtonViewModel Schedule
        {
            get => _schedule;
            set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // FlowPerArea
        private DoubleViewModel _flowPerArea;

        public DoubleViewModel FlowPerArea
        {
            get => _flowPerArea;
            set { this.Set(() => _flowPerArea = value, nameof(FlowPerArea)); }
        }

        // AirChangesPerHour
        private DoubleViewModel _airChangesPerHour;

        public DoubleViewModel AirChangesPerHour
        {
            get => _airChangesPerHour;
            set { this.Set(() => _airChangesPerHour = value, nameof(AirChangesPerHour)); }
        }

        // LostFraction
        private DoubleViewModel _flowPerZone;

        public DoubleViewModel FlowPerZone
        {
            get => _flowPerZone;
            set { this.Set(() => _flowPerZone = value, nameof(FlowPerZone)); }
        }

        public VentilationAbridged Default { get; private set; }
        public VentilationViewModel(ModelProperties libSource, List<VentilationAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new VentilationAbridged(Guid.NewGuid().ToString());
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateVentilationAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateVentilationAbridged();


            if (loads.Distinct().Count() == 1) 
                this.IsCheckboxChecked = loads.FirstOrDefault() == null;
            else
                this.IsCheckboxVaries();


            //FlowPerPerson
            this.FlowPerPerson = new DoubleViewModel((n) => _refHBObj.FlowPerPerson = n);
            this.FlowPerPerson.SetUnits(Units.VolumeFlowUnit.CubicMeterPerSecond, Units.UnitType.AirFlowRate);
            if (loads.Select(_ => _?.FlowPerPerson).Distinct().Count() > 1)
                this.FlowPerPerson.SetNumberText(ReservedText.Varies);
            else
                this.FlowPerPerson.SetBaseUnitNumber(_refHBObj.FlowPerPerson);


            //Schedule
            var sch = libSource.Energy.ScheduleList.FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            sch = sch ?? GetDummyScheduleObj(_refHBObj.Schedule);
            this.Schedule = new OptionalButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(ReservedText.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //FlowPerArea
            this.FlowPerArea = new DoubleViewModel((n) => _refHBObj.FlowPerArea = n);
            this.FlowPerArea.SetUnits(Units.VolumeFlowPerAreaUnit.CubicMeterPerSecondPerSquareMeter, Units.UnitType.AirFlowRateArea);
            if (loads.Select(_ => _?.FlowPerArea).Distinct().Count() > 1)
                this.FlowPerArea.SetNumberText(ReservedText.Varies);
            else
                this.FlowPerArea.SetBaseUnitNumber(_refHBObj.FlowPerArea);


            //AirChangesPerHour
            this.AirChangesPerHour = new DoubleViewModel((n) => _refHBObj.AirChangesPerHour = n);
            this.AirChangesPerHour.SetDisplayUnitAbbreviation("1/hour");
            if (loads.Select(_ => _?.AirChangesPerHour).Distinct().Count() > 1)
                this.AirChangesPerHour.SetNumberText(ReservedText.Varies);
            else
                this.AirChangesPerHour.SetNumberText(_refHBObj.AirChangesPerHour.ToString());


            //FlowPerZone
            this.FlowPerZone = new DoubleViewModel((n) => _refHBObj.FlowPerZone = n);
            this.FlowPerZone.SetUnits(Units.VolumeFlowUnit.CubicMeterPerSecond, Units.UnitType.AirFlowRate);
            if (loads.Select(_ => _?.FlowPerZone).Distinct().Count() > 1)
                this.FlowPerZone.SetNumberText(ReservedText.Varies);
            else
                this.FlowPerZone.SetBaseUnitNumber(_refHBObj.FlowPerZone);
        }

        public VentilationAbridged MatchObj(VentilationAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked.GetValueOrDefault())
                return null;

            if (this.IsVaries)
                return obj?.DuplicateVentilationAbridged();

            obj = obj?.DuplicateVentilationAbridged() ?? new VentilationAbridged(Guid.NewGuid().ToString());

            if (!this.FlowPerPerson.IsVaries)
                obj.FlowPerPerson = this._refHBObj.FlowPerPerson;
            if (!this.Schedule.IsVaries)
                obj.Schedule = this._refHBObj.Schedule;
            if (!this.FlowPerArea.IsVaries)
                obj.FlowPerArea = this._refHBObj.FlowPerArea;

            if (!this.AirChangesPerHour.IsVaries)
                obj.AirChangesPerHour = this._refHBObj.AirChangesPerHour;
            if (!this.FlowPerZone.IsVaries)
                obj.FlowPerZone = this._refHBObj.FlowPerZone;
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

        public RelayCommand RemoveScheduleCommand => new RelayCommand(() =>
        {
            this.Schedule.SetPropetyObj(null);
        });
    }


}
