using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{
    public class DoorViewModel : INotifyPropertyChanged
    {
        private Door _hbObj;
        public Door HoneybeeObject
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

        public List<AnyOf<Outdoors, Surface>> Bcs =>
            new List<AnyOf<Outdoors, Surface>>()
            {
                new Outdoors(), new Surface(new List<string>())
            };

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value == -1)
                    throw new Exception("selected index set to -1");

                _selectedIndex = value;
                OnPropertyChanged();

                if (this.HoneybeeObject.BoundaryCondition.Obj.GetType().Name != Bcs[value].Obj.GetType().Name)
                {
                    //MessageBox.Show(Bcs[value]);
                    this.HoneybeeObject.BoundaryCondition = Bcs[value];
                    this.ActionWhenChanged("Set boundary condition");
                    
                }
                
            }
        }


        private bool _isOutdoor;
        public bool IsOutdoor
        {
            get { return _isOutdoor; }
            set
            {
                if (_isOutdoor != value)
                {
                    //MessageBox.Show("isoutdoor: " + value);
                    _isOutdoor = value;
                    OnPropertyChanged();
                }
            }

        }

        private static readonly DoorViewModel _instance = new DoorViewModel();
        public static DoorViewModel Instance => _instance;

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

        private DoorViewModel()
        {
        }

        public void Update(Door honeybeeObj, Action<string> actionWhenChanged)
        {
            HoneybeeObject = honeybeeObj;
            //HoneybeeObject.DisplayName = honeybeeObj.DisplayName ?? string.Empty;
            IsOutdoor = honeybeeObj.BoundaryCondition.Obj is Outdoors;
            SelectedIndex = Bcs.FindIndex(_ => _.Obj.GetType().Name == this.HoneybeeObject.BoundaryCondition.Obj.GetType().Name);
            ActionWhenChanged = actionWhenChanged ?? delegate (string m) { };
        }

        void OnPropertyChanged([CallerMemberName] string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }



}
