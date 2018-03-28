using System.Windows.Media;
using Mite.WPF.ViewModel.Common;
using TT;

namespace Mite.WPF.ViewModel.Design.Common
{
    public class GraphLatticeVmD : GraphLatticeVm
    {
        public GraphLatticeVmD() 
            : base(new R<int>(minX: 0, maxX: DataSz2.X, minY: 0, maxY: DataSz2.Y), "Title D", "TitleX D", "TitleY D")
        {

            _testArray = new float[DataSz2.X * DataSz2.Y];
            for (var i = 0; i < DataSz2.Y; i++)
            {
                for (var j = 0; j < DataSz2.X; j++)
                {
                    _testArray[i * DataSz2.X + j] = (2 * j - DataSz2.X) / ((float)(DataSz2.Y * 0.75));
                }
            }

            SetUpdater(UpDato);
        }

        private readonly float[] _testArray;
        private static readonly Sz2<int> DataSz2 = new Sz2<int>(25, 25);

        private object UpDato(P2<int> dataLoc, R<double> imagePatch)
        {
            var offset = dataLoc.X + dataLoc.Y * DataSz2.X;
            var color = ColorSets.GetLegColor(ColorSets.RedBlueSFLeg, _testArray[offset]);
            return new RV<float, Color>(
                minX: (float)imagePatch.MinX,
                maxX: (float)imagePatch.MaxX,
                minY: (float)imagePatch.MinY,
                maxY: (float)imagePatch.MaxY,
                v: color);
        }

    }
}
