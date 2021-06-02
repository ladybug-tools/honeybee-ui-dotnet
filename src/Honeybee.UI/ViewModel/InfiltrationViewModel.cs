using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class InfiltrationViewModel : CheckboxPanelViewModel
    {
        private InfiltrationAbridged _refHBObj => this.refObjProperty as InfiltrationAbridged;
        // double flowPerExteriorArea, string schedule,  double constantCoefficient = 1, double temperatureCoefficient = 0, double velocityCoefficient = 0

        // FlowPerExteriorArea
        private DoubleViewModel _flowPerExteriorArea;

        public DoubleViewModel FlowPerExteriorArea
        {
            get => _flowPerExteriorArea;
            private set {
                this.Set(() => _flowPerExteriorArea = value, nameof(FlowPerExteriorArea)); 
            }
        }
        

        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            private set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // ConstantCoefficient
        private DoubleViewModel _constantCoefficient;

        public DoubleViewModel ConstantCoefficient
        {
            get => _constantCoefficient;
            private set { this.Set(() => _constantCoefficient = value, nameof(ConstantCoefficient)); }
        }

        // TemperatureCoefficient
        private DoubleViewModel _temperatureCoefficient;

        public DoubleViewModel TemperatureCoefficient
        {
            get => _temperatureCoefficient;
            private set { this.Set(() => _temperatureCoefficient = value, nameof(TemperatureCoefficient)); }
        }

        // velocityCoefficient
        private DoubleViewModel _velocityCoefficient;

        public DoubleViewModel VelocityCoefficient
        {
            get => _velocityCoefficient;
            private set { this.Set(() => _velocityCoefficient = value, nameof(VelocityCoefficient)); }
        }

     
        public InfiltrationViewModel(ModelProperties libSource, List<InfiltrationAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {

            this.refObjProperty = loads.FirstOrDefault()?.DuplicateInfiltrationAbridged();
            this.refObjProperty = this._refHBObj ?? new InfiltrationAbridged(Guid.NewGuid().ToString(), 0, "");


            if (loads.Count == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //FlowPerExteriorArea
            this.FlowPerExteriorArea = new DoubleViewModel((n) => _refHBObj.FlowPerExteriorArea = n);
            if (loads.Select(_ => _?.FlowPerExteriorArea).Distinct().Count() > 1)
                this.FlowPerExteriorArea.SetNumberText(this.Varies);
            else
                this.FlowPerExteriorArea.SetNumberText(_refHBObj.FlowPerExteriorArea.ToString());


            //Schedule
            var sch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (loads.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(this.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //ConstantCoefficient
            this.ConstantCoefficient = new DoubleViewModel((n) => _refHBObj.ConstantCoefficient = n);
            if (loads.Select(_ => _?.ConstantCoefficient).Distinct().Count() > 1)
                this.ConstantCoefficient.SetNumberText(this.Varies);
            else
                this.ConstantCoefficient.SetNumberText(_refHBObj.ConstantCoefficient.ToString());


            //TemperatureCoefficient
            this.TemperatureCoefficient = new DoubleViewModel((n) => _refHBObj.TemperatureCoefficient = n);
            if (loads.Select(_ => _?.TemperatureCoefficient).Distinct().Count() > 1)
                this.TemperatureCoefficient.SetNumberText(this.Varies);
            else
                this.TemperatureCoefficient.SetNumberText(_refHBObj.TemperatureCoefficient.ToString());


            //VelocityCoefficient
            this.VelocityCoefficient = new DoubleViewModel((n) => _refHBObj.VelocityCoefficient = n);
            if (loads.Select(_ => _?.VelocityCoefficient).Distinct().Count() > 1)
                this.VelocityCoefficient.SetNumberText(this.Varies);
            else
                this.VelocityCoefficient.SetNumberText(_refHBObj.VelocityCoefficient.ToString());
        }

        public InfiltrationAbridged MatchObj(InfiltrationAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateInfiltrationAbridged() ?? new InfiltrationAbridged(Guid.NewGuid().ToString(), 0, "");

            if (!this.FlowPerExteriorArea.IsVaries)
                obj.FlowPerExteriorArea = this._refHBObj.FlowPerExteriorArea;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing required infiltration schedule!");
                obj.Schedule = this._refHBObj.Schedule;
            }
            if (!this.ConstantCoefficient.IsVaries)
                obj.ConstantCoefficient = this._refHBObj.ConstantCoefficient;

            if (!this.TemperatureCoefficient.IsVaries)
                obj.TemperatureCoefficient = this._refHBObj.TemperatureCoefficient;
            if (!this.VelocityCoefficient.IsVaries)
                obj.VelocityCoefficient = this._refHBObj.VelocityCoefficient;
            return obj;
        }

        public RelayCommand ScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetSelector(_libSource.Energy);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Schedule.SetPropetyObj(dialog_rc);
            }
        });

    }


}
