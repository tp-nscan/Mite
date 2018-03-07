using System.Collections.Generic;
using Mite.ViewModel.Common;
using TT;

namespace Mite.ViewModel.Design.Common
{
    public class Grid4DVmD : Grid4DVm<float>
    {
        private static int GridStride = 15;
        public Grid4DVmD() : base(Bounds, CursorSt, ColorSets.RedBlueSFLeg, "Test title")
        {
           UpdateData(TestData);
        }

        public static IEnumerable<LS2V<int,float>> TestData 
            => Grid2dCnxn.GradientStar(new Sz2<int>(GridStride, GridStride));

        public static P2<int> Bounds = new P2<int>(GridStride, GridStride);

        public static P2<int> CursorSt = new P2<int>(GridStride, GridStride);

    }
}