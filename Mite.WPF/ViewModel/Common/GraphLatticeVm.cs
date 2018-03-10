﻿using System;
using System.Linq;
using System.Windows.Media;
using Mite.Common;
using TT;

namespace Mite.WPF.ViewModel.Common
{
    public class GraphLatticeVm : BindableBase
    {
        public GraphLatticeVm(R<int> latticeBounds, string title="Title", string titleX = "TitleX", string titleY = "TitleY")
        {
            _wbImageVm = new WbImageVm();
            _imageSize = new Sz2<double>(1.0, 1.0);
            _wbImageVm.ImageData = Id.InitImageData();
            LatticeBounds = latticeBounds;
            MinX = new IntRangeVm(min:LatticeBounds.MinX, max: LatticeBounds.MaxX, cur: LatticeBounds.MinX);
            MinX.OnCurValChanged.Subscribe(v=>CurvalChanged());
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
            MinX.MaxVal = MaxX.CurVal;
            MaxX.MinVal = MinX.CurVal;
            MinY.MaxVal = MaxY.CurVal;
            MaxY.MinVal = MinY.CurVal;
        }

        private Action<P2<int>, R<double>, ImageData> cellUpdater;
        public Action<P2<int>, R<double>, ImageData> GetCellUpdater()
        {
            return cellUpdater;
        }

        public void SetUpdater(Action<P2<int>, R<double>, ImageData> value)
        {
            cellUpdater = value;
            if(WbImageVm != null) Update();
        }

        void Update()
        {
            var bRect = new R<float>(minX: 0, maxX: (float)ImageSize.X, 
                                     minY: 0, maxY: (float)ImageSize.Y);
            double spanX = MaxX.CurVal - MinX.CurVal;
            double spanY = MaxY.CurVal - MinY.CurVal;
            for(int i = MinX.CurVal; i < MaxX.CurVal; i++)
            {
                for (int j = MinY.CurVal; j < MaxY.CurVal; j++)
                {
                    cellUpdater(new P2<int>(x: i, y: j),
                                new R<double>(
                                        minX: i * ImageSize.X,
                                        maxX: (i + i) * ImageSize.X,
                                        minY: i * ImageSize.X,
                                        maxY: (i + i) * ImageSize.Y
                                       ),
                                       _wbImageVm.ImageData);
                }
            }
        }

        public R<int> LatticeBounds { get; }

        private readonly WbImageVm _wbImageVm;
        public WbImageVm WbImageVm
        {
           get { return _wbImageVm; }
        }

        private IntRangeVm _maxX;
        public IntRangeVm MaxX
        {
            get { return _maxX; }
            set { SetProperty(ref _maxX, value); }
        }

        private IntRangeVm _minX;
        public IntRangeVm MinX
        {
            get { return _minX; }
            set { SetProperty(ref _minX, value); }
        }

        private IntRangeVm _minY;
        public IntRangeVm MinY
        {
            get { return _minY; }
            set { SetProperty(ref _minY, value); }
        }

        private IntRangeVm _maxY;
        public IntRangeVm MaxY
        {
            get { return _maxY; }
            set { SetProperty(ref _maxY, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _titleX;
        public string TitleX
        {
            get { return _titleX; }
            set { SetProperty(ref _titleX, value); }
        }

        private string _titleY;
        public string TitleY
        {
            get { return _titleY; }
            set { SetProperty(ref _titleY, value); }
        }

        private Sz2<double> _imageSize;
        public Sz2<double> ImageSize
        {
            get { return _imageSize; }
            set {
                SetProperty(ref _imageSize, value);
                Update();
            }
        }
    }
}
