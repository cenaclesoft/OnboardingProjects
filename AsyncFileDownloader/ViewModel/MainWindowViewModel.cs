using AsyncFileDownloader.Helper;
using AsyncFileDownloader.Manager;
using AsyncFileDownloader.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
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
            DownloadItems = new ObservableCollection<DownloadItem>
            {
                new DownloadItem { Url = Strings.url1 },
                new DownloadItem { Url = Strings.url2 },
                new DownloadItem { Url = Strings.url3 }
            };

            // 2. Command 초기화
            DownloadAllCommand = new RelayCommandAsync(OnDownloadAllAsync, CanDownloadAll);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
        }
        #endregion


        #region Binding Properties

        public ObservableCollection<DownloadItem> DownloadItems { get; }

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
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = Strings.select_file_location;
            saveFileDialog.Filter = Strings.all_files;
            saveFileDialog.FileName = Strings.download_result;

            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }

            string baseFilePath = saveFileDialog.FileName;
            StatusMessage = Strings.download_ongoing;

            var urls = new[] { Strings.url1, Strings.url2, Strings.url3 };

            _cts = new CancellationTokenSource();

            DateTime startTime = DateTime.Now;

            var downloadTasks = new List<Task>();

            for (int i = 0; i < urls.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(urls[i])) continue;
                downloadTasks.Add(
                    FileDownloadManager.DownloadAsync(urls[i], $"{baseFilePath}_{i + 1}...", _cts.Token, DownloadItems[i].ProgressHandler));
            }

            await Task.WhenAll(downloadTasks);

            DateTime endTime = DateTime.Now;

            StatusMessage = StatusMessage = $"{Strings.download_finished} {(endTime - startTime).TotalSeconds:F1}(초)"; ;

            _cts?.Dispose();
            _cts = null;
        }

        private bool CanDownloadAll()
        {
            // TODO: 다운로드 중일때는 잠궈
            return true;
        }

        // TODO: 참조 걸리게 하는거 오름차트 참조
        public ICommand CancelCommand { get; set; }

        private void OnCancel()
        {
            // TODO: 현재 Cancel 클릭 시 오류 발생
            _cts?.Cancel();
            CleanUpDownloadStatus();
        }

        private bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion


        #region Helpers

        private CancellationTokenSource _cts;


        private void CleanUpDownloadStatus()
        {
            // 진행도 초기화
            for (int i = 0; i < DownloadItems.Count; i++)
            {
                DownloadItems[i].Progress = 0f;
            }

            // 상태메시지 (다운로드 취소)
            StatusMessage = Strings.cancel_download;
        }

        #endregion

    }
}
