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
            { UnitType.Speeddd, SpeedUnit.MeterPerSecond },
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

        //public static void U()
        //{
        //    //var a2 = UnitsNet.VolumeFlow.FromCubicFeetPerMinute(100) * UnitsNet.ReciprocalArea.FromInverseSquareFeet(20);
        //    UnitsNet.Units.VolumeFlowPerAreaUnit
        //}



    }



}
