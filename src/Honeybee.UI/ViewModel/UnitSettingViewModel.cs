using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class UnitSettingViewModel : ViewModelBase
    {
        #region Length
        private Units.LengthUnit _length = Units.LengthUnit.Meter;
        public Units.LengthUnit Length
        {
            get => _length;
            set { Set(() => _length = value, nameof(Length)); LengthAbbv = GetAbbr(value); }
        }
        private bool _lengthEnabled = true;
        public bool LengthEnabled
        {
            get => _lengthEnabled;
            set => Set(() => _lengthEnabled = value, nameof(LengthEnabled));
        }
        private string _LengthAbbv;
        public string LengthAbbv
        {
            get => _LengthAbbv;
            set => Set(() => _LengthAbbv = value, nameof(LengthAbbv));
        }
        #endregion

        #region Area
        private Units.AreaUnit _area = Units.AreaUnit.SquareMeter;
        public Units.AreaUnit Area
        {
            get => _area;
            set { Set(() => _area = value, nameof(Area)); AreaAbbv = GetAbbr(value); }
}
        private bool _AreaEnabled = true;
        public bool AreaEnabled
        {
            get => _AreaEnabled;
            set => Set(() => _AreaEnabled = value, nameof(AreaEnabled));
        }
        private string _AreaAbbv;
        public string AreaAbbv
        {
            get => _AreaAbbv;
            set => Set(() => _AreaAbbv = value, nameof(AreaAbbv));
        }
        #endregion

        #region Volume

        private Units.VolumeUnit _Volume = Units.VolumeUnit.CubicMeter;
        public Units.VolumeUnit Volume
        {
            get => _Volume;
            set { Set(() => _Volume = value, nameof(Volume)); VolumeAbbv = GetAbbr(value); }
}
        private bool _VolumeEnabled = true;
        public bool VolumeEnabled
        {
            get => _VolumeEnabled;
            set => Set(() => _VolumeEnabled = value, nameof(VolumeEnabled));
        }
        private string _VolumeAbbv;
        public string VolumeAbbv
        {
            get => _VolumeAbbv;
            set => Set(() => _VolumeAbbv = value, nameof(VolumeAbbv));
        }

        #endregion

        #region Temperature
        private Units.TemperatureUnit _Temperature = Units.TemperatureUnit.DegreeCelsius;
        public Units.TemperatureUnit Temperature
        {
            get => _Temperature;
            set { Set(() => _Temperature = value, nameof(Temperature)); TemperatureAbbv = GetAbbr(value); }
        }
        private bool _TemperatureEnabled = true;
        public bool TemperatureEnabled
        {
            get => _TemperatureEnabled;
            set => Set(() => _TemperatureEnabled = value, nameof(TemperatureEnabled));
        }
        private string _TemperatureAbbv;
        public string TemperatureAbbv
        {
            get => _TemperatureAbbv;
            set => Set(() => _TemperatureAbbv = value, nameof(TemperatureAbbv));
        }
        #endregion

        #region TemperatureDelta
        private Units.TemperatureDeltaUnit _TemperatureDelta = Units.TemperatureDeltaUnit.DegreeCelsius;
        public Units.TemperatureDeltaUnit TemperatureDelta
        {
            get => _TemperatureDelta;
            set { Set(() => _TemperatureDelta = value, nameof(TemperatureDelta)); TemperatureDeltaAbbv = GetAbbr(value); }
        }
        private bool _TemperatureDeltaEnabled = true;
        public bool TemperatureDeltaEnabled
        {
            get => _TemperatureDeltaEnabled;
            set => Set(() => _TemperatureDeltaEnabled = value, nameof(TemperatureDeltaEnabled));
        }
        private string _TemperatureDeltaAbbv;
        public string TemperatureDeltaAbbv
        {
            get => _TemperatureDeltaAbbv;
            set => Set(() => _TemperatureDeltaAbbv = value, nameof(TemperatureDeltaAbbv));
        }
        #endregion

        #region Power
        private Units.PowerUnit _Power = Units.PowerUnit.Watt;
        public Units.PowerUnit Power
        {
            get => _Power;
            set { Set(() => _Power = value, nameof(Power)); PowerAbbv = GetAbbr(value); }
        }
        private bool _PowerEnabled = true;
        public bool PowerEnabled
        {
            get => _PowerEnabled;
            set => Set(() => _PowerEnabled = value, nameof(PowerEnabled));
        }
        private string _PowerAbbv;
        public string PowerAbbv
        {
            get => _PowerAbbv;
            set => Set(() => _PowerAbbv = value, nameof(PowerAbbv));
        }
        #endregion

        #region Energy

        private Units.EnergyUnit _Energy = Units.EnergyUnit.KilowattHour;
        public Units.EnergyUnit Energy
        {
            get => _Energy;
            set { Set(() => _Energy = value, nameof(Energy)); EnergyAbbv = GetAbbr(value); }
        }

        private bool _EnergyEnabled = true;
        public bool EnergyEnabled
        {
            get => _EnergyEnabled;
            set => Set(() => _EnergyEnabled = value, nameof(EnergyEnabled));
        }

        private string _EnergyAbbv;
        public string EnergyAbbv
        {
            get => _EnergyAbbv;
            set => Set(() => _EnergyAbbv = value, nameof(EnergyAbbv));
        }
        #endregion

        #region PowerDensity
        private Units.HeatFluxUnit _PowerDensity = Units.HeatFluxUnit.WattPerSquareMeter;
        public Units.HeatFluxUnit PowerDensity
        {
            get => _PowerDensity;
            set { Set(() => _PowerDensity = value, nameof(PowerDensity)); PowerDensityAbbv = GetAbbr(value); }
        }

        private bool _PowerDensityEnabled = true;
        public bool PowerDensityEnabled
        {
            get => _PowerDensityEnabled;
            set => Set(() => _PowerDensityEnabled = value, nameof(PowerDensityEnabled));
        }

        private string _PowerDensityAbbv;
        public string PowerDensityAbbv
        {
            get => _PowerDensityAbbv;
            set => Set(() => _PowerDensityAbbv = value, nameof(PowerDensityAbbv));
        }
        #endregion

        #region AirFlowRate
        private Units.VolumeFlowUnit _AirFlowRate = Units.VolumeFlowUnit.CubicMeterPerSecond;
        public Units.VolumeFlowUnit AirFlowRate
        {
            get => _AirFlowRate;
            set { Set(() => _AirFlowRate = value, nameof(AirFlowRate)); AirFlowRateAbbv = GetAbbr(value); }
        }

        private bool _AirFlowRateEnabled = true;
        public bool AirFlowRateEnabled
        {
            get => _AirFlowRateEnabled;
            set => Set(() => _AirFlowRateEnabled = value, nameof(AirFlowRateEnabled));
        }

        private string _AirFlowRateAbbv;
        public string AirFlowRateAbbv
        {
            get => _AirFlowRateAbbv;
            set => Set(() => _AirFlowRateAbbv = value, nameof(AirFlowRateAbbv));
        }

        #endregion

        #region PeopleDensity
        private Units.ReciprocalAreaUnit _PeopleDensity = Units.ReciprocalAreaUnit.InverseSquareMeter;
        public Units.ReciprocalAreaUnit PeopleDensity
        {
            get => _PeopleDensity;
            set { Set(() => _PeopleDensity = value, nameof(PeopleDensity)); PeopleDensityAbbv = GetAbbr(value); }
        }

        private bool _PeopleDensityEnabled = true;
        public bool PeopleDensityEnabled
        {
            get => _PeopleDensityEnabled;
            set => Set(() => _PeopleDensityEnabled = value, nameof(PeopleDensityEnabled));
        }

        private string _PeopleDensityAbbv;
        public string PeopleDensityAbbv
        {
            get => _PeopleDensityAbbv;
            set => Set(() => _PeopleDensityAbbv = value, nameof(PeopleDensityAbbv));
        }

        #endregion

        #region AirFlowRateArea
        private Units.VolumeFlowPerAreaUnit _AirFlowRateArea = Units.VolumeFlowPerAreaUnit.CubicMeterPerSecondPerSquareMeter;
        public Units.VolumeFlowPerAreaUnit AirFlowRateArea
        {
            get => _AirFlowRateArea;
            set { Set(() => _AirFlowRateArea = value, nameof(AirFlowRateArea)); AirFlowRateAreaAbbv = GetAbbr(value); }
        }

        private bool _AirFlowRateAreaEnabled = true;
        public bool AirFlowRateAreaEnabled
        {
            get => _AirFlowRateAreaEnabled;
            set => Set(() => _AirFlowRateAreaEnabled = value, nameof(AirFlowRateAreaEnabled));
        }

        private string _AirFlowRateAreaAbbv;
        public string AirFlowRateAreaAbbv
        {
            get => _AirFlowRateAreaAbbv;
            set => Set(() => _AirFlowRateAreaAbbv = value, nameof(AirFlowRateAreaAbbv));
        }

        #endregion

        #region Speed
        private Units.SpeedUnit _Speed = Units.SpeedUnit.MeterPerSecond;
        public Units.SpeedUnit Speed
        {
            get => _Speed;
            set { Set(() => _Speed = value, nameof(Speed)); SpeedAbbv = GetAbbr(value); }
        }

        private bool _SpeedEnabled = true;
        public bool SpeedEnabled
        {
            get => _SpeedEnabled;
            set => Set(() => _SpeedEnabled = value, nameof(SpeedEnabled));
        }

        private string _SpeedAbbv;
        public string SpeedAbbv
        {
            get => _SpeedAbbv;
            set => Set(() => _SpeedAbbv = value, nameof(SpeedAbbv));
        }

        #endregion

        #region Illuminance
        private Units.IlluminanceUnit _Illuminance = Units.IlluminanceUnit.Lux;
        public Units.IlluminanceUnit Illuminance
        {
            get => _Illuminance;
            set { Set(() => _Illuminance = value, nameof(Illuminance)); IlluminanceAbbv = GetAbbr(value); }
        }

        private bool _IlluminanceEnabled = true;
        public bool IlluminanceEnabled
        {
            get => _IlluminanceEnabled;
            set => Set(() => _IlluminanceEnabled = value, nameof(IlluminanceEnabled));
        }

        private string _IlluminanceAbbv;
        public string IlluminanceAbbv
        {
            get => _IlluminanceAbbv;
            set => Set(() => _IlluminanceAbbv = value, nameof(IlluminanceAbbv));
        }

        #endregion

        private bool _useIP = false;
        public bool UseIP
        {
            get => _useIP;
            set
            {
                _useIP = value;
                var dic = _useIP ? Units.IPUnits : Units.SIUnits;

                if (this.LengthEnabled && dic[Units.UnitType.Length] is Units.LengthUnit l)
                    Length = l;
                if (this.AreaEnabled && dic[Units.UnitType.Area] is Units.AreaUnit a)
                    Area = a;
                if (this.VolumeEnabled && dic[Units.UnitType.Volume] is Units.VolumeUnit v)
                    Volume = v;
                if (this.TemperatureEnabled && dic[Units.UnitType.Temperature] is Units.TemperatureUnit t)
                    Temperature = t;
                if (this.TemperatureDeltaEnabled && dic[Units.UnitType.TemperatureDelta] is Units.TemperatureDeltaUnit td)
                    TemperatureDelta = td;

                if (this.PowerEnabled && dic[Units.UnitType.Power] is Units.PowerUnit PowerV)
                    Power = PowerV;
                if (this.EnergyEnabled && dic[Units.UnitType.Energy] is Units.EnergyUnit EnergyV)
                    Energy = EnergyV;
                if (this.PowerDensityEnabled && dic[Units.UnitType.PowerDensity] is Units.HeatFluxUnit PowerDensityV)
                    PowerDensity = PowerDensityV;
                if (this.AirFlowRateEnabled && dic[Units.UnitType.AirFlowRate] is Units.VolumeFlowUnit AirFlowRateV)
                    AirFlowRate = AirFlowRateV;
                if (this.PeopleDensityEnabled && dic[Units.UnitType.PeopleDensity] is Units.ReciprocalAreaUnit PeopleDensityV)
                    PeopleDensity = PeopleDensityV;

                if (this.AirFlowRateAreaEnabled && dic[Units.UnitType.AirFlowRateArea] is Units.VolumeFlowPerAreaUnit AirFlowRateAreaV)
                    AirFlowRateArea = AirFlowRateAreaV;
                if (this.SpeedEnabled && dic[Units.UnitType.Speed] is Units.SpeedUnit SpeedV)
                    Speed = SpeedV;
                if (this.IlluminanceEnabled && dic[Units.UnitType.Illuminance] is Units.IlluminanceUnit IlluminanceV)
                    Illuminance = IlluminanceV;

            }
        }

        public UnitSettingViewModel(Dictionary<Units.UnitType, Enum> hardcodedUnits)
        {
            LengthEnabled = !GetUnitOrDefault<Units.LengthUnit>(hardcodedUnits, Units.UnitType.Length, out var l);
            Length = l;
            AreaEnabled = !GetUnitOrDefault<Units.AreaUnit>(hardcodedUnits, Units.UnitType.Area, out var a);
            Area = a;
            VolumeEnabled = !GetUnitOrDefault<Units.VolumeUnit>(hardcodedUnits, Units.UnitType.Volume, out var v);
            Volume = v;
            TemperatureEnabled = !GetUnitOrDefault<Units.TemperatureUnit>(hardcodedUnits, Units.UnitType.Temperature, out var t);
            Temperature = t;
            TemperatureDeltaEnabled = !GetUnitOrDefault<Units.TemperatureDeltaUnit>(hardcodedUnits, Units.UnitType.TemperatureDelta, out var td);
            TemperatureDelta = td;

            PowerEnabled = !GetUnitOrDefault<Units.PowerUnit>(hardcodedUnits, Units.UnitType.Power, out var p);
            Power = p;
            EnergyEnabled = !GetUnitOrDefault<Units.EnergyUnit>(hardcodedUnits, Units.UnitType.Energy, out var e);
            Energy = e;
            PowerDensityEnabled = !GetUnitOrDefault<Units.HeatFluxUnit>(hardcodedUnits, Units.UnitType.PowerDensity, out var pd);
            PowerDensity = pd;
            AirFlowRateEnabled = !GetUnitOrDefault<Units.VolumeFlowUnit>(hardcodedUnits, Units.UnitType.AirFlowRate, out var af);
            AirFlowRate = af;
            PeopleDensityEnabled = !GetUnitOrDefault<Units.ReciprocalAreaUnit>(hardcodedUnits, Units.UnitType.PeopleDensity, out var pp);
            PeopleDensity = pp;

            AirFlowRateAreaEnabled = !GetUnitOrDefault<Units.VolumeFlowPerAreaUnit>(hardcodedUnits, Units.UnitType.AirFlowRateArea, out var aa);
            AirFlowRateArea = aa;
            SpeedEnabled = !GetUnitOrDefault<Units.SpeedUnit>(hardcodedUnits, Units.UnitType.Speed, out var s);
            Speed = s;
            IlluminanceEnabled = !GetUnitOrDefault<Units.IlluminanceUnit>(hardcodedUnits, Units.UnitType.Illuminance, out var i);
            Illuminance = i;
        }

        public void ApplySetting()
        {
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Length, this.Length);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Area, this.Area);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Volume, this.Volume);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Temperature, this.Temperature);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.TemperatureDelta, this.TemperatureDelta);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Power, this.Power);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Energy, this.Energy);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.PowerDensity, this.PowerDensity);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.AirFlowRate, this.AirFlowRate);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.PeopleDensity, this.PeopleDensity);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.AirFlowRateArea, this.AirFlowRateArea);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Speed, this.Speed);
            Units.CustomUnitSettings.TryAddValue(Units.UnitType.Illuminance, this.Illuminance);
        }

        private static string GetAbbr(Enum u)
        {
            var unitsNetUnit = ToUnitsNetEnum(u);
            var v = Convert.ToInt32(unitsNetUnit);
            var t = unitsNetUnit.GetType();
            var abb = UnitsNet.UnitAbbreviationsCache.Default.GetDefaultAbbreviation(t, v);
            return $"[{abb}]";
        }

        private static bool GetUnitOrDefault<T>(Dictionary<Units.UnitType, Enum> dic, Units.UnitType t, out T v)
        {
            v = default;

            if (dic == null || !dic.Any())
            {
                // get default value
                if (Units.CustomUnitSettings.TryGetValue(t, out var custom) && custom is T cvt) 
                    v = cvt;
                return false;
            }
            else if(dic.TryGetValue(t, out var value) && value is T vt)
            {
                v = vt;
                return true;
            }
            else
            {
                return false;
            }
               
        }

        private static Enum ToUnitsNetEnum(Enum inputEnum)
        {
            if (inputEnum == default)
                return inputEnum;

            var t = inputEnum.GetType().Name.ToString();
            var displayUnitType = typeof(UnitsNet.Angle).Assembly.GetType($"UnitsNet.Units.{t}");
            var ee = Enum.Parse(displayUnitType, inputEnum.ToString()) as Enum;
            return ee;
        }

        
    }
}
