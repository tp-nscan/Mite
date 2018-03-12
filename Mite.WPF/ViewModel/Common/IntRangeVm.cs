using Mite.Common;
using System;
using System.Reactive.Subjects;
using System.Windows.Input;

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
            get => _curVal;
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
            get => _maxVal;
            set => SetProperty(ref _maxVal, value);
        }

        private int _minVal;
        public int MinVal
        {
            get => _minVal;
            set => SetProperty(ref _minVal, value);
        }

        #region IncreaseCommand

        RelayCommand _increaseCommand;

        public ICommand IncreaseCommand => _increaseCommand ?? (_increaseCommand = new RelayCommand(
            DoIncrease,
            CanIncrease
            ));

        private void DoIncrease()
        {
            CurVal = CurVal + 1;
        }

        bool CanIncrease()
        {
            return true; // CurVal < MaxVal;
        }

        #endregion // IncreaseCommand



        #region DecreaseCommand

        RelayCommand _decreaseCommand;

        public ICommand DecreaseCommand => _decreaseCommand ?? (_decreaseCommand = new RelayCommand(
                                               DoDecrease,
                                               CanDecrease
        ));

        private void DoDecrease()
        {
            CurVal = CurVal - 1;
        }

        bool CanDecrease()
        {
            return true; // CurVal > MinVal;
        }

        #endregion // DecreaseCommand

        }
}
