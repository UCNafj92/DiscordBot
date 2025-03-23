using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Extensions.Configuration;
using DiscordBirthdayApp;

class Program
{
    /// <summary>
    /// Main entry point for the Discord bot application.
    /// Loads configuration from appsettings.json and starts the bot service.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
    static async Task Main(string[] args)
    {
        // ✅ Load configuration from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Base directory of the app
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // ✅ Read settings from config
        var token = config["Discord:Token"];
        var guildId = ulong.Parse(config["Discord:GuildId"]);
        var ownerIds = config.GetSection("Discord:OwnerIds").Get<ulong[]>();

        // ✅ Display loaded config (for debugging purposes)
        Console.WriteLine($"✅ Token: {token.Substring(0, 5)}... (hidden for security)"); // Only show first 5 chars
        Console.WriteLine($"✅ Guild ID: {guildId}");
        Console.WriteLine($"✅ Owner IDs: {string.Join(", ", ownerIds)}");

        // ✅ Pass configuration to the bot service
        BotService bot = BotService.Instance;
        await bot.RunBotAsync(token, guildId, ownerIds);
    }
}
