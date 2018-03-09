using Mite.WPF.ViewModel.Common;
using TT;

namespace Mite.WPF.ViewModel.Design.Common
{
    public class Grid2DVmD : Grid2DVm<float>
    {
        public Grid2DVmD() : base(TestStrides, ColorSets.RedBlueSFLeg, "Test title")
        {
            UpdateData(DesignData.Grid2DTestData(TestStrides));
        }

        public static Sz2<int> TestStrides = new Sz2<int>(30,15);
        
    }
}