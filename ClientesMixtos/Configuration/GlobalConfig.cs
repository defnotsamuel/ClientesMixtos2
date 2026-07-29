using System.IO;
using System.Text.Json;

namespace ClientesMixtos.Configuration
{
    public class GlobalConfig
    {
        public string ConnectionString { get; private set; } = "mongodb://localhost:27017";
        public string DatabaseName { get; private set; } = "ClientesMixtos";

        public GlobalConfig()
        {
            LoadFromFile("config.json");
        }

        public void SetConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void SetDatabaseName(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var configJson = File.ReadAllText(filePath);
                var config = JsonSerializer.Deserialize<ConfigDto>(configJson);
                if (config is not null)
                {
                    ConnectionString = config.ConnectionString;
                    DatabaseName = config.DatabaseName;
                }
            }
            else
            {
                SaveToFile(filePath);
            }
        }

        public void SaveToFile(string filePath)
        {
            var configJson = JsonSerializer.Serialize(new ConfigDto
            {
                ConnectionString = ConnectionString,
                DatabaseName = DatabaseName
            });
            File.WriteAllText(filePath, configJson);
        }

        public void Save()
        {
            SaveToFile("config.json");
        }

        private class ConfigDto
        {
            public string ConnectionString { get; set; } = "mongodb://localhost:27017";
            public string DatabaseName { get; set; } = "ClientesMixtos";
        }
    }
}
