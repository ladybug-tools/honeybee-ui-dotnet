using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class InternalMassViewModel : CheckboxPanelViewModel
    {
        private InternalMassAbridged _refHBObj => this.refObjProperty as InternalMassAbridged;
        // string identifier, string construction, double area, string displayName = null

        // FlowPerArea
        private DoubleViewModel _area;

        public DoubleViewModel Area
        {
            get => _area;
            private set => this.Set(() => _area = value, nameof(Area));
        }


        // Construction
        private ButtonViewModel _construction;

        public ButtonViewModel Construction
        {
            get => _construction;
            private set => this.Set(() => _construction = value, nameof(Construction));
        }

        public InternalMassAbridged Default { get; private set; }
        public InternalMassViewModel(ModelProperties libSource, List<InternalMassAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new InternalMassAbridged(Guid.NewGuid().ToString(), "c", 0);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateInternalMassAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateInternalMassAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //Area
            this.Area = new DoubleViewModel((n) => _refHBObj.Area = n);
            if (loads.Select(_ => _?.Area).Distinct().Count() > 1)
                this.Area.SetNumberText(this.Varies);
            else
                this.Area.SetNumberText(_refHBObj.Area.ToString());


            //Construction
            var sch = libSource.Energy.Constructions
                .OfType<IIDdBase>()
                .FirstOrDefault(_ => _.Identifier == _refHBObj.Construction);
            this.Construction = new ButtonViewModel((n) => _refHBObj.Construction = n?.Identifier);
            if (loads.Select(_ => _?.Construction).Distinct().Count() > 1)
                this.Construction.SetBtnName(this.Varies);
            else
                this.Construction.SetPropetyObj(sch);


        }

        public InternalMassAbridged MatchObj(InternalMassAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateInternalMassAbridged() ?? new InternalMassAbridged(Guid.NewGuid().ToString(), "", 0);

            if (!this.Area.IsVaries)
                obj.Area = this._refHBObj.Area;
            if (!this.Construction.IsVaries)
            {
                if (this._refHBObj.Construction == null)
                    throw new ArgumentException("Missing required construction of the internal mass!");
                obj.Construction = this._refHBObj.Construction;
            }
          
            return obj;
        }

        public RelayCommand ConstructionCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_ConstructionManager(_libSource.Energy, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Construction.SetPropetyObj(dialog_rc[0]);
            }
        });

    }


}
