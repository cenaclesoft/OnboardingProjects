using System;
using System.Linq;
using System.Threading.Tasks;
<<<<<<< HEAD
<<<<<<< Updated upstream
=======
using System.Windows;
using System.Windows.Controls;
>>>>>>> Assignment3
using System.Windows.Input;
using System.Windows.Threading;
using TodoApp.Helper;
using TodoApp.Manager;
using TodoApp.Properties;
=======
using Microsoft.Win32;
using TodoApp.Business;
using TodoApp.Data;
using TodoApp.Helper;
>>>>>>> Stashed changes

namespace TodoApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructor

        public MainWindowViewModel()
        {
            TodoList = new TodoCollection();

<<<<<<< Updated upstream
            AddTodoCommand = new RelayCommand(onAddTodo, CanAddTodo);
            DeleteTodoCommand = new RelayCommand(onDeleteTodo);
<<<<<<< HEAD
=======
            InitializeCommands();
>>>>>>> Stashed changes
=======
            SaveOnJsonCommand = new RelayCommandAsync(OnSaveOnJson);
            LoadCommand = new RelayCommandAsync(OnLoad);
            LoadSampleCommand = new RelayCommandAsync(OnLoadSample);
>>>>>>> Assignment3
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
                // feedback:
                // - AddTodoCommand는 빈 값이 아닐때만 레이즈
                // - 근데 그렇게하면 텍스트를 지워서 빈 값이 됐을때 버튼이 비활성화 되지 않음.
                // if (string.IsNullOrWhiteSpace(TodoInput) == false)
                // {
                //     AddTodoCommand.RaiseCanExecuteChanged();
                // }
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

<<<<<<< HEAD
<<<<<<< Updated upstream
        public void onAddTodo()
=======
        private void OnAddTodo() // 컨벤션 수정 on -> On
>>>>>>> Stashed changes
=======
        private void onAddTodo()
>>>>>>> Assignment3
        {
            TodoList.Add(new TodoItem(TodoInput));
            TodoInput = string.Empty;
        }

        // feedback:
        // - 'if-else'문이 부자연스럽다.
        // answer: 
        private bool CanAddTodo()
        {
            if (string.IsNullOrWhiteSpace(TodoInput)) return false;
            return true;
        }


<<<<<<< HEAD
<<<<<<< Updated upstream
        public ICommand DeleteTodoCommand { get; }

        public void onDeleteTodo(object parameter)
=======
        public RelayCommand DeleteTodoCommand { get; private set; }

        private void OnDeleteTodo(object obj) // 컨벤션 수정 on -> On
>>>>>>> Stashed changes
=======
        public RelayCommand DeleteTodoCommand { get; }

        private void onDeleteTodo(object parameter)
>>>>>>> Assignment3
        {
            if (obj is TodoItem item)
            {
                TodoList.Remove(item);
            }
        }

<<<<<<< HEAD
<<<<<<< Updated upstream
=======

        public RelayCommandAsync SaveOnJsonCommand { get; private set; }
=======

        public RelayCommandAsync SaveOnJsonCommand { get; }
>>>>>>> Assignment3

        private async Task OnSaveOnJson(object parameter)
        {
            string path = OpenSaveFileDialog();
            if (path == null)
            {
                return;
            }

            try
            {
<<<<<<< HEAD
                var models = TodoList.Select(item => new TodoItemModel(item.IsCompleted, item.TodoContent));
                
                await JsonServiceManager.Instance.SaveAsync(path, models);

=======
                // 실제 저장 후 2초 인위 지연 (요구사항)
                await JsonServiceManager.Instance.SaveAsync(path, TodoList);
                // 저장 완료
>>>>>>> Assignment3
                StatusMessage = "저장 완료!";
                TitleMessage = $"TODO App - {path}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"저장 실패: {ex.Message}";
            }

<<<<<<< HEAD
            // 3초 지연 후 상태메시지 초기화 (요구사항)
=======
            // 3초 지연 후 상태메시지 초기화
>>>>>>> Assignment3
            await Task.Delay(3000);
            StatusMessage = "";
        }


<<<<<<< HEAD
        public RelayCommandAsync LoadCommand { get; private set; }

        private async Task OnLoad(object parameter)
        {
            var path = OpenLoadFileDialog(); // *
=======
        public RelayCommandAsync LoadCommand { get; }

        private async Task OnLoad(object parameter)
        {
            string path = OpenLoadFileDialog();

>>>>>>> Assignment3
            if (path == null)
            {
                return;
            }

<<<<<<< HEAD
            try
            {
                // feedback:
                // - TodoList.Clear를 먼저 수행한다.
                TodoList.Clear();
                
                // var를 사용
                var models = await JsonServiceManager.Instance.LoadAsync(path);

                foreach (var model in models)
                {
                    TodoList.Add(new TodoItem(model.IsCompleted, model.TodoContent));
=======
            TodoCollection loaded;

            try
            {
                loaded = await JsonServiceManager.Instance.LoadAsync(path);

                TodoList.Clear();
                foreach (var item in loaded)
                {
                    TodoList.Add(item);
>>>>>>> Assignment3
                }

                StatusMessage = "불러오기 완료!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"불러오기 실패: {ex.Message}";
            }
        }

<<<<<<< HEAD
        public RelayCommandAsync LoadSampleCommand { get; private set; }
=======
        public RelayCommandAsync LoadSampleCommand { get; }
>>>>>>> Assignment3

        private async Task OnLoadSample(object parameter)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "서버에서 가져오는 중...";

<<<<<<< HEAD
                var models = await HttpClientManager.Instance.GetSampleAsync();

                if (models == null)
=======
                TodoCollection loaded = await HttpClientManager.Instance.GetSampleAsync();

                if (loaded == null)
>>>>>>> Assignment3
                {
                    StatusMessage = "Json 파싱 실패";
                    return;
                }

                TodoList.Clear();

<<<<<<< HEAD
                foreach (var model in models)
                {
                    TodoList.Add(new TodoItem(model.IsCompleted, model.TodoContent));
=======
                foreach (var item in loaded)
                {
                    TodoList.Add(item);
>>>>>>> Assignment3
                }

                StatusMessage = "5건 추가 완료";

                // 1. 방식 A : Task.Run 내부에서 직접 컬렉션에 아이템을 추가할 경우,
                // 이 형식의 CollectionView에서는 발송자 스레드와 다른 스레드에서의 SourceCollection에 대한 변경내용을
                // 지원하지 않는다는 오류를 띄우며 정상적인 UI 갱신이 이루어지지 않는다.
                // 
                // await Task.Run(() =>
                // {
                //     foreach (var item in loaded)
                //     {
                //         TodoList.Add(item);
                //     }
                // });


                // 2. 방식 B: Task.Run 내부에서 기존 컬렉션을 수정하는 것은 잘못된 동작이지만,
                // Dispatcher를 통해 큐에 직접 작업을 등록하는 형식으로 수행하면 DispatcherObejct 수정이 가능하다.

                // await Task.Run(() =>
                // {
                //     Dispatcher dispatcher = Application.Current.Dispatcher;

                //     Action updateAction = () =>
                //     {
                //         foreach (var item in loaded)
                //         {
                //             TodoList.Add(item);
                //         }
                //     };

                //     dispatcher.Invoke(updateAction);
                // });


                // 3. 방식 C : 기존 async/await를 이용하는 방식
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                StatusMessage = $"네트워크 오류 : {ex.Message}"; // 빈칸 X
=======
                StatusMessage = $"네트워크 오류 : {ex.Message}";
>>>>>>> Assignment3
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

<<<<<<< HEAD
>>>>>>> Stashed changes
        #endregion


        #region Life Cycle

        // feedback: 초기화는 하나로 모아서 관리
        private void InitializeCommands()
        {
            AddTodoCommand = new RelayCommand(OnAddTodo, _ => CanAddTodo());
            DeleteTodoCommand = new RelayCommand(OnDeleteTodo);
            SaveOnJsonCommand = new RelayCommandAsync(OnSaveOnJson);
            LoadCommand = new RelayCommandAsync(OnLoad);
            LoadSampleCommand = new RelayCommandAsync(OnLoadSample);
        }

=======
>>>>>>> Assignment3
        #endregion
    }
}
