using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;
using System.Linq;
using System.Collections.Generic;
using HoneybeeSchema;
using UnitsNet;

namespace Honeybee.UI
{
    internal class Dialog_BuildingType : Dialog<HB.BuildingTypes>
    {
        public Dialog_BuildingType()
        {

            var _hbobj = (HB.BuildingTypes)0;


            var layout = new DynamicLayout();
            //layout.DefaultSpacing = new Size(3, 15);
            layout.DefaultPadding = new Padding(5);

            Resizable = false;
            Title = $"Building Types - {DialogHelper.PluginName}";
            Width = 400;
            //WindowStyle = WindowStyle.Default;
            //MinimumSize = new Size(450, 620);
            this.Icon = DialogHelper.HoneybeeIcon;

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e)
                => Close(_hbobj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            // Building type
            var effStdItems = Enum.GetValues(typeof(HB.BuildingTypes)).Cast<HB.BuildingTypes>().Select(_ => _.ToString()).ToList();
            effStdItems.Insert(0, "<None>");
            var effStdDP = new DropDown();
            effStdDP.DataStore = effStdItems;
            effStdDP.SelectedValueBinding.Bind(
                Binding.Delegate(() =>
                {
                    var o = _hbobj.ToString();
                    o = o == "0" ? "<None>" : o;
                    return (object)o;
                },
                v =>
                {
                    Enum.TryParse<HB.BuildingTypes>(v?.ToString(), out var cz);
                    _hbobj = cz;
                }));

            layout.AddRow("Building Types:");
            layout.AddRow(effStdDP);
            layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
            layout.AddRow(null);

            Content = layout;


        }
    }

    internal class Dialog_EfficiencyStandards : Dialog<HB.EfficiencyStandards>
    {
        public Dialog_EfficiencyStandards()
        {

            var _hbobj = (HB.EfficiencyStandards)0;


            var layout = new DynamicLayout();
            //layout.DefaultSpacing = new Size(3, 15);
            layout.DefaultPadding = new Padding(5);

            Resizable = false;
            Title = $"Efficiency Standards - {DialogHelper.PluginName}";
            //WindowStyle = WindowStyle.Default;
            //MinimumSize = new Size(450, 620);
            Width = 400;
            this.Icon = DialogHelper.HoneybeeIcon;

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e)
                => Close(_hbobj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            // Building type
            var effStdItems = Enum.GetValues(typeof(HB.EfficiencyStandards)).Cast<HB.EfficiencyStandards>().Select(_ => _.ToString()).ToList();
            effStdItems.Insert(0, "<None>");
            var effStdDP = new DropDown();
            effStdDP.DataStore = effStdItems;
            effStdDP.SelectedValueBinding.Bind(
                Binding.Delegate(() =>
                {
                    var o = _hbobj.ToString();
                    o = o == "0" ? "<None>" : o;
                    return (object)o;
                },
                v =>
                {
                    Enum.TryParse<HB.EfficiencyStandards>(v?.ToString(), out var cz);
                    _hbobj = cz;
                }));

            layout.AddRow("Efficiency Standards:");
            layout.AddRow(effStdDP);
            layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
            layout.AddRow(null);

            Content = layout;


        }
    }

    internal class Dialog_WeatherFile : Dialog<string>
    {
        public Dialog_WeatherFile(string url)
        {

            var _hbobj = url;


            var layout = new DynamicLayout();
            layout.DefaultPadding = new Padding(5);

            Resizable = false;
            Title = $"Weather Url/File - {DialogHelper.PluginName}";
            Width = 400;
            this.Icon = DialogHelper.HoneybeeIcon;

            DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e)
                => Close(_hbobj);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();


            // Building type
            var effStdItems = new TextBox();
            effStdItems.Text = _hbobj;
            effStdItems.TextChanged += (s, e) => _hbobj = effStdItems.Text;

            layout.AddRow("Weather Url/File Path:");
            layout.AddRow(effStdItems);
            layout.AddSeparateRow(null, this.DefaultButton, this.AbortButton, null);
            layout.AddRow(null);

            Content = layout;


        }
    }


    public class Dialog_ProjectInfo : Dialog<HB.ProjectInfo>
    {
        //double north = 0.0,
        //List<string> weatherUrls = null,
        //Location location = null,
        //ClimateZones ashraeClimateZone = (ClimateZones)0,
        //List<BuildingTypes> buildingType = null,
        //List<EfficiencyStandards> vintage = null

        public Dialog_ProjectInfo(HB.ProjectInfo projInfo)
        {
            try
            {
                //_hbObj = HB.ModelEnergyProperties.Default.Materials.First(_ => _.Obj is HB.EnergyWindowMaterialGas).Obj as HB.EnergyWindowMaterialGas;
                var _hbobj = projInfo;

                var infoLayout = new DynamicLayout();
                infoLayout.DefaultSpacing = new Size(3, 3);
                infoLayout.DefaultPadding = new Padding(5, 3);
                //layout.Height = 450;

                Resizable = false;
                Title = $"Project Information - {DialogHelper.PluginName}";
                Width = 450;
                //WindowStyle = WindowStyle.Default;
                //MinimumSize = new Size(450, 620);
                this.Icon = DialogHelper.HoneybeeIcon;


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e)
                    => Close(_hbobj);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();


                // North
                var northNum = new NumericStepper() { };
                northNum.MinValue = -360;
                northNum.MaxValue = 360;
                northNum.MaximumDecimalPlaces = 2;
                northNum.ValueBinding.Bind(Binding.Delegate(() => _hbobj.North, v => _hbobj.North = v));
                northNum.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.ProjectInfo), nameof(_hbobj.North)));

                infoLayout.AddSeparateRow("North");
                infoLayout.AddSeparateRow(northNum);

                // Weather files
                _hbobj.WeatherUrls = _hbobj.WeatherUrls?.Where(_=>!string.IsNullOrEmpty(_))?.ToList() ?? new List<string>();
                _hbobj.WeatherUrls = _hbobj.WeatherUrls?.Distinct()?.ToList();

                var weatherFiles = new GridView();
                weatherFiles.Height = 100;
                weatherFiles.DataStore = _hbobj.WeatherUrls;
                weatherFiles.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<string, string>(r => System.IO.Path.GetFileName( r)) },
                    HeaderText = "Name",
                    Width = 100
                }); 
                weatherFiles.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<string, string>(r => r) },
                    HeaderText = "Url/Path"
                });
                var addWea = new Button() { Text = "+", Width = 20, Height = 20 };
                addWea.Click += (s, e) =>
                {
                    var d = new Dialog_WeatherFile(null);
                    var rs = d.ShowModal(this);
                    if (!string.IsNullOrEmpty(rs))
                    {
                        _hbobj.WeatherUrls.Add(rs);
                        weatherFiles.DataStore = _hbobj.WeatherUrls;
                    }
                };
                var removeWea = new Button() { Text = "-", Width = 20, Height = 20 };
                removeWea.Click += (s, e) =>
                {
                    if (weatherFiles.SelectedRow < 0)
                    {
                        MessageBox.Show("Nothing is selected!");
                    }
                    else if (weatherFiles.SelectedRow < _hbobj.WeatherUrls.Count)
                    {
                        _hbobj.WeatherUrls.RemoveAt(weatherFiles.SelectedRow);
                        weatherFiles.DataStore = _hbobj.WeatherUrls;
                    }

                };
                infoLayout.AddSeparateRow("Weather Urls/Files", null, addWea, removeWea);
                infoLayout.AddSeparateRow(weatherFiles);

                // Climate Zone
                var climateZoneItems = Enum.GetValues(typeof(HB.ClimateZones)).Cast<HB.ClimateZones>().Select(_ => _.ToString()).ToList();
                climateZoneItems.Insert(0, "<None>");
                var climateZone = new DropDown();
                climateZone.DataStore = climateZoneItems;
                climateZone.SelectedValueBinding.Bind(
                    Binding.Delegate(() =>
                    {
                        var o = _hbobj.AshraeClimateZone.ToString();
                        o = o == "0" ? "<None>" : o;
                        return (object)o;
                    },
                    v =>
                    {
                        Enum.TryParse<HB.ClimateZones>(v?.ToString(), out var cz);
                        _hbobj.AshraeClimateZone = cz;
                    }));
                climateZone.ToolTip = Utility.NiceDescription(HB.SummaryAttribute.GetSummary(typeof(HB.ProjectInfo), nameof(_hbobj.AshraeClimateZone)));
                infoLayout.AddSeparateRow("Climate Zone");
                infoLayout.AddSeparateRow(climateZone);


                // List<BuildingTypes> buildingType
                _hbobj.BuildingType = _hbobj.BuildingType?.Distinct()?.ToList() ?? new List<BuildingTypes>();

                var bds = new GridView();
                bds.Height = 45;
                bds.ShowHeader = false;
                bds.DataStore = _hbobj.BuildingType.OfType<object>();
                bds.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<BuildingTypes, string>(r => r.ToString()) },
                    Width = 350
                });
                var addBd = new Button() { Text = "+", Width = 20, Height = 20 };
                addBd.Click += (s, e) => 
                {
                    var d = new Dialog_BuildingType();
                    var rs = d.ShowModal(this);
                    if (rs != 0)
                    {
                        _hbobj.BuildingType.Add(rs);
                        bds.DataStore = _hbobj.BuildingType.OfType<object>();
                    }
                };
                var removeBd = new Button() { Text = "-", Width = 20, Height = 20 };
                removeBd.Click += (s, e) =>
                {
                    if (bds.SelectedRow < 0)
                    {
                        MessageBox.Show("Nothing is selected!");
                    }
                    else if (bds.SelectedRow < _hbobj.BuildingType.Count)
                    {
                        _hbobj.BuildingType.RemoveAt(bds.SelectedRow);
                        bds.DataStore = _hbobj.BuildingType.OfType<object>();
                    }

                };
                infoLayout.AddSeparateRow("Building Type", null, addBd, removeBd);
                infoLayout.AddSeparateRow(bds);


                // List<EfficiencyStandards> vintage = null
                _hbobj.Vintage = _hbobj.Vintage?.Distinct()?.ToList() ?? new List<EfficiencyStandards>();

                var vtg = new GridView();
                vtg.Height = 45;
                vtg.ShowHeader = false;
                vtg.DataStore = _hbobj.Vintage.OfType<object>();
                vtg.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<EfficiencyStandards, string>(r => r.ToString()) },
                    Width = 350
                });
                var addVtg = new Button() { Text = "+", Width = 20, Height = 20 };
                addVtg.Click += (s, e) =>
                {
                    var d = new Dialog_EfficiencyStandards();
                    var rs = d.ShowModal(this);
                    if (rs != 0)
                    {
                        _hbobj.Vintage.Add(rs);
                        vtg.DataStore = _hbobj.Vintage.OfType<object>();
                    }
                };
                var removeVtg = new Button() { Text = "-", Width = 20, Height = 20 };
                removeVtg.Click += (s, e) =>
                {
                    if (vtg.SelectedRow < 0)
                    {
                        MessageBox.Show("Nothing is selected!");
                    }
                    else if (vtg.SelectedRow < _hbobj.Vintage.Count)
                    {
                        _hbobj.Vintage.RemoveAt(vtg.SelectedRow);
                        vtg.DataStore = _hbobj.Vintage.OfType<object>();
                    }

                };
                infoLayout.AddSeparateRow("Vintage", null, addVtg, removeVtg);
                infoLayout.AddSeparateRow(vtg);


                infoLayout.AddRow(null);

                // Location
                _hbobj.Location = _hbobj.Location ?? new HB.Location();
                var loc = _hbobj.Location;
           
                var gpPanel = new DynamicLayout();
                gpPanel.DefaultSpacing = new Size(3, 3);
                gpPanel.DefaultPadding = new Padding(5);

                var city = new TextBox();
                city.TextBinding.Bind(Binding.Delegate(() => loc.City, v => loc.City = v));
                gpPanel.AddSeparateRow("City");
                gpPanel.AddSeparateRow(city);

                var lat = new NumericStepper() { };
                lat.ValueBinding.Bind(Binding.Delegate(() => loc.Latitude, v => loc.Latitude = v)); 
                gpPanel.AddSeparateRow("Latitude");
                gpPanel.AddSeparateRow(lat);


                var lon = new NumericStepper() { };
                lon.ValueBinding.Bind(Binding.Delegate(() => loc.Longitude, v => loc.Longitude = v));
                gpPanel.AddSeparateRow("Longitude");
                gpPanel.AddSeparateRow(lon);

                var Elevation = new NumericStepper() { };
                Elevation.ValueBinding.Bind(Binding.Delegate(() => loc.Elevation, v => loc.Elevation = v));
                gpPanel.AddSeparateRow("Elevation");
                gpPanel.AddSeparateRow(Elevation);

                var TimeZone = new NumericStepper() { };
                var timeZoneAuto = new CheckBox() { Text = "Auto Calculate" };
                timeZoneAuto.Checked = loc.TimeZone?.Obj is Autocalculate;
                timeZoneAuto.CheckedChanged += (s, e) =>
                {
                    var auto = timeZoneAuto.Checked.GetValueOrDefault();
                    TimeZone.Enabled = !auto;
                    if (auto) 
                        loc.TimeZone = new Autocalculate();
                    else 
                        loc.TimeZone = TimeZone.Value;
                };

                TimeZone.Enabled = !timeZoneAuto.Checked.GetValueOrDefault();
                TimeZone.ValueBinding.Bind(
                    Binding.Delegate(
                    () => loc.TimeZone?.Obj is double d ? d : 0,
                    v => loc.TimeZone = v)
                    );
                gpPanel.AddSeparateRow("TimeZone");
                gpPanel.AddSeparateRow(timeZoneAuto);
                gpPanel.AddSeparateRow(TimeZone);


                var Source = new TextBox();
                Source.TextBinding.Bind(Binding.Delegate(() => loc.Source, v => loc.Source = v));
                gpPanel.AddSeparateRow("Source");
                gpPanel.AddSeparateRow(Source);

                var StationId = new TextBox();
                StationId.TextBinding.Bind(Binding.Delegate(() => loc.StationId, v => loc.StationId = v));
                gpPanel.AddSeparateRow("Station Id");
                gpPanel.AddSeparateRow(StationId);

                gpPanel.AddRow(null);

                var tap = new TabControl();
                tap.Height = 450;
                var tapPage1 = new TabPage(infoLayout) { Text = "Information" };
                tap.Pages.Add(tapPage1);

                var location = new TabPage(gpPanel) { Text = "Location" };
                tap.Pages.Add(location);


                var mainLayout = new DynamicLayout();
                mainLayout.DefaultSpacing = new Size(3, 3);
                mainLayout.DefaultPadding = new Padding(5);
                mainLayout.AddSeparateRow(tap);

                var jsonBtn = new Button();
                jsonBtn.Text = "Data";
                jsonBtn.Click += (s, e) =>
                {
                    Dialog_Message.ShowFullMessage(this, _hbobj.ToJson(true));
                };

                mainLayout.AddSeparateRow(jsonBtn, null, this.DefaultButton, this.AbortButton);

                mainLayout.AddRow(null);

                Content = mainLayout;
            }
            catch (Exception e)
            {
                Dialog_Message.Show(this, e);
                //throw e;
            }
        }



    }
}
