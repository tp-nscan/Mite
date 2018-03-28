using System.Windows.Media;
using Mite.WPF.Common;
using TT;

namespace Mite.WPF.ViewModel.Common
{
    public class TileVm : BindableBase
    {

        private R<float> _boundingRect;
        public R<float> BoundingRect
        {
            get => _boundingRect;
            set
            {
                SetProperty(ref _boundingRect, value);
                Width = value.MaxX - value.MinX;
                Height = value.MaxY - value.MinY;
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private double _height;
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        private string _textA;
        public string TextA
        {
            get => _textA;
            set => SetProperty(ref _textA, value);
        }
        private string _textB;
        public string TextB
        {
            get => _textB;
            set => SetProperty(ref _textB, value);
        }

        private string _textC;
        public string TextC
        {
            get => _textC;
            set => SetProperty(ref _textC, value);
        }
    }
}
