using System.Text.Json.Serialization;

namespace Server.Models;

public class User
{
    [JsonIgnore]
    public int Id { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}