using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Energy;
using HoneybeeSchema.Radiance;
using System;

namespace Honeybee.UI
{
    public class Dialog_SHW : DialogForm<SHWSystem>
    {
        //private ModelEnergyProperties ModelEnergyProperties { get; set; }
        public Dialog_SHW(HoneybeeSchema.SHWSystem shw = default, bool lockedMode = false, Func<string> roomIDPicker = default)
        {
            var sys = shw ?? new SHWSystem($"SHWSystem_{Guid.NewGuid().ToString().Substring(0, 8)}");
            var vm = new SHWViewModel(sys, this);
            vm.SetAmbientCoffConditionRoomPicker(roomIDPicker);

            //Padding = new Padding(4);
            Title = $"Service Hot Water - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            Width = 450;
            this.Icon = DialogHelper.HoneybeeIcon;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(5, 5);
            layout.Padding = new Padding(10);

            var hbType = sys.GetType();
            //string identifier,
            //string displayName = null,
            //object userData = null,
            //SHWEquipmentType equipmentType = SHWEquipmentType.Gas_WaterHeater,
            //AnyOf<double, Autocalculate> heaterEfficiency = null,
            //AnyOf< double, string> ambientCondition = null,
            //double ambientLossCoefficient = 6.0



            // Identifier
            var idPanel = DialogHelper.MakeIDEditor(vm.Identifier, vm, _ => _.Identifier);
            var idLabel = new Label() { Text = "ID: " };
            var idDescription = HoneybeeSchema.SummaryAttribute.GetSummary(hbType, nameof(sys.Identifier));
            idLabel.ToolTip = Utility.NiceDescription(idDescription);


            var nameLabel = new Label() { Text = "Name: " };
            nameLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hbType, nameof(sys.DisplayName)));
            var nameTbx = new TextBox();
            nameTbx.Bind(_ => _.Text, vm, _ => _.Name);


            var panelName = new DynamicLayout();
            panelName.DefaultSpacing = new Size(0, 5);
            panelName.AddRow(idLabel, idPanel);
            panelName.AddRow(nameLabel, nameTbx);


            var eqpTypeLabel = new Label() { Text = "Equipment Type:" };
            eqpTypeLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hbType, nameof(sys.EquipmentType)));

            var heaterEffLabel = new Label() { Text = "Heater Efficiency:" };
            heaterEffLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hbType, nameof(sys.HeaterEfficiency)));


            var ambientLabel = new Label() { Text = "Ambient Condition:" };
            ambientLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hbType, nameof(sys.AmbientCondition)));

            var ambientCoffLabel = new Label() { Text = "Ambient Loss Coefficient [W/K]" };
            ambientCoffLabel.ToolTip = Utility.NiceDescription(HoneybeeSchema.SummaryAttribute.GetSummary(hbType, nameof(sys.AmbientLossCoefficient)));

            var eqpType = new EnumDropDown<SHWEquipmentType>();
            var heaterEffAuto = new RadioButton() { Text = "Autocalculate" };
            var heaterEffNumber = new RadioButton();
            var heaterEff = new NumericStepper() { MaximumDecimalPlaces = 2};

            // ambientCoffCondition
            var ambientLayout = new DynamicLayout();
            ambientLayout.DefaultSpacing = new Size(5, 5);

            var ambientCoffConditionNumber = new RadioButton();
            var ambientCoffConditionRoom = new RadioButton() { Text = "Room ID" };

            var ambientCoffCondition = new DoubleText() { Width = 370 };
            var ambientCoffConditionRoomLayout = new DynamicLayout();
            var ambientCoffConditionRoomID = new TextBox();
            var ambientCoffConditionRoomID_btn = new Button();
            ambientCoffConditionRoomLayout.AddRow(ambientCoffConditionRoomID);
            ambientCoffConditionRoomLayout.AddRow(ambientCoffConditionRoomID_btn);

            var ambientCoffConditionUnit = new Label();
            ambientCoffConditionUnit.TextBinding.Bind(vm, _ => _.AmbientCoffConditionNumber.DisplayUnitAbbreviation);

            ambientLayout.AddSeparateRow(ambientLabel);
            ambientLayout.AddSeparateRow(ambientCoffConditionNumber, ambientCoffCondition, ambientCoffConditionUnit);
            ambientLayout.AddSeparateRow(ambientCoffConditionRoom, ambientCoffConditionRoomLayout);
            ambientLayout.Bind(_ => _.Enabled, vm, _ => _.AmbientCoffConditionEnabled);


            var ambientLossCoefficient = new NumericStepper() {  MaximumDecimalPlaces = 2 };

            eqpType.Bind(_=>_.SelectedValue, vm, _ => _.EquipType);


            heaterEffAuto.Bind(c => c.Checked, vm, _ => _.HeaterEffAuto);
            heaterEffNumber.Bind(c => c.Checked, vm, _ => _.HeaterEffEnabled);
            heaterEff.Bind(c => c.Value, vm, _ => _.HeaterEff);
            heaterEff.Bind(c => c.Enabled, vm, _ => _.HeaterEffEnabled);

            ambientCoffConditionNumber.Bind(c => c.Checked, vm, _ => _.AmbientCoffConditionNumberEnabled);
            ambientCoffConditionRoom.Bind(c => c.Checked, vm, _ => _.AmbientCoffConditionRoomIDEnabled);
            //ambientCoffCondition.Bind(c => c.Value, vm, _ => _.AmbientCoffConditionNumber);
            ambientCoffCondition.Bind(c => c.Enabled, vm, _ => _.AmbientCoffConditionNumberEnabled);
            ambientCoffConditionRoomLayout.Bind(c => c.Enabled, vm, _ => _.AmbientCoffConditionRoomIDEnabled);

            ambientCoffConditionRoomID.Bind(c => c.Text, vm, _ => _.AmbientCoffConditionRoomID);
            ambientCoffConditionRoomID.Bind(_ => _.Visible, vm, _ => _.VisibleAmbientCoffConditionRoomInput);
            ambientCoffConditionRoomID_btn.Bind(_=>_.Text, vm, _ => _.AmbientCoffConditionRoomID);
            ambientCoffConditionRoomID_btn.Bind(_ => _.Visible, vm, _ => _.VisibleAmbientCoffConditionRoomPicker);
            ambientCoffConditionRoomID_btn.Command = vm.AmbientCoffConditionRoomPickerCommand;

            ambientCoffCondition.ReservedText = ReservedText.Varies;
            ambientCoffCondition.SetDefault(22);
            ambientCoffCondition.TextBinding.Bind(vm, _ => _.AmbientCoffConditionNumber.NumberText);
         

            ambientLossCoefficient.Bind(_ => _.Value, vm, _ => _.AmbientLostCoff);

            layout.AddRow(panelName);
            layout.AddRow(eqpTypeLabel);
            layout.AddRow(eqpType);

            layout.AddRow(heaterEffLabel);
            layout.AddRow(heaterEffAuto);
            layout.AddSeparateRow(heaterEffNumber, heaterEff);

            layout.AddRow(ambientLayout);

            layout.AddRow(ambientCoffLabel);
            layout.AddRow(ambientLossCoefficient);

            var locked = new CheckBox() { Text = "Locked", Enabled = false };
            locked.Checked = lockedMode;

            var OKButton = new Button { Text = "OK", Enabled = !lockedMode };
            OKButton.Click += (sender, e) => {
                try
                {
                    OkCommand.Execute(vm.GreateSys(shw));
                }
                catch (Exception er)
                {
                    Dialog_Message.Show(this, er);
                    //throw;
                }
            }; 

            var abortButton = new Button { Text = "Cancel" };
            abortButton.Click += (sender, e) => Close();

            var hbData = new Button { Text = "Data" };
            hbData.Click += (sender, e) => Dialog_Message.Show(this, vm.GreateSys(shw).ToJson(true), "Schema Data");

            layout.AddSeparateRow(locked, null, OKButton, abortButton, null, hbData);
            layout.AddRow(null);
            Content = layout;

        }


    }
}
