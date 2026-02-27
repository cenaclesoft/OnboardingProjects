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
using TodoApp.Manager;
using TodoApp.Properties;

namespace TodoApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        // TODO: 앱 기능 확장
        #region Constructor

        public MainWindowViewModel()
        {
            TodoList = new TodoCollection();

            AddTodoCommand = new RelayCommand(onAddTodo, CanAddTodo);
            DeleteTodoCommand = new RelayCommand(onDeleteTodo);
            SaveOnJsonCommand = new RelayCommandAsync(OnSaveOnJson);
            LoadCommand = new RelayCommandAsync(OnLoad);
        }

        #endregion


        #region Binding Properties

        public TodoCollection TodoList { get; }


        private string _titleMessage;

        public string TitleMessage
        {
            get => _titleMessage;
            set => SetProperty<string>(ref _titleMessage, value);
        }


        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty<string>(ref _statusMessage, value);
        }


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

        private void onAddTodo()
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


        public RelayCommand DeleteTodoCommand { get; }

        private void onDeleteTodo(object parameter)
        {
            if (parameter is TodoItem item)
            {
                TodoList.Remove(item);
            }
        }


        public RelayCommandAsync SaveOnJsonCommand { get; }

        private async Task OnSaveOnJson(object parameter)
        {
            string path = OpenSaveFileDialog();
            if (path == null)
            {
                return;
            }

            try
            {
                // 실제 저장 후 2초 인위 지연 (요구사항)
                await JsonServiceManager.Instance.SaveAsync(path, TodoList);
                // 저장 완료
                StatusMessage = "저장 완료!";
                TitleMessage = $"TODO App - {path}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"저장 실패: {ex.Message}";
            }
            
            // 3초 지연 후 상태메시지 초기화
            await Task.Delay(3000);
            StatusMessage = "";
        }


        public RelayCommandAsync LoadCommand { get; }

        private async Task OnLoad(object parameter)
        {
            string path = OpenLoadFileDialog();
            
            if (path == null)
            {
                return;
            }

            TodoCollection loaded;

            try
            {
                loaded = await JsonServiceManager.Instance.LoadAsync(path);

                TodoList.Clear();
                foreach (var item in loaded)
                {
                    TodoList.Add(item);
                }

                StatusMessage = "불러오기 완료!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"불러오기 실패: {ex.Message}";
            }
        }

        #endregion


        #region Helpers

        private string OpenSaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "파일로 내보내기";
            saveFileDialog.Filter = "JSON files (*.json)|*.json";
            saveFileDialog.FileName = "todos.json";

            if (saveFileDialog.ShowDialog() == false)
            {
                return null;
            }

            StatusMessage = "저장 중...";

            return saveFileDialog.FileName;
        }

        private string OpenLoadFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "파일 불러오기";
            openFileDialog.Filter = "JSON files (*.json)|*.json";
            
            if (openFileDialog.ShowDialog() == false)
            {
                return null;
            }

            StatusMessage = "불러오는 중...";

            return openFileDialog.FileName;
        }

        #endregion
    }
}
