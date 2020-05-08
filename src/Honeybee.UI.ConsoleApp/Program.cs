using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;

namespace Honeybee.UI.ConsoleApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Eto.Forms.Application().Run(new MyForm());
            Console.ReadLine();
        }

        public class MyForm : Form
        {
            public MyForm()
            {
                ClientSize = new Eto.Drawing.Size(400, 300);
                Title = "Eto.Forms";

                var panel = new DynamicLayout();
                var btn = new Button() { Text="Energy Property"};
                btn.Click += (s, e) =>
                {
                    var energyProp = new HoneybeeSchema.RoomEnergyPropertiesAbridged();
                    var dialog = new Honeybee.UI.Dialog_RoomEnergyProperty(energyProp);
                    var dialog_rc = dialog.ShowModal();
                    if (dialog_rc != null)
                    {
                        Console.WriteLine(dialog_rc.ToJson());
                    }
                };

                var Messagebtn = new Button() { Text = "message text" };
                Messagebtn.Click += (s, e) =>
                {
                    var energyProp = new HoneybeeSchema.RoomEnergyPropertiesAbridged();
                    Dialog_Message.Show(energyProp.ToJson());
                    
                };

                var cSetbtn = new Button() { Text = "ConstructionSet" };
                cSetbtn.Click += (s, e) =>
                {
                    var cSet = new HoneybeeSchema.ConstructionSetAbridged(identifier: Guid.NewGuid().ToString());
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(cSet);
                    dialog.ShowModal();

                };

                var pTypebtn = new Button() { Text = "ProgramType" };
                pTypebtn.Click += (s, e) =>
                {
                    var pType = new HoneybeeSchema.ProgramTypeAbridged(identifier: Guid.NewGuid().ToString());
                    var dialog = new Honeybee.UI.Dialog_ProgramType(pType);
                    dialog.ShowModal();

                };

                panel.AddSeparateRow(btn);
                panel.AddSeparateRow(Messagebtn);
                panel.AddSeparateRow(cSetbtn);
                panel.AddSeparateRow(pTypebtn);
                panel.AddSeparateRow(null);
                Content = panel;
            }
        }
    }
}
