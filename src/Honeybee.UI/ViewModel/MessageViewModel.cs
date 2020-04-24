using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Honeybee.UI
{
    public class MessageViewModel : INotifyPropertyChanged
    {
        private string _messageText;
        public string MessageText
        {
            get { return _messageText; }
            private set
            {
                if (_messageText != value)
                {
                    _messageText = value;
                    OnPropertyChanged();
                }
            }
        }


        private static MessageViewModel _instance;
        public static MessageViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MessageViewModel();
                }
                return _instance;
            }
        }
  

        private MessageViewModel()
        {
        }

        public void Update(string message)
        {
            MessageText = message;
        }

        void OnPropertyChanged([CallerMemberName] string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }



}
