using Mite.Common;
using System;
using System.ComponentModel;
using System.Reactive.Subjects;

namespace Mite.WPF.ViewModel.Common
{
    public class IntRangeVm : BindableBase
    {
        private readonly Subject<int> _curValChanged = new Subject<int>();
        public IObservable<int> OnCurValChanged => _curValChanged;

        public IntRangeVm(int min, int max, int cur)
        {
            _curVal = cur;
            _minVal = min;
            _maxVal = max;
        }

        private int _curVal;
        public int CurVal
        {
            get { return _curVal; }
            set
            {
                if ((value > MaxVal) || (value < MinVal))
                    return;
                SetProperty(ref _curVal, value);
                _curValChanged.OnNext(value);
            }
        }

        private int _maxVal;
        public int MaxVal
        {
            get { return _maxVal; }
            set
            {
                SetProperty(ref _maxVal, value);
            }
        }

        private int _minVal;
        public int MinVal
        {
            get { return _minVal; }
            set
            {
                SetProperty(ref _minVal, value);
            }
        }
    }

}
