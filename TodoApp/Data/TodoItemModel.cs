using System;
using System.Threading;

namespace TodoApp.Data
{
    public class TodoItemModel
    {
        private static int _nextId;
        private const int DefaultUserId = 1;

        public int UserId { get; }
        public int Id { get; }
        public bool IsCompleted { get; }
        public string TodoContent { get; }

        public TodoItemModel(bool isCompleted, string todoContent)
        {
            Id = Interlocked.Increment(ref _nextId);
            UserId = DefaultUserId;
            IsCompleted = isCompleted;
            TodoContent = todoContent;
        }

        public static TodoItemModel From(TodoItemDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            return new TodoItemModel(
                isCompleted: dto.Completed,
                todoContent: dto.Title
            );
        }

        public TodoItemDto ToDto()
        {
            return new TodoItemDto
            {
                UserId = UserId,
                Id = Id,
                Completed = IsCompleted,
                Title = TodoContent
            };
        }
    }
}
