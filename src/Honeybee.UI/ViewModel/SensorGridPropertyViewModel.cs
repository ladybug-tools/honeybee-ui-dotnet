using Eto.Forms;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class SensorGridPropertyViewModel : ViewModelBase
    {
        private SensorGrid _refHBObj;

        private List<SensorGrid> _hbObjs;
        public int TabIndex
        {
            get => 0;
            set { this.Set(null, nameof(TabIndex)); }
        }

        #region Property

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

        private bool _isGroupIDVaries;
        public string GroupID
        {
            get => _refHBObj.GroupIdentifier;
            set
            {
                _isGroupIDVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.GroupIdentifier = value, nameof(GroupID));
            }
        }

        private bool _isRoomIDVaries;
        public string RoomID
        {
            get => _refHBObj.RoomIdentifier;
            set
            {
                _isRoomIDVaries = value == ReservedText.Varies;
                this.Set(() => _refHBObj.RoomIdentifier = value, nameof(RoomID));
            }
        }

        #endregion

        private View.SensorGridProperty _control;

        public SensorGrid Default { get; private set; }
        internal SensorGridPropertyViewModel(View.SensorGridProperty panel)
        {
            this.Default = new SensorGrid("id", new List<Sensor>());
            _refHBObj = this.Default.DuplicateSensorGrid();

            this._control = panel;
            Update(new List<SensorGrid>() { _refHBObj });

        }

        public void Update(List<SensorGrid> objs)
        {
            this.TabIndex = 0;
            this._refHBObj = objs.FirstOrDefault().DuplicateSensorGrid();

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

            // GroupIdentifier
            if (objs.Select(_ => _.GroupIdentifier).Distinct().Count() > 1)
                this.GroupID = ReservedText.Varies;
            else
                this.GroupID = this._refHBObj.GroupIdentifier;


            // RoomIdentifier
            if (objs.Select(_ => _.RoomIdentifier).Distinct().Count() > 1)
                this.RoomID = ReservedText.Varies;
            else
                this.RoomID = this._refHBObj.RoomIdentifier;


            this._hbObjs = objs.Select(_ => _.DuplicateSensorGrid()).ToList();
        }

        public List<SensorGrid> GetSensorGrids()
        {
            var refObj = this._refHBObj;
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this._isGroupIDVaries)
                    item.GroupIdentifier = refObj.GroupIdentifier;

                if (!this._isRoomIDVaries)
                    item.RoomIdentifier = refObj.RoomIdentifier;

            }

            return this._hbObjs;
        }

        public ICommand HBDataBtnClick => new RelayCommand(() => {

            Honeybee.UI.Dialog_Message.Show(this._control, this._refHBObj.ToJson(true), "Schema Data");
        });
    }


}
