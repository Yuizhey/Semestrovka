namespace Server.Models;

public class MainPageMovie
{
    public int Id { get; set; }
    public string RuTitle { get; set; }
    public int ReleaseYear { get; set; }
    public string ImageSource { get; set; }
    public decimal KP_Rating { get; set; }
    
    public decimal IMDB_Rating { get; set; }
}