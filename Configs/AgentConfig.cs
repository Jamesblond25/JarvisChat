
namespace JarvisChat.Configs
{
    public class AgentConfig
    {
        public string Name { get; set; }
        public string PromptTemplate { get; set; }
        public string Model { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string? DataURL { get; set; }
        public Dictionary<string, string>? Headers { get; set; }  // Ensure this line exists
        public string? Body { get; set; }
    }
}

