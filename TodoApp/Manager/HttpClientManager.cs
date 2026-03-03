using AsyncFileDownloader.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TodoApp.Manager
{
    internal class HttpClientManager
    {
        private HttpClient _httpClient;

        private HttpClientManager()
        {
            _httpClient = new HttpClient();
        }

        public static HttpClientManager Instance { get; } = new HttpClientManager();

        public async Task<TodoCollection> GetSampleAsync()
        {
            string path = "https://jsonplaceholder.typicode.com/todos?_limit=5";

            Uri uri = new Uri(path);

            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TodoCollection>(json);
        }
    }
}
