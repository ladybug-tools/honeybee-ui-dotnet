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

        private double _sensibleHR;
        public double SensibleHR
        {
            get => _sensibleHR;
            set
            {
                Set(() => _sensibleHR = value, nameof(SensibleHR));
            }
        }

     
        private double _latentHR;
        public double LatentHR
        {
            get => _latentHR;
            set
            {
                Set(() => _latentHR = value, nameof(LatentHR));
            }
        }

       
        private double _heatingAirTemperature;
        public double HeatingAirTemperature
        {
            get => _heatingAirTemperature;
            set
            {
                Set(() => _heatingAirTemperature = value, nameof(HeatingAirTemperature));
            }
        }

        private double _coolingAirTemperature;
        public double CoolingAirTemperature
        {
            get => _coolingAirTemperature;
            set
            {
                Set(() => _coolingAirTemperature = value, nameof(CoolingAirTemperature));
            }
        }

        private double _heatingLimit;
        public double HeatingLimit
        {
            get => _heatingLimit;
            set
            {
                Set(() => _heatingLimit = value, nameof(HeatingLimit));
                this.HeatingLimitNumber = true;
            }
        }

        private double _coolingLimit;
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

        private string _HeatingAvaliabilityScheduleID;
        private OptionalButtonViewModel _HeatingAvaliabilitySchedule;
        public OptionalButtonViewModel HeatingAvaliabilitySchedule
        {
            get => _HeatingAvaliabilitySchedule;
            set { this.Set(() => _HeatingAvaliabilitySchedule = value, nameof(HeatingAvaliabilitySchedule)); }
        }


        private string _CoolingAvaliabilityScheduleID;
        private OptionalButtonViewModel _CoolingAvaliabilitySchedule;
        public OptionalButtonViewModel CoolingAvaliabilitySchedule
        {
            get => _CoolingAvaliabilitySchedule;
            set { this.Set(() => _CoolingAvaliabilitySchedule = value, nameof(CoolingAvaliabilitySchedule)); }
        }


        private ModelEnergyProperties _libSource;
        private Dialog_IdealAirLoad _control;
        public IdealAirLoadViewModel(ModelEnergyProperties libSource, HoneybeeSchema.IdealAirSystemAbridged hvac, Dialog_IdealAirLoad control)
        {
            _libSource = libSource;
            _control = control;

            // Add a new
            if (hvac == null)
                throw new ArgumentNullException(nameof(hvac));


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

            //AvaliabilitySchedule
            this.HeatingAvaliabilitySchedule = new OptionalButtonViewModel((n) => _HeatingAvaliabilityScheduleID = n?.Identifier);
            var heatingSch = libSource.ScheduleList.FirstOrDefault(_ => _.Identifier == hvac.HeatingAvailability);
            heatingSch = heatingSch ?? GetDummyScheduleObj(hvac.HeatingAvailability);
            this.HeatingAvaliabilitySchedule.SetPropetyObj(heatingSch);


            this.CoolingAvaliabilitySchedule = new OptionalButtonViewModel((n) => _CoolingAvaliabilityScheduleID = n?.Identifier);
            var CoolingSch = libSource.ScheduleList.FirstOrDefault(_ => _.Identifier == hvac.CoolingAvailability);
            CoolingSch = CoolingSch ?? GetDummyScheduleObj(hvac.CoolingAvailability);
            this.CoolingAvaliabilitySchedule.SetPropetyObj(CoolingSch);

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

            obj.HeatingAvailability = this._HeatingAvaliabilityScheduleID;
            obj.CoolingAvailability = this._CoolingAvaliabilityScheduleID;

            return obj;
        }

        public RelayCommand HeatingAvaliabilityCommand => new RelayCommand(() =>
        {
            var lib = _libSource;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                this.HeatingAvaliabilitySchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand RemoveHeatingAvaliabilityCommand => new RelayCommand(() =>
        {
            this.HeatingAvaliabilitySchedule.SetPropetyObj(null);
        });

        public RelayCommand CoolingAvaliabilityCommand => new RelayCommand(() =>
        {
            var lib = _libSource;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                this.CoolingAvaliabilitySchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand RemoveCoolingAvaliabilityCommand => new RelayCommand(() =>
        {
            this.CoolingAvaliabilitySchedule.SetPropetyObj(null);
        });

    }





}
