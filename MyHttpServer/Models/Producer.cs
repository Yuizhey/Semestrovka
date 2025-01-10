using System.Text.Json.Serialization;

namespace Server.Models;

public class Producer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("directedfilmscount")]
    public int DirectedFilmsCount { get; set; }
    
    [JsonPropertyName("birthyear")]
    public int BirthYear{ get; set; }
    
    [JsonPropertyName("birthcountry")]
    public string BirthCountry{ get; set; }
}