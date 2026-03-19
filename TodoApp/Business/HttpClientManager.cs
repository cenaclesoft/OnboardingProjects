using System;
using System.Collections.Generic;
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


        public async Task<IEnumerable<TodoItemModel>> GetSampleAsync(Uri uri)
        {
            return await Task.Run(async () =>
            {
                var response = await _httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode == false)
                {
                    throw new HttpRequestException($"서버 응답 오류: {(int)response.StatusCode} {response.ReasonPhrase}");
                }

                var json = await response.Content.ReadAsStringAsync();

                var dtos = JsonConvert.DeserializeObject<IEnumerable<TodoItemDto>>(json);

                var models = new List<TodoItemModel>();

                foreach (var dto in dtos)
                {
                    models.Add(TodoItemModel.From(dto));
                }

                return models;
            });
        }
    }
}
