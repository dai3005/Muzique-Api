namespace Muzique_Api.Models
{
    public class User
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string? password { get; set; }
        public string? nameSearch { get; set; }
        public string? coverImageUrl { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get;set; }
    }

    public class UserEmail
    {
        public string email { get; set; }

    }

    public class UserUpdate
    {
        public int userId { get; set; }
        public string? name { get; set; }
        public string? nameSearch { get; set; }
        public string? coverImageUrl { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class ChangePassword
    {
        public int? userId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public DateTime? updatedAt { get; set; }
    }
    public class UserViewModel
    {
        public List<User> ListData { get; set; }
        public int TotalRes { get; set; }
    }

    public class UserLogin
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    class DataObject
    {
        public int id { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class HistoryModel
    {
        public int? userId { get; set; }
        public int objectId { get; set; }
        public string type { get; set; }
    }

    public class HistorySong
    {
        public int userHistorySongId { get; set; }
        public int? userId { get; set; }
        public int songId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class LikeSong
    {
        public int userLikeSongId { get; set; }
        public int? userId { get; set; }
        public int songId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class HistoryAlbum
    {
        public int userHistoryAlbumId { get; set; }
        public int? userId { get; set; }
        public int albumId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class LikeAlbum
    {
        public int userLikeAlbumId { get; set; }
        public int? userId { get; set; }
        public int albumId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class HistoryArtist
    {
        public int userHistoryArtistId { get; set; }
        public int? userId { get; set; }
        public int artistId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
    public class LikeArtist
    {
        public int userLikeArtistId { get; set; }
        public int? userId { get; set; }
        public int artistId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class HistoryPlaylist
    {
        public int userHistoryPlaylistId { get; set; }
        public int? userId { get; set; }
        public int playlistId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
    public class LikePlaylist
    {
        public int userHistoryPlaylistId { get; set; }
        public int? userId { get; set; }
        public int playlistId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
