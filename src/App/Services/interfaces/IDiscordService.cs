using Discord;

namespace VidyaBot.App.Services;

public interface IDiscordService
{
    /// <summary>
    /// Connects the bot to Discord.
    /// </summary>
    /// <returns></returns>
    Task ConnectAsync();
}