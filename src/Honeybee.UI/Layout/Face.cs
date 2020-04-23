using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using HoneybeeSchema;
using System.Collections.Generic;
using System;

namespace Honeybee.UI
{
    public static partial class PanelHelper
    {
        /// <summary>
        /// Create Eto panel based on Honeybee geomerty. 
        /// If input HoneybeeObj is a duplicated object, geometryReset action must be provided, 
        /// otherwise no changes would be applied to original honeybee object.
        /// </summary>
        /// <param name="HoneybeeObj"></param>
        /// <param name="geometryReset"></param>
        /// <returns></returns>
        public static Panel GenFacePanel(Face HoneybeeObj, System.Action<string> geometryReset = default)
        {

            var face = HoneybeeObj;
            var objID = face.Identifier;
            geometryReset = geometryReset ?? delegate (string m) { }; //Do nothing if geometryReset is null
            var layout = new DynamicLayout { };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);


            layout.AddSeparateRow(new Label { Text = $"ID: {face.Identifier}" });
            
            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTBox = new TextBox() { };
            face.DisplayName = face.DisplayName ?? string.Empty;
            nameTBox.TextBinding.Bind(face, m => m.DisplayName );
            nameTBox.LostFocus += (s, e) => { geometryReset($"Set Face Name: {face.DisplayName}"); };
            layout.AddSeparateRow(nameTBox);

            
            layout.AddSeparateRow(new Label { Text = "Face Type:" });
            var faceTypeDP = new EnumDropDown<Face.FaceTypeEnum>();
            faceTypeDP.SelectedValueBinding.Bind(Binding.Delegate(() => face.FaceType, v => face.FaceType = v));
            faceTypeDP.LostFocus += (s, e) => { geometryReset($"Set Face Type: {face.FaceType}"); };
            layout.AddSeparateRow(faceTypeDP);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = face.Properties.Energy ?? new FaceEnergyPropertiesAbridged();
                energyProp = FaceEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_FaceEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    face.Properties.Energy = dialog_rc;
                    geometryReset($"Set Face Energy Properties");
                }
            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            bcBtn.Enabled = face.BoundaryCondition.Obj is Outdoors;
            bcBtn.Click += (s, e) => 
            {
                if (face.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        face.BoundaryCondition = dialog_rc;
                        geometryReset($"Set Face Boundary Condition");
                    }
                }
                //Dialogs.ShowMessage("Work in progress", "Honeybee");
            }; 
       

            var bcs = new List<AnyOf<Ground, Outdoors, Adiabatic, Surface>>() { new Ground(), new Outdoors(), new Adiabatic(), new Surface(new List<string>()) };
            var bcDP = DialogHelper.MakeDropDownForAnyOf(face.BoundaryCondition.Obj.GetType().Name, v => face.BoundaryCondition = v, bcs);
            bcDP.SelectedIndexChanged += (s, e) =>
            {
                bcBtn.Enabled = false;
                if (bcDP.SelectedKey == nameof(Outdoors))
                    bcBtn.Enabled = true;

            };

            layout.AddSeparateRow(bcDP);
            layout.AddSeparateRow(bcBtn);


            var apertureLBox = new ListBox();
            apertureLBox.Height = 100;
            var apertures = face.Apertures ?? new List<Aperture>();
            var faceCount = 0;
            if (apertures.Any())
            {
                var validApertures = apertures;
                faceCount = validApertures.Count();
                //Check if displace name is null, in case this hb object is not created from rhino. 
                var faceItems = validApertures.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                apertureLBox.Items.AddRange(faceItems);
               
            }
            layout.AddSeparateRow(new Label { Text = $"Apertures: (total: {faceCount})" });
            layout.AddSeparateRow(apertureLBox);


            var doorLBox = new ListBox();
            doorLBox.Height = 50;
            var doors = face.Doors ?? new List<Door>();
            if (doors.Any())
            {
                //Check if displace name is null, in case this hb object is not created from rhino. 
                var faceItems = doors.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                doorLBox.Items.AddRange(faceItems);
            }
            layout.AddSeparateRow(new Label { Text = $"Doors:" });
            layout.AddSeparateRow(doorLBox);

            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.Height = 50;
            var inShds = face.IndoorShades;
            if (inShds != null)
            {
                var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                inShadesListBox.Items.AddRange(idShds);
            }
            layout.AddSeparateRow(inShadesListBox);

            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            var outShds = face.OutdoorShades;
            if (outShds != null)
            {
                var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
                outShadesListBox.Items.AddRange(outShdItems);
            }
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(face.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }

        private static Panel _facePanel;
        public static Panel UpdateFacePanel(Face HoneybeeObj, Action<string> geometryReset = default)
        {
            var vm = FaceViewModel.Instance;
            vm.Update(HoneybeeObj, geometryReset);
            if (_facePanel == null)
            {
                _facePanel = GenFacePanel2();
            }
            return _facePanel;
        }

        //private static Panel GenFacePanel2()
        //{

        //    var layout = new DynamicLayout { };
        //    layout.MinimumSize = new Size(100, 200);
        //    layout.Spacing = new Size(5, 5);
        //    layout.Padding = new Padding(10);
        //    layout.DefaultSpacing = new Size(2, 2);

        //    var id = new Label();
        //    id.TextBinding.BindDataContext((FaceViewModel m) => m.Identifier);
        //    layout.AddSeparateRow(id);

        //    layout.AddSeparateRow(new Label { Text = "face Name:" });
        //    var nameTBox = new TextBox() { };
        //    nameTBox.TextBinding.BindDataContext((FaceViewModel m) => m.DisplayName);
        //    layout.AddSeparateRow(nameTBox);


        //    layout.AddSeparateRow(new Label { Text = "Face Type:" });
        //    var faceTypeDP = new EnumDropDown<Face.FaceTypeEnum>();
        //    layout.AddSeparateRow(faceTypeDP);


        //    layout.AddSeparateRow(new Label { Text = "Properties:" });
        //    var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
        //    faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
        //    layout.AddSeparateRow(faceRadPropBtn);
        //    var faceEngPropBtn = new Button { Text = "Energy Properties" };
        //    layout.AddSeparateRow(faceEngPropBtn);


        //    layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
        //    var bcBtn = new Button { Text = "Edit Boundary Condition" };


        //    var apertureLBox = new ListBox();
        //    apertureLBox.Height = 100;

        //    layout.AddSeparateRow(apertureLBox);


        //    var doorLBox = new ListBox();
        //    doorLBox.Height = 50;
        //    layout.AddSeparateRow(new Label { Text = $"Doors:" });
        //    layout.AddSeparateRow(doorLBox);

        //    layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
        //    var inShadesListBox = new ListBox();
        //    inShadesListBox.Height = 50;
        //    layout.AddSeparateRow(inShadesListBox);

        //    layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
        //    var outShadesListBox = new ListBox();
        //    outShadesListBox.Height = 50;
        //    layout.AddSeparateRow(outShadesListBox);


        //    layout.Add(null);
        //    layout.DataContext = FaceViewModel.Instance;

        //    return layout;


        //}
        public static Panel GenFacePanel2()
        {
            var vm = FaceViewModel.Instance;
            //var face = vm.HoneybeeObject;
            //var objID = face.Identifier;

            var layout = new DynamicLayout { DataContext = FaceViewModel.Instance };
            layout.MinimumSize = new Size(100, 200);
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var id = new Label();
            id.TextBinding.BindDataContext((FaceViewModel m) => m.HoneybeeObject.Identifier);
            layout.AddSeparateRow(new Label { Text = "ID: " }, id);

            layout.AddSeparateRow(new Label { Text = "Name:" });
            var nameTB = new TextBox() { };
            nameTB.TextBinding.BindDataContext((FaceViewModel m) => m.HoneybeeObject.DisplayName);
            nameTB.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Face Name {vm.HoneybeeObject.DisplayName}"); };
            layout.AddSeparateRow(nameTB);


            layout.AddSeparateRow(new Label { Text = "Face Type:" });
            var faceTypeDP = new EnumDropDown<Face.FaceTypeEnum>();
            //faceTypeDP.BindDataContext(c=>c.DataStore, (FaceViewModel m) => Enum.GetNames( typeof(Face.FaceTypeEnum)));
            //faceTypeDP.SelectedValueBinding.BindDataContext(Binding.Delegate<FaceViewModel, Face.FaceTypeEnum>((m) => m.HoneybeeObject.FaceType, (m, v) => m.HoneybeeObject.FaceType = v));
            faceTypeDP.SelectedValueBinding.BindDataContext((FaceViewModel m) => m.HoneybeeObject.FaceType);
            faceTypeDP.LostFocus += (s, e) => { vm.ActionWhenChanged($"Set Face Type: {vm.HoneybeeObject.FaceType}"); };
            layout.AddSeparateRow(faceTypeDP);


            layout.AddSeparateRow(new Label { Text = "Properties:" });
            var faceRadPropBtn = new Button { Text = "Radiance Properties (WIP)" };
            faceRadPropBtn.Click += (s, e) => MessageBox.Show("Work in progress", "Honeybee");
            layout.AddSeparateRow(faceRadPropBtn);
            var faceEngPropBtn = new Button { Text = "Energy Properties" };
            faceEngPropBtn.Click += (s, e) =>
            {
                var energyProp = vm.HoneybeeObject.Properties.Energy ?? new FaceEnergyPropertiesAbridged();
                energyProp = FaceEnergyPropertiesAbridged.FromJson(energyProp.ToJson());
                var dialog = new Dialog_FaceEnergyProperty(energyProp);
                var dialog_rc = dialog.ShowModal();
                if (dialog_rc != null)
                {
                    vm.HoneybeeObject.Properties.Energy = dialog_rc;
                    vm.ActionWhenChanged($"Set Face Energy Properties");
                }
            };
            layout.AddSeparateRow(faceEngPropBtn);


            layout.AddSeparateRow(new Label { Text = "Boundary Condition:" });
            var bcBtn = new Button { Text = "Edit Boundary Condition" };
            //bcBtn.BindDataContext(Binding.Property<bool>(c =>c.Enabled));
            bcBtn.BindDataContext(c => c.Enabled, (FaceViewModel m) => m.IsOutdoor, DualBindingMode.OneWay);
            //bcBtn.
            //bcBtn.Enabled = FaceViewModel.Instance.HoneybeeObject.BoundaryCondition.Obj is Outdoors;
            bcBtn.Click += (s, e) =>
            {
                if (vm.HoneybeeObject.BoundaryCondition.Obj is Outdoors outdoors)
                {
                    //MessageBox.Show("Work in progress", "Honeybee");
                    var od = Outdoors.FromJson(outdoors.ToJson());
                    var dialog = new UI.Dialog_BoundaryCondition_Outdoors(od);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        vm.HoneybeeObject.BoundaryCondition = dialog_rc;
                        vm.ActionWhenChanged($"Set Face Boundary Condition");
                    }
                }
                else
                {
                    MessageBox.Show("Only Outdoors type has additional properties to edit!");
                }
                //Dialogs.ShowMessage("Work in progress", "Honeybee");
            };


            var bcs = new List<AnyOf<Ground, Outdoors, Adiabatic, Surface>>() { new Ground(), new Outdoors(), new Adiabatic(), new Surface(new List<string>()) };
            //var bcsNames = new List<string>() { nameof(Ground), nameof(Outdoors), nameof(Adiabatic), nameof(Surface)};
            //var dropdownItems = bcs.Select(_ => new ListItem() { Text = _.Obj.GetType().Name, Tag = _ }).ToList();
            var bcDP = new DropDown();
            //bcDP.Items.AddRange(dropdownItems);

            //var bcDP = DialogHelper.MakeDropDownForAnyOf(vm.HoneybeeObject.BoundaryCondition.Obj.GetType().Name, v => vm.HoneybeeObject.BoundaryCondition = v, bcs);
            //bcDP.SelectedIndexBinding.BindDataContext(
            //    Binding.Delegate<FaceViewModel, int>( 
            //        m => bcs.FindIndex(_ => _.Obj.GetType().Name == m.HoneybeeObject.BoundaryCondition.Obj.GetType().Name), 
            //        (f,v)=> f.HoneybeeObject.BoundaryCondition = bcs[v])
            //   );

            bcDP.BindDataContext(
                c => c.DataStore, (FaceViewModel m) => m.Bcs
                );
            bcDP.ItemTextBinding = Binding.Delegate<AnyOf<Ground, Outdoors, Adiabatic, Surface>, string>(m => m.Obj.GetType().Name);
            bcDP.SelectedIndexBinding.BindDataContext((FaceViewModel m) => m.SelectedIndex);
            //bcDP.SelectedValueBinding.BindDataContext((FaceViewModel m) => m.BC);

            //bcDP.SelectedValueBinding.BindDataContext(
            //    (FaceViewModel m) => m.BC);
            //bcDP.SelectedIndexBinding.BindDataContext(
            //    Binding.Delegate< AnyOf<Ground, Outdoors, Adiabatic, Surface>, int >(
            //    (anyOfObj) => bcs.FindIndex(_ => _.Obj.GetType().Name == anyOfObj.Obj.GetType().Name),
            //    (anyOfObj, i) => vm.HoneybeeObject.BoundaryCondition = anyOfObj)
            //    );
            //bcDP.SelectedValueBinding.BindDataContext(
            //    Binding.Delegate<AnyOf<Ground, Outdoors, Adiabatic, Surface>, string>((v) => v.Obj.GetType().Name, (v, m) => vm.HoneybeeObject.BoundaryCondition = v ));

            //bcDP.SelectedValueBinding.BindDataContext((FaceViewModel m) => m.IsOutdoor);
            //bcDP.SelectedIndexChanged += (s, e) =>
            //{
            //    bcBtn.Enabled = false;
            //    if (bcDP.SelectedKey == nameof(Outdoors))
            //        bcBtn.Enabled = true;
                
            //};

            layout.AddSeparateRow(bcDP);
            layout.AddSeparateRow(bcBtn);


            var aptCount = new Label();
            aptCount.TextBinding.BindDataContext((FaceViewModel m) => m.ApertureCount);
            layout.AddSeparateRow(new Label { Text = $"Apertures:" }, null, new Label { Text = $"Total: " }, aptCount);
            var apertureLBox = new ListBox();
            apertureLBox.Height = 100;
            apertureLBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.Apertures);
            apertureLBox.ItemTextBinding = Binding.Delegate<Aperture, string>(m => m.DisplayName ?? m.Identifier);

            //var apertures = vm.HoneybeeObject.Apertures ?? new List<Aperture>();
            //var faceCount = 0;
            //if (apertures.Any())
            //{
            //    var validApertures = apertures;
            //    faceCount = validApertures.Count();
            //    //Check if displace name is null, in case this hb object is not created from rhino. 
            //    var faceItems = validApertures.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
            //    apertureLBox.Items.AddRange(faceItems);

            //}
           
            layout.AddSeparateRow(apertureLBox);


            layout.AddSeparateRow(new Label { Text = $"Doors:" });
            var doorLBox = new ListBox();
            doorLBox.Height = 50;
            doorLBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.Doors);
            doorLBox.ItemTextBinding = Binding.Delegate<Door, string>(m => m.DisplayName ?? m.Identifier);

            //var doors = vm.HoneybeeObject.Doors ?? new List<Door>();
            //if (doors.Any())
            //{
            //    //Check if displace name is null, in case this hb object is not created from rhino. 
            //    var faceItems = doors.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
            //    doorLBox.Items.AddRange(faceItems);
            //}
            layout.AddSeparateRow(doorLBox);

            layout.AddSeparateRow(new Label { Text = "IndoorShades:" });
            var inShadesListBox = new ListBox();
            inShadesListBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.IndoorShades);
            inShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            inShadesListBox.Height = 50;
            //var inShds = vm.HoneybeeObject.IndoorShades;
            //if (inShds != null)
            //{
            //    var idShds = inShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
            //    inShadesListBox.Items.AddRange(idShds);
            //}
            layout.AddSeparateRow(inShadesListBox);

            layout.AddSeparateRow(new Label { Text = "OutdoorShades:" });
            var outShadesListBox = new ListBox();
            outShadesListBox.Height = 50;
            outShadesListBox.BindDataContext(c => c.DataStore, (FaceViewModel m) => m.HoneybeeObject.OutdoorShades);
            outShadesListBox.ItemTextBinding = Binding.Delegate<Shade, string>(m => m.DisplayName ?? m.Identifier);
            //var outShds = vm.HoneybeeObject.OutdoorShades;
            //if (outShds != null)
            //{
            //    var outShdItems = outShds.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Tag = _ });
            //    outShadesListBox.Items.AddRange(outShdItems);
            //}
            layout.AddSeparateRow(outShadesListBox);


            layout.Add(null);
            var data_button = new Button { Text = "Honeybee Data" };
            data_button.Click += (sender, e) => MessageBox.Show(vm.HoneybeeObject.ToJson(), "Honeybee Data");
            layout.AddSeparateRow(data_button, null);

            return layout;

        }
    }
}
