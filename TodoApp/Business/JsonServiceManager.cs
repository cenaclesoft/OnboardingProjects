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
            await Task.Run(async () =>
            {
                var todoItemDtos = models.Select(model => model.ToDto());
                string json = JsonConvert.SerializeObject(todoItemDtos);
                await WriteAsync(path, json);
                await Task.Delay(2000);
            });
        }

        public async Task<IEnumerable<TodoItemModel>> LoadAsync(string path)
        {
            var dtos = await Task.Run(() =>
            {
                string json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<TodoItemDto>>(json);
            });

            return dtos.Select(dto => TodoItemModel.From(dto));
        }


        #region Helpers

        private async Task WriteAsync(string path, string json)
        {
            await Task.Run(() => File.WriteAllText(path, json));
        }

        #endregion
    }
}
