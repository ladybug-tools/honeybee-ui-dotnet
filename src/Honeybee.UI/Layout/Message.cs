using Eto.Drawing;
using Eto.Forms;

namespace Honeybee.UI
{
    public static partial class PanelHelper
    {
        private static Panel _messagePanel;
        public static Panel UpdateMessagePanel(string messageText)
        {
            var vm = MessageViewModel.Instance;
            vm.Update(messageText);
            if (_messagePanel == null)
            {
                _messagePanel = GenMessagePanel();
            }
            return _messagePanel;
        }
        
        private static Panel GenMessagePanel()
        {
            var vm = MessageViewModel.Instance;

            var layout = new DynamicLayout { DataContext = vm };
            layout.Spacing = new Size(5, 5);
            layout.Padding = new Padding(10);
            layout.DefaultSpacing = new Size(2, 2);

            var textArea = new RichTextArea();
            textArea.Height = 300;
            
            textArea.TextBinding.BindDataContext((MessageViewModel m) => m.MessageText);
            layout.AddSeparateRow(textArea);
            
            return layout;

        }
    }
}
