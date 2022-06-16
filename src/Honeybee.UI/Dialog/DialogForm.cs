using Eto.Forms;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{
    public abstract class DialogForm<T> : Form
    {
        private T _result;
        private System.Action<T> _returnFunc;
        public void ShowModal(Eto.Forms.Control owner, System.Action<T> returnFunc)
        {
            _returnFunc = returnFunc;
            if (!this.Loaded)
            {
                var c = owner.Bounds.Center;
                c.X = c.X - this.Width / 2;
                c.Y = c.Y - 200;
                this.Location = c;
            }

            this.Show();
        }

        public void Close(T obj)
        {
            _result = obj;
            base.Close();
        }

        public override void Close()
        {
            _result = default(T);
            base.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _returnFunc?.Invoke(_result);
            base.OnClosed(e);
        }

        public RelayCommand<T> OkCommand =>
      new RelayCommand<T>((T obj) =>
      {
          try
          {
              var isValid = false;
              if (obj is HB.IIDdBase idd)
                  idd.Identifier = idd.DisplayName;
              if (obj is HB.OpenAPIGenBaseModel m)
                  isValid = m.IsValid(true);
              if (isValid)
                  this.Close(obj);
              else
                  MessageBox.Show($"Invalid {obj.GetType().Name}, please check all inputs again!");
          }
          catch (System.Exception err)
          {
              MessageBox.Show(err.Message);
          }

      });
    }
}
