using Eto.Drawing;
using Eto.Forms;
using System;
using System.Linq;
using UnitsNet;

namespace Honeybee.UI
{
    public class Dialog_Message : Dialog
    {
        private MessageViewModel vm;
        private Dialog_Message(string message, string fullMessage, string title, string info)
        {
            try
            {
               
                Padding = new Padding(5);
                Resizable = true;
                this.Icon = DialogHelper.HoneybeeIcon;
                WindowStyle = WindowStyle.Default;
                MinimumSize = new Size(350, 200);
                Width = 500;


                vm = MessageViewModel.Instance;
                vm.Update(message, fullMessage, title, info);


                this.Bind(_ => _.Title, vm, _ => _.TitleText);

                var layout = new DynamicLayout { };
                //layout.Spacing = new Size(10, 20);
                layout.Padding = new Padding(10, 20);
                layout.Spacing = new Size(4, 4);
                layout.DefaultSpacing = new Size(4, 4);

                var textLabel = new Label();
                //textLabel.Height = 50;
                textLabel.TextAlignment = TextAlignment.Center;
                textLabel.Bind(_ => _.Text, vm, _ => _.MessageText);
                textLabel.Bind(_ => _.Visible, vm, _ => _.HasMessageText);
                layout.AddRow(textLabel);

                var textArea = new RichTextArea();
                textArea.Height = 300;
                textArea.Bind(_ => _.Text, vm, _ => _.FullMessageText);
                textArea.Bind(_ => _.Visible, vm, _ => _.ShowFullMessageText);
                layout.AddSeparateRow(controls: new[] { textArea }, xscale: true, yscale: true);

                var infoLabel = new Label();
                infoLabel.Enabled = false;
                infoLabel.Bind(_ => _.Text, vm, _ => _.InfoText);
               

                var moreDetailBtn = new Button();
                moreDetailBtn.Bind(_ => _.Text, vm, _ => _.DetailButtonText);
                moreDetailBtn.Bind(_ => _.Visible, vm, _ => _.ShowDetailButton);
                moreDetailBtn.Command = vm.DetailBtnClick;

                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => Close();

                layout.AddSeparateRow(infoLabel, null, moreDetailBtn, this.DefaultButton );


                //Create layout
                Content = layout;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
                //Rhino.RhinoApp.WriteLine(e.Message);
            }
            
        }

       

        public static void Show(Control owner, string message, string fullMessage, string title, string info)
        {
            var dia = new Dialog_Message(message, fullMessage, title, info);
            if (owner == null)
                dia.ShowModal();
            else
                dia.ShowModal(owner);
        }

        public static void Show(Control owner, string fullMessage, string title)
        {
            Show(owner, null, fullMessage, title, null);
        }

        public static void Show(Control owner, string message)
        {
            Show(owner, message, null, null, null);
        }

        public static void ShowFullMessage(Control owner, string fullMessage)
        {
            Show(owner, fullMessage, null);
        }

        public static void Show(string message)
        {
            Show(null, message);
        }

        public static void ShowFullMessage(string fullMessage)
        {
            ShowFullMessage(null, fullMessage);
        }


        public static void Show(Control owner, System.Exception error)
        {
            var msg = error.Message ?? error?.InnerException?.Message;
            var fullmsg = error.HelpLink != null ? $"{error.HelpLink}{Environment.NewLine}{error}" : error.ToString();
            Show(owner, msg, fullmsg, "Error", error.HelpLink);
        }

        public static void Show(System.Exception error)
        {
            Show(null, error);
        }


    }
}
