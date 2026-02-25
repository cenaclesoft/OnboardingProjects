namespace TodoApp.ViewModel
{
    public class TodoItem : ViewModelBase
    {
        public TodoItem(string todoContent)
        {
            _isChecked = false;
            _todoContent = todoContent;
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        private string _todoContent;

        public string TodoContent
        {
            get => _todoContent;
        }
    }
}