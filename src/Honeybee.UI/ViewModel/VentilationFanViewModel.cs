using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class VentilationFanViewModel : CheckboxPanelViewModel<VentilationFan>
    {
        private VentilationFan _refHBObj => this.refObjProperty;
        // double flowRate, double pressureRise, double efficiency, string displayName = null, VentilationType ventilationType = VentilationType.Balanced, VentilationControlAbridged control = null

        // FlowRate
        private DoubleViewModel _flowRate;

        public DoubleViewModel FlowRate
        {
            get => _flowRate;
            set
            {
                this.Set(() => _flowRate = value, nameof(FlowRate));
            }
        }

        public string FlowRateDescription => Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(VentilationFan), nameof(_refHBObj.FlowRate)));

        // Control
        private VentilationControlViewModel _control;

        public VentilationControlViewModel Control
        {
            get => _control;
            set { this.Set(() => _control = value, nameof(Control)); }
        }

        public string ControlDescription => Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(VentilationFan), nameof(_refHBObj.Control)));

        // PressureRise
        private DoubleViewModel _pressureRise;

        public DoubleViewModel PressureRise
        {
            get => _pressureRise;
            set { this.Set(() => _pressureRise = value, nameof(PressureRise)); }
        }

        public string PressureRiseDescription => Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(VentilationFan), nameof(_refHBObj.PressureRise)));


        // Efficiency
        private DoubleViewModel _efficiency;

        public DoubleViewModel Efficiency
        {
            get => _efficiency;
            set { this.Set(() => _efficiency = value, nameof(Efficiency)); }
        }

        public string EfficiencyDescription => Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(VentilationFan), nameof(_refHBObj.Efficiency)));

        public VentilationType VentilationType
        {
            get => _refHBObj.VentilationType;
            set
            {
                this.Set(() => _refHBObj.VentilationType = value, nameof(VentilationType));
            }
        }


        public string VentilationTypeDescription => Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(typeof(VentilationFan), nameof(_refHBObj.VentilationType)));

        public VentilationFan Default { get; private set; }
        public VentilationFanViewModel(ModelProperties libSource, List<VentilationFan> loads, Action<VentilationFan> setAction) : base(libSource, setAction)
        {
            this.Default = new VentilationFan(Guid.NewGuid().ToString(), 0, 75, 0.2);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateVentilationFan() ?? this.Default.DuplicateVentilationFan();
            
            if (loads.Distinct().Count() == 1)
                this.IsCheckboxChecked = loads.FirstOrDefault() == null;
            else
                this.IsCheckboxVaries();


            //FlowRate
            this.FlowRate = new DoubleViewModel((n) => _refHBObj.FlowRate = n);
            this.FlowRate.SetUnits(Units.VolumeFlowUnit.CubicMeterPerSecond, Units.UnitType.AirFlowRate);
            if (loads.Select(_ => _?.FlowRate).Distinct().Count() > 1)
                this.FlowRate.SetNumberText(ReservedText.Varies);
            else
                this.FlowRate.SetBaseUnitNumber(_refHBObj.FlowRate);


            //Control
            this.Control = new VentilationControlViewModel(libSource, new List<VentilationControlAbridged> { _refHBObj.Control }, (s) =>_refHBObj.Control = s);

            //PressureRise
            this.PressureRise = new DoubleViewModel((n) => _refHBObj.PressureRise = n);
            this.PressureRise.SetUnits(Units.PressureUnit.Pascal, Units.UnitType.Pressure);
            if (loads.Select(_ => _?.PressureRise).Distinct().Count() > 1)
                this.PressureRise.SetNumberText(ReservedText.Varies);
            else
                this.PressureRise.SetBaseUnitNumber(_refHBObj.PressureRise);


            //Efficiency
            this.Efficiency = new DoubleViewModel((n) => _refHBObj.Efficiency = n);
            if (loads.Select(_ => _?.Efficiency).Distinct().Count() > 1)
                this.Efficiency.SetNumberText(ReservedText.Varies);
            else
                this.Efficiency.SetBaseUnitNumber(_refHBObj.Efficiency);


            //VentilationType
            this.VentilationType = this._refHBObj.VentilationType;

        }

        public VentilationFan MatchObj(VentilationFan obj)
        {
            // by room program type
            if (this.IsCheckboxChecked.GetValueOrDefault())
                return null;

            if (this.IsVaries)
                return obj?.DuplicateVentilationFan();

            obj = obj?.DuplicateVentilationFan() ?? this.Default.DuplicateVentilationFan();

            if (!this.FlowRate.IsVaries)
                obj.FlowRate = this._refHBObj.FlowRate;

            obj.Control = this.Control.MatchObj(obj.Control);

            if (!this.PressureRise.IsVaries)
                obj.PressureRise = this._refHBObj.PressureRise;

            if (!this.Efficiency.IsVaries)
                obj.Efficiency = this._refHBObj.Efficiency;

            return obj;
        }


    }


}
