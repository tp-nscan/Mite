using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Mite.Common;
using TT;

namespace Mite.ViewModel.Common
{
    public class LegendVm : BindableBase
    {
        public LegendVm(string minVal, string midVal, string maxVal, 
            Color minCol, Color[] midColors, Color maxColor)
        {
            WbImageVm = new WbImageVm();
            _minVal = minVal;
            _midVal = midVal;
            _maxVal = maxVal;
            _minCol = minCol;
            _midColors = midColors;
            _maxColor = maxColor;

            WbImageVm.ImageData = Id.MakeImageData(
                imageSize: new Sz2<double>(-1, -1),
                plotPoints: Enumerable.Empty<P2V<float, Color>>(),
                filledRects: PlotRectangles,
                openRects: Enumerable.Empty<RV<float, Color>>(),
                plotLines: Enumerable.Empty<LS2V<float, Color>>()
            );
            var c = Color.FromRgb((byte) 0, (byte) 255, (byte) 255);
            var c2 = Colors.MediumSeaGreen;
        }

        IEnumerable<RV<float, Color>> PlotRectangles
        {
            get
            {
                return
                    MidColors.Select(
                        (c, i) => new RV<float, Color>(
                            minX: i + 1,
                            minY: 0,
                            maxX: i + 2,
                            maxY: 1,
                            v: c)
                        ).Concat(new[]
                        {
                            new RV<float, Color>(
                            minX: 0,
                            minY: 0,
                            maxX: 1,
                            maxY: 1,
                            v: MinCol)
                        }).Concat(new[]
                        {
                            new RV<float, Color>(
                            minX: MidColors.Length + 1,
                            minY: 0,
                            maxX: MidColors.Length + 2,
                            maxY: 1,
                            v: MaxColor)
                        });
            }
        }

        public WbImageVm WbImageVm { get; }

        private string _minVal;
        public string MinVal
        {
            get { return _minVal; }
            set
            {
                SetProperty(ref _minVal, value);
            }
        }

        private string _midVal;
        public string MidVal
        {
            get { return _midVal; }
            set
            {
                SetProperty(ref _midVal, value);
            }
        }

        private string _maxVal;
        public string MaxVal
        {
            get { return _maxVal; }
            set
            {
                SetProperty(ref _maxVal, value);
            }
        }


        private Color _minCol;
        public Color MinCol
        {
            get { return _minCol; }
            set
            {
                SetProperty(ref _minCol, value);
            }
        }

        private Color[] _midColors;
        public Color[] MidColors
        {
            get { return _midColors; }
            set
            {
                SetProperty(ref _midColors, value);
            }
        }

        private Color _maxColor;
        public Color MaxColor
        {
            get { return _maxColor; }
            set
            {
                SetProperty(ref _maxColor, value);
            }
        }
    }
}
