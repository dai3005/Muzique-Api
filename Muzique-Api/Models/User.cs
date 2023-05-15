namespace Muzique_Api.Models
{
    public class User
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string? password { get; set; }
        public string nameSearch { get; set; }
        public string coverImageUrl { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get;set; }
    }

    public class UserViewModel
    {
        public List<User> ListData { get; set; }
        public int TotalRes { get; set; }
    }
}
