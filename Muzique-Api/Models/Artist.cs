namespace Muzique_Api.Models
{
    public class Artist
    {
        public int artistId { get; set; }
        public string name { get; set; }
        public string nameSearch { get; set; }
        public string description { get; set; }
        public string coverImageUrl { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public List<int>? listSongId { get; set; }
    }

    public class ArtistViewModel
    {
        public List<Artist> ListData { get; set; }
        public int TotalRes { get; set; }
    }
}
