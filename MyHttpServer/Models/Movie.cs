using System.Text.Json.Serialization;

namespace Server.Models;

public class Movie
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("rutitle")]
    public string RuTitle { get; set; }
    
    [JsonPropertyName("releaseyear")]
    public int ReleaseYear { get; set; }
    
    [JsonPropertyName("imagesource")]
    public string ImageSource { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; }
}