using Microsoft.Extensions.Logging;

namespace VidyaBot.App.Logging;

/// <summary>
/// Source generated log messages used across the application.
/// </summary>
public static partial class AppLogger
{
    /// <summary>
    /// Logs information about receiving a command from a user in a guild.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="guildId">The ID of the guild.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Received command from user {UserId} in guild {GuildId}."
    )]
    public static partial void LogReceivedCommand(this ILogger logger, ulong userId, ulong guildId);

    /// <summary>
    /// Logs a generic error message with an optional exception.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="errorMessage">The error message to log.</param>
    /// <param name="exception">The optional exception associated with the error.</param>
    [LoggerMessage(
        level: LogLevel.Error,
        message: "{errorMessage}"
    )]
    public static partial void LogGenericError(this ILogger logger, string errorMessage, Exception? exception = null);

    /// <summary>
    /// Logs the execution of a process with the specified command and arguments.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="command">The command being executed.</param>
    /// <param name="arguments">The arguments passed to the command.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Executing command: {Command} {Arguments}"
    )]
    public static partial void LogExecutingProcess(this ILogger logger, string command, string arguments);

    /// <summary>
    /// Logs the save path for a video file.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="outputPath">The path where the video file will be saved.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Video file will be saved to {OutputPath}."
    )]
    public static partial void LogVideoFileSavePath(this ILogger logger, string outputPath);

    /// <summary>
    /// Logs information about sending a file to a guild.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="guildId">The ID of the guild.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Sending file to guild {GuildId}."
    )]
    public static partial void LogSendingFile(this ILogger logger, ulong guildId);

    /// <summary>
    /// Logs an error message when sending a file to a guild fails.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="exception">The optional exception associated with the failure.</param>
    [LoggerMessage(
        level: LogLevel.Error,
        message: "Failed to send file to guild {GuildId}."
    )]
    public static partial void LogFailedToSendFile(this ILogger logger, ulong guildId, Exception? exception = null);

    /// <summary>
    /// Logs a message indicating that the application is connecting to Discord.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Connecting to Discord..."
    )]
    public static partial void LogConnectingToDiscord(this ILogger logger);

    /// <summary>
    /// Logs a message indicating the initialization of the Discord Interaction Service.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Initializing Discord Interaction Service..."
    )]
    public static partial void LogInitializingDiscordInteractionService(this ILogger logger);

    /// <summary>
    /// Logs a message indicating that the application is running in debug mode
    /// and registering slash commands for a specific guild.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="guildId">The ID of the guild.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Running in debug mode. Registering slash commands to test guild '{GuildId}'."
    )]
    public static partial void LogRunningInDebugMode(this ILogger logger, ulong guildId);

    /// <summary>
    /// Logs the registration of slash commands globally.
    /// </summary>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Registering slash commands globally."
    )]
    public static partial void LogRegisteringSlashCommandsGlobally(this ILogger logger);

    /// <summary>
    /// Logs information that slash commands have been loaded.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="slashCommandsLoaded">The slash commands loaded.</param>
    [LoggerMessage(
        level: LogLevel.Information,
        message: "Slash commands loaded: {SlashCommandsLoaded}"
    )]
    public static partial void LogSlashCommandsLoaded(this ILogger logger, string slashCommandsLoaded);
}