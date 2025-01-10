using System.Text.Json.Serialization;

namespace Server.Models;

public class CardReaction
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("likescount")]
    public int Likes_Count { get; set; }
    
    [JsonPropertyName("dislikescount")]
    public int Dislikes_Count { get; set; }
}