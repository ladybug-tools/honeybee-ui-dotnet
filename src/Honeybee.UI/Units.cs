using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public static class Units
    {
        public enum UnitType
        {
            Length,
            Area,
            Volume,
            Temperature,
            TemperatureDelta,
            Power,
            Energy,
            PowerDensity,
            AirFlowRate,
            PeopleDensity,
            AirFlowRateArea,
            Speed,
            Illuminance,
            Conductivity,
            Resistance,
            UValue,
            Density,
            SpecificEntropy,
            Dimensionless

        }

        public static Dictionary<UnitType, Enum> _customUnitSettings;

        public static Dictionary<UnitType, Enum> CustomUnitSettings
        {
            get
            {
                if (_customUnitSettings == null)
                    _customUnitSettings = Units.LoadUnits();
                return _customUnitSettings;
            }
            //private set; 
        }
        public static string UnitSettingFile
        {
            get
            {
                var root = System.IO.Path.GetDirectoryName(typeof(Units).Assembly.Location);
                var file = System.IO.Path.Combine(HoneybeeSchema.Helper.Pathes.UserAppDataDotnet, "UISettings.json");
                return file;
            }
            //private set; 
        }

      

        public static Dictionary<UnitType, Enum> SIUnits = new Dictionary<UnitType, Enum>() {
            { UnitType.Length, LengthUnit.Meter },
            { UnitType.Area, AreaUnit.SquareMeter },
            { UnitType.Volume, VolumeUnit.CubicMeter },
            { UnitType.Temperature, TemperatureUnit.DegreeCelsius },
            { UnitType.TemperatureDelta, TemperatureDeltaUnit.DegreeCelsius },
            { UnitType.Power, PowerUnit.Watt },
            { UnitType.Energy, EnergyUnit.KilowattHour },
            { UnitType.PowerDensity, HeatFluxUnit.WattPerSquareMeter },
            { UnitType.AirFlowRate, VolumeFlowUnit.CubicMeterPerSecond },
            { UnitType.PeopleDensity, ReciprocalAreaUnit.InverseSquareMeter },
            { UnitType.AirFlowRateArea, VolumeFlowPerAreaUnit.CubicMeterPerSecondPerSquareMeter },
            { UnitType.Speed, SpeedUnit.MeterPerSecond },
            { UnitType.Illuminance, IlluminanceUnit.Lux },
            { UnitType.Conductivity, ThermalConductivityUnit.WattPerMeterKelvin },
            { UnitType.Resistance, ThermalResistanceUnit.SquareMeterKelvinPerWatt },
            { UnitType.UValue, HeatTransferCoefficientUnit.WattPerSquareMeterKelvin },
            { UnitType.Density, DensityUnit.KilogramPerCubicMeter },
            { UnitType.SpecificEntropy, SpecificEntropyUnit.JoulePerKilogramKelvin },
        };

        public static Dictionary<UnitType, Enum> IPUnits = new Dictionary<UnitType, Enum>() {
            { UnitType.Length, LengthUnit.Foot },
            { UnitType.Area, AreaUnit.SquareFoot },
            { UnitType.Volume, VolumeUnit.CubicFoot },
            { UnitType.Temperature, TemperatureUnit.DegreeFahrenheit },
            { UnitType.TemperatureDelta, TemperatureDeltaUnit.DegreeFahrenheit },
            { UnitType.Power, PowerUnit.BritishThermalUnitPerHour },
            { UnitType.Energy, EnergyUnit.KilobritishThermalUnit },
            { UnitType.PowerDensity, HeatFluxUnit.WattPerSquareFoot },
            { UnitType.AirFlowRate, VolumeFlowUnit.CubicFootPerMinute },
            { UnitType.PeopleDensity, ReciprocalAreaUnit.InverseSquareFoot },
            { UnitType.AirFlowRateArea, VolumeFlowPerAreaUnit.CubicFootPerMinutePerSquareFoot },
            { UnitType.Speed, SpeedUnit.FootPerMinute },
            { UnitType.Illuminance, IlluminanceUnit.Lux },   
            { UnitType.Conductivity, ThermalConductivityUnit.BtuPerHourFootFahrenheit },
            { UnitType.Resistance, ThermalResistanceUnit.HourSquareFeetDegreeFahrenheitPerBtu },
            { UnitType.UValue, HeatTransferCoefficientUnit.BtuPerSquareFootDegreeFahrenheit },
            { UnitType.Density, DensityUnit.PoundPerCubicFoot },
            { UnitType.SpecificEntropy, SpecificEntropyUnit.BtuPerPoundFahrenheit },
        };

        //public static Dictionary<LengthUnit, HoneybeeSchema.Units> LengthMapper = new Dictionary<LengthUnit, HoneybeeSchema.Units>() { 
        //    { LengthUnit.Foot, HoneybeeSchema.Units.Feet },
        //    { LengthUnit.Inch, HoneybeeSchema.Units.Inches },
        //    { LengthUnit.Meter, HoneybeeSchema.Units.Meters },
        //    { LengthUnit.Centimeter, HoneybeeSchema.Units.Centimeters },
        //    { LengthUnit.Millimeter, HoneybeeSchema.Units.Millimeters }
        //};

        #region Units

        public enum LengthUnit
        {
            Foot,
            Inch,
            Meter,
            Millimeter,
            Centimeter
        }

        public enum AreaUnit
        {
            SquareFoot,
            SquareInch,
            SquareMeter,
            SquareMillimeter,
            SquareCentimeter,
        }
        public enum VolumeUnit
        {
            CubicFoot,
            CubicInch,
            CubicMeter,
            CubicMillimeter,
            CubicCentimeter,
        }
    
        public enum TemperatureDeltaUnit
        {
            DegreeCelsius,
            DegreeFahrenheit,
        }

        public enum TemperatureUnit
        {
          
            DegreeCelsius,
            DegreeFahrenheit,
          
        }
        public enum PowerUnit
        {
            BritishThermalUnitPerHour,
            JoulePerHour,
            GigajoulePerHour,
            Kilowatt,
            Watt
        }

        public enum EnergyUnit
        {
            KilobritishThermalUnit,
            BritishThermalUnit,
            Joule,
            KilowattHour,
            WattHour
        }

        public enum VolumeFlowUnit
        {
            CubicFootPerMinute,
            CubicMeterPerSecond,
        }

        public enum HeatFluxUnit
        {
            KilowattPerSquareMeter,
            WattPerSquareFoot,
            WattPerSquareMeter
        }

        public enum ReciprocalAreaUnit
        {
            InverseSquareFoot,
            InverseSquareMeter,
        }

        public enum VolumeFlowPerAreaUnit
        {
            CubicFootPerMinutePerSquareFoot,
            CubicMeterPerSecondPerSquareMeter
        }

        public enum SpeedUnit
        {
            FootPerMinute,
            MeterPerSecond,
        }

        public enum IlluminanceUnit
        {
            Lux,
        }

        public enum ThermalConductivityUnit
        {
            BtuPerHourFootFahrenheit,
            WattPerMeterKelvin
        }

        public enum HeatTransferCoefficientUnit
        {
            WattPerSquareMeterKelvin,
            BtuPerSquareFootDegreeFahrenheit
        }

        public enum ThermalResistanceUnit
        {
            HourSquareFeetDegreeFahrenheitPerBtu,
            SquareMeterDegreeCelsiusPerWatt,
            SquareMeterKelvinPerWatt,
        }

        public enum DensityUnit
        {
            KilogramPerCubicMeter,
            PoundPerCubicFoot,
        }
        public enum SpecificEntropyUnit
        {
            BtuPerPoundFahrenheit,
            JoulePerKilogramKelvin,
            KilojoulePerKilogramKelvin,
        }

        #endregion


        //public static void U()
        //{
        //    //var a2 = UnitsNet.VolumeFlow.FromCubicFeetPerMinute(100) * UnitsNet.ReciprocalArea.FromInverseSquareFeet(20);
        //    UnitsNet.Units.HeatTransferCoefficientUnit.WattPerSquareMeterKelvin.ThermalResistanceUnit.SquareMeterKelvinPerWatt.BtuPerSquareFootDegreeFahrenheit
        //}

        public static void TryAddValue(this Dictionary<UnitType, Enum> CustomUnitSettings, UnitType unitType, Enum value)
        {
            if (CustomUnitSettings == null)
                return;
            if (CustomUnitSettings.ContainsKey(unitType))
            {
                CustomUnitSettings[unitType] = value;
            }
            else
            {
                CustomUnitSettings.Add(unitType, value);
            }
        }

        public static Enum ToUnitsNetEnum(this Enum inputHBEnum)
        {
            if (inputHBEnum == default)
                return inputHBEnum;

            var t = inputHBEnum.GetType().Name.ToString();
            var displayUnitType = typeof(UnitsNet.Angle).Assembly.GetType($"UnitsNet.Units.{t}");
            var ee = Enum.Parse(displayUnitType, inputHBEnum.ToString()) as Enum;
            return ee;
        }

        public static double ConvertValueWithUnits(double value, Enum fromUnit, Enum toUnit)
        {
            if (fromUnit == default || toUnit == default)
                return value;
            if (fromUnit == toUnit)
                return value;


            var quantity = UnitsNet.Quantity.From(value, fromUnit);
            return quantity.ToUnit(toUnit).Value;
        }

        public static string GetAbbreviation(this Enum unit)
        {
            var v = Convert.ToInt32(unit);
            var t = unit.GetType();
            return UnitsNet.UnitAbbreviationsCache.Default.GetDefaultAbbreviation(t, v);
        }


      
        internal static void SaveUnits()
        {
            if (CustomUnitSettings == null) return;
            var dic = CustomUnitSettings;
            var js = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            System.IO.File.WriteAllText(UnitSettingFile, js);

        }


        internal static Dictionary<UnitType, Enum> LoadUnits()
        {
            var refDic = SIUnits;

            if (System.IO.File.Exists(UnitSettingFile))
            {
                var json = System.IO.File.ReadAllText(UnitSettingFile);
                var objs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<UnitType, int>>(json);
                var units = new Dictionary<UnitType, Enum>();

                foreach (var item in objs)
                {
                    var old = refDic[item.Key];
                    var saved = Enum.ToObject(old.GetType(), item.Value) as Enum;
                    units.Add(item.Key, saved);
                }

                return units;
            }
            else
            {

                return refDic.ToDictionary(_ => _.Key, _ => _.Value);
            }
           


        }


    }

}
