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

            string? bodyContent = string.IsNullOrWhiteSpace(agent.Body) ? null : agent.Body;

            string result = await AgentHelpers.FetchDataFromUrlAsync(
                agent.DataURL ?? throw new System.Exception("DataURL is missing."),
                new System.Collections.Generic.Dictionary<string, string>(),
                _client,
                agent.Headers,
                bodyContent == null ? null : new System.Net.Http.StringContent(
                    bodyContent,
                    Encoding.UTF8,
                    "application/json"
                ),
                bodyContent
            );
            
            return result;
        }
    }


}

