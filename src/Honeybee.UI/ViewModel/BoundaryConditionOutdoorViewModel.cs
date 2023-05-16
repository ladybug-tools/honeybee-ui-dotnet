using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class BoundaryConditionOutdoorViewModel : ViewModelBase
    {
        private Outdoors _refHBObj;

        // SunExposure
        private CheckboxViewModel _sunExposure;
        public CheckboxViewModel SunExposure
        {
            get => _sunExposure;
            set => this.Set(() => _sunExposure = value, nameof(SunExposure)); 
        }

        // WindExposure
        private CheckboxViewModel _windExposure;
        public CheckboxViewModel WindExposure
        {
            get => _windExposure;
            set => this.Set(() => _windExposure = value, nameof(WindExposure)); 
        }

        // ViewFactor
        private DoubleViewModel _viewFactor;

        public DoubleViewModel ViewFactor
        {
            get => _viewFactor;
            set => this.Set(() => _viewFactor = value, nameof(ViewFactor)); 
        }
        
        private bool _isViewFactorAutocalculate;

        public bool IsViewFactorAutocalculate
        {
            get => _isViewFactorAutocalculate;
            set
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
            set { this.Set(() => _isViewFactorInputEnabled = value, nameof(IsViewFactorInputEnabled)); }
        }


        public Outdoors Default { get; private set; }

        public BoundaryConditionOutdoorViewModel(List<Outdoors> objs, Action<Outdoors> setAction)
        {
            this.Default = new Outdoors();
            this._refHBObj = objs.FirstOrDefault()?.DuplicateOutdoors();
            this._refHBObj = this._refHBObj ?? this.Default.DuplicateOutdoors();


            //SunExposure
            this.SunExposure = new CheckboxViewModel(_ => _refHBObj.SunExposure = _);
            if (objs.Select(_ => _?.SunExposure).Distinct().Count() > 1)
                this.SunExposure.SetCheckboxVaries();
            else
                this.SunExposure.SetCheckboxChecked(this._refHBObj.SunExposure);


            //WindExposure
            this.WindExposure = new CheckboxViewModel(_ => _refHBObj.WindExposure = _);
            if (objs.Select(_ => _?.WindExposure).Distinct().Count() > 1)
                this.WindExposure.SetCheckboxVaries();
            else
                this.WindExposure.SetCheckboxChecked(this._refHBObj.WindExposure);


            //ViewFactor
            this.ViewFactor = new DoubleViewModel((n) => _refHBObj.ViewFactor = n);
            var vfs = objs.Select(_ => _?.ViewFactor).Distinct();
            if (vfs.Count() > 1)
            {
                this.ViewFactor.SetNumberText(ReservedText.Varies);
                this.IsViewFactorAutocalculate = false;
            }
            else
            {
                this.IsViewFactorAutocalculate = vfs?.FirstOrDefault(_ => _?.Obj is Autocalculate) != null;
                if (!IsViewFactorAutocalculate)
                {
                    var num = _refHBObj.ViewFactor?.Obj is double tt ? tt.ToString() : "0";
                    this.ViewFactor.SetNumberText(num);
                    this.IsViewFactorAutocalculate = false;
                }
                else
                {

                    this.ViewFactor.SetNumberText("0");
                    this.IsViewFactorAutocalculate = true;
                }
            }

            setAction?.Invoke(this._refHBObj);

        }

        public Outdoors MatchObj(Outdoors obj)
        {
            obj = obj?.DuplicateOutdoors() ?? new Outdoors();

            if (!this.SunExposure.IsVaries)
                obj.SunExposure = this._refHBObj.SunExposure;
            if (!this.WindExposure.IsVaries)
                obj.WindExposure = this._refHBObj.WindExposure;
            if (!this.ViewFactor.IsVaries)
                obj.ViewFactor = this._refHBObj.ViewFactor;

            return obj;
        }


    }


}
