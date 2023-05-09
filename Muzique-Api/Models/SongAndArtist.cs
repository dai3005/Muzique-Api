namespace Muzique_Api.Models
{
    public class SongAndArtist
    {
        public int songAndArtistId { get; set; }
        public int artistId { get; set; }
        public int songId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
