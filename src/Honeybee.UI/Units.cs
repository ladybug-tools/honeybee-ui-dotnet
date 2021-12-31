using System;
using System.Collections.Generic;

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
            Illuminance
        }



        public static Dictionary<UnitType, Enum> CustomUnitSettings = new Dictionary<UnitType, Enum>() {
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
        };

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
        };

        public static Dictionary<UnitType, Enum> IPUnits = new Dictionary<UnitType, Enum>() {
            { UnitType.Length, LengthUnit.Foot },
            { UnitType.Area, AreaUnit.SquareFoot },
            { UnitType.Volume, VolumeUnit.CubicFoot },
            { UnitType.Temperature, TemperatureUnit.DegreeFahrenheit },
            { UnitType.TemperatureDelta, TemperatureDeltaUnit.DegreeFahrenheit },
            { UnitType.Power, PowerUnit.Watt },
            { UnitType.Energy, EnergyUnit.KilobritishThermalUnit },
            { UnitType.PowerDensity, HeatFluxUnit.WattPerSquareFoot },
            { UnitType.AirFlowRate, VolumeFlowUnit.CubicFootPerMinute },
            { UnitType.PeopleDensity, ReciprocalAreaUnit.InverseSquareFoot },
            { UnitType.AirFlowRateArea, VolumeFlowPerAreaUnit.CubicFootPerMinutePerSquareFoot },
            { UnitType.Speed, SpeedUnit.FootPerMinute },
            { UnitType.Illuminance, IlluminanceUnit.Lux },
        };

        public static Dictionary<LengthUnit, HoneybeeSchema.Units> LengthMapper = new Dictionary<LengthUnit, HoneybeeSchema.Units>() { 
            { LengthUnit.Foot, HoneybeeSchema.Units.Feet },
            { LengthUnit.Inch, HoneybeeSchema.Units.Inches },
            { LengthUnit.Meter, HoneybeeSchema.Units.Meters },
            { LengthUnit.Centimeter, HoneybeeSchema.Units.Centimeters },
            { LengthUnit.Millimeter, HoneybeeSchema.Units.Millimeters }
        };
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
            SquareKilometer,
            SquareMeter,
        }
        public enum VolumeUnit
        {
            CubicCentimeter,
            CubicFoot,
            CubicInch,
            CubicMeter,
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

        public static void U()
        {
            //var a2 = UnitsNet.VolumeFlow.FromCubicFeetPerMinute(100) * UnitsNet.ReciprocalArea.FromInverseSquareFeet(20);
            //UnitsNet.Units.EnergyUnit.KilobritishThermalUnit
        }

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

    }



}
