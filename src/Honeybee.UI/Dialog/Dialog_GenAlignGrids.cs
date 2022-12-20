using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Display;
using Eto;
using LadybugDisplaySchema;

namespace Honeybee.UI
{
    public class Dialog_GenAlignGrids : Form
    {

        private static Dialog_GenAlignGrids _instance;
        public static Dialog_GenAlignGrids Instance
        {
            get
            {
                _instance = _instance ?? new Dialog_GenAlignGrids();
                return _instance;
            }
        }
        private GenAlignGridsPanelViewModel _vm;

        private Dialog_GenAlignGrids()
        {
            try
            {
                this.Icon = Honeybee.UI.Config.HoneybeeIcon;
                this.Title = $"Generate Alignment Grids - {DialogHelper.PluginName}";
                this.Topmost = true;
                this.Minimizable = false;
                this.Maximizable = false;
                this.ShowInTaskbar = false;
                this.Resizable = false;

                this.Width = 330;

                var layout = new DynamicLayout();
                layout.DefaultPadding = new Eto.Drawing.Padding(5);
                layout.DefaultSpacing = new Eto.Drawing.Size(5, 5);

                _vm = new GenAlignGridsPanelViewModel();

                var directionBtn = new Button() { Text = "Reset" };
                directionBtn.Command = _vm.DirectionCommand;


                var gridSizeNum = new NumericStepper() { DecimalPlaces = 2, MaximumDecimalPlaces = 3, Increment = 0.01, MinValue = 0.001 };
                var distanceTolNum = new NumericStepper() { DecimalPlaces = 2, MaximumDecimalPlaces = 3, Increment = 0.01, MinValue = 0 };
                var angleTolNum = new NumericStepper() { DecimalPlaces = 0, MaximumDecimalPlaces = 1, Increment = 1, MinValue = 0 };
                var keepPercentNum = new NumericStepper() { DecimalPlaces = 0, MaximumDecimalPlaces = 0, Increment = 10, MinValue = 1, MaxValue = 100 };
                var autoZoomCheck = new CheckBox() { Text = "Auto Zoom" };
                var showOriginal = new CheckBox() { Text = "Show Grid" };

                gridSizeNum.ValueBinding.Bind(_vm, _ => _.GridSize);
                distanceTolNum.ValueBinding.Bind(_vm, _ => _.DistanceTol);
                angleTolNum.ValueBinding.Bind(_vm, _ => _.AngleTol);
                keepPercentNum.ValueBinding.Bind(_vm, _ => _.KeepPercent);
                autoZoomCheck.CheckedBinding.Bind(_vm, _ => _.AutoZoom);
                showOriginal.CheckedBinding.Bind(_vm, _ => _.ShowOriginal);

                var totalPts = new Label();
                totalPts.Bind(_ => _.Text, _vm, _ => _.TotalCount);

                //var tolerance = new Label();
                //tolerance.Bind(_ => _.Text, _vm, _ => _.Tolerance);

                //var unit = new Label();
                //unit.Bind(_ => _.Text, _vm, _ => _.Unit);


                //var zoomBtn = new Button() { Text = "Zoom" };
                //zoomBtn.Command = _vm.ZoomToAllCommand;

                var pgList = new GridView();
                pgList.ShowHeader = false;
                pgList.Height = 150;
                pgList.Bind(_ => _.DataStore, _vm, _ => _.GridCollection);
                pgList.Columns.Add(new GridColumn
                {
                    DataCell = new TextBoxCell { Binding = Binding.Delegate<AlignGrid, string>(r => $"{r.ToString()}") }
                });
                pgList.SelectedItemsChanged += (s, e) => _vm.HighlightSelectGrids(pgList.SelectedItems.OfType<AlignGrid>());
                pgList.AllowMultipleSelection = true;

                var previewBtn = new Button() { Text = "Preview" };
                previewBtn.Command = _vm.PreviewCommand;
                var OKBtn = new Button() { Text = "Bake" };
                OKBtn.Command = _vm.BakeCommand;
                //var CancelBtn = new Button() { Text = "Close" };
                //CancelBtn.Command = _vm.CancelCommand;

                layout.AddSeparateRow("Direction:", directionBtn);
                layout.AddSeparateRow("Grid size:", gridSizeNum);
                layout.AddSeparateRow("Merge Distance:", distanceTolNum);
                layout.AddSeparateRow("Angle:", angleTolNum);
                layout.AddSeparateRow("Keep top %:", keepPercentNum);

                layout.AddRow(pgList);
                layout.AddSeparateRow(totalPts, "grid line(s)", previewBtn, null, autoZoomCheck, showOriginal);
                layout.AddSeparateRow(previewBtn, null, OKBtn);

                layout.AddRow(null);

                this.Content = layout;
            }
            catch (Exception e)
            {
                Dialog_Message.Show(e);
            }
            
        }



        public void Setup(
            Func<Vector3D> getDirection,
            Action<IEnumerable<LineSegment3D>> bake,
            Action clear
            )
        {
            _vm.GetDirection = getDirection;
            _vm.Bake = bake;
            _vm.Clear = clear;
        }
        public void Setup(
            Action<IEnumerable<AlignGrid>> showGrids,
            Action<IEnumerable<AlignGrid>, bool> highlightSelected = default,
            Action<IEnumerable<LineSegment3D>> showOriginal = default
            )
        {
            _vm.ShowGrids = showGrids;
            _vm.HighlightSelected = highlightSelected;
            _vm.ShowAllOriginals = showOriginal;
        }

        public void ShowModal(List<LineSegment3D> sourceObjs, double gridSize, Eto.Forms.Window owner = default)
        {
            var o = owner ?? Config.Owner;
            if (!this.Loaded && o != null)
            {
                var c = o.Bounds.Center;
                c.X = c.X - this.Width / 2;
                c.Y = c.Y - 200;
                this.Location = c;
            }

            this.Show();

            _vm.ReCompute(sourceObjs, gridSize);
        }

     
       
        protected override void OnUnLoad(EventArgs e)
        {
            base.OnUnLoad(e);
            _vm.Reset();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance?.Dispose();
            _instance = null;
        }

    }


    internal class GenAlignGridsPanelViewModel : ViewModelBase
    {
        private List<LineSegment3D> _sourceObjs = new List<LineSegment3D>();


        private string _total = "0";
        internal string TotalCount
        {
            get => _total;
            set => Set(value, ref _total);
        }



        private double _distanceTol;
        internal double DistanceTol
        {
            get => _distanceTol;
            set
            {
                if (_distanceTol == value)
                    return;
                Set(Math.Max(value, _gridSize), ref _distanceTol);
            }
        }

        private double _gridSize;
        internal double GridSize
        {
            get => _gridSize;
            set
            {
                if (_gridSize == value)
                    return;
                Set(value, ref _gridSize);
                DistanceTol = value * 2;
            }
        }

        private double _AngleTol;
        internal double AngleTol
        {
            get => _AngleTol;
            set
            {
                if (_AngleTol == value)
                    return;
                Set(value, ref _AngleTol);
            }
        }

        private int _keepPercent;
        internal int KeepPercent
        {
            get => _keepPercent;
            set
            {
                if (_keepPercent == value)
                    return;
                Set(value, ref _keepPercent);
                FilterByPercent();
            }
        }

        private bool _AutoZoom;
        internal bool AutoZoom
        {
            get => _AutoZoom;
            set
            {
                if (_AutoZoom == value)
                    return;
                Set(value, ref _AutoZoom);
            }
        }

        private bool _ShowOriginal;
        internal bool ShowOriginal
        {
            get => _ShowOriginal;
            set
            {
                if (_ShowOriginal == value)
                    return;
                Set(value, ref _ShowOriginal);

                ShowGlobalPlanes();
            }
        }

        private Vector3D RefVector { get; set; }


        //private string _unit = String.Empty;
        //internal string Unit
        //{
        //    get => _unit;
        //    set => Set(value, ref _unit);
        //}

        private List<LineSegment3D> _allOriginalGrids = new List<LineSegment3D>();
        private List<AlignGrid> _allGrids = new List<AlignGrid>();

        private ExtendedObservableCollection<AlignGrid> _gridCollection = new ExtendedObservableCollection<AlignGrid>();
        internal ExtendedObservableCollection<AlignGrid> GridCollection
        {
            get => _gridCollection;
            set => Set(value, ref _gridCollection);
        }

        public static GenAlignGridsPanelViewModel Instance;
        public GenAlignGridsPanelViewModel()
        {
            Instance = this;
        }

        internal Func<Vector3D> GetDirection;
        internal Action Clear;
        internal Action<IEnumerable<AlignGrid>> ShowGrids;
        internal Action<IEnumerable<AlignGrid>, bool> HighlightSelected;
        internal Action<IEnumerable<LineSegment3D>> ShowAllOriginals;
        internal Action<IEnumerable<LineSegment3D>> Bake;
        //internal Action<BoundingBox> ZoomTo;

        internal RelayCommand DirectionCommand => new RelayCommand(() =>
        {
            if (GetDirection == null)
            {
                Dialog_Message.Show("This command is not available on this platform, please contact the developer!");
                return;
            }
            var vec = GetDirection?.Invoke();
            RefVector = vec.Normalize();
            PreviewCommand.Execute(null);

        });

        internal RelayCommand PreviewCommand => new RelayCommand(() =>
        {
            ReCompute();
        });
        internal RelayCommand BakeCommand => new RelayCommand(() =>
        {
            if (Bake == null)
            {
                Dialog_Message.Show("This command is not available on this platform, please contact the developer!");
                return;
            }
            var lines = GridCollection.Select(_ => _.WeightedGridLine);
            Bake?.Invoke(lines);
        });

        internal RelayCommand CancelCommand => new RelayCommand(() =>
        {
            Dialog_GenAlignGrids.Instance?.Close();
        });

        public void Reset()
        {
            //clear
            this._allGrids.Clear();
            this.GridCollection.Clear();
            this.Clear?.Invoke();
            this.TotalCount = "0";
        }

        private void FilterByPercent()
        {
            if (KeepPercent <= 0) return;
            if (_allGrids.Count == 0)
                return;

            var count = (int)this.KeepPercent * _allGrids.Count / 100;
            count = Math.Max(1, count);


            var keptWeight = _allGrids.OrderByDescending(_ => _.WeightPoints.Count).Take(count).Last().WeightPoints.Count;
            GridCollection.Clear();
            GridCollection.AddRange(_allGrids.Where(_ => _.WeightPoints.Count >= keptWeight));

            TotalCount = GridCollection.Count.ToString();

            ShowAllGrids(GridCollection.ToList());
            ShowGlobalPlanes();
        }

        internal void ReCompute()
        {
            if (this._sourceObjs != null && this._sourceObjs.Any())
            {
                var globalPlanes = AlignGrid.GenGlobalPlanes(_sourceObjs, RefVector, GridSize);  
                var grids = AlignGrid.GenGrids(_sourceObjs, globalPlanes, GridSize, AngleTol, DistanceTol);
            
                _allGrids = grids;
                _allOriginalGrids = globalPlanes.Select(_ => AlignGrid.GenGridLine(_)).ToList();

                FilterByPercent();

            }
        }

        internal void ReCompute(List<LineSegment3D> sourceObjs, double gridSize)
        {

            Reset();
            //Unit = unit.ToString();

            RefVector = RefVector ?? Vector3D.YAxis;
            GridSize = GridSize == default ? gridSize : GridSize;
            AngleTol = AngleTol == default ? 1 : AngleTol;
            DistanceTol = DistanceTol == default ? GridSize * 2 : DistanceTol;
            KeepPercent = KeepPercent == default ? 20 : KeepPercent;

            _sourceObjs = sourceObjs;

            ReCompute();
        }

        private void ShowAllGrids(IEnumerable<AlignGrid> grids)
        {
            if (ShowGrids == null)
            {
                Dialog_Message.Show("This command is not available on this platform, please contact the developer!");
                return;
            }
            ShowGrids?.Invoke(grids);
        }

        internal void HighlightSelectGrids(IEnumerable<AlignGrid> grids)
        {
            if (HighlightSelected == null)
            {
                Dialog_Message.Show("This command is not available on this platform, please contact the developer!");
                return;
            }
            HighlightSelected?.Invoke(grids, this.AutoZoom);
        }

        private void ShowGlobalPlanes()
        {
            if (ShowAllOriginals == null)
            {
                Dialog_Message.Show("This command is not available on this platform, please contact the developer!");
                return;
            }

            if (ShowOriginal)
                this.ShowAllOriginals?.Invoke(this._allOriginalGrids);
            else
                this.ShowAllOriginals?.Invoke(null);


        }


    }




}
