using System.Text.Json.Serialization;

namespace AgentsManager
{
    public class Header
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class HttpAgentConfig
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("promptTemplate")]
        public string PromptTemplate { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }

        [JsonPropertyName("dataURL")]
        public string? DataURL { get; set; }

        [JsonPropertyName("headers")]
        public List<Header>? Headers { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }
}
