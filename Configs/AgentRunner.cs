using System.Text;
using System.Text.Json;
using JarvisChat;

namespace JarvisChat.Configs
{
    public class AgentRunner
    {
        private readonly System.Net.Http.HttpClient _client;
        private readonly System.Collections.Generic.List<JarvisChat.Configs.AgentConfig> _agents;

        public AgentRunner(System.Collections.Generic.List<JarvisChat.Configs.AgentConfig> agents)
        {
            _client = new System.Net.Http.HttpClient();
            _agents = agents;
        }

        public async System.Threading.Tasks.Task<string> RunAgentAsync(string agentName)
        {
            var agent = _agents.Find(a => a.Name.Equals(agentName, System.StringComparison.OrdinalIgnoreCase));
            if (agent == null)
            {
                throw new System.Exception($"Agent '{agentName}' not found.");
            }

            if (string.IsNullOrWhiteSpace(agent.Type))
            {
                throw new System.Exception($"Agent '{agentName}' is missing a Type.");
            }

            string agentType = agent.Type.Trim().ToUpperInvariant();

            if (agentType == "HTTP")
            {
                string? bodyContent = string.IsNullOrWhiteSpace(agent.Body) ? null : agent.Body;

                return await AgentHelpers.FetchDataFromUrlAsync(
                    agent.DataURL ?? throw new System.Exception("DataURL is missing."),
                    new System.Collections.Generic.Dictionary<string, string>(),
                    _client,
                    agent.Headers,
                    bodyContent == null ? null : new System.Net.Http.StringContent(
                        bodyContent,
                        System.Text.Encoding.UTF8,
                        "application/json"
                    ),
                    bodyContent
                );
            }
            else if (agentType == "POWERSHELL")
            {
                string result = AgentHelpers.RunPowerShellScript(agent);
                // Replace {cpu} in the prompt with the PowerShell result
                return agent.PromptTemplate.Replace("{cpu}", result);
            }

            else
            {
                throw new System.Exception($"Agent type '{agent.Type}' is not supported.");
            }
        }
    }
}

