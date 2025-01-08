using System.Text.Json.Serialization;

namespace Server.Models;

public class DeleteUserRequestModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}