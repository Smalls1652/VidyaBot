using Microsoft.Extensions.Logging;

namespace VidyaBot.App.Logging;

public static partial class AppLogger
{
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Received command from user {UserId} in guild {GuildId}."
    )]
    public static partial void ReceivedCommandLog(this ILogger logger, ulong userId, ulong guildId);

    [LoggerMessage(
        level: LogLevel.Error,
        message: "{errorMessage}"
    )]
    public static partial void ErrorLog(this ILogger logger, string errorMessage, Exception? exception = null);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Executing command: {Command} {Arguments}"
    )]
    public static partial void ExecutingProcessLog(this ILogger logger, string command, string arguments);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Video file will be saved to {OutputPath}."
    )]
    public static partial void VideoFileSavePathLog(this ILogger logger, string outputPath);
}