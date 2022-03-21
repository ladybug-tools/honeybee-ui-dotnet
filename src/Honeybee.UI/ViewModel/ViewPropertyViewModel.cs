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

        private DoubleViewModel _px;
        public DoubleViewModel Px
        {
            get => _px;
            set => this.Set(() => _px = value, nameof(Px));
        }
        private DoubleViewModel _py;
        public DoubleViewModel Py
        {
            get => _py;
            set => this.Set(() => _py = value, nameof(Py));
        }
        private DoubleViewModel _pz;
        public DoubleViewModel Pz
        {
            get => _pz;
            set => this.Set(() => _pz = value, nameof(Pz));
        }

        private DoubleViewModel _Vx;
        public DoubleViewModel Vx
        {
            get => _Vx;
            set => this.Set(() => _Vx = value, nameof(Vx));
        }
        private DoubleViewModel _Vy;
        public DoubleViewModel Vy
        {
            get => _Vy;
            set => this.Set(() => _Vy = value, nameof(Vy));
        }
        private DoubleViewModel _Vz;
        public DoubleViewModel Vz
        {
            get => _Vz;
            set => this.Set(() => _Vz = value, nameof(Vz));
        }

        private DoubleViewModel _Ux;
        public DoubleViewModel Ux
        {
            get => _Ux;
            set => this.Set(() => _Ux = value, nameof(Ux));
        }
        private DoubleViewModel _Uy;
        public DoubleViewModel Uy
        {
            get => _Uy;
            set => this.Set(() => _Uy = value, nameof(Uy));
        }
        private DoubleViewModel _Uz;
        public DoubleViewModel Uz
        {
            get => _Uz;
            set => this.Set(() => _Uz = value, nameof(Uz));
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
            this.Default = new HoneybeeSchema.View("id", new List<double>() { 0,0,0}, new List<double>() {0,0,0 }, new List<double>() { 0, 0, 0 });
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

            // Position x
            this.Px = new DoubleViewModel(_ => _refHBObj.Position[0] = _);
            if (objs.Select(_ => _?.Position[0]).Distinct().Count() > 1)
                this.Px.SetNumberText(ReservedText.Varies);
            else
                this.Px.SetNumberText(this._refHBObj.Position[0].ToString());

            // Position y
            this.Py = new DoubleViewModel(_ => _refHBObj.Position[1] = _);
            if (objs.Select(_ => _?.Position[1]).Distinct().Count() > 1)
                this.Py.SetNumberText(ReservedText.Varies);
            else
                this.Py.SetNumberText(this._refHBObj.Position[1].ToString());

            // Position z
            this.Pz = new DoubleViewModel(_ => _refHBObj.Position[2] = _);
            if (objs.Select(_ => _?.Position[2]).Distinct().Count() > 1)
                this.Pz.SetNumberText(ReservedText.Varies);
            else
                this.Pz.SetNumberText(this._refHBObj.Position[2].ToString());

            // Vector x
            this.Vx = new DoubleViewModel(_ => _refHBObj.Direction[0] = _);
            if (objs.Select(_ => _?.Direction[0]).Distinct().Count() > 1)
                this.Vx.SetNumberText(ReservedText.Varies);
            else
                this.Vx.SetNumberText(this._refHBObj.Direction[0].ToString());

            // Vector y
            this.Vy = new DoubleViewModel(_ => _refHBObj.Direction[1] = _);
            if (objs.Select(_ => _?.Direction[1]).Distinct().Count() > 1)
                this.Vy.SetNumberText(ReservedText.Varies);
            else
                this.Vy.SetNumberText(this._refHBObj.Direction[1].ToString());

            // Vector z
            this.Vz = new DoubleViewModel(_ => _refHBObj.Direction[2] = _);
            if (objs.Select(_ => _?.Direction[2]).Distinct().Count() > 1)
                this.Vz.SetNumberText(ReservedText.Varies);
            else
                this.Vz.SetNumberText(this._refHBObj.Direction[2].ToString());

            // Up x
            this.Ux = new DoubleViewModel(_ => _refHBObj.UpVector[0] = _);
            if (objs.Select(_ => _?.UpVector[0]).Distinct().Count() > 1)
                this.Ux.SetNumberText(ReservedText.Varies);
            else
                this.Ux.SetNumberText(this._refHBObj.UpVector[0].ToString());

            // Up y
            this.Uy = new DoubleViewModel(_ => _refHBObj.UpVector[1] = _);
            if (objs.Select(_ => _?.UpVector[1]).Distinct().Count() > 1)
                this.Uy.SetNumberText(ReservedText.Varies);
            else
                this.Uy.SetNumberText(this._refHBObj.UpVector[1].ToString());

            // Up z
            this.Uz = new DoubleViewModel(_ => _refHBObj.UpVector[2] = _);
            if (objs.Select(_ => _?.UpVector[2]).Distinct().Count() > 1)
                this.Uz.SetNumberText(ReservedText.Varies);
            else
                this.Uz.SetNumberText(this._refHBObj.UpVector[2].ToString());


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
            refObj.IsValid(true);
            foreach (var item in this._hbObjs)
            {

                if (!this._isDisplayNameVaries)
                    item.DisplayName = refObj.DisplayName;

                if (!this.Px.IsVaries)
                    item.Position[0] = refObj.Position[0];

                if (!this.Py.IsVaries)
                    item.Position[1] = refObj.Position[1];

                if (!this.Pz.IsVaries)
                    item.Position[2] = refObj.Position[2];

                if (!this.Vx.IsVaries)
                    item.Direction[0] = refObj.Direction[0];

                if (!this.Vy.IsVaries)
                    item.Direction[1] = refObj.Direction[1];

                if (!this.Vz.IsVaries)
                    item.Direction[2] = refObj.Direction[2];

                if (!this.Ux.IsVaries)
                    item.UpVector[0] = refObj.UpVector[0];

                if (!this.Uy.IsVaries)
                    item.UpVector[1] = refObj.UpVector[1];

                if (!this.Uz.IsVaries)
                    item.UpVector[2] = refObj.UpVector[2];

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
