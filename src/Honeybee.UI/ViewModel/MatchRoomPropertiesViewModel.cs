using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{
    public class MatchRoomPropertiesViewModel : ViewModelBase
    {
        private bool _name = true;
        public bool Name
        {
            get => _name;
            set => Set(() => _name = value, nameof(Name));
        }

        private bool _story = true;
        public bool Story
        {
            get => _story;
            set => Set(() => _story = value, nameof(Story));
        }

        private bool _multiplier = true;
        public bool Multiplier
        {
            get => _multiplier;
            set => Set(() => _multiplier = value, nameof(Multiplier));
        }

        private bool _mSet = true;
        public bool ModifierSet
        {
            get => _mSet;
            set => Set(() => _mSet = value, nameof(ModifierSet));
        }

        private bool _cSet = true;
        public bool ConstructionSet
        {
            get => _cSet;
            set => Set(() => _cSet = value, nameof(ConstructionSet));
        }

        private bool _pType = true;
        public bool ProgramType
        {
            get => _pType;
            set => Set(() => _pType = value, nameof(ProgramType));
        }

        private bool _hvac = true;
        public bool HVAC
        {
            get => _hvac;
            set => Set(() => _hvac = value, nameof(HVAC));
        }

        private bool _user = true;
        public bool User
        {
            get => _user;
            set => Set(() => _user = value, nameof(User));
        }


        private bool _light = true;
        public bool Lighting
        {
            get => _light;
            set => Set(() => _light = value, nameof(Lighting));
        }

        private bool _people = true;
        public bool People
        {
            get => _people;
            set => Set(() => _people = value, nameof(People));
        }

        private bool _elec = true;
        public bool ElecEquipment
        {
            get => _elec;
            set => Set(() => _elec = value, nameof(ElecEquipment));
        }

        private bool _gas = true;
        public bool GasEquipment
        {
            get => _gas;
            set => Set(() => _gas = value, nameof(GasEquipment));
        }

        private bool _vent = true;
        public bool Ventilation
        {
            get => _vent;
            set => Set(() => _vent = value, nameof(Ventilation));
        }

        private bool _infil = true;
        public bool Infiltration
        {
            get => _infil;
            set => Set(() => _infil = value, nameof(Infiltration));
        }

        private bool _setPt = true;
        public bool Setpoint
        {
            get => _setPt;
            set => Set(() => _setPt = value, nameof(Setpoint));
        }

        private bool _hotWater = true;
        public bool ServiceHotWater
        {
            get => _hotWater;
            set => Set(() => _hotWater = value, nameof(ServiceHotWater));
        }

        private bool _masses = true;
        public bool InternalMasses
        {
            get => _masses;
            set => Set(() => _masses = value, nameof(InternalMasses));
        }


        private bool _ventControl = true;
        public bool VentControl
        {
            get => _ventControl;
            set => Set(() => _ventControl = value, nameof(VentControl));
        }

        private bool _daylightControl = true;
        public bool DaylightControl
        {
            get => _daylightControl;
            set => Set(() => _daylightControl = value, nameof(DaylightControl));
        }

        private bool _all = true;
        public bool All
        {
            get => _all;
            set {
                Name = value;
                Story = value;
                Multiplier = value;
                User = value;

                ModifierSet = value;
                ConstructionSet = value;
                ProgramType = value; 
                HVAC = value;

                Lighting = value;
                People = value;
                ElecEquipment = value;
                GasEquipment = value;
                Ventilation = value;
                Infiltration = value;
                Setpoint = value;
                ServiceHotWater = value;
                InternalMasses = value;

                VentControl = value;
                DaylightControl = value;
            }
        }


        private HB.Room _sourceRoom;
        private IEnumerable<HB.Room> _targetRooms;
        public MatchRoomPropertiesViewModel(HB.Room sourceRoom, IEnumerable<HB.Room> targetRooms)
        {
            this._sourceRoom = sourceRoom.DuplicateRoom();
            this._targetRooms = targetRooms.Select(_ => _.DuplicateRoom());
        }

        public List<HB.Room> GetUpdatedRooms()
        {
            var rooms = this._targetRooms.ToList();
            foreach (var room in rooms)
            {
                var s = this._sourceRoom.DuplicateRoom();
                if (Name) room.DisplayName = s.DisplayName;
                if (Story) room.Story = s.Story;
                if (Multiplier) room.Multiplier = s.Multiplier;
                if (User) room.UserData = s.UserData;

                room.Properties = room.Properties ?? new HB.RoomPropertiesAbridged();
                room.Properties.Radiance = room.Properties.Radiance ?? new HB.RoomRadiancePropertiesAbridged();
                room.Properties.Energy = room.Properties.Energy ?? new HB.RoomEnergyPropertiesAbridged();

                if (ModifierSet) room.Properties.Radiance.ModifierSet = s.Properties?.Radiance?.ModifierSet;
                if (ConstructionSet) room.Properties.Energy.ConstructionSet = s.Properties?.Energy?.ConstructionSet;
                if (ProgramType) room.Properties.Energy.ProgramType = s.Properties?.Energy?.ProgramType;
                if (HVAC) room.Properties.Energy.Hvac = s.Properties?.Energy?.Hvac;

                if (Lighting) room.Properties.Energy.Lighting = s.Properties?.Energy?.Lighting;
                if (People) room.Properties.Energy.People = s.Properties?.Energy?.People;
                if (ElecEquipment) room.Properties.Energy.ElectricEquipment = s.Properties?.Energy?.ElectricEquipment;
                if (GasEquipment) room.Properties.Energy.GasEquipment = s.Properties?.Energy?.GasEquipment;
                if (Ventilation) room.Properties.Energy.Ventilation = s.Properties?.Energy?.Ventilation;
                if (Infiltration) room.Properties.Energy.Infiltration = s.Properties?.Energy?.Infiltration;
                if (Setpoint) room.Properties.Energy.Setpoint = s.Properties?.Energy?.Setpoint;
                if (ServiceHotWater) room.Properties.Energy.ServiceHotWater = s.Properties?.Energy?.ServiceHotWater;
                if (InternalMasses) room.Properties.Energy.InternalMasses = s.Properties?.Energy?.InternalMasses;

                if (VentControl) room.Properties.Energy.WindowVentControl = s.Properties?.Energy?.WindowVentControl;
                if (DaylightControl) room.Properties.Energy.DaylightingControl = s.Properties?.Energy?.DaylightingControl;
            }

            return rooms;
        }

    }
}
