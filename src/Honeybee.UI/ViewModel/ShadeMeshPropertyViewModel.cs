using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class ShadeMeshPropertyViewModel : ViewModelBase
    {
        private ShadeMesh _refHBObj;

        private List<ShadeMesh> _hbObjs;
        public int TabIndex
        {
            get => 0;
            set { this.Set(null, nameof(TabIndex)); }
        }

        public string Identifier
        {
            get => _refHBObj.Identifier;
            set { this.Set(() => _refHBObj.Identifier = value, nameof(_refHBObj.Identifier)); }
        }


        private bool _isDisplayNameVaries;
        public string DisplayName
        {
            get => _refHBObj.DisplayName;
            set
            {
                _isDisplayNameVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.DisplayName = value, nameof(DisplayName));
            }
        }

        private CheckboxViewModel _isDetached;
        public CheckboxViewModel IsDetached
        {
            get => _isDetached;
            set => this.Set(() => _isDetached = value, nameof(IsDetached));
        }


        #region Radiance
        // string modifier = null, string modifierBlk = null, string dynamicGroupIdentifier = null, List<RadianceSubFaceStateAbridged> states = null
        private CheckboxButtonViewModel _modifier;
        public CheckboxButtonViewModel Modifier
        {
            get => _modifier;
            set => this.Set(() => _modifier = value, nameof(Modifier));
        }

        private CheckboxButtonViewModel _modifierBlk;
        public CheckboxButtonViewModel ModifierBlk
        {
            get => _modifierBlk;
            set => this.Set(() => _modifierBlk = value, nameof(ModifierBlk)); 
        }

        #endregion

        #region Energy

        //string construction = null, VentilationOpening ventOpening = null

        private CheckboxButtonViewModel _construction;
        public CheckboxButtonViewModel Construction
        {
            get => _construction;
            set => this.Set(() => _construction = value, nameof(Construction)); 
        }

        // TransmittanceSchedule
        private CheckboxButtonViewModel _transmittanceSchedule;

        public CheckboxButtonViewModel TransmittanceSchedule
        {
            get => _transmittanceSchedule;
            set => this.Set(() => _transmittanceSchedule = value, nameof(TransmittanceSchedule)); 
        }

        #endregion

        #region UserData
        private UserDataViewModel _userData;
        public UserDataViewModel UserData
        {
            get => _userData;
            set { this.Set(() => _userData = value, nameof(UserData)); }
        }

        #endregion

        private View.ShadeMeshProperty _control;
        private ModelProperties _libSource { get; set; }
        public ShadeMesh Default { get; private set; }
        internal ShadeMeshPropertyViewModel(View.ShadeMeshProperty panel)
        {
            this.Default = new ShadeMesh("id", new Mesh3D(new List<List<double>>(), new List<List<int>>()), new ShadeMeshPropertiesAbridged());
            _refHBObj = this.Default.DuplicateShadeMesh();
            _libSource = new ModelProperties(ModelEnergyProperties.Default, ModelRadianceProperties.Default);
            this._control = panel;
            Update(_libSource, new List<ShadeMesh>() { _refHBObj });

        }

        public void Update(ModelProperties libSource, List<ShadeMesh> objs)
        {
            this.TabIndex = 0;
            this._libSource = libSource;
            this._refHBObj = objs.FirstOrDefault().DuplicateShadeMesh();
            var defaultEnergy = new ShadeMeshEnergyPropertiesAbridged();
            var defaultRadiance = new ShadeMeshRadiancePropertiesAbridged();
            _refHBObj.Properties.Energy = _refHBObj.Properties.Energy ?? defaultEnergy;
            _refHBObj.Properties.Radiance = _refHBObj.Properties.Radiance ?? defaultRadiance;

            // Identifier
            if (objs.Select(_ => _.Identifier).Distinct().Count() > 1)
                this.Identifier = ReservedText.Varies;
            else
                this.Identifier = this._refHBObj.Identifier;

            // DisplayName
            if (objs.Select(_ => _.DisplayName).Distinct().Count() > 1)
                this.DisplayName = ReservedText.Varies;
            else
                this.DisplayName = this._refHBObj.DisplayName;

            // IsDetached
            this.IsDetached = new CheckboxViewModel(_ => _refHBObj.IsDetached = _);
            if (objs.Select(_ => _?.IsDetached).Distinct().Count() > 1)
                this.IsDetached.SetCheckboxVaries();
            else
                this.IsDetached.SetCheckboxChecked(this._refHBObj.IsDetached);


            // Modifier
            var mdf = _libSource.Radiance.Modifiers?
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()?
                .FirstOrDefault(_ => _.Identifier  == _refHBObj.Properties.Radiance.Modifier);
            this.Modifier = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.Modifier = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.Modifier).Distinct().Count() > 1)
                this.Modifier.SetBtnName(ReservedText.Varies);
            else
                this.Modifier.SetPropetyObj(mdf);

            // ModifierBlk
            var mdfblk = _libSource.Radiance.Modifiers?
                .OfType<HoneybeeSchema.Radiance.IIDdRadianceBaseModel>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Radiance.ModifierBlk);
            this.ModifierBlk = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Radiance.ModifierBlk = s?.Identifier);

            if (objs.Select(_ => _.Properties.Radiance?.ModifierBlk).Distinct().Count() > 1)
                this.ModifierBlk.SetBtnName(ReservedText.Varies);
            else
                this.ModifierBlk.SetPropetyObj(mdfblk);

            // Construction
            var cts = _libSource.Energy.Constructions?
                .OfType<HoneybeeSchema.Energy.IIDdEnergyBaseModel>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.Construction);
            this.Construction = new CheckboxButtonViewModel((s) => _refHBObj.Properties.Energy.Construction = s?.Identifier);

            if (objs.Select(_ => _.Properties.Energy?.Construction).Distinct().Count() > 1)
                this.Construction.SetBtnName(ReservedText.Varies);
            else
                this.Construction.SetPropetyObj(cts);


            //TransmittanceSchedule
            var sch = _libSource.Energy.Schedules?
                .OfType<IIDdBase>()?
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Properties.Energy.TransmittanceSchedule);
            this.TransmittanceSchedule = new CheckboxButtonViewModel((n) => _refHBObj.Properties.Energy.TransmittanceSchedule = n?.Identifier);
            if (objs.Select(_ => _.Properties.Energy?.TransmittanceSchedule).Distinct().Count() > 1)
                this.TransmittanceSchedule.SetBtnName(ReservedText.Varies);
            else
                this.TransmittanceSchedule.SetPropetyObj(sch);

            // User data
            var allUserData = objs.Select(_ => _.UserData).Distinct().ToList();
            this.UserData = new UserDataViewModel(allUserData, (s) => _refHBObj.UserData = s, _control);


            this._hbObjs = objs.Select(_ => _.DuplicateShadeMesh()).ToList();
        }

        public List<ShadeMesh> GetShadeMeshs()
        {
            var refObj = this._refHBObj;
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this.IsDetached.IsVaries)
                    item.IsDetached = refObj.IsDetached;

                //string construction = null, VentilationOpening ventOpening = null
                // energy
                item.Properties.Energy = item.Properties.Energy ?? new ShadeMeshEnergyPropertiesAbridged();

                if (!this.Construction.IsVaries)
                    item.Properties.Energy.Construction = refObj.Properties.Energy.Construction;

                if (!this.TransmittanceSchedule.IsVaries)
                {
                    item.Properties.Energy.TransmittanceSchedule = this._refHBObj.Properties.Energy.TransmittanceSchedule;
                }

                // string modifier = null, string modifierBlk = null, string dynamicGroupIdentifier = null, List<RadianceSubFaceStateAbridged> states = null
                // radiance 
                item.Properties.Radiance = item.Properties.Radiance ?? new ShadeMeshRadiancePropertiesAbridged();

                if (!this.Modifier.IsVaries)
                    item.Properties.Radiance.Modifier = refObj.Properties.Radiance.Modifier;

                if (!this.ModifierBlk.IsVaries)
                    item.Properties.Radiance.ModifierBlk = refObj.Properties.Radiance.ModifierBlk;

                // User data
                item.UserData = this.UserData.MatchObj(item.UserData);


                // validate
                item.IsValid(true);
            }

            return this._hbObjs;
        }

        public ICommand ModifierCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Radiance;
            var dialog = new Dialog_ModifierManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.Modifier.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand ModifierBlkCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Radiance;
            var dialog = new Dialog_ModifierManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.ModifierBlk.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand ConstructionCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ConstructionManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                var c = dialog_rc[0];
                var isShadeMeshC = c is HoneybeeSchema.Energy.IShadeConstruction;
                if (!isShadeMeshC)
                {
                    Honeybee.UI.Dialog_Message.Show($"Cannot assign {c?.GetType()?.Name} to a ShadeMesh!");
                    return;
                }
                this.Construction.SetPropetyObj(c);
            }
        });
        public RelayCommand ScheduleCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ScheduleRulesetManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(this._control);
            if (dialog_rc != null)
            {
                this.TransmittanceSchedule.SetPropetyObj(dialog_rc[0]);
            }
        });

        public ICommand HBDataBtnClick => new RelayCommand(() => {

            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });
    }



}
