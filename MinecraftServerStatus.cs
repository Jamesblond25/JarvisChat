using System.Text.Json;
using System.Text.Json.Serialization;

public class MinecraftServerStatus
{
    public string? Hostname { get; set; }

    [JsonPropertyName("motd")]
    public Motd? MotdData { get; set; }

    [JsonPropertyName("players")]
    public Players? Players { get; set; }

    [JsonPropertyName("online")]
    public bool Online { get; set; }

    public static async Task<string> CheckMinecraftServerAsync(string address, HttpClient client)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.mcsrvstat.us/3/{address}");
            request.Headers.UserAgent.ParseAdd("JarvisAI/1.0");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var status = JsonSerializer.Deserialize<MinecraftServerStatus>(json);

            if (status == null || !status.Online)
                return $"The Minecraft server at {address} is currently offline.";

            string motd = status.MotdData?.Clean != null && status.MotdData.Clean.Length > 0
                ? string.Join(" ", status.MotdData.Clean)
                : "No MOTD";

            string players = $"{status.Players.Online}/{status.Players.Max}";

            return $"Server: {status.Hostname}\nMOTD: {motd}\nPlayers: {players}";
        }
        catch (Exception ex)
        {
            return $"Failed to check server: {ex.Message}";
        }
    }
}

public class Motd
{
    [JsonPropertyName("clean")]
    public string[]? Clean { get; set; }
}


public class Players
{
    [JsonPropertyName("online")]
    public int Online { get; set; }
    [JsonPropertyName("max")]
    public int Max { get; set; }
}

