using System.Text.Json.Serialization;

// used to deserialize json, 'record' field in a response
public class Recording
{
    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("artist-credit")]
    public List<Artist> Artist { get; set; }
}