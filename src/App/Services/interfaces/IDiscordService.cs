using Discord;

namespace VidyaBot.App.Services;

/// <summary>
/// Interface for services that connect to Discord.
/// </summary>
public interface IDiscordService
{
    /// <summary>
    /// Connects the bot to Discord.
    /// </summary>
    /// <returns></returns>
    Task ConnectAsync();
}