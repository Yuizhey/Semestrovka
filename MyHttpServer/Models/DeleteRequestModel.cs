using System.Text.Json.Serialization;

namespace Server.Models;

public class DeleteRequestModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}