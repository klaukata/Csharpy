// for handling http requests to external api
public class Client
{
    private readonly HttpClient client;

    public Client()
    {
        client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Csharpy");  // required by MusicBrainz API
    }
    public async Task<string> GetTrackList(string trackName)
    {
        if (string.IsNullOrWhiteSpace(trackName))
        {
            Console.WriteLine("Track name cannot be null or empty.");
            return "";
        }
        try
        {
            string requestUri = $"https://musicbrainz.org/ws/2/recording?query={trackName}&limit=5&fmt=json";

            // send the request
            HttpResponseMessage response = await client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode(); // error if status code == 2xx
            return await response.Content.ReadAsStringAsync(); // return response as a string

        }
        catch (HttpRequestException ex)
        {
            // HTTP-specific errors (network issues, invalid responses)
            Console.WriteLine($"HTTP request failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            // other errors
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
        return ""; // empty string in case of a failure
    }
}

