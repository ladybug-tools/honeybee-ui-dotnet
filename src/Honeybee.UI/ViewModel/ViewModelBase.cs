using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
    
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
