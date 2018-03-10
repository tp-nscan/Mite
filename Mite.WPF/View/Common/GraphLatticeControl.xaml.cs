
using Mite.WPF.ViewModel.Common;

namespace Mite.View.Common
{
    public sealed partial class GraphLatticeControl
    {
        public GraphLatticeControl()
        {
            InitializeComponent();
        }

        private void wbImage_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            var dc = DataContext as GraphLatticeVm;
            if (dc != null)
            {
                dc.ImageSize = new TT.Sz2<double>(e.NewSize.Width, e.NewSize.Height);
            }
        }
    }
}
