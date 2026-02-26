using AsyncFileDownloader.Helper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TodoApp.Helper;
using TodoApp.Properties;

namespace TodoApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructor

        public MainWindowViewModel()
        {
            TodoList = new TodoCollection();

            AddTodoCommand = new RelayCommand(onAddTodo, CanAddTodo);
            DeleteTodoCommand = new RelayCommand(onDeleteTodo);
        }

        #endregion

        
        #region Binding Properties
        
        public ObservableCollection<TodoItem> TodoList { get; }


        private string _todoInput;
        
        public string TodoInput
        {
            get => _todoInput;
            set
            {
                SetProperty<string>(ref _todoInput, value);
                AddTodoCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region Commands

        public RelayCommand AddTodoCommand { get; }

        public void onAddTodo()
        {
            TodoList.Add(new TodoItem(TodoInput));
            TodoInput = string.Empty;
        }

        private bool CanAddTodo(object parameter)
        {
            if (string.IsNullOrWhiteSpace(TodoInput))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public ICommand DeleteTodoCommand { get; }

        public void onDeleteTodo(object parameter)
        {
            if (parameter is TodoItem item)
            {
                TodoList.Remove(item);
            }
        }

        #endregion
    }
}
