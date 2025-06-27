using System.Text.Json;

namespace JarvisChat.Configs
{
    public static class AgentLoader
    {
        public static List<AgentConfig> LoadAgents(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<AgentConfig>>(json);
        }
    }
}

