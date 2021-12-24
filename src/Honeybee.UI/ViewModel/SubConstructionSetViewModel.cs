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
            {
                // check in-model lib source before system lib
                c = libSource.ConstructionList.FirstOrDefault(_ => _.Identifier == cName);
                c = c ?? SystemEnergyLib.ConstructionList.FirstOrDefault(_ => _.Identifier == cName);
            }

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
                _libSource.AddConstruction(rs);
            }
        });
    }

}
