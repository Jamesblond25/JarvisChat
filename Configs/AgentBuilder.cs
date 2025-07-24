using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JarvisChat.Configs;

namespace JarvisChat
{
    public static class AgentBuilder
    {
        private static readonly HttpClient http = new();

        public static async Task<AgentConfig> BuildAgentFromPromptAsync(string userPrompt)
        {
            var systemPrompt = @"
You are an agent builder for a JARVIS-like AI assistant.
Based on the user's request, decide:
- If the agent should run a script (e.g., PowerShell)
- Or if it should call an external API if so find one.

Respond ONLY with a valid JSON that follows this structure and keep all data structures:
{
  ""Name"": string,
  ""Type"": string (either HTTP or Script)
  ""Description"": string,
  ""PromptTemplate"": string,
  ""Model"": Jarvis:latest,
  ""DataURL"": string or null,
  ""Script"": string or null,
  ""Headers"": { string: string },
  ""Body"": string or null
}
No explanation, no markdown — just raw JSON.";

            var requestBody = new
            {
                model = "Jarvis", // or any Ollama model you're running
                prompt = systemPrompt + "\n\nUser: " + userPrompt + "\n\nAgentConfig JSON:",
                stream = false
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await http.PostAsync("http://localhost:11434/api/generate", jsonContent);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Ollama response failed: {response.StatusCode}");

            var rawJson = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(rawJson);
            var rawText = document.RootElement.GetProperty("response").GetString();

            if (string.IsNullOrWhiteSpace(rawText))
                throw new Exception("Ollama response was empty or missing 'response' field.");

            // Strip optional backticks or ```json if present
            rawText = rawText.Trim().Trim('`');
            if (rawText.StartsWith("json", StringComparison.OrdinalIgnoreCase))
                rawText = rawText.Substring(4).Trim();

            // Now parse it as AgentConfig
            var agent = JsonSerializer.Deserialize<AgentConfig>(
                rawText,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return agent ?? throw new Exception("Failed to parse AgentConfig JSON.");
        }
    }
}
