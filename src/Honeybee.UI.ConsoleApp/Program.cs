using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using HoneybeeSchema;

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
                    Dialog_Message.Show(this, energyProp.ToJson());
                    
                };

                var cSetbtn = new Button() { Text = "ConstructionSet" };
                cSetbtn.Click += (s, e) =>
                {
                    var cSet = new HoneybeeSchema.ConstructionSetAbridged(identifier: Guid.NewGuid().ToString());
                    var dialog = new Honeybee.UI.Dialog_ConstructionSet(cSet);
                    dialog.ShowModal(this);

                };

                var pTypebtn = new Button() { Text = "ProgramType" };
                pTypebtn.Click += (s, e) =>
                {
                    var pType = new HoneybeeSchema.ProgramTypeAbridged(identifier: Guid.NewGuid().ToString());
                    var dialog = new Honeybee.UI.Dialog_ProgramType(pType);
                    dialog.ShowModal(this);

                };

                var pTypeMngbtn = new Button() { Text = "ProgramTypeManager" };
                pTypeMngbtn.Click += (s, e) =>
                {
                    var md = new Model("", new ModelProperties(ModelEnergyProperties.Default));
                    var dialog = new Honeybee.UI.Dialog_ProgramTypeManager(md);
                    dialog.ShowModal(this);

                };

                var schbtn = new Button() { Text = "ScheduleRulesetManager" };
                schbtn.Click += (s, e) =>
                {
                    var md = new Model("", new ModelProperties(ModelEnergyProperties.Default));
                    var dialog = new Honeybee.UI.Dialog_ScheduleRulesetManager(md);
                    dialog.ShowModal(this);

                };


                panel.AddSeparateRow(btn);
                panel.AddSeparateRow(Messagebtn);
                panel.AddSeparateRow(cSetbtn);
                panel.AddSeparateRow(pTypebtn);
                panel.AddSeparateRow(pTypeMngbtn);
                panel.AddSeparateRow(schbtn);
                panel.AddSeparateRow(null);
                Content = panel;
            }
        }
    }
}
