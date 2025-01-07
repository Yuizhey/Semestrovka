namespace Server.Models;

public class CardPageMovie
{
    public string RuTitle { get; set; }
    public string EngTitle { get; set; }
    public int ReleaseYear { get; set; }
    public string ImageSource { get; set; }
    public decimal KP_Rating { get; set; }
    public decimal IMDB_Rating { get; set; }
    public int Likes_Count { get; set; }
    public int Dislikes_Count { get; set; }
    public string MovieDescription { get; set; }
    public string Country { get; set; }
    public string Name { get; set; }
    public string Quality { get; set; }
    public string VideoURL { get; set; }
}