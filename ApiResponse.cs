using System.Text.Json.Serialization;

// used to deserialize json, 'recording' field in a response
public class ApiResponse
{
    [JsonPropertyName("recordings")]
    public List<Recording> Recordings { get; set; }
}