using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{
    public class ShadeViewModel : INotifyPropertyChanged
    {
        private Shade _hbObj;
        public Shade HoneybeeObject
        {
            get { return _hbObj; }
            private set
            {
                if (_hbObj != value)
                {
                    _hbObj = value;
                    OnPropertyChanged();
                }
            }
        }

        private static readonly ShadeViewModel _instance = new ShadeViewModel();
        public static ShadeViewModel Instance => _instance;


        public static Action<string> _action;
        public Action<string> ActionWhenChanged
        {
            get { return _action; }
            private set
            {
                if (_action != value)
                {
                    _action = value;
                    OnPropertyChanged();
                }
            }
        }

        private ShadeViewModel()
        {
        }

        public void Update(Shade honeybeeObj, Action<string> actionWhenChanged)
        {
            HoneybeeObject = honeybeeObj;
            ActionWhenChanged = actionWhenChanged ?? delegate (string m) { };
        }

        void OnPropertyChanged([CallerMemberName] string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }



}
