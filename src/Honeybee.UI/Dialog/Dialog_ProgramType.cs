using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using HoneybeeSchema;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using HoneybeeSchema.Energy;

namespace Honeybee.UI
{

    public class Dialog_ProgramType: Dialog<HB.ProgramTypeAbridged>
    {
        private static ProgramTypeViewModel _vm;
        public Dialog_ProgramType(HB.ProgramTypeAbridged ProgramType)
        {
            try
            {
                _vm = ProgramTypeViewModel.Instance;
                _vm.hbObj = ProgramType ?? new HB.ProgramTypeAbridged(identifier: Guid.NewGuid().ToString());

                Padding = new Padding(5);
                Resizable = true;
                Title = "Construction Set - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 500);
                //Height = 800;
                this.Icon = DialogHelper.HoneybeeIcon;
                

                Func<HB.Energy.ISchedule> getSelectedSche = () =>
                {
                    return null;
                    //if (libraryLBox.SelectedIndex == -1)
                    //    return null;
                    //return (libraryLBox.Items[libraryLBox.SelectedIndex] as ListItem).Tag as HB.Energy.ISchedule;
                };

                var defaultByProgramType = "By Room Program Type";
                //Get people
                var isPplNull = _vm.hbObj.People == null;
                //_vm.hbObj.People = _vm.hbObj.People ?? new PeopleAbridged(Guid.NewGuid().ToString(), 0, "", "");
                //_vm.hbObj.People.LatentFraction = new Autocalculate();
                var ppl = new List<List<Control>>() { };
                ppl.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.PPL_DisplayName));
                ppl.AddRange(GenInputControl("People/Area", (ProgramTypeViewModel m) => m.PPL_PeoplePerArea));
                ppl.AddRange(GenInputControl("Occupancy Schedule", (ProgramTypeViewModel m) => m.PPL_OccupancySchedule, getSelectedSche));
                ppl.AddRange(GenInputControl("Activity Schedule", (ProgramTypeViewModel m) => m.PPL_ActivitySchedule, getSelectedSche));
                ppl.AddRange(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.PPL_RadiantFraction));
                ppl.AddRange(GenInputControl("Latent Fraction", typeof(Autocalculate), (ProgramTypeViewModel m) => m.PPL_IsLatentFractionAutocalculate, (ProgramTypeViewModel m) => m.PPL_LatentFraction));
                var peopleGroup = GenGroup("People", ppl, isPplNull, () => _vm.People = null);

                //_vm.hbObj.Lighting = _vm.hbObj.Lighting ?? new LightingAbridged(Guid.NewGuid().ToString(), 0, "");
                var isLpdNull = _vm.hbObj.Lighting == null;
                var lpd = new List<List<Control>>() { };
                lpd.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.LPD_DisplayName));
                lpd.AddRange(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.LPD_WattsPerArea));
                lpd.AddRange(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.LPD_Schedule, getSelectedSche));
                lpd.AddRange(GenInputControl("Visible Schedule", (ProgramTypeViewModel m) => m.LPD_VisibleFraction));
                lpd.AddRange(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.LPD_RadiantFraction));
                lpd.AddRange(GenInputControl("Return Air Fraction", (ProgramTypeViewModel m) => m.LPD_ReturnAirFraction));
                var lightingGroup = GenGroup("Lighting", lpd, isLpdNull, () => _vm.Lighting = null);

                //_vm.hbObj.ElectricEquipment = _vm.hbObj.ElectricEquipment ?? new ElectricEquipmentAbridged(Guid.NewGuid().ToString(), 0, "");
                var isEqpNull = _vm.hbObj.ElectricEquipment == null;
                var eqp = new List<List<Control>>() { };
                eqp.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.EQP_DisplayName));
                eqp.AddRange(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.EQP_WattsPerArea));
                eqp.AddRange(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.EQP_Schedule, getSelectedSche));
                eqp.AddRange(GenInputControl("Radiant Schedule", (ProgramTypeViewModel m) => m.EQP_RadiantFraction));
                eqp.AddRange(GenInputControl("Latent Fraction", (ProgramTypeViewModel m) => m.EQP_LatentFraction));
                eqp.AddRange(GenInputControl("Lost Fraction", (ProgramTypeViewModel m) => m.EQP_LostFraction));
                var equipmentGroup = GenGroup("Electric Equipment", eqp, isEqpNull, () => _vm.ElectricEquipment = null);

                //_vm.hbObj.GasEquipment = _vm.hbObj.GasEquipment ?? new GasEquipmentAbridged(Guid.NewGuid().ToString(), 0, "");
                var isGasNull = _vm.hbObj.GasEquipment == null;
                var gas = new List<List<Control>>() { };
                gas.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.GAS_DisplayName));
                gas.AddRange(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.GAS_WattsPerArea));
                gas.AddRange(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.GAS_Schedule, getSelectedSche));
                gas.AddRange(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.GAS_RadiantFraction));
                gas.AddRange(GenInputControl("Latent Fraction", (ProgramTypeViewModel m) => m.GAS_LatentFraction));
                gas.AddRange(GenInputControl("Lost Fraction", (ProgramTypeViewModel m) => m.GAS_LostFraction));
                var gasEqpGroup = GenGroup("Gas Equipment", gas, isGasNull, () => _vm.GasEquipment = null);

                //_vm.hbObj.Infiltration = _vm.hbObj.Infiltration ?? new InfiltrationAbridged(Guid.NewGuid().ToString(), 0, "");
                var isInfNull = _vm.hbObj.Infiltration == null;
                var inf = new List<List<Control>>() { };
                inf.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.INF_DisplayName));
                inf.AddRange(GenInputControl("Flow/FacadeArea", (ProgramTypeViewModel m) => m.INF_FlowPerExteriorArea));
                inf.AddRange(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.INF_Schedule, getSelectedSche));
                inf.AddRange(GenInputControl("Velocity Coefficient", (ProgramTypeViewModel m) => m.INF_VelocityCoefficient));
                inf.AddRange(GenInputControl("Temperature Coefficient", (ProgramTypeViewModel m) => m.INF_TemperatureCoefficient));
                inf.AddRange(GenInputControl("Constant Coefficient", (ProgramTypeViewModel m) => m.INF_ConstantCoefficient));
                var infiltrationGroup = GenGroup("Infiltration", inf, isInfNull, () => _vm.Infiltration = null);

                //_vm.hbObj.Ventilation = _vm.hbObj.Ventilation ?? new VentilationAbridged(Guid.NewGuid().ToString());
                var isVntNull = _vm.hbObj.Ventilation == null;
                var vnt = new List<List<Control>>() { };
                vnt.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.VNT_DisplayName));
                vnt.AddRange(GenInputControl("Flow/Area", (ProgramTypeViewModel m) => m.VNT_FlowPerArea));
                vnt.AddRange(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.VNT_Schedule, getSelectedSche));
                vnt.AddRange(GenInputControl("Flow/Person", (ProgramTypeViewModel m) => m.VNT_FlowPerPerson));
                vnt.AddRange(GenInputControl("Flow/Zone", (ProgramTypeViewModel m) => m.VNT_FlowPerZone));
                vnt.AddRange(GenInputControl("AirChanges/Hour", (ProgramTypeViewModel m) => m.VNT_AirChangesPerHour));
                var ventilationGroup = GenGroup("Ventilation", vnt, isVntNull, () =>_vm.Ventilation = null);

                //_vm.hbObj.Setpoint = _vm.hbObj.Setpoint ?? new SetpointAbridged(Guid.NewGuid().ToString(), "cooling sch", "heating sch");
                var isSptNull = _vm.hbObj.Setpoint == null;
                var spt = new List<List<Control>>() { };
                spt.AddRange(GenInputControl("Name", (ProgramTypeViewModel m) => m.SPT_DisplayName));
                spt.AddRange(GenInputControl("Cooling Schedule", (ProgramTypeViewModel m) => m.SPT_CoolingSchedule, getSelectedSche));
                spt.AddRange(GenInputControl("Heating Schedule", (ProgramTypeViewModel m) => m.SPT_HeatingSchedule, getSelectedSche));
                spt.AddRange(GenInputControl("Humidifying Schedule", (ProgramTypeViewModel m) => m.SPT_HumidifyingSchedule, getSelectedSche));
                spt.AddRange(GenInputControl("Dehumidifying Schedule", (ProgramTypeViewModel m) => m.SPT_DehumidifyingSchedule, getSelectedSche));
                var setpoinGroup = GenGroup("Setpoint", spt, isSptNull, () => _vm.Setpoint = null);

                // Json Data
                var hbData = new Button { Text = "HBData" };
                hbData.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, _vm.hbObj.ToJson(), "Honeybee Data");

                //Left panel
                //var panelLeft = new TableLayout() { DataContext = _vm };

                var panelLeft = new DynamicLayout() { DataContext = _vm };
                //panelLeft.Height = 600;
                panelLeft.Padding = new Padding(5);
                panelLeft.Spacing = new Size(5, 5);
          
                var nameTbx = new TextBox();
                _vm.hbObj.DisplayName = _vm.hbObj.DisplayName ?? $"New ProgramType {_vm.hbObj.Identifier.Substring(0, 5)}";
                nameTbx.TextBinding.Bind(_vm.hbObj, c => c.DisplayName);
                //panelLeft.BeginScrollable(BorderType.None);
                //panelLeft.Height = 600;
                //panelLeft.Width = 400;
                panelLeft.AddSeparateRow(new Label() { Text = "ID: " }, new Label() { Text = _vm.hbObj.Identifier, Enabled = false });
                panelLeft.AddSeparateRow(new Label() { Text = "Name:" });
                panelLeft.AddSeparateRow(nameTbx);

                var loadsPanel = new DynamicLayout();
                loadsPanel.Padding = new Padding(0, 0, 0, 15);
                //loadsPanel.Spacing = new Size(5, 5);
                loadsPanel.Height = 600;
                loadsPanel.Width = 340;
                //panelLeft.BeginVertical();
                loadsPanel.BeginScrollable(BorderType.None);
                loadsPanel.AddSeparateRow(peopleGroup);
                loadsPanel.AddSeparateRow(lightingGroup);
                loadsPanel.AddSeparateRow(equipmentGroup);
                loadsPanel.AddSeparateRow(gasEqpGroup);
                loadsPanel.AddSeparateRow(infiltrationGroup);
                loadsPanel.AddSeparateRow(ventilationGroup);
                loadsPanel.AddSeparateRow(setpoinGroup);
                loadsPanel.AddSeparateRow(null);
                loadsPanel.EndScrollable();
                //panelLeft.EndVertical();


                panelLeft.AddSeparateRow(loadsPanel);
                panelLeft.AddSeparateRow(hbData);
                panelLeft.AddSeparateRow(null);
                //panelLeft.EndScrollable();

                //panelLeft.Rows.Add(new Label() { Text = $"ID: {_vm.hbObj.Identifier}", Enabled = false });
                //panelLeft.Rows.Add(new Label() { Text = "Name:" });
                //panelLeft.Rows.Add(nameTbx);
                //panelLeft.Rows.Add(peopleGroup);
                //panelLeft.Rows.Add(lightingGroup);
                //panelLeft.Rows.Add(equipmentGroup);
                //panelLeft.Rows.Add(gasEqpGroup);
                //panelLeft.Rows.Add(infiltrationGroup);
                //panelLeft.Rows.Add(ventilationGroup);
                //panelLeft.Rows.Add(setpoinGroup);
                //panelLeft.Rows.Add(hbData);
                //panelLeft.Rows.Add(null);

                //var scro = new Eto.Forms.Scrollable();
                //scro.Size = new Size(400, 500);
                //scro.Content = panelLeft;

                var controls = GenLibraryPanel();
                var libControls = controls.allControls;
                var libraryLBox = controls.libraryLBox;

                // Add to program
                var borrowBtn = new Button() { Text = "Set to program" };
                borrowBtn.Click += (s, e) =>
                {
                    var selected = libraryLBox.SelectedItem;
                    if (selected == null)
                        return;

                    if (selected is PeopleAbridged pp)
                    {
                        _vm.People = pp;
                        peopleGroup.Content.Visible = true;
                    }
                    else if (selected is LightingAbridged lt)
                    {
                        _vm.Lighting = lt;
                        lightingGroup.Content.Visible = true;
                    }
                    else if (selected is ElectricEquipmentAbridged eq)
                    {
                        _vm.ElectricEquipment = eq;
                        equipmentGroup.Content.Visible = true;
                    }
                    else if (selected is GasEquipmentAbridged gs)
                    {
                        _vm.GasEquipment = gs;
                        gasEqpGroup.Content.Visible = true;
                    }
                    else if (selected is InfiltrationAbridged nf)
                    {
                        _vm.Infiltration = nf;
                        infiltrationGroup.Content.Visible = true;
                    }
                    else if (selected is VentilationAbridged vn)
                    {
                        _vm.Ventilation = vn;
                        ventilationGroup.Content.Visible = true;
                    }
                    else if (selected is SetpointAbridged sp)
                    {
                        _vm.Setpoint = sp;
                        setpoinGroup.Content.Visible = true;
                    }

                };


                var panelRight = new DynamicLayout();
                panelRight.BackgroundColor = Color.FromArgb(100, 100, 100);
                panelRight.Padding = new Padding(5);
                foreach (var item in libControls)
                {
                    panelRight.AddRow(item);
                }
                panelRight.AddRow(borrowBtn);
                panelRight.AddRow(null);


                //var panelAll = new DynamicLayout() { };
                //panelAll.AddRow(panelLeft, panelRight);

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(_vm.hbObj);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();


                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton) }
                };
                panelLeft.AddSeparateRow(buttons);

                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    new TableRow(panelLeft, panelRight)
                }
                };


            }
            catch (Exception e)
            {
                throw e;
            }


        }

        private static (List<Control> allControls, GridView libraryLBox) GenLibraryPanel()
        {
            var hbLoads = new List<HB.Energy.IIDdEnergyBaseModel>();
            hbLoads.AddRange(EnergyLibrary.DefaultPeopleLoads);
            hbLoads.AddRange(EnergyLibrary.DefaultLightingLoads);
            hbLoads.AddRange(EnergyLibrary.DefaultElectricEquipmentLoads);
            hbLoads.AddRange(EnergyLibrary.GasEquipmentLoads);
            hbLoads.AddRange(EnergyLibrary.DefaultInfiltrationLoads);
            hbLoads.AddRange(EnergyLibrary.DefaultVentilationLoads);
            hbLoads.AddRange(EnergyLibrary.DefaultSetpoints);
            hbLoads.Add(new PeopleAbridged("5662244", 1.555, "AlwayOn", "Typical setting", null, 0.6, 0.95));

            var itemsToShow = hbLoads;

            // Library List
            var library_GV = new GridView();
            library_GV.Height = 450;
            library_GV.Width = 400;
            //library_GV.GridLines = GridLines.Horizontal;
            library_GV.ShowHeader = false;
            library_GV.AllowMultipleSelection = false;

            // Preview 
            var preview_GV = new GridView() {};
            preview_GV.Height = 150;
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
                    itemsToShow = EnergyLibrary.StandardsSchedules.ToList<HB.Energy.IIDdEnergyBaseModel>();
                }
                else
                {
                    itemsToShow = hbLoads;
                }

                searchTBox.Text = null;
                //library_GV.Items.Clear();

                //var filteredItems = itemsToShow.Select(_ => new ListItem() { Text = $"{_.GetType().Name.Replace("Abridged","")}: {_.DisplayName ?? _.Identifier}", Tag = _ });
                library_GV.DataStore = itemsToShow;

            };

            var controls = new List<Control>() {
                "Library",
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
        //DynamicLayout GenTextBoxWithBtn(object currentValue, Func<object> getSelected, Action<object> setAction, Type setType)
        //{
        //    var panel = new DynamicLayout();

        //    CommonControl inputControl = new TextBox();

        //    var width = 250;
        //    if (currentValue is double cValue)
        //    {
        //        // user input number
        //        var num_NS = new NumericStepper();
        //        num_NS.MaximumDecimalPlaces = 2;
        //        num_NS.MinValue = 0;
        //        num_NS.Increment = 0.1;
        //        num_NS.Value = cValue;
        //        num_NS.Width = width;
        //        num_NS.Height = 25;
        //        inputControl = num_NS;

        //        panel.AddRow(inputControl);
        //    }
        //    else if (setType == typeof(string))
        //    {
        //        // user input text
        //        var tbx = new TextBox() {};
        //        tbx.Width = width;
        //        tbx.Text = currentValue?.ToString();
        //        inputControl = tbx;
        //        panel.AddRow(inputControl);
        //    }
        //    else
        //    {
        //        // Select from library
        //        var tbx = new TextBox() { PlaceholderText = "By Global" };
        //        tbx.Width = width - 25;
        //        tbx.Enabled = false;
        //        tbx.Text = currentValue?.ToString();
        //        inputControl = tbx;

        //        //tbx.Bindings = Binding.Delegate(() => getFunc().Identifier, (c) => setAction(c));
        //        var inWallBtn = new Button() { Text = string.IsNullOrEmpty(currentValue?.ToString()) ? "+" : "-", Width = 25 };
        //        inWallBtn.ToolTip = "Add from selected item of left library";
        //        inWallBtn.Click += (sender, e) =>
        //        {
        //            var txt = inWallBtn.Text;
        //            string newText = null;
        //            if (txt == "+")
        //            {
        //                var selectedConstr = getSelected() as HoneybeeSchema.IIDdBase;
        //                if (selectedConstr == null)
        //                    return;

        //                if (selectedConstr.GetType() != setType)
        //                {
        //                    MessageBox.Show(this, $"{selectedConstr.GetType().Name.Replace("Abridged", "")} cannot be set to where {setType.Name.Replace("Abridged", "")} is required!");
        //                    return;
        //                }
        //                setAction(selectedConstr);
        //                newText = selectedConstr.DisplayName ?? selectedConstr.Identifier;

        //                txt = "-";
        //            }
        //            else
        //            {
        //                newText = null;
        //                setAction(null);
        //                txt = "+";
        //            }

        //            //tbx.Text = newText;
        //            inWallBtn.Text = txt;
        //        };

        //        panel.AddRow(inputControl, inWallBtn);
        //    }


        //    return panel;
        //}
        private readonly int _inputControlWidth = 300;
        List<List<Control>> GenInputControl<TObject>(string inputName, Expression<Func<TObject, int>> propertyExpression) 
            where TObject : ViewModelBase
        {
            //var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 150 };
            // user input integer
            var num_NS = new NumericMaskedTextBox<int>();
            num_NS.Width = _inputControlWidth;
            num_NS.Height = 25;
            num_NS.ValueBinding.BindDataContext(propertyExpression);

            var rows = new List<List<Control>>()
            {
                new List<Control>(){ inputLabel },
                new List<Control>(){ num_NS }
            };
            return rows;
        }
        List<List<Control>> GenInputControl<TObject>(string inputName, Expression<Func<TObject, double>> propertyExpression) 
            where TObject: ViewModelBase
        {
            //var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 150 };
            // user input number
            var num_NS = new NumericMaskedTextBox<double>();
            num_NS.Width = _inputControlWidth;
            num_NS.Height = 25;
            num_NS.ValueBinding.BindDataContext(propertyExpression);

            var rows = new List<List<Control>>()
            {
                new List<Control>(){ inputLabel },
                new List<Control>(){ num_NS }
            };
            return rows;
        }
        List<List<Control>> GenInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyExpression) 
            where TObject : ViewModelBase
        {
            var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 150 };
            // user input text
            var text_TB = new TextBox();
            text_TB.Width = _inputControlWidth;
            text_TB.Height = 25;
            text_TB.TextBinding.BindDataContext(propertyExpression);

            var rows = new List<List<Control>>()
            {
                new List<Control>(){ inputLabel },
                new List<Control>(){ text_TB }
            };
            return rows;
        }

        List<List<Control>> GenInputControl<TObject>(string inputName, Type altNumType, Expression<Func<TObject, bool?>> ifIAltnumberExpression, Expression<Func<TObject, double>> propertyExpression)
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

            var rows = new List<List<Control>>()
            {
                new List<Control>(){ inputLabel },
                new List<Control>(){ altNum_CBox },
                new List<Control>(){ numInput }
            };
            return rows;

        }

        List<List<Control>> GenInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyExpression, Func<object> getSelectedFromLib) where TObject : ViewModelBase
        {
            var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 150};
            // Select from library
            var tbx = new TextBox() { PlaceholderText = "By Global" };
            tbx.Width = _inputControlWidth - 25;
            tbx.Enabled = false;
            tbx.TextBinding.BindDataContext(propertyExpression);
           
            // Button for selecting item from library
            var inWallBtn = new Button() { Text = tbx.Text == null ? "＋" : "−", Width = 25 };
            inWallBtn.ToolTip = "Add from selected item of left library";
            inWallBtn.Click += (sender, e) =>
            {
                var txt = inWallBtn.Text;
                string newText = null;
                if (txt == "＋")
                {
                    var selectedConstr = getSelectedFromLib() as HoneybeeSchema.IIDdBase;
                    if (selectedConstr == null)
                        return;

                    //if (selectedConstr.GetType() != setType)
                    //{
                    //    MessageBox.Show(this, $"{selectedConstr.GetType().Name.Replace("Abridged", "")} cannot be set to where {setType.Name.Replace("Abridged", "")} is required!");
                    //    return;
                    //}
                    //setAction(selectedConstr);
                    newText = selectedConstr.DisplayName ?? selectedConstr.Identifier;

                    txt = "−";
                }
                else
                {
                    newText = null;
                    //setAction(null);
                    txt = "＋";
                }

                //tbx.Text = newText;
                inWallBtn.Text = txt;
            };

            var rows = new List<List<Control>>()
            {
                new List<Control>(){ inputLabel },
                new List<Control>(){ tbx, inWallBtn }
            };
            return rows;
        }
       
        private GroupBox GenGroup(string groupName, List<List<Control>> inputCtrlRows, bool isNull, Action removeLoadAction)
        {
            var gp = new GroupBox() { Text = groupName, BackgroundColor = Color.FromArgb(255, 255, 255) };
            var layout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };
            foreach (var row in inputCtrlRows)
            {
                var rowItems = row.ToArray();
                layout.AddSeparateRow(rowItems);
            }
            //remove button
            var rm = new Button() { Text = $"Remove {groupName}" };
            rm.Click += (s, e) =>
            {
                layout.Visible = false;
                removeLoadAction();
            };
            layout.AddSeparateRow(rm);
            layout.Visible = !isNull;
            gp.Content = layout;
            
            return gp;
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
