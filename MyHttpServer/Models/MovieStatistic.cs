namespace Server.Models;

public class MovieStatistic
{
    // Уникальный идентификатор
    public int Id { get; set; }

    // Рейтинг на КиноПоиске
    public decimal KP_Rating { get; set; }

    // Рейтинг на IMDB
    public decimal IMDB_Rating { get; set; }

    // Количество лайков
    public int Likes_Count { get; set; }

    // Количество дизлайков
    public int Dislikes_Count { get; set; }

    // Идентификатор фильма (внешний ключ)
    public int MovieId { get; set; }
}