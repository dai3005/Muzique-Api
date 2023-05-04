namespace Muzique_Api.Helpers
{
    public class DeleteFile
    {
        IWebHostEnvironment _env;

        public DeleteFile(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task DeleteFileAsync(string file)
        {
            string fullPath = _env.WebRootPath + file;

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
}
}
