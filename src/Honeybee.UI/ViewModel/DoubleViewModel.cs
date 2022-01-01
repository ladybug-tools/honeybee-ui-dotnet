using System;
using UnitsNet;

namespace Honeybee.UI
{
    public class DoubleViewModel : ViewModelBase
    {
        public bool IsVaries;
        public Action<double> SetHBProperty { get; private set; }

        public string DisplayUnitAbbreviation { get; private set; }
        public Enum DisplayUnit { get; private set; }
        public Enum BaseUnit { get; private set; }

        private string _numberText;
        public string NumberText // with display unit
        {
            get => _numberText;
            private set
            {
                IsVaries = value == this.Varies;
                if (IsVaries)
                {
                    this.Set(() => _numberText = value, nameof(NumberText));
                }
                else if (TryParse(value, out var number))
                {
                    var converted = ToBaseValue(number);
                    SetHBProperty?.Invoke(converted);
                    this.Set(() => _numberText = number.ToString(), nameof(NumberText));
                }
            }
        }


        public DoubleViewModel(Action<double> setAction)
        {
            this.SetHBProperty = setAction;
        }

        /// <summary>
        /// Set number with display units
        /// </summary>
        /// <param name="valueWithDisplayUnit"></param>
        public void SetNumberText(string valueWithDisplayUnit)
        {
            this.NumberText = valueWithDisplayUnit;
        }

        /// <summary>
        /// Set number with base units
        /// </summary>
        /// <param name="name"></param>
        public void SetBaseUnitNumber(double valueWithBaseUnit)
        {
            //convert value to displayNumber
            var d = ToDisplayValue(valueWithBaseUnit);
            this.NumberText = d.ToString();
        }

        /// <summary>
        /// User enum type from one of Honeybee.UI.Units
        /// </summary>
        /// <param name="displayUnit"></param>
        /// <param name="baseUnit"></param>
        public void SetUnits(Enum baseUnit, Enum displayUnit = default)
        {
            this.BaseUnit = ToUnitsNetEnum(baseUnit);
            this.DisplayUnit = ToUnitsNetEnum(displayUnit);

            this.DisplayUnit = this.DisplayUnit ?? this.BaseUnit;

            var v = Convert.ToInt32(DisplayUnit);
            var t = DisplayUnit.GetType();
            this.DisplayUnitAbbreviation = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(t, v);
        }

        /// <summary>
        /// Get display unit based on UnitType where users specified.
        /// </summary>
        /// <param name="baseUnit"></param>
        /// <param name="unitType"></param>
        public void SetUnits(Enum baseUnit, Units.UnitType unitType)
        {
            Units.CustomUnitSettings.TryGetValue(unitType, out var displayUnit);
            SetUnits(baseUnit, displayUnit);
        }

        /// <summary>
        /// DisplayUnitAbbreviation has nothing to do with number conversions, it is only for UI.
        /// </summary>
        /// <param name="unit"></param>
        public void SetDisplayUnitAbbreviation(string unit)
        {
            this.DisplayUnitAbbreviation = unit;
        }

        private static bool TryParse(string text, out double value) => Utility.TryParse(text, out value);

        private double ToBaseValue(double value) => Units.ConvertValueWithUnits(value, this.DisplayUnit, this.BaseUnit);

        private double ToDisplayValue(double value) => Units.ConvertValueWithUnits(value, this.BaseUnit, this.DisplayUnit);

        private static Enum ToUnitsNetEnum(Enum inputEnum) => Units.ToUnitsNetEnum(inputEnum);
    }

}
