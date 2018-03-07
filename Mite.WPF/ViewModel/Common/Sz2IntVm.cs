using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using Mite.Common;
using TT;

namespace Mite.ViewModel.Common
{
    public class Sz2IntVm : BindableBase, IDataErrorInfo
    {
        private readonly Subject<Sz2<int>> _sizeChanged
            = new Subject<Sz2<int>>();
        public IObservable<Sz2<int>> OnSizeChanged => _sizeChanged;

        public Sz2IntVm(Sz2<int> sz2Int)
        {
            Sz2Int = sz2Int;
            _x = Sz2Int.X.ToString();
            _y = Sz2Int.Y.ToString();
        }

        public Sz2<int> Sz2Int { get; private set; }

        private string _x;
        public string X
        {
            get { return _x; }
            set { SetProperty(ref _x, value); }
        }

        private string _y;
        public string Y
        {
            get { return _y; }
            set { SetProperty(ref _y, value); }
        }

        #region UpdateCommand

        RelayCommand _updateCommand;

        public ICommand UpdateCommand => _updateCommand ?? (_updateCommand = new RelayCommand(
            DoUpdate,
            CanUpdate
            ));

        private void DoUpdate()
        {
            Sz2Int = new Sz2<int>(x:int.Parse(_x), y: int.Parse(_y));
            _sizeChanged.OnNext(Sz2Int);
        }

        bool CanUpdate()
        {
            return Error == String.Empty;
        }

        #endregion // UpdateCommand

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "X":
                        return IntGtZ(X);
                    case "Y":
                        return IntGtZ(Y);
                }
                return String.Empty;
            }
        }
        
        public string Error => this["X"] + this["Y"];

        static string IntGtZ(string val)
        {
            int outVal;
            if (Int32.TryParse(val, out outVal))
            {
                return (outVal > 0) ? String.Empty : "must be > 0";
            }
            return "must be an integer";
        }
    }
}
