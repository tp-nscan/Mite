using System.Windows.Media;
using Mite.WPF.ViewModel.Common;

namespace Mite.WPF.ViewModel.Design.Common
{
    public class LegendVmD : LegendVm
    {
        public LegendVmD() : base("minVal", "midVal", "maxVal", Colors.Pink,
            new [] {Colors.Red, Colors.Green, Colors.Blue}, Colors.DeepPink)
        {
        }
    }
}
