using System;
using System.Reactive.Subjects;
using System.Windows;
using Mite.Common;
using TT;

namespace Mite.ViewModel.Common
{
    public class WbImageVm : BindableBase
    {
        private readonly Subject<Point> _pointerChanged
            = new Subject<Point>();
        public IObservable<Point> OnPointerChanged => _pointerChanged;

        ImageData _imageData;
        public ImageData ImageData
        {
            get { return _imageData; }
            set { SetProperty(ref _imageData, value); }
        }

        private Point _pointerPosition;
        public Point PointerPosition
        {
            get { return _pointerPosition; }
            set
            {
                SetProperty(ref _pointerPosition, value);
                _pointerChanged.OnNext(value);
            }
        }
    }
}
