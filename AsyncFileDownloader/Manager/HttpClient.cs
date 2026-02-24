using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncFileDownloader.Manager
{
    public class HttpClient
    {
        private HttpClient() {
            Instance = new HttpClient();
        }

        // why private set?
        public static HttpClient Instance { get; private set; }
    }
}
