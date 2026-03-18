using System;

namespace TodoApp.Data
{
    public class TodoItemModel
    {
        public bool IsCompleted { get; }
        public string TodoContent { get; }

        public TodoItemModel(bool isCompleted, string todoContent)
        {
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
                Completed = IsCompleted,
                Title = TodoContent
            };
        }
    }
}
