using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;

namespace Honeybee.UI
{

    public class Dialog_MatchRoomProperties: Dialog<List<HB.Room>>
    {
        public Dialog_MatchRoomProperties(HB.Room sourceRoom, IEnumerable<HB.Room> targetRooms)
        {
            var vm = new MatchRoomPropertiesViewModel(sourceRoom, targetRooms);

            Padding = new Padding(5);
            Title = "Match Room Properties";
            WindowStyle = WindowStyle.Default;
            Width = 300;
            this.Icon = Honeybee.UI.DialogHelper.HoneybeeIcon;
            var layout = new DynamicLayout();

            this.DefaultButton = new Button { Text = "OK" };
            DefaultButton.Click += (sender, e) => Close(vm.GetUpdatedRooms());


            this.AbortButton = new Button { Text = "Close" };
            AbortButton.Click += (sender, e) => Close();


            // all controls
            var allToggle = new CheckBox() { Text = "Select/Unselect All" };
            allToggle.Bind(_ => _.Checked, vm, m => m.All);
            layout.AddRow(allToggle);


            HoneybeeSchema.Room dummy = null;
            // General
            layout.BeginGroup("General");

            var name = new CheckBox() { Text = "Name" };
            name.CheckedBinding.Bind(vm, m => m.Name);
            layout.AddRow(name);

            var story = new CheckBox() { Text = nameof(dummy.Story) };
            story.CheckedBinding.Bind(vm, m => m.Story);
            layout.AddRow(story);

            var multi = new CheckBox() { Text = nameof(dummy.Multiplier) };
            multi.CheckedBinding.Bind(vm, m => m.Multiplier);
            layout.AddRow(multi);

            var mset = new CheckBox() { Text = "Modifier Set" };
            mset.CheckedBinding.Bind(vm, m => m.ModifierSet);
            layout.AddRow(mset);

            var cset = new CheckBox() { Text = "Construction Set" };
            cset.CheckedBinding.Bind(vm, m => m.ConstructionSet);
            layout.AddRow(cset);

            var ptype = new CheckBox() { Text = "Program Type" };
            ptype.CheckedBinding.Bind(vm, m => m.ProgramType);
            layout.AddRow(ptype);

            var hvac = new CheckBox() { Text = "HVAC System" };
            hvac.CheckedBinding.Bind(vm, m => m.HVAC);
            layout.AddRow(hvac);

            var user = new CheckBox() { Text = "User Data" };
            user.CheckedBinding.Bind(vm, m => m.User);
            layout.AddRow(user);
            layout.EndGroup();


            //Loads
            layout.BeginGroup("Loads");

            var ltn = new CheckBox() { Text = nameof(dummy.Properties.Energy.Lighting) };
            ltn.CheckedBinding.Bind(vm, m => m.Lighting);
            layout.AddRow(ltn);

            var ppl = new CheckBox() { Text = nameof(dummy.Properties.Energy.People) };
            ppl.CheckedBinding.Bind(vm, m => m.People);
            layout.AddRow(ppl);

            var elecEqp = new CheckBox() { Text = "Electric Equipment" };
            elecEqp.CheckedBinding.Bind(vm, m => m.ElecEquipment);
            layout.AddRow(elecEqp);

            var gas = new CheckBox() { Text = "Gas Equipment" };
            gas.CheckedBinding.Bind(vm, m => m.GasEquipment);
            layout.AddRow(gas);

            var vent = new CheckBox() { Text = nameof(dummy.Properties.Energy.Ventilation) };
            vent.CheckedBinding.Bind(vm, m => m.Ventilation);
            layout.AddRow(vent);

            var infil = new CheckBox() { Text = nameof(dummy.Properties.Energy.Infiltration) };
            infil.CheckedBinding.Bind(vm, m => m.Infiltration);
            layout.AddRow(infil);

            var spt = new CheckBox() { Text = nameof(dummy.Properties.Energy.Setpoint) };
            spt.CheckedBinding.Bind(vm, m => m.Setpoint);
            layout.AddRow(spt);

            var hotWater = new CheckBox() { Text = "Service Hot Water" };
            hotWater.CheckedBinding.Bind(vm, m => m.ServiceHotWater);
            layout.AddRow(hotWater);

            var masses = new CheckBox() { Text = "Internal Masses" };
            masses.CheckedBinding.Bind(vm, m => m.InternalMasses);
            layout.AddRow(masses);

            layout.EndGroup();


            // Controls
            layout.BeginGroup("Controls");

            var vCtrl = new CheckBox() { Text = "Window Ventilation Control" };
            vCtrl.CheckedBinding.Bind(vm, m => m.VentControl);
            layout.AddRow(vCtrl);

            var dCtrl = new CheckBox() { Text = "Daylighting Control" };
            dCtrl.CheckedBinding.Bind(vm, m => m.DaylightControl);
            layout.AddRow(dCtrl);

            layout.EndGroup();



            //layout.DefaultPadding = new Padding(10);
            layout.DefaultSpacing = new Size(3, 3);
            layout.DefaultPadding = new Padding(5);
    
            layout.AddSeparateRow(null, DefaultButton, AbortButton, null);
            layout.AddRow(null);

            Content = layout;

        }



    }
}
