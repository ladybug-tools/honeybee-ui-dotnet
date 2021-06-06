using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class SetpointViewModel : CheckboxPanelViewModel
    {
        private SetpointAbridged _refHBObj => this.refObjProperty as SetpointAbridged;
        // string coolingSchedule, string heatingSchedule, string displayName = null, string humidifyingSchedule = null, string dehumidifyingSchedule = null

        // CoolingSchedule
        private ButtonViewModel _coolingSchedule;

        public ButtonViewModel CoolingSchedule
        {
            get => _coolingSchedule;
            private set {
                this.Set(() => _coolingSchedule = value, nameof(CoolingSchedule)); 
            }
        }


        // HeatingSchedule
        private ButtonViewModel _heatingSchedule;

        public ButtonViewModel HeatingSchedule
        {
            get => _heatingSchedule;
            private set { this.Set(() => _heatingSchedule = value, nameof(HeatingSchedule)); }
        }

        // HumidifyingSchedule
        private ButtonViewModel _humidifyingSchedule;

        public ButtonViewModel HumidifyingSchedule
        {
            get => _humidifyingSchedule;
            private set { this.Set(() => _humidifyingSchedule = value, nameof(HumidifyingSchedule)); }
        }

        // dehumidifyingSchedule
        private ButtonViewModel _dehumidifyingSchedule;

        public ButtonViewModel DehumidifyingSchedule
        {
            get => _dehumidifyingSchedule;
            private set { this.Set(() => _dehumidifyingSchedule = value, nameof(DehumidifyingSchedule)); }
        }


        public SetpointAbridged Default { get; private set; }
        public SetpointViewModel(ModelProperties libSource, List<SetpointAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new SetpointAbridged(Guid.NewGuid().ToString(), "", "");
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateSetpointAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateSetpointAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //CoolingSchedule
            var clSch = libSource.Energy.Schedules
               .OfType<IIDdBase>()
               .FirstOrDefault(_ => _.Identifier == _refHBObj.CoolingSchedule);
            this.CoolingSchedule = new ButtonViewModel((n) => _refHBObj.CoolingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.CoolingSchedule).Distinct().Count() > 1)
                this.CoolingSchedule.SetBtnName(this.Varies);
            else
                this.CoolingSchedule.SetPropetyObj(clSch);


            //HeatingSchedule
            var htSch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.HeatingSchedule);
            this.HeatingSchedule = new ButtonViewModel((n) => _refHBObj.HeatingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.HeatingSchedule).Distinct().Count() > 1)
                this.HeatingSchedule.SetBtnName(this.Varies);
            else
                this.HeatingSchedule.SetPropetyObj(htSch);


            //HumidifyingSchedule
            var huSch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.HumidifyingSchedule);
            this.HumidifyingSchedule = new ButtonViewModel((n) => _refHBObj.HumidifyingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.HumidifyingSchedule).Distinct().Count() > 1)
                this.HumidifyingSchedule.SetBtnName(this.Varies);
            else
                this.HumidifyingSchedule.SetPropetyObj(huSch);


            //DehumidifyingSchedule
            var dhSch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.DehumidifyingSchedule);
            this.DehumidifyingSchedule = new ButtonViewModel((n) => _refHBObj.DehumidifyingSchedule = n?.Identifier);
            if (loads.Select(_ => _?.DehumidifyingSchedule).Distinct().Count() > 1)
                this.DehumidifyingSchedule.SetBtnName(this.Varies);
            else
                this.DehumidifyingSchedule.SetPropetyObj(dhSch);

            

        }

        public SetpointAbridged MatchObj(SetpointAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateSetpointAbridged() ?? new SetpointAbridged(Guid.NewGuid().ToString(), "", "");

            if (!this.CoolingSchedule.IsVaries)
            {
                if (this._refHBObj.CoolingSchedule == null)
                    throw new ArgumentException("Missing required setpoint cooling schedule!");
                obj.CoolingSchedule = this._refHBObj.CoolingSchedule;
            }
       
            if (!this.HeatingSchedule.IsVaries)
            {
                if (this._refHBObj.HeatingSchedule == null)
                    throw new ArgumentException("Missing required setpoint heating schedule!");
                obj.HeatingSchedule = this._refHBObj.HeatingSchedule;
            }

            if (!this.HumidifyingSchedule.IsVaries)
                obj.HumidifyingSchedule = this._refHBObj.HumidifyingSchedule;

            if (!this.DehumidifyingSchedule.IsVaries)
                obj.DehumidifyingSchedule = this._refHBObj.DehumidifyingSchedule;


            return obj;
        }

        public RelayCommand CoolingScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.CoolingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand HeatingScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HeatingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand HumidifyingScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HumidifyingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand DehumidifyingScheduleCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ScheduleRulesetManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.DehumidifyingSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

       
    }


}
