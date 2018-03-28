using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;
using Mite.WPF.Common;

namespace Mite.WPF.ViewModel.Common
{
    public class GroupSelectorVm : BindableBase
    {
        private readonly Subject<ItemSelectorVm> _selectionChanged
            = new Subject<ItemSelectorVm>();
        public IObservable<ItemSelectorVm> OnSelectionChanged => _selectionChanged;

        public GroupSelectorVm()
        {
            _itemSelectors = new ObservableCollection<ItemSelectorVm>();
        }

        private ObservableCollection<ItemSelectorVm> _itemSelectors;
        public ObservableCollection<ItemSelectorVm> ItemSelectors
        {
            get { return _itemSelectors; }
            set { _itemSelectors = value; }
        }

        readonly Dictionary<ItemSelectorVm,IDisposable> _itemSubscr 
                = new Dictionary<ItemSelectorVm, IDisposable>();

        public void AddItem(int key, string title)
        {
            var itemSelector = new ItemSelectorVm(title,key);
            _itemSubscr[itemSelector] = itemSelector.OnSelectionChanged.Subscribe(HandleSelectionChanged);
            _itemSelectors.Add(itemSelector);
        }

        public void RemoveItem(ItemSelectorVm itemSelectorVm)
        {
            _itemSubscr[itemSelectorVm].Dispose();
            _itemSubscr.Remove(itemSelectorVm);
        }

        void HandleSelectionChanged(ItemSelectorVm itemSelector)
        {
            _selectionChanged.OnNext(itemSelector);
        }

        private Orientation _orientation;
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                SetProperty(ref _orientation, value);
            }
        }

        public IEnumerable<int> SelectedKeys
        {
            get
            {
                return ItemSelectors.Where(s => s.IsSelected)
                    .Select(i => i.Key);
            }
        }
    }
}
