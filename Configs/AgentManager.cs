using System.Text.Json;
using JarvisChat.Configs;

namespace JarvisChat
{
    public static class AgentManager
    {
        private static readonly string AgentsFilePath = "C:\\Users\\niall\\Desktop\\Programming\\JarvisChat\\Configs\\Agents.json";
        private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

        public static async Task<List<AgentConfig>> LoadAgentsAsync()
        {
            try
            {
                if (!File.Exists(AgentsFilePath))
                {
                    Console.WriteLine("Agent file does not exist, creating new one.");
                    return new();
                }

                var json = await File.ReadAllTextAsync(AgentsFilePath);
                var agents = JsonSerializer.Deserialize<List<AgentConfig>>(json, Options) ?? new();
                Console.WriteLine($"Loaded {agents.Count} agents.");
                return agents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error reading agents file: {ex.Message}");
                return new();
            }
        }

        public static async Task SaveAgentsAsync(List<AgentConfig> agents)
        {
            try
            { 
                var json = JsonSerializer.Serialize(agents, Options);

                Console.WriteLine($"Saving {agents.Count} agents to {AgentsFilePath}");
                await File.WriteAllTextAsync(AgentsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to agents.json: {ex.Message}");
            }
        }

        public static async Task<AgentConfig?> AddAgentAsync(AgentConfig newAgent)
        {
            var agentslist = await LoadAgentsAsync();

            if (agentslist.Any(a => a.Name.Equals(newAgent.Name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Agent '{newAgent.Name}' already exists. Skipping add.");
                return null;
            }

            agentslist.Add(newAgent);
            int counter = 0;
            foreach (var agent in agentslist)
            {
                counter += 1;
            }
            Console.WriteLine(counter);
            return newAgent;
        }
    }
}
