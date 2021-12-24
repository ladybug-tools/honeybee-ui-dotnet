using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class SubConstructionSetViewModel : CheckboxButtonViewModel
    {
        private ModelEnergyProperties _libSource { get; set; }
        public SubConstructionSetViewModel(ref ModelEnergyProperties libSource, string cName, Action<HoneybeeSchema.IIDdBase> setAction) : base(setAction)
        {
            HoneybeeSchema.Energy.IConstruction c = null;
            if (!string.IsNullOrEmpty(cName))
                c = SystemEnergyLib.ConstructionList.FirstOrDefault(_ => _.Identifier == cName);

            this.SetPropetyObj(c);

            _libSource = libSource;
        }

        public ICommand SelectConstrucitonCommand => new RelayCommand(() =>
        {
            var lib = _libSource;
            var dialog = new Dialog_ConstructionManager(ref lib, true);
            var dialog_rc = dialog.ShowModal();
            if (dialog_rc != null)
            {
                var rs = dialog_rc[0];
                this.SetPropetyObj(rs);

                var matNames = rs.GetAbridgedConstructionMaterials();
                var mats = SystemEnergyLib.MaterialList.Where(_ => matNames.Contains(_.Identifier));
                _libSource.AddConstruction(rs);
                _libSource.AddMaterials(mats);
            }
        });
    }

}
