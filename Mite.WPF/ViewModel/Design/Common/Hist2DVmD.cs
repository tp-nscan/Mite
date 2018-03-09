using System;
using System.Collections.Generic;
using System.Linq;
using Mite.WPF.ViewModel.Common;
using TT;

namespace Mite.WPF.ViewModel.Design.Common
{
    public class Hist2DvmD : Hist2DVm
    {
        public Hist2DvmD() : base(BinCts, ColorLegT, "Design title")
        {
            UpdateData(TestData());
        }

        public static IEnumerable<P2<float>> TestData()
        { 
            var retVal = GenBT.TestP2N(0.0f, 1.0f, 263, 1000000).ToArray();

            //for (var i = 0; i < 1000; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine($"{retVal[i].X}\t{retVal[i].Y}");
            //}

            return retVal;
        }

        static Sz2<int> BinCts => new Sz2<int>(100,100);

        static R<float> TestBounds => new R<float>(minX:-1.0f, maxX:2.2f, minY:1.2f, maxY:3.4f);

        static Func<int[], ColorLeg<int>> ColorLegT => 
             ColorSets.WcHistLegInts;

    }
}
