using System.Diagnostics;
using Microsoft.Extensions.Logging;
using VidyaBot.App.Logging;

namespace VidyaBot.App.Modules;

public partial class VideoDownloadCommandModule
{
    private async Task DownloadVideoFileAsync(string url, string outputPath)
    {
        ProcessStartInfo downloadStartInfo = new()
        {
            FileName = "python3",
            ArgumentList =
            {
                "-m",
                "yt_dlp",
                "-f",
                "bestvideo[ext=mp4][height<=?720]+bestaudio[ext=m4a]/best[ext=mp4]",
                "--recode-video",
                "mp4",
                "--output",
                "%(id)s.%(ext)s",
                "--no-progress",
                url
            },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = outputPath,
        };

        _logger.LogExecutingProcess(downloadStartInfo.FileName, string.Join(" ", downloadStartInfo.ArgumentList));
        using Process process = new()
        {
            StartInfo = downloadStartInfo
        };

        process.Start();

        while (!process.HasExited)
        {
            string? error = await process.StandardError.ReadToEndAsync();

            if (error is not null && !string.IsNullOrEmpty(error))
            {
                process.Kill();
                throw new Exception(error);
            }
        }
    }
}