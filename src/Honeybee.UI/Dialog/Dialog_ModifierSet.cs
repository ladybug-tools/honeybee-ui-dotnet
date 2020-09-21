using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using System.Text.RegularExpressions;

namespace Honeybee.UI
{
   
    public class Dialog_ModifierSet: Dialog<HB.ModifierSetAbridged>
    {

        private static IEnumerable<HB.ModifierBase> _modifiers;

        public IEnumerable<HB.ModifierBase> Modifiers
        {
            get
            {
                var libObjs = HB.Helper.EnergyLibrary.DefaultModelRadianceProperties.Modifiers.OfType<HB.ModifierBase>().ToList();
                var inModelObjs = HB.Helper.EnergyLibrary.InModelRadianceProperties.Modifiers.OfType<HB.ModifierBase>().ToList();

                libObjs.AddRange(inModelObjs);
                libObjs = libObjs.Distinct().ToList();
                _modifiers = libObjs;

                return _modifiers;
            }
        }

        public Dialog_ModifierSet(HB.ModifierSetAbridged modifierSet)
        {
            var mSet = modifierSet ?? new HB.ModifierSetAbridged(identifier: Guid.NewGuid().ToString());

            Padding = new Padding(5);
            Resizable = true;
            Title = "Modifier Set - Honeybee";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(450, 600);
            this.Icon = DialogHelper.HoneybeeIcon;

            IEnumerable<HB.ModifierBase> modifiers = Modifiers;

            // Modifier List
            var lib = new GridView();
            lib.Height = 450;
            lib.Width = 300;

            lib.ShowHeader = false;
            var nameCol = new GridColumn() { DataCell = new TextBoxCell(0) };
            lib.Columns.Add(nameCol);

            // Library items
            lib.DataStore = modifiers;


            var idCell = new TextBoxCell
            {
                Binding = Binding.Delegate<HB.Energy.IIDdEnergyBaseModel, string>(r => r.DisplayName ?? r.Identifier)
            };
            lib.Columns.Add(new GridColumn() { DataCell = idCell });

            // Modifier Layers
            var ModifierDetailsLBox = new ListBox();
            ModifierDetailsLBox.Height = 150;
            ModifierDetailsLBox.Items.Add(new ListItem() { Text = "Modifier Details" });


            lib.MouseMove += (sender, e) =>
            {
                if (e.Buttons != MouseButtons.Primary)
                    return;

                var dragableArea = lib.Bounds;
                dragableArea.Width -= 20;
                dragableArea.Height -= 20;
                var iscontained = e.Location.Y < dragableArea.Height && e.Location.X < dragableArea.Width;
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
                ModifierDetailsLBox.Items.Clear();

                //Check current selected item from library
                var selItem = lib.SelectedItem as HB.HoneybeeObject;
                if (selItem == null)
                    return;

                //Update Preview
                var details = selItem.ToString(true).Split('\n').Select(_ => new ListItem() { Text = _ });
                ModifierDetailsLBox.Items.AddRange(details);

            };


            var searchTBox = new TextBox() { PlaceholderText = "Search" };

            searchTBox.TextChanged += (sender, e) =>
            {
                var input = searchTBox.Text;
                if (string.IsNullOrWhiteSpace(input))
                {
                    lib.DataStore = modifiers;
                    return;
                }
                var regexPatten = ".*" + input.Replace(" ", "(.*)") + ".*";
                var filtered = modifiers.Where(_ => Regex.IsMatch(_.Identifier, regexPatten, RegexOptions.IgnoreCase) || (_.DisplayName != null ? Regex.IsMatch(_.DisplayName, regexPatten, RegexOptions.IgnoreCase) : false));
                lib.DataStore = filtered;

            };


            Func<HB.ModifierBase> getSelected = () =>
            {
                if (lib.SelectedRow == -1)
                    return null;
                return lib.SelectedItem as HB.ModifierBase;
            };

            //WallModifierSetAbridged
            mSet.WallSet = mSet.WallSet ?? new HB.WallModifierSetAbridged();
            var setWallSetActions = new List<(string, string, Action<HB.ModifierBase>)>() { };
            setWallSetActions.Add(("Exterior", mSet.WallSet.ExteriorModifier, (cons) => mSet.WallSet.ExteriorModifier = cons?.Identifier));
            setWallSetActions.Add(("Interior", mSet.WallSet.InteriorModifier, (cons) => mSet.WallSet.InteriorModifier = cons?.Identifier));
            var wallGroup = GenPanelOpaqueConstrSet("Wall Modifier Set", getSelected, setWallSetActions);

            //FloorModifierSetAbridged
            mSet.FloorSet = mSet.FloorSet ?? new HB.FloorModifierSetAbridged();
            var setfloorSetActions = new List<(string, string, Action<HB.ModifierBase>)>() { };
            setfloorSetActions.Add(("Exterior", mSet.FloorSet.ExteriorModifier, (cons) => mSet.FloorSet.ExteriorModifier = cons?.Identifier));
            setfloorSetActions.Add(("Interior", mSet.FloorSet.InteriorModifier, (cons) => mSet.FloorSet.InteriorModifier = cons?.Identifier));
            var floorGroup = GenPanelOpaqueConstrSet("Floor Modifier Set", getSelected, setfloorSetActions);

            //RoofCeilingModifierSetAbridged
            mSet.RoofCeilingSet = mSet.RoofCeilingSet ?? new HB.RoofCeilingModifierSetAbridged();
            var setRoofSetActions = new List<(string, string, Action<HB.ModifierBase>)>() { };
            setRoofSetActions.Add(("Exterior", mSet.RoofCeilingSet.ExteriorModifier, (cons) => mSet.RoofCeilingSet.ExteriorModifier = cons?.Identifier));
            setRoofSetActions.Add(("Interior", mSet.RoofCeilingSet.InteriorModifier, (cons) => mSet.RoofCeilingSet.InteriorModifier = cons?.Identifier));
            var roofCeilingGroup = GenPanelOpaqueConstrSet("Roof/Ceiling Modifier Set", getSelected, setRoofSetActions);

            //ApertureModifierSetAbridged
            mSet.ApertureSet = mSet.ApertureSet ?? new HB.ApertureModifierSetAbridged();
            var setApertureSetActions = new List<(string, string, Action<HB.ModifierBase>)>() { };
            setApertureSetActions.Add(("Exterior", mSet.ApertureSet.WindowModifier, (cons) => mSet.ApertureSet.WindowModifier = cons?.Identifier));
            setApertureSetActions.Add(("Operable", mSet.ApertureSet.OperableModifier, (cons) => mSet.ApertureSet.OperableModifier = cons?.Identifier));
            setApertureSetActions.Add(("Skylight", mSet.ApertureSet.SkylightModifier, (cons) => mSet.ApertureSet.SkylightModifier = cons?.Identifier));
            setApertureSetActions.Add(("Interior", mSet.ApertureSet.InteriorModifier, (cons) => mSet.ApertureSet.InteriorModifier = cons?.Identifier));
            var apertureGroup = GenPanelOpaqueConstrSet("Aperture Modifier Set", getSelected, setApertureSetActions);

            //DoorModifierSetAbridged
            mSet.DoorSet = mSet.DoorSet ?? new HB.DoorModifierSetAbridged();
            var setDoorSetActions = new List<(string, string, Action<HB.ModifierBase>)>() { };
            setDoorSetActions.Add(("Exterior", mSet.DoorSet.ExteriorModifier, (cons) => mSet.DoorSet.ExteriorModifier = cons?.Identifier));
            setDoorSetActions.Add(("Interior", mSet.DoorSet.InteriorModifier, (cons) => mSet.DoorSet.InteriorModifier = cons?.Identifier));
            setDoorSetActions.Add(("Exterior Glass", mSet.DoorSet.ExteriorGlassModifier, (cons) => mSet.DoorSet.ExteriorGlassModifier = cons?.Identifier));
            setDoorSetActions.Add(("Interior Glass", mSet.DoorSet.InteriorGlassModifier, (cons) => mSet.DoorSet.InteriorGlassModifier = cons?.Identifier));
            setDoorSetActions.Add(("Overhead", mSet.DoorSet.OverheadModifier, (cons) => mSet.DoorSet.OverheadModifier = cons?.Identifier));
            var doorGroup = GenPanelOpaqueConstrSet("Door Modifier Set", getSelected, setDoorSetActions);

            //ShadeModifier
            //var shadeSet = cSet.ShadeModifier;
            var setShadeSetActions = new List<(string, string, Action<HB.ModifierBase>)>() { };
            setShadeSetActions.Add(("Exterior", mSet.ShadeSet.ExteriorModifier, (cons) => mSet.ShadeSet.ExteriorModifier = cons?.Identifier));
            setShadeSetActions.Add(("Interior", mSet.ShadeSet.InteriorModifier, (cons) => mSet.ShadeSet.InteriorModifier = cons?.Identifier));
            var shadeGroup = GenPanelOpaqueConstrSet("Shade Modifier", getSelected, setShadeSetActions);

            //AirBoundaryModifier
            var setAirBdActions = new List<(string, string, Action<HB.ModifierBase>)>() { };
            setAirBdActions.Add(("Air Boundary", mSet.AirBoundaryModifier, (cons) => mSet.AirBoundaryModifier = cons?.Identifier));
            var airBdGroup = GenPanelOpaqueConstrSet("Air Boundary Modifier", getSelected, setAirBdActions);


            //Left panel
            var panelLeft = new DynamicLayout();
            panelLeft.DefaultSpacing = new Size(0, 5);
            var panelNames = new DynamicLayout();
            panelNames.Padding = new Padding(10, 5, 15, 5);
            panelNames.Spacing = new Size(5, 5);
            panelLeft.BeginScrollable(BorderType.None);
            panelLeft.Height = 600;
            var nameTbx = new TextBox();
            mSet.DisplayName = mSet.DisplayName ?? $"ModifierSet {mSet.Identifier.Substring(0, 5)}";
            nameTbx.TextBinding.Bind(mSet, c => c.DisplayName);
            panelNames.AddRow(new Label() { Text = "ID: ", Width = 75 }, new Label() { Text = mSet.Identifier, Enabled = false });
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
            panelRight.AddRow(searchTBox);
            panelRight.AddRow(lib);
            panelRight.AddRow(ModifierDetailsLBox);

            var panelAll = new DynamicLayout() { };
            panelAll.AddRow(panelLeft, panelRight);


            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => Close(mSet);

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            var hbData = new Button { Text = "HBData" };
            hbData.Click += (sender, e) => Dialog_Message.Show(Helper.Owner, mSet.ToJson(), "Honeybee Data");

            var buttons = new TableLayout
            {
                Padding = new Padding(5, 10, 5, 5),
                Spacing = new Size(10, 10),
                Rows = { new TableRow(null, OKButton, this.AbortButton, null, hbData) }
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
        private Color _defaultTextBackgroundColor = new TextBox().BackgroundColor;

        Control GenDropInArea(string currentValue, Action<HB.ModifierBase> setAction)
        {
            //var width = 300;
            //var height = 60;

            var layerPanel = new PixelLayout();
            var dropInValue = new TextBox() { PlaceholderText = "Drag from library" };
            dropInValue.Width = 300;
            dropInValue.Enabled = false;
            dropInValue.Text = currentValue;
            dropInValue.Height = 25;
          
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
                var newValue = HB.ModifierBase.FromJson(value);

                deleteBtn.Visible = true;
                dropInValue.Text = newValue.DisplayName?? newValue.Identifier;
                setAction(newValue);

            };

       
            layerPanel.Add(dropInValue, 0, 0);
            layerPanel.Add(dropIn, 0, 0);
            layerPanel.Add(deleteBtn, dropInValue.Width-24, 0);
            return layerPanel;
        }

        private GroupBox GenPanelOpaqueConstrSet(string groupName, Func<HB.ModifierBase> getSelected, IEnumerable<(string label, string currentValue, Action<HB.ModifierBase> setAction)> setActions)
        {
            
            //Wall Modifier Set
            var wallGroup = new GroupBox() { Text = groupName };
            var wallLayout = new DynamicLayout() { Spacing = new Size(3, 3), Padding = new Padding(5) };

            foreach (var item in setActions)
            {
                var inWall = GenDropInArea(item.currentValue, item.setAction);
                wallLayout.AddRow(new Label() { Text = item.label, Width = 75 }, inWall);
            }
           
            wallGroup.Content = wallLayout;
            return wallGroup;

        }

        

    }
}
