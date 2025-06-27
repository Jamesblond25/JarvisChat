using JarvisChat.Configs;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public static class AgentHelpers
{
    public static async Task<string> FetchDataFromUrlAsync(
        string url,
        Dictionary<string, string> inputVars,
        HttpClient client,
        Dictionary<string, string>? headers = null,
        HttpContent? body = null)
    {
        if (string.IsNullOrEmpty(url))
            throw new ArgumentException("URL must not be null or empty", nameof(url));
        if (client == null)
            throw new ArgumentNullException(nameof(client));
        if (inputVars == null)
            inputVars = new Dictionary<string, string>();

        // Replace placeholders in URL
        foreach (var pair in inputVars)
        {
            url = url.Replace("{" + pair.Key + "}", pair.Value, StringComparison.OrdinalIgnoreCase);
        }

        Console.WriteLine($"Fetching URL: {url}");

        var request = new HttpRequestMessage(
            body == null ? HttpMethod.Get : HttpMethod.Post,
            url);

        // Add headers
        if (headers != null)
        {
            foreach (var header in headers)
            {
                // Some headers like User-Agent must be added here safely
                if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    // Fallback: try adding to content headers if body is present
                    if (body != null)
                    {
                        body.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }
        }

        // If no User-Agent header provided, add a default one
        if (headers == null || !headers.ContainsKey("User-Agent"))
        {
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        }

        // Add body if present
        if (body != null)
        {
            request.Content = body;
        }

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"HTTP Request error: {e.Message}");
            throw;
        }

        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        Console.WriteLine("Response received");

        return responseBody;
    }
}
