using System;
using System.Collections.Generic;
using System.Windows.Media;
using Mite.Common;
using TT;
using Microsoft.FSharp.Core;

namespace Mite.WPF.ViewModel.Common
{
    public class GraphLatticeVm : BindableBase
    {
        public GraphLatticeVm(R<int> latticeBounds, string title="Title", string titleX = "TitleX", string titleY = "TitleY")
        {
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

        public R<int> LatticeBounds { get; }

        public WbImageVm WbImageVm { get; }

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
    }
}
