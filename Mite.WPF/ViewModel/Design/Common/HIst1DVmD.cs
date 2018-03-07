using System.Collections.Generic;
using System.Linq;
using Mite.ViewModel.Common;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

namespace Mite.ViewModel.Design.Common
{
    public class Hist1DvmD : Hist1DVm
    {
        public Hist1DvmD() : base(-0.4f, 4.1f, 15)
        {
            UpdateData(TestData());
            GraphVm.Title = "Design Title";
            GraphVm.TitleX = "Design Title X";
            GraphVm.TitleY = "Design Title Y";
        }

        public static IEnumerable<float> TestData()
        {
            var g = new Normal(0.0, 1.0);
            var randy = new MersenneTwister();

            g.RandomSource = randy;
            var dbls = new double[100000];
            g.Samples(dbls);
            return dbls.Select(d => (float) d);
        }
    }
}
