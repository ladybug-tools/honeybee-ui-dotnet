using Eto.Forms;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class ViewPropertyViewModel : ViewModelBase
    {
        private HoneybeeSchema.View _refHBObj;

        private List<HoneybeeSchema.View> _hbObjs;
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


        public ViewType ViewType
        {
            get => _refHBObj.ViewType;
            set
            {
                ViewTypeText = ViewTypeNames[value];
                this.Set(() => _refHBObj.ViewType = value, nameof(ViewType));
            }
        }

        private bool _isViewTypeVaries;
        private string _viewTypeText;
        public string ViewTypeText
        {
            get => _viewTypeText;
            set
            {
                _isViewTypeVaries = value == ReservedText.Varies;
                this.Set(() => _viewTypeText = value, nameof(ViewTypeText));
            }
        }

        //* 0 Perspective (v)
        //* 1 Hemispherical fisheye(h)
        //* 2 Parallel(l)
        //* 3 Cylindrical panorama(c)
        //* 4 Angular fisheye(a)
        //* 5 Planisphere[stereographic] projection(s)
        internal static Dictionary<ViewType, string> ViewTypeNames = 
            new Dictionary<ViewType, string>() {
                { ViewType.v, "Perspective (v)" } ,
                { ViewType.h, "Hemispherical fisheye (h)" } ,
                { ViewType.l, "Parallel (l)" } ,
                { ViewType.c, "Cylindrical panorama (c)" } ,
                { ViewType.a, "Angular fisheye (a)" } ,
                { ViewType.s, "Planisphere[stereographic] projection (s)" } ,
            };

        private DoubleViewModel _hSize;
        public DoubleViewModel HSize
        {
            get => _hSize;
            set => this.Set(() => _hSize = value, nameof(HSize));
        }

        private DoubleViewModel _vSize;
        public DoubleViewModel VSize
        {
            get => _vSize;
            set => this.Set(() => _vSize = value, nameof(VSize));
        }

        private DoubleViewModel _shift;
        public DoubleViewModel Shift
        {
            get => _shift;
            set => this.Set(() => _shift = value, nameof(Shift));
        }

        private DoubleViewModel _Lift;
        public DoubleViewModel Lift
        {
            get => _Lift;
            set => this.Set(() => _Lift = value, nameof(Lift));
        }

        //ForeClip
        private DoubleViewModel _foreClip;
        public DoubleViewModel ForeClip
        {
            get => _foreClip;
            set => this.Set(() => _foreClip = value, nameof(ForeClip));
        }

        //AftClip
        private DoubleViewModel _AftClip;
        public DoubleViewModel AftClip
        {
            get => _AftClip;
            set => this.Set(() => _AftClip = value, nameof(AftClip));
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
        
        private View.ViewProperty _control;
      
        public HoneybeeSchema.View Default { get; private set; }
        internal ViewPropertyViewModel(View.ViewProperty panel)
        {
            this.Default = new HoneybeeSchema.View("id", new List<double>(), new List<double>(), new List<double>());
            _refHBObj = this.Default.DuplicateView();
   
            this._control = panel;
            Update(new List<HoneybeeSchema.View>() { _refHBObj });

        }

        public void Update(List<HoneybeeSchema.View> objs)
        {
            this.TabIndex = 0;
            this._refHBObj = objs.FirstOrDefault().DuplicateView();

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

            // ViewType
            if (objs.Select(_ => _.ViewType).Distinct().Count() > 1)
                this.ViewTypeText = ReservedText.Varies;
            else
                this.ViewType = this._refHBObj.ViewType;

            // HSize
            this.HSize = new DoubleViewModel(_ => _refHBObj.HSize = _);
            if (objs.Select(_ => _?.HSize).Distinct().Count() > 1)
                this.HSize.SetNumberText(ReservedText.Varies);
            else
                this.HSize.SetNumberText(this._refHBObj.HSize.ToString());

            // VSize
            this.VSize = new DoubleViewModel(_ => _refHBObj.VSize = _);
            if (objs.Select(_ => _?.VSize).Distinct().Count() > 1)
                this.VSize.SetNumberText(ReservedText.Varies);
            else
                this.VSize.SetNumberText(this._refHBObj.VSize.ToString());

            // Shift
            this.Shift = new DoubleViewModel(_ => _refHBObj.Shift = _);
            if (objs.Select(_ => _?.Shift).Distinct().Count() > 1)
                this.Shift.SetNumberText(ReservedText.Varies);
            else
                this.Shift.SetNumberText(this._refHBObj.Shift.ToString());

            // Lift
            this.Lift = new DoubleViewModel(_ => _refHBObj.Lift = _);
            if (objs.Select(_ => _?.Lift).Distinct().Count() > 1)
                this.Lift.SetNumberText(ReservedText.Varies);
            else
                this.Lift.SetNumberText(this._refHBObj.Lift.ToString());

            // ForeClip
            this.ForeClip = new DoubleViewModel(_ => _refHBObj.ForeClip = _);
            if (objs.Select(_ => _?.ForeClip).Distinct().Count() > 1)
                this.ForeClip.SetNumberText(ReservedText.Varies);
            else
                this.ForeClip.SetNumberText(this._refHBObj.ForeClip.ToString());

            // AftClip
            this.AftClip = new DoubleViewModel(_ => _refHBObj.AftClip = _);
            if (objs.Select(_ => _?.AftClip).Distinct().Count() > 1)
                this.AftClip.SetNumberText(ReservedText.Varies);
            else
                this.AftClip.SetNumberText(this._refHBObj.AftClip.ToString());


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


            this._hbObjs = objs.Select(_ => _.DuplicateView()).ToList();
        }

        public List<HoneybeeSchema.View> GetViews()
        {
            var refObj = this._refHBObj;
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this.HSize.IsVaries)
                    item.HSize = refObj.HSize;

                if (!this.VSize.IsVaries)
                    item.VSize = refObj.VSize;

                if (!this.Shift.IsVaries)
                    item.Shift = refObj.Shift;

                if (!this.Lift.IsVaries)
                    item.Lift = refObj.Lift;

                if (!this.ForeClip.IsVaries)
                    item.ForeClip = refObj.ForeClip;

                if (!this.AftClip.IsVaries)
                    item.AftClip = refObj.AftClip;

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
