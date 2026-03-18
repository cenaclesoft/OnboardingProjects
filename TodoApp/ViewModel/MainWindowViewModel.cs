using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using TodoApp.Business;
using TodoApp.Data;
using TodoApp.Helper;

namespace TodoApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructor

        public MainWindowViewModel()
        {
            TodoList = new TodoCollection();

            InitializeCommands();
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

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty<bool>(ref _isBusy, value);
        }

        #endregion


        #region Commands

        public RelayCommand AddTodoCommand { get; private set; }

        private void OnAddTodo()
        {
            TodoList.Add(new TodoItem(TodoInput));
            TodoInput = string.Empty;
        }

        private bool CanAddTodo()
        {
            if (string.IsNullOrWhiteSpace(TodoInput)) return false;
            return true;
        }


        public RelayCommand DeleteTodoCommand { get; private set; }

        private void OnDeleteTodo(object obj)
        {
            if (obj is TodoItem item)
            {
                TodoList.Remove(item);
            }
        }


        public RelayCommandAsync SaveOnJsonCommand { get; private set; }

        private async Task OnSaveOnJson(object parameter)
        {
            string path = OpenSaveFileDialog();
            if (path == null)
            {
                return;
            }

            try
            {
                var models = TodoList.Select(item => new TodoItemModel(item.IsCompleted, item.TodoContent));

                await JsonServiceManager.Instance.SaveAsync(path, models);

                StatusMessage = "저장 완료!";
                TitleMessage = $"TODO App - {path}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"저장 실패: {ex.Message}";
            }

            await Task.Delay(3000);
            StatusMessage = "";
        }


        public RelayCommandAsync LoadCommand { get; private set; }

        private async Task OnLoad(object parameter)
        {
            var path = OpenLoadFileDialog();
            if (path == null)
            {
                return;
            }

            try
            {
                TodoList.Clear();

                var models = await JsonServiceManager.Instance.LoadAsync(path);

                foreach (var model in models)
                {
                    TodoList.Add(new TodoItem(model.IsCompleted, model.TodoContent));
                }

                StatusMessage = "불러오기 완료!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"불러오기 실패: {ex.Message}";
            }
        }

        public RelayCommandAsync LoadSampleCommand { get; private set; }

        private async Task OnLoadSample(object parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "서버에서 가져오는 중...";

                var models = await HttpClientManager.Instance.GetSampleAsync();

                if (models == null)
                {
                    StatusMessage = "Json 파싱 실패";
                    return;
                }

                TodoList.Clear();

                foreach (var model in models)
                {
                    TodoList.Add(new TodoItem(model.IsCompleted, model.TodoContent));
                }

                StatusMessage = "5건 추가 완료";
            }
            catch (Exception ex)
            {
                StatusMessage = $"네트워크 오류 : {ex.Message}";
            }
            finally
            {
                IsBusy = false;
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


        #region Life Cycle

        private void InitializeCommands()
        {
            AddTodoCommand = new RelayCommand(OnAddTodo, _ => CanAddTodo());
            DeleteTodoCommand = new RelayCommand(OnDeleteTodo);
            SaveOnJsonCommand = new RelayCommandAsync(OnSaveOnJson);
            LoadCommand = new RelayCommandAsync(OnLoad);
            LoadSampleCommand = new RelayCommandAsync(OnLoadSample);
        }

        #endregion
    }
}
