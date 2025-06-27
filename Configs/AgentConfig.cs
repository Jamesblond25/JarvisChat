using System.Text.Json.Serialization;

public class AgentConfig
{
    public string Name { get; set; }

    public string PromptTemplate { get; set; }

    public string Model { get; set; }

    public Dictionary<string, string> Parameters { get; set; }

    [JsonPropertyName("DataURL")]
    public string? DataURL { get; set; }

    public Dictionary<string, string>? Headers { get; set; }

    public string? Body { get; set; }
}
