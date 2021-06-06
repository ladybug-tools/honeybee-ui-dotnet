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
            private set {
                this.Set(() => _flowPerPerson = value, nameof(FlowPerPerson)); 
            }
        }
        

        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            private set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // FlowPerArea
        private DoubleViewModel _flowPerArea;

        public DoubleViewModel FlowPerArea
        {
            get => _flowPerArea;
            private set { this.Set(() => _flowPerArea = value, nameof(FlowPerArea)); }
        }

        // AirChangesPerHour
        private DoubleViewModel _airChangesPerHour;

        public DoubleViewModel AirChangesPerHour
        {
            get => _airChangesPerHour;
            private set { this.Set(() => _airChangesPerHour = value, nameof(AirChangesPerHour)); }
        }

        // LostFraction
        private DoubleViewModel _flowPerZone;

        public DoubleViewModel FlowPerZone
        {
            get => _flowPerZone;
            private set { this.Set(() => _flowPerZone = value, nameof(FlowPerZone)); }
        }

        public VentilationAbridged Default { get; private set; }
        public VentilationViewModel(ModelProperties libSource, List<VentilationAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new VentilationAbridged(Guid.NewGuid().ToString());
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateVentilationAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateVentilationAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //FlowPerPerson
            this.FlowPerPerson = new DoubleViewModel((n) => _refHBObj.FlowPerPerson = n);
            if (loads.Select(_ => _?.FlowPerPerson).Distinct().Count() > 1)
                this.FlowPerPerson.SetNumberText(this.Varies);
            else
                this.FlowPerPerson.SetNumberText(_refHBObj.FlowPerPerson.ToString());


            //Schedule
            var sch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(this.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //FlowPerArea
            this.FlowPerArea = new DoubleViewModel((n) => _refHBObj.FlowPerArea = n);
            if (loads.Select(_ => _?.FlowPerArea).Distinct().Count() > 1)
                this.FlowPerArea.SetNumberText(this.Varies);
            else
                this.FlowPerArea.SetNumberText(_refHBObj.FlowPerArea.ToString());


            //AirChangesPerHour
            this.AirChangesPerHour = new DoubleViewModel((n) => _refHBObj.AirChangesPerHour = n);
            if (loads.Select(_ => _?.AirChangesPerHour).Distinct().Count() > 1)
                this.AirChangesPerHour.SetNumberText(this.Varies);
            else
                this.AirChangesPerHour.SetNumberText(_refHBObj.AirChangesPerHour.ToString());


            //LostFraction
            this.FlowPerZone = new DoubleViewModel((n) => _refHBObj.FlowPerZone = n);
            if (loads.Select(_ => _?.FlowPerZone).Distinct().Count() > 1)
                this.FlowPerZone.SetNumberText(this.Varies);
            else
                this.FlowPerZone.SetNumberText(_refHBObj.FlowPerZone.ToString());
        }

        public VentilationAbridged MatchObj(VentilationAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateVentilationAbridged() ?? new VentilationAbridged(Guid.NewGuid().ToString());

            if (!this.FlowPerPerson.IsVaries)
                obj.FlowPerPerson = this._refHBObj.FlowPerPerson;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing required schedule of the lighting load!");
                obj.Schedule = this._refHBObj.Schedule;
            }
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
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Schedule.SetPropetyObj(dialog_rc[0]);
            }
        });

    }


}
