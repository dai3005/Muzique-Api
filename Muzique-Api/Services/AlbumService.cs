using Dapper;
using Muzique_Api.Models;
using System.Data;

namespace Muzique_Api.Services
{
    public class AlbumService : BaseService
    {
        public AlbumService() : base() { }

        public AlbumService(IDbConnection db) : base(db) { }

        public AlbumViewModel GetListAlbum(int page, int rowperpage, string keyword)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from `album` where 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and nameSearch like @keyword ";
            }
            AlbumViewModel albumViewModel = new AlbumViewModel();
            albumViewModel.ListData = new List<Album>();
            albumViewModel.TotalRes = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                albumViewModel.TotalRes = totalRows;
            }
            if (page == 0)
            {
                page = 1;
            }
            int pageOffSet = (page - 1) * rowperpage;
            if (rowperpage <= 0)
            {
                query += " order by createdAt desc";
            }
            else
            {
                query += " order by createdAt desc limit " + pageOffSet + "," + rowperpage;
            }
            albumViewModel.ListData = this._connection.Query<Album>(querySelect + query, new { keyword = keyword }).ToList();

            return albumViewModel;
        }

        public Album GetAlbumDetail(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `album` where albumId = @id";
            return this._connection.Query<Album>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertAlbum(Album model)
        {
            string insert = "INSERT INTO `album`(`albumId`, `name`, `description`, `coverImageUrl`, `createdAt`, `nameSearch`,`artistId`)" +
                " VALUES (null,@name,@description,@coverImageUrl,@createdAt,@nameSearch,@artistId)";
            int status = this._connection.Execute(insert, model);
            return status > 0;
        }

        public bool UpdateAlbum (Album model)
        {
            string query = "UPDATE `album` SET `name`=@name,`description`=@description,`coverImageUrl`=@coverImageUrl" +
                ",`updatedAt`=@updatedAt,`nameSearch`=@nameSearch,`artistId`=@artistId WHERE albumId = @albumId";
            int status = this._connection.Execute(query, model);
            return status > 0;
        }

        public bool DeleteAlbum(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `album` WHERE albumId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public bool DeleteAlbumSong(int id, IDbTransaction transaction = null)
        {
            string update = "UPDATE `song` SET `albumId`= 0 WHERE albumId = @id;";
            int status = this._connection.Execute(update, new { id = id }, transaction);
            return status > 0;
        }
    }
}
