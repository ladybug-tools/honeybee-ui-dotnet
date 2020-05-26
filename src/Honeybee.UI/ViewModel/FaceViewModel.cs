using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{
    public class FaceViewModel : INotifyPropertyChanged
    {
        private Face _hbObj;
        public Face HoneybeeObject
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

        private string _apertureCount;
        public string ApertureCount
        {
            get { return _apertureCount.ToString(); }
            private set
            {
                if (_apertureCount != value)
                {
                    _apertureCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<AnyOf<Ground, Outdoors, Adiabatic, Surface>> Bcs =>
            new List<AnyOf<Ground, Outdoors, Adiabatic, Surface>>()
            {
                new Ground(), new Outdoors(), new Adiabatic(), new Surface(new List<string>())
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

        private static readonly FaceViewModel _instance = new FaceViewModel();
        public static FaceViewModel Instance => _instance;

  

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
        private FaceViewModel()
        {
        }

        public void Update(Face honeybeeObj, Action<string> actionWhenChanged)
        {
            HoneybeeObject = honeybeeObj;
            //HoneybeeObject.DisplayName = honeybeeObj.DisplayName ?? string.Empty;
            ApertureCount = honeybeeObj.Apertures.Count.ToString();
            IsOutdoor = honeybeeObj.BoundaryCondition.Obj is Outdoors;
            //BC = new Outdoors();
            //BC = honeybeeObj.BoundaryCondition.Obj.GetType().Name;
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
