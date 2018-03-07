using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Mite.Common;
using TT;

namespace Mite.ViewModel.Common
{
    public class GraphVm : BindableBase
    {
        public GraphVm()
        {
            WbImageVm = new WbImageVm();
        }

        public WbImageVm WbImageVm { get; }

        public void SetData(
            double imageWidth,
            double imageHeight,
            IEnumerable<P2V<float,Color>> plotPoints,
            IEnumerable<LS2V<float, Color>> plotLines,
            IEnumerable<RV<float, Color>> filledRects,
            IEnumerable<RV<float, Color>> openRects
        )
        {
            WbImageVm.ImageData = Id.MakeImageData(
                    imageSize: new Sz2<double>(imageWidth, imageHeight), 
                    plotPoints: plotPoints,
                    filledRects: filledRects,
                    openRects: openRects,
                    plotLines: plotLines
                );

            MinX = WbImageVm.ImageData.boundingRect.MinX;
            MinY = WbImageVm.ImageData.boundingRect.MinY;
            MaxX = WbImageVm.ImageData.boundingRect.MaxX;
            MaxY = WbImageVm.ImageData.boundingRect.MaxY;
        }

        public void SetData(
            R<float> boundingRect,
            double imageWidth,
            double imageHeight,
            IEnumerable<P2V<float, Color>> plotPoints,
            IEnumerable<LS2V<float, Color>> plotLines,
            IEnumerable<RV<float, Color>> filledRects,
            IEnumerable<RV<float, Color>> openRects
        )
        {

            WbImageVm.ImageData = new ImageData(
                    boundingRect: boundingRect,
                    plotPoints: plotPoints.ToArray(),
                    filledRects: filledRects.ToArray(),
                    openRects: openRects.ToArray(),
                    plotLines: plotLines.ToArray(),
                    imageSize: new Sz2<double>(imageWidth, imageHeight)
                );

            MinX = boundingRect.MinX;
            MinY = boundingRect.MinY;
            MaxX = boundingRect.MaxX;
            MaxY = boundingRect.MaxY;
        }

        private float _minX;
        public float MinX
        {
            get { return _minX; }
            set
            {
                SetProperty(ref _minX, value);
                MinStrX = value.ToLegendFormatCode();
            }
        }

        private float _minY;
        public float MinY
        {
            get { return _minY; }
            set
            {
                SetProperty(ref _minY, value);
                MinStrY = value.ToLegendFormatCode();
            }
        }

        private float _maxX;
        public float MaxX
        {
            get { return _maxX; }
            set
            {
                SetProperty(ref _maxX, value);
                MaxStrX = value.ToLegendFormatCode();
            }
        }

        private float _maxY;
        public float MaxY
        {
            get { return _maxY; }
            set
            {
                SetProperty(ref _maxY, value);
                MaxStrY = value.ToLegendFormatCode();
            }
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
