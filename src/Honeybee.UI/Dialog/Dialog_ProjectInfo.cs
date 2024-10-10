// Ignore Spelling: Epw

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

    public class Dialog_ProjectInfo : Dialog<HB.ProjectInfo>
    {


        //double north = 0.0,
        //List<string> weatherUrls = null,
        //Location location = null,
        //ClimateZones ashraeClimateZone = (ClimateZones)0,
        //List<BuildingTypes> buildingType = null,
        //List<EfficiencyStandards> vintage = null

        public Dialog_ProjectInfo(HB.ProjectInfo projInfo, Func<ProjectInfo, ProjectInfo>  getInfoFromEpw =default)
        {
            try
            {
                var _hbobj = projInfo;
                var _vm = new ProjectInfoViewModel(projInfo, getInfoFromEpw, this);
                var hbType = typeof(HB.ProjectInfo);

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
                    => Close(_vm.GetHBObj());

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();


                // North
                var northNum = new NumericStepper() { };
                northNum.MinValue = -360;
                northNum.MaxValue = 360;
                northNum.MaximumDecimalPlaces = 2;
                northNum.Bind(_ => _.Value, _vm, _ => _.North);

                var nl = new Label() { Text = "North" };
                nl.Bind(_ => _.ToolTip, _vm, _ => _.NorthTip);
                infoLayout.AddSeparateRow(nl);
                infoLayout.AddSeparateRow(northNum);


                // Weather files
                var weatherFiles = new GridView();
                weatherFiles.Height = 100;
                weatherFiles.Bind(_=>_.DataStore, _vm, _ => _.WeatherFiles);
                weatherFiles.SelectedItemBinding.Bind( _vm, _ => _.WeatherFile);

                //weatherFiles.Bind(_ => _.selected, _vm, _ => _.WeatherFile);
                weatherFiles.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<string, string>(r => System.IO.Path.GetFileName(r)) },
                    HeaderText = "Name",
                    Width = 200
                }); 
                weatherFiles.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<string, string>(r => r) },
                    HeaderText = "Url/Path"
                });
                var addWea = new Button() { Text = "+", Width = 20, Height = 20 };
                addWea.Command = _vm.AddEpwCommand;
                var removeWea = new Button() { Text = "-", Width = 20, Height = 20 };
                removeWea.Command = _vm.RemoveEpwCommand;


                var genLocation = new Button() { Text = "Get location from EPW" };
                genLocation.ToolTip = "Get location data from the selected EPW file";
                genLocation.Command = _vm.EpwLocationReaderCommand;
                genLocation.Bind(_ => _.Enabled, _vm, _ => _.HasEpwReader);

                var weal = new Label() { Text = "Weather Urls/Files" };
                weal.Bind(_ => _.ToolTip, _vm, _ => _.WeatherFile);
                infoLayout.AddSeparateRow(weal, null, addWea, removeWea);
                infoLayout.AddSeparateRow(weatherFiles);
                infoLayout.AddSeparateRow(genLocation);



                // List<BuildingTypes> buildingType
                var bds = new GridView();
                bds.Height = 45;
                bds.ShowHeader = false;

                bds.Bind(_ => _.DataStore, _vm, _ => _.BuildingTypes);
                bds.SelectedItemBinding.Bind(_vm, _ => _.BuildingType);
                bds.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<string, string>(r => r) },
                    Width = 350
                });
                var addBd = new Button() { Text = "+", Width = 20, Height = 20 };
                addBd.Command = _vm.AddBuildingTypeCommand;
                var removeBd = new Button() { Text = "-", Width = 20, Height = 20 };
                removeBd.Command = _vm.RemoveBuildingTypeCommand;

                var btl = new Label() { Text = "Building Type" };
                btl.Bind(_ => _.ToolTip, _vm, _ => _.BuildingTypesTip);
                infoLayout.AddSeparateRow(btl, null, addBd, removeBd);
                infoLayout.AddSeparateRow(bds);


                // List<EfficiencyStandards> vintage = null
                var vtg = new GridView();
                vtg.Height = 45;
                vtg.ShowHeader = false;

                vtg.Bind(_ => _.DataStore, _vm, _ => _.Vintages);
                vtg.SelectedItemBinding.Bind(_vm, _ => _.Vintage);

                vtg.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<string, string>(r => r) },
                    Width = 350
                });
                var addVtg = new Button() { Text = "+", Width = 20, Height = 20 };
                addVtg.Command = _vm.AddVintageCommand;
                var removeVtg = new Button() { Text = "-", Width = 20, Height = 20 };
                removeVtg.Command = _vm.RemoveVintageCommand;

                var effl = new Label() { Text = "Building Vintage" };
                effl.Bind(_ => _.ToolTip, _vm, _ => _.VintageTip);
                infoLayout.AddSeparateRow(effl, null, addVtg, removeVtg);
                infoLayout.AddSeparateRow(vtg);


                // Location
                _hbobj.Location = _hbobj.Location ?? new HB.Location();
                var loc = _hbobj.Location;
           
                var gpPanel = new DynamicLayout();
                //gpPanel.DefaultSpacing = new Size(3, 3);
                gpPanel.DefaultPadding = new Padding(5,3);

                
                // Climate Zone
                var climateZone = new DropDown();
                climateZone.Bind(_ => _.DataStore, _vm, _ => _.ClimateZones);
                climateZone.SelectedValueBinding.Bind(_vm, _ => _.ClimateZone);
                
                var czl = new Label() { Text = "Climate Zone" };
                czl.Bind(_ => _.ToolTip, _vm, _ => _.ClimateZoneTip);
                gpPanel.AddSeparateRow(czl);
                gpPanel.AddSeparateRow(climateZone);


                var city = new TextBox();
                city.Bind(_ => _.Text, _vm, _ => _.City);
                var cityl = new Label() { Text = "City" };
                cityl.Bind(_ => _.ToolTip, _vm, _ => _.CityTip);
                gpPanel.AddSeparateRow(cityl);
                gpPanel.AddSeparateRow(city);


                var lat = new NumericStepper() { };
                lat.Bind(_ => _.Value, _vm, _ => _.Latitude);
                var ll = new Label() { Text = "Latitude" };
                ll.Bind(_ => _.ToolTip, _vm, _ => _.LatitudeTip);
                gpPanel.AddSeparateRow(ll);
                gpPanel.AddSeparateRow(lat);


                var lon = new NumericStepper() { };
                lon.Bind(_ => _.Value, _vm, _ => _.Longitude);
                var Longitudel = new Label() { Text = "Longitude" };
                Longitudel.Bind(_ => _.ToolTip, _vm, _ => _.LongitudeTip);
                gpPanel.AddSeparateRow(Longitudel);
                gpPanel.AddSeparateRow(lon);

                var Elevation = new NumericStepper() { };
                Elevation.Bind(_ => _.Value, _vm, _ => _.Elevation);
                var Elevationl = new Label() { Text = "Elevation" };
                Elevationl.Bind(_ => _.ToolTip, _vm, _ => _.ElevationTip);
                gpPanel.AddSeparateRow(Elevationl);
                gpPanel.AddSeparateRow(Elevation);

                var TimeZone = new NumericStepper() { };
                TimeZone.DecimalPlaces = 0;
                var timeZoneAuto = new CheckBox() { Text = "Auto Calculate" };
                timeZoneAuto.Bind(_ => _.Checked, _vm, _ => _.IsTimeZoneAutoCalculate);


                TimeZone.Bind(_ => _.Value, _vm, _ => _.TimeZone);
                TimeZone.Bind(_ => _.Enabled, _vm, _ => _.TimeZoneEnabled);
                var TimeZonel = new Label() { Text = "Time Zone" };
                TimeZonel.Bind(_ => _.ToolTip, _vm, _ => _.TimeZoneTip);
                gpPanel.AddSeparateRow(TimeZonel);
                gpPanel.AddSeparateRow(timeZoneAuto);
                gpPanel.AddSeparateRow(TimeZone);


                var Source = new TextBox();
                Source.Bind(_ => _.Text, _vm, _ => _.Source);
                var Sourcel = new Label() { Text = "Source" };
                Sourcel.Bind(_ => _.ToolTip, _vm, _ => _.SourceTip);
                gpPanel.AddSeparateRow(Sourcel);
                gpPanel.AddSeparateRow(Source);

                var StationId = new TextBox();
                StationId.Bind(_ => _.Text, _vm, _ => _.StationId);
                var StationIdl = new Label() { Text = "Station Id" };
                StationIdl.Bind(_ => _.ToolTip, _vm, _ => _.StationIdTip);
                gpPanel.AddSeparateRow(StationIdl);
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
                    Dialog_Message.ShowFullMessage(this, _vm.GetHBObj().ToJson(true));
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
