namespace MyHttpServer.Helpers;

public static class TemplateStorage
{
    public static string MovieCardTemplate => @"
        <div class='cards-item' onclick='location.href=""card?id={id}""'>
            <div class='cards-item-year'>
                <p class='cards-item-year-info'>{releaseyear}</p>
            </div>
            <img src='{imagesource}' alt='' class='cards-item-img'>
            <div class='cards-item-info'>
                <p class='cards-item-info-name'>{rutitle}</p>
            </div>
            <div class='cards-item-marks'>
                <p class='cards-item-marks-first'>КП <span class='cards-item-mark'>{kp_rating}</span></p>
                <p class='cards-item-marks-second'>IMDB <span class='cards-item-mark'>{imdb_rating}</span></p>
            </div>
        </div>";
    
    public static string MovieDetailsTemplate => @"
        <div class='main-filmAbout-image'>
            <div class='main-filmAbout-image-photo'>
                <div class='main-photo-year'>
                    <p class='main-photo-year-info'>{releaseyear}</p>
                </div>
                <img src='{imagesource}' alt='' class='main-photo-image'>
            </div>
            <div class='main-filmAbout-image-marks'>
                <div class='main-marks-positive'>
                    <img src='images/like.png' alt='' class='main-marks-positive-image'>
                    <p class='main-marks-positive-count'>{likes_count}</p>
                </div>
                <div class='main-marks-negative'>
                    <p class='main-marks-negative-count'>{dislikes_count}</p>
                    <img src='images/dislike.png' alt='' class='main-marks-negative-image'>                 
                </div>
            </div>
        </div>

        <div class='main-filmAbout-info'>
            <h1 class='main-filmAbout-info-title'>{title} ({releaseyear}) Смотреть онлайн</h1>
            <p class='main-filmAbout-info-description'>{description}</p>
            <div class='main-filmAbout-info-full'>
                <div class='main-filmAbout-info-fullFirst'>
                    {additionalInfo}
                </div>
                <div class='main-filmAbout-info-fullSecond'>
                    <div class='fullSecond-quality'>
                        <p class='fullSecond-quality-1'>Качество:</p>
                        <p class='fullSecond-quality-2'>{quality}</p>
                    </div>
                    <div class='fullSecond-marks'>
                        <div class='fullSecond-marksKP'>
                            КП {kp_rating}
                        </div>
                        <div class='fullSecond-marksIMDB'>
                            IMDB {imdb_rating}
                        </div>
                    </div>
                </div>
            </div>
        </div>";

    public static string UnauthorizedPlaceholder => "ВОЙТИ";
    public static string AuthorizedPlaceholder => "КАБИНЕТ";
    public static string AdminUsersTable => @"
                <tr>
                    <td>{id}</td>
                    <td>{login}</td>
                    <td>{password}</td>
                    <td>{email}</td>
                </tr>";
    
    public static string AdminProducerTable => @"
                <tr>
                    <td>{id}</td>
                    <td>{name}</td>
                    <td>{directedfilmscount}</td>
                    <td>{birthyear}</td>
                    <td>{birthcountry}</td>
                </tr>";
    
    public static string AdminMoviesTable => @"
                <tr>
                    <td>{id}</td>
                    <td>{rutitle}</td>
                    <td>{releaseyear}</td>
                    <td>{imagesource}</td>
                    <td>{status}</td>
                </tr>";
    
    public static string AdminMovieDetailsTable => @"
                <tr>
                    <td>{id}</td>
                    <td>{moviedescription}</td>
                    <td>{country}</td>
                    <td>{producerid}</td>
                    <td>{engtitle}</td>
                    <td>{movieid}</td>
                    <td>{quality}</td>
                    <td>{videourl}</td>
                </tr>";
    
    public static string AdminMovieStatsTable => @"
                <tr>
                    <td>{id}</td>
                    <td>{kp_rating}</td>
                    <td>{imdb_rating}</td>
                    <td>{likes_count}</td>
                    <td>{dislikes_count}</td>
                    <td>{movie_id}</td>
                </tr>";
}