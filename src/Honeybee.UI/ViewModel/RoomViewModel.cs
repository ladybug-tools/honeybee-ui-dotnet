using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Honeybee.UI
{
    public class RoomViewModel : INotifyPropertyChanged
    {
        private Room _hbObj;
        public Room HoneybeeObject
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

        private string _faceCount;
        public string FaceCount
        {
            get { return _faceCount.ToString(); }
            private set
            {
                if (_faceCount != value)
                {
                    _faceCount = value;
                    OnPropertyChanged();
                }
            }
        }


        private static readonly RoomViewModel _instance = new RoomViewModel();
        public static RoomViewModel Instance => _instance;

       

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

        public static Action<string> _redraw;
        public Action<string> Redraw
        {
            get { return _redraw; }
            private set
            {
                if (_redraw != value)
                {
                    _redraw = value;
                    OnPropertyChanged();
                }
            }
        }
        private RoomViewModel()
        {
        }

        public void Update(Room honeybeeRoom, Action<string> actionWhenChanged, Action<string> redrawDisplay)
        {
            HoneybeeObject = honeybeeRoom;
            //HoneybeeObject.DisplayName = honeybeeRoom.DisplayName ?? string.Empty;
            FaceCount = honeybeeRoom.Faces.Count().ToString();
            ActionWhenChanged = actionWhenChanged ?? delegate (string m) { };
            Redraw = redrawDisplay ?? delegate (string m) { };
        }

        void OnPropertyChanged([CallerMemberName] string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
          
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public string GetIDTitle()
        {
            return $"ID: {HoneybeeObject.Identifier}";
        }
    }



}
