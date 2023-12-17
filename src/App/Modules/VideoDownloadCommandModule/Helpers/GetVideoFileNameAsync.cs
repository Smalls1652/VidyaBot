using System.Diagnostics;
using Microsoft.Extensions.Logging;
using VidyaBot.App.Logging;

namespace VidyaBot.App.Modules;

public partial class VideoDownloadCommandModule
{
    /// <summary>
    /// Get the output file name 'yt-dlp' will return for the given URL.
    /// </summary>
    /// <param name="url">The URL from the video sharing site.</param>
    /// <returns>The output file name 'yt-dlp' will return.</returns>
    /// <exception cref="Exception">A generic error occurred with the process.</exception>
    /// <exception cref="NullReferenceException">The process returned a null value for the filename.</exception>
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

        _logger.LogExecutingProcess(printFileNameStartInfo.FileName, string.Join(" ", printFileNameStartInfo.ArgumentList));

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

        return fileName is null ? throw new NullReferenceException("The filename for the video returned as null.") : fileName;
    }
}