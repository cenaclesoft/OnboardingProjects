using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Manager
{
    internal class HttpClientManager
    {
        private HttpClientManager() { }

        public static HttpClientManager Instance { get; } = new HttpClientManager();

        public static void SomeFunction ()
        {
            
        }
    }
}
