namespace Muzique_Api.Models
{
    public class SongAndPlaylist
    {
        public int songAndPlaylistId { get; set; }
        public int playlistId { get; set; }
        public string songId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
