using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Linq;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class SubModifierSetViewModel : CheckboxButtonViewModel
    {
        private ModelRadianceProperties _libSource { get; set; }
        public SubModifierSetViewModel(ref ModelRadianceProperties libSource, string cName, Action<HoneybeeSchema.IIDdBase> setAction) : base(setAction)
        {
            HoneybeeSchema.Radiance.IModifier c = null;
            if (!string.IsNullOrEmpty(cName))
            {
                // check in-model lib source before system lib
                c = libSource.ModifierList.FirstOrDefault(_ => _.Identifier == cName);
                c = c ?? SystemRadianceLib.ModifierList.FirstOrDefault(_ => _.Identifier == cName);
            }

            this.SetPropetyObj(c);

            _libSource = libSource;
        }

        public ICommand SelectModifierCommand => new RelayCommand(() =>
        {
            var lib = _libSource;
            var dialog = new Dialog_ModifierManager(ref lib, true);
            var dialog_rc = dialog.ShowModal();
            if (dialog_rc != null)
            {
                var rs = dialog_rc[0];
                this.SetPropetyObj(rs);
                _libSource.AddModifier(rs);
            }
        });
    }

}
