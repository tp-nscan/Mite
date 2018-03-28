using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Mite.WPF.Common;
using Mite.WPF.ViewModel.Common;

namespace Mite.WPF.ViewModel.Design.Common
{
    public class GroupSelectorVmD : GroupSelectorVm
    {
        public GroupSelectorVmD()
        {
            Items.ForEach(v=>AddItem(v,v.ToString()));
            Orientation = Orientation.Horizontal;
        }

        public static IEnumerable<int> Items => Enumerable.Range(0, 15);
    }
}
