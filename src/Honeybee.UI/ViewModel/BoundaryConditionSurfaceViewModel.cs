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
        private string _adjacentSurfaceText = "room_id>>face_id";
        public string AdjacentSurfaceText
        {
            get => _adjacentSurfaceText;
            private set
            {
                _isAdjacentSurfaceVaries = value == this.Varies;
                this.Set(() => _adjacentSurfaceText = value, nameof(AdjacentSurfaceText));

            }
        }

        public Surface Default { get; private set; }
        private string _separator = ">>";
        public BoundaryConditionSurfaceViewModel(List<Surface> objs, Action<Surface> setAction)
        {
            this.Default = new Surface(new List<string>());
            this._refHBObj = objs.FirstOrDefault()?.DuplicateSurface();
            this._refHBObj = this._refHBObj ?? this.Default.DuplicateSurface();


            if (objs.Select(_ => string.Join(_separator, _?.BoundaryConditionObjects)).Distinct().Count() > 1)
                this.AdjacentSurfaceText = this.Varies;
            else
                this.AdjacentSurfaceText = string.Join(_separator, this._refHBObj.BoundaryConditionObjects);

            setAction?.Invoke(this._refHBObj);

        }

        public Surface MatchObj(Surface obj)
        {
            obj = obj?.DuplicateSurface() ?? new Surface(new List<string>());

            if (!this._isAdjacentSurfaceVaries)
            {
                var adjList = this.AdjacentSurfaceText
                    .Split( new string[] { _separator }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(_=>_.Trim()).ToList();
                obj.BoundaryConditionObjects = adjList;
            }
               
            return obj;
        }


    }


}
