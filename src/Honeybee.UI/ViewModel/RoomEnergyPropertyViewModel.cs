using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Energy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    [Obsolete("This is deprecated, please use RoomPropertyViewModel instead", true)]
    public class RoomEnergyPropertyViewModel : ViewModelBase
    {
        public RoomEnergyPropertiesAbridged HoneybeeObject { get; set; }

        #region Selected Value


        private ConstructionSetAbridged _const;

        public ConstructionSetAbridged ConstructionSet
        {
            get { return _const; }
            private set
            {
                this.Set(() => _const = value, nameof(ConstructionSet));
                this.HoneybeeObject.ConstructionSet = value.Identifier == this.SetNull ? null : value.Identifier;
            }
        }

        private ProgramTypeAbridged _pType;

        public ProgramTypeAbridged ProgramType
        {
            get { return _pType; }
            private set
            {
                this.Set(() => _pType = value, nameof(ProgramType));
                this.HoneybeeObject.ProgramType = value.Identifier == this.SetNull ? null : value.Identifier;
            }
        }

        private IHvac _hvac;

        public IHvac HVAC
        {
            get { return _hvac; }
            private set
            {
                this.Set(() => _hvac = value, nameof(HVAC));
                this.HoneybeeObject.Hvac = value.Identifier == this.SetNull ? null : value.Identifier;
            }
        }

        private PeopleAbridged _ppl;

        public PeopleAbridged People
        {
            get { return _ppl; }
            private set
            {
                this.Set(() => _ppl = value, nameof(People));
                this.HoneybeeObject.People = value.Identifier == this.ByProgramType ? null : value;
            }
        }

        private LightingAbridged _lpd;

        public LightingAbridged Lighting
        {
            get { return _lpd; }
            private set
            {
                this.Set(() => _lpd = value, nameof(Lighting));
                this.HoneybeeObject.Lighting = value.Identifier == this.ByProgramType ? null : value;
            }
        }

        private ElectricEquipmentAbridged _eqp;

        public ElectricEquipmentAbridged ElectricEquipment
        {
            get { return _eqp; }
            private set
            {
                this.Set(() => _eqp = value, nameof(ElectricEquipment));
                this.HoneybeeObject.ElectricEquipment = value.Identifier == this.ByProgramType ? null : value;
            }
        }

        private GasEquipmentAbridged _gas;

        public GasEquipmentAbridged GasEquipment
        {
            get { return _gas; }
            private set
            {
                this.Set(() => _gas = value, nameof(GasEquipment));
                this.HoneybeeObject.GasEquipment = value.Identifier == this.ByProgramType ? null : value;
            }
        }

        private InfiltrationAbridged _inf;

        public InfiltrationAbridged Infiltration
        {
            get { return _inf; }
            private set
            {
                this.Set(() => _inf = value, nameof(Infiltration));
                this.HoneybeeObject.Infiltration = value.Identifier == this.ByProgramType ? null : value;
            }
        }

        private VentilationAbridged _vent;

        public VentilationAbridged Ventilation
        {
            get { return _vent; }
            private set
            {
                this.Set(() => _vent = value, nameof(Ventilation));
                this.HoneybeeObject.Ventilation = value.Identifier == this.ByProgramType ? null : value;
            }
        }

        private SetpointAbridged _stp;

        public SetpointAbridged Setpoint
        {
            get { return _stp; }
            private set
            {
                this.Set(() => _stp = value, nameof(Setpoint));
                this.HoneybeeObject.Setpoint = value.Identifier == this.ByProgramType ? null : value;
            }
        }
        #endregion


        public ObservableCollection<ConstructionSetAbridged> ConstructionSets { get; set; }
        public ObservableCollection<ProgramTypeAbridged> ProgramTypes { get; set; }
        public ObservableCollection<IHvac> Hvacs { get; set; }

        public ObservableCollection<PeopleAbridged> Peoples { get; set; }
        public ObservableCollection<LightingAbridged> Lightings { get; set; }
        public ObservableCollection<ElectricEquipmentAbridged> ElectricEquipments { get; set; }
        public ObservableCollection<GasEquipmentAbridged> GasEquipments { get; set; }
        public ObservableCollection<InfiltrationAbridged> Infiltrations { get; set; }
        public ObservableCollection<VentilationAbridged> Ventilations { get; set; }
        public ObservableCollection<SetpointAbridged> Setpoints { get; set; }



        //public Action<string> ActionWhenChanged { get; private set; }
        private string SetNull = "SetNull";
        private string AddNew = "AddNew";
        private string NoChanges = "No Changes";
        private string ByProgramType = "By Room Program Type";
        public ModelEnergyProperties ModelEnergyProperties { get; set; }

        public Control Control { get; set; }
        public RoomEnergyPropertyViewModel(Control control, ModelEnergyProperties libSource, RoomEnergyPropertiesAbridged roomEnergy, bool updateChangesOnly)
        {
            this.Control = control;
            this.ModelEnergyProperties = libSource;

            var EnergyProp = roomEnergy ?? new RoomEnergyPropertiesAbridged();
            if (updateChangesOnly)
            {
                EnergyProp = new RoomEnergyPropertiesAbridged(
                   NoChanges,
                   NoChanges,
                   NoChanges,
                   new PeopleAbridged(NoChanges, 0, "", ""),
                   new LightingAbridged(NoChanges, 0, ""),
                   new ElectricEquipmentAbridged(NoChanges, 0, ""),
                   new GasEquipmentAbridged(NoChanges, 0, ""),
                   new ServiceHotWaterAbridged(NoChanges, 0, ""),
                   new InfiltrationAbridged(NoChanges, 0, ""),
                   new VentilationAbridged(NoChanges),
                   new SetpointAbridged(NoChanges, "", "")
                   );
            }
            this.HoneybeeObject = EnergyProp.DuplicateRoomEnergyPropertiesAbridged();

            this.PrepConstructionSets(updateChangesOnly);
            this.PrepProgramTypes(updateChangesOnly);
            this.PrepHvacs(updateChangesOnly);

            this.PrepPeoples(updateChangesOnly);
            this.PrepLpd(updateChangesOnly);
            this.PrepEpq(updateChangesOnly);
            this.PrepGas(updateChangesOnly);
            this.PrepInf(updateChangesOnly);
            this.PrepVent(updateChangesOnly);
            this.PrepSetpoint(updateChangesOnly);

        }

        #region Prepare dropdown data


        private void PrepConstructionSets(bool updateChangesOnly)
        {
            var items = this.ModelEnergyProperties.ConstructionSets
                ?.OfType<ConstructionSetAbridged>()
                ?.OrderBy(_ => _.DisplayName ?? _.Identifier)
                ?.ToList();

            items = items ?? new List<ConstructionSetAbridged>();
            var cSets = new ObservableCollection<ConstructionSetAbridged>(items);
            // null case:
            var nullValue = new ConstructionSetAbridged(SetNull, "By Global Building Construction Set");
            cSets.Insert(0, nullValue);

            var noChange = new ConstructionSetAbridged(NoChanges);
            if (updateChangesOnly)
                cSets.Insert(0, noChange);

            this.ConstructionSets = cSets;

            // set selected item
            var found = cSets.FirstOrDefault(_ => _.Identifier == this.HoneybeeObject.ConstructionSet);
            this.ConstructionSet = found ?? (updateChangesOnly ? noChange : nullValue);
        }

        private void PrepProgramTypes(bool updateChangesOnly)
        {
            var items = this.ModelEnergyProperties.ProgramTypes
                ?.OfType<ProgramTypeAbridged>()
                ?.OrderBy(_ => _.DisplayName ?? _.Identifier)
                ?.ToList();

            items = items ?? new List<ProgramTypeAbridged>();
            var pTypes = new ObservableCollection<ProgramTypeAbridged>(items);

            var nullValue = new ProgramTypeAbridged(SetNull, "Unoccupied, NoLoads");
            pTypes.Insert(0, nullValue);

            var noChange = new ProgramTypeAbridged(NoChanges);
            if (updateChangesOnly)
                pTypes.Insert(0, noChange);

            this.ProgramTypes = pTypes;

            // set selected item
            var found = pTypes.FirstOrDefault(_ => _.Identifier == this.HoneybeeObject.ProgramType);
            this.ProgramType = found ?? (updateChangesOnly ? noChange : nullValue);
        }

        private void PrepHvacs(bool updateChangesOnly)
        {
            var items = this.ModelEnergyProperties.Hvacs
                ?.OfType<IHvac>()
                ?.OrderBy(_ => _.DisplayName ?? _.Identifier)
                ?.ToList();

            items = items ?? new List<IHvac>();
            var pTypes = new ObservableCollection<IHvac>(items);
            var nullValue = new IdealAirSystemAbridged(SetNull, "Unconditioned");
            pTypes.Insert(0, nullValue);

            var noChange = new IdealAirSystemAbridged(NoChanges);
            if (updateChangesOnly)
                pTypes.Insert(0, noChange);

            this.Hvacs = pTypes;

            // set selected item
            var found = pTypes.FirstOrDefault(_ => _.Identifier == this.HoneybeeObject.Hvac);
            this.HVAC = found ?? (updateChangesOnly ? noChange : nullValue);
        }

        private void PrepPeoples(bool updateChangesOnly)
        {
            var items = HoneybeeSchema.Helper.EnergyLibrary.DefaultPeopleLoads
                .OrderBy(_ => _.DisplayName ?? _.Identifier);
            var collection = new ObservableCollection<PeopleAbridged>(items);

            var nullValue = new PeopleAbridged(ByProgramType, 0, "", "");
            collection.Insert(0, nullValue);

            var noChange = new PeopleAbridged(NoChanges, 0, "", "");
            if (updateChangesOnly)
                collection.Insert(0, noChange);

            this.Peoples = collection;

            // set selected item
            this.People = this.HoneybeeObject.People ?? (updateChangesOnly ? noChange : nullValue);
        }
        private void PrepLpd(bool updateChangesOnly)
        {
            var items = HoneybeeSchema.Helper.EnergyLibrary.DefaultLightingLoads
                .OrderBy(_ => _.DisplayName ?? _.Identifier);
            var collection = new ObservableCollection<LightingAbridged>(items);

            var nullValue = new LightingAbridged(ByProgramType, 0, "");
            collection.Insert(0, nullValue);

            var noChange = new LightingAbridged(NoChanges, 0, "");
            if (updateChangesOnly)
                collection.Insert(0, noChange);

            this.Lightings = collection;

            // set selected item
            this.Lighting = this.HoneybeeObject.Lighting ?? (updateChangesOnly ? noChange : nullValue);
        }
        private void PrepEpq(bool updateChangesOnly)
        {
            var items = HoneybeeSchema.Helper.EnergyLibrary.DefaultElectricEquipmentLoads
                .OrderBy(_ => _.DisplayName ?? _.Identifier);
            var collection = new ObservableCollection<ElectricEquipmentAbridged>(items);

            var nullValue = new ElectricEquipmentAbridged(ByProgramType, 0, "");
            collection.Insert(0, nullValue);

            var noChange = new ElectricEquipmentAbridged(NoChanges, 0, "");
            if (updateChangesOnly)
                collection.Insert(0, noChange);

            this.ElectricEquipments = collection;

            // set selected item
            this.ElectricEquipment = this.HoneybeeObject.ElectricEquipment ?? (updateChangesOnly ? noChange : nullValue);
        }

        private void PrepGas(bool updateChangesOnly)
        {
            var items = HoneybeeSchema.Helper.EnergyLibrary.GasEquipmentLoads
                .OrderBy(_ => _.DisplayName ?? _.Identifier);
            var collection = new ObservableCollection<GasEquipmentAbridged>(items);

            var nullValue = new GasEquipmentAbridged(ByProgramType, 0, "");
            collection.Insert(0, nullValue);

            var noChange = new GasEquipmentAbridged(NoChanges, 0, "");
            if (updateChangesOnly)
                collection.Insert(0, noChange);

            this.GasEquipments = collection;

            // set selected item
            this.GasEquipment = this.HoneybeeObject.GasEquipment ?? (updateChangesOnly ? noChange : nullValue);
        }

        private void PrepInf(bool updateChangesOnly)
        {
            var items = HoneybeeSchema.Helper.EnergyLibrary.DefaultInfiltrationLoads;
            var collection = new ObservableCollection<InfiltrationAbridged>(items);

            var nullValue = new InfiltrationAbridged(ByProgramType, 0, "");
            collection.Insert(0, nullValue);

            var noChange = new InfiltrationAbridged(NoChanges, 0, "");
            if (updateChangesOnly)
                collection.Insert(0, noChange);

            this.Infiltrations = collection;

            // set selected item
            this.Infiltration = this.HoneybeeObject.Infiltration ?? (updateChangesOnly ? noChange : nullValue);
        }

        private void PrepVent(bool updateChangesOnly)
        {
            var items = HoneybeeSchema.Helper.EnergyLibrary.DefaultVentilationLoads
                .OrderBy(_ => _.DisplayName ?? _.Identifier);
            var collection = new ObservableCollection<VentilationAbridged>(items);

            var nullValue = new VentilationAbridged(ByProgramType);
            collection.Insert(0, nullValue);

            var noChange = new VentilationAbridged(NoChanges);
            if (updateChangesOnly)
                collection.Insert(0, noChange);

            this.Ventilations = collection;

            // set selected item
            this.Ventilation = this.HoneybeeObject.Ventilation ?? (updateChangesOnly ? noChange : nullValue);
        }
        private void PrepSetpoint(bool updateChangesOnly)
        {
            var items = HoneybeeSchema.Helper.EnergyLibrary.DefaultSetpoints
                .OrderBy(_ => _.DisplayName ?? _.Identifier);
            var collection = new ObservableCollection<SetpointAbridged>(items);

            var nullValue = new SetpointAbridged(ByProgramType, "", "");
            collection.Insert(0, nullValue);

            var noChange = new SetpointAbridged(NoChanges, "", "");
            if (updateChangesOnly)
                collection.Insert(0, noChange);

            this.Setpoints = collection;

            // set selected item
            this.Setpoint = this.HoneybeeObject.Setpoint ?? (updateChangesOnly ? noChange : nullValue);
        }
        #endregion

        #region Commands
        public ICommand AddNewConstructionSet => new RelayCommand(() =>
        {
            var localLib = this.ModelEnergyProperties.DuplicateModelEnergyProperties();
            var dialog = new Dialog_OpsConstructionSet(localLib);
            var dialog_rc = dialog.ShowModal(this.Control);
            if (dialog_rc.constructionSet != null)
            {
                var newItem = dialog_rc.constructionSet;
                this.ConstructionSets.Insert(0, newItem);
                this.ConstructionSet = newItem;

                var cons = dialog_rc.constructions.ToList();
                var mats = dialog_rc.materials.ToList();
                this.ModelEnergyProperties.AddConstructions(cons);
                this.ModelEnergyProperties.AddConstructionSet(newItem);
                this.ModelEnergyProperties.AddMaterials(mats);

                MessageBox.Show(
                    this.Control,
                    $"ConstructionSet [{newItem.DisplayName ?? newItem.Identifier}] is added to model along with [{cons.Count}] constructions and [{mats.Count}] materials.",
                    MessageBoxType.Information);
            }
        });
        public ICommand AddNewProgramType => new RelayCommand(() =>
        {
            var localLib = this.ModelEnergyProperties.DuplicateModelEnergyProperties();
            var dialog = new Dialog_OpsProgramTypes(localLib);
            var dialog_rc = dialog.ShowModal(this.Control);
            if (dialog_rc.programType != null)
            {
                var newItem = dialog_rc.programType;
                this.ProgramTypes.Insert(0, newItem);
                this.ProgramType = newItem;

                this.ModelEnergyProperties.AddSchedules(dialog_rc.schedules);
                this.ModelEnergyProperties.AddProgramType(newItem);

                MessageBox.Show(
                    this.Control,
                    $"ProgramType [{newItem.DisplayName ?? newItem.Identifier}] is added to model along with [{dialog_rc.schedules.Count()}] schedules.",
                    MessageBoxType.Information
                    );
            }
        });

        public ICommand AddNewHVAC => new RelayCommand(() =>
        {
            var dialog = new Dialog_OpsHVACs();
            var dialog_rc = dialog.ShowModal(this.Control);
            if (dialog_rc != null)
            {
                this.Hvacs.Insert(0, dialog_rc);
                this.HVAC = dialog_rc;
                this.ModelEnergyProperties.AddHVAC(dialog_rc);

                MessageBox.Show(
                    this.Control,
                    $"A new HVAC [{dialog_rc.DisplayName ?? dialog_rc.Identifier}] is added to model.",
                    MessageBoxType.Information
                    );
            }
        });
        #endregion

    }



}
