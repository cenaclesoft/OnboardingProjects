using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TodoApp.ViewModel;

namespace TodoApp.Helper
{
    public class TodoCollection : ObservableCollection<TodoItem>
    {
        #region Constructor

        public TodoCollection()
        {
            CompletedCount = 0;
            TotalCount = 0;
        }

        public TodoCollection(IEnumerable<TodoItem> items) : base(items)
        {
            CompletedCount = 0;
            TotalCount = Count;
        }
        #endregion


        #region Binding Properties

        private int _completedCount;

        public int CompletedCount
        {
            get => _completedCount;
            private set => SetProperty<int>(ref _completedCount, value);
        }

        private int _totalCount;

        public int TotalCount
        {
            get => _totalCount;
            private set => SetProperty<int>(ref _totalCount, value);
        }

        #endregion


        #region Helpers

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is TodoItem item)) return;
            if (e.PropertyName != nameof(TodoItem.IsCompleted)) return;

            if (item.IsCompleted)
                ++CompletedCount;
            else if (CompletedCount > 0)
                --CompletedCount;
        }


        protected override void InsertItem(int index, TodoItem item)
        {
            base.InsertItem(index, item);

            item.PropertyChanged -= OnItemPropertyChanged;
            item.PropertyChanged += OnItemPropertyChanged;

            ++TotalCount;

            if (item.IsCompleted)
            {
                ++CompletedCount;
            }
        }


        protected override void RemoveItem(int index)
        {
            var itemToRemove = this[index];

            itemToRemove.PropertyChanged -= OnItemPropertyChanged;

            --TotalCount;

            if (itemToRemove.IsCompleted && (CompletedCount > 0))
            {
                --CompletedCount;
            }

            base.RemoveItem(index);
        }


        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }

            base.ClearItems();

            CompletedCount = 0;
            TotalCount = 0;
        }

        protected bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(member, value))
            {
                return false;
            }

            member = value;

            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            return true;
        }

        #endregion
    }
}
