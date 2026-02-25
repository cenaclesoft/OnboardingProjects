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
            DownloadItems = new ObservableCollection<DownloadItem> {
                new DownloadItem("Link 1", Strings.url1),
                new DownloadItem("Link 2", Strings.url2),
                new DownloadItem("Link 3", Strings.url3)
            };

            DownloadAllCommand = new RelayCommandAsync(OnDownloadAllAsync, CanDownloadAll);
            CancelCommand = new RelayCommand(OnCancel, CanCancel);
        }
        
        #endregion


        #region Binding Properties

        public ObservableCollection<DownloadItem> DownloadItems { get; }

        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
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


            string[] urls = new string[] { Strings.url1, Strings.url2, Strings.url3 };

            List<Task> downloadTasks = new List<Task>();

            _cts = new CancellationTokenSource();

            for (int i = 0; i < urls.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(urls[i])) continue;

                downloadTasks.Add(
                    FileDownloadManager.DownloadAsync(
                        urls[i], 
                        $"{baseFilePath}_{i + 1}", 
                        _cts.Token, 
                        DownloadItems[i].ProgressHandler
                    )
                );
            }

            try
            {
                DateTime startTime = DateTime.Now;

                await Task.WhenAll(downloadTasks);

                DateTime endTime = DateTime.Now;

                StatusMessage = StatusMessage = $"{Strings.download_finished} {(endTime - startTime).TotalSeconds:F1}(초)"; ;
            }
            catch (OperationCanceledException)
            {
                CleanUpDownloadStatus();
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        private bool CanDownloadAll(object parameter)
        {
            return true;
        }

        public ICommand CancelCommand { get; set; }

        private void OnCancel()
        {
            _cts?.Cancel();
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
            for (int i = 0; i < DownloadItems.Count; i++)
            {
                DownloadItems[i].Progress = 0f;
            }

            StatusMessage = Strings.cancel_download;
        }

        #endregion

    }
}
