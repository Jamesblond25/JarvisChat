
namespace JarvisChat.Configs
{
    public class AgentConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string PromptTemplate { get; set; }
        public string Model { get; set; }
        public string? DataURL { get; set; }
        public string? Script { get; set; }  // <-- Add this
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
    }
}