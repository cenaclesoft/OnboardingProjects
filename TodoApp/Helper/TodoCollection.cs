using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TodoApp.ViewModel;

namespace AsyncFileDownloader.Helper
{
    public class TodoCollection : ObservableCollection<TodoItem>
    {
        #region Constructor

        public TodoCollection()
        {
            CompletedCount = 0;
        }

        #endregion


        #region Binding Properties

        private int _completedCount;

        public int CompletedCount
        {
            get => _completedCount;
            private set => SetProperty<int>(ref _completedCount, value);
        }

        #endregion


        #region Helpers

        protected override void InsertItem(int index, TodoItem item)
        {
            base.InsertItem(index, item);
            item.PropertyChanged += OnItemPropertyChanged;
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

        protected override void RemoveItem(int index)
        {
            var itemToRemove = this[index];

            itemToRemove.PropertyChanged -= OnItemPropertyChanged;
            if (itemToRemove.IsCompleted && (CompletedCount > 0))
            {
                --CompletedCount;
            }
            base.RemoveItem(index);
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
