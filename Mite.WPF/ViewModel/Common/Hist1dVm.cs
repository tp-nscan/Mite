using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Mite.Common;
using TT;

namespace Mite.ViewModel.Common
{
    public class Hist1DVm : BindableBase
    {
        public Hist1DVm(float min, float max, int binCount)
        {
            _enforceBounds = true;
            BinCount = binCount;
            GraphVm = new GraphVm();
            MinValue = min;
            MaxValue = max;
        }

        public Hist1DVm(int binCount)
        {
            _enforceBounds = false;
            BinCount = binCount;
            GraphVm = new GraphVm();
        }

        private int _binCount;
        public int BinCount
        {
            get { return _binCount; }
            set
            {
                SetProperty(ref _binCount, value);
                UpdateGraphVm();
            }
        }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }
    
        public List<float> Values { get; private set; }

        private IDisposable _szChangedSubscr;
        private GraphVm _graphVm;
        public GraphVm GraphVm
        {
            get { return _graphVm; }
            set
            {
                SetProperty(ref _graphVm, value);
            }
        }

        private bool _enforceBounds;
        public bool EnforceBounds
        {
            get { return _enforceBounds; }
            set
            {
                _enforceBounds = value;
                UpdateData(Values);
            }
        }

        public void UpdateData(IEnumerable<float> values)
        {
            if (values == null) return;

            if (EnforceBounds)
            {
                Values = values.Where(t => t > MinValue && t < MaxValue).ToList();
            }
            else
            {
                Values = values.ToList();
                MinValue = Values.Min();
                MaxValue = Values.Max();
            }
            UpdateGraphVm();
        }

        private void UpdateGraphVm()
        {
            if (Values==null) return;

            var bins = Histos.Histogram1d(
                min: MinValue, 
                max: MaxValue, 
                vals: Values, 
                binCount: BinCount);

            var maxFreq = bins.Max(p => p.V);
            
            GraphVm.SetData(
                boundingRect: new R<float>(maxY:maxFreq, minX: MinValue, minY: 0.0f, maxX: MaxValue),
                plotPoints: Enumerable.Empty<P2V<float,Color>>(),
                plotLines: Enumerable.Empty<LS2V<float, Color>>(),
                filledRects: MakePlotRectangles(hist: bins),
                openRects: Enumerable.Empty<RV<float, Color>>());
        }


        static IEnumerable<RV<float, Color>> MakePlotRectangles(
            IEnumerable<IV<float,int>> hist)
        {
            return 
                hist.Select(
                    v => new RV<float, Color>(
                            minX: v.Min,
                            minY: 0,
                            maxX: v.Max,
                            maxY: v.V,
                            v: Colors.Aqua
                        ));
        }

    }
}
