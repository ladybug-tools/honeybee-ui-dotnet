using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema.Energy;

namespace Honeybee.UI
{
    public class BoundaryConditionOtherSideTemperatureViewModel : ViewModelBase
    {
        private OtherSideTemperature _refHBObj;


        private DoubleViewModel _heatTransferCoefficient;

        public DoubleViewModel HeatTransferCoefficient
        {
            get => _heatTransferCoefficient;
            set => this.Set(() => _heatTransferCoefficient = value, nameof(HeatTransferCoefficient));
        }

        private DoubleViewModel _temperature;

        public DoubleViewModel Temperature
        {
            get => _temperature;
            set => this.Set(() => _temperature = value, nameof(Temperature));
        }

        private bool _isTemperatureAutocalculate;

        public bool IsTemperatureAutocalculate
        {
            get => _isTemperatureAutocalculate;
            set
            {

                if (value)
                    this._refHBObj.Temperature = new Autocalculate();
                else
                    this.Temperature.SetNumberText(this.Temperature.NumberText);

                IsTemperatureInputEnabled = !value;
                this.Set(() => _isTemperatureAutocalculate = value, nameof(IsTemperatureAutocalculate));
            }
        }

        private bool _isTemperatureInputEnabled;

        public bool IsTemperatureInputEnabled
        {
            get => _isTemperatureInputEnabled;
            set { this.Set(() => _isTemperatureInputEnabled = value, nameof(IsTemperatureInputEnabled)); }
        }

        public OtherSideTemperature Default { get; private set; }
        public BoundaryConditionOtherSideTemperatureViewModel(List<OtherSideTemperature> objs, Action<OtherSideTemperature> setAction)
        {
            this.Default = new OtherSideTemperature( temperature: new Autocalculate());
            this._refHBObj = objs.FirstOrDefault()?.DuplicateOtherSideTemperature();
            this._refHBObj = this._refHBObj ?? this.Default.DuplicateOtherSideTemperature();


            // HeatTransferCoefficient
            this.HeatTransferCoefficient = new DoubleViewModel((n) => _refHBObj.HeatTransferCoefficient = n);
            if (objs.Select(_ => _?.HeatTransferCoefficient).Distinct().Count() > 1)
                this.HeatTransferCoefficient.SetNumberText(ReservedText.Varies);
            else
                this.HeatTransferCoefficient.SetNumberText(_refHBObj.HeatTransferCoefficient.ToString());

            // Temperature
            this.Temperature = new DoubleViewModel((n) => _refHBObj.Temperature = n);
            this.Temperature.SetUnits(Units.TemperatureUnit.DegreeCelsius, Units.UnitType.Temperature);
            var tps = objs.Select(_ => _?.Temperature).Distinct();
            if (tps.Count() > 1)
            {
                this.IsTemperatureAutocalculate = false;
                this.Temperature.SetNumberText(ReservedText.Varies);
            }
            else
            {
                this.IsTemperatureAutocalculate = tps?.FirstOrDefault(_ => _?.Obj is Autocalculate) != null;
                if (!IsTemperatureAutocalculate)
                {
                    var t = _refHBObj.Temperature?.Obj is double tt ? tt : 0;
                    this.Temperature.SetBaseUnitNumber(t);
                }
                else
                    this.Temperature.SetNumberText("0");
            }

            setAction?.Invoke(this._refHBObj);


        }

        public OtherSideTemperature MatchObj(OtherSideTemperature obj)
        {
            obj = obj?.DuplicateOtherSideTemperature() ?? new OtherSideTemperature(temperature: new Autocalculate());

            if (!this.HeatTransferCoefficient.IsVaries)
                obj.HeatTransferCoefficient = this._refHBObj.HeatTransferCoefficient;

            if (!this.Temperature.IsVaries)
                obj.Temperature = this._refHBObj.Temperature;

            return obj;
        }




    }


}
