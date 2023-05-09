using Dapper;
using Muzique_Api.Models;
using System.Data;

namespace Muzique_Api.Services
{
    public class PlaylistService : BaseService
    {

        public PlaylistService() : base() { }

        public PlaylistService(IDbConnection db) : base(db) { }

        public PlaylistViewModel GetListPlaylist(int page, int rowperpage, string keyword)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from `playlist` where 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and nameSearch like @keyword ";
            }
            PlaylistViewModel playlistViewModel = new PlaylistViewModel();
            playlistViewModel.ListPlaylist = new List<Playlist>();
            playlistViewModel.TotalRes = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                playlistViewModel.TotalRes = totalRows;
            }
            if (page == 0)
            {
                page = 1;
            }
            int pageOffSet = (page - 1) * rowperpage;
            if (rowperpage < 0)
            {
                query += " order by createdAt desc";
            }
            else
            {
                query += " order by createdAt desc limit " + pageOffSet + "," + rowperpage;
            }
            playlistViewModel.ListPlaylist = this._connection.Query<Playlist>(querySelect + query, new { keyword = keyword }).ToList();

            return playlistViewModel;
        }

        public Playlist GetPlaylistDetail(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `playlist` where playlistId = @id";
            return this._connection.Query<Playlist>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertPlaylist(Playlist model)
        {
            string insert = "INSERT INTO `playlist`(`playlistId`, `name`, `description`, `coverImageUrl`, `createdAt`, `nameSearch`,`type`,`userId`)" +
                " VALUES (null,@name,@description,@coverImageUrl,@createdAt,@nameSearch,SYSTEM,null)";
            int status = this._connection.Execute(insert, model);
            return status > 0;
        }

        public bool UpdatePlaylist(Playlist model)
        {
            string query = "UPDATE `playlist` SET `name`=@name,`coverImageUrl`=@coverImageUrl,`updatedAt`=@updatedAt,`nameSearch`=@nameSearch" +
                ",`description`=@description WHERE playlistID = @playlistId";
            int status = this._connection.Execute(query, model);
            return status > 0;
        }

        public bool DeletePlaylist(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `playlist` WHERE playlistId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public bool DeleteGenreSong(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song_and_playlist` WHERE playlistId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }
    }
}
