using System.Text.Json.Serialization;

namespace Server.Models;

public class MovieDetails
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("moviedescription")]
    public string MovieDescription { get; set; }
    
    [JsonPropertyName("country")]
    public string Country { get; set; }
    
    [JsonPropertyName("producerid")]
    public int ProducerId { get; set; }
    
    [JsonPropertyName("engtitle")]
    public string EngTitle { get; set; }
    
    [JsonPropertyName("movieid")]
    public int MovieId { get; set; }
    
    [JsonPropertyName("quality")]
    public string Quality { get; set; }
    
    [JsonPropertyName("videourl")]
    public string VideoURL { get; set; }
}