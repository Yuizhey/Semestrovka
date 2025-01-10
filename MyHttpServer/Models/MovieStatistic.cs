using System.Text.Json.Serialization;

namespace Server.Models;

public class MovieStatistic
{
    // Уникальный идентификатор
    [JsonPropertyName("id")]
    public int Id { get; set; }

    // Рейтинг на КиноПоиске
    [JsonPropertyName("kp_rating")]
    public decimal KP_Rating { get; set; }

    // Рейтинг на IMDB
    [JsonPropertyName("imdb_rating")]
    public decimal IMDB_Rating { get; set; }

    // Количество лайков
    [JsonPropertyName("likes_count")]
    public int Likes_Count { get; set; }

    // Количество дизлайков
    [JsonPropertyName("dislikes_count")]
    public int Dislikes_Count { get; set; }

    // Идентификатор фильма (внешний ключ)
    [JsonPropertyName("movie_id")]
    public int Movie_Id { get; set; }
}