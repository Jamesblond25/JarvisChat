using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JarvisChat.Configs
{
    public class AgentConfig
    {
        public string Name { get; set; }

        public string PromptTemplate { get; set; }

        public string Model { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        [JsonPropertyName("DataURL")]
        public string? DataURL { get; set; }

        // New: optional headers dictionary
        public Dictionary<string, string>? Headers { get; set; }

        // New: optional body string (e.g., JSON)
        public string? Body { get; set; }
    }

}
