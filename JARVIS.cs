using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using JarvisChat.Configs;

namespace JarvisChat
{


    public class Jarvis
    {
        private readonly List<AgentConfig> agentConfigs = AgentLoader.LoadAgents("Configs/Agents.json");
        private AgentRunner? runner;


        private readonly HttpClient client = new() { Timeout = Timeout.InfiniteTimeSpan };
        private readonly SpeechSynthesizer tts = new();
        private Process? ollamaProcess;
        private readonly List<Message> conversationHistory = new();

        private class Message
        {
            [JsonPropertyName("role")]
            public string? Role { get; set; }

            [JsonPropertyName("content")]
            public string? Content { get; set; }
        }

        public async Task RunAsync()
        {
            StartOllama();
            InitializeTTS();
            runner = new AgentRunner(agentConfigs);

            Console.Title = "J.A.R.V.I.S";
            Console.WriteLine("=== Jarvis AI Assistant ===");
            Console.WriteLine("Type 'exit' to quit or 'Jarvis:<command>' to trigger an agent action.\n");
            Console.WriteLine("Are we using voice or text?");
            var mode = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "text";

            if (mode == "voice")
                await RunVoiceModeLoop();
            else
                await RunTextModeLoop();
        }

        private void StartOllama()
        {
            ollamaProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = "run jarvis",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            ollamaProcess.Start();
        }

        private void InitializeTTS()
        {
            tts.SelectVoice("Microsoft David Desktop");
            tts.Rate = 3;
        }

        private async Task RunTextModeLoop()
        {
            while (true)
            {
                Console.Write("You: ");
                var userInput = Console.ReadLine();

                if (userInput == null) continue;

                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    await ShutdownOllama();
                    break;
                }

                // Try handle as agent command first
                bool handled = HandleAgentCommand(userInput);
                if (handled) continue;

                conversationHistory.Add(new Message { Role = "user", Content = userInput });

                var response = await ChatWithOllama();
                conversationHistory.Add(new Message { Role = "assistant", Content = response });

                Console.WriteLine("Jarvis: " + response + "\n");
                tts.SpeakAsync(response);
            }
        }
        private async Task RunVoiceModeLoop()
        {
            while (true)
            {
                var userInput = GetVoiceInput();
                Console.WriteLine("You (voice): " + userInput);

                if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (userInput.StartsWith("Jarvis:", StringComparison.OrdinalIgnoreCase))
                {
                    HandleAgentCommand(userInput.Substring("Jarvis:".Length).Trim());
                    continue;
                }

                conversationHistory.Add(new Message { Role = "user", Content = userInput });

                var response = await ChatWithOllama();
                conversationHistory.Add(new Message { Role = "assistant", Content = response });

                Console.WriteLine("Jarvis: " + response + "\n");
                tts.SpeakAsync(response);
            }
        }

        private async Task<string> ChatWithOllama()
        {
            var requestBody = new
            {
                model = "jarvis",
                stream = false,
                messages = conversationHistory
            };

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:11434/api/chat", content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseString);
                return doc.RootElement.GetProperty("message").GetProperty("content").GetString() ?? "";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task ShutdownOllama()
        {
            Console.WriteLine("Jarvis: Shutting down...");
            try
            {
                var stopContent = new StringContent("{\"name\":\"jarvis\"}", Encoding.UTF8, "application/json");
                var stopResponse = await client.PostAsync("http://localhost:11434/api/stop", stopContent);

                if (stopResponse.IsSuccessStatusCode)
                    Console.WriteLine("Ollama model stopped via API.");
                else
                    Console.WriteLine($"Failed to stop model via API: {stopResponse.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API stop call failed: {ex.Message}");
            }
            foreach (var proc in Process.GetProcessesByName("ollama"))
            {
                try
                {
                    proc.Kill(true);
                    proc.WaitForExit();
                    proc.Dispose();
                    Console.WriteLine($"Killed process {proc.Id} ({proc.ProcessName})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private string GetVoiceInput()
        {
            using var recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.LoadGrammar(new DictationGrammar());

            Console.WriteLine("(Listening...)");

            try
            {
                var result = recognizer.Recognize(TimeSpan.FromSeconds(10));
                return result?.Text ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Jarvis] Voice recognition error: " + ex.Message);
                return "";
            }
        }

        private bool HandleAgentCommand(string command)
        {
            if (runner == null) return false;
            try
            {
                var parts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return false;

                var agentName = parts[0];
                var inputVars = new Dictionary<string, string>();

                var config = agentConfigs.FirstOrDefault(a => a.Name.Equals(agentName, StringComparison.OrdinalIgnoreCase));
                if (config == null) return false;
                string agentresult = runner.RunAgentAsync(agentName).GetAwaiter().GetResult();
                string result = config.PromptTemplate + agentresult;

                conversationHistory.Add(new Jarvis.Message { Role = "user", Content = result });

                var respomse = ChatWithOllama().GetAwaiter().GetResult();

                conversationHistory.Add(new Jarvis.Message { Role = "assistant", Content = respomse });

                Console.WriteLine("\nJarvis: " + respomse + "\n");
                tts.SpeakAsync(respomse);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error running agent: " + ex.Message);
                return false;
            }
        }

        public float avg(List<float> values)
        {
            float total = 0;
            foreach (var v in values)
            {
                total += v;
            }
            float avg = total / values.Count;
            float avg2 = (float)Math.Round(avg, 2);
            return avg2;
        }
    }


}