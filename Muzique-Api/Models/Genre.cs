namespace Muzique_Api.Models
{
    public class Genre
    {
        public int genreId { get; set; }
        public string name { get; set; }
        public string nameSearch { get; set; }
        public string description { get; set; }
        public string coverImageUrl { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class GenreViewModel
    {
        public List<Genre> ListData { get; set; }
        public int TotalRes { get; set; }
    }
}
