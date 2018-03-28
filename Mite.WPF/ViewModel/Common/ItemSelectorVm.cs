using System;
using System.Reactive.Subjects;
using Mite.WPF.Common;

namespace Mite.WPF.ViewModel.Common
{
    public class ItemSelectorVm : BindableBase
    {
        private readonly Subject<ItemSelectorVm> _selectionChanged
            = new Subject<ItemSelectorVm>();
        public IObservable<ItemSelectorVm> OnSelectionChanged => _selectionChanged;

        public ItemSelectorVm(string title, int key)
        {
            Title = title;
            Key = key;
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                SetProperty(ref _isSelected, value);
                _selectionChanged.OnNext(this);
            }
        }

        public int Key { get; }
    }
}
