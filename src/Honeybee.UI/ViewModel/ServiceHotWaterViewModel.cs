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
            private set {
                this.Set(() => _isFlowPerArea = value, nameof(FlowPerArea)); 
            }
        }
        

        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            private set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // TargetTemperature
        private DoubleViewModel _isTargetTemperature;

        public DoubleViewModel TargetTemperature
        {
            get => _isTargetTemperature;
            private set { this.Set(() => _isTargetTemperature = value, nameof(TargetTemperature)); }
        }

        // latentFraction
        private DoubleViewModel _isLatentFraction;

        public DoubleViewModel LatentFraction
        {
            get => _isLatentFraction;
            private set { this.Set(() => _isLatentFraction = value, nameof(LatentFraction)); }
        }

        // SensibleFraction
        private DoubleViewModel _isSensibleFraction;

        public DoubleViewModel SensibleFraction
        {
            get => _isSensibleFraction;
            private set { this.Set(() => _isSensibleFraction = value, nameof(SensibleFraction)); }
        }

        public ServiceHotWaterAbridged Default { get; private set; }
        public ServiceHotWaterViewModel(ModelProperties libSource, List<ServiceHotWaterAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new ServiceHotWaterAbridged(Guid.NewGuid().ToString(), 0, "");
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateServiceHotWaterAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateServiceHotWaterAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.FlowPerArea = new DoubleViewModel((n) => _refHBObj.FlowPerArea = n);
            if (loads.Select(_ => _?.FlowPerArea).Distinct().Count() > 1)
                this.FlowPerArea.SetNumberText(this.Varies);
            else
                this.FlowPerArea.SetNumberText(_refHBObj.FlowPerArea.ToString());


            //Schedule
            var sch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(this.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //RadiantFraction
            this.TargetTemperature = new DoubleViewModel((n) => _refHBObj.TargetTemperature = n);
            if (loads.Select(_ => _?.TargetTemperature).Distinct().Count() > 1)
                this.TargetTemperature.SetNumberText(this.Varies);
            else
                this.TargetTemperature.SetNumberText(_refHBObj.TargetTemperature.ToString());


            //LatentFraction
            this.LatentFraction = new DoubleViewModel((n) => _refHBObj.LatentFraction = n);
            if (loads.Select(_ => _?.LatentFraction).Distinct().Count() > 1)
                this.LatentFraction.SetNumberText(this.Varies);
            else
                this.LatentFraction.SetNumberText(_refHBObj.LatentFraction.ToString());


            //LostFraction
            this.SensibleFraction = new DoubleViewModel((n) => _refHBObj.SensibleFraction = n);
            if (loads.Select(_ => _?.SensibleFraction).Distinct().Count() > 1)
                this.SensibleFraction.SetNumberText(this.Varies);
            else
                this.SensibleFraction.SetNumberText(_refHBObj.SensibleFraction.ToString());
        }

        public ServiceHotWaterAbridged MatchObj(ServiceHotWaterAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateServiceHotWaterAbridged() ?? new ServiceHotWaterAbridged(Guid.NewGuid().ToString(), 0, "");

            if (!this.FlowPerArea.IsVaries)
                obj.FlowPerArea = this._refHBObj.FlowPerArea;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing required schedule of the lighting load!");
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
