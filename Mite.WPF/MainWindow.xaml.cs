using System.Linq;
using System.Windows;
using Mite.WPF.ViewModel.Design.Common;

namespace Mite.WPF
{
    using Cpp.CLI;
    using TT;

    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private float[] _backSlashArray;
        void InitLogicStuff()
        {
            var dataSz = new Sz2<int>(25, 25);
            var dilly = System.IO.Path.GetFullPath("../../../../Debug/Mite.Cpp.dll");
            //Logic.InitializeLibrary(@"C:\Users\tpnsc\source\repos\Mite\Debug\Mite.Cpp.dll");
            Logic.InitializeLibrary(dilly);

            using (var wrapper = new Logic())
            {
                _backSlashArray = wrapper.BackSlashArray(dataSz.X, 0, 1).ToArray();
            }
        }


        private void d1ToXY(object sender, RoutedEventArgs e)
        {
            GraphTilesControl.DataContext = new GraphTilesVmD();
        }


        private void Next_Thing(object sender, RoutedEventArgs e)
        {
            GraphTilesControl.DataContext = new GpuArrayVmD();
        }
    }
}
