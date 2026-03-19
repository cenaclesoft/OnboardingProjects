using Newtonsoft.Json;

namespace TodoApp.Data
{
    public class TodoItemDto
    {
        public int UserId { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public bool Completed { get; set; }
    }
}
