using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using Honeybee.UI.ViewModel;
using System.Collections.Generic;

namespace Honeybee.UI.View
{

    public class RoomPropertyDialog : Dialog<List<HB.Room>>
    {
        public RoomPropertyDialog(HB.ModelProperties libSource, List<HB.Room> rooms)
        {
            var p = new DynamicLayout();
            p.DefaultSpacing = new Size(4, 4);
            p.DefaultPadding = new Padding(4);

            p.AddRow(RoomProperty.Instance);
            RoomProperty.Instance.UpdatePanel(libSource, rooms);

            var OKButton = new Button() { Text = "OK" };
            OKButton.Click += (s, e) =>
            {
                this.Close(RoomProperty.Instance.GetRooms());
            };

            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();

            p.AddSeparateRow(null, OKButton, this.AbortButton, null);
            this.Content = p;
        }
    }

    public class RoomProperty : Panel
    {
        private RoomPropertyViewModel _vm { get; set; }
        private static RoomProperty _instance;
        public static RoomProperty Instance
        {
            get
            {
                _instance = _instance ?? new RoomProperty();
                return _instance;
            }
        }

        public RoomProperty()
        {
            this._vm = new RoomPropertyViewModel(this);
            Initialize();
        }


        public void UpdatePanel(HB.ModelProperties libSource, List<HB.Room> HoneybeeObjs)
        {
            this._vm.Update(libSource, HoneybeeObjs);
        }

        public List<HB.Room> GetRooms()
        {
            return this._vm.GetRooms();
        }

        private void Initialize()
        {
            var vm = this._vm;
            var layout = new DynamicLayout();
            layout.Width = 400;

            layout.DefaultSpacing = new Size(4, 4);
            layout.DefaultPadding = new Padding(4);

       
  
            var id = new Label();
            id.Width = 250;
            id.TextBinding.Bind(vm, (_) => _.Identifier);
            layout.AddRow("ID: ", id);

            var nameTB = new TextBox() { };
            nameTB.TextBinding.Bind(vm, (_) => _.DisplayName);
            layout.AddRow("Name:", nameTB);

            var storyTB = new TextBox() { };
            storyTB.TextBinding.Bind(vm, (_) => _.Story);
            layout.AddRow("Story:", storyTB);

            var multiplier_NS = new NumericText();
            multiplier_NS.ReservedText = _vm.Varies;
            multiplier_NS.TextBinding.Bind(vm, _ => _.MultiplierText);
            layout.AddRow("Multiplier:", multiplier_NS);


            //Get modifierSet
            var modifierSetDP = new Button();
            modifierSetDP.Width = 200;
            modifierSetDP.Bind((t) => t.Enabled, vm, v => v.IsModifierSetBtnEnabled);
            modifierSetDP.TextBinding.Bind(vm, _ => _.ModifierSetName);
            modifierSetDP.Command = vm.ModifierSetCommand;
            var modifierSetByProgram = new CheckBox() { Text = vm.ByGlobalModifierSet };
            modifierSetByProgram.CheckedBinding.Bind(vm, _ => _.IsByGlobalModifierSet);

            layout.AddRow("Modifier Set:", modifierSetByProgram);
            layout.AddRow(null, modifierSetDP);


            //Get constructions
            var constructionSetDP = new Button();
            constructionSetDP.Width = 200;
            constructionSetDP.Bind(_ => _.Enabled, vm, _ => _.IsCSetBtnEnabled);
            constructionSetDP.TextBinding.Bind(vm, _ => _.ConstructionSetName);
            constructionSetDP.Command = vm.RoomConstructionSetCommand;
            var cSetByProgram = new CheckBox() { Text = vm.ByGlobalConstructionSet };
            cSetByProgram.CheckedBinding.Bind(vm, _ => _.IsByGlobalConstructionSet);

            layout.AddRow("Construction Set:", cSetByProgram );
            layout.AddRow(null, constructionSetDP);


            //Get programs
            var programTypesDP = new Button();
            programTypesDP.Bind((t) => t.Enabled, vm, v => v.IsPTypeBtnEnabled);
            programTypesDP.TextBinding.Bind(vm, _ => _.ProgramTypeName);
            programTypesDP.Command = vm.RoomProgramTypeCommand;
            var pTypeByProgram = new CheckBox() { Text = vm.Unoccupied };
            pTypeByProgram.CheckedBinding.Bind(vm, _ => _.IsUnoccupied);

            layout.AddRow("Program Type:", pTypeByProgram);
            layout.AddRow(null, programTypesDP);


            //Get HVACs
            var hvacDP = new Button();
            hvacDP.Bind((t) => t.Enabled, vm, v => v.IsHVACBtnEnabled);
            hvacDP.TextBinding.Bind(vm, _ => _.HVACName);
            hvacDP.Command = vm.RoomHVACCommand;
            var hvacByProgram = new CheckBox() { Text = vm.Unconditioned };
            hvacByProgram.CheckedBinding.Bind(vm, _ => _.IsUnconditioned);

            layout.AddRow("HVAC:", hvacByProgram);
            layout.AddRow(null, hvacDP);




            layout.Add(null);
            var data_button = new Button { Text = "Schema Data" };
            data_button.Command = this._vm.HBDataBtnClick;
            layout.AddSeparateRow(data_button, null);


            this.Content = layout;
        }
    }


}
