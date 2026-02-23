using AsyncFileDownloader.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncFileDownloader.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            HttpRequestUrl1 = Strings.url1;
            HttpRequestUrl2 = Strings.url2;
            HttpRequestUrl3 = Strings.url3;
            StatusMessage = "";
        }

        private string _httpRequestUrl1;
        public string HttpRequestUrl1 
        { 
            get => _httpRequestUrl1; 
            set => SetProperty<string>(ref _httpRequestUrl1, value); 
        }

        private string _httpRequestUrl2;
        public string HttpRequestUrl2 
        { 
            get => _httpRequestUrl2; 
            set => SetProperty<string>(ref _httpRequestUrl2, value); 
        }

        private string _httpRequestUrl3;
        public string HttpRequestUrl3 
        { 
            get => _httpRequestUrl3; 
            set => SetProperty<string>(ref _httpRequestUrl3, value); 
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            private set => SetProperty<string>(ref _statusMessage, value);
        }
    }
    public enum EDownloadStatus { };

}
