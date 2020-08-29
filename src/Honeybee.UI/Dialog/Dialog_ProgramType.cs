using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using HoneybeeSchema.Energy;

namespace Honeybee.UI
{

    public class Dialog_ProgramType: Dialog<HB.ProgramTypeAbridged>
    {
        private static ProgramTypeViewModel _vm;

        private static Panel _peopleGroup;
        private static Panel _lightingGroup;
        private static Panel _equipmentGroup; 
        private static Panel _gasEqpGroup;
        private static Panel _infiltrationGroup;
        private static Panel _ventilationGroup;
        private static Panel _setpoinGroup;

        private static IEnumerable<HB.Energy.ILoad> _loads;

        public static IEnumerable<HB.Energy.ILoad> Loads
        {
            get
            {
                if (_loads == null)
                {
                    var libObjs = new List<HB.Energy.ILoad>();
                    libObjs.AddRange(EnergyLibrary.DefaultPeopleLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultLightingLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultElectricEquipmentLoads);
                    libObjs.AddRange(EnergyLibrary.GasEquipmentLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultInfiltrationLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultVentilationLoads);
                    libObjs.AddRange(EnergyLibrary.DefaultSetpoints);
                    _loads = libObjs;
                }
            

                return _loads;
            }
        }

        private static IEnumerable<HB.IDdEnergyBaseModel> _schedules;

        public static IEnumerable<HB.IDdEnergyBaseModel> Schedules
        {
            get
            {
                //var libObjs = HB.Helper.EnergyLibrary.StandardsSchedules.ToList<HB.IDdEnergyBaseModel>();
                var inModelObjs = HB.Helper.EnergyLibrary.InModelEnergyProperties.Schedules
                    .Select(_ => _.Obj as HB.IDdEnergyBaseModel);

                //libObjs.AddRange(inModelObjs);
                _schedules = inModelObjs;

                return _schedules;
            }
        }

        public Dialog_ProgramType(HB.ProgramTypeAbridged ProgramType)
        {
            try
            {
                _vm = ProgramTypeViewModel.Instance;
                _vm.hbObj = ProgramType ?? new HB.ProgramTypeAbridged(identifier: Guid.NewGuid().ToString());

                Padding = new Padding(5);
                Resizable = true;
                Title = "Program Type - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 600);
                //Height = 800;
                this.Icon = DialogHelper.HoneybeeIcon;

                //Generate Library Panel
                var controls = GenLibraryPanel();
                var libControls = controls.allControls;
                var libraryLBox = controls.libraryLBox;

                //Func<HB.Energy.IIDdEnergyBaseModel> getSelectedSche = () =>
                //{
                //    if (libraryLBox.SelectedItem == null)
                //    {
                //        MessageBox.Show(this, "Nothing is selected from library");
                //        return null;
                //    }
                //    return libraryLBox.SelectedItem as HB.Energy.IIDdEnergyBaseModel;
                //};

                //Generate ProgramType Panel
                var pTypePanel = GenProgramTypePanel();

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(_vm.hbObj);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();


                var panelLeft = new DynamicLayout();
                panelLeft.DefaultPadding = new Padding(5);
                panelLeft.DefaultSpacing = new Size(5, 5);
                //panelLeft.Height = 600;
                //panelLeft.Width = 360;

                panelLeft.AddSeparateRow(pTypePanel);
                panelLeft.AddSeparateRow(null);
                panelLeft.AddSeparateRow(DefaultButton, AbortButton, null);
                panelLeft.AddSeparateRow(null);


               

                //// Add to program
                //var borrowBtn = new Button() { Text = "Add to program" };
                //borrowBtn.Click += (s, e) =>
                //{
                //    var selected = libraryLBox.SelectedItem;
                //    if (selected == null)
                //    {
                //        MessageBox.Show(this, "Nothing is selected from library");
                //        return;
                //    }
                        

                //    if (selected is PeopleAbridged pp)
                //    {
                //        _vm.People = pp;
                //        _peopleGroup.Content.Visible = true;
                //    }
                //    else if (selected is LightingAbridged lt)
                //    {
                //        _vm.Lighting = lt;
                //        _lightingGroup.Content.Visible = true;
                //    }
                //    else if (selected is ElectricEquipmentAbridged eq)
                //    {
                //        _vm.ElectricEquipment = eq;
                //        _equipmentGroup.Content.Visible = true;
                //    }
                //    else if (selected is GasEquipmentAbridged gs)
                //    {
                //        _vm.GasEquipment = gs;
                //        _gasEqpGroup.Content.Visible = true;
                //    }
                //    else if (selected is InfiltrationAbridged nf)
                //    {
                //        _vm.Infiltration = nf;
                //        _infiltrationGroup.Content.Visible = true;
                //    }
                //    else if (selected is VentilationAbridged vn)
                //    {
                //        _vm.Ventilation = vn;
                //        _ventilationGroup.Content.Visible = true;
                //    }
                //    else if (selected is SetpointAbridged sp)
                //    {
                //        _vm.Setpoint = sp;
                //        _setpoinGroup.Content.Visible = true;
                //    }
                //    else
                //    {
                //        MessageBox.Show(this, $"{selected.GetType().Name.Replace("Abridged", "")} cannot be added to program type directly!");
                //    }

                //};

                //var panelMiddle = new DynamicLayout();
                //panelMiddle.BackgroundColor = Color.FromArgb(150, 150, 150);
                //panelMiddle.DefaultPadding = new Padding(20);
                //panelMiddle.Width = 1;

               
                var panelRight = new DynamicLayout();
                //panelRight.BackgroundColor = Color.FromArgb(200, 200, 200);
                panelRight.DefaultPadding = new Padding(5);
                foreach (var item in libControls)
                {
                    panelRight.AddRow(item);
                }
                //panelRight.AddRow(borrowBtn);
                panelRight.AddRow(null);

                var rightTab = new TabControl();
                rightTab.Pages.Add(new TabPage(panelRight) { Text = "Library"});

                //var panelAll = new DynamicLayout() { };
                //panelAll.AddRow(panelLeft, panelRight);



                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    new TableRow(panelLeft, rightTab)
                }
                };


            }
            catch (Exception e)
            {
                throw e;
            }


        }





        private DynamicLayout GenProgramTypePanel()
        {
            _peopleGroup = GenPeoplePanel();

            _lightingGroup = GenLightingPanel();

            _equipmentGroup = GenEquipPanel();

            _gasEqpGroup = GenGasEquipPanel();

            _infiltrationGroup = GenInfiltrationPanel();

            _ventilationGroup = GenVentilaitonPanel();

            _setpoinGroup = GenSetpointPanel();

            // Json Data
            var hbData = new Button { Text = "HBData" };
            hbData.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, _vm.hbObj.ToJson(), "Honeybee Data");

            //Left panel
            //var panelLeft = new TableLayout() { DataContext = _vm };

            var pTypePanel = new DynamicLayout() { DataContext = _vm };
            pTypePanel.Height = 650;
            pTypePanel.Width = 340;
            pTypePanel.DefaultSpacing = new Size(5, 5);
            //pTypePanel.DefaultSpacing = new Size(10,10);

            var nameTbx = new TextBox();
            _vm.hbObj.DisplayName = _vm.hbObj.DisplayName ?? $"New ProgramType {_vm.hbObj.Identifier.Substring(0, 5)}";
            nameTbx.TextBinding.Bind(_vm.hbObj, c => c.DisplayName);
            pTypePanel.BeginScrollable(BorderType.None);

            pTypePanel.AddSeparateRow(new Label() { Text = "ID: " }, new Label() { Text = _vm.hbObj.Identifier, Enabled = false });
            pTypePanel.AddSeparateRow(new Label() { Text = "Name:" });
            pTypePanel.AddSeparateRow(nameTbx);
            pTypePanel.AddSpace();

            pTypePanel.AddSeparateRow(_peopleGroup);
            pTypePanel.AddSeparateRow(_lightingGroup);
            pTypePanel.AddSeparateRow(_equipmentGroup);
            pTypePanel.AddSeparateRow(_gasEqpGroup);
            pTypePanel.AddSeparateRow(_infiltrationGroup);
            pTypePanel.AddSeparateRow(_ventilationGroup);
            pTypePanel.AddSeparateRow(_setpoinGroup);
            pTypePanel.AddSpace();
            pTypePanel.AddSeparateRow(hbData);
            pTypePanel.AddSeparateRow(null);
            pTypePanel.EndScrollable();
            //panelLeft.EndVertical();

            return pTypePanel;
        }

        private Panel GenSetpointPanel()
        {

            //_vm.hbObj.Setpoint = _vm.hbObj.Setpoint ?? new SetpointAbridged(Guid.NewGuid().ToString(), "cooling sch", "heating sch");
            var isSptNull = _vm.hbObj.Setpoint == null;
            var spt = new List<Control>() { };
            spt.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.SPT_DisplayName));
            spt.AddRange(GenDropInInputControl("Cooling Schedule", (ProgramTypeViewModel m) => m.SPT_CoolingSchedule, (ProgramTypeViewModel m) => m.SPT_CoolingScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            spt.AddRange(GenDropInInputControl("Heating Schedule", (ProgramTypeViewModel m) => m.SPT_HeatingSchedule, (ProgramTypeViewModel m) => m.SPT_HeatingScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            spt.AddRange(GenDropInInputControl("Humidifying Schedule", (ProgramTypeViewModel m) => m.SPT_HumidifyingSchedule, (ProgramTypeViewModel m) => m.SPT_HumidifyingScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            spt.AddRange(GenDropInInputControl("Dehumidifying Schedule", (ProgramTypeViewModel m) => m.SPT_DehumidifyingSchedule, (ProgramTypeViewModel m) => m.SPT_DehumidifyingScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            return GenGroup("Setpoint", spt, isSptNull, (v) => _vm.Setpoint = v as HB.SetpointAbridged, typeof(HB.SetpointAbridged));
        }

        private Panel GenVentilaitonPanel()
        {
            //_vm.hbObj.Ventilation = _vm.hbObj.Ventilation ?? new VentilationAbridged(Guid.NewGuid().ToString());
            var isVntNull = _vm.hbObj.Ventilation == null;
            var vnt = new List<Control>() { };
            vnt.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.VNT_DisplayName));
            vnt.AddRange(GenInputControl("Flow/Area", (ProgramTypeViewModel m) => m.VNT_FlowPerArea));
            vnt.AddRange(GenDropInInputControl("Schedule", (ProgramTypeViewModel m) => m.VNT_Schedule, (ProgramTypeViewModel m) => m.VNT_ScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            vnt.AddRange(GenInputControl("Flow/Person", (ProgramTypeViewModel m) => m.VNT_FlowPerPerson));
            vnt.AddRange(GenInputControl("Flow/Zone", (ProgramTypeViewModel m) => m.VNT_FlowPerZone));
            vnt.AddRange(GenInputControl("AirChanges/Hour", (ProgramTypeViewModel m) => m.VNT_AirChangesPerHour));
            return GenGroup("Ventilation", vnt, isVntNull, (v) => _vm.Ventilation = v as HB.VentilationAbridged, typeof(HB.VentilationAbridged));
        }

        private Panel GenInfiltrationPanel()
        {
            //_vm.hbObj.Infiltration = _vm.hbObj.Infiltration ?? new InfiltrationAbridged(Guid.NewGuid().ToString(), 0, "");
            var isInfNull = _vm.hbObj.Infiltration == null;
            var inf = new List<Control>() { };
            inf.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.INF_DisplayName));
            inf.AddRange(GenInputControl("Flow/FacadeArea", (ProgramTypeViewModel m) => m.INF_FlowPerExteriorArea));
            inf.AddRange(GenDropInInputControl("Schedule", (ProgramTypeViewModel m) => m.INF_Schedule, (ProgramTypeViewModel m) => m.INF_ScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            inf.AddRange(GenInputControl("Velocity Coefficient", (ProgramTypeViewModel m) => m.INF_VelocityCoefficient));
            inf.AddRange(GenInputControl("Temperature Coefficient", (ProgramTypeViewModel m) => m.INF_TemperatureCoefficient));
            inf.AddRange(GenInputControl("Constant Coefficient", (ProgramTypeViewModel m) => m.INF_ConstantCoefficient));
            return GenGroup("Infiltration", inf, isInfNull, (v) => _vm.Infiltration = v as HB.InfiltrationAbridged, typeof(HB.InfiltrationAbridged));
        }

        private Panel GenGasEquipPanel()
        {
            //_vm.hbObj.GasEquipment = _vm.hbObj.GasEquipment ?? new GasEquipmentAbridged(Guid.NewGuid().ToString(), 0, "");
            var isGasNull = _vm.hbObj.GasEquipment == null;
            var gas = new List<Control>() { };
            gas.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.GAS_DisplayName));
            gas.AddRange(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.GAS_WattsPerArea));
            gas.AddRange(GenDropInInputControl("Schedule", (ProgramTypeViewModel m) => m.GAS_Schedule, (ProgramTypeViewModel m) => m.GAS_ScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            gas.AddRange(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.GAS_RadiantFraction));
            gas.AddRange(GenInputControl("Latent Fraction", (ProgramTypeViewModel m) => m.GAS_LatentFraction));
            gas.AddRange(GenInputControl("Lost Fraction", (ProgramTypeViewModel m) => m.GAS_LostFraction));
            return GenGroup("Gas Equipment", gas, isGasNull, (v) => _vm.GasEquipment = v as HB.GasEquipmentAbridged, typeof(HB.GasEquipmentAbridged));
        }

        private Panel GenEquipPanel()
        {
            //_vm.hbObj.ElectricEquipment = _vm.hbObj.ElectricEquipment ?? new ElectricEquipmentAbridged(Guid.NewGuid().ToString(), 0, "");
            var isEqpNull = _vm.hbObj.ElectricEquipment == null;
            var eqp = new List<Control>() { };
            eqp.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.EQP_DisplayName));
            eqp.AddRange(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.EQP_WattsPerArea));
            eqp.AddRange(GenDropInInputControl("Schedule", (ProgramTypeViewModel m) => m.EQP_Schedule, (ProgramTypeViewModel m) => m.EQP_ScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            eqp.AddRange(GenInputControl("Radiant Schedule", (ProgramTypeViewModel m) => m.EQP_RadiantFraction));
            eqp.AddRange(GenInputControl("Latent Fraction", (ProgramTypeViewModel m) => m.EQP_LatentFraction));
            eqp.AddRange(GenInputControl("Lost Fraction", (ProgramTypeViewModel m) => m.EQP_LostFraction));
            return GenGroup("Electric Equipment", eqp, isEqpNull, (v) => _vm.ElectricEquipment = v as HB.ElectricEquipmentAbridged, typeof(HB.ElectricEquipmentAbridged));
        }

        private Panel GenLightingPanel()
        {
            //_vm.hbObj.Lighting = _vm.hbObj.Lighting ?? new LightingAbridged(Guid.NewGuid().ToString(), 0, "");
            var isLpdNull = _vm.hbObj.Lighting == null;
            var lpd = new List<Control>() { };
            lpd.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.LPD_DisplayName));
            lpd.AddRange(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.LPD_WattsPerArea));
            lpd.AddRange(GenDropInInputControl("Schedule", (ProgramTypeViewModel m) => m.LPD_Schedule, (ProgramTypeViewModel m) => m.LPD_ScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            lpd.AddRange(GenInputControl("Visible Fraction", (ProgramTypeViewModel m) => m.LPD_VisibleFraction));
            lpd.AddRange(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.LPD_RadiantFraction));
            lpd.AddRange(GenInputControl("Return Air Fraction", (ProgramTypeViewModel m) => m.LPD_ReturnAirFraction));
            return GenGroup("Lighting", lpd, isLpdNull, (v) => _vm.Lighting = v as HB.LightingAbridged, typeof(HB.LightingAbridged));
        }

        private Panel GenPeoplePanel()
        {
        
            //Get people
            var isPplNull = _vm.hbObj.People == null;
            //_vm.hbObj.People = _vm.hbObj.People ?? new PeopleAbridged(Guid.NewGuid().ToString(), 0, "", "");
            //_vm.hbObj.People.LatentFraction = new Autocalculate();
            var ppl = new List<Control>() { };
            ppl.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.PPL_DisplayName));
            ppl.AddRange(GenInputControl("People/Area", (ProgramTypeViewModel m) => m.PPL_PeoplePerArea));
            ppl.AddRange(GenDropInInputControl("Occupancy Schedule", (ProgramTypeViewModel m) => m.PPL_OccupancySchedule, (ProgramTypeViewModel m) => m.PPL_OccupancyScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            ppl.AddRange(GenDropInInputControl("Activity Schedule", (ProgramTypeViewModel m) => m.PPL_ActivitySchedule, (ProgramTypeViewModel m) => m.PPL_ActivityScheduleName, typeof(HB.ScheduleRulesetAbridged)));
            ppl.AddRange(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.PPL_RadiantFraction));
            ppl.AddRange(GenInputControl("Latent Fraction", typeof(HB.Autocalculate), (ProgramTypeViewModel m) => m.PPL_IsLatentFractionAutocalculate, (ProgramTypeViewModel m) => m.PPL_LatentFraction));
            return GenGroup("People", ppl, isPplNull, (v) => _vm.People = v as HB.PeopleAbridged, typeof(HB.PeopleAbridged));
        }

        private static (List<Control> allControls, GridView libraryLBox) GenLibraryPanel()
        {
            //IEnumerable<HB.Energy.IIDdEnergyBaseModel> hbLoads = Loads;

            IEnumerable<HB.Energy.IIDdEnergyBaseModel> itemsToShow = Loads;

            // Library List
            var library_GV = new GridView();
            library_GV.Height = 450;
            library_GV.Width = 380;
            //library_GV.GridLines = GridLines.Horizontal;
            library_GV.ShowHeader = false;
            library_GV.AllowMultipleSelection = false;

            // Preview 
            var preview_GV = new GridView() {};
            preview_GV.Height = 180;
            preview_GV.ShowHeader = false;
            //preview_GV.GridLines = GridLines.Horizontal;
            var nameCol = new GridColumn() { DataCell = new TextBoxCell(0) };
            var valueCol = new GridColumn() { DataCell = new TextBoxCell(1) };
            preview_GV.Columns.Add(nameCol);
            preview_GV.Columns.Add(valueCol);

            // Library items
            //var allItems = itemsToShow.Select(_ => new ListItem() { Text = $"{_.GetType().Name.Replace("Abridged", "")}: {_.DisplayName??_.Identifier}" , Tag = _ });
            library_GV.DataStore = itemsToShow;

            var typeCell = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IIDdEnergyBaseModel, string>( r => $"{r.GetType().Name.Replace("Abridged", "").Replace("Equipment", "Equip")}")
            };
            var idCell = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IIDdEnergyBaseModel, string>( r => r.DisplayName?? r.Identifier)
            };
            
            library_GV.Columns.Add(new GridColumn() { DataCell = typeCell});
            library_GV.Columns.Add(new GridColumn() { DataCell = idCell});


            library_GV.SelectedItemsChanged += (s, e) =>
            {
                //Clear preview first
                preview_GV.DataStore = new List<string>();

                //Check current selected item from library
                var selItem = library_GV.SelectedItem;
                if (selItem == null)
                    return;

                //Update Preview
                preview_GV.DataStore = GetPreviewData(selItem);

            };

            library_GV.MouseMove += (sender, e) =>
            {
                if (e.Buttons != MouseButtons.Primary)
                    return;

                var lib = library_GV;
                var dragableArea = lib.Bounds;
                dragableArea.Width -= 20;
                dragableArea.Height -= 20;
                var iscontained = e.Location.Y < dragableArea.Height && e.Location.X < dragableArea.Width;
                //name.Text = $"{dragableArea.Width}x{dragableArea.Height}, {new Point(e.Location).X}:{new Point(e.Location).Y}, {dragableArea.Contains(new Point(e.Location))}";
                if (!iscontained)
                    return;

                var cell = library_GV.GetCellAt(e.Location);
                if (cell.RowIndex == -1 || cell.ColumnIndex == -1)
                    return;

                var selected = (lib.SelectedItem as HB.HoneybeeObject).ToJson();
                var data = new DataObject();
                data.SetString(selected, "HBObj");
                lib.DoDragDrop(data, DragEffects.Move);
                e.Handled = true;

            };
        
            // Search box
            var searchTBox = new TextBox() { PlaceholderText = "Search" };
            searchTBox.TextChanged += (sender, e) =>
            {
                var input = searchTBox.Text;
                //library_GV.Items.Clear();
                if (string.IsNullOrWhiteSpace(input))
                {
                    library_GV.DataStore = itemsToShow;
                    return;
                }
                var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                var filtered = itemsToShow.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null ? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
                //var filteredItems = filtered.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                library_GV.DataStore = filtered;

            };

            // Library type
            var libraryType_DD = new DropDown();
            libraryType_DD.Items.Add(new ListItem() { Key = "Load", Text = "Internal Load" });
            libraryType_DD.Items.Add(new ListItem() { Key = "Schedule", Text = "Schedule" });
            libraryType_DD.SelectedIndex = 0;
            libraryType_DD.SelectedIndexChanged += (sender, e) =>
            {
                var selectedType = libraryType_DD.SelectedKey;
                if (selectedType == "Schedule")
                {
                    //itemsToShow.Clear();
                    itemsToShow = Schedules;
                }
                else
                {
                    itemsToShow = Loads;
                }

                searchTBox.Text = null;
                //library_GV.Items.Clear();

                //var filteredItems = itemsToShow.Select(_ => new ListItem() { Text = $"{_.GetType().Name.Replace("Abridged","")}: {_.DisplayName ?? _.Identifier}", Tag = _ });
                library_GV.DataStore = itemsToShow;

            };

           
            var controls = new List<Control>() {
                libraryType_DD,
                searchTBox,
                library_GV,
                preview_GV
            };
          
            return (controls, library_GV);

            
        }
        private static List<object> GetPreviewData(object selectedItem )
        {
            var layers = new List<object>();
            if (selectedItem is HB.HoneybeeObject obj)
            {
                var detailedString = obj.ToString(detailed: true);
                var str = detailedString.Split(new[] { "\n" }, StringSplitOptions.None).Skip(1);
                foreach (var item in str)
                {
                    var values = item.Trim().Split(new[] { ':' }, 2).ToList();
                    if (values.Count==2)
                    {
                        layers.Add(values);
                    }
                    
                }
            }
           
            return layers;
          
        }
   
        private readonly int _inputControlWidth = 300;
        List<Control> GenInputControl<TObject>(string inputName, Expression<Func<TObject, int>> propertyExpression) 
            where TObject : ViewModelBase
        {
            var inputLabel = new Label() { Text = inputName, Width = 150 };
            // user input integer
            var num_NS = new MaskedTextBox<int>();
            num_NS.ShowPlaceholderWhenEmpty = true;
            num_NS.PlaceholderText = "0";
            num_NS.Provider = new NumericMaskedTextProvider<int>() { AllowDecimal = false, AllowSign = true };

            num_NS.Width = _inputControlWidth;
            num_NS.Height = 25;
            num_NS.ValueBinding.BindDataContext(propertyExpression);

            var rows = new List<Control>()
            {
                inputLabel,
                num_NS
            };
            return rows;
        }
        List<Control> GenInputControl<TObject>(string inputName, Expression<Func<TObject, double>> propertyExpression) 
            where TObject: ViewModelBase
        {
            //var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 150 };
            // user input number
            var num_NS = new MaskedTextBox<double>();
            num_NS.ShowPlaceholderWhenEmpty = true;
            num_NS.PlaceholderText = "0";
            num_NS.Provider = new NumericMaskedTextProvider<double>() { AllowDecimal = true, AllowSign = true };
            num_NS.Width = _inputControlWidth;
            num_NS.Height = 25;
            num_NS.ValueBinding.BindDataContext(propertyExpression);

            var rows = new List<Control>()
            {
                inputLabel,
                num_NS
            };
            return rows;
        }
        List<Control> GenInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyExpression) 
            where TObject : ViewModelBase
        {
            var inputLabel = new Label() { Text = inputName, Width = 150 };
            // user input text
            var text_TB = new TextBox();
            text_TB.Width = _inputControlWidth;
            text_TB.Height = 25;
            text_TB.TextBinding.BindDataContext(propertyExpression);

            var rows = new List<Control>()
            {
                inputLabel,
                text_TB
            };
            return rows;
        }

        List<Control> GenInputControl<TObject>(string inputName, Type altNumType, Expression<Func<TObject, bool?>> ifIAltnumberExpression, Expression<Func<TObject, double>> propertyExpression)
            where TObject : ViewModelBase
        {
            //var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 150 };
            //var anyOfType = typeof(TValue);

            // number input
            var numInput = new NumericMaskedTextBox<double>();
            numInput.ValueBinding.BindDataContext(propertyExpression);

            // check box for IAltnumber (autosize, autocalculate, etc)
            var altNum_CBox = new CheckBox() { Text = altNumType.Name };
            altNum_CBox.CheckedBinding.BindDataContext(ifIAltnumberExpression);
            altNum_CBox.CheckedChanged += (s, e) => {
                numInput.Enabled = !altNum_CBox.Checked.Value;
                if (numInput.Enabled)
                {
                    var preValue = numInput.Value;
                    numInput.Value = preValue + 0.01;
                    numInput.Value = preValue;
                    //vm.LatentFraction = numInput.Value;
                }

            };

            var rows = new List<Control>()
            {
                inputLabel,
                altNum_CBox,
                numInput
            };
            return rows;

        }
        //List<Control> GenDropInInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyExpression, Type type)
        //     where TObject : ViewModelBase
        //{
        //    var inputLabel = new Label() { Text = inputName, Width = 150 };

        //    var layerPanel = new PixelLayout();
        //    var dropInValueIdentifier = new TextBox() { PlaceholderText = "Drag from library" };
        //    dropInValueIdentifier.Width = 312;
        //    dropInValueIdentifier.Enabled = false;
        //    //dropInValue.TextBinding.Bind(propertyExpression)
        //    dropInValueIdentifier.TextBinding.BindDataContext(propertyExpression);

        //    // for display name
        //    var dropInValueName = new TextBox() { };
        //    dropInValueName.Width = 312;
        //    dropInValueName.Enabled = false;
        //    //dropInValue.TextBinding.Bind(propertyExpression)
        //    dropInValueName.TextBinding.BindDataContext(propertyExpression);



        //    var dropIn = new Drawable();
        //    dropIn.AllowDrop = true;
        //    dropIn.Width = dropInValueIdentifier.Width;
        //    dropIn.Height = 25;
        //    dropIn.BackgroundColor = Colors.Transparent;

        //    var deleteBtn = new Button();
        //    deleteBtn.Text = "✕";
        //    deleteBtn.Width = 24;
        //    deleteBtn.Height = 24;

        //    deleteBtn.Click += (s, e) =>
        //    {
        //        dropInValueIdentifier.Text = null;
        //        deleteBtn.Visible = false;
        //        //setAction(null);
        //    };

        //    dropInValueIdentifier.TextChanged += (s, e) =>
        //    {
        //        deleteBtn.Visible = !string.IsNullOrEmpty(dropInValueIdentifier.Text);
        //    };

        //    dropIn.DragLeave += (sender, e) =>
        //    {
        //        dropInValueIdentifier.BackgroundColor = Colors.White;
        //    };
        //    dropIn.DragOver += (sender, e) =>
        //    {
        //        e.Effects = DragEffects.Move;
        //        dropInValueIdentifier.BackgroundColor = Colors.Yellow;
        //    };
        //    dropIn.DragDrop += (sender, e) =>
        //    {
        //        // Get drop-in object
        //        var value = e.Data.GetObject("HBObj");
        //        var newValue = type.GetMethod("FromJson").Invoke(null, new object[] { value }) as HB.IIDdBase;

        //        if (newValue == null)
        //        {
        //            MessageBox.Show(this, $"{type.Name.Replace("Abridged", "")} is required!");
        //            return;
        //        }


        //        deleteBtn.Visible = true;
        //        //dropInValue.Text = newValue.DisplayName ?? newValue.Identifier;
        //        dropInValueIdentifier.Text = newValue.Identifier;
        //        //dropInValueName.Text = newValue.DisplayName;
        //        //setAction(newValue.Identifier);

        //    };


        //    layerPanel.Add(dropInValueIdentifier, 0, 0);
        //    layerPanel.Add(dropInValueName, 20, 0);
        //    layerPanel.Add(dropIn, 0, 0);
        //    layerPanel.Add(deleteBtn, dropInValueIdentifier.Width - 24, 0);
        //    return new List<Control>() { inputLabel, layerPanel };


        //}
        List<Control> GenDropInInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyIdentifierExpression, Expression<Func<TObject, string>> propertyNameExpression, Type type)
            where TObject : ViewModelBase
        {
            var w = 317;
            var h = 25;
            var inputLabel = new Label() { Text = inputName, Width = 150 };

            // for tracking ID
            var layerPanel = new PixelLayout();
            var dropInValueIdentifier = new TextBox() {};
            dropInValueIdentifier.Width = w;
            dropInValueIdentifier.Height = h;
            dropInValueIdentifier.Enabled = false;
            dropInValueIdentifier.Visible = false;
            dropInValueIdentifier.TextBinding.BindDataContext(propertyIdentifierExpression);

            // for display name
            var dropInValueName = new TextBox() { PlaceholderText = "Drag from library" };
            dropInValueName.Width = w;
            dropInValueName.Height = h;
            dropInValueName.Enabled = false;
            dropInValueIdentifier.BackgroundColor = Colors.Transparent;
            dropInValueName.TextBinding.BindDataContext(propertyNameExpression);



            var dropIn = new Drawable();
            dropIn.AllowDrop = true;
            dropIn.Width = w;
            dropIn.Height = h;
            dropIn.BackgroundColor = Colors.Transparent;

            var deleteBtn = new Button();
            deleteBtn.Text = "✕";
            deleteBtn.Width = 24;
            deleteBtn.Height = 24;

            deleteBtn.Click += (s, e) =>
            {
                dropInValueIdentifier.Text = null;
                deleteBtn.Visible = false;
                //setAction(null);
            };

            dropInValueIdentifier.TextChanged += (s, e) =>
            {
                deleteBtn.Visible = !string.IsNullOrEmpty(dropInValueIdentifier.Text);
            };

            dropIn.DragLeave += (sender, e) =>
            {
                dropInValueName.BackgroundColor = Colors.Transparent;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                dropInValueName.BackgroundColor = Colors.LightGrey;
            };
            dropIn.DragDrop += (sender, e) =>
            {
                // Get drop-in object
                var value = e.Data.GetString("HBObj");
                var newValue = type.GetMethod("FromJson").Invoke(null, new object[] { value }) as HB.IIDdBase;

                if (newValue == null)
                {
                    MessageBox.Show(this, $"{type.Name.Replace("Abridged", "")} is required!");
                    return;
                }


                deleteBtn.Visible = true;
                //dropInValue.Text = newValue.DisplayName ?? newValue.Identifier;
                dropInValueIdentifier.Text = newValue.Identifier;
                //dropInValueName.Text = newValue.DisplayName;
                //setAction(newValue.Identifier);

            };


            layerPanel.Add(dropInValueIdentifier, 0, 0);
            layerPanel.Add(dropInValueName, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            layerPanel.Add(deleteBtn, dropInValueIdentifier.Width - 24, 0);
            return new List<Control>() { inputLabel, layerPanel };


        }

        //List<Control> GenInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyExpression, Func<object> getSelectedFromLib, Type type) where TObject : ViewModelBase
        //{
        //    var panel = new DynamicLayout();
        //    var inputLabel = new Label() { Text = inputName, Width = 150};
        //    // Select from library
        //    var tbx = new TextBox() { PlaceholderText = "Add one from library" };
        //    tbx.Width = _inputControlWidth - 25;
        //    tbx.Enabled = false;
        //    tbx.TextBinding.BindDataContext(propertyExpression);

        //    // Button for selecting item from library
        //    var inWallBtn = new Button() { Width = 25 };
        //    UpdateAddBtn(tbx.Text == null);

        //    inWallBtn.Click += (sender, e) =>
        //    {
        //        var txt = inWallBtn.Text;
        //        string newText = null;
        //        if (txt == "＋")
        //        {
        //            var selectedItem = getSelectedFromLib() as IIDdBase;
        //            if (selectedItem == null)
        //                return;

        //            if (!type.IsAssignableFrom(selectedItem.GetType()))
        //            {
        //                MessageBox.Show(this, $"Selected {selectedItem.GetType().Name.Replace("Abridged", "")} cannot be set to {type.Name.Replace("Abridged", "")}!");
        //                return;
        //            }
        //            //TODO: this will be a problem when schedule has set a display name.
        //            newText = selectedItem.DisplayName ?? selectedItem.Identifier;

        //        }

        //        tbx.Text = newText;
        //        UpdateAddBtn(newText == null);
        //    };

        //    var rows = new List<Control>()
        //    {
        //        inputLabel,tbx,inWallBtn
        //    };
        //    return rows;

        //    void UpdateAddBtn(bool isAdd)
        //    {
        //        if (isAdd)
        //        {
        //            inWallBtn.Text = "＋";
        //            inWallBtn.ToolTip = "Add from library";
        //        }
        //        else
        //        {
        //            inWallBtn.Text = "−";
        //            inWallBtn.ToolTip = "Remove";
        //        }
        //    }
        //}

        //public delegate void SetLoadAction(ILoad load);
        private DynamicLayout GenGroup(string groupName, List<Control> inputCtrlRows, bool isNull, Action<ILoad> setAction, Type type)
        {

            //var gp = new GroupBox() { Text = groupName};
            var separater = new DynamicLayout() { Height = 1, BackgroundColor = Color.FromArgb(150, 150, 150) };
            var titleLabel = new Label() { Text = groupName }; 
            var f = titleLabel.Font;
            titleLabel.Font = new Font(f.Family, f.Size, FontStyle.Bold);
     

            var layout = new DynamicLayout() { DefaultPadding = new Padding(0, 0, 0, 15) };

            var layoutLoads = new DynamicLayout() { DefaultPadding = new Padding(0,3) };
            layoutLoads.Visible = !isNull;
            var layerDropIn = new PixelLayout();
            layerDropIn.Visible = isNull;


            foreach (var row in inputCtrlRows)
            {
                layoutLoads.AddSeparateRow(row);
            }
            //remove button
            var rm = new Button() { Text = $"Remove {groupName}" };
            rm.Click += (s, e) =>
            {
                layoutLoads.Visible = false;
                layerDropIn.Visible = true;
                setAction(null);
            };
            layoutLoads.AddSeparateRow(rm);



            
            var dropInValue = new Label();
            dropInValue.Text = "Drag from library";
            //dropInValue.BackgroundColor = Colors.Transparent;
            dropInValue.Enabled = false;
            dropInValue.Width = _inputControlWidth;
            //dropInValue.Height = 300;
            dropInValue.TextAlignment = TextAlignment.Center;
            dropInValue.VerticalAlignment = VerticalAlignment.Center;


            // Drop in area for add a new Load
            var dropIn = new Drawable();
            dropIn.AllowDrop = true;
            dropIn.Width = dropInValue.Width;
            dropIn.Height = 35;
            dropIn.BackgroundColor = Colors.Transparent;
            dropIn.DragLeave += (sender, e) =>
            {
                layout.BackgroundColor = Colors.Transparent;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                layout.BackgroundColor = Colors.LightGrey;
            };
            dropIn.DragDrop += (sender, e) =>
            {
                // Get drop-in object
                var value = e.Data.GetString("HBObj");
                var newValue = type.GetMethod("FromJson").Invoke(null, new object[] { value }) as HB.Energy.ILoad;


                if (newValue == null)
                {
                    MessageBox.Show(this, $"{type.Name.Replace("Abridged", "")} is required!");
                    return;
                }


                layoutLoads.Visible = true;
                layerDropIn.Visible = false;
                setAction(newValue);

            };

          
            layerDropIn.Add(dropInValue, 0, 5);
            layerDropIn.Add(dropIn, 0, 0);

            layout.AddRow(separater);
            layout.AddRow(titleLabel);
            layout.AddRow(layoutLoads);
            layout.AddRow(layerDropIn);


            
            return layout;
        }
       
        //private GroupBox GenPanel(string groupName, Func<HB.Energy.IConstruction> getSelected, IEnumerable<(string label, Expression<Func<ViewModelBase, object>> propertyExpression, Type setType)> setActions)
        //{

        //    //Wall Construction Set
        //    var wallGroup = new GroupBox() { Text = groupName };

        //    var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };
        //    foreach (var item in setActions)
        //    {
        //        var inWall = GenTextBoxWithBtn(getSelected, item.propertyExpression, item.setType);
        //        wallLayout.AddRow(new Label() { Text = item.label, Width = 125 }, inWall, null);
        //    }

        //    //var exWall = GenTextBoxWithBtn(getSelected, (cons) => c.ExteriorConstruction = cons.Identifier);
        //    //var gWall =  GenTextBoxWithBtn(getSelected, (cons) => c.GroundConstruction = cons.Identifier);

        //    //wallLayout.AddRow(new Label() { Text = "Exterior", Width = 75 }, exWall, null);
        //    //wallLayout.AddRow(new Label() { Text = "Ground", Width = 75 }, gWall, null);
        //    wallGroup.Content = wallLayout;
        //    return wallGroup;

        //}


        //private GroupBox GenPanelFloorSet()
        //{
        //    //Wall Construction Set
        //    var wallGroup = new GroupBox() { Text = "Floor Construction Set" };
        //    var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };
        //    var inWall = new TextBox() { PlaceholderText = "By Global" };
        //    inWall.Width = 300;
        //    var inWallBtn = new Button() { Text = "+", Width = 30 };
        //    inWallBtn.Click += (sender, e) =>
        //    {
        //        var txt = inWallBtn.Text;
        //        inWall.Text = txt == "+" ? "A New Construction" : null;
        //        inWallBtn.Text = txt == "+" ? "-" : "+";
        //    };

        //    var exWall = new TextBox() { PlaceholderText = "By Global" };
        //    var exWallBtn = new Button() { Text = "+", Width = 30 };
        //    exWallBtn.Click += (sender, e) =>
        //    {
        //        var txt = exWallBtn.Text;
        //        exWall.Text = txt == "+" ? "A New Construction" : null;
        //        exWallBtn.Text = txt == "+" ? "-" : "+";
        //    };

        //    var gWall = new TextBox() { PlaceholderText = "By Global" };
        //    var gWallBtn = new Button() { Text = "+", Width = 30 };
        //    gWallBtn.Click += (sender, e) =>
        //    {
        //        var txt = gWallBtn.Text;
        //        gWall.Text = txt == "+" ? "A New Construction" : null;
        //        gWallBtn.Text = txt == "+" ? "-" : "+";
        //    };

        //    wallLayout.AddRow(new Label() { Text = "Interior", Width = 50 }, inWall, inWallBtn, null);
        //    wallLayout.AddRow(new Label() { Text = "Exterior", Width = 50 }, exWall, exWallBtn, null);
        //    wallLayout.AddRow(new Label() { Text = "Ground", Width = 50 }, gWall, gWallBtn, null);
        //    wallGroup.Content = wallLayout;
        //    return wallGroup;

        //}

        //private GroupBox GenPanelAirBoundary(string groupName, ListBox tbox)
        //{
        //    //Wall Construction Set
        //    var wallGroup = new GroupBox() { Text = groupName };
        //    var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };
        //    var texBox = GenTextBoxWithBtn(tbox);

        //    wallLayout.AddRow(new Label() { Text = "AirBoundary", Width = 75 }, texBox, null);
        //    wallGroup.Content = wallLayout;
        //    return wallGroup;

        //}



    }
}
