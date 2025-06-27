using JarvisChat.Configs;
using System.Text.Json;

public static class AgentLoader
{
    public static List<AgentConfig> LoadAgents(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<AgentConfig>>(json);
    }
}
