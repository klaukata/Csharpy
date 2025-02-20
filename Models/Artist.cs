using System.Text.Json.Serialization;

// used to deserialize json, artist.name field in a response
public class Artist
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}