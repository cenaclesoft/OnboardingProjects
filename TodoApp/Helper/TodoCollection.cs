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

        // feedback: Depth가 너무 길다.
        // private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        // {
        //     if (sender is TodoItem item)
        //     {
        //         if (e.PropertyName == nameof(TodoItem.IsCompleted))
        //         {
        //             if (item.IsCompleted)
        //             {
        //                 ++CompletedCount;
        //             }
        //             else
        //             {
        //                 if (CompletedCount > 0)
        //                 {
        //                     --CompletedCount;
        //                 }
        //             }
        //         }
        //     }
        // }
        // answer: 
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
<<<<<<< Updated upstream
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TodoItem.IsCompleted))
            {
                if (sender is TodoItem item)
                {
                    if (item.IsCompleted)
                    {
                        ++CompletedCount;
                    }
                    else
                    {
                        if (CompletedCount > 0) 
                            --CompletedCount;
                    }
                }
            }
        }
=======

            ++TotalCount;

            if (item.IsCompleted)
            {
                ++CompletedCount;
            }
        }

>>>>>>> Stashed changes

        protected override void RemoveItem(int index)
        {
            var itemToRemove = this[index];

            itemToRemove.PropertyChanged -= OnItemPropertyChanged;
<<<<<<< Updated upstream
=======

            --TotalCount;

>>>>>>> Stashed changes
            if (itemToRemove.IsCompleted && (CompletedCount > 0))
            {
                --CompletedCount;
            }
<<<<<<< Updated upstream
            base.RemoveItem(index);
        }

=======

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

>>>>>>> Stashed changes
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
