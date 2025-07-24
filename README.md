# JarvisChat

**JarvisChat** is a personal AI assistant designed to run on Windows, with support for **text** and **voice interaction**. Built using C# and integrated with [Ollama](https://ollama.com) for local LLM (large language model) chat, Jarvis is extensible with custom agents, file-based memory, and planned remote control via Android.

---

## ✨ Features

- 🎙️ Voice + Text chat support
- 🤖 Local LLM integration using Ollama
- 🧠 JSON-configured agent system
- 🗃️ Persistent memory via local storage
- 🧩 Plugin-based architecture

> 🧭 **Planned:**
> - 📅 WPF Frontend For The End User
> - 📱 Android remote control app

---

## 🛠️ Installation

### 1. Requirements

- .NET 6 or later
- [Ollama](https://ollama.com) installed and running
- Windows 10/11
- Microphone (for voice features)

### 2. Clone the Repository

```bash
git clone https://github.com/Jamesblond25/JarvisChat.git
cd JarvisChat
3. Build and Run
You can open the solution in Visual Studio, or use the .NET CLI:

bash
Copy
Edit
dotnet build
dotnet run --project JarvisChat
Ensure ollama is running a local model such as llama3:

bash
Copy
Edit
ollama run llama3
🧠 How It Works
You speak or type a message

Jarvis sends the message to a local LLM (via Ollama)

It processes the response and routes through agents or plugins

If voice is enabled, it speaks the answer back to you

📁 Project Structure
bash
Copy
Edit
JarvisChat/
├── Agents/                # Agent logic (configurable via JSON)
├── Plugins/               # Extendable plugin system
├── UI/                    # Console or planned WPF frontend
├── Data/                  # Persistent memory and settings
├── AgentConfigs/          # JSON configs for custom agents
└── Program.cs
🧪 Planned Features
Feature	Status	Description
📱 Android App	Planned	WireGuard-powered control app for Jarvis on PC
📅 WPF FrontEnd
🔐 Authentication	Planned	Basic local user access controls
🌐 Web Interface	Planned	Remote control and chat via browser
```
🙌 Acknowledgments
Ollama – Local LLM framework

.NET – Development platform

System.Speech – Voice synthesis
