using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public static string None => "<None>";
        public string Varies => "<varies>";
        public string NoUserData => "No UserData";
        public string Unconditioned => "Unconditioned";
        public string Unoccupied => "Unoccupied, No Loads";
        public string ByGlobalConstructionSet => "By Global Construction Set";
        public string ByProgramType => "By Room Program Type";
        public string ByGlobalModifierSet => "By Global Modifier Set";
        public string NoControl => "No Control";
        public string ByGlobalSetting => "By Global Setting";
        public string ByParentSetting => "By Parent Setting";
        public string NoSchedule => "No Control Schedule";

        private static HoneybeeSchema.ModelEnergyProperties _systemEnergyLib;
        internal static HoneybeeSchema.ModelEnergyProperties SystemEnergyLib
        {
            get
            {
                if (_systemEnergyLib == null)
                {
                    var eng = HoneybeeSchema.ModelEnergyProperties.Default;
                    eng.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.StandardEnergyLibrary);
                    eng.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.UserEnergyLibrary);
                    _systemEnergyLib = eng;
                }
                return _systemEnergyLib;
            }
        }

        private static HoneybeeSchema.ModelRadianceProperties _systemRadianceLib;
        internal static HoneybeeSchema.ModelRadianceProperties SystemRadianceLib 
        { 
            get 
            {
                if (_systemRadianceLib == null)
                {
                    var rad = HoneybeeSchema.ModelRadianceProperties.Default;
                    //rad.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.StandardRadianceLibrary); 
                    rad.MergeWith(HoneybeeSchema.Helper.EnergyLibrary.UserRadianceLibrary);
                    _systemRadianceLib = rad;
                }
                return _systemRadianceLib;
            } 
        }


        protected void Set(Action setAction, string memberName)
        {
            setAction?.Invoke();
            OnPropertyChanged(memberName);
        }
        void OnPropertyChanged([CallerMemberName] string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public event PropertyChangedEventHandler PropertyChanged;


        public void RefreshControl(string memberName)
        {
            OnPropertyChanged(memberName);
        }

        public void RefreshControls(IEnumerable<string> memberNames)
        {
            foreach (var item in memberNames)
            {
                OnPropertyChanged(item);
            }
            
        }
    }



}
