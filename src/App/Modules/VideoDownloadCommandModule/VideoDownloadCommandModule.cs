using Discord;
using Discord.Interactions;
using VidyaBot.App.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace VidyaBot.App.Modules;

/// <summary>
/// <see cref="InteractionModuleBase"/> for handling download commands.
/// </summary>
public partial class VideoDownloadCommandModule : InteractionModuleBase
{
    private readonly IDiscordService _discordService;
    private readonly ILogger<VideoDownloadCommandModule> _logger;
    private readonly IConfiguration _configuration;

    public VideoDownloadCommandModule(IDiscordService discordService, ILogger<VideoDownloadCommandModule> logger, IConfiguration configuration)
    {
        _discordService = discordService;
        _logger = logger;
        _configuration = configuration;
    }
}