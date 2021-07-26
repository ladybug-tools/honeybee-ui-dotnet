using Eto.Drawing;
using Eto.Forms;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    
    public class Dialog_ConstructionManager : Dialog<List<HB.Energy.IConstruction>>
    {
        private bool _returnSelectedOnly;
        private GridView _gd { get; set; }
        //private ModelEnergyProperties _modelEnergyProperties { get; set; }
        private ConstructionManagerViewModel _vm { get; set; }
        private Dialog_ConstructionManager()
        {
            Padding = new Padding(5);
            Resizable = true;
            Title = $"Construction Manager - {DialogHelper.PluginName}";
            WindowStyle = WindowStyle.Default;
            MinimumSize = new Size(800, 300);
            this.Icon = DialogHelper.HoneybeeIcon;
        }


        public Dialog_ConstructionManager(ref HB.ModelEnergyProperties libSource, bool returnSelectedOnly = false) : this()
        {
            this._returnSelectedOnly = returnSelectedOnly;
            //this._modelEnergyProperties = libSource;
            this._vm = new ConstructionManagerViewModel(libSource, this);
            Content = Init();
        }

        private DynamicLayout Init()
        {

            var layout = new DynamicLayout();
            layout.DefaultPadding = new Padding(10);
            layout.DefaultSpacing = new Size(5, 5);

            var addNew = new Button { Text = "Add" };
            addNew.Command = _vm.AddCommand;

            var duplicate = new Button { Text = "Duplicate" };
            duplicate.Command = _vm.DuplicateCommand;

            var edit = new Button { Text = "Edit" };
            edit.Command = _vm.EditCommand;

            var remove = new Button { Text = "Remove" };
            remove.Command = _vm.RemoveCommand;

            layout.AddSeparateRow("Constructions:", null, addNew, duplicate, edit, remove);

     
            this._gd = GenGridView();
            this._gd.Height = 250;
            layout.AddRow(this._gd);
            var gd = this._gd;


            var OKButton = new Button { Text = "OK" };
            OKButton.Click += (sender, e) => OkCommand.Execute(null);


            AbortButton = new Button { Text = "Cancel" };
            AbortButton.Click += (sender, e) => Close();
            layout.AddSeparateRow(null, OKButton, AbortButton, null);


            gd.CellDoubleClick += (s, e) => _vm.EditCommand.Execute(null);

            return layout;

        }

        private GridView GenGridView()
        {
            var gd = new GridView();
            gd.Bind(_ => _.DataStore, _vm, _=>_.GridViewDataCollection);
            gd.SelectedItemsChanged += (s, e) => {
                _vm.SelectedData = gd.SelectedItem as ConstructionViewData;
            };

            gd.Height = 250;

            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell {Binding = Binding.Delegate<ConstructionViewData, string>(r =>r.Name ) }, 
                HeaderText = "Name" 
            });

          
            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell {Binding = Binding.Delegate<ConstructionViewData, string>(r => r.CType)}, 
                HeaderText = "Type" 
            });

            gd.Columns.Add(new GridColumn { 
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.RValue) }, 
                HeaderText = "RValue[m2·K/W]"
            });

            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.RValueIP) },
                HeaderText = "RValue[h·ft2·F/Btu]"
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.UFactor) },
                HeaderText = "UFactor[W/m2·K]"
            });
            gd.Columns.Add(new GridColumn
            {
                DataCell = new TextBoxCell { Binding = Binding.Delegate<ConstructionViewData, string>(r => r.UFactorIP) },
                HeaderText = "UFactor[Btu/h·ft2·F]"
            });
            return gd;
        }


        public RelayCommand OkCommand => new RelayCommand(() =>
        {
            var allItems = _vm.GridViewDataCollection.Select(_ => _.Construction).ToList();
            _vm.UpdateLibSource(allItems);

            var itemsToReturn = allItems;

            if (this._returnSelectedOnly)
            {
                var d = _vm.SelectedData;
                if (d == null)
                {
                    MessageBox.Show(this, "Nothing is selected!");
                    return;
                }
                itemsToReturn = new List<HB.Energy.IConstruction>() { d.Construction };
            }

        
    
            Close(itemsToReturn);
        });



    }
}
