using JarvisChat.Configs;
using System.Text.Json;
using System.Text;

public class AgentRunner
{
    private readonly Dictionary<string, AgentConfig> _agents;
    private readonly HttpClient _client;

    public AgentRunner(List<AgentConfig> configs, HttpClient sharedClient)
    {
        _agents = configs.ToDictionary(a => a.Name, a => a);
        _client = sharedClient;
    }

    public async Task<string> RunAgentAsync(string agentName)
    {
        if (!_agents.TryGetValue(agentName, out var config))
            throw new Exception($"Agent '{agentName}' not found.");

        var inputVars = new Dictionary<string, string>();
        Console.WriteLine($"DataUrl is: '{config.DataURL}'");

        // Extract placeholders from PromptTemplate and DataUrl using regex
        var promptTokens = ExtractPlaceholders(config.PromptTemplate); // likely just "data"
        var urlTokens = ExtractPlaceholders(config.DataURL); // empty now, no placeholders
        var allTokens = promptTokens.Concat(urlTokens).Distinct();

        // Since no tokens except "data", and "data" is skipped for input,
        // you won’t be prompted for anything.
        foreach (var token in allTokens)
        {
            if (token.Equals("data", StringComparison.OrdinalIgnoreCase))
                continue; // skip 'data'
                          // No other tokens => no prompts
        }


        // Fetch JSON data if DataUrl is set
        if (!string.IsNullOrEmpty(config.DataURL))
        {
            var jsonData = await AgentHelpers.FetchDataFromUrlAsync(config.DataURL, inputVars, _client);
            inputVars["data"] = jsonData;
        }

        // Build prompt by replacing placeholders with values (including injected data)
        string prompt = ApplyTemplate(config.PromptTemplate, inputVars);

        // Prepare request payload
        var request = new
        {
            model = config.Model,
            prompt = prompt,
            stream = false,
            options = config.Parameters
        };

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("http://localhost:11434/api/generate", content);
        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseText);
        return doc.RootElement.GetProperty("response").GetString() ?? "No content.";
    }

    // Regex-based placeholder extractor
    private IEnumerable<string> ExtractPlaceholders(string template)
    {
        if (string.IsNullOrEmpty(template))
            return Enumerable.Empty<string>();

        var matches = System.Text.RegularExpressions.Regex.Matches(template, @"\{(.*?)\}");
        return matches.Select(m => m.Groups[1].Value).Distinct();
    }

    // Simple placeholder replacement
    private string ApplyTemplate(string template, Dictionary<string, string> vars)
    {
        foreach (var pair in vars)
        {
            template = template.Replace("{" + pair.Key + "}", pair.Value, StringComparison.OrdinalIgnoreCase);
        }
        return template;
    }

}
