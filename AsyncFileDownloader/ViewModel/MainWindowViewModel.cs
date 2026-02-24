using AsyncFileDownloader.Helper;
using AsyncFileDownloader.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AsyncFileDownloader.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            // 1. Binding Properties 초기값 설정
            HttpRequestUrl1 = Strings.url1;
            HttpRequestUrl2 = Strings.url2;
            HttpRequestUrl3 = Strings.url3;
            StatusMessage = "";

            // 2. Command 초기화
            DownloadAllCommand = new RelayCommand(OnDownloadAll, CanDownloadAll);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
        }

        #region Binding Properties
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
        #endregion

        #region Helpers
        private static HttpClient _httpClient = new HttpClient();

        #endregion

        #region Commands
        public ICommand DownloadAllCommand { get; set; }
        private void OnDownloadAll()
        {
            // 명령 실행 시 처리할 로직
            MessageBox.Show("버튼이 클릭되었습니다!(Download All)");
        }

        private bool CanDownloadAll(object parameter)
        {
            // 명령 실행 가능 여부를 반환
            return true;
        }

        public ICommand CancelCommand { get; set; }
        private void OnCancel()
        {
            // 명령 실행 시 처리할 로직
            MessageBox.Show("버튼이 클릭되었습니다! (취소)");
        }

        private bool CanCancel(object parameter)
        {
            // 명령 실행 가능 여부를 반환
            return true;
        }
        #endregion
    }
    public enum EDownloadStatus { };

}
