using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using HoneybeeSchema;
using System.Linq.Expressions;

namespace Honeybee.UI
{

    public class Dialog_ProgramType: Dialog<HB.ProgramTypeAbridged>
    {

        public Dialog_ProgramType(HB.ProgramTypeAbridged ProgramType)
        {
            try
            {
                var vm = ProgramTypeViewModel.Instance;
                vm.hbObj = ProgramType ?? new HB.ProgramTypeAbridged(identifier: Guid.NewGuid().ToString());
           
                Padding = new Padding(5);
                Resizable = true;
                Title = "Construction Set - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);
                this.Icon = DialogHelper.HoneybeeIcon;

                var hbLoads = EnergyLibrary.DefaultPeopleLoads.ToList();
                hbLoads.Add(new PeopleAbridged("5662244", 1.555, "AlwayOn", "Typical setting", null, 0.6, 0.95));

                // Construction List
                var libraryLBox = new ListBox();
                libraryLBox.Height = 450;
                libraryLBox.Width = 300;

                //// Construction Layers
                //var constructionLayersLBox = new ListBox();
                //constructionLayersLBox.Height = 150;
                //constructionLayersLBox.Items.Add(new ListItem() { Text = "Construction Details" });

                //HB.OpaqueConstructionAbridged selectedConstr = null;
                var allItems = hbLoads.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                libraryLBox.Items.AddRange(allItems);
                //constrLBox.SelectedKeyChanged += (s, e) => 
                //{
                //    if (constrLBox.SelectedIndex == -1)
                //    {
                //        constructionLayersLBox.Items.Clear();
                //        constructionLayersLBox.Items.Add(new ListItem() { Text = "Construction Details" });
                //        return;
                //    }


                //    var selectedConst = (constrLBox.Items[constrLBox.SelectedIndex] as ListItem).Tag;
                //    var layers = new List<string>();
                //    if (selectedConst is HB.OpaqueConstructionAbridged opq)
                //    {
                //        layers.Add("----------------Outer------------------");
                //        layers.AddRange(opq.Layers);
                //        layers.Add("----------------Inner------------------");
                //    }
                //    else if (selectedConst is HB.WindowConstructionAbridged win)
                //    {
                //        layers.Add("----------------Outer------------------");
                //        layers.AddRange(win.Layers);
                //        layers.Add("----------------Inner------------------");
                //    }
                //    else if (selectedConst is HB.ShadeConstruction shd)
                //    {
                //        layers.Add($"{nameof(shd.SolarReflectance)}: {shd.SolarReflectance}");
                //        layers.Add($"{nameof(shd.VisibleReflectance)}: {shd.VisibleReflectance}");
                //        layers.Add($"{nameof(shd.IsSpecular)}: {shd.IsSpecular}");
                //    }
                //    else if (selectedConst is HB.AirBoundaryConstructionAbridged air)
                //    {
                //        layers.Add($"{nameof(air.AirMixingPerArea)}: {air.AirMixingPerArea}");
                //        layers.Add($"{nameof(air.AirMixingSchedule)}: {air.AirMixingSchedule}");
                //    }
                //    var layersItems = layers.Select(_ => new ListItem() { Text = _});
                //    constructionLayersLBox.Items.Clear();
                //    constructionLayersLBox.Items.AddRange(layersItems);

                //};


                //var searchTBox = new TextBox() { PlaceholderText= "Search"};

                //searchTBox.TextChanged += (sender, e) =>
                //{
                //    var input = searchTBox.Text;
                //    constrLBox.Items.Clear();
                //    if (string.IsNullOrWhiteSpace(input))
                //    {
                //        constrLBox.Items.AddRange(allItems);
                //        return;
                //    }
                //    var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                //    var filtered = constrs.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
                //    var filteredItems = filtered.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                //    constrLBox.Items.AddRange(filteredItems);

                //};

                //var constructionTypes = new DropDown();
                //constructionTypes.Items.Add(new ListItem() { Key = "Opaque", Text = "Opaque Construction" });
                //constructionTypes.Items.Add(new ListItem() { Key = "Window", Text = "Window Construction" });
                ////constructionTypes.Items.Add(new ListItem() { Key = "Shade Construction" });
                ////constructionTypes.Items.Add(new ListItem() { Key = "AirBoundary Construction" });
                //constructionTypes.SelectedIndex = 0;
                //constructionTypes.SelectedIndexChanged += (sender, e) =>
                //{
                //    var selectedType = constructionTypes.SelectedKey;

                //    if (selectedType == "Window")
                //    {
                //        constrs = EnergyLibrary.StandardsWindowConstructions;
                //    }
                //    else
                //    {
                //        constrs = EnergyLibrary.StandardsOpaqueConstructions;
                //    }
                //    searchTBox.Text = null;
                //    constrLBox.Items.Clear();

                //    var filteredItems = constrs.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                //    constrLBox.Items.AddRange(filteredItems);

                //};

                Func<HB.Energy.ISchedule> getSelectedSche = () => 
                {
                    if (libraryLBox.SelectedIndex == -1)
                        return null;
                    return (libraryLBox.Items[libraryLBox.SelectedIndex] as ListItem).Tag as HB.Energy.ISchedule; 
                };

                var defaultByProgramType = "By Room Program Type";
                //Get people
                vm.hbObj.People = vm.hbObj.People ?? new PeopleAbridged(Guid.NewGuid().ToString(), 0, "", "");
                vm.hbObj.People.LatentFraction = new Autocalculate();
                var ppl = new List<Control>() { };
                ppl.Add(GenInputControl("People/Area", (ProgramTypeViewModel m) => m.PPL_PeoplePerArea));
                ppl.Add(GenInputControl("Occupancy Schedule", (ProgramTypeViewModel m) => m.PPL_OccupancySchedule, getSelectedSche));
                ppl.Add(GenInputControl("Activity Schedule", (ProgramTypeViewModel m) => m.PPL_ActivitySchedule, getSelectedSche));
                ppl.Add(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.PPL_RadiantFraction));
                ppl.Add(GenInputControl("Latent Fraction", typeof(Autocalculate), (ProgramTypeViewModel m) => m.PPL_IsLatentFractionAutocalculate, (ProgramTypeViewModel m) => m.PPL_LatentFraction));
                var peopleGroup = GenGroup("People", ppl);

                vm.hbObj.Lighting = vm.hbObj.Lighting ?? new LightingAbridged(Guid.NewGuid().ToString(), 0, "");
                var lpd = new List<Control>() { };
                lpd.Add(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.LPD_WattsPerArea));
                lpd.Add(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.LPD_Schedule, getSelectedSche));
                lpd.Add(GenInputControl("Visible Schedule", (ProgramTypeViewModel m) => m.LPD_VisibleFraction));
                lpd.Add(GenInputControl("Radiant Fraction", (ProgramTypeViewModel m) => m.LPD_RadiantFraction));
                lpd.Add(GenInputControl("Return Air Fraction", (ProgramTypeViewModel m) => m.LPD_ReturnAirFraction));
                var lightingGroup = GenGroup("Lighting", lpd);

                vm.hbObj.ElectricEquipment = vm.hbObj.ElectricEquipment ?? new ElectricEquipmentAbridged(Guid.NewGuid().ToString(), 0, "");
                var eqp = new List<Control>() { };
                eqp.Add(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.EQP_WattsPerArea));
                eqp.Add(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.EQP_Schedule, getSelectedSche));
                eqp.Add(GenInputControl("Radiant Schedule", (ProgramTypeViewModel m) => m.EQP_RadiantFraction));
                eqp.Add(GenInputControl("Latent Fraction", (ProgramTypeViewModel m) => m.EQP_LatentFraction));
                eqp.Add(GenInputControl("Lost Fraction", (ProgramTypeViewModel m) => m.EQP_LostFraction));
                var equipmentGroup = GenGroup("Electric Equipment", eqp);

                vm.hbObj.GasEquipment = vm.hbObj.GasEquipment ?? new GasEquipmentAbridged(Guid.NewGuid().ToString(), 0, "");
                var gas = new List<Control>() { };
                gas.Add(GenInputControl("Watts/Area", (ProgramTypeViewModel m) => m.GAS_WattsPerArea));
                gas.Add(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.GAS_Schedule, getSelectedSche));
                gas.Add(GenInputControl("Radiant Schedule", (ProgramTypeViewModel m) => m.GAS_RadiantFraction));
                gas.Add(GenInputControl("Latent Fraction", (ProgramTypeViewModel m) => m.GAS_LatentFraction));
                gas.Add(GenInputControl("Lost Fraction", (ProgramTypeViewModel m) => m.GAS_LostFraction));
                var gasEqpGroup = GenGroup("Gas Equipment", gas);

                vm.hbObj.Infiltration = vm.hbObj.Infiltration ?? new InfiltrationAbridged(Guid.NewGuid().ToString(), 0, "");
                var inf = new List<Control>() { };
                inf.Add(GenInputControl("Flow/FacadeArea", (ProgramTypeViewModel m) => m.INF_FlowPerExteriorArea));
                inf.Add(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.INF_Schedule, getSelectedSche));
                inf.Add(GenInputControl("Velocity Coefficient", (ProgramTypeViewModel m) => m.INF_VelocityCoefficient));
                inf.Add(GenInputControl("Temperature Coefficient", (ProgramTypeViewModel m) => m.INF_TemperatureCoefficient));
                inf.Add(GenInputControl("Constant Coefficient", (ProgramTypeViewModel m) => m.INF_ConstantCoefficient));
                var infiltrationGroup = GenGroup("Infiltration", inf);

                vm.hbObj.Ventilation = vm.hbObj.Ventilation ?? new VentilationAbridged(Guid.NewGuid().ToString());
                var vnt = new List<Control>() { };
                vnt.Add(GenInputControl("Flow/Area", (ProgramTypeViewModel m) => m.VNT_FlowPerArea));
                vnt.Add(GenInputControl("Schedule", (ProgramTypeViewModel m) => m.INF_Schedule, getSelectedSche));
                vnt.Add(GenInputControl("Flow/Person", (ProgramTypeViewModel m) => m.VNT_FlowPerPerson));
                vnt.Add(GenInputControl("Flow/Zone", (ProgramTypeViewModel m) => m.VNT_FlowPerZone));
                vnt.Add(GenInputControl("AirChanges/Hour", (ProgramTypeViewModel m) => m.VNT_AirChangesPerHour));
                var ventilationGroup = GenGroup("Ventilation", vnt);

                vm.hbObj.Setpoint = vm.hbObj.Setpoint ?? new SetpointAbridged(Guid.NewGuid().ToString(), "cooling sch", "heating sch");
                var spt = new List<Control>() { };
                spt.Add(GenInputControl("Cooling Schedule", (ProgramTypeViewModel m) => m.SPT_CoolingSchedule, getSelectedSche));
                spt.Add(GenInputControl("Heating Schedule", (ProgramTypeViewModel m) => m.SPT_HeatingSchedule, getSelectedSche));
                spt.Add(GenInputControl("Humidifying Schedule", (ProgramTypeViewModel m) => m.SPT_HumidifyingSchedule, getSelectedSche));
                spt.Add(GenInputControl("Dehumidifying Schedule", (ProgramTypeViewModel m) => m.SPT_DehumidifyingSchedule, getSelectedSche));
                var setpoinGroup = GenGroup("Setpoint", spt);


                //Left panel
                var panelLeft = new DynamicLayout() { DataContext = vm };
                var panelNames = new DynamicLayout();
                panelNames.Padding = new Padding(10, 5, 15, 5);
                panelNames.Spacing = new Size(5, 5);
                panelLeft.BeginScrollable(BorderType.None);
                panelLeft.Height = 600;
                var nameTbx = new TextBox();
                vm.hbObj.DisplayName = vm.hbObj.DisplayName ?? $"New ProgramType {vm.hbObj.Identifier.Substring(0, 5)}";
                nameTbx.TextBinding.Bind(vm.hbObj, c => c.DisplayName);
                panelNames.AddRow(new Label() { Text = "ID: ", Width = 75 }, new Label() { Text = vm.hbObj.Identifier, Enabled = false });
                panelNames.AddRow(new Label() { Text = "Name:", Width = 75 }, nameTbx);
                panelLeft.AddRow(panelNames);
                panelLeft.AddRow(peopleGroup);
                panelLeft.AddRow(lightingGroup);
                panelLeft.AddRow(equipmentGroup);
                panelLeft.AddRow(gasEqpGroup);
                panelLeft.AddRow(infiltrationGroup);
                panelLeft.AddRow(ventilationGroup);
                panelLeft.AddRow(setpoinGroup);
                panelLeft.AddRow(null);


                var borrowBtn = new Button() { Text = "Borrow" };
                
                borrowBtn.Click += (s, e) =>
                {
                    var selected = (libraryLBox.Items[libraryLBox.SelectedIndex] as ListItem).Tag as PeopleAbridged;
                    vm.UpdatePeople(selected);
                };


                var panelRight = new DynamicLayout();
                panelRight.Padding = new Padding(5);
                //panelRight.AddRow(constructionTypes);
                //panelRight.AddRow(searchTBox);
                panelRight.AddRow(libraryLBox);
                //panelRight.AddRow(constructionLayersLBox);
                panelRight.AddRow(borrowBtn, null);
                panelRight.AddRow(null);

                var panelAll = new DynamicLayout() { };
                panelAll.AddRow(panelLeft, panelRight);


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(vm.hbObj);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var hbData = new Button { Text = "HBData" };
                hbData.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, vm.hbObj.ToJson(), "Honeybee Data");

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null, hbData) }
                };


                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    panelAll,
                    new TableRow(buttons),
                    null
                }
                };

               
            }
            catch (Exception e)
            {
                throw e;
            }


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
        private readonly int _inputControlWidth = 250;
        DynamicLayout GenInputControl<TObject>(string inputName, Expression<Func<TObject, int>> propertyExpression) 
            where TObject : ViewModelBase
        {
            var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 125 };
            // user input integer
            var num_NS = new NumericMaskedTextBox<int>();
            num_NS.Width = _inputControlWidth;
            num_NS.Height = 25;
            num_NS.ValueBinding.BindDataContext(propertyExpression);
            panel.AddRow(inputLabel, num_NS);
            return panel;
        }
        DynamicLayout GenInputControl<TObject>(string inputName, Expression<Func<TObject, double>> propertyExpression) 
            where TObject: ViewModelBase
        {
            var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 125 };
            // user input number
            var num_NS = new NumericMaskedTextBox<double>();
            num_NS.Width = _inputControlWidth;
            num_NS.Height = 25;
            num_NS.ValueBinding.BindDataContext(propertyExpression);
            panel.AddRow(inputLabel, num_NS);
            return panel;
        }
        DynamicLayout GenInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyExpression) 
            where TObject : ViewModelBase
        {
            var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 125 };
            // user input text
            var text_TB = new TextBox();
            text_TB.Width = _inputControlWidth;
            text_TB.Height = 25;
            text_TB.TextBinding.BindDataContext(propertyExpression);

            panel.AddRow(inputLabel, text_TB);
            return panel;
        }

        DynamicLayout GenInputControl<TObject>(string inputName, Type altNumType, Expression<Func<TObject, bool?>> ifIAltnumberExpression, Expression<Func<TObject, double>> propertyExpression)
            where TObject : ViewModelBase
        {
            var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 125 };
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

            panel.AddRow(inputLabel, altNum_CBox);
            panel.AddRow(new Label() {Width = 125 }, numInput);

            return panel;
        }

        DynamicLayout GenInputControl<TObject>(string inputName, Expression<Func<TObject, string>> propertyExpression, Func<object> getSelectedFromLib) where TObject : ViewModelBase
        {
            var panel = new DynamicLayout();
            var inputLabel = new Label() { Text = inputName, Width = 125 };
            // Select from library
            var tbx = new TextBox() { PlaceholderText = "By Global" };
            tbx.Width = _inputControlWidth - 25;
            tbx.Enabled = false;
            tbx.TextBinding.BindDataContext(propertyExpression);
           
            // Button for selecting item from library
            var inWallBtn = new Button() { Text = tbx.Text == null ? "+" : "-", Width = 25 };
            inWallBtn.ToolTip = "Add from selected item of left library";
            inWallBtn.Click += (sender, e) =>
            {
                var txt = inWallBtn.Text;
                string newText = null;
                if (txt == "+")
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

                    txt = "-";
                }
                else
                {
                    newText = null;
                    //setAction(null);
                    txt = "+";
                }

                //tbx.Text = newText;
                inWallBtn.Text = txt;
            };

            panel.AddRow(inputLabel, tbx, inWallBtn);
            return panel;
        }
        //DynamicLayout GenTextBoxWithBtn<TObject,TValue>(Func<object> getSelected, Expression<Func<TObject, TValue>> propertyExpression, Type setType) where TObject:ViewModelBase
        //{
        //    var panel = new DynamicLayout();

        //    CommonControl inputControl = new TextBox();

        //    var width = 250;
        //    if (typeof(TValue) == typeof(double))
        //    {
        //        // user input number
        //        var num_NS = new NumericStepper();
        //        num_NS.MaximumDecimalPlaces = 2;
        //        num_NS.MinValue = 0;
        //        num_NS.Increment = 0.1;
        //        //num_NS.Value = cValue;
        //        num_NS.Width = width;
        //        num_NS.Height = 25;
        //        num_NS.ValueBinding.BindDataContext(propertyExpression);
        //        inputControl = num_NS;

        //        panel.AddRow(inputControl);
        //    }
        //    else if (typeof(TValue) == typeof(string))
        //    {
        //        // user input text
        //        var tbx = new TextBox() { };
        //        tbx.Width = width;
        //        Func<ProgramTypeViewModel, string> f = (ProgramTypeViewModel m) => m.People.DisplayName;
        //        Expression<Func<ViewModelBase, string>> ex = (ProgramTypeViewModel m) => m.People.DisplayName;
      
        //        tbx.TextBinding.BindDataContext(ex);
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
        //        //var inWallBtn = new Button() { Text = string.IsNullOrEmpty(currentValue?.ToString()) ? "+" : "-", Width = 25 };
        //        //inWallBtn.ToolTip = "Add from selected item of left library";
        //        //inWallBtn.Click += (sender, e) =>
        //        //{
        //        //    var txt = inWallBtn.Text;
        //        //    string newText = null;
        //        //    if (txt == "+")
        //        //    {
        //        //        var selectedConstr = getSelected() as HoneybeeSchema.IIDdBase;
        //        //        if (selectedConstr == null)
        //        //            return;

        //        //        if (selectedConstr.GetType() != setType)
        //        //        {
        //        //            MessageBox.Show(this, $"{selectedConstr.GetType().Name.Replace("Abridged", "")} cannot be set to where {setType.Name.Replace("Abridged", "")} is required!");
        //        //            return;
        //        //        }
        //        //        setAction(selectedConstr);
        //        //        newText = selectedConstr.DisplayName ?? selectedConstr.Identifier;

        //        //        txt = "-";
        //        //    }
        //        //    else
        //        //    {
        //        //        newText = null;
        //        //        setAction(null);
        //        //        txt = "+";
        //        //    }

        //        //    //tbx.Text = newText;
        //        //    inWallBtn.Text = txt;
        //        //};

        //        //panel.AddRow(inputControl, inWallBtn);
        //        panel.AddRow(inputControl,);
        //    }


        //    return panel;
        //}

        //private GroupBox GenPanel(string groupName, Func<HB.Energy.IConstruction> getSelected, IEnumerable<(string label, object currentValue, Action<object> setAction, Type setType)> setActions)
        //{
            
        //    //Wall Construction Set
        //    var wallGroup = new GroupBox() { Text = groupName };
            
        //    var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };
        //    foreach (var item in setActions)
        //    {
        //        var inWall = GenTextBoxWithBtn(item.currentValue, getSelected, item.setAction, item.setType);
        //        wallLayout.AddRow(new Label() { Text = item.label, Width = 125 }, inWall, null);
        //    }
           
        //    //var exWall = GenTextBoxWithBtn(getSelected, (cons) => c.ExteriorConstruction = cons.Identifier);
        //    //var gWall =  GenTextBoxWithBtn(getSelected, (cons) => c.GroundConstruction = cons.Identifier);

        //    //wallLayout.AddRow(new Label() { Text = "Exterior", Width = 75 }, exWall, null);
        //    //wallLayout.AddRow(new Label() { Text = "Ground", Width = 75 }, gWall, null);
        //    wallGroup.Content = wallLayout;
        //    return wallGroup;

        //}
        private GroupBox GenGroup(string groupName, List<Control> inputCtrls)
        {
            var wallGroup = new GroupBox() { Text = groupName };
            var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };
            foreach (var item in inputCtrls)
            {
                wallLayout.AddRow(item);
            }
            wallGroup.Content = wallLayout;
            return wallGroup;
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
