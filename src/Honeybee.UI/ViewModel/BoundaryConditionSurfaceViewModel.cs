using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class BoundaryConditionSurfaceViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        private Surface _refHBObj;

        private bool _isAdjacentSurfaceVaries;
        private List<string> _adjacentSurfaceText = new List<string>() { "face_id", "room_id" };
        public List<string> AdjacentSurfaceText
        {
            get => _adjacentSurfaceText;
            set
            {
                _isAdjacentSurfaceVaries = value.FirstOrDefault() == ReservedText.Varies;
                this.Set(() => _adjacentSurfaceText = value, nameof(AdjacentSurfaceText));

            }
        }

        public Surface Default { get; private set; }
        private Action<Surface> _setAction;
        public BoundaryConditionSurfaceViewModel(List<Surface> objs, Action<Surface> setAction)
        {
            this.Default = new Surface(new List<string>());
            this._refHBObj = objs.FirstOrDefault()?.DuplicateSurface();
            this._refHBObj = this._refHBObj ?? this.Default.DuplicateSurface();


            if (objs.Select(_ => string.Join(";", _?.BoundaryConditionObjects)).Distinct().Count() > 1)
                this.AdjacentSurfaceText = new List<string>() { ReservedText.Varies };
            else
                this.AdjacentSurfaceText = this._refHBObj.BoundaryConditionObjects ?? new List<string>() { "Not Set" };

            setAction?.Invoke(this._refHBObj);
            _setAction = setAction;
        }

        public Surface MatchObj(Surface obj)
        {
            obj = obj?.DuplicateSurface() ?? new Surface(new List<string>());

            if (!this._isAdjacentSurfaceVaries)
            {
                var adjList = this.AdjacentSurfaceText
                    .Select(_=>_.Trim()).ToList();
                obj.BoundaryConditionObjects = adjList;
            }
               
            return obj;
        }


        public void EditSurfaceBC(Eto.Forms.Control host = default)
        {
            var dialog = new Dialog_SurfaceBoundaryCondition(this.AdjacentSurfaceText);
            var dialog_rc = dialog.ShowModal(host);
            if (dialog_rc != null)
            {
                var cleaned = dialog_rc.Select(_ => _.Trim()).Where(_=> !string.IsNullOrEmpty(_) && _ != null).ToList();
                this.AdjacentSurfaceText = cleaned;
                this._refHBObj.BoundaryConditionObjects = cleaned;
                _setAction?.Invoke(this._refHBObj);
            }
        }


    }


}
