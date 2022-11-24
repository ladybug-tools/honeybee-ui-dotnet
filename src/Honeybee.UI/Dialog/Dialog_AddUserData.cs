using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class Dialog_AddUserData : Dialog<UserDataItem>
    {
        public Dialog_AddUserData(IEnumerable<UserDataItem> allItems, UserDataItem userData = default)
        {
            try
            {
                Title = $"User Data - {DialogHelper.PluginName}";
                WindowStyle = WindowStyle.Default;
                this.Icon = DialogHelper.HoneybeeIcon;

                var p = new DynamicLayout();
                p.Width = 400;
                p.DefaultSpacing = new Size(4, 4);
                p.DefaultPadding = new Padding(4);


                var k = new TextBox();
                k.Text = userData?.Key;
                p.AddRow("Key");
                p.AddRow(k);

                var v = new TextArea() {Height = 100 };
                v.Text = userData?.Value;
                p.AddRow("Value");
                p.AddRow(v);

                p.AddRow(null);

                var OKButton = new Button() { Text = "OK" };
                OKButton.Click += (s, e) =>
                {
                    try
                    {
                        var key = k.Text.Trim();
                        if (allItems != null)
                        {
                            var keys = allItems.Select(_ => _.Key);
                            if (keys.Contains(key) && userData?.Key != key)
                            {
                                MessageBox.Show($"Key:[{key}] already exists, please use a different key");
                                return;
                            }
                        }

                        this.Close(new UserDataItem(key, v.Text));
                    }
                    catch (Exception er)
                    {
                        Dialog_Message.Show(this, er);
                    }

                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                p.AddSeparateRow(null, OKButton, this.AbortButton, null);
                p.Add(null);
                this.Content = p;
            }
            catch (Exception e)
            {
                Dialog_Message.Show(this, e);
                //throw;
            }

        }
    }
}
