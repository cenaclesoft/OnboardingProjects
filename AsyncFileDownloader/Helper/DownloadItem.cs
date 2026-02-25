using AsyncFileDownloader.ViewModel;
using System;

namespace AsyncFileDownloader.Helper
{
    public class DownloadItem : ViewModelBase
    {
        private string _linkTitle;

        public string LinkTitle
        {
            get => _linkTitle;
            private set => SetProperty<string>(ref _linkTitle, value);
        }

        private string _fileName;

        public string FileName
        {
            get => _fileName; 
            private set => SetProperty<string>(ref _fileName, value);
        }


        private string _url;
        
        public string Url
        {
            get => _url;
            set => SetProperty<string>(ref _url, value);
        }


        private string _savePath;

        public string SavePath
        {
            get => _url;
            private set => SetProperty<string>(ref _savePath, value);
        }


        private double _progress;

        public double Progress
        {
            get => _progress;
            set => SetProperty<double>(ref _progress, value);
        }

        public IProgress<double> ProgressHandler { get; }

        public DownloadItem(string linkTitle, string url)
        {
            LinkTitle = linkTitle;
            Url = url;
            ProgressHandler = new Progress<double>(val => Progress = val);
        }
    }
}
