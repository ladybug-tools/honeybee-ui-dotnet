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
            set
            {
                if (_messageText != value)
                {
                    _messageText = value;
                    OnPropertyChanged();
                }
            }
        }

        private static readonly MessageViewModel _instance = new MessageViewModel();
        public static MessageViewModel Instance => _instance;



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
