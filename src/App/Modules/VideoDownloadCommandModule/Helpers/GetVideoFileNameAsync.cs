using System.Diagnostics;
using Microsoft.Extensions.Logging;
using VidyaBot.App.Logging;

namespace VidyaBot.App.Modules;

public partial class VideoDownloadCommandModule
{
    private async Task<string> GetVideoFileNameAsync(string url)
    {
        ProcessStartInfo printFileNameStartInfo = new()
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
                "--print",
                "filename",
                url
            },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetTempPath(),
        };

        _logger.ExecutingProcessLog(printFileNameStartInfo.FileName, string.Join(" ", printFileNameStartInfo.ArgumentList));

        using Process printFileNameProcess = new()
        {
            StartInfo = printFileNameStartInfo
        };

        printFileNameProcess.Start();

        await printFileNameProcess.WaitForExitAsync();

        string? error = await printFileNameProcess.StandardError.ReadToEndAsync();

        if (error is not null && !string.IsNullOrEmpty(error))
        {
            throw new Exception(error);
        }

        string? fileName = await printFileNameProcess.StandardOutput.ReadLineAsync();

        if (fileName is null)
        {
            throw new NullReferenceException("The filename for the video returned as null.");
        }

        return fileName;
    }
}