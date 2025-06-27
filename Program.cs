using AgentManager;
using LLMManager;

class Program
{
    static async Task Main(string[] args)
    {
        AgentRegistry.Load();


        var agent = AgentRegistry.Find("mistral");
        if (agent == null)
        {
            Console.WriteLine("Agent not found.");
            return;
        }

        var input = "The quick brown fox jumps over the lazy dog.";
        var finalPrompt = agent.PromptTemplate.Replace("{{input}}", input);

        var llm = new OllamaClient();
        var output = await llm.SendPrompt(agent.Model, finalPrompt);

        Console.WriteLine("Response:");
        Console.WriteLine(output);
    }
}
