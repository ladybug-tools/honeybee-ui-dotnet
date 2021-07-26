using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class ConstructionManagerViewModel : ViewModelBase
    {
     
        private DataStoreCollection<ConstructionViewData> _gridViewDataCollection = new DataStoreCollection<ConstructionViewData>();
        internal DataStoreCollection<ConstructionViewData> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        internal ConstructionViewData SelectedData { get; set; }

        private HB.ModelEnergyProperties _modelEnergyProperties { get; set; }
        private Control _control;
    
        public ConstructionManagerViewModel(HB.ModelEnergyProperties libSource, Control control = default)
        {
            ConstructionViewData.LibSource = libSource;
            _control = control;
            _modelEnergyProperties = libSource;

            var constructions = libSource.ConstructionList;
            var data = constructions.Select(_ => new ConstructionViewData(_));
            GridViewDataCollection = new DataStoreCollection<ConstructionViewData>(data);

        }

        public void UpdateLibSource(List<HB.Energy.IConstruction> newItems)
        {
            this._modelEnergyProperties.Constructions.Clear();
            this._modelEnergyProperties.AddConstructions(newItems);
        }

        public ICommand AddOpaqueConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Opaque Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.OpaqueConstructionAbridged(name, new List<string>(), name);

            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                GridViewDataCollection.Add(new ConstructionViewData(dialog_rc));
            }
        });



        public ICommand AddWindowConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Window Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.WindowConstructionAbridged(name, new List<string>(), name);

            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                GridViewDataCollection.Add(new ConstructionViewData(dialog_rc));
            }
        });
        public ICommand AddShadeConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New Shade Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.ShadeConstruction(name, name);

            var dialog = new Honeybee.UI.Dialog_Construction_Shade(newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                GridViewDataCollection.Add(new ConstructionViewData(dialog_rc));
            }
        });

        public ICommand AddAirBoundaryConstructionCommand => new RelayCommand(() => {
            var id = Guid.NewGuid().ToString();
            var name = $"New AirBoundary Construction {id.Substring(0, 5)}";
            var newConstrucion = new HB.AirBoundaryConstructionAbridged(name, name);

            var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(newConstrucion);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                GridViewDataCollection.Add(new ConstructionViewData(dialog_rc));
            }
        });

        public ICommand AddWindowConstructionShadeCommand => new RelayCommand(() => {
            //var id = Guid.NewGuid().ToString();
            //var name = $"New AirBoundary Construction {id.Substring(0, 5)}";
            //var newConstrucion = new HB.WindowConstructionShadeAbridged(name, name);

            //var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(newConstrucion);
            //var dialog_rc = dialog.ShowModal(_control);
            //if (dialog_rc != null)
            //{
            //    GridViewDataCollection.Add(new ConstructionViewData(dialog_rc));
            //}
        });


        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var menuDic = new Dictionary<string, ICommand>()
            {
                { "Opaque", AddOpaqueConstructionCommand},
                { "Window", AddWindowConstructionCommand},
                { "Shade",AddShadeConstructionCommand },
                { "AirBoundary", AddAirBoundaryConstructionCommand}
            };

            var contextMenu = new ContextMenu();
            foreach (var item in menuDic)
            {
                contextMenu.Items.Add(
                  new Eto.Forms.ButtonMenuItem()
                  {
                      Text = item.Key,
                      Command = item.Value
                  });
            }
            contextMenu.Show();
        });

        public RelayCommand DuplicateCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to duplicate!");
                return;
            }

            
            var dup = selected.Construction.Duplicate() as HB.Energy.IConstruction;
            var name = $"{dup.Identifier}_dup";
            dup.Identifier = name;
            dup.DisplayName = name;
            var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, dup);
            var dialog_rc = dialog.ShowModal(_control);
            if (dialog_rc != null)
            {
                GridViewDataCollection.Add(new ConstructionViewData(dialog_rc));
            }
        });

        public RelayCommand EditCommand => new RelayCommand(() =>
        {
            var selected = this.SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }

            var selectedObj = selected.Construction;
            HB.Energy.IConstruction dialog_rc;
            if (selectedObj is HB.ShadeConstruction shd)
            {
                var dup = shd.DuplicateShadeConstruction();
                var dialog = new Honeybee.UI.Dialog_Construction_Shade(dup);
                dialog_rc = dialog.ShowModal(_control);
            }
            else if (selectedObj is HB.AirBoundaryConstructionAbridged airBoundary)
            {
                var dup = airBoundary.DuplicateAirBoundaryConstructionAbridged();
                var dialog = new Honeybee.UI.Dialog_Construction_AirBoundary(dup);
                dialog_rc = dialog.ShowModal(_control);
            }
            else
            {
                // Opaque Construction or Window Construciton
                var dup = selectedObj.Duplicate() as HB.Energy.IConstruction;
                var dialog = new Honeybee.UI.Dialog_Construction(this._modelEnergyProperties, dup);
                dialog_rc = dialog.ShowModal(_control);
            }

            if (dialog_rc == null) return;
            var newItem = new ConstructionViewData(dialog_rc);
            var index = GridViewDataCollection.IndexOf(selected);
            GridViewDataCollection.RemoveAt(index);
            GridViewDataCollection.Insert(index, newItem);


        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            var selected = SelectedData;
            if (selected == null)
            {
                MessageBox.Show(_control, "Nothing is selected to edit!");
                return;
            }

            var res = MessageBox.Show(_control, $"Are you sure you want to delete:\n {selected.Name}", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                GridViewDataCollection.Remove(selected);
            }
        });
    }



    internal class ConstructionViewData
    {
        public string Name { get; }
        public string CType { get; }
        public string RValue { get; }
        public string RValueIP { get; }
        public string UFactor { get; }
        public string UFactorIP { get; }
        public HB.Energy.IConstruction Construction { get; }
        public static HB.ModelEnergyProperties LibSource { get; set; }
        public ConstructionViewData(HB.Energy.IConstruction c)
        {
            this.Name = c.DisplayName ?? c.Identifier;
            this.CType = c.GetType().Name.Replace("Abridged", "").Replace("Construction", "");
            if (c is HB.Energy.IThermalConstruction tc)
            {
                tc.CalThermalValues(LibSource);
                this.RValue = Math.Round(tc.RValue, 5).ToString();
                this.UFactor = Math.Round(tc.UFactor, 5).ToString();

                this.RValueIP = Math.Round(tc.RValue * 5.678263337, 5).ToString();
                this.UFactorIP = Math.Round(tc.UFactor / 5.678263337, 5).ToString();
            }
            this.Construction = c;

        }
    }

}
