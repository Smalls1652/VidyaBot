using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using VidyaBot.App.Logging;

namespace VidyaBot.App.Modules;

public partial class VideoDownloadCommandModule
{
    private async Task<FileInfo> ConvertVideoFileAsync(FileInfo inputFile)
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "ffmpeg",
            ArgumentList =
            {
                "-i",
                inputFile.FullName,
                "-vf",
                "scale=-1:720",
                "-c:v",
                "h264",
                "-crf",
                "25",
                "-preset",
                "fast",
                "-c:a",
                "copy",
                $"./{inputFile.Name.Replace(inputFile.Extension, "")}_converted.mp4"
            },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = inputFile.Directory!.FullName
        };

        _logger.LogExecutingProcess(startInfo.FileName, string.Join(' ', startInfo.ArgumentList));

        Process process = new()
        {
            StartInfo = startInfo
        };

        process.Start();

        await process.WaitForExitAsync();

        _logger.LogInformation("Conversion process exited with code {ExitCode}.", process.ExitCode);

        if (process.ExitCode != 0)
        {
            string? error = await process.StandardError.ReadToEndAsync();
            throw new Exception(error);
        }

        return new(Path.Combine(inputFile.Directory!.FullName, $"{inputFile.Name.Replace(inputFile.Extension, "")}_converted.mp4"));
    }
}