namespace Muzique_Api.Models
{
    public class Playlist
    {
        public int playlistId { get; set; }
        public string name { get; set; }
        public string nameSearch { get; set; }
        public string description { get; set; }
        public string coverImageUrl { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string type { get; set; }
        public int userId { get; set; }
        public List<int>? listSongId { get; set; }
    }

    public class PlaylistViewModel
    {
        public List<Playlist> ListData { get; set; }
        public int TotalRes { get; set; }
    }
}
