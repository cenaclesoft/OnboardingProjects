using AsyncFileDownloader.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoApp.Manager
{
    internal class JsonServiceManager
    {
        public static JsonServiceManager Instance { get; } = new JsonServiceManager();

        private JsonServiceManager() { }

        public async Task SaveAsync(string path, TodoCollection todoList)
        {
            string json = JsonSerializer.Serialize(todoList);
        
            await WriteAsync(path, json);
            
            await Task.Delay(2000);
        }

        private async Task WriteAsync(string path, string json)
        {
            await Task.Run(() => File.WriteAllText(path, json));
        }

        public async Task<TodoCollection> LoadAsync(string path)
        {
            string json = await Task.Run(() => File.ReadAllText(path));

            TodoCollection todoList = JsonSerializer.Deserialize<TodoCollection>(json);

            return todoList;
        }
    }
}
