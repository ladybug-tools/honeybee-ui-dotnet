using Eto.Forms;
using HB = HoneybeeSchema;

namespace Honeybee.UI
{
    public abstract class Dialog_ResourceEditor<T> : Dialog<T>
    {

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
                    Close(obj);
                else
                    MessageBox.Show($"Invalid {obj.GetType().Name}, please check all inputs again!");
            }
            catch (System.Exception err)
            {
                Dialog_Message.Show(this, err);
            }

        });
    }
}
