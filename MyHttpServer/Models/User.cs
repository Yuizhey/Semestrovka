using System.Text.Json.Serialization;

namespace Server.Models;

public class User
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Email{ get; set; }
}