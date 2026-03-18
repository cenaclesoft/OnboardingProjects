namespace TodoApp.ViewModel
{
    public class TodoItem : ViewModelBase
    {
        public TodoItem(string todoContent)
        {
            _isCompleted = false;
            _todoContent = todoContent;
        }

        public TodoItem(bool isCompleted, string todoContent)
        {
            _isCompleted = isCompleted;
            _todoContent = todoContent;
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