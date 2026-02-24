using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                        {
                            var buffer = new byte[4096];
                            long totalReadBytes = 0;
                            int readBytes;

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
                // 취소는 예외가 아니므로 단순 throw
                throw; 
            }
            catch (Exception ex)
            {
                // 그 외 모든 에러 전파
                throw new Exception($"다운로드 실패: {url}", ex);
            }
        }
    }
}
