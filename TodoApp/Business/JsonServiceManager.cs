using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TodoApp.Data;

namespace TodoApp.Business
{
    internal class JsonServiceManager
    {
        #region Singleton

        public static JsonServiceManager Instance { get; } = new JsonServiceManager();

        private JsonServiceManager() { }

        #endregion


        public async Task SaveAsync(string path, IEnumerable<TodoItemModel> models)
        {
            var todoItemDtos = models.Select(model => model.ToDto());

            string json = await Task.Run(() => JsonConvert.SerializeObject(todoItemDtos));

            await WriteAsync(path, json);

            await Task.Delay(2000);
        }

        private async Task WriteAsync(string path, string json)
        {
            await Task.Run(() => File.WriteAllText(path, json));
        }

        public async Task<IEnumerable<TodoItemModel>> LoadAsync(string path)
        {
            string json = await Task.Run(() => File.ReadAllText(path));

            var todoItemDtos = await Task.Run(() => JsonConvert.DeserializeObject<List<TodoItemDto>>(json));

            return todoItemDtos.Select(dto => TodoItemModel.From(dto));
        }
    }
}
