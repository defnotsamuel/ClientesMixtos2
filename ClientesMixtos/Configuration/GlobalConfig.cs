using System.IO;
using System.Text.Json;

namespace ClientesMixtos.Configuration
{
    public static class GlobalConfig
    {

        public class Config
        {
            public string ConnectionString { get; set; } = "mongodb://localhost:27017";
            public string DatabaseName { get; set; } = "ClientesMixtos";
            public string Theme { get; set; } = "Light";

            public void LoadFromFile(string filePath)
            {
                if (File.Exists(filePath))
                {
                    var configJson = File.ReadAllText(filePath);
                    var config = JsonSerializer.Deserialize<Config>(configJson);
                    if (config is not null)
                    {
                        ConnectionString = config.ConnectionString;
                        DatabaseName = config.DatabaseName;
                        Theme = config.Theme;
                    }
                } else
                {
                    // If the file does not exist, create it with default values
                    SaveToFile(filePath);
                }
            }

            public void SaveToFile(string filePath)
            {
                var configJson = JsonSerializer.Serialize(this);
                File.WriteAllText(filePath, configJson);
            }
        }

        private static readonly Config _config = new();

        static GlobalConfig()
        {
            _config.LoadFromFile("config.json");
        }


        public static void SetConnectionString(string connectionString)
        {
            _config.ConnectionString = connectionString;
        }

        public static void SetDatabaseName(string databaseName)
        {
            _config.DatabaseName = databaseName;
        }

        public static void SetTheme(string theme)
        {
            _config.Theme = theme;
        }

        public static string ConnectionString()
        {
            return _config.ConnectionString;
        }

        public static string DatabaseName()
        {
            return _config.DatabaseName;
        }

        public static string Theme()
        {
            return _config.Theme;
        }


        public static void SaveConfig()
        {
            _config.SaveToFile("config.json");
        }
    }
}
