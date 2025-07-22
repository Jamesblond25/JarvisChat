using System.Text.Json;
using System.Threading.Tasks;
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
            return newAgent;
        }

        public static async void DeleteAgent(string AgentName)
        {
            var listagents = await LoadAgentsAsync();
            foreach (var agentConfig in listagents.ToList())
            {
                if( agentConfig.Name.Equals(AgentName, StringComparison.OrdinalIgnoreCase))
                {
                    listagents.Remove(agentConfig);
                    Console.WriteLine($"{AgentName} Has been Deleted!");
                    SaveAgentsAsync(listagents).GetAwaiter().GetResult();
                    return;
                }
            }
            Console.WriteLine("Cant Find Agent to delete!");
        }
    }
}
