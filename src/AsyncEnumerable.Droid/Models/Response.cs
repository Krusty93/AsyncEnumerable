using System.Text.Json.Serialization;

namespace AsyncEnumerable.Client.Models;

public class Response
{
    [JsonPropertyName("value")]
    public int Value { get; set; }
}
