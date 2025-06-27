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
        foreach (var pair in inputVars)
        {
            url = url.Replace("{" + pair.Key + "}", pair.Value, StringComparison.OrdinalIgnoreCase);
        }

        Console.WriteLine($"Fetching URL: {url}");

        var request = new HttpRequestMessage(body == null ? HttpMethod.Get : HttpMethod.Post, url);

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        if (body != null)
        {
            request.Content = body;
        }

        try
        {
            var response = await client.SendAsync(request);

            // If failure status, read the content to see details
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}.\nResponse content: {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response received");
            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            throw;
        }
    }

}
