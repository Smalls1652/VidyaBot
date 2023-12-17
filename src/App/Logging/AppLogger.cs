using Microsoft.Extensions.Logging;

namespace VidyaBot.App.Logging;

public static partial class AppLogger
{
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Received command from user {UserId} in guild {GuildId}."
    )]
    public static partial void LogReceivedCommand(this ILogger logger, ulong userId, ulong guildId);

    [LoggerMessage(
        level: LogLevel.Error,
        message: "{errorMessage}"
    )]
    public static partial void LogGenericError(this ILogger logger, string errorMessage, Exception? exception = null);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Executing command: {Command} {Arguments}"
    )]
    public static partial void LogExecutingProcess(this ILogger logger, string command, string arguments);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Video file will be saved to {OutputPath}."
    )]
    public static partial void LogVideoFileSavePath(this ILogger logger, string outputPath);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Sending file to guild {GuildId}."
    )]
    public static partial void LogSendingFile(this ILogger logger, ulong guildId);

    [LoggerMessage(
        level: LogLevel.Error,
        message: "Failed to send file to guild {GuildId}."
    )]
    public static partial void LogFailedToSendFile(this ILogger logger, ulong guildId, Exception? exception = null);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Connecting to Discord..."
    )]
    public static partial void LogConnectingToDiscord(this ILogger logger);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Initializing Discord Interaction Service..."
    )]
    public static partial void LogInitializingDiscordInteractionService(this ILogger logger);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Running in debug mode. Registering slash commands to test guild '{GuildId}'."
    )]
    public static partial void LogRunningInDebugMode(this ILogger logger, ulong guildId);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Registering slash commands globally."
    )]
    public static partial void LogRegisteringSlashCommandsGlobally(this ILogger logger);

    [LoggerMessage(
        level: LogLevel.Information,
        message: "Slash commands loaded: {SlashCommandsLoaded}"
    )]
    public static partial void LogSlashCommandsLoaded(this ILogger logger, string slashCommandsLoaded);
}