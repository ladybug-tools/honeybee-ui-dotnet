using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using HoneybeeSchema;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Honeybee.UI
{
   
    public class Dialog_ConstructionSet: Dialog<HB.ConstructionSetAbridged>
    {

        private static IEnumerable<HB.Energy.IConstruction> _opaqueConstructions;

        public IEnumerable<HB.Energy.IConstruction> OpaqueConstructions
        {
            get
            {
                var libObjs = HB.Helper.EnergyLibrary.StandardsOpaqueConstructions.Values.ToList();
                var inModelObjs = this.ModelEnergyProperties.Constructions.OfType<HB.OpaqueConstructionAbridged>();

                libObjs.AddRange(inModelObjs);
                _opaqueConstructions = libObjs;

                return _opaqueConstructions;
            }
        }

        private static IEnumerable<HB.Energy.IConstruction> _windowConstructions;

        public IEnumerable<HB.Energy.IConstruction> WindowConstructions
        {
            get
            {
                var libObjs = HB.Helper.EnergyLibrary.StandardsWindowConstructions.Values.ToList();
                var inModelObjs = this.ModelEnergyProperties.Constructions.OfType<HB.WindowConstructionAbridged>();

                libObjs.AddRange(inModelObjs);
                _windowConstructions = libObjs;

                return _windowConstructions;
            }
        }

        private static IEnumerable<HB.Energy.IConstruction> _airBoundaryConstructions;

        public IEnumerable<HB.Energy.IConstruction> AirBoundaryConstructions
        {
            get
            {
                _airBoundaryConstructions = _airBoundaryConstructions ?? this.ModelEnergyProperties.Constructions.OfType<HB.AirBoundaryConstructionAbridged>().ToList();
                return _airBoundaryConstructions;
            }
        }

        private static IEnumerable<HB.Energy.IConstruction> _shadeConstructions;

        public IEnumerable<HB.Energy.IConstruction> ShadeConstructions
        {
            get
            {
                _shadeConstructions = _shadeConstructions ?? this.ModelEnergyProperties.Constructions.OfType<HB.ShadeConstruction>().ToList();
                return _shadeConstructions;
            }
        }

        private HB.ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_ConstructionSet(HB.ModelEnergyProperties libSource, HB.ConstructionSetAbridged constructionSet)
        {
            try
            {
                this.ModelEnergyProperties = libSource;
                var cSet = constructionSet ?? new HB.ConstructionSetAbridged(identifier: Guid.NewGuid().ToString());
                //cSet.WallSet = new WallConstructionSetAbridged("interior C");

                Padding = new Padding(5);
                Resizable = true;
                Title = $"Construction Set - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 600);
                this.Icon = DialogHelper.HoneybeeIcon;

                IEnumerable<HB.Energy.IConstruction> constrs = OpaqueConstructions;

                // Construction List
                var lib = new GridView();
                lib.Height = 450;
                lib.Width = 300;

                lib.ShowHeader = false;
                var nameCol = new GridColumn() { DataCell = new TextBoxCell(0) };
                lib.Columns.Add(nameCol);

                // Library items
                lib.DataStore = constrs;


                var idCell = new TextBoxCell
                {
                    Binding = Binding.Delegate<HB.Energy.IIDdEnergyBaseModel, string>(r => r.DisplayName ?? r.Identifier)
                };
                lib.Columns.Add(new GridColumn() { DataCell = idCell });

                // Construction Layers
                var constructionLayersLBox = new ListBox();
                constructionLayersLBox.Height = 150;
                constructionLayersLBox.Items.Add(new ListItem() { Text = "Construction Details" });

          
                lib.MouseMove += (sender, e) =>
                {
                    if (e.Buttons != MouseButtons.Primary)
                        return;

                    var dragableArea = lib.Bounds;
                    dragableArea.Width -= 20;
                    dragableArea.Height -= 20;
                    var iscontained = e.Location.Y < dragableArea.Height && e.Location.X < dragableArea.Width;
                    //name.Text = $"{dragableArea.Width}x{dragableArea.Height}, {new Point(e.Location).X}:{new Point(e.Location).Y}, {dragableArea.Contains(new Point(e.Location))}";
                    if (!iscontained)
                        return;

                    if (lib.SelectedItem == null)
                        return;

                    var selected = (lib.SelectedItem as HB.HoneybeeObject).ToJson();
                    var data = new DataObject();
                    data.SetString(selected, "HBObj");
                    lib.DoDragDrop(data, DragEffects.Move);
                    e.Handled = true;

                };
                lib.SelectedItemsChanged += (s, e) => 
                {
                    //Clear preview first
                    constructionLayersLBox.Items.Clear();

                    //Check current selected item from library
                    var selItem = lib.SelectedItem as HB.HoneybeeObject;
                    if (selItem == null)
                        return;

                    //Update Preview
                    var selectedConst = selItem;
                    var layers = new List<string>();
                    if (selectedConst is HB.OpaqueConstructionAbridged opq)
                    {
                        layers.Add("----------------Outer------------------");
                        layers.AddRange(opq.Materials);
                        layers.Add("----------------Inner------------------");
                    }
                    else if (selectedConst is HB.WindowConstructionAbridged win)
                    {
                        layers.Add("----------------Outer------------------");
                        layers.AddRange(win.Materials);
                        layers.Add("----------------Inner------------------");
                    }
                    else if (selectedConst is HB.ShadeConstruction shd)
                    {
                        layers.Add($"{nameof(shd.SolarReflectance)}: {shd.SolarReflectance}");
                        layers.Add($"{nameof(shd.VisibleReflectance)}: {shd.VisibleReflectance}");
                        layers.Add($"{nameof(shd.IsSpecular)}: {shd.IsSpecular}");
                    }
                    else if (selectedConst is HB.AirBoundaryConstructionAbridged air)
                    {
                        layers.Add($"{nameof(air.AirMixingPerArea)}: {air.AirMixingPerArea}");
                        layers.Add($"{nameof(air.AirMixingSchedule)}: {air.AirMixingSchedule}");
                    }
                    var layersItems = layers.Select(_ => new ListItem() { Text = _});
                    constructionLayersLBox.Items.Clear();
                    constructionLayersLBox.Items.AddRange(layersItems);

                };


                var searchTBox = new TextBox() { PlaceholderText= "Search"};
                
                searchTBox.TextChanged += (sender, e) =>
                {
                    var input = searchTBox.Text;
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        lib.DataStore = constrs;
                        return;
                    }
                    var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                    var filtered = constrs.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
                    lib.DataStore = filtered;

                };

                var constructionTypes = new DropDown();
                constructionTypes.Items.Add(new ListItem() { Key = "Opaque", Text = "Opaque Construction" });
                constructionTypes.Items.Add(new ListItem() { Key = "Window", Text = "Window Construction" });
                constructionTypes.Items.Add(new ListItem() { Key = "Shade", Text = "Shade Construction" });
                constructionTypes.Items.Add(new ListItem() { Key = "AirBoundary", Text = "AirBoundary Construction" });
                constructionTypes.SelectedIndex = 0;
                constructionTypes.SelectedIndexChanged += (sender, e) =>
                {
                    var selectedType = constructionTypes.SelectedKey;
                    switch (selectedType)
                    {
                        case "Opaque":
                            constrs = OpaqueConstructions;
                            break;
                        case "Window":
                            constrs = WindowConstructions;
                            break;
                        case "Shade":
                            constrs = ShadeConstructions;
                            break;
                        case "AirBoundary":
                            constrs = AirBoundaryConstructions;
                            break;
                        default:
                            break;
                    }

                    searchTBox.Text = null;
                    lib.DataStore = constrs;


                };

                Func<HB.Energy.IConstruction> getSelected = () => 
                {
                    if (lib.SelectedRow == -1)
                        return null;
                    return lib.SelectedItem as HB.Energy.IConstruction; 
                }; 

                //WallConstructionSetAbridged
                cSet.WallSet = cSet.WallSet ?? new HB.WallConstructionSetAbridged();
                var setWallSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setWallSetActions.Add(("Exterior", cSet.WallSet.ExteriorConstruction, (cons) => cSet.WallSet.ExteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setWallSetActions.Add(("Ground", cSet.WallSet.GroundConstruction, (cons) => cSet.WallSet.GroundConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setWallSetActions.Add(("Interior", cSet.WallSet.InteriorConstruction, (cons) => cSet.WallSet.InteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                var wallGroup = GenPanelOpaqueConstrSet("Wall Construction Set", getSelected, setWallSetActions);

                //FloorConstructionSetAbridged
                cSet.FloorSet = cSet.FloorSet ?? new HB.FloorConstructionSetAbridged();
                var setfloorSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setfloorSetActions.Add(("Exterior", cSet.FloorSet.ExteriorConstruction, (cons) => cSet.FloorSet.ExteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setfloorSetActions.Add(("Ground", cSet.FloorSet.GroundConstruction, (cons) => cSet.FloorSet.GroundConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setfloorSetActions.Add(("Interior", cSet.FloorSet.InteriorConstruction, (cons) => cSet.FloorSet.InteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                var floorGroup = GenPanelOpaqueConstrSet("Floor Construction Set", getSelected, setfloorSetActions);

                //RoofCeilingConstructionSetAbridged
                cSet.RoofCeilingSet = cSet.RoofCeilingSet ?? new HB.RoofCeilingConstructionSetAbridged();
                var setRoofSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setRoofSetActions.Add(("Exterior", cSet.RoofCeilingSet.ExteriorConstruction, (cons) => cSet.RoofCeilingSet.ExteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setRoofSetActions.Add(("Ground", cSet.RoofCeilingSet.GroundConstruction, (cons) => cSet.RoofCeilingSet.GroundConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setRoofSetActions.Add(("Interior", cSet.RoofCeilingSet.InteriorConstruction, (cons) => cSet.RoofCeilingSet.InteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                var roofCeilingGroup = GenPanelOpaqueConstrSet("Roof/Ceiling Construction Set", getSelected, setRoofSetActions);

                //ApertureConstructionSetAbridged
                cSet.ApertureSet = cSet.ApertureSet ?? new HB.ApertureConstructionSetAbridged();
                var setApertureSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setApertureSetActions.Add(("Exterior", cSet.ApertureSet.WindowConstruction, (cons) => cSet.ApertureSet.WindowConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setApertureSetActions.Add(("Operable", cSet.ApertureSet.OperableConstruction, (cons) => cSet.ApertureSet.OperableConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setApertureSetActions.Add(("Skylight", cSet.ApertureSet.SkylightConstruction, (cons) => cSet.ApertureSet.SkylightConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setApertureSetActions.Add(("Interior", cSet.ApertureSet.InteriorConstruction, (cons) => cSet.ApertureSet.InteriorConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                var apertureGroup = GenPanelOpaqueConstrSet("Aperture Construction Set", getSelected, setApertureSetActions);

                //DoorConstructionSetAbridged
                cSet.DoorSet = cSet.DoorSet ?? new HB.DoorConstructionSetAbridged();
                var setDoorSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setDoorSetActions.Add(("Exterior", cSet.DoorSet.ExteriorConstruction, (cons) => cSet.DoorSet.ExteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setDoorSetActions.Add(("Interior", cSet.DoorSet.InteriorConstruction, (cons) => cSet.DoorSet.InteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setDoorSetActions.Add(("Exterior Glass", cSet.DoorSet.ExteriorGlassConstruction, (cons) => cSet.DoorSet.ExteriorGlassConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setDoorSetActions.Add(("Interior Glass", cSet.DoorSet.InteriorGlassConstruction, (cons) => cSet.DoorSet.InteriorGlassConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setDoorSetActions.Add(("Overhead", cSet.DoorSet.OverheadConstruction, (cons) => cSet.DoorSet.OverheadConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                var doorGroup = GenPanelOpaqueConstrSet("Door Construction Set", getSelected, setDoorSetActions);

                //ShadeConstruction
                //var shadeSet = cSet.ShadeConstruction;
                var setShadeSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setShadeSetActions.Add(("Shade", cSet.ShadeConstruction, (cons) => cSet.ShadeConstruction = cons?.Identifier, typeof(HB.ShadeConstruction)));
                var shadeGroup = GenPanelOpaqueConstrSet("Shade Construction", getSelected, setShadeSetActions);

                //AirBoundaryConstruction
                var setAirBdActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setAirBdActions.Add(("Air Boundary", cSet.AirBoundaryConstruction, (cons) => cSet.AirBoundaryConstruction = cons?.Identifier, typeof(HB.AirBoundaryConstructionAbridged)));
                var airBdGroup = GenPanelOpaqueConstrSet("Air Boundary Construction", getSelected, setAirBdActions);
      

                //Left panel
                var panelLeft = new DynamicLayout();
                panelLeft.DefaultSpacing = new Size(0, 5);
                var panelNames = new DynamicLayout();
                panelNames.Padding = new Padding(10, 5, 15, 5);
                panelNames.Spacing = new Size(5, 5);
                panelLeft.BeginScrollable(BorderType.None);
                panelLeft.Height = 600;
                var nameTbx = new TextBox();
                cSet.DisplayName = cSet.DisplayName ?? $"New ConstructionSet {cSet.Identifier.Substring(0, 5)}";
                nameTbx.TextBinding.Bind(cSet, c => c.DisplayName);
                panelNames.AddRow(new Label() { Text = "Name:", Width = 75 }, nameTbx);
                panelLeft.AddRow(panelNames);
                panelLeft.AddRow(wallGroup);
                panelLeft.AddRow(floorGroup);
                panelLeft.AddRow(roofCeilingGroup);
                panelLeft.AddRow(apertureGroup);
                panelLeft.AddRow(doorGroup);
                panelLeft.AddRow(shadeGroup);
                panelLeft.AddRow(airBdGroup);

                var panelRight = new DynamicLayout();
                panelRight.Padding = new Padding(5);
                panelRight.AddRow(constructionTypes);
                panelRight.AddRow(searchTBox);
                panelRight.AddRow(lib);
                panelRight.AddRow(constructionLayersLBox);

                var panelAll = new DynamicLayout() { };
                panelAll.AddRow(panelLeft, panelRight);


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(cSet);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var hbData = new Button { Text = "Schema Data" };
                hbData.Click += (sender, e) => Dialog_Message.Show(Config.Owner, cSet.ToJson(true), "Schema Data");

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
        private Eto.Drawing.Color _defaultTextBackgroundColor = new TextBox().BackgroundColor;

        Control GenDropInArea(string currentValue, Action<HB.Energy.IConstruction> setAction, Type setType)
        {
            //var width = 300;
            //var height = 60;

            var layerPanel = new PixelLayout();
            var dropInValue = new TextBox() { PlaceholderText = "Drag from library" };
            dropInValue.Width = 300;
            dropInValue.Enabled = false;
            dropInValue.Text = currentValue;
            dropInValue.Height = 25;
          
            var backGround = Eto.Drawing.Color.FromArgb(230, 230, 230);
            //dropInValue.BackgroundColor = backGround;

            var dropIn = new Drawable();
            dropIn.AllowDrop = true;
            dropIn.Width = dropInValue.Width;
            dropIn.Height = 25;
            dropIn.BackgroundColor = Colors.Transparent;

            var deleteBtn = new Button();
            deleteBtn.Text = "✕";
            deleteBtn.Width = 24;
            deleteBtn.Height = 24;
            deleteBtn.Visible = !string.IsNullOrEmpty(currentValue);
            deleteBtn.Click += (s, e) => 
            {
                dropInValue.Text = null;
                deleteBtn.Visible = false;
                setAction(null);
                
            };

            dropIn.DragLeave += (sender, e) =>
            {
                dropInValue.BackgroundColor = _defaultTextBackgroundColor;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                dropInValue.BackgroundColor = Colors.Gray;
            };
            dropIn.DragDrop += (sender, e) =>
            {
                // Get drop-in object
                var value = e.Data.GetString("HBObj");

                HB.Energy.IConstruction newValue = null;
                try
                {
                    newValue = setType.GetMethod("FromJson").Invoke(null, new object[] { value }) as HB.Energy.IConstruction;
                }
                catch (TargetInvocationException ex)
                {
                    newValue = null;
                }
                if (newValue == null)
                {
                    MessageBox.Show(this, $"{setType.Name.Replace("Abridged", "")} is required!");
                    return;
                }


                deleteBtn.Visible = true;
                dropInValue.Text = newValue.DisplayName?? newValue.Identifier;
                setAction(newValue);

            };

       
            layerPanel.Add(dropInValue, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            layerPanel.Add(deleteBtn, dropInValue.Width-24, 0);
            return layerPanel;
        }


        DynamicLayout GenTextBoxWithBtn(string currentValue, Func<HB.Energy.IConstruction> getSelected, Action<HB.Energy.IConstruction> setAction, Type setType)
        {
            var panel = new DynamicLayout();
            var tbx = new TextBox() { PlaceholderText = "By Global" };
            tbx.Width = 300;
            tbx.Enabled = false;
            tbx.Text = currentValue;

            //tbx.Bindings = Binding.Delegate(() => getFunc().Identifier, (c) => setAction(c));
            var inWallBtn = new Button() { Text = string.IsNullOrEmpty(currentValue)? "+": "-", Width = 25 };
            inWallBtn.Click += (sender, e) =>
            {
                var txt = inWallBtn.Text;
                string newC = null;
                if (txt == "+")
                {
                    var selectedConstr = getSelected();
                    if (selectedConstr == null)
                        return;

                    if (selectedConstr.GetType() != setType)
                    {
                        MessageBox.Show(this, $"{selectedConstr.GetType().Name.Replace("Abridged", "")} cannot be set to where {setType.Name.Replace("Abridged", "")} is required!");
                        return;
                    }
                    //var selectedConstr = (tbox.Items[tbox.SelectedIndex] as ListItem).Tag as OpaqueConstructionAbridged;
                    setAction(selectedConstr);
                    newC = selectedConstr.DisplayName ?? selectedConstr.Identifier;

                    txt = "-";
                }
                else
                {
                    newC = null;
                    setAction(null);
                    txt = "+";
                }

                tbx.Text = newC;
                inWallBtn.Text = txt;
            };

            panel.AddRow(tbx, inWallBtn);
            return panel;
        }

        private GroupBox GenPanelOpaqueConstrSet(string groupName, Func<HB.Energy.IConstruction> getSelected, IEnumerable<(string label, string currentValue, Action<HB.Energy.IConstruction> setAction, Type setType)> setActions)
        {
            
            //Wall Construction Set
            var wallGroup = new GroupBox() { Text = groupName };
            var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };

            foreach (var item in setActions)
            {
                var inWall = GenDropInArea(item.currentValue, item.setAction, item.setType);
                wallLayout.AddRow(new Label() { Text = item.label, Width = 75 }, inWall);
            }
           
            //var exWall = GenTextBoxWithBtn(getSelected, (cons) => c.ExteriorConstruction = cons.Identifier);
            //var gWall =  GenTextBoxWithBtn(getSelected, (cons) => c.GroundConstruction = cons.Identifier);

            //wallLayout.AddRow(new Label() { Text = "Exterior", Width = 75 }, exWall, null);
            //wallLayout.AddRow(new Label() { Text = "Ground", Width = 75 }, gWall, null);
            wallGroup.Content = wallLayout;
            return wallGroup;

        }

        

    }
}
