using System.Windows;
using System.Windows.Media.Imaging;
using Mite.WPF.Common;
using TT;

namespace Mite.WPF.View.Common
{
    public sealed partial class WbImage
    {
        public WbImage()
        {
            InitializeComponent();
            SizeChanged += WbImage_SizeChanged;
        }

        private void WbImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MakeBitmap();
        }

        public ImageData ImageData
        {
            get => (ImageData) GetValue(ImageDataProperty);
            set => SetValue(ImageDataProperty, value);
        }

        public static readonly DependencyProperty ImageDataProperty =
            DependencyProperty.Register("ImageData", typeof(ImageData), typeof(WbImage),
                new PropertyMetadata(null, OnImageDataChanged));

        private static void OnImageDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageData = (ImageData)e.NewValue;
            if (imageData == null)
            {
                return;
            }

            var wbImage = d as WbImage;

            if (!ReadyToDisplay(wbImage)) return;

            wbImage?.MakeBitmap();
        }

        private static bool ReadyToDisplay(WbImage wbImage)
        {
            return wbImage?.ImageData != null;
        }

        private WriteableBitmap _writeableBmp;
        private void MakeBitmap()
        {
            if ((ActualWidth < 1) || (ActualHeight < 1)) return;

            if (ImageData == null) return;

            _writeableBmp = BitmapFactory.New((int)ActualWidth, (int)ActualHeight);

            //if (ImageData.imageSize.X < 0.0)
            //{
            //    _writeableBmp = BitmapFactory.New((int)ActualWidth, (int)ActualHeight);
            //}
            //else
            //{
            //    _writeableBmp = BitmapFactory.New((int)ImageData.imageSize.X, (int)ImageData.imageSize.Y);
            //}

            RootImage.Source = _writeableBmp;

            using (_writeableBmp.GetBitmapContext())
            {
                XFactor = ActualWidth / ImageData.boundingRect.Width();
                YFactor = ActualHeight / ImageData.boundingRect.Height();
                var minX = ImageData.boundingRect.MinX;
                var minY = ImageData.boundingRect.MinY;

                foreach (var plotRectangle in ImageData.filledRects)
                {
                    _writeableBmp.FillRectangle(
                            XWindow(plotRectangle.MinX, minX),
                            YWindow(plotRectangle.MinY, ActualHeight, minY),
                            XWindow(plotRectangle.MinX + plotRectangle.Width(), minX),
                            YWindow(plotRectangle.MinY + plotRectangle.Height(), ActualHeight, minY),
                            plotRectangle.V
                        );
                }

                foreach (var plotRectangle in ImageData.openRects)
                {
                    _writeableBmp.DrawRectangle(
                            XWindow(plotRectangle.MinX, minX),
                            YWindow(plotRectangle.MinY, ActualHeight, minY),
                            XWindow(plotRectangle.MinX + plotRectangle.Width(), minX),
                            YWindow(plotRectangle.MinY + plotRectangle.Height(), ActualHeight, minY),
                            plotRectangle.V
                        );
                }

                foreach (var plotLine in ImageData.plotLines)
                {
                    _writeableBmp.DrawLineAa(
                        XWindow(plotLine.X1, minX),
                        YWindow(plotLine.Y1, ActualHeight, minY),
                        XWindow(plotLine.X2, minX),
                        YWindow(plotLine.Y2, ActualHeight, minY),
                        plotLine.V
                        );
                }


                foreach (var plotPoint in ImageData.plotPoints)
                {
                    _writeableBmp.FillRectangle(
                        XWindow(plotPoint.X, minX),
                        YWindow(plotPoint.Y, ActualHeight, minY),
                        XWindow(plotPoint.X + 1, minX),
                        YWindow(plotPoint.Y + 1, ActualHeight, minY),
                        plotPoint.V
                        );
                }


            } // Invalidates on exit of using block
        }


        private double XFactor { get; set; }

        private double YFactor { get; set; }

        public int XWindow(double xVal, double minX)
        {
            return (int)((xVal - minX) * XFactor);
        }

        public int YWindow(double yVal, double imageHeight, double minY)
        {
            return (int)(imageHeight - (yVal - minY) * YFactor);
        }

        public Point PointerPosition
        {
            get => (Point)GetValue(PointerPositionProperty);
            set => SetValue(PointerPositionProperty, value);
        }

        public static readonly DependencyProperty PointerPositionProperty =
            DependencyProperty.Register("PointerPosition", typeof(Point), typeof(WbImage), null);

        private void RootImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var s = e.GetPosition(this);
            PointerPosition = new Point
            {
                X = ImageData.boundingRect.MinX + s.X / XFactor,
                Y = ImageData.boundingRect.MaxY - s.Y / YFactor
            };
        }
    }
}
