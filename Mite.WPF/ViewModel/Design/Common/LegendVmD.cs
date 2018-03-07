using System.Windows.Media;
using Mite.ViewModel.Common;

namespace Mite.ViewModel.Design.Common
{
    public class LegendVmD : LegendVm
    {
        public LegendVmD() : base("minVal", "midVal", "maxVal", Colors.Pink,
            new [] {Colors.Red, Colors.Green, Colors.Blue}, Colors.DeepPink)
        {
        }
    }
}
