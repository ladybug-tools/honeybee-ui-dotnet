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
        private DoubleViewModel _wattsPerArea;

        public DoubleViewModel WattsPerArea
        {
            get => _wattsPerArea;
            private set {
                this.Set(() => _wattsPerArea = value, nameof(WattsPerArea)); 
            }
        }
        

        // Schedule
        private ButtonViewModel _schedule;

        public ButtonViewModel Schedule
        {
            get => _schedule;
            private set { this.Set(() => _schedule = value, nameof(Schedule)); }
        }

        // RadiantFractionText
        private DoubleViewModel _radiantFraction;

        public DoubleViewModel RadiantFraction
        {
            get => _radiantFraction;
            private set { this.Set(() => _radiantFraction = value, nameof(RadiantFraction)); }
        }

        // VisibleFraction
        private DoubleViewModel _visibleFraction;

        public DoubleViewModel VisibleFraction
        {
            get => _visibleFraction;
            private set { this.Set(() => _visibleFraction = value, nameof(VisibleFraction)); }
        }

        // BaselineWattsPerArea
        private DoubleViewModel _baselineWattsPerArea;

        public DoubleViewModel BaselineWattsPerArea
        {
            get => _baselineWattsPerArea;
            private set { this.Set(() => _baselineWattsPerArea = value, nameof(BaselineWattsPerArea)); }
        }

        // BaselineWattsPerArea
        private DoubleViewModel _returnAirFraction;

        public DoubleViewModel ReturnAirFraction
        {
            get => _returnAirFraction;
            private set { this.Set(() => _returnAirFraction = value, nameof(ReturnAirFraction)); }
        }

        public LightingAbridged Default { get; private set; }

        public LightingViewModel(ModelProperties libSource, List<LightingAbridged> lights, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new LightingAbridged(Guid.NewGuid().ToString(), 0, "");
            this.refObjProperty = lights.FirstOrDefault()?.DuplicateLightingAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateLightingAbridged();



            if (lights.Count == 1 && lights.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //WattsPerArea
            this.WattsPerArea = new DoubleViewModel((n) => _refHBObj.WattsPerArea = n);
            if (lights.Select(_ => _?.WattsPerArea).Distinct().Count() > 1)
                this.WattsPerArea.SetNumberText(this.Varies);
            else
                this.WattsPerArea.SetNumberText(_refHBObj.WattsPerArea.ToString());


            //Schedule
            var sch = libSource.Energy.Schedules
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Schedule);
            this.Schedule = new ButtonViewModel((n) => _refHBObj.Schedule = n?.Identifier);
            if (lights.Select(_ => _?.Schedule).Distinct().Count() > 1)
                this.Schedule.SetBtnName(this.Varies);
            else
                this.Schedule.SetPropetyObj(sch);


            //RadiantFraction
            this.RadiantFraction = new DoubleViewModel((n) => _refHBObj.RadiantFraction = n);
            if (lights.Select(_ => _?.RadiantFraction).Distinct().Count() > 1)
                this.RadiantFraction.SetNumberText(this.Varies);
            else
                this.RadiantFraction.SetNumberText(_refHBObj.RadiantFraction.ToString());


            //VisibleFraction
            this.VisibleFraction = new DoubleViewModel((n) => _refHBObj.VisibleFraction = n);
            if (lights.Select(_ => _?.VisibleFraction).Distinct().Count() > 1)
                this.VisibleFraction.SetNumberText(this.Varies);
            else
                this.VisibleFraction.SetNumberText(_refHBObj.VisibleFraction.ToString());


            //ReturnAirFraction
            this.ReturnAirFraction = new DoubleViewModel((n) => _refHBObj.ReturnAirFraction = n);
            if (lights.Select(_ => _?.ReturnAirFraction).Distinct().Count() > 1)
                this.ReturnAirFraction.SetNumberText(this.Varies);
            else
                this.ReturnAirFraction.SetNumberText(_refHBObj.ReturnAirFraction.ToString());


            //BaselineWattsPerArea
            this.BaselineWattsPerArea = new DoubleViewModel((n) => _refHBObj.BaselineWattsPerArea = n);
            if (lights.Select(_ => _?.BaselineWattsPerArea).Distinct().Count() > 1)
                this.BaselineWattsPerArea.SetNumberText(this.Varies);
            else
                this.BaselineWattsPerArea.SetNumberText(_refHBObj.BaselineWattsPerArea.ToString());
        }

        public LightingAbridged MatchObj(LightingAbridged obj)
        {
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateLightingAbridged() ?? new LightingAbridged(Guid.NewGuid().ToString(), 0, "");

            if (!this.WattsPerArea.IsVaries)
                obj.WattsPerArea = this._refHBObj.WattsPerArea;
            if (!this.Schedule.IsVaries)
            {
                if (this._refHBObj.Schedule == null)
                    throw new ArgumentException("Missing required schedule of the lighting load!");
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
