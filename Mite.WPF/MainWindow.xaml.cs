using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mite.ViewModel.Common;

namespace Mite.WPF
{
    using Cpp.CLI;
    using Microsoft.FSharp.Core;
    using Microsoft.Win32;
    using TT;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GraphVm = new GraphVm();
            GraphVm.Title = "Graph Title";
            GraphVm.TitleX = "Axis X Title";
            GraphVm.TitleY = "Axis Y Title";
            Wank.DataContext = GraphVm;
        }

        static Sz2<int> DataSz2 = new Sz2<int>(5, 5);
        GraphVm GraphVm;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Native Library|Mite.Cpp.dll",
                InitialDirectory = Environment.CurrentDirectory
            };

            var result = fileOpenDialog.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                Logic.InitializeLibrary(fileOpenDialog.FileName);

                using (var wrapper = new Logic())
                {
                    MessageBox.Show("The answer is " + wrapper.Get());
                }
            }
        }


        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            var dilly = System.IO.Path.GetFullPath("../../../../Debug/Mite.Cpp.dll");
            //Logic.InitializeLibrary(@"C:\Users\tpnsc\source\repos\Mite\Debug\Mite.Cpp.dll");
            Logic.InitializeLibrary(dilly);

            using (var wrapper = new Logic())
            {
                var res = wrapper.BackSlashArray(DataSz2.X, 0, 1).ToArray();
                var dataWin = new R<int>(0, 3, 1, 5);
                var displayWin = new R<float>(0, 400, 0, 400);
                var clipWin = new R<float>(0, 300, 0, 400);


                var cf = FuncConvert.ToFSharpFunc<int, Color>(
                    x => ColorSets.GetLegColor(ColorSets.RedBlueSFLeg, res[x]));
                var rawst = DesignData.RasterizeArrayToRects(dataWin, displayWin, DataSz2.Y, cf).ToArray();

                var mid = Id.MakeImageData(
                    plotPoints: Enumerable.Empty<P2V<float, Color>>(),
                    plotLines: Enumerable.Empty<LS2V<float, Color>>(),
                    filledRects: rawst,
                    openRects: Enumerable.Empty<RV<float, Color>>()
                    );

                var rawst2 = Id.ClipImageData(mid, clipWin);

                GraphVm.SetData(
                    plotPoints: Enumerable.Empty<P2V<float, Color>>(),
                    plotLines: Enumerable.Empty<LS2V<float, Color>>(),
                    filledRects: rawst2.filledRects,
                    openRects: Enumerable.Empty<RV<float, Color>>()
                   );


                //MessageBox.Show("The answer is " + wrapper.GetAdd(5));
            }
        }
    }
}
