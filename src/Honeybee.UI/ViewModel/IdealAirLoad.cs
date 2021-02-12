using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Honeybee.UI
{
    public class IdealAirLoadViewModel : ViewModelBase
    {

        private string _name;
        public string Name
        {
            get => _name ?? $"IdealAirLoad {Guid.NewGuid().ToString().Substring(0,5)}";
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _name = value, nameof(Name));
            }
        }

        private IEnumerable<string> _economizers = Enum.GetNames(typeof(AllAirEconomizerType)).ToList();
        public IEnumerable<string> Economizers => _economizers;

        private string _economizer;
        public string Economizer
        {
            get => _economizer ?? Economizers.First();
            set
            {
                Set(() => _economizer = value, nameof(Economizer));
            }
        }

        private bool _DCV;
        public bool DCV
        {
            get => _DCV;
            set
            {
                Set(() => _DCV = value, nameof(DCV));
            }
        }

        private bool _economizerEnabled = true;
        public bool EconomizerVisable
        {
            get => _economizerEnabled;
            set
            {
                Set(() => _economizerEnabled = value, nameof(EconomizerVisable));
            }
        }

        private double _sensibleHR = 0.5;
        public double SensibleHR
        {
            get => _sensibleHR;
            set
            {
                Set(() => _sensibleHR = value, nameof(SensibleHR));
            }
        }

     
        private double _latentHR = 0.5;
        public double LatentHR
        {
            get => _latentHR;
            set
            {
                Set(() => _latentHR = value, nameof(LatentHR));
            }
        }

       
        private double _heatingAirTemperature = 0.5;
        public double HeatingAirTemperature
        {
            get => _heatingAirTemperature;
            set
            {
                Set(() => _heatingAirTemperature = value, nameof(HeatingAirTemperature));
            }
        }

        private double _coolingAirTemperature = 0.5;
        public double CoolingAirTemperature
        {
            get => _coolingAirTemperature;
            set
            {
                Set(() => _coolingAirTemperature = value, nameof(CoolingAirTemperature));
            }
        }

        private double _heatingLimit = 0.5;
        public double HeatingLimit
        {
            get => _heatingLimit;
            set
            {
                Set(() => _heatingLimit = value, nameof(HeatingLimit));
                this.HeatingLimitNumber = true;
            }
        }

        private double _coolingLimit = 0.5;
        public double CoolingLimit
        {
            get => _coolingLimit;
            set
            {
                Set(() => _coolingLimit = value, nameof(CoolingLimit));
                this.CoolingLimitNumber = true;
            }
        }

        private bool _heatingLimitAutosized = true;
        public bool HeatingLimitAutosized
        {
            get => _heatingLimitAutosized;
            set
            {
                Set(() => _heatingLimitAutosized = value, nameof(HeatingLimitAutosized));
                if (value == true)
                {
                    this.HeatingLimitNoLimit = false;
                    this.HeatingLimitNumber = false;
                }
            }
        }

        private bool _coolingLimitAutosized = true;
        public bool CoolingLimitAutosized
        {
            get => _coolingLimitAutosized;
            set
            {
                Set(() => _coolingLimitAutosized = value, nameof(CoolingLimitAutosized));
                if (value == true)
                {
                    this.CoolingLimitNoLimit = false;
                    this.CoolingLimitNumber = false;
                }
            }
        }


        private bool _heatingLimitNoLimit;
        public bool HeatingLimitNoLimit
        {
            get => _heatingLimitNoLimit;
            set
            {
                Set(() => _heatingLimitNoLimit = value, nameof(HeatingLimitNoLimit));
                if (value == true)
                {
                    this.HeatingLimitAutosized = false;
                    this.HeatingLimitNumber = false;
                }
            }
        }

        private bool _coolingLimitNoLimit;
        public bool CoolingLimitNoLimit
        {
            get => _coolingLimitNoLimit;
            set
            {
                Set(() => _coolingLimitNoLimit = value, nameof(CoolingLimitNoLimit));
                if (value == true)
                {
                    this.CoolingLimitAutosized = false;
                    this.CoolingLimitNumber = false;
                }
            }
        }

        private bool _heatingLimitNumber;
        public bool HeatingLimitNumber
        {
            get => _heatingLimitNumber;
            set
            {
                Set(() => _heatingLimitNumber = value, nameof(HeatingLimitNumber));
                if (value == true)
                {
                    this.HeatingLimitAutosized = false;
                    this.HeatingLimitNoLimit = false;
                }
            }
        }

        private bool _coolingLimitNumber;
        public bool CoolingLimitNumber
        {
            get => _coolingLimitNumber;
            set
            {
                Set(() => _coolingLimitNumber = value, nameof(CoolingLimitNumber));
                if (value == true)
                {
                    this.CoolingLimitAutosized = false;
                    this.CoolingLimitNoLimit = false;
                }
            }
        }



        public IdealAirLoadViewModel(HoneybeeSchema.IdealAirSystemAbridged hvac)
        {
            // Add a new
            if (hvac == null)
                return;

            // Editing existing obj
            this.Name = hvac.DisplayName ?? $"Ideal Air System {Guid.NewGuid().ToString().Substring(0, 8)}";
            this.DCV = hvac.DemandControlledVentilation;
            this.Economizer = hvac.EconomizerType.ToString();

            this.CoolingAirTemperature = hvac.CoolingAirTemperature;
            this.HeatingAirTemperature = hvac.HeatingAirTemperature;

            this.SensibleHR = hvac.SensibleHeatRecovery;
            this.LatentHR = hvac.LatentHeatRecovery;

            if (hvac.CoolingLimit == null || hvac.CoolingLimit.Obj is Autosize)
                this.CoolingLimitAutosized = true;
            else if (hvac.CoolingLimit.Obj is double cln)
                this.CoolingLimit = cln;
            else
                this.CoolingLimitNoLimit = true;

            if (hvac.HeatingLimit == null || hvac.HeatingLimit.Obj is Autosize)
                this.HeatingLimitAutosized = true;
            else if (hvac.HeatingLimit.Obj is double htn)
                this.HeatingLimit = htn;
            else
                this.HeatingLimitNoLimit = true;

        }


        public IdealAirSystemAbridged GreateHvac(HoneybeeSchema.IdealAirSystemAbridged existing = default)
        {
            
            var id = Guid.NewGuid().ToString().Substring(0, 8);
            id = $"IdealAirSystem_{id}";
            id = existing == null ? id : existing.Identifier;

            var obj = new IdealAirSystemAbridged(id);

            obj.DisplayName = this.Name;
            if (Enum.TryParse<EconomizerType>(this.Economizer, out var eco))
                obj.EconomizerType = eco;

            obj.CoolingAirTemperature = this.CoolingAirTemperature;
            obj.HeatingAirTemperature = this.HeatingAirTemperature;

            obj.SensibleHeatRecovery = this.SensibleHR;
            obj.LatentHeatRecovery = this.LatentHR;

            if (this.HeatingLimitAutosized)
                obj.HeatingLimit = new Autosize();
            else if (this.HeatingLimitNoLimit)
                obj.HeatingLimit = new NoLimit();
            else
                obj.HeatingLimit = this.HeatingLimit;

            if (this.CoolingLimitAutosized)
                obj.CoolingLimit = new Autosize();
            else if (this.CoolingLimitNoLimit)
                obj.CoolingLimit = new NoLimit();
            else
                obj.CoolingLimit = this.CoolingLimit;

            obj.DemandControlledVentilation = this.DCV;

            return obj;
        }



    }





}
