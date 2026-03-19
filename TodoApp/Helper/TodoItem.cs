using TodoApp.Data;

namespace TodoApp.ViewModel
{
    public class TodoItem : ViewModelBase
    {
        public TodoItem(string todoContent)
        {
            _isCompleted = false;
            _todoContent = todoContent;
        }

        public TodoItem(TodoItemModel model)
        {
            _isCompleted = model.IsCompleted;
            _todoContent = model.TodoContent;
        }

        public TodoItemModel ToModel()
        {
            return new TodoItemModel(IsCompleted, TodoContent);
        }

        private bool _isCompleted;

        public bool IsCompleted
        {
            get => _isCompleted;
            set => SetProperty(ref _isCompleted, value);
        }

        private string _todoContent;

        public string TodoContent
        {
            get => _todoContent;
            set => SetProperty(ref _todoContent, value);
        }
    }
}