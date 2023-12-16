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

        string fileName;
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

        string outputDirectoryPath = Path.Combine(
            _configuration.GetSection("VideoDownloadPath").Value ?? Path.GetTempPath(),
            Guid.NewGuid().ToString()
        );

        _logger.VideoFileSavePathLog(outputDirectoryPath);

        Directory.CreateDirectory(outputDirectoryPath);

        string outputFilePath = Path.Combine(outputDirectoryPath, fileName);

        try
        {
            await DownloadVideoFileAsync(url, outputDirectoryPath);
        }
        catch (Exception e)
        {
            Directory.Delete(outputDirectoryPath, true);
            
            _logger.ErrorLog(e.Message, e);

            await FollowupAsync(
                text: "Something went wrong downloading the video. 😰"
            );

            return;
        }

        FileInfo outputFile = new(outputFilePath);

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

            Directory.Delete(outputDirectoryPath, true);
        }
    }
}