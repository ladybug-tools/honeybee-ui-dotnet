using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System;

namespace Honeybee.UI
{
    public class Dialog_ModifierSet_Interior : Dialog<InteriorSet>
    {

        private ModifierSetViewModel_Interior _vm;
        public Dialog_ModifierSet_Interior(ref HB.ModelRadianceProperties libSource, InteriorSet ModifierSet, bool lockedMode = false)
        {
            try
            {
                _vm = new ModifierSetViewModel_Interior(this, ref libSource, ModifierSet);

                Padding = new Padding(5);
                Resizable = true;
                Title = $"Interior Modifier Set - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                Width = 450;
                this.Icon = DialogHelper.HoneybeeIcon;


                var interiorGroup = GenInteriorSetPanel(lockedMode);
                
                //Left panel
                var panelLeft = new DynamicLayout();
                panelLeft.DefaultSpacing = new Size(0, 5);
            
                panelLeft.AddRow(interiorGroup);
        

                var locked = new CheckBox() { Text = "Locked", Enabled = false };
                locked.Checked = lockedMode;

                var OkButton = new Button { Text = "OK" , Enabled = !lockedMode };
                OkButton.Click += (sender, e) => 
                {
                    var obj = _vm.GetHBObject();
                    this.Close(obj);
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(locked, null, OkButton, this.AbortButton, null) }
                };


                //Create layout
                Content = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                {
                    panelLeft,
                    new TableRow(buttons),
                    null
                }
                };

               
            }
            catch (Exception e)
            {
                Dialog_Message.ShowFullMessage(e.ToString());
                //throw e;
            }

        }

   
        private GroupBox GenInteriorSetPanel(bool lockedMode)
        {
            var vm = this._vm;

            var layout = new DynamicLayout() { Enabled = !lockedMode};

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

        
            var itr = new Button();
            itr.Command = vm.WallIntSet.SelectModifierCommand;
            itr.TextBinding.Bind(vm, _ => _.WallIntSet.BtnName);
            itr.Bind(_ => _.Enabled, vm, _ => _.WallIntSet.IsBtnEnabled);
            var itrByGlobal = new CheckBox() { Text = ReservedText.NoChange };
            itrByGlobal.CheckedBinding.Bind(vm, _ => _.WallIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Wall", null, itrByGlobal);
            layout.AddRow(itr);

            var ceiling = new Button();
            ceiling.Command = vm.RoofIntSet.SelectModifierCommand;
            ceiling.TextBinding.Bind(vm, _ => _.RoofIntSet.BtnName);
            ceiling.Bind(_ => _.Enabled, vm, _ => _.RoofIntSet.IsBtnEnabled);
            var clnByGlobal = new CheckBox() { Text = ReservedText.NoChange };
            clnByGlobal.CheckedBinding.Bind(vm, _ => _.RoofIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Ceiling", null, clnByGlobal);
            layout.AddRow(ceiling);

            var intFloor = new Button();
            intFloor.Command = vm.FloorIntSet.SelectModifierCommand;
            intFloor.TextBinding.Bind(vm, _ => _.FloorIntSet.BtnName);
            intFloor.Bind(_ => _.Enabled, vm, _ => _.FloorIntSet.IsBtnEnabled);
            var itrFlrByGlobal = new CheckBox() { Text = ReservedText.NoChange };
            itrFlrByGlobal.CheckedBinding.Bind(vm, _ => _.FloorIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Floor", null, itrFlrByGlobal);
            layout.AddRow(intFloor);

            var apt = new Button();
            apt.Command = vm.ApertureIntSet.SelectModifierCommand;
            apt.TextBinding.Bind(vm, _ => _.ApertureIntSet.BtnName);
            apt.Bind(_ => _.Enabled, vm, _ => _.ApertureIntSet.IsBtnEnabled);
            var aptByGlobal = new CheckBox() { Text = ReservedText.NoChange };
            aptByGlobal.CheckedBinding.Bind(vm, _ => _.ApertureIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Window", null, aptByGlobal);
            layout.AddRow(apt);


            var door = new Button();
            door.Command = vm.DoorIntSet.SelectModifierCommand;
            door.TextBinding.Bind(vm, _ => _.DoorIntSet.BtnName);
            door.Bind(_ => _.Enabled, vm, _ => _.DoorIntSet.IsBtnEnabled);
            var doorByGlobal = new CheckBox() { Text = ReservedText.NoChange };
            doorByGlobal.CheckedBinding.Bind(vm, _ => _.DoorIntSet.IsCheckboxChecked);
            layout.AddSeparateRow("Door", null, doorByGlobal);
            layout.AddRow(door);


            var doorGlass = new Button();
            doorGlass.Command = vm.DoorIntGlassSet.SelectModifierCommand;
            doorGlass.TextBinding.Bind(vm, _ => _.DoorIntGlassSet.BtnName);
            doorGlass.Bind(_ => _.Enabled, vm, _ => _.DoorIntGlassSet.IsBtnEnabled);
            var doorGlassByGlobal = new CheckBox() { Text = ReservedText.NoChange };
            doorGlassByGlobal.CheckedBinding.Bind(vm.DoorIntGlassSet, _ => _.IsCheckboxChecked);
            layout.AddSeparateRow("Glass Door", null, doorGlassByGlobal);
            layout.AddRow(doorGlass);

            layout.AddRow(null);

            var gp = new GroupBox() { Text = "Interior Modifier Set" };
            gp.Content = layout;

            return gp;
        }

 
  
       

    }
}
