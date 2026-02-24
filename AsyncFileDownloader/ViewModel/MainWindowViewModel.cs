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
            DownloadAllCommand = new RelayCommandAsync(OnDownloadAllAsync, CanDownloadAll);
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

        #region Commands
        public ICommand DownloadAllCommand { get; set; }
        private async Task OnDownloadAllAsync()
        {
            await SaveFileAsync(null);
        }

        private bool CanDownloadAll()
        {
            if (string.IsNullOrWhiteSpace(HttpRequestUrl1) || string.IsNullOrWhiteSpace(HttpRequestUrl2) || string.IsNullOrWhiteSpace(HttpRequestUrl3))
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

        #region Helpers
        private static HttpClient _httpClient = new HttpClient();

        private CancellationTokenSource _cts;

        private async Task SaveFileAsync(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = Strings.select_file_location;
            saveFileDialog.Filter = Strings.all_files;
            saveFileDialog.FileName = Strings.download_result; // 기본 파일 이름인데 여기에 인덱스 붙혀서 파일저장

            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }

            string baseFilePath = saveFileDialog.FileName;
            StatusMessage = Strings.download_ongoing;

            try
            {
                DateTime startTime = DateTime.Now; // 시간 측정 시작 --------------

                var downloadTasks = new List<Task>();
                var urls = new[] { HttpRequestUrl1, HttpRequestUrl2, HttpRequestUrl3 };

                _cts = new CancellationTokenSource();

                downloadTasks.AddRange(urls.Select((url, index) =>
                    DownloadWithProgressAsync(url, $"{baseFilePath}_{index + 1}{Path.GetExtension(url)}", index, _cts.Token)));

                await Task.WhenAll(downloadTasks);
                
                DateTime endTime = DateTime.Now; // 시간 측정 끝 ------------------

                StatusMessage = StatusMessage = $"{Strings.download_finished} {(endTime - startTime).TotalSeconds:F1}(초)"; ;
            }
            catch (Exception ex)
            {
                StatusMessage = $"오류 발생: {ex.Message}";
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
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

            StatusMessage = Strings.cancel_download;
        }
        #endregion
        
    }
}
