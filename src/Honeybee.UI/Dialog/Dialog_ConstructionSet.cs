using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using HoneybeeSchema;
using System.Text.RegularExpressions;

namespace Honeybee.UI
{
    public class dropArea: Eto.Forms.Control
    {

    }
    public class Dialog_ConstructionSet: Dialog<HB.ConstructionSetAbridged>
    {

        //private HB.ConstructionSetAbridged _constructionSet { get; set; }

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

                IEnumerable<HB.Energy.IConstruction> constrs = EnergyLibrary.StandardsOpaqueConstructions;

                // Construction List
                var constrLBox = new ListBox();
                constrLBox.Height = 450;
                constrLBox.Width = 300;

                // Construction Layers
                var constructionLayersLBox = new ListBox();
                constructionLayersLBox.Height = 150;
                constructionLayersLBox.Items.Add(new ListItem() { Text = "Construction Details" });

                //HB.OpaqueConstructionAbridged selectedConstr = null;
                var allItems = constrs.Select(_ => new ListItem() { Text = _.Identifier, Key = _.Identifier, Tag = _ });
                constrLBox.Items.AddRange(allItems);
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
                        constrs = EnergyLibrary.StandardsWindowConstructions;
                    }
                    else
                    {
                        constrs = EnergyLibrary.StandardsOpaqueConstructions;
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
                var inWall = GenTextBoxWithBtn(item.currentValue, getSelected, item.setAction, item.setType);
                wallLayout.AddRow(new Label() { Text = item.label, Width = 75 }, inWall, null);
            }
           
            //var exWall = GenTextBoxWithBtn(getSelected, (cons) => c.ExteriorConstruction = cons.Identifier);
            //var gWall =  GenTextBoxWithBtn(getSelected, (cons) => c.GroundConstruction = cons.Identifier);

            //wallLayout.AddRow(new Label() { Text = "Exterior", Width = 75 }, exWall, null);
            //wallLayout.AddRow(new Label() { Text = "Ground", Width = 75 }, gWall, null);
            wallGroup.Content = wallLayout;
            return wallGroup;

        }

        

        private GroupBox GenPanelFloorSet()
        {
            //Wall Construction Set
            var wallGroup = new GroupBox() { Text = "Floor Construction Set" };
            var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };
            var inWall = new TextBox() { PlaceholderText = "By Global" };
            inWall.Width = 300;
            var inWallBtn = new Button() { Text = "+", Width = 30 };
            inWallBtn.Click += (sender, e) =>
            {
                var txt = inWallBtn.Text;
                inWall.Text = txt == "+" ? "A New Construction" : null;
                inWallBtn.Text = txt == "+" ? "-" : "+";
            };

            var exWall = new TextBox() { PlaceholderText = "By Global" };
            var exWallBtn = new Button() { Text = "+", Width = 30 };
            exWallBtn.Click += (sender, e) =>
            {
                var txt = exWallBtn.Text;
                exWall.Text = txt == "+" ? "A New Construction" : null;
                exWallBtn.Text = txt == "+" ? "-" : "+";
            };

            var gWall = new TextBox() { PlaceholderText = "By Global" };
            var gWallBtn = new Button() { Text = "+", Width = 30 };
            gWallBtn.Click += (sender, e) =>
            {
                var txt = gWallBtn.Text;
                gWall.Text = txt == "+" ? "A New Construction" : null;
                gWallBtn.Text = txt == "+" ? "-" : "+";
            };

            wallLayout.AddRow(new Label() { Text = "Interior", Width = 50 }, inWall, inWallBtn, null);
            wallLayout.AddRow(new Label() { Text = "Exterior", Width = 50 }, exWall, exWallBtn, null);
            wallLayout.AddRow(new Label() { Text = "Ground", Width = 50 }, gWall, gWallBtn, null);
            wallGroup.Content = wallLayout;
            return wallGroup;

        }

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
