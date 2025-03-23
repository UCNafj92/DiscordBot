using System;
using System.Collections.Generic;
using System.IO;
using DiscordBirthdayApp.Model;
using Newtonsoft.Json;

/// <summary>
/// Handles loading and saving birthdays to a JSON file.
/// </summary>
public class BirthdayStorage
{
    private const string FilePath = "birthdays.json";

    /// <summary>
    /// Singleton instance of <see cref="BirthdayStorage"/> to ensure a single shared instance.
    /// </summary>
    private static readonly Lazy<BirthdayStorage> lazy = new Lazy<BirthdayStorage>(() => new BirthdayStorage());

    /// <summary>
    /// Gets the singleton instance of the <see cref="BirthdayStorage"/> class.
    /// </summary>
    public static BirthdayStorage Instance
    {
        get
        {
            return lazy.Value;
        }
    }

    /// <summary>
    /// Private constructor to enforce singleton pattern and ensure JSON file exists.
    /// </summary>
    private BirthdayStorage()
    {
        string fullPath = Path.GetFullPath(FilePath);
        Console.WriteLine($"🔹 JSON file path: {fullPath}");

        if (!File.Exists(FilePath))
        {
            Console.WriteLine("⚠️ Birthdays JSON file not found. Creating a new one...");
            File.WriteAllText(FilePath, "{}"); // Creates an empty JSON file
        }
        else
        {
            Console.WriteLine("✅ Birthdays JSON file found.");
        }
    }

    /// <summary>
    /// Loads the list of birthdays from the JSON file.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Member"/> objects representing stored birthdays.
    /// </returns>
    public List<Member> LoadBirthdays()
    {
        Console.WriteLine($"📂 Loading birthdays from file: {FilePath}");

        if (!File.Exists(FilePath))
        {
            Console.WriteLine("❌ Birthday JSON file not found. Creating a new one.");
            File.WriteAllText(FilePath, "[]"); // Empty JSON array
            return new List<Member>();
        }

        try
        {
            // ✅ Read JSON content
            var json = File.ReadAllText(FilePath);

            // ✅ Deserialize JSON into a list of members
            var data = JsonConvert.DeserializeObject<List<Member>>(json) ?? new List<Member>();

            Console.WriteLine($"✅ Successfully loaded birthdays from JSON: {string.Join(", ", data.Select(m => $"{m.UserId}: {m.Birthday}"))}");
            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to read birthdays JSON: {ex.Message}");
            return new List<Member>();
        }
    }

    /// <summary>
    /// Saves or updates a user's birthday in the JSON file.
    /// </summary>
    /// <param name="userId">The Discord user ID.</param>
    /// <param name="date">The birthday date in format "DD-MM".</param>
    public void SaveBirthday(ulong userId, string date)
    {
        // ✅ Load existing birthdays from the JSON file
        List<Member> members = LoadBirthdays();
        Member member = new Member { UserId = userId, Birthday = date };

        Console.WriteLine($"📂 JSON File Location: {FilePath}");

        // ✅ Check if the user already exists
        var existingMember = members.FirstOrDefault(m => m.UserId == userId);
        if (existingMember != null)
        {
            Console.WriteLine($"🔄 Updating birthday for user {userId} from {existingMember.Birthday} to {date}");
            existingMember.Birthday = date;
        }
        else
        {
            Console.WriteLine($"🆕 Adding new birthday for user {userId}: {date}");
            members.Add(member);
        }

        try
        {
            // ✅ Write the updated list back to JSON
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(members, Formatting.Indented));
            Console.WriteLine($"✅ Birthday successfully saved for {userId}: {date}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to save JSON file: {ex.Message}");
        }
    }
}
