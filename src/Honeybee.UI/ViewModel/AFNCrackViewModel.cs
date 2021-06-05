using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class AFNCrackViewModel : CheckboxPanelViewModel<AFNCrack>
    {
        private AFNCrack _refHBObj => this.refObjProperty;
        // double flowCoefficient, double flowExponent = 0.65

        // FlowCoefficient
        private DoubleViewModel _flowCoefficient;

        public DoubleViewModel FlowCoefficient
        {
            get => _flowCoefficient;
            private set
            {
                this.Set(() => _flowCoefficient = value, nameof(FlowCoefficient));
            }
        }

        // flowExponent
        private DoubleViewModel _flowExponent;

        public DoubleViewModel FlowExponent
        {
            get => _flowExponent;
            private set { this.Set(() => _flowExponent = value, nameof(FlowExponent)); }
        }


        public AFNCrack Default { get; private set; }
        public AFNCrackViewModel(ModelProperties libSource, List<AFNCrack> loads, Action<AFNCrack> setAction) : base(libSource, setAction)
        {
            this.Default = new AFNCrack(0);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateAFNCrack();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateAFNCrack();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //FlowCoefficient
            this.FlowCoefficient = new DoubleViewModel((n) => _refHBObj.FlowCoefficient = n);
            if (loads.Select(_ => _?.FlowCoefficient).Distinct().Count() > 1)
                this.FlowCoefficient.SetNumberText(this.Varies);
            else
                this.FlowCoefficient.SetNumberText(_refHBObj.FlowCoefficient.ToString());

            //FlowExponent
            this.FlowExponent = new DoubleViewModel((n) => _refHBObj.FlowExponent = n);
            if (loads.Select(_ => _?.FlowExponent).Distinct().Count() > 1)
                this.FlowExponent.SetNumberText(this.Varies);
            else
                this.FlowExponent.SetNumberText(_refHBObj.FlowExponent.ToString());


        }

        public AFNCrack MatchObj(AFNCrack obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateAFNCrack() ?? new AFNCrack(0);

            if (!this.FlowCoefficient.IsVaries)
                obj.FlowCoefficient = this._refHBObj.FlowCoefficient;

            if (!this.FlowExponent.IsVaries)
                obj.FlowExponent = this._refHBObj.FlowExponent;

            return obj;
        }

    }




}
