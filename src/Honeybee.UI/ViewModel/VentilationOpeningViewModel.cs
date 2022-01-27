using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{

    public class VentilationOpeningViewModel : CheckboxPanelViewModel<VentilationOpening>
    {
        private VentilationOpening _refHBObj => this.refObjProperty;
        // double fractionAreaOperable = 0.5, double fractionHeightOperable = 1, double dischargeCoefficient = 0.45,
        // bool windCrossVent = false, double flowCoefficientClosed = 0, double flowExponentClosed = 0.65, double twoWayThreshold = 0.0001

        // FractionAreaOperable
        private DoubleViewModel _fractionAreaOperable;
        public DoubleViewModel FractionAreaOperable
        {
            get => _fractionAreaOperable;
            set => this.Set(() => _fractionAreaOperable = value, nameof(FractionAreaOperable));
        }

        // FractionHeightOperable
        private DoubleViewModel _fractionHeightOperable;
        public DoubleViewModel FractionHeightOperable
        {
            get => _fractionHeightOperable;
            set => this.Set(() => _fractionHeightOperable = value, nameof(FractionHeightOperable)); 
        }

        // DischargeCoefficient
        private DoubleViewModel _dischargeCoefficient;
        public DoubleViewModel DischargeCoefficient
        {
            get => _dischargeCoefficient;
            set => this.Set(() => _dischargeCoefficient = value, nameof(DischargeCoefficient));
        }

        // WindCrossVent
        private CheckboxViewModel _windCrossVent;
        public CheckboxViewModel WindCrossVent
        {
            get => _windCrossVent;
            set => this.Set(() => _windCrossVent = value, nameof(WindCrossVent));
        }

        // FlowCoefficientClosed
        private DoubleViewModel _flowCoefficientClosed;
        public DoubleViewModel FlowCoefficientClosed
        {
            get => _flowCoefficientClosed;
            set => this.Set(() => _flowCoefficientClosed = value, nameof(FlowCoefficientClosed));
        }

        // flowExponentClosed
        private DoubleViewModel _flowExponentClosed;
        public DoubleViewModel FlowExponentClosed
        {
            get => _flowExponentClosed;
            set => this.Set(() => _flowExponentClosed = value, nameof(FlowExponentClosed));
        }

        // twoWayThreshold
        private DoubleViewModel _twoWayThreshold;
        public DoubleViewModel TwoWayThreshold
        {
            get => _twoWayThreshold;
            set => this.Set(() => _twoWayThreshold = value, nameof(TwoWayThreshold));
        }

        public VentilationOpening Default { get; private set; }
        public VentilationOpeningViewModel(ModelProperties libSource, List<VentilationOpening> loads, Action<VentilationOpening> setAction) : base(libSource, setAction)
        {
            this.Default = new VentilationOpening();
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateVentilationOpening();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateVentilationOpening();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //FractionAreaOperable
            this.FractionAreaOperable = new DoubleViewModel((n) => _refHBObj.FractionAreaOperable = n);
            if (loads.Select(_ => _?.FractionAreaOperable).Distinct().Count() > 1)
                this.FractionAreaOperable.SetNumberText(this.Varies);
            else
                this.FractionAreaOperable.SetNumberText(_refHBObj.FractionAreaOperable.ToString());

            //FractionHeightOperable
            this.FractionHeightOperable = new DoubleViewModel((n) => _refHBObj.FractionHeightOperable = n);
            if (loads.Select(_ => _?.FractionHeightOperable).Distinct().Count() > 1)
                this.FractionHeightOperable.SetNumberText(this.Varies);
            else
                this.FractionHeightOperable.SetNumberText(_refHBObj.FractionHeightOperable.ToString());

            //DischargeCoefficient
            this.DischargeCoefficient = new DoubleViewModel((n) => _refHBObj.DischargeCoefficient = n);
            if (loads.Select(_ => _?.DischargeCoefficient).Distinct().Count() > 1)
                this.DischargeCoefficient.SetNumberText(this.Varies);
            else
                this.DischargeCoefficient.SetNumberText(_refHBObj.DischargeCoefficient.ToString());

            // windCrossVent
            this.WindCrossVent = new CheckboxViewModel(_ => _refHBObj.WindCrossVent = _);
            if (loads.Select(_ => _?.WindCrossVent).Distinct().Count() > 1)
                this.WindCrossVent.SetCheckboxVaries();
            else
                this.WindCrossVent.SetCheckboxChecked(this._refHBObj.WindCrossVent);


            //flowCoefficientClosed
            this.FlowCoefficientClosed = new DoubleViewModel((n) => _refHBObj.FlowCoefficientClosed = n);
            if (loads.Select(_ => _?.FlowCoefficientClosed).Distinct().Count() > 1)
                this.FlowCoefficientClosed.SetNumberText(this.Varies);
            else
                this.FlowCoefficientClosed.SetNumberText(_refHBObj.FlowCoefficientClosed.ToString());

            //FlowExponentClosed
            this.FlowExponentClosed = new DoubleViewModel((n) => _refHBObj.FlowExponentClosed = n);
            if (loads.Select(_ => _?.FlowExponentClosed).Distinct().Count() > 1)
                this.FlowExponentClosed.SetNumberText(this.Varies);
            else
                this.FlowExponentClosed.SetNumberText(_refHBObj.FlowExponentClosed.ToString());

            //twoWayThreshold
            this.TwoWayThreshold = new DoubleViewModel((n) => _refHBObj.TwoWayThreshold = n);
            if (loads.Select(_ => _?.TwoWayThreshold).Distinct().Count() > 1)
                this.TwoWayThreshold.SetNumberText(this.Varies);
            else
                this.TwoWayThreshold.SetNumberText(_refHBObj.TwoWayThreshold.ToString());

        }

        public VentilationOpening MatchObj(VentilationOpening obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateVentilationOpening() ?? new VentilationOpening();

            if (!this.FractionAreaOperable.IsVaries)
                obj.FractionAreaOperable = this._refHBObj.FractionAreaOperable;

            if (!this.FractionHeightOperable.IsVaries)
                obj.FractionHeightOperable = this._refHBObj.FractionHeightOperable;

            if (!this.DischargeCoefficient.IsVaries)
                obj.DischargeCoefficient = this._refHBObj.DischargeCoefficient;

            if (!this.WindCrossVent.IsVaries)
                obj.WindCrossVent = this._refHBObj.WindCrossVent;

            if (!this.FlowCoefficientClosed.IsVaries)
                obj.FlowCoefficientClosed = this._refHBObj.FlowCoefficientClosed;

            if (!this.FlowExponentClosed.IsVaries)
                obj.FlowExponentClosed = this._refHBObj.FlowExponentClosed;

            if (!this.TwoWayThreshold.IsVaries)
                obj.TwoWayThreshold = this._refHBObj.TwoWayThreshold;

            return obj;
        }

    }




}
