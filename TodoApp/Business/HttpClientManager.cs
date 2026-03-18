using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TodoApp.Data;

namespace TodoApp.Business
{
    internal class HttpClientManager
    {
        #region Singleton

        public static HttpClientManager Instance { get; } = new HttpClientManager();

        private HttpClientManager()
        {
            _httpClient = new HttpClient();
        }

        #endregion


        private HttpClient _httpClient;


        public async Task<IEnumerable<TodoItemModel>> GetSampleAsync()
        {
            var uri = new Uri("https://jsonplaceholder.typicode.com/todos?_limit=5");

            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var dtos = await Task.Run(() => JsonConvert.DeserializeObject<List<TodoItemDto>>(json));

            return dtos.Select(dto => TodoItemModel.From(dto));
        }
    }
}
