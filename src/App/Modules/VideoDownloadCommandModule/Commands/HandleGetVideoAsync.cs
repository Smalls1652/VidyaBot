using System.Diagnostics;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using VidyaBot.App.Logging;

namespace VidyaBot.App.Modules;

public partial class VideoDownloadCommandModule
{
    [SlashCommand(
        name: "getvideo",
        description: "Download a video from a video sharing site and send it to the channel."
    )]
    private async Task HandleGetVideoAsync(
        [Summary(
            name: "url",
            description: "The URL of the video to download."
        )]
        string url
    )
    {
        await DeferAsync();

        _logger.ReceivedCommandLog(Context.User.Id, Context.Guild.Id);

        string fileName = string.Empty;
        try
        {
            fileName = await GetVideoFileNameAsync(url);
        }
        catch (NullReferenceException e)
        {
            _logger.ErrorLog(e.Message, e);

            string errorText = $@"
Something went wrong getting data about the video.
                    
**Error message:**
```log
{e.Message}
```";

            await FollowupAsync(
                text: errorText
            );

            return;
        }
        catch (Exception e)
        {
            _logger.ErrorLog(e.Message, e);

            string errorText = $@"
Something went wrong getting the filename.
                    
**Error message:**
```log
{e.Message}
```";

            await FollowupAsync(
                text: errorText
            );

            return;
        }


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
            WorkingDirectory = Path.GetTempPath(),
        };

        _logger.ExecutingProcessLog(downloadStartInfo.FileName, string.Join(" ", downloadStartInfo.ArgumentList));
        using Process process = new()
        {
            StartInfo = downloadStartInfo
        };

        process.Start();

        while (!process.HasExited)
        {
            string? output = await process.StandardOutput.ReadLineAsync();
            string? downloadProcError = await process.StandardError.ReadLineAsync();

            if (output is not null)
            {
                _logger.LogInformation("{output}", output);
            }

            if (downloadProcError is not null)
            {
                _logger.LogError("{error}", downloadProcError);

                await FollowupAsync(
                    text: "Something went wrong. Please try again later."
                );

                return;
            }
        }

        FileInfo outputFile = new(Path.Combine(Path.GetTempPath(), fileName));

        ButtonBuilder sourceUrlButton = new(
            label: "Source",
            style: ButtonStyle.Link,
            url: url
        );

        ComponentBuilder componentBuilder = new ComponentBuilder()
            .WithButton(sourceUrlButton, 0);

        using FileStream fileStream = outputFile.OpenRead();

        _logger.LogInformation("Sending file to guild {GuildId}.", Context.Guild.Id);

        try
        {
            await FollowupWithFileAsync(
                fileName: fileName,
                fileStream: fileStream,
                components: componentBuilder.Build()
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send file to guild {GuildId}.", Context.Guild.Id);

            await FollowupAsync(
                text: "Something went wrong. Please try again later."
            );
        }
        finally
        {
            fileStream.Close();

            outputFile.Delete();
        }
    }
}