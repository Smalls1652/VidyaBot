using Discord;
using Discord.Interactions;
using VidyaBot.App.Services;
using Microsoft.Extensions.Logging;

namespace VidyaBot.App.Modules;

public partial class VideoDownloadCommandModule : InteractionModuleBase
{
    private readonly IDiscordService _discordService;
    private readonly ILogger<VideoDownloadCommandModule> _logger;

    public VideoDownloadCommandModule(IDiscordService discordService, ILogger<VideoDownloadCommandModule> logger)
    {
        _discordService = discordService;
        _logger = logger;
    }
}