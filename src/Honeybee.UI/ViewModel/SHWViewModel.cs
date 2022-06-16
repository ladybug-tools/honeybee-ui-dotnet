using Eto.Forms;
using HoneybeeSchema;
using HoneybeeSchema.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Honeybee.UI
{
    public class SHWViewModel : ViewModelBase
    {

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                Set(() => _name = value, nameof(Name));
            }
        }


        private SHWEquipmentType _EquipType;
        public SHWEquipmentType EquipType
        {
            get => _EquipType;
            set 
            {
                Set(() => _EquipType = value, nameof(EquipType));
                // disable AmbientCoffCondition for HeatPump_WaterHeater
                AmbientCoffConditionEnabled = value != SHWEquipmentType.HeatPump_WaterHeater;
            } 
        }


        private double _HeaterEff;
        public double HeaterEff
        {
            get => _HeaterEff;
            set
            {
                Set(() => _HeaterEff = value, nameof(HeaterEff));
                this.HeaterEffEnabled = true;
            }
        }

        private bool _AmbientCoffConditionEnabled;
        public bool AmbientCoffConditionEnabled
        {
            get => _AmbientCoffConditionEnabled;
            set => Set(() => _AmbientCoffConditionEnabled = value, nameof(AmbientCoffConditionEnabled));
        }

        private double _ambientCoffCondition;
        private DoubleViewModel _AmbientCoffConditionNumber;
        public DoubleViewModel AmbientCoffConditionNumber
        {
            get => _AmbientCoffConditionNumber;
            set
            {
                Set(() => _AmbientCoffConditionNumber = value, nameof(AmbientCoffConditionNumber));
                this.AmbientCoffConditionNumberEnabled = true;
            }
        }



        private string _AmbientCoffConditionRoomID;
        public string AmbientCoffConditionRoomID
        {
            get => _AmbientCoffConditionRoomID;
            set
            {
                Set(() => _AmbientCoffConditionRoomID = value, nameof(AmbientCoffConditionRoomID));
                this.AmbientCoffConditionRoomIDEnabled = true;
            }
        }

        private bool _HeaterEffAuto = true;
        public bool HeaterEffAuto
        {
            get => _HeaterEffAuto;
            set
            {
                Set(() => _HeaterEffAuto = value, nameof(HeaterEffAuto));
                if (value == true)
                {
                    this.HeaterEffEnabled = false;
                }
            }
        }


        private bool _HeaterEffEnabled;
        public bool HeaterEffEnabled
        {
            get => _HeaterEffEnabled;
            set
            {
                Set(() => _HeaterEffEnabled = value, nameof(HeaterEffEnabled));
                if (value == true)
                {
                    this.HeaterEffAuto = false;
                }
            }
        }


        private bool _AmbientCoffConditionNumberEnabled;
        public bool AmbientCoffConditionNumberEnabled
        {
            get => _AmbientCoffConditionNumberEnabled;
            set
            {
                Set(() => _AmbientCoffConditionNumberEnabled = value, nameof(AmbientCoffConditionNumberEnabled));
                if (value == true)
                {
                    this.AmbientCoffConditionRoomIDEnabled = false;
                }
            }
        }

        private bool _AmbientCoffConditionRoomIDEnabled;
        public bool AmbientCoffConditionRoomIDEnabled
        {
            get => _AmbientCoffConditionRoomIDEnabled;
            set
            {
                Set(() => _AmbientCoffConditionRoomIDEnabled = value, nameof(AmbientCoffConditionRoomIDEnabled));
                if (value == true)
                {
                    this.AmbientCoffConditionNumberEnabled = false;
                }

                this.VisibleAmbientCoffConditionRoomInput = AmbientCoffConditionRoomPicker == null;
                this.VisibleAmbientCoffConditionRoomPicker = AmbientCoffConditionRoomPicker != null;
            }
        }
        public Func<string> AmbientCoffConditionRoomPicker { get; set; }


        public bool VisibleAmbientCoffConditionRoomInput
        {
            get => AmbientCoffConditionRoomPicker == null;
            set => this.RefreshControl(nameof(VisibleAmbientCoffConditionRoomInput));
        }

        public bool VisibleAmbientCoffConditionRoomPicker
        {
            get => AmbientCoffConditionRoomPicker != null;
            set => this.RefreshControl(nameof(VisibleAmbientCoffConditionRoomPicker));
        }

        private double _AmbientLostCoff;
        public double AmbientLostCoff
        {
            get => _AmbientLostCoff;
            set => Set(() => _AmbientLostCoff = value, nameof(AmbientLostCoff));
        }

        private Dialog_SHW _control;
        public SHWViewModel(HoneybeeSchema.SHWSystem hvac, Dialog_SHW control)
        {
            _control = control;

            // Add a new
            if (hvac == null)
                throw new ArgumentNullException(nameof(hvac));


            // Editing existing obj
            this.Name = hvac.DisplayName ?? $"SHWSystem {Guid.NewGuid().ToString().Substring(0, 8)}";
            this.EquipType = hvac.EquipmentType;


            if (hvac.HeaterEfficiency == null || hvac.HeaterEfficiency.Obj is Autocalculate)
                this.HeaterEffAuto = true;
            else if (hvac.HeaterEfficiency.Obj is double htn)
                this.HeaterEff = htn;

            this.AmbientLostCoff = hvac.AmbientLossCoefficient;

            this.AmbientCoffConditionNumber = new DoubleViewModel((n) => _ambientCoffCondition = n);
            this.AmbientCoffConditionNumber.SetUnits(Units.TemperatureUnit.DegreeCelsius, Units.UnitType.Temperature);

            this.AmbientCoffConditionRoomIDEnabled = false;
            if (hvac.AmbientCondition == null)
                this.AmbientCoffConditionNumber.SetBaseUnitNumber(22);
            else if (hvac.AmbientCondition.Obj is double condition)
                this.AmbientCoffConditionNumber.SetBaseUnitNumber(condition);
            else if (hvac.AmbientCondition.Obj is string htn)
            {
                this.AmbientCoffConditionRoomID = htn;
                this.AmbientCoffConditionRoomIDEnabled = true;
            }
           

        }


        public SHWSystem GreateSys(HoneybeeSchema.SHWSystem existing = default)
        {
            
            var id = Guid.NewGuid().ToString().Substring(0, 8);
            id = $"SHWSystem_{id}";
            id = existing == null ? id : existing.Identifier;

            var obj = new SHWSystem(id);

            obj.DisplayName = this.Name;
            obj.EquipmentType = this.EquipType;


            if (this.HeaterEffAuto)
                obj.HeaterEfficiency = new Autocalculate();
            else
                obj.HeaterEfficiency = this.HeaterEff;

            if (obj.EquipmentType != SHWEquipmentType.HeatPump_WaterHeater)
            {
                if (this.AmbientCoffConditionNumberEnabled)
                    obj.AmbientCondition = this._ambientCoffCondition;
                else
                {
                    if (string.IsNullOrEmpty(this.AmbientCoffConditionRoomID))
                        throw new ArgumentException("Room ID for Ambient CoffCondition cannot be empty");
                    obj.AmbientCondition = this.AmbientCoffConditionRoomID;
                }
            }
            
            obj.AmbientLossCoefficient = this.AmbientLostCoff;

            obj.IsValid(true);
            return obj;
        }

        public void SetAmbientCoffConditionRoomPicker(Func<string> RoomIDPicker)
        {
            this.AmbientCoffConditionRoomPicker = RoomIDPicker;
        }

        public RelayCommand AmbientCoffConditionRoomPickerCommand => new RelayCommand(() =>
        {
            try
            {
                if (this.AmbientCoffConditionRoomPicker == null)
                    return;

                this._control.ParentWindow.Visible = false;
                var roomID = this.AmbientCoffConditionRoomPicker?.Invoke();
                //if (this._control == null || this._control.IsDisposed)
                //    return;
                if (!string.IsNullOrEmpty(roomID))
                {
                    this.AmbientCoffConditionRoomID = roomID;
                }
            }
            finally
            {
                this._control.ParentWindow.Visible = true;
            }
            
        });


    }





}
