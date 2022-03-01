using Eto.Forms;
using System.Windows.Input;

namespace Honeybee.UI
{
    public class OptionalButton : DynamicLayout
    {
        private Button button;
        private Button button2;
        public BindableBinding<TextControl, string> TextBinding => this.button.TextBinding;
        public ICommand Command
        {
            get => this.button.Command;
            set => this.button.Command = value;
        }
        public ICommand RemoveCommand
        {
            get => this.button2.Command;
            set => this.button2.Command = value;
        }

        public string Text
        {
            get => this.button.Text;
            set => this.button.Text = value;
        }
        public int Width
        {
            get => this.button.Width + 15;
            set => this.button.Width = value - 15;
        }
    
        public bool IsRemoveVisable
        {
            get => this.button2.Visible;
            set => this.button2.Visible = value;
        }

        public OptionalButton(): base()
        {

            button = new Button() { };
            button2 = new Button() { Text = "-", Width = 15, ToolTip = "Remove" };
            this.BeginHorizontal();
            this.Add(button, true, true);
            this.Add(button2);
            this.EndHorizontal();
            this.DefaultSpacing= new Eto.Drawing.Size(-1, 0);
        }



    }

}
