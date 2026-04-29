using System.Text.Json;

namespace CursorKeep.Services
{
    public class AppConfig
    {
        public string BotToken { get; set; } = "";
        public long? AuthorizedChatId { get; set; }
    }

    public class ConfigService
    {
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CursorKeep", "config.json");

        private AppConfig _config;

        public ConfigService() => _config = Load();

        public string BotToken => _config.BotToken;
        public long? AuthorizedChatId => _config.AuthorizedChatId;

        public void SaveToken(string token)
        {
            _config.BotToken = token;
            Save();
        }

        public void SaveAuthorizedChatId(long chatId)
        {
            _config.AuthorizedChatId = chatId;
            Save();
        }

        private AppConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                    return JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(ConfigPath)) ?? new AppConfig();
            }
            catch { }
            return new AppConfig();
        }

        private void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
