using System.Collections.Generic;
using System.Windows.Media;
using Mite.Common;
using TT;
using Microsoft.FSharp.Core;

namespace Mite.WPF.ViewModel.Common
{
    public class GraphVm : BindableBase
    {
        public GraphVm()
        {
            WbImageVm = new WbImageVm();
            GraphData = Id.MakeGraphData(
                    title: "Title",
                    titleX: "titleX",
                    titleY: "titleY",
                    xLabeler: FuncConvert.ToFSharpFunc<float, string>(x => x.ToString()),
                    yLabeler: FuncConvert.ToFSharpFunc<float, string>(x => x.ToString())
                );
        }

        public WbImageVm WbImageVm { get; }

        GraphData GraphData { get; }

        public void SetData(
            IEnumerable<P2V<float,Color>> plotPoints,
            IEnumerable<LS2V<float, Color>> plotLines,
            IEnumerable<RV<float, Color>> filledRects,
            IEnumerable<RV<float, Color>> openRects
        )
        {
            WbImageVm.ImageData = Id.MakeImageData(
                    plotPoints: plotPoints,
                    filledRects: filledRects,
                    openRects: openRects,
                    plotLines: plotLines
                );

            MinStrX = GraphData.xLabeler.Invoke(WbImageVm.ImageData.boundingRect.MinX);
            MinStrY = GraphData.yLabeler.Invoke(WbImageVm.ImageData.boundingRect.MinY);
            MaxStrX = GraphData.xLabeler.Invoke(WbImageVm.ImageData.boundingRect.MaxX);
            MaxStrY = GraphData.yLabeler.Invoke(WbImageVm.ImageData.boundingRect.MaxY);
        }

        public void SetData(
            R<float> boundingRect,
            IEnumerable<P2V<float, Color>> plotPoints,
            IEnumerable<LS2V<float, Color>> plotLines,
            IEnumerable<RV<float, Color>> filledRects,
            IEnumerable<RV<float, Color>> openRects
        )
        {
            WbImageVm.ImageData = Id.MakeImageDataAndClip(
                clipRegion: boundingRect,
                plotPoints: plotPoints,
                filledRects: filledRects,
                openRects: openRects,
                plotLines: plotLines
             );

            MinStrX = GraphData.xLabeler.Invoke(WbImageVm.ImageData.boundingRect.MinX);
            MinStrY = GraphData.yLabeler.Invoke(WbImageVm.ImageData.boundingRect.MinY);
            MaxStrX = GraphData.xLabeler.Invoke(WbImageVm.ImageData.boundingRect.MaxX);
            MaxStrY = GraphData.yLabeler.Invoke(WbImageVm.ImageData.boundingRect.MaxY);
        }

        private string _maxStrX;
        public string MaxStrX
        {
            get { return _maxStrX; }
            set { SetProperty(ref _maxStrX, value); }
        }

        private string _minStrX;
        public string MinStrX
        {
            get { return _minStrX; }
            set { SetProperty(ref _minStrX, value); }
        }

        private string _minStrY;
        public string MinStrY
        {
            get { return _minStrY; }
            set { SetProperty(ref _minStrY, value); }
        }

        private string _maxStrY;
        public string MaxStrY
        {
            get { return _maxStrY; }
            set { SetProperty(ref _maxStrY, value); }
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

        private string _watermark;
        public string Watermark
        {
            get { return _watermark; }
            set { SetProperty(ref _watermark, value); }
        }
    }
}
