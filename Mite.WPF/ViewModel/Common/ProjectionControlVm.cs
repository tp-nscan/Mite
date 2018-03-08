using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Mite.Common;
using MathNet.Numerics.LinearAlgebra;
using TT;

namespace Mite.ViewModel.Common
{
    public class ProjectionControlVm : BindableBase
    {
        public ProjectionControlVm()
        {
            XSelectorVm = new GroupSelectorVm {Orientation = Orientation.Horizontal};
            YSelectorVm = new GroupSelectorVm();
            GraphVm = new GraphVm();
        }

        public ProjectionControlVm(IEnumerable<int> keys) : this()
        {
            var keyLst = keys.ToList();
            keyLst.ForEach(k =>
            {
                XSelectorVm.AddItem(k,k.ToString());
                YSelectorVm.AddItem(k, k.ToString());
            });
        }

        private IDisposable _xSelectorSubscr;
        private GroupSelectorVm _xSelectorVm;
        public GroupSelectorVm XSelectorVm
        {
            get { return _xSelectorVm; }
            set
            {
                SetProperty(ref _xSelectorVm, value);
                _xSelectorSubscr?.Dispose();
                _xSelectorSubscr =
                    _xSelectorVm.OnSelectionChanged
                        .Subscribe(sz => UpdateData(StateMatrix, ProjMatrix));
            }
        }

        private IDisposable _ySelectorSubscr;
        private GroupSelectorVm _ySelectorVm;
        public GroupSelectorVm YSelectorVm
        {
            get { return _ySelectorVm; }
            set
            {
                SetProperty(ref _ySelectorVm, value);
                _ySelectorSubscr?.Dispose();
                _ySelectorSubscr =
                    _ySelectorVm.OnSelectionChanged
                        .Subscribe(sz => UpdateData(StateMatrix, ProjMatrix));
            }
        }
        
        private GraphVm _graphVm;
        public GraphVm GraphVm
        {
            get { return _graphVm; }
            set
            {
                SetProperty(ref _graphVm, value);
            }
        }

        public void UpdateData(Matrix<float> state, Matrix<float> proj)
        {
            if(state==null || proj==null) return;
            if (! XSelectorVm.SelectedKeys.Any()) return;
            if (! YSelectorVm.SelectedKeys.Any()) return;
            StateMatrix = state;
            ProjMatrix = proj;
            var ject = MatrixUt.ProjectTo2D
                    (
                        basis: ProjMatrix.Transpose(),
                        states: StateMatrix,
                        dexX: XSelectorVm.SelectedKeys,
                        dexY: YSelectorVm.SelectedKeys
                    );

            GraphVm.SetData(
                plotPoints: new List<P2V<float, Color>>(), 
                plotLines: new List<LS2V<float, Color>>(), 
                filledRects: MakePlotRectangles(points: ject),
                openRects: new List<RV<float, Color>>());

        }

        static List<RV<float, Color>> MakePlotRectangles(
            IEnumerable<P2<float>> points)
        {
                    return
                        points.Select(
                            v => new RV<float, Color>(
                                    minX: v.X,
                                    minY: v.Y,
                                    maxX: v.X + 2.0f,
                                    maxY: v.Y + 2.0f,
                                    v: Colors.Aqua
                                )).ToList();
        }

        public Matrix<float> ProjMatrix { get; private set; }

        public Matrix<float> StateMatrix { get; private set; }

        //public List<P2V<float,float>> Values { get; private set; }


        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _titleX;
        public string TitleX
        {
            get { return _titleX; }
            set { SetProperty(ref _titleX, value); }
        }

        private string _titleY;
        public string TitleY
        {
            get { return _titleY; }
            set { SetProperty(ref _titleY, value); }
        }
    }
}
