using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class BoundaryConditionOutdoorViewModel : ViewModelBase
    {
        public string Varies => "<varies>";
        private Outdoors _refHBObj;

        // SunExposure
        private bool? _sunExposure;
        public bool? SunExposure
        {
            get => _sunExposure;
            private set {
                if (value.HasValue)
                    _refHBObj.SunExposure = value.Value;
                this.Set(() => _sunExposure = value, nameof(SunExposure)); 
            }  
        }

        // WindExposure
        private bool? _windExposure;
        public bool? WindExposure
        {
            get => _windExposure;
            private set {
                if (value.HasValue)
                    _refHBObj.WindExposure = value.Value;
                this.Set(() => _windExposure = value, nameof(WindExposure)); 
            }
        }

        // ViewFactor
        private DoubleViewModel _viewFactor;

        public DoubleViewModel ViewFactor
        {
            get => _viewFactor;
            private set => this.Set(() => _viewFactor = value, nameof(ViewFactor)); 
        }
        
        private bool _isViewFactorAutocalculate;

        public bool IsViewFactorAutocalculate
        {
            get => _isViewFactorAutocalculate;
            private set
            {

                if (value)
                    this._refHBObj.ViewFactor = new Autocalculate();
                else
                    this.ViewFactor.SetNumberText(this.ViewFactor.NumberText);

                IsViewFactorInputEnabled = !value;
                this.Set(() => _isViewFactorAutocalculate = value, nameof(IsViewFactorAutocalculate));
            }
        }

        private bool _isViewFactorInputEnabled;

        public bool IsViewFactorInputEnabled
        {
            get => _isViewFactorInputEnabled;
            private set { this.Set(() => _isViewFactorInputEnabled = value, nameof(IsViewFactorInputEnabled)); }
        }


        public Outdoors Default { get; private set; }

        public BoundaryConditionOutdoorViewModel(List<Outdoors> objs, Action<Outdoors> setAction)
        {
            this.Default = new Outdoors();
            this._refHBObj = objs.FirstOrDefault()?.DuplicateOutdoors();
            this._refHBObj = this._refHBObj ?? this.Default.DuplicateOutdoors();
          

            //SunExposure
            if (objs.Select(_ => _?.SunExposure).Distinct().Count() > 1)
                this.SunExposure = null;
            else
                this.SunExposure = _refHBObj.SunExposure;


            //WindExposure
            if (objs.Select(_ => _?.WindExposure).Distinct().Count() > 1)
                this.WindExposure = null;
            else
                this.WindExposure = _refHBObj.WindExposure;


            //ViewFactor
            this.ViewFactor = new DoubleViewModel((n) => _refHBObj.ViewFactor = n);
            var vfs = objs.Select(_ => _?.ViewFactor).Distinct();
            if (vfs.Count() > 1)
            {
                this.IsViewFactorAutocalculate = false;
                this.ViewFactor.SetNumberText(this.Varies);
            }
            else
            {
                this.ViewFactor.SetNumberText("0");
                this.IsViewFactorAutocalculate = vfs?.FirstOrDefault(_ => _?.Obj is Autocalculate) != null;
                if (!IsViewFactorAutocalculate)
                {
                    this.ViewFactor.SetNumberText(_refHBObj.ViewFactor.ToString());
                }
            }

            setAction?.Invoke(this._refHBObj);

        }

        public Outdoors MatchObj(Outdoors obj)
        {
            obj = obj?.DuplicateOutdoors() ?? new Outdoors();

            if (this.SunExposure.HasValue)
                obj.SunExposure = this._refHBObj.SunExposure;
            if (this.WindExposure.HasValue)
                obj.WindExposure = this._refHBObj.WindExposure;
            if (!this.ViewFactor.IsVaries)
                obj.ViewFactor = this._refHBObj.ViewFactor;

            return obj;
        }


    }


}
