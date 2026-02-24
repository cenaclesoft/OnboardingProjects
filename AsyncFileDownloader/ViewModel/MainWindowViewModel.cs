using AsyncFileDownloader.Helper;
using AsyncFileDownloader.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AsyncFileDownloader.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructor
        public MainWindowViewModel()
        {
            // 1. Binding Properties 초기값 설정
            HttpRequestUrl1 = Strings.url1;
            HttpRequestUrl2 = Strings.url2;
            HttpRequestUrl3 = Strings.url3;
            StatusMessage = "";

            // 2. Command 초기화
            DownloadAllCommand = new RelayCommandAsync(OnDownloadAll, CanDownloadAll);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
        }
        #endregion

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

        private double _httpRequestProgress1;
        public double HttpRequestProgress1
        {
            get => _httpRequestProgress1;
            private set => SetProperty(ref _httpRequestProgress1, value);
        }

        private double _httpRequestProgress2;
        public double HttpRequestProgress2
        {
            get => _httpRequestProgress2;
            private set => SetProperty(ref _httpRequestProgress2, value);
        }

        private double _httpRequestProgress3;
        public double HttpRequestProgress3
        {
            get => _httpRequestProgress3;
            private set => SetProperty(ref _httpRequestProgress3, value);
        }

        private double _fileSizeUrl1;
        public double FileSizeUrl1
        {
            get => _httpRequestProgress1;
            private set => SetProperty(ref _httpRequestProgress1, value);
        }

        private double _fileSizeUrl2;
        public double FileSizeUrl2
        {
            get => _fileSizeUrl2;
            private set => SetProperty(ref _fileSizeUrl2, value);
        }
        private double _fileSizeUrl3;
        public double FileSizeUrl3
        {
            get => _fileSizeUrl3;
            private set => SetProperty(ref _fileSizeUrl3, value);
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

        private CancellationTokenSource _cts;

        // TODO : 현재 하드코딩된 String 리소스 관리해야됨
        private async void SaveFile(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "파일 저장 위치 선택";
            saveFileDialog.Filter = "모든 파일 (*.*)|*.*"; // 파일 필터
            saveFileDialog.FileName = "DownloadResult"; // 기본 파일 이름인데 여기에 인덱스 붙혀서 파일저장

            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }

            string baseFilePath = saveFileDialog.FileName;
            StatusMessage = "다운로드 중";

            try
            {
                var downloadTasks = new List<Task>();
                var urls = new[] { HttpRequestUrl1, HttpRequestUrl2, HttpRequestUrl3 };

                _cts = new CancellationTokenSource();

                downloadTasks.AddRange(urls.Select((url, index) =>
                    DownloadWithProgressAsync(url, $"{baseFilePath}_{index + 1}{Path.GetExtension(url)}", index, _cts.Token)));

                await Task.WhenAll(downloadTasks);
                _cts.Dispose();
                _cts = null;

                StatusMessage = "모든 다운로드가 완료되었습니다!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"오류 발생: {ex.Message}";
            }
        }

        private async Task DownloadWithProgressAsync(string url, string destinationPath, int index, CancellationToken token)
        {
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                if (index == 0) FileSizeUrl1 = totalBytes;
                else if (index == 1) FileSizeUrl2 = totalBytes;
                else if (index == 2) FileSizeUrl3 = totalBytes;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    var buffer = new byte[4096]; // 4KB
                    long totalReadBytes = 0;
                    int readBytes;

                    while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, readBytes, token);
                        totalReadBytes += readBytes;

                        if (totalBytes != -1)
                        {
                            // TODO : 여기 좀 이상? 
                            // 배열로 바꿀 수 있는데 배열 Binding 어떻게 하는지 모르겠음.
                            // SetProperty 하나 바뀌면 event로 상위 프로퍼티에 전파시켜야 할 것 같은데 일단 TODO
                            double progress = (double)totalReadBytes / totalBytes * 100;

                            if (index == 0) HttpRequestProgress1 = progress;
                            else if (index == 1) HttpRequestProgress2 = progress;
                            else if (index == 2) HttpRequestProgress3 = progress;
                        }

                        token.ThrowIfCancellationRequested();
                    }
                }
            }
        }
        private void CleanUpDownloadStatus()
        {
            HttpRequestProgress1 = 0;
            HttpRequestProgress2 = 0;
            HttpRequestProgress3 = 0;

            FileSizeUrl1 = 0;
            FileSizeUrl2 = 0;
            FileSizeUrl3 = 0;
        }
        #endregion

        #region Commands
        public ICommand DownloadAllCommand { get; set; }
        private async Task OnDownloadAll()
        {
            SaveFile(null);
        }

        private bool CanDownloadAll()
        {
            if (string.IsNullOrWhiteSpace(HttpRequestUrl1) ||
                string.IsNullOrWhiteSpace(HttpRequestUrl2) ||
                string.IsNullOrWhiteSpace(HttpRequestUrl3))
            {
                return false;
            }

            return true;
        }

        public ICommand CancelCommand { get; set; }
        private void OnCancel()
        {
            _cts?.Cancel();
            CleanUpDownloadStatus();
        }

        private bool CanCancel(object parameter)
        {
            return true;
        }
        #endregion
    }
}
