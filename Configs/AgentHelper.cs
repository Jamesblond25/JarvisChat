using System.Text;
namespace JarvisChat.Configs
{
    public static class AgentHelpers
    {
        public static async Task<string> FetchDataFromUrlAsync(
            string url,
            Dictionary<string, string> inputVars,
            HttpClient client,
            Dictionary<string, string>? headers = null,
            HttpContent? bodyString = null,
            string? bodyContent = null)
        {
            // Replace URL placeholders if any
            foreach (var pair in inputVars)
            {
                url = url.Replace("{" + pair.Key + "}", pair.Value, StringComparison.OrdinalIgnoreCase);
            }

            Console.WriteLine($"Fetching URL: {url}");

            var request = new HttpRequestMessage(
                bodyString == null ? HttpMethod.Get : HttpMethod.Post,
                url);

            // Add headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    // For User-Agent and other special headers, TryAddWithoutValidation is safer
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Add body content if present
            if (bodyString != null)
            {
                request.Content = bodyString;
            }

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}. Response content: {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response received");

            return responseBody;
        }


    }
}

