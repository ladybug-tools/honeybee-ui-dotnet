using Eto.Forms;
using LadybugDisplaySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using LB = LadybugDisplaySchema;

namespace Honeybee.UI
{
    public class LegendViewModel : ViewModelBase
    {
        private const string _unit = "px"; 
        public string Title
        {
            get => _legendParameter.Title;
            set => Set(() => _legendParameter.Title = value, nameof(Title));
        }

        #region 2D Properties

        public int X2D
        {
            get => (int)GetPxValue(_legend2D.OriginX, 10);
            set => Set(() => _legend2D.OriginX = SetPxValue(value), nameof(X2D));
        }  
   
        public int Y2D
        {
            get => (int)GetPxValue(_legend2D.OriginY, 100);
            set => Set(() => _legend2D.OriginY = SetPxValue(value), nameof(Y2D));
        }
  
        public int W2D
        {
            get => (int)GetPxValue(_legend2D.SegmentWidth, 25);
            set => Set(() => _legend2D.SegmentWidth = SetPxValue(value), nameof(W2D));
        }
        public int H2D
        {
            get => (int)GetPxValue(_legend2D.SegmentHeight, 36);
            set => Set(() => _legend2D.SegmentHeight = SetPxValue( value), nameof(H2D));
        }
        public int TextHeight2D
        {
            get => (int)GetPxValue(_legend2D.TextHeight, 12);
            set => Set(() => _legend2D.TextHeight = SetPxValue( value), nameof(TextHeight2D));
        }

        private bool _noneColorEnabled;

        public bool NoneColorEnabled
        {
            get { return _noneColorEnabled; }
            set { _noneColorEnabled = value; }
        }

        private Eto.Drawing.Color _noneColor = new Eto.Drawing.Color(50,50,50);
        public Eto.Drawing.Color NoneColor
        {
            get => _noneColor;
            set => Set(() => _noneColor = value, nameof(NoneColor));
        }


        #endregion

        #region 3D properties

        public double X3D
        {
            get => GetValue(_legend3D?.BasePlane?.O?.GetElementByIndex(0));
            set => Set(() => _legend3D.BasePlane.O[0] = value, nameof(X3D));
        }

        public double Y3D
        {
            get => GetValue(_legend3D?.BasePlane?.O?.GetElementByIndex(1));
            set => Set(() => _legend3D.BasePlane.O[1] = value, nameof(Y3D));
        }

        public double Z3D
        {
            get => GetValue(_legend3D?.BasePlane?.O?.GetElementByIndex(2));
            set => Set(() => _legend3D.BasePlane.O[2] = value, nameof(Z3D));
        }

        public double W3D
        {
            get => GetValue(_legend3D?.SegmentWidth);
            set => Set(() => _legend3D.SegmentWidth = value, nameof(W3D));
        }
        public double H3D
        {
            get => GetValue(_legend3D?.SegmentHeight);
            set => Set(() => _legend3D.SegmentHeight= value, nameof(H3D));
        }
        public double TextHeight3D
        {
            get => GetValue(_legend3D?.TextHeight);
            set => Set(() => _legend3D.TextHeight = value, nameof(TextHeight3D));
        }

        #endregion

        #region Common Properties

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

        #endregion
        
        public bool IsNumberValues
        {
            get => !_legendParameter.HasOrdinalDictionary;
            //set => Set(() => _legendParameter.StringValues = !value, nameof(IsNumberValues));
        }

        public bool IsHorizontal2D
        {
            get => !_legendParameter.Vertical;
            set { 
                Set(() => _legendParameter.Vertical = !value, nameof(IsHorizontal2D));
                if (value)
                {
                    if (W2D > H2D)
                        return;
                    var _w = W2D;
                    W2D = H2D;
                    H2D = _w;
                }
                else
                {
                    if (W2D < H2D)
                        return;
                    var _w = W2D;
                    W2D = H2D;
                    H2D = _w;
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

        private LB.Legend3DParameters _legend3D => _legendParameter.Properties3d;
        private LB.Legend2DParameters _legend2D => _legendParameter.Properties2d;

        private LB.LegendParameters _legendParameter;
        private Control _control;

        public LegendViewModel(LB.LegendParameters parameter, Control control)
        {
            _control = control;
            _legendParameter = parameter ?? new LB.LegendParameters(50, 100);

            _noneColorEnabled = _legendParameter.HasNoneColor(out var nC);
            if (_noneColorEnabled)
                _noneColor = Eto.Drawing.Color.FromArgb(nC.R, nC.G, nC.B);

            var colors = _legendParameter.ColorsWithDefault.Select(_ => Eto.Drawing.Color.FromArgb(_.R, _.G, _.B));
            colors = colors.Reverse();
            var vd = colors.Select(_ => new ColorDataItem(_));
            GridViewDataCollection = new DataStoreCollection<ColorDataItem>(vd);
        }

        private static double GetValue(double? input, double defaultValue = default)
        {
            return input.GetValueOrDefault(defaultValue);
        }
        private static double GetValue(AnyOf<LB.Default, double> input, double defaultValue = default)
        {
            var value = defaultValue;
            if (input == null || input.Obj is Default)
                value = defaultValue;
            else if(input.Obj is double dd)
                value = dd;

            return value;
        }
        private static double GetPxValue(AnyOf<LB.Default, string> input, double defaultValue = default)
        {
            var value = defaultValue;
            if (input == null || input.Obj is Default)
                value = defaultValue;
            else if (input.Obj is string ss)
            {
                ss = ss.ToLower().Trim();
                if (ss.Contains(_unit))
                {
                    ss = ss.Replace(_unit, "");
                    double.TryParse(ss, out value);
                }
                else value = defaultValue;
            }
            else
                value = defaultValue;

            return value;
        }

        private static string SetPxValue(double value)
        {
            return $"{value}{_unit}";
        }

        public bool Validate()
        {
            var valid = Max >= Min;
            if (!valid)
                Eto.Forms.MessageBox.Show(_control, "Max value has to be smaller than Min value");
         

            if (Max.Equals(Min) && NumSeg >= 2)
            {
                Eto.Forms.MessageBox.Show(_control, "Max equals min value, cannot draw a legend with multiple segment count. Change it to 1!");
                return false;
            }
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
            if (_noneColorEnabled)
                lg = lg.SetNoneColor(new Color(_noneColor.Rb, _noneColor.Gb, _noneColor.Bb));
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

        public void UpdateNoneColor()
        {
            var oldC = this.NoneColor;
            var newC = EditColor(oldC);
            if (newC == oldC)
                return;
            this.NoneColor = newC;

        }

        private Eto.Drawing.Color EditColor(Eto.Drawing.Color cs)
        {
            var dia = new Eto.Forms.ColorDialog();
            dia.Color = cs;
            dia.AllowAlpha = false;
            var res = dia.ShowDialog(_control);
            if (res != DialogResult.Ok)
                return cs;
            return dia.Color;
        }


        public System.Windows.Input.ICommand EditColorsCommand => new RelayCommand<ColorDataItem>((cs) => {
            if (cs == null)
                return;

            var diaColor = EditColor(cs.Color);
            if (diaColor == cs.Color)
                return;

            var row = GridViewDataCollection.IndexOf(cs);
            var updatedItem = new ColorDataItem(diaColor);
            GridViewDataCollection[row] = updatedItem;
            SelectedRow = row;
            
        });

        public System.Windows.Input.ICommand AddColorsCommand => new RelayCommand<List<LB.Color>>((cs) => {
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
