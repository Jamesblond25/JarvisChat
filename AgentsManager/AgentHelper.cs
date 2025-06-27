using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgentsManager
{
    public class AgentHelper
    {
        public static List<HttpAgentConfig> Agents { get; private set; } = new();

        public static void LoadHttpAgents(string fileName = "agents.json")
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Could not find agent config file.", fileName);

            var json = File.ReadAllText(fileName);
            Agents = JsonSerializer.Deserialize<List<HttpAgentConfig>>(json) ?? new List<HttpAgentConfig>();
        }




        public static async Task<string> ExecuteAgentHttp(string agentName)
        {
            var agent = Agents.FirstOrDefault(a =>
                a.Name.Equals(agentName, StringComparison.OrdinalIgnoreCase));

            if (agent == null)
                throw new Exception($"Agent with name '{agentName}' not found.");

            using var client = new HttpClient();

            if (agent.Headers != null)
            {
                foreach (var header in agent.Headers)
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            var content = new StringContent(agent.Body ?? "", Encoding.UTF8, "application/json");
            var response = await client.PostAsync(agent.DataURL, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
