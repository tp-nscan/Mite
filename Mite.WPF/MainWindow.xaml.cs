using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Mite.WPF.ViewModel.Common;

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
            InitLogicStuff3();
        }


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

        float[] _backSlashArray;
        Sz2<int> _dataSz2 = new Sz2<int>(25, 25);

        void InitLogicStuff()
        {

            var dilly = System.IO.Path.GetFullPath("../../../../Debug/Mite.Cpp.dll");
            //Logic.InitializeLibrary(@"C:\Users\tpnsc\source\repos\Mite\Debug\Mite.Cpp.dll");
            Logic.InitializeLibrary(dilly);

            using (var wrapper = new Logic())
            {
                _backSlashArray = wrapper.BackSlashArray(_dataSz2.X, 0, 1).ToArray();
            }

        }

        void InitLogicStuff2()
        {
            _backSlashArray = new float[_dataSz2.X * _dataSz2.Y];

            for (int i=0; i< _dataSz2.Y; i++)
            {
                for (int j = 0; j < _dataSz2.X; j++)
                {
                    _backSlashArray[i * _dataSz2.X + j] = ((float)i) / ((float)_dataSz2.Y);
                }
            }
        }

        void InitLogicStuff3()
        {
            _backSlashArray = new float[_dataSz2.X * _dataSz2.Y];

            for (int i = 0; i < _dataSz2.Y; i++)
            {
                for (int j = 0; j < _dataSz2.X; j++)
                {
                    _backSlashArray[i * _dataSz2.X + j] = ((float)(2 * i - _dataSz2.X)) / ((float)(_dataSz2.Y * 0.75));
                }
            }
        }


        private void Button_Click2(object sender, RoutedEventArgs e)
        {        
            var GraphVm = new GraphVm();
            GraphVm.Title = "Graph Title";
            GraphVm.TitleX = "Axis X Title";
            GraphVm.TitleY = "Axis Y Title";
            Wank.DataContext = GraphVm;

      
            var dataWin = new R<int>(0, 3, 1, 5);
            var displayWin = new R<float>(0, 400, 0, 400);
            var clipWin = new R<float>(0, 300, 0, 400);


            var cf = FuncConvert.ToFSharpFunc<int, Color>(
                x => ColorSets.GetLegColor(ColorSets.RedBlueSFLeg, _backSlashArray[x]));
            var rawst = DesignData.RasterizeArrayToRects(dataWin, displayWin, _dataSz2.Y, cf).ToArray();

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

        }

        private object UpDato(P2<int> dataLoc, R<double> imagePatch)
        {
            var offset = dataLoc.X + dataLoc.Y * _dataSz2.X;
            var color = ColorSets.GetLegColor(ColorSets.RedBlueSFLeg, _backSlashArray[offset]);
            return new RV<float, Color>(
                minX: (float)imagePatch.MinX, 
                maxX: (float)imagePatch.MaxX, 
                minY: (float)imagePatch.MinY,
                maxY: (float)imagePatch.MaxY,
                v:color);
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            var vm = new GraphLatticeVm(new R<int>(minX: 0, maxX: _dataSz2.X, minY: 0, maxY: _dataSz2.Y));
            vm.SetUpdater(new Func<P2<int>, R<double>, object>(UpDato));
            Wank.DataContext = vm;
        }
    }
}
