using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBirthdayApp;

/// <summary>
/// Defines the slash commands for the bot.
/// Includes commands for checking birthdays, setting birthdays, and checking bot responsiveness.
/// </summary>
public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly BotService _botService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlashCommands"/> class.
    /// </summary>
    /// <param name="botService">The bot service to handle birthday checks and bot state.</param>
    public SlashCommands(BotService botService)
    {
        _botService = botService;
    }

    /// <summary>
    /// Manually triggers the birthday check.
    /// </summary>
    /// <remarks>
    /// This command can be used by admins or users with the necessary permissions to manually force a birthday check.
    /// </remarks>
    [SlashCommand("checkbirthdays", "Manually trigger the birthday check.")]
    public async Task CheckBirthdaysCommand()
    {
        Console.WriteLine($"🔹 {Context.User.Username} triggered a manual birthday check.");

        // ✅ Defer to prevent timeout while processing
        await DeferAsync(ephemeral: true);

        try
        {
            // ✅ Call the birthday check on the BotService
            await _botService.CheckBirthdays();
            Console.WriteLine($"✅ Birthday check completed.");

            await FollowupAsync("✅ Birthday check completed!", ephemeral: true);
            Console.WriteLine($"✅ FollowupAsync() completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error during birthday check: {ex}");
            await FollowupAsync("❌ An error occurred while checking birthdays.", ephemeral: true);
        }
    }

    /// <summary>
    /// Responds with "Pong!" to test if the bot is responsive.
    /// </summary>
    /// <remarks>
    /// This is a simple health check command to ensure the bot is online and responsive.
    /// </remarks>
    [SlashCommand("ping", "Check if the bot is responsive.")]
    public async Task PingCommand()
    {
        Console.WriteLine("🔹 Ping command executed.");

        try
        {
            await RespondAsync("🏓 Pong!", ephemeral: true);
            Console.WriteLine("✅ RespondAsync() completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ RespondAsync() failed: {ex}");
        }
    }

    /// <summary>
    /// Sets the user's birthday.
    /// </summary>
    /// <param name="date">The birthday date in format "DD-MM".</param>
    /// <remarks>
    /// This stores the birthday in the JSON file and links it to the user's Discord ID.
    /// </remarks>
    [SlashCommand("setbirthday", "Set your birthday (format: DD-MM).")]
    public async Task SetBirthdayCommand(string date)
    {
        Console.WriteLine($"🔹 Received /setbirthday command. Attempting to set birthday to {date} for {Context.User.Username} ({Context.User.Id})");

        // ✅ Defer to prevent timeout while processing
        await DeferAsync(ephemeral: true);

        try
        {
            // ✅ Save the birthday to the JSON file using BirthdayStorage
            BirthdayStorage.Instance.SaveBirthday(Context.User.Id, date);
            Console.WriteLine($"✅ SaveBirthday() executed successfully for {Context.User.Username}");

            await FollowupAsync($"🎂 Your birthday has been set to {date}!", ephemeral: true);
            Console.WriteLine($"✅ FollowupAsync() completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in SetBirthdayCommand: {ex}");
            await FollowupAsync("❌ An error occurred while saving your birthday.", ephemeral: true);
        }
    }
}
