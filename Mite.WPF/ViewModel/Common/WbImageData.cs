//using System.Collections.Generic;
//using System.Windows.Media;
//using Mite.Common;
//using TT;

//namespace Mite.ViewModel.Core
//{
//    public class WbImageData : BindableBase
//    {
//        public WbImageData(
//            R<float> boundingRect,
//            List<P2V<float, Color>> plotPoints,
//            List<RV<float, Color>> filledRects,
//            List<RV<float, Color>> openRects,
//            List<LS2V<float, Color>> plotLines,
//            double imageWidth,
//            double imageHeight)
//        {
//            BoundingRect = boundingRect;
//            ImageHeight = imageHeight;
//            ImageWidth = imageWidth;
//            PlotPoints = plotPoints;
//            FilledRectangles = filledRects;
//            OpenRectangles = openRects;
//            PlotLines = plotLines;
//        }

//        public WbImageData(
//            List<P2V<float, Color>> plotPoints,
//            List<RV<float, Color>> filledRects,
//            List<RV<float, Color>> openRects,
//            List<LS2V<float, Color>> plotLines, 
//            double imageWidth, 
//            double imageHeight)
//        {
//            ImageHeight = imageHeight;
//            ImageWidth = imageWidth;
//            PlotPoints = plotPoints;
//            FilledRectangles = filledRects;
//            OpenRectangles = openRects;
//            PlotLines = plotLines;

//            //BoundingRect = RectExt.NegInfRect
//            //    .BoundingRect(plotLines.Select(pl => pl.X1))
//            //    .BoundingRect(plotLines.Select(pl => pl.X2))
//            //    .BoundingRect(plotLines.Select(pl => pl.Y1))
//            //    .BoundingRect(plotLines.Select(pl => pl.Y2))
//            //    .BoundingRect(filledRects.Select(pl => pl.MinX))
//            //    .BoundingRect(filledRects.Select(pl => pl.MaxX))
//            //    .BoundingRect(filledRects.Select(pl => pl.MinY))
//            //    .BoundingRect(filledRects.Select(pl => pl.MaxY))
//            //    .BoundingRect(openRects.Select(pl => pl.MinX))
//            //    .BoundingRect(openRects.Select(pl => pl.MaxX))
//            //    .BoundingRect(openRects.Select(pl => pl.MinY))
//            //    .BoundingRect(openRects.Select(pl => pl.MaxY))
//            //    .BoundingRect(plotPoints.Select(pl => pl.X))
//            //    .BoundingRect(plotPoints.Select(pl => pl.Y));
//        }

//        public R<float> BoundingRect { get; }

//        public double ImageWidth
//        {
//            get;
//            private set;
//        }

//        public double ImageHeight
//        {
//            get;
//            private set;
//        }

//        public List<P2V<float, Color>> PlotPoints
//        {
//            get;
//            private set;
//        }

//        public List<RV<float, Color>> FilledRectangles
//        {
//            get;
//            private set;
//        }

//        public List<RV<float, Color>> OpenRectangles
//        {
//            get;
//            private set;
//        }

//        public List<LS2V<float, Color>> PlotLines
//        {
//            get;
//            private set;
//        }

//    }
//}
