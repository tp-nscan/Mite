using System.Collections.Generic;
using System.Linq;
using Mite.Common;
using Mite.ViewModel.Common;

namespace Mite.ViewModel.Design.Common
{
    public class ProjectionControlVmD : ProjectionControlVm
    {
        public ProjectionControlVmD()
        {
            Title = "Design Title";
            TitleX = "Design X";
            TitleY = "Design Y";

            ItemsX.ForEach(v => XSelectorVm.AddItem(v, v.ToString()));
            ItemsY.ForEach(v => YSelectorVm.AddItem(v, v.ToString()));

        }

        public static IEnumerable<int> ItemsX => Enumerable.Range(0, 15);

        public static IEnumerable<int> ItemsY => Enumerable.Range(5, 35);
    }
}
