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
            set => this.Set(() => _area = value, nameof(Area));
        }


        // Construction
        private ButtonViewModel _construction;

        public ButtonViewModel Construction
        {
            get => _construction;
            set => this.Set(() => _construction = value, nameof(Construction));
        }
        public Func<double> InternalMassAreaPicker { get; set; }
 
        public bool EnableInternalMassAreaPicker
        {
            get => InternalMassAreaPicker != null;
            set => this.RefreshControl(nameof(EnableInternalMassAreaPicker));
        }

        public InternalMassAbridged Default { get; private set; }
        public InternalMassViewModel(ModelProperties libSource, List<InternalMassAbridged> loads, Action<IIDdBase> setAction):base(libSource, setAction)
        {
            this.Default = new InternalMassAbridged(Guid.NewGuid().ToString(), ReservedText.None, 0);
            this.refObjProperty = loads.FirstOrDefault()?.DuplicateInternalMassAbridged();
            this.refObjProperty = this._refHBObj ?? this.Default.DuplicateInternalMassAbridged();


            if (loads.Distinct().Count() == 1 && loads.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }


            //Area
            this.Area = new DoubleViewModel((n) => _refHBObj.Area = n);
            this.Area.SetUnits(Units.AreaUnit.SquareMeter, Units.UnitType.Area);
            if (loads.Select(_ => _?.Area).Distinct().Count() > 1)
                this.Area.SetNumberText(ReservedText.Varies);
            else
                this.Area.SetBaseUnitNumber(_refHBObj.Area);


            //Construction
            var sch = libSource.Energy.ConstructionList.FirstOrDefault(_ => _.Identifier == _refHBObj.Construction);
            this.Construction = new ButtonViewModel((n) => _refHBObj.Construction = n?.Identifier);
            if (loads.Select(_ => _?.Construction).Distinct().Count() > 1)
                this.Construction.SetBtnName(ReservedText.Varies);
            else
                this.Construction.SetPropetyObj(sch);


        }

        public InternalMassAbridged MatchObj(InternalMassAbridged obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            obj = obj?.DuplicateInternalMassAbridged() ?? new InternalMassAbridged(Guid.NewGuid().ToString(), "Not Set", 0);

            if (!this.Area.IsVaries)
                obj.Area = this._refHBObj.Area;
            if (!this.Construction.IsVaries)
            {
                if (this._refHBObj.Construction == null)
                    throw new ArgumentException("Missing a required construction of the internal mass!");
                obj.Construction = this._refHBObj.Construction;
            }
          
            return obj;
        }

        public RelayCommand ConstructionCommand => new RelayCommand(() =>
        {
            var lib = _libSource.Energy;
            var dialog = new Dialog_ConstructionManager(ref lib, true);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.Construction.SetPropetyObj(dialog_rc[0]);
            }
        });

        public RelayCommand InternalMassAreaCommand => new RelayCommand(() =>
        {
            var area = this.InternalMassAreaPicker?.Invoke();
            if (area.HasValue)
            {
                this.Area.SetNumberText(area.Value.ToString());
            }

        });

    }


}
