using Eto.Forms;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class MessageViewModel : ViewModelBase
    {
        private string _TitleText;
        public string TitleText
        {
            get => string.IsNullOrEmpty(_TitleText) ? DialogHelper.PluginName : $"{_TitleText} - {DialogHelper.PluginName}";
            set { this.Set(() => _TitleText = value, nameof(TitleText)); }
        }

        private string _InfoText;
        public string InfoText
        {
            get => _InfoText;
            set { this.Set(() => _InfoText = value, nameof(InfoText)); }
        }

        private string _messageText;
        public string MessageText
        {
            get => _messageText;
            set { this.Set(() => _messageText = value, nameof(MessageText)); }
        }

        public bool HasMessageText => !string.IsNullOrEmpty(this.MessageText);

        private string _FullMessageText;
        public string FullMessageText
        {
            get => _FullMessageText;
            set { this.Set(() => _FullMessageText = value, nameof(FullMessageText)); }
        }

        public bool HasFullMessageText => !string.IsNullOrEmpty(this.FullMessageText);
       

        private bool _ShowFullMessageText;
        public bool ShowFullMessageText
        {
            get 
            {
                if (!HasMessageText)
                    return true;
                return _ShowFullMessageText;
            }
            set
            { 
                this.Set(() => _ShowFullMessageText = value, nameof(ShowFullMessageText));
                this.DetailButtonText = value ? "Hide details": "Show details";
            }
        }

        private string _DetailButtonText = "Show details";
        public string DetailButtonText
        {
            get => _DetailButtonText;
            set { this.Set(() => _DetailButtonText = value, nameof(DetailButtonText)); }
        }

        private bool _ShowDetailButton = true;
        public bool ShowDetailButton
        {
            get
            {
                if (!HasMessageText)
                    return false;
                return _ShowDetailButton;
            }
            set { this.Set(() => _ShowDetailButton = value, nameof(ShowDetailButton)); }
        }


        private static MessageViewModel _instance;
        public static MessageViewModel Instance
        {
            get => _instance = _instance ?? new MessageViewModel();
        }



        private MessageViewModel()
        {
        }


        public void Update(string message, string fullMessage, string title, string info)
        {
            MessageText = message;
            FullMessageText = fullMessage; 
            TitleText = title;
            InfoText = info;

            this.ShowFullMessageText = this.HasMessageText ? false: true;
            this.ShowDetailButton = this.HasFullMessageText;
        }


        public ICommand DetailBtnClick => new RelayCommand(() =>
        {
            if (!this.HasFullMessageText)
                return;

            this.ShowFullMessageText = !this.ShowFullMessageText;

            if (this.ShowFullMessageText)
            {
               

            }
            else // hide
            {
            }


        });
    }



}
