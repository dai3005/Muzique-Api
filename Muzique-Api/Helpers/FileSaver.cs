namespace Muzique_Api.Helpers
{
    public class FileSaver
    {
        IWebHostEnvironment _env;

        public FileSaver(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> FileSaveAsync(IFormFile file, string filePath)
        {
            string filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string route = Path.Combine(_env.WebRootPath, filePath);

            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }

            string fileRoute = Path.Combine(route, filename);

            using(FileStream fs = File.Create(fileRoute))
            {
                await file.OpenReadStream().CopyToAsync(fs);
            }

            string[] arrfileRoute = fileRoute.Split("wwwroot\\");

            string fileDetail ="/" + arrfileRoute[1];

            return fileDetail;
        }
    }
}
