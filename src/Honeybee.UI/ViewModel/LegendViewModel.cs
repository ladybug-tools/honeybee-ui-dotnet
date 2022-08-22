using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using LB = LadybugDisplaySchema;

namespace Honeybee.UI
{
    public class LegendViewModel : ViewModelBase
    {
        public string Title
        {
            get => _legendParameter.Title;
            set => Set(() => _legendParameter.Title = value, nameof(Title));
        }

        public int X
        {
            get => (int)_legendParameter.X;
            set => Set(() => _legendParameter.BasePlane.O[0] = value, nameof(X));
        }
   
        public int Y
        {
            get => (int)_legendParameter.Y;
            set => Set(() => _legendParameter.BasePlane.O[1] = value, nameof(Y));
        }
  
        public int W
        {
            get => (int)_legendParameter.SegmentWidthValue;
            set => Set(() => _legendParameter.SegmentWidth = value, nameof(W));
        }
        public int H
        {
            get => (int)_legendParameter.SegmentHeightValue;
            set => Set(() => _legendParameter.SegmentHeight = value, nameof(H));
        }
        public int FontHeight
        {
            get => (int)_legendParameter.TextHeightValue;
            set => Set(() => _legendParameter.TextHeight = value, nameof(FontHeight));
        }
        //public Eto.Drawing.Color FontColor
        //{
        //    get => Eto.Drawing.Color.FromArgb(_legendParameter.FontColor.R, _legendParameter.FontColor.G, _legendParameter.FontColor.B) ;
        //    set => Set(() => _legendParameter.FontColor = new HoneybeeSchema.Color(value.Rb, value.Gb, value.Bb), nameof(FontColor));
        //}

        public double Min
        {
            get => _legendParameter.MinValue;
            set => Set(() => _legendParameter.Min = value, nameof(Min));
        }
     
        public double Max
        {
            get => _legendParameter.MaxValue;
            set => Set(() => _legendParameter.Max = value, nameof(Max));
        }
      
        public int NumSeg
        {
            get => _legendParameter.SegmentCountValue;
            set => Set(() => _legendParameter.SegmentCount = value, nameof(NumSeg));
        }
        public int DecimalPlaces
        {
            get => _legendParameter.DecimalCount;
            set => Set(() => _legendParameter.DecimalCount = value, nameof(DecimalPlaces));
        }
        public bool Continuous
        {
            get => _legendParameter.ContinuousLegend;
            set => Set(() => _legendParameter.ContinuousLegend = value, nameof(Continuous));
        }

        public bool IsNumberValues
        {
            get => !_legendParameter.HasOrdinalDictionary;
            //set => Set(() => _legendParameter.StringValues = !value, nameof(IsNumberValues));
        }

        public bool IsHorizontal
        {
            get => !_legendParameter.Vertical;
            set { 
                Set(() => _legendParameter.Vertical = !value, nameof(IsHorizontal));
                if (value)
                {
                    if (W > H)
                        return;
                    var _w = W;
                    W = H;
                    H = _w;
                }
                else
                {
                    if (W < H)
                        return;
                    var _w = W;
                    W = H;
                    H = _w;
                }
            }
        }

        private DataStoreCollection<ColorDataItem> _gridViewDataCollection = new DataStoreCollection<ColorDataItem>();
        public DataStoreCollection<ColorDataItem> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }
        private ColorDataItem _selectedItem;
        public ColorDataItem SelectedItem
        {
            get => _selectedItem;
            set 
            {
                HasColorSelected = value != null;
                Set(() => _selectedItem = value, nameof(SelectedItem)); 
            }
        }
        private bool hasColorSelected;

        public bool HasColorSelected
        {
            get => hasColorSelected;
            set => Set(() => hasColorSelected = value, nameof(HasColorSelected));
        }

        private int _selectedRow;
        public int SelectedRow
        {
            get => _selectedRow;
            set => Set(() => _selectedRow = value, nameof(SelectedRow));
        }

        private LB.LegendParameters _legendParameter;
        private Control _control;

        public LegendViewModel(LB.LegendParameters parameter, Control control)
        {
            _control = control;
            _legendParameter = parameter ?? new LB.LegendParameters(50, 100);
            var colors = _legendParameter.Colors.Select(_ => Eto.Drawing.Color.FromArgb(_.R, _.G, _.B));
            colors = colors.Reverse();
            var vd = colors.Select(_ => new ColorDataItem(_));
            GridViewDataCollection = new DataStoreCollection<ColorDataItem>(vd);
        }
        public bool Validate()
        {
            var valid = Max >= Min;
            if (!valid)
                Eto.Forms.MessageBox.Show(_control, "Max value has to be smaller than Min value");
            return valid;
        }
        private List<LB.Color> GetColors()
        {
            var colors = GridViewDataCollection.Select(_ => _.Color)
            .Select(_ => new LB.Color(_.Rb, _.Gb, _.Bb))
            .ToList();
            colors.Reverse();
            return colors;
        }
        public LB.LegendParameters GetLegend()
        {
            var lg = this._legendParameter;
            lg.Colors = GetColors();
            return lg;

        }

        //public RelayCommand FontColorCommand => new RelayCommand(() =>
        //{
        //    var dia = new Eto.Forms.ColorDialog();
        //    dia.AllowAlpha = false;
        //    dia.Color = FontColor;
        //    var res = dia.ShowDialog(_control);
        //    if (res != DialogResult.Ok)
        //        return;
        //    FontColor = dia.Color;

        //});

        
        public RelayCommand PresetCommand => new RelayCommand(() => {

            var contextMenu = new ContextMenu();
            // User's colors
            var colors = LegendColorSet.GetUserColorSets();
            var user_item = new Eto.Forms.ButtonMenuItem();
            user_item.Text = "User color sets";
            var hasUserColor = false;
            if (colors != null && colors.Any())
            {
                var uItems = colors.Select(_ => {
                    var a_item = new Eto.Forms.ButtonMenuItem();
                    a_item.Text = _.Key;
                    a_item.Command = AddColorsCommand;
                    a_item.CommandParameter = _.Value;
                    return a_item;
                });
                user_item.Items.AddRange(uItems);
                hasUserColor = true;
            }

            // LBT colors
            var csets = LegendColorSet.Presets;
            var menuItems = csets.Select(_ => {
                var a_item = new Eto.Forms.ButtonMenuItem();
                a_item.Text = _.Key;
                a_item.Command = AddColorsCommand;
                a_item.CommandParameter = _.Value;
                return a_item;
            });
            if (hasUserColor)
            {
                var lbt_item = new Eto.Forms.ButtonMenuItem();
                lbt_item.Text = "LBT color sets";
                lbt_item.Items.AddRange(menuItems);
                contextMenu.Items.Add(lbt_item);
                contextMenu.Items.Add(user_item);
            }
            else
            {
                contextMenu.Items.AddRange(menuItems);
            }
            
            contextMenu.Show();
        });

        public System.Windows.Input.ICommand EditColorsCommand => new RelayCommand<ColorDataItem>((cs) => {
            if (cs == null)
                return;

            var dia = new Eto.Forms.ColorDialog();
            dia.Color = cs.Color;
            dia.AllowAlpha = false;
            var res = dia.ShowDialog(_control);
            if (res != DialogResult.Ok)
                return;
        
            var row = GridViewDataCollection.IndexOf(cs);
            var updatedItem = new ColorDataItem(dia.Color);
            GridViewDataCollection[row] = updatedItem;
            SelectedRow = row;
            
        });

        public System.Windows.Input.ICommand AddColorsCommand => new RelayCommand<List<HoneybeeSchema.Color>>((cs) => {
            if (cs == null || !cs.Any())
                return;
            var colors = cs.Select(_ => Eto.Drawing.Color.FromArgb(_.R, _.G, _.B)).ToList();
            colors.Reverse();
            var vd = colors.Select(_ => new ColorDataItem(_));

            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(vd);
        });

     
        public RelayCommand MoveUpCommand => new RelayCommand(() =>
        {
            //var row = gridView.SelectedRow;
            if (!HasColorSelected)
            {
                var msg = "Nothing is selected!";
                Eto.Forms.MessageBox.Show(msg);
                return;
            }

            var row = GridViewDataCollection.IndexOf(SelectedItem);
            if (row == 0 ) // already the first
                return;

            var newRow = row - 1;
            var c = GridViewDataCollection[row];
            GridViewDataCollection.RemoveAt(row);
            GridViewDataCollection.Insert(newRow, c);
            SelectedRow = newRow;
        });

        public RelayCommand MoveDownCommand => new RelayCommand(() =>
        {

            //var row = gridView.SelectedRow;
            if (!HasColorSelected)
            {
                var msg = "Nothing is selected!";
                Eto.Forms.MessageBox.Show(msg);
                return;
            }

            var row = GridViewDataCollection.IndexOf(SelectedItem);
            if (row == GridViewDataCollection.Count -1) // already the last
                return;

            var newRow = row + 1;
            var c = GridViewDataCollection[row];
            GridViewDataCollection.RemoveAt(row);
            GridViewDataCollection.Insert(newRow, c);
            SelectedRow = newRow;

        });

        public RelayCommand AddCommand => new RelayCommand(() =>
        {
            var newRow = HasColorSelected ? GridViewDataCollection.IndexOf(SelectedItem) : GridViewDataCollection.Count - 1;

            var dia = new Eto.Forms.ColorDialog();
            dia.AllowAlpha = false;

            if (HasColorSelected)
                dia.Color = SelectedItem.Color;

            var res = dia.ShowDialog(_control);
            if (res != DialogResult.Ok)
                return;
            var newColor = dia.Color;
            newRow = newRow + 1;
            var newItem = new ColorDataItem(newColor);
            GridViewDataCollection.Insert(newRow, newItem);
            SelectedRow = newRow;
        });

        public RelayCommand RemoveCommand => new RelayCommand(() =>
        {
            if (!HasColorSelected)
            {
                var msg = "Nothing is selected!";
                Eto.Forms.MessageBox.Show(msg);
                return;
            }

            var row = GridViewDataCollection.IndexOf(SelectedItem);
            GridViewDataCollection.RemoveAt(row);
            SelectedRow = row;
        });
        public RelayCommand SaveCommand => new RelayCommand(() =>
        {
            var colors = GetColors();
            var dia = new Dialog_SaveColorSet(colors);
            dia.ShowModal(_control);
        });

        public RelayCommand FlipCommand => new RelayCommand(() =>
        {
            var cs = GridViewDataCollection.ToList();
            cs.Reverse();
            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(cs);
            //gridView.DataStore = cs;
        });



    }

    public class ColorDataItem
    {
        public Eto.Drawing.Color Color { get; set; }
        public string Text { get; set; }
        public ColorDataItem(Eto.Drawing.Color color)
        {
            this.Color = color;
            this.Text = $"{color.Rb}, {color.Gb}, {color.Bb}";
        }

       
    }

    
}
