namespace Muzique_Api.Models
{
    public class AppSetting
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string Default { get; set; }
    }
}
