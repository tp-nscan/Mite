
using Mite.WPF.ViewModel.Common;

namespace Mite.WPF.View.Common
{
    public sealed partial class GraphLatticeControl
    {
        public GraphLatticeControl()
        {
            InitializeComponent();
        }

        private void wbImage_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (DataContext is GraphLatticeVm dc)
            {
                dc.ImageSize = new TT.Sz2<double>(e.NewSize.Width, e.NewSize.Height);
            }
        }
    }
}
