using System.Text.Json.Serialization;

namespace Server.Models;

public class DeleteRequestModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}