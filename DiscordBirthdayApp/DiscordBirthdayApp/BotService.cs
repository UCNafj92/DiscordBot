using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using DiscordBirthdayApp.Model;
using Newtonsoft.Json.Linq;

namespace DiscordBirthdayApp
{
    /// <summary>
    /// Handles the core functionality of the Discord bot, including connecting to the Discord gateway, 
    /// handling interactions, and managing scheduled tasks like daily birthday checks.
    /// </summary>
    public class BotService
    {
        private DiscordSocketClient _client;
        private InteractionService _interactionService;
        private IServiceProvider _services;
        private string _token;
        private ulong _guildId;
        private ulong[] _ownerIds;
        private static System.Timers.Timer _dailyTimer;

        /// <summary>
        /// Singleton instance of <see cref="BotService"/> to ensure a single running instance.
        /// </summary>
        private static readonly Lazy<BotService> lazy = new Lazy<BotService>(() => new BotService());

        /// <summary>
        /// Gets the singleton instance of the bot service.
        /// </summary>
        public static BotService Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private BotService()
        {
        }

        /// <summary>
        /// Initializes and starts the bot.
        /// </summary>
        /// <param name="token">The bot token.</param>
        /// <param name="guildId">The ID of the guild (server) where the bot will operate.</param>
        /// <param name="ownerIds">Array of IDs for bot owners.</param>
        public async Task RunBotAsync(string token, ulong guildId, ulong[] ownerIds)
        {
            _token = token;
            _guildId = guildId;
            _ownerIds = ownerIds;

            // ✅ Configure DiscordSocketClient with UseInteractionSnowflakeDate = false
            var config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                UseInteractionSnowflakeDate = false, // 🔹 Prevents timestamp issues causing timeouts
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildMembers
            };

            _client = new DiscordSocketClient(config);
            _interactionService = new InteractionService(_client);

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_interactionService)
                .AddSingleton(BotService.Instance)
                .BuildServiceProvider();

            _client.Log += Log;
            _client.Ready += OnReady;

            _client.InteractionCreated += async (interaction) =>
            {
                Console.WriteLine($"🔹 Received interaction of type: {interaction.Type}");

                var ctx = new SocketInteractionContext(_client, interaction);
                var result = await _interactionService.ExecuteCommandAsync(ctx, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine($"❌ Command execution failed: {result.ErrorReason}");
                }
                else
                {
                    Console.WriteLine("✅ Command executed successfully!");
                }
            };

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Logs messages from the Discord client to the console.
        /// </summary>
        /// <param name="msg">The log message.</param>
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts the daily check for birthdays at midnight.
        /// </summary>
        private void StartDailyCheck()
        {
            DateTime now = DateTime.Now;
            DateTime nextMidnight = now.Date.AddDays(1); // Next day at 00:00
            TimeSpan timeUntilMidnight = nextMidnight - now;

            Console.WriteLine($"⏳ First birthday check in: {timeUntilMidnight.TotalHours} hours");

            _dailyTimer = new System.Timers.Timer(timeUntilMidnight.TotalMilliseconds);
            _dailyTimer.Elapsed += async (sender, e) => await RunDailyCheck();
            _dailyTimer.AutoReset = false;
            _dailyTimer.Enabled = true;
        }

        /// <summary>
        /// Executes the daily birthday check and updates user roles.
        /// </summary>
        private async Task RunDailyCheck()
        {
            await CheckBirthdays();

            // Restart the timer to check again in 24 hours
            _dailyTimer = new System.Timers.Timer(86400000); // 24 hours
            _dailyTimer.Elapsed += async (sender, e) => await RunDailyCheck();
            _dailyTimer.AutoReset = true;
            _dailyTimer.Enabled = true;
        }

        /// <summary>
        /// Checks the birthdays of members and updates the "Birthday" role.
        /// </summary>
        public async Task CheckBirthdays()
        {
            var today = DateTime.Now.ToString("dd-MM");
            var guild = _client.GetGuild(_guildId);

            if (guild == null)
            {
                Console.WriteLine("❌ Guild not found!");
                return;
            }

            var role = guild.Roles.FirstOrDefault(r => r.Name == "Birthday");

            if (role == null)
            {
                Console.WriteLine("🔹 Birthday role not found, creating...");
                try
                {
                    var restRole = await guild.CreateRoleAsync("Birthday", GuildPermissions.None, Color.Gold, isHoisted: true, isMentionable: true);
                    role = guild.Roles.FirstOrDefault(r => r.Id == restRole.Id);

                    if (role == null)
                    {
                        Console.WriteLine("❌ Failed to retrieve the newly created Birthday role.");
                        return;
                    }
                    Console.WriteLine("✅ Birthday role created successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to create Birthday role: {ex.Message}");
                    return;
                }
            }

            Console.WriteLine("📂 Reloading birthdays from JSON file...");
            List<Member> members = BirthdayStorage.Instance.LoadBirthdays();

            foreach (var member in members)
            {
                var guildUser = guild.GetUser(member.UserId);
                if (guildUser == null) continue;

                if (member.Birthday == today)
                {
                    if (!guildUser.Roles.Contains(role))
                    {
                        await guildUser.AddRoleAsync(role);
                        Console.WriteLine($"✅ Added Birthday role to {guildUser.Username}");
                    }
                }
                else
                {
                    if (guildUser.Roles.Contains(role))
                    {
                        await guildUser.RemoveRoleAsync(role);
                        Console.WriteLine($"🔹 Removed Birthday role from {guildUser.Username}");
                    }
                }
            }
        }

        /// <summary>
        /// Called when the bot connects to the Discord gateway.
        /// Loads interaction modules and registers commands.
        /// </summary>
        private async Task OnReady()
        {
            Console.WriteLine($"{_client.CurrentUser} is online and connected!");

            Console.WriteLine("🔹 Loading interaction modules...");
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            Console.WriteLine($"🔹 Loaded {_interactionService.Modules.Count} modules.");

            await _interactionService.RegisterCommandsToGuildAsync(_guildId);
            Console.WriteLine("✅ Slash commands registered!");

            // ✅ Start the daily birthday check at midnight
            StartDailyCheck();
        }
    }
}
