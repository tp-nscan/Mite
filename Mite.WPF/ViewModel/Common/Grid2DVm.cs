using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Mite.Common;
using TT;

namespace Mite.ViewModel.Common
{
    public class Grid2DVm<T> : BindableBase
    {
        public Grid2DVm(Sz2<int> strides, ColorLeg<T> colorLeg, string title = "")
        {
            Strides = strides;
            ColorLeg = colorLeg;
            WbImageVm = new WbImageVm();
            Title = title;
        }
        
        private WbImageVm _wbImageVm;
        public WbImageVm WbImageVm
        {
            get { return _wbImageVm; }
            set
            {
                SetProperty(ref _wbImageVm, value);
            }
        }

        public Sz2<int> Strides { get; }

        public ColorLeg<T> ColorLeg { get;}

        public List<P2V<int,T>> Values { get; private set; }

        public void UpdateData(IEnumerable<P2V<int,T>> values)
        {
            if (values == null) return;

            Values = values.ToList();
            WbImageVm.ImageData = Id.MakeImageData(
                    plotPoints: new List<P2V<float, Color>>(), 
                    filledRects: Values.Select(MakeRectangle).ToList(),
                    openRects: new List<RV<float, Color>>(), 
                    plotLines: new List<LS2V<float, Color>>()
                );
        }

        public RV<float, Color> MakeRectangle(P2V<int, T> v)
        {
            return new RV<float, Color>(
                    minX: v.X,
                    minY: v.Y,
                    maxX: v.X + 1.0f,
                    maxY: v.Y + 1.0f,
                    v: ColorSets.GetLegColor(ColorLeg, v.V)
                );
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
    }
}
