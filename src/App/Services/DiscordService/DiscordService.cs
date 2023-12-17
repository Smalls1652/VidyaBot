using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VidyaBot.App.Logging;
using VidyaBot.App.Modules;

namespace VidyaBot.App.Services;

/// <summary>
/// Service for interacting with Discord.
/// </summary>
public class DiscordService : IDiscordService
{
    private readonly DiscordSocketClient _socketClient;
    private readonly ILogger<DiscordService> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;

    
    /// <summary>
    /// Initializes a new instance of <see cref="DiscordService"/>.
    /// </summary>
    /// <param name="socketClient">The Discord socket client.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public DiscordService(DiscordSocketClient socketClient, ILogger<DiscordService> logger, IConfiguration config, IServiceProvider serviceProvider)
    {
        _socketClient = socketClient;
        _logger = logger;
        _config = config;
        _serviceProvider = serviceProvider;
    }

    private InteractionService? _interactionService;

    /// <inheritdoc />
    public async Task ConnectAsync()
    {
        // Log into Discord
        _logger.LogConnectingToDiscord();

        if (_config.GetValue<string>("DiscordClientToken") is null)
        {
            Exception errorException = new("DiscordClientToken is null. Please set the DiscordClientToken environment variable.");

            _logger.LogGenericError(errorException.Message, errorException);
            throw errorException;
        }
        
        await _socketClient.LoginAsync(
            tokenType: TokenType.Bot,
            token: _config.GetValue<string>("DiscordClientToken")
        );

        await _socketClient.StartAsync();

        _logger.LogInitializingDiscordInteractionService();
        _interactionService = new(
            discord: _socketClient.Rest
        );

        await _interactionService.AddModuleAsync<VideoDownloadCommandModule>(_serviceProvider);

        _socketClient.Log += HandleLog;
        _socketClient.Ready += OnClientReadyAsync;
        _socketClient.InteractionCreated += HandleSlashCommand;
    }

    /// <summary>
    /// Handles the client ready event and registers slash commands.
    /// </summary>
    /// <remarks>
    /// This method will register slash commands to a test guild
    /// if the application is running in debug mode.
    /// </remarks>
    /// <returns></returns>
    private async Task OnClientReadyAsync()
    {
#if DEBUG
        ulong testGuildId = _config.GetValue<ulong>("DiscordTestGuildId");
        _logger.LogRunningInDebugMode(testGuildId);
        await _interactionService!.RegisterCommandsToGuildAsync(
            guildId: testGuildId,
            deleteMissing: true
        );
#else
        _logger.LogRegisteringSlashCommandsGlobally();
        await _interactionService!.RegisterCommandsGloballyAsync(
            deleteMissing: true
        );
#endif

        string slashCommandsLoadedString = string.Join(",", _interactionService.SlashCommands);
        _logger.LogSlashCommandsLoaded(slashCommandsLoadedString);
    }

    
    /// <summary>
    /// Handles a slash command interaction.
    /// </summary>
    /// <param name="interaction">The socket interaction.</param>
    /// <returns></returns>
    private async Task HandleSlashCommand(SocketInteraction interaction)
    {
        SocketInteractionContext interactionContext = new(_socketClient, interaction);
        await _interactionService!.ExecuteCommandAsync(interactionContext, _serviceProvider);
    }

    /// <summary>
    /// Handles the logging of a <see cref="LogMessage"/> from the Discord client.
    /// </summary>
    /// <param name="logMessage">The log message to handle.</param>
    /// <returns></returns>
    private Task HandleLog(LogMessage logMessage)
    {
        LogLevel logLevel = logMessage.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(logMessage.Severity), logMessage.Severity, "Unknown log severity.")
        };

        _logger.Log(
            logLevel: logLevel,
            message: logMessage.Exception is CommandException cmdException ? $"{cmdException.Command.Aliases.First()} failed to execute in {cmdException.Context.Channel}." : logMessage.Message,
            exception: logMessage.Exception
        );

        return Task.CompletedTask;
    }
}