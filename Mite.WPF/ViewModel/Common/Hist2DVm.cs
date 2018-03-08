using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Mite.Common;
using TT;

namespace Mite.ViewModel.Common
{
    public class Hist2DVm : BindableBase
    {
        public Hist2DVm(Sz2<int> binCounts, R<float> bounds,
                        Func<int[], ColorLeg<int>> colorLegger, 
                        string title = "")
        {
            _enforceBounds = true;
            BinCounts = new Sz2IntVm(binCounts);
            Bounds = bounds;
            ColorLegger = colorLegger;
            GraphVm = new GraphVm();
        }

        public Hist2DVm(Sz2<int> binCounts,
                        Func<int[], ColorLeg<int>> colorLegger, 
                        string title = "")
        {
            _enforceBounds = false;
            BinCounts = new Sz2IntVm(binCounts);
            ColorLegger = colorLegger;
            GraphVm = new GraphVm();
        }


        private IDisposable _xSelectorSubscr;
        private Sz2IntVm _binCounts;
        public Sz2IntVm BinCounts
        {
            get { return _binCounts; }
            set
            {
                SetProperty(ref _binCounts, value);
                _xSelectorSubscr?.Dispose();
                _xSelectorSubscr =
                    _binCounts.OnSizeChanged
                        .Subscribe(sz => UpdateGraphVm());
                UpdateGraphVm();
            }
        }

        public Func<int[], ColorLeg<int>> ColorLegger { get; }

        public R<float> Bounds { get; private set; }

        public P2<float>[] Values { get; private set; }

        private LegendVm _legendVm;
        public LegendVm LegendVm
        {
            get { return _legendVm; }
            set
            {
                SetProperty(ref _legendVm, value);
            }
        }

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

        public void UpdateData(IEnumerable<P2<float>> values)
        {
            if (EnforceBounds)
            {
                Values = BT.FilterRP(Bounds, values).ToArray();
            }
            else
            {
                Values = values.ToArray();
                Bounds = BT.BoundRectP2F32(Values);
            }

            UpdateGraphVm();
        }

        public void UpdateGraphVm()
        {
            if (Values == null) return;

            var bins = A2dUt.ToP2V(Histos.Histogram2d(
                bounds: Bounds,
                binCount: BinCounts.Sz2Int,
                vals: Values)).ToArray();

            var counts = bins.Select(b => b.V.V).ToArray();
            var colorLeg = ColorLegger(counts);
            LegendVm = new LegendVm(
                minVal: "<" + colorLeg.minV,
                midVal: ColorSets.GetLegMidVal(colorLeg).ToString(),
                maxVal: ">" + colorLeg.maxV,
                minCol: colorLeg.minC,
                midColors: colorLeg.spanC,
                maxColor: colorLeg.maxC
                );

            GraphVm.Watermark = $"Bins count: [{BinCounts.X}, {BinCounts.Y}]";
            GraphVm.SetData(
                boundingRect: Bounds,
                plotPoints: Enumerable.Empty<P2V<float, Color>>(),
                plotLines: Enumerable.Empty<LS2V<float, Color>>(),
                filledRects: MakePlotRectangles(colorLeg: colorLeg, hist: bins),
                openRects: Enumerable.Empty<RV<float, Color>>());

        }

        private List<RV<float, Color>> MakePlotRectangles(
            ColorLeg<int> colorLeg,
            IEnumerable<P2V<int, RV<float, int>>> hist)
        {
            return hist.Select(
                v => new RV<float, Color>(
                        minX: v.V.MinX,
                        minY: v.V.MinY,
                        maxX: v.V.MaxX,
                        maxY: v.V.MaxY,
                        v: ColorSets.GetLegColor(colorLeg, v.V.V)
                    )).ToList();
        }



    }
}
