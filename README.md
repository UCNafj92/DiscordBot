# 🎂 Discord Birthday Bot

A Discord bot that assigns and manages a "Birthday" role for users on their birthday. This project is built using **C#**, **.NET 8.0**, and the **Discord.NET** library.

---

## 🚀 Features
✅ Assigns a "Birthday" role to users on their birthday  
✅ Removes the role after the birthday ends  
✅ Allows users to set their own birthday using slash commands  
✅ Manual birthday check command available for admins  

---

## 🛠️ Requirements
### 1. **Bot Token**
- You need a Discord bot token:
   - Go to the [Discord Developer Portal](https://discord.com/developers/applications).
   - Create a new bot application.
   - Copy the token from the "Bot" settings.

### 2. **Enable Developer Mode**
- Open Discord and go to **User Settings → Advanced**.
- Turn on **Developer Mode**.

### 3. **Get Guild ID and User ID**
- Right-click your server name → **Copy ID** → Save it as the `GuildId`  
- Right-click your user profile → **Copy ID** → Save it as the `OwnerIds`

### 4. **Create an Invite Link**
- In the Discord Developer Portal:
   - Go to the "OAuth2" settings.
   - Under **Scopes**, select `bot` and `applications.commands`.
   - Under **Bot Permissions**, select the following:
     - ✅ `Manage Roles`
     - ✅ `Send Messages`
     - ✅ `Read Messages`
     - ✅ `Use Slash Commands`
   - Generate the invite link → Add the bot to your server.

### 5. **Set Role Priority**
- The bot’s role must be **higher** than the "Birthday" role in the role hierarchy.
- Go to **Server Settings → Roles** → Drag the bot's role above the "Birthday" role.

---

## 🏗️ Setup Instructions
### 1. **Clone the Repository**

git clone https://github.com/your-username/discord-birthday-bot.git
cd discord-birthday-bot

### 2. **Configure Settings**
- Copy the example settings file:

```
cp appsettings.example.json appsettings.json
```
- Open appsettings.json and fill in your bot's information:
```
{
  "Discord": {
    "Token": "YOUR_BOT_TOKEN",
    "GuildId": "YOUR_GUILD_ID",
    "OwnerIds": [123456789012345678]
  }
}
```

### 📦 **Dependencies**
Use NuGet to install the required libraries:
```
dotnet add package Discord.Net --version 3.12.0
dotnet add package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 8.0.0
dotnet add package Newtonsoft.Json --version 13.0.3
```

## **Commands Overview**
| Command  | Second Header |
| ------------- | ------------- |
| /setbirthday DD-MM  | Sets your birthday (format: DD-MM)  |
| /checkbirthdays  | Manually triggers the birthday check  |

## **Notes**
The bot will automatically check birthdays daily at midnight.

Make sure the bot has the correct permissions to manage roles and send messages.

If you change the bot token, update appsettings.json and restart the bot.