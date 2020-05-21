using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using HoneybeeSchema;
using System.Text.RegularExpressions;

namespace Honeybee.UI
{
    public class dropArea: Eto.Forms.Control
    {

    }
    public class Dialog_ConstructionSet: Dialog<HB.ConstructionSetAbridged>
    {

        private static IEnumerable<HB.Energy.IConstruction> _opaqueConstructions;

        public IEnumerable<HB.Energy.IConstruction> OpaqueConstructions
        {
            get
            {
                var libObjs = HB.Helper.EnergyLibrary.StandardsOpaqueConstructions.ToList();
                var inModelObjs = HB.Helper.EnergyLibrary.InModelEnergyProperties.Constructions
                    .Where(_ => _.Obj is HB.OpaqueConstructionAbridged)
                    .Select(_ => _.Obj as HB.OpaqueConstructionAbridged);

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
                var libObjs = HB.Helper.EnergyLibrary.StandardsWindowConstructions.ToList();
                var inModelObjs = HB.Helper.EnergyLibrary.InModelEnergyProperties.Constructions
                    .Where(_ => _.Obj is HB.WindowConstructionAbridged)
                    .Select(_ => _.Obj as HB.WindowConstructionAbridged);

                libObjs.AddRange(inModelObjs);
                _windowConstructions = libObjs;

                return _windowConstructions;
            }
        }
        public Dialog_ConstructionSet(HB.ConstructionSetAbridged constructionSet)
        {
            try
            {
                
                var cSet = constructionSet ?? new HB.ConstructionSetAbridged(identifier: Guid.NewGuid().ToString());
                //cSet.WallSet = new WallConstructionSetAbridged("interior C");

                Padding = new Padding(5);
                Resizable = true;
                Title = "Construction Set - Honeybee";
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(450, 200);
                this.Icon = DialogHelper.HoneybeeIcon;

                IEnumerable<HB.Energy.IConstruction> constrs = OpaqueConstructions;

                // Construction List
                var constrLBox = new ListBox();
                constrLBox.Height = 450;
                constrLBox.Width = 300;

                // Construction Layers
                var constructionLayersLBox = new ListBox();
                constructionLayersLBox.Height = 150;
                constructionLayersLBox.Items.Add(new ListItem() { Text = "Construction Details" });

                //HB.OpaqueConstructionAbridged selectedConstr = null;
                var allItems = constrs.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Key = _.DisplayName ?? _.Identifier, Tag = _ });
                constrLBox.Items.AddRange(allItems);
                constrLBox.MouseMove += (sender, e) =>
                {
                    var dragableArea = constrLBox.Bounds;
                    dragableArea.Width -= 20;
                    dragableArea.Height -= 20;
                    var iscontained = e.Location.Y < dragableArea.Height && e.Location.X < dragableArea.Width;
                    //name.Text = $"{dragableArea.Width}x{dragableArea.Height}, {new Point(e.Location).X}:{new Point(e.Location).Y}, {dragableArea.Contains(new Point(e.Location))}";
                    if (!iscontained)
                        return;

                    if (e.Buttons == MouseButtons.Primary && constrLBox.SelectedIndex != -1)
                    {
                        var selected = ((constrLBox.Items[constrLBox.SelectedIndex] as ListItem).Tag as HB.HoneybeeObject).ToJson();
                        var data = new DataObject();
                        data.SetObject(selected, "HBObj");
                        constrLBox.DoDragDrop(data, DragEffects.Move);
                        e.Handled = true;
                    }
                };
                constrLBox.SelectedKeyChanged += (s, e) => 
                {
                    if (constrLBox.SelectedIndex == -1)
                    {
                        constructionLayersLBox.Items.Clear();
                        constructionLayersLBox.Items.Add(new ListItem() { Text = "Construction Details" });
                        return;
                    }
                        

                    var selectedConst = (constrLBox.Items[constrLBox.SelectedIndex] as ListItem).Tag;
                    var layers = new List<string>();
                    if (selectedConst is HB.OpaqueConstructionAbridged opq)
                    {
                        layers.Add("----------------Outer------------------");
                        layers.AddRange(opq.Layers);
                        layers.Add("----------------Inner------------------");
                    }
                    else if (selectedConst is HB.WindowConstructionAbridged win)
                    {
                        layers.Add("----------------Outer------------------");
                        layers.AddRange(win.Layers);
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
                    constrLBox.Items.Clear();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        constrLBox.Items.AddRange(allItems);
                        return;
                    }
                    var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                    var filtered = constrs.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
                    var filteredItems = filtered.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                    constrLBox.Items.AddRange(filteredItems);

                };

                var constructionTypes = new DropDown();
                constructionTypes.Items.Add(new ListItem() { Key = "Opaque", Text = "Opaque Construction" });
                constructionTypes.Items.Add(new ListItem() { Key = "Window", Text = "Window Construction" });
                //constructionTypes.Items.Add(new ListItem() { Key = "Shade Construction" });
                //constructionTypes.Items.Add(new ListItem() { Key = "AirBoundary Construction" });
                constructionTypes.SelectedIndex = 0;
                constructionTypes.SelectedIndexChanged += (sender, e) =>
                {
                    var selectedType = constructionTypes.SelectedKey;

                    if (selectedType == "Window")
                    {
                        constrs = WindowConstructions;
                    }
                    else
                    {
                        constrs = OpaqueConstructions;
                    }
                    searchTBox.Text = null;
                    constrLBox.Items.Clear();

                    var filteredItems = constrs.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                    constrLBox.Items.AddRange(filteredItems);

                };

                Func<HB.Energy.IConstruction> getSelected = () => 
                {
                    if (constrLBox.SelectedIndex == -1)
                        return null;
                    return (constrLBox.Items[constrLBox.SelectedIndex] as ListItem).Tag as HB.Energy.IConstruction; 
                }; 

                //WallConstructionSetAbridged
                cSet.WallSet = cSet.WallSet ?? new WallConstructionSetAbridged();
                var setWallSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setWallSetActions.Add(("Exterior", cSet.WallSet.ExteriorConstruction, (cons) => cSet.WallSet.ExteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setWallSetActions.Add(("Ground", cSet.WallSet.GroundConstruction, (cons) => cSet.WallSet.GroundConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setWallSetActions.Add(("Interior", cSet.WallSet.InteriorConstruction, (cons) => cSet.WallSet.InteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                var wallGroup = GenPanelOpaqueConstrSet("Wall Construction Set", getSelected, setWallSetActions);

                //FloorConstructionSetAbridged
                cSet.FloorSet = cSet.FloorSet ?? new FloorConstructionSetAbridged();
                var setfloorSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setfloorSetActions.Add(("Exterior", cSet.FloorSet.ExteriorConstruction, (cons) => cSet.FloorSet.ExteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setfloorSetActions.Add(("Ground", cSet.FloorSet.GroundConstruction, (cons) => cSet.FloorSet.GroundConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setfloorSetActions.Add(("Interior", cSet.FloorSet.InteriorConstruction, (cons) => cSet.FloorSet.InteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                var floorGroup = GenPanelOpaqueConstrSet("Floor Construction Set", getSelected, setfloorSetActions);

                //RoofCeilingConstructionSetAbridged
                cSet.RoofCeilingSet = cSet.RoofCeilingSet ?? new RoofCeilingConstructionSetAbridged();
                var setRoofSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setRoofSetActions.Add(("Exterior", cSet.RoofCeilingSet.ExteriorConstruction, (cons) => cSet.RoofCeilingSet.ExteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setRoofSetActions.Add(("Ground", cSet.RoofCeilingSet.GroundConstruction, (cons) => cSet.RoofCeilingSet.GroundConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                setRoofSetActions.Add(("Interior", cSet.RoofCeilingSet.InteriorConstruction, (cons) => cSet.RoofCeilingSet.InteriorConstruction = cons?.Identifier, typeof(HB.OpaqueConstructionAbridged)));
                var roofCeilingGroup = GenPanelOpaqueConstrSet("Roof/Ceiling Construction Set", getSelected, setRoofSetActions);

                //ApertureConstructionSetAbridged
                cSet.ApertureSet = cSet.ApertureSet ?? new ApertureConstructionSetAbridged();
                var setApertureSetActions = new List<(string, string, Action<HB.Energy.IConstruction>, Type)>() { };
                setApertureSetActions.Add(("Exterior", cSet.ApertureSet.WindowConstruction, (cons) => cSet.ApertureSet.WindowConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setApertureSetActions.Add(("Operable", cSet.ApertureSet.OperableConstruction, (cons) => cSet.ApertureSet.OperableConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setApertureSetActions.Add(("Skylight", cSet.ApertureSet.SkylightConstruction, (cons) => cSet.ApertureSet.SkylightConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                setApertureSetActions.Add(("Interior", cSet.ApertureSet.InteriorConstruction, (cons) => cSet.ApertureSet.InteriorConstruction = cons?.Identifier, typeof(HB.WindowConstructionAbridged)));
                var apertureGroup = GenPanelOpaqueConstrSet("Aperture Construction Set", getSelected, setApertureSetActions);

                //DoorConstructionSetAbridged
                cSet.DoorSet = cSet.DoorSet ?? new DoorConstructionSetAbridged();
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
                var panelNames = new DynamicLayout();
                panelNames.Padding = new Padding(10, 5, 15, 5);
                panelNames.Spacing = new Size(5, 5);
                panelLeft.BeginScrollable(BorderType.None);
                panelLeft.Height = 600;
                var nameTbx = new TextBox();
                cSet.DisplayName = cSet.DisplayName ?? $"New ConstructionSet {cSet.Identifier.Substring(0, 5)}";
                nameTbx.TextBinding.Bind(cSet, c => c.DisplayName);
                panelNames.AddRow(new Label() { Text = "ID: ", Width = 75 }, new Label() { Text = cSet.Identifier, Enabled = false });
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
                panelRight.AddRow(constrLBox);
                panelRight.AddRow(constructionLayersLBox);

                var panelAll = new DynamicLayout() { };
                panelAll.AddRow(panelLeft, panelRight);


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close(cSet);

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var hbData = new Button { Text = "HBData" };
                hbData.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, cSet.ToJson(), "Honeybee Data");

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


        Control GenDropInArea(string currentValue, Action<HB.Energy.IConstruction> setAction, Type setType)
        {
            //var width = 300;
            //var height = 60;

            var layerPanel = new PixelLayout();
            var dropInValue = new TextBox() { PlaceholderText = "Drag from library" };
            dropInValue.Width = 300;
            dropInValue.Enabled = false;
            dropInValue.Text = currentValue;

          
            var backGround = Color.FromArgb(230, 230, 230);
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
                dropInValue.BackgroundColor = Colors.White;
            };
            dropIn.DragOver += (sender, e) =>
            {
                e.Effects = DragEffects.Move;
                dropInValue.BackgroundColor = Colors.Yellow;
            };
            dropIn.DragDrop += (sender, e) =>
            {
                // Get drop-in object
                var value = e.Data.GetObject("HBObj");
                var newValue = setType.GetMethod("FromJson").Invoke(null, new object[] { value }) as HB.Energy.IConstruction;


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
