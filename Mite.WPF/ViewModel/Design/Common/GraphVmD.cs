using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Mite.ViewModel.Common;
using TT;

namespace Mite.ViewModel.Design.Common
{
    public class GraphVmD : GraphVm
    {
        public GraphVmD()
        {
            SetData(
                plotPoints: PlotPoints, 
                plotLines: Enumerable.Empty<LS2V<float, Color>>(), 
                filledRects: PlotRectangles,
                openRects: Enumerable.Empty<RV<float, Color>>()
                    );

            Title = "Design Title";
            TitleX = "Design Title X";
            TitleY = "Design Title Y";
        }

        static IEnumerable<P2V<float, Color>> PlotPoints
        {
            get
            {
                return ColorSets.RedBlueSpan
                        .ToList()
                        .Select((c, i) => new P2V<float, Color>(
                                            x: i,
                                            y: i,
                                            v: c
                            )
                         );
            }
        }

        static IEnumerable<RV<float, Color>> PlotRectangles
        {
            get
            {
                return
                   ColorSets.RedBlueSpan
                    .ToList()
                    .Select((c, i) => new RV<float, Color>(
                                        minX: 200 - i * 2,
                                        minY: i * 2,
                                        maxX: 200 - i * 2 + RectSize,
                                        maxY: i * 2 + RectSize,
                                        v: c
                        )
                     );
            }
        }

        private const float RectSize = 10.0f;
    }
}
