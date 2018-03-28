using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Mite.WPF.Common;
using TT;

namespace Mite.WPF.ViewModel.Common
{
    public class GraphTilesVm : BindableBase
    {
        public GraphTilesVm(R<int> latticeBounds, string title = "Title", string titleX = "TitleX", string titleY = "TitleY")
        {
           // _imageSize = new Sz2<double>(1.0, 1.0);
           // _tilesVm.ImageData = Id.InitImageData();
            LatticeBounds = latticeBounds;
            MinX = new IntRangeVm(min: LatticeBounds.MinX, max: LatticeBounds.MaxX, cur: LatticeBounds.MinX);
            MinX.OnCurValChanged.Subscribe(v => CurvalChanged());
            MaxX = new IntRangeVm(min: LatticeBounds.MinX, max: LatticeBounds.MaxX, cur: LatticeBounds.MaxX);
            MaxX.OnCurValChanged.Subscribe(v => CurvalChanged());
            MinY = new IntRangeVm(min: LatticeBounds.MinY, max: LatticeBounds.MaxY, cur: LatticeBounds.MinY);
            MinY.OnCurValChanged.Subscribe(v => CurvalChanged());
            MaxY = new IntRangeVm(min: LatticeBounds.MinY, max: LatticeBounds.MaxY, cur: LatticeBounds.MaxY);
            MaxY.OnCurValChanged.Subscribe(v => CurvalChanged());

            Title = title;
            TitleX = titleX;
            TitleY = titleY;
        }

        void CurvalChanged()
        {
            MinX.MaxVal = MaxX.CurVal - 1;
            MaxX.MinVal = MinX.CurVal + 1;
            MinY.MaxVal = MaxY.CurVal - 1;
            MaxY.MinVal = MinY.CurVal + 1;
            Update();
        }

        private Func<P2<int>, R<double>, TileVm> _cellUpdater;
        public Func<P2<int>, R<double>, TileVm> GetCellUpdater()
        {
            return _cellUpdater;
        }

        public void SetUpdater(Func<P2<int>, R<double>, TileVm> value)
        {
            _cellUpdater = value;
            Update();
        }

        void Update()
        {
            if(ImageSize is null) return;

            var cellSize = new Sz2<double>
            (
                x: ImageSize.X / (MaxX.CurVal - MinX.CurVal),
                y: ImageSize.Y / (MaxY.CurVal - MinY.CurVal)
            );

            TileVms.Clear();

            for (var i = MinX.CurVal; i < MaxX.CurVal; i++)
            {
                for (var j = MinY.CurVal; j < MaxY.CurVal; j++)
                {
                    TileVms.Add(_cellUpdater(
                        new P2<int>(x: i, y: j),
                        new R<double>(
                            minX: (i - MinX.CurVal) * cellSize.X,
                            maxX: (i + 1 - MinX.CurVal) * cellSize.X,
                            minY: (j - MinY.CurVal) * cellSize.Y,
                            maxY: (j + 1 - MinY.CurVal) * cellSize.Y)
                        ));
                }
            }
        }

        public R<int> LatticeBounds { get; }

        public ObservableCollection<TileVm> TileVms { get; set; } = new ObservableCollection<TileVm>();

        private IntRangeVm _maxX;
        public IntRangeVm MaxX
        {
            get => _maxX;
            set => SetProperty(ref _maxX, value);
        }

        private IntRangeVm _minX;
        public IntRangeVm MinX
        {
            get => _minX;
            set => SetProperty(ref _minX, value);
        }

        private IntRangeVm _minY;
        public IntRangeVm MinY
        {
            get => _minY;
            set => SetProperty(ref _minY, value);
        }

        private IntRangeVm _maxY;
        public IntRangeVm MaxY
        {
            get => _maxY;
            set => SetProperty(ref _maxY, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _titleX;
        public string TitleX
        {
            get => _titleX;
            set => SetProperty(ref _titleX, value);
        }

        private string _titleY;
        public string TitleY
        {
            get => _titleY;
            set => SetProperty(ref _titleY, value);
        }

        private Sz2<double> _imageSize;
        public Sz2<double> ImageSize
        {
            get => _imageSize;
            set
            {
                SetProperty(ref _imageSize, value);
                Update();
            }
        }
    }
}
