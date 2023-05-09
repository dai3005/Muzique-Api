namespace Muzique_Api.Models
{
    public class SongAndGenre
    {
        public int songAndGenreId { get; set; }
        public int genreId { get; set; }
        public int songId { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
