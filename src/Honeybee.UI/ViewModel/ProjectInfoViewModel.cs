// Ignore Spelling: Epw

using Eto.Forms;
using HB = HoneybeeSchema;
using HoneybeeSchema;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Honeybee.UI
{
    public class ProjectInfoViewModel : ViewModelBase
    {
        private HB.ProjectInfo _hbObj;
        public double North
        {
            get => _hbObj.North;
            set => Set(() => _hbObj.North = value, nameof(North));
        }

        private string _NorthTip;
        public string NorthTip
        {
            get => _NorthTip;
            set => Set(() => _NorthTip = value, nameof(NorthTip));
        }



        private DataStoreCollection<string> _weatherFiles = new DataStoreCollection<string>();
        public DataStoreCollection<string> WeatherFiles
        {
            get => _weatherFiles;
            set => this.Set(() => _weatherFiles = value, nameof(_weatherFiles));
        }
        private string _WeatherFile;
        public string WeatherFile
        {
            get => _WeatherFile;
            set => Set(() => _WeatherFile = value, nameof(WeatherFile));
        }

        private string _WeatherFilesTip;
        public string WeatherFilesTip
        {
            get => _WeatherFilesTip;
            set => Set(() => _WeatherFilesTip = value, nameof(WeatherFilesTip));
        }


        private IEnumerable<string> _buildingTypeNames = Enum.GetNames(typeof(BuildingTypes)).ToList();
        private DataStoreCollection<string> _buildingTypes = new DataStoreCollection<string>();
        public DataStoreCollection<string> BuildingTypes
        {
            get => _buildingTypes;
            set => this.Set(() => _buildingTypes = value, nameof(_buildingTypes));
        }
        private string _BuildingType;
        public string BuildingType
        {
            get => _BuildingType;
            set => Set(() => _BuildingType = value, nameof(BuildingType));
        }

        private string _BuildingTypesTip;
        public string BuildingTypesTip
        {
            get => _BuildingTypesTip;
            set => Set(() => _BuildingTypesTip = value, nameof(BuildingTypesTip));
        }


        private IEnumerable<string> _vintageNames = Enum.GetNames(typeof(EfficiencyStandards)).ToList();
        private DataStoreCollection<string> _vintages = new DataStoreCollection<string>();
        public DataStoreCollection<string> Vintages
        {
            get => _vintages;
            set => this.Set(() => _vintages = value, nameof(_vintages));
        }
        private string _Vintage;
        public string Vintage
        {
            get => _Vintage;
            set => Set(() => _Vintage = value, nameof(Vintage));
        }

        private string _VintageTip;
        public string VintageTip
        {
            get => _VintageTip;
            set => Set(() => _VintageTip = value, nameof(VintageTip));
        }


        public ClimateZones ClimateZone
        {
            get => _hbObj.AshraeClimateZone;
            set => Set(() => _hbObj.AshraeClimateZone = value, nameof(ClimateZone));
        }
            
        private string _ClimateZoneTip;
        public string ClimateZoneTip
        {
            get => _ClimateZoneTip;
            set => Set(() => _ClimateZoneTip = value, nameof(ClimateZoneTip));
        }


        //string city = "-", double latitude = 0.0, double longitude = 0.0, AnyOf<Autocalculate, double> timeZone = null, double elevation = 0.0, string stationId = null, string source = null
        public string City
        {
            get => _hbObj.Location.City;
            set => Set(() => _hbObj.Location.City = value, nameof(City));
        }
        private string _CityTip;
        public string CityTip
        {
            get => _CityTip;
            set => Set(() => _CityTip = value, nameof(CityTip));
        }

        public double Latitude
        {
            get => _hbObj.Location.Latitude;
            set => Set(() => _hbObj.Location.Latitude = value, nameof(Latitude));
        }
        private string _LatitudeTip;
        public string LatitudeTip
        {
            get => _LatitudeTip;
            set => Set(() => _LatitudeTip = value, nameof(LatitudeTip));
        }

        public double Longitude
        {
            get => _hbObj.Location.Longitude;
            set => Set(() => _hbObj.Location.Longitude = value, nameof(Longitude));
        }
        private string _LongitudeTip;
        public string LongitudeTip
        {
            get => _LongitudeTip;
            set => Set(() => _LongitudeTip = value, nameof(LongitudeTip));
        }

        public bool IsTimeZoneAutoCalculate
        {
            get => _hbObj.Location.TimeZone?.Obj is Autocalculate;
            set => Set(() =>
            {
                if (value)
                    _hbObj.Location.TimeZone = new Autocalculate();
                else
                    TimeZone = 0;

                TimeZoneEnabled = !value;
            }
            ,nameof(IsTimeZoneAutoCalculate));
        }

        private bool _TimeZoneEnabled;
        public bool TimeZoneEnabled
        {
            get => _TimeZoneEnabled;
            set => Set(() => _TimeZoneEnabled = value, nameof(TimeZoneEnabled));
        }

        public double TimeZone
        {
            get => _hbObj.Location.TimeZone?.Obj is double dd ? dd : 0;
            set => Set(() => _hbObj.Location.TimeZone = value, nameof(TimeZone));
        }
        private string _TimeZoneTip;
        public string TimeZoneTip
        {
            get => _TimeZoneTip;
            set => Set(() => _TimeZoneTip = value, nameof(TimeZoneTip));
        }

        public double Elevation
        {
            get => _hbObj.Location.Elevation;
            set => Set(() => _hbObj.Location.Elevation = value, nameof(Elevation));
        }
        private string _ElevationTip;
        public string ElevationTip
        {
            get => _ElevationTip;
            set => Set(() => _ElevationTip = value, nameof(ElevationTip));
        }

        public string StationId
        {
            get => _hbObj.Location.StationId;
            set => Set(() => _hbObj.Location.StationId = value, nameof(StationId));
        }
        private string _StationIdTip;
        public string StationIdTip
        {
            get => _StationIdTip;
            set => Set(() => _StationIdTip = value, nameof(StationIdTip));
        }

        public string Source
        {
            get => _hbObj.Location.Source;
            set => Set(() => _hbObj.Location.Source = value, nameof(Source));
        }

        private string _SourceTip;
        public string SourceTip
        {
            get => _SourceTip;
            set => Set(() => _SourceTip = value, nameof(SourceTip));
        }

        public Func<ProjectInfo, ProjectInfo> GetInfoFromEpw { get; private set; }
        public bool HasEpwReader => GetInfoFromEpw != null;
        private Control _control;
        public ProjectInfoViewModel(HB.ProjectInfo projectInfo, Func<ProjectInfo, ProjectInfo> getInfoFromEpw, Control control)
        {
            _control = control;

            Update(projectInfo);
            this.GetInfoFromEpw = getInfoFromEpw;

            // gen doc tips

            var clsTp = typeof(ProjectInfo);
            this.NorthTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(clsTp, nameof(_hbObj.North)));
            this.WeatherFilesTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(clsTp, nameof(_hbObj.WeatherUrls)));
            this.ClimateZoneTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(clsTp, nameof(_hbObj.AshraeClimateZone)));
            this.VintageTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(clsTp, nameof(_hbObj.Vintage)));
            this.BuildingTypesTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(clsTp, nameof(_hbObj.BuildingType)));


            var locTp = typeof(Location);
            this.CityTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(locTp, nameof(_hbObj.Location.City))); 
            this.LatitudeTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(locTp, nameof(_hbObj.Location.Latitude)));
            this.LongitudeTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(locTp, nameof(_hbObj.Location.Longitude)));
            this.TimeZoneTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(locTp, nameof(_hbObj.Location.TimeZone)));
            this.SourceTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(locTp, nameof(_hbObj.Location.Source)));
            this.StationIdTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(locTp, nameof(_hbObj.Location.StationId)));
            this.ElevationTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(locTp, nameof(_hbObj.Location.Elevation)));


        }


        public void Update(HB.ProjectInfo projectInfo)
        {
            _hbObj = projectInfo;
            _hbObj.WeatherUrls = _hbObj.WeatherUrls ?? new System.Collections.Generic.List<string>();
            _hbObj.WeatherUrls = _hbObj.WeatherUrls?.Distinct()?.ToList();
            this.WeatherFiles.AddRange(_hbObj.WeatherUrls);

            _hbObj.BuildingType = _hbObj.BuildingType ?? new System.Collections.Generic.List<BuildingTypes>();
            _hbObj.BuildingType = _hbObj.BuildingType?.Distinct()?.ToList();
            this.BuildingTypes.AddRange(_hbObj.BuildingType.Select(_ => _.ToString()));

            _hbObj.Vintage = _hbObj.Vintage ?? new System.Collections.Generic.List<EfficiencyStandards>();
            _hbObj.Vintage = _hbObj.Vintage?.Distinct()?.ToList();
            this.Vintages.AddRange(_hbObj.Vintage.Select(_ => _.ToString()));

            _hbObj.Location = _hbObj.Location ?? new Location();
            var location = _hbObj.Location;

            this.City = location.City;
            this.Latitude = location.Latitude;
            this.Longitude = location.Longitude;

            location.TimeZone = location.TimeZone ?? new Autocalculate();

            if (location.TimeZone.Obj is double d)
                this.TimeZone = d;
            else
            {
                this.IsTimeZoneAutoCalculate = true;
            }
              

            this.Elevation = location.Elevation;
            this.StationId = location.StationId;
            this.Source = location.Source;
        }

        public ProjectInfo GetHBObj()
        {
            var hbObj = _hbObj.DuplicateProjectInfo();

            hbObj.WeatherUrls = this.WeatherFiles.OfType<string>().ToList();
            hbObj.BuildingType = this.BuildingTypes.Select(_=>(BuildingTypes)Enum.Parse(typeof(BuildingTypes), _)).ToList();
            hbObj.Vintage = this.Vintages.Select(_ => (EfficiencyStandards)Enum.Parse(typeof(EfficiencyStandards), _)).ToList();

            return hbObj;
        }

        public RelayCommand EpwLocationReaderCommand => new RelayCommand(() =>
        {
            try
            {
                var sel = this.WeatherFile;
                if (string.IsNullOrEmpty(sel))
                    throw new ArgumentException("Nothing is selected!");

                // only one weather file for getting location information
                var dummy = this.GetHBObj();
                var oldWeatherFiles = dummy.WeatherUrls;
                dummy.WeatherUrls = new List<string> { sel };

                var updatedObj = this.GetInfoFromEpw?.Invoke(dummy);
                if (updatedObj == null)
                    throw new ArgumentException("Invalid Project Information!");

                // add original weather files back
                updatedObj.WeatherUrls = oldWeatherFiles;
                Update(updatedObj);
            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
           
        });

        public RelayCommand AddEpwCommand => new RelayCommand(() =>
        {
            try
            {
                var d = new Dialog_WeatherFile(null);
                var rs = d.ShowModal(this._control);
                if (!string.IsNullOrEmpty(rs))
                {
                    this.WeatherFiles.Add(rs);
                }
            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
        });

        public RelayCommand RemoveEpwCommand => new RelayCommand(() =>
        {
            try
            {
                var sel = this.WeatherFile;
                if (string.IsNullOrEmpty(sel))
                    throw new ArgumentException("Nothing is selected!");

                this.WeatherFiles.Remove(sel);
            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
        });

        public RelayCommand AddBuildingTypeCommand => new RelayCommand(() =>
        {
            try
            {
                var d = new Dialog_BuildingType();
                var rs = d.ShowModal(this._control);
                if (rs != 0)
                {
                    this.BuildingTypes.Add(rs.ToString());
                }

            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
        });

        public RelayCommand RemoveBuildingTypeCommand => new RelayCommand(() =>
        {
            try
            {
                var sel = this.BuildingType;
                if (string.IsNullOrEmpty(sel))
                    throw new ArgumentException("Nothing is selected!");

                this.BuildingTypes.Remove(sel);
            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
        });

        public RelayCommand AddVintageCommand => new RelayCommand(() =>
        {
            try
            {
                var d = new Dialog_EfficiencyStandards();
                var rs = d.ShowModal(this._control);
                if (rs != 0)
                {
                    this.Vintages.Add(rs.ToString());
                }

            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
        });

        public RelayCommand RemoveVintageCommand => new RelayCommand(() =>
        {
            try
            {
                var sel = this.Vintage;
                if (string.IsNullOrEmpty(sel))
                    throw new ArgumentException("Nothing is selected!");

                this.Vintages.Remove(sel);
            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
        });


    }
}
