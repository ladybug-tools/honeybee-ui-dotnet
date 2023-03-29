using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HoneybeeSchema;

namespace Honeybee.UI
{
    internal class ModifierManagerViewModel : ManagerBaseViewModel<ModifierViewData>
    {
      
        private HB.ModelRadianceProperties _modelEnergyProperties { get; set; }
        private static ManagerItemComparer<ModifierViewData> _viewDataComparer = new ManagerItemComparer<ModifierViewData>();

        public ModifierManagerViewModel(HB.ModelRadianceProperties libSource, Control control = default):base(control)
        {
            _modelEnergyProperties = libSource;

            this._userData = libSource.ModifierList.Select(_ => new ModifierViewData(_)).ToList();
            this._systemData = SystemRadianceLib.ModifierList.Select(_ => new ModifierViewData(_)).ToList();
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();


            ResetDataCollection();

        }

      
        public void UpdateLibSource()
        {
            var newItems = this._userData.Select(_ => _.Modifier);
            this._modelEnergyProperties.Modifiers.Clear();
            this._modelEnergyProperties.AddModifiers(newItems);
        }

        public List<HB.Radiance.IModifier> GetUserItems(bool selectedOnly)
        {

            UpdateLibSource();

            var itemsToReturn = new List<HB.Radiance.IModifier>();

            if (selectedOnly)
            {
                var d = SelectedData;
                if (d == null)
                {
                    MessageBox.Show(_control, "Nothing is selected!");
                    return null;
                }
                else if (!this._userData.Contains(d))
                {
                    // user selected an item from system library, now add it to model EnergyProperties
                    this._modelEnergyProperties.AddModifier(d.Modifier);
                }

                itemsToReturn.Add(d.Modifier);
            }
            else
            {
                itemsToReturn = this._modelEnergyProperties.ModifierList.ToList();
            }
            return itemsToReturn;
        }


        private void AddModifier(HB.Radiance.IModifier newModifier)
        {
            if (newModifier == null) return;
            var newItem = CheckObjName(newModifier);
            this._userData.Insert(0, new ModifierViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
            ResetDataCollection();
        }
        public ICommand AddPlasticCommand => new RelayCommand<HB.Radiance.IModifier>((obj) => {

            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"Plastic_{id}";
            var newModifier = obj as Plastic ?? new Plastic(id, displayName: name);

            var dialog = new Honeybee.UI.Dialog_Modifier<Plastic>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });
        public ICommand AddGlassCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"Glass_{id}";
            var newModifier = new Glass(id, displayName: name);

            var dialog = new Honeybee.UI.Dialog_Modifier<Glass>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });
        public ICommand AddBSDFCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"BSDF_{id}";
            var newModifier = new BSDF(id, "Replace with your BSDF data", displayName: name); 

            var dialog = new Honeybee.UI.Dialog_Modifier<BSDF>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });
        public ICommand AddGlowCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"Glow_{id}";
            var newModifier = new Glow(id, displayName: name); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Glow>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });
        public ICommand AddLightCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"Light_{id}";
            var newModifier = new Light(id,  displayName: name); 

            var dialog = new Honeybee.UI.Dialog_Modifier<Light>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });
        public ICommand AddTransCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"Trans_{id}";
            var newModifier = new Trans(id, displayName: name); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Trans>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });
        public ICommand AddMetalCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"Metal_{id}";
            var newModifier = new Metal(id, displayName: name); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Metal>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });
        public ICommand AddMirrorCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString().Substring(0, 5);
            var name = $"Mirror_{id}";
            var newModifier = new Mirror(id, displayName: name); ;

            var dialog = new Honeybee.UI.Dialog_Modifier<Mirror>(newModifier);
            var dialog_rc = dialog.ShowModal(_control);
            AddModifier(dialog_rc);
        });

        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var modifierDic = new Dictionary<string, ICommand>()
            {
                { "Plastic", AddPlasticCommand},
                { "Glass", AddGlassCommand},
                { "Trans",AddTransCommand },
                { "Metal", AddMetalCommand},
                { "Mirror", AddMirrorCommand},
                { "Glow",AddGlowCommand },
                { "Light", AddLightCommand},
                { "BSDF", AddBSDFCommand}
            };

            var contextMenu = new ContextMenu();

            foreach (var item in modifierDic)
            {
                contextMenu.Items.Add(
                  new Eto.Forms.ButtonMenuItem()
                  {
                      Text = item.Key,
                      Command = item.Value
                  });
            }
            contextMenu.Show();
        });


        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to duplicate!");
                return;
            }


            var dup = selected.Modifier.Duplicate() as HB.Radiance.IModifier;
            var name = $"{dup.DisplayName ?? dup.Identifier}_dup";
            dup.Identifier = Guid.NewGuid().ToString().Substring(0, 5);
            dup.DisplayName = name;
            switch (dup)
            {
                case Plastic obj:
                    AddPlasticCommand.Execute(obj);
                    break;
                case Glass obj:
                    AddGlassCommand.Execute(obj);
                    break;
                case Trans obj:
                    AddTransCommand.Execute(obj);
                    break;
                case Metal obj:
                    AddMetalCommand.Execute(obj);
                    break;
                case Mirror obj:
                    AddMirrorCommand.Execute(obj);
                    break;
                case Glow obj:
                    AddGlowCommand.Execute(obj);
                    break;
                case Light obj:
                    AddLightCommand.Execute(obj);
                    break;
                case BSDF obj:
                    AddBSDFCommand.Execute(obj);
                    break;
                default:
                    break;
            }
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }
          
            var dup = selected.Modifier.Duplicate() as HB.Radiance.IModifier;
            HB.Radiance.IModifier dialog_rc = DialogHelper.EditModifier(dup, selected.Locked, _control);

            if (dialog_rc == null) return;

            var newItem = CheckObjName(dialog_rc, selected.Name);
            var index = _userData.IndexOf(selected);
            _userData.RemoveAt(index);
            _userData.Insert(index, new ModifierViewData(newItem));
            this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
            ResetDataCollection();
        });


        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var selected = SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }

            if (selected.Locked)
            {
                MessageBox.Show(_control, "You cannot remove an item of system library!");
                return;
            }

            var res = MessageBox.Show(_control, $"Are you sure you want to delete:\n {selected.Name}", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                this._userData.Remove(selected);
                this._allData = _userData.Concat(_systemData).Distinct(_viewDataComparer).ToList();
                ResetDataCollection();
            }
        });

    }



    internal class ModifierViewData: ManagerViewDataBase
    {
        public string CType { get; }
        public string Reflectance { get; }
        public string Transmittance { get; }
        public string Emittance { get; }
        public string Source { get; } = "Model";
        public bool Locked { get; }
        public HB.Radiance.IModifier Modifier { get; }

        //private static IEnumerable<string> NRELLibraryIds =
        //    HB.Helper.EnergyLibrary.StandardsOpaqueModifiers.Keys
        //    .Concat(HB.Helper.EnergyLibrary.StandardsWindowModifiers.Keys);

        private static IEnumerable<string> LBTLibraryIds =
         HB.ModelRadianceProperties.Default.ModifierList.Select(_ => _.Identifier);

        private static IEnumerable<string> UserLibIds = HB.Helper.EnergyLibrary.UserModifiers.Select(_ => _.Identifier);
        private static IEnumerable<string> LockedLibraryIds = LBTLibraryIds.Concat(UserLibIds);

        public ModifierViewData(HB.Radiance.IModifier c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            this.CType = c.GetType().Name;
          
            this.Modifier = c;
            c.CalVisualValues();
            switch (c)
            {
                case HB.Plastic p:
                    this.Reflectance = Math.Round(p.Reflectance, 5).ToString();
                    break;
                case HB.Glass g:
                    this.Transmittance = Math.Round(g.Transmittance, 5).ToString();
                    this.Reflectance = Math.Round(g.Reflectance, 5).ToString();
                    break;
                case HB.BSDF b:
                    this.Reflectance = Math.Round(b.Reflectance, 5).ToString();
                    this.Transmittance = Math.Round(b.Transmittance, 5).ToString();
                    break;
                case HB.Glow glow:
                    this.Emittance = Math.Round(glow.Emittance, 5).ToString();
                    break;
                case HB.Light l:
                    this.Emittance = Math.Round(l.Emittance, 5).ToString();
                    break;
                case HB.Trans t:
                    this.Transmittance = Math.Round(t.Transmittance, 5).ToString();
                    this.Reflectance = Math.Round(t.Reflectance, 5).ToString();
                    break;
                case HB.Metal m:
                    this.Reflectance = Math.Round(m.Reflectance, 5).ToString();
                    break;
                case HB.Mirror mr:
                    this.Reflectance = Math.Round(mr.Reflectance, 5).ToString();
                    break;
                default:
                    break;
            }
           
            this.SearchableText = $"{this.Name}_{this.CType}";

            //check if system library
            this.Locked = LockedLibraryIds.Contains(c.Identifier);

            if (LBTLibraryIds.Contains(c.Identifier)) this.Source = "LBT";
            else if (UserLibIds.Contains(c.Identifier)) this.Source = "User";
        }


    }

}
