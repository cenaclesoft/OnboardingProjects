using AsyncFileDownloader.ViewModel;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncFileDownloader.Manager
{
    public class FileDownloadManager
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static FileDownloadManager Instance => new FileDownloadManager();

        private FileDownloadManager() { }

        public static async Task DownloadAsync(string url, string destinationPath, CancellationToken token, IProgress<double> progress)
        {
            try
            {
                await Task.Run(async () =>
                {
                    using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
                    {
                        response.EnsureSuccessStatusCode();

                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())

                        using (FileStream fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                        {
                            byte[] buffer = new byte[8192]; // 8KB
                            long totalReadBytes = 0;
                            int readBytes = 0;

                            while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                            {
                                token.ThrowIfCancellationRequested();

                                await fileStream.WriteAsync(buffer, 0, readBytes, token);

                                totalReadBytes += readBytes;

                                if (totalBytes != -1)
                                {
                                    double currentProgress = (double)totalReadBytes / totalBytes * 100;
                                    progress.Report(currentProgress);
                                }
                            }
                        }
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"실패: {url}", ex);
            }
        }
    }
}
