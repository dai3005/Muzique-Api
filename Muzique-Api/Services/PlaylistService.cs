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
            string query = " from `playlist` where type='system' ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and nameSearch like @keyword ";
            }
            PlaylistViewModel playlistViewModel = new PlaylistViewModel();
            playlistViewModel.ListData = new List<Playlist>();
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
            if (rowperpage <= 0)
            {
                query += " order by createdAt desc";
            }
            else
            {
                query += " order by createdAt desc limit " + pageOffSet + "," + rowperpage;
            }
            playlistViewModel.ListData = this._connection.Query<Playlist>(querySelect + query, new { keyword = keyword }).ToList();

            return playlistViewModel;
        }

        public Playlist GetPlaylistById(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `playlist` where playlistId = @id";
            return this._connection.Query<Playlist>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertPlaylist(Playlist model)
        {
            string insert = "INSERT INTO `playlist`(`playlistId`, `name`, `description`, `coverImageUrl`, `createdAt`, `nameSearch`,`type`,`userId`)" +
                " VALUES (null,@name,@description,@coverImageUrl,@createdAt,@nameSearch,@type,@userId)";
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

        public bool DeletePlaylistSong(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song_and_playlist` WHERE playlistId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public object GetPlaylistDetail(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `playlist` where playlistId = @id";
            string queryListSong = "select songId from `song_and_playlist` where playlistId=@id";
            object playlist = this._connection.Query<object>(query, new { id }, transaction).FirstOrDefault();
            List<int> listSongId = this._connection.Query<int>(queryListSong, new { id }, transaction).ToList();
            return new
            {
                playlist,
                listSongId
            };
        }

        public List<int> GetListSongByPlaylistId(int id, IDbTransaction transaction = null)
        {
            string query = "select songId from `song_and_playlist` where playlistId=@id";
            List<int> listSongIds = this._connection.Query<int>(query, new { id }, transaction).ToList();
            return listSongIds;
        }

        public bool UpdatePlaylistName(Playlist model, IDbTransaction transaction = null)
        {
            string query = "UPDATE `playlist` SET `name`=@name WHERE playlistID = @playlistId and userId = @userId";
            int status = this._connection.Execute(query, model,transaction);
            return status > 0;
        }

        public bool UpdatePlaylistImage(Playlist model, IDbTransaction transaction = null)
        {
            string query = "UPDATE `playlist` SET `coverImageUrl`=@coverImageUrl,`updatedAt`=@updatedAt WHERE playlistID = @playlistId and userId = @userId";
            int status = this._connection.Execute(query, model,transaction);
            return status > 0;
        }

        public bool AddSongToPlaylist(SongPlaylist model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `song_and_playlist`(`songAndPlaylistId`, `songId`, `playlistId`, `createdAt`) VALUES (null,@songId,@playlistId,@createdAt)";
            int status = this._connection.Execute(insert, model,transaction);
            return status > 0;
        }

        public bool DeleteSongFromPlaylist(SongPlaylist model, IDbTransaction transaction = null)
        {
            string query = "DELETE FROM `song_and_playlist` WHERE playlistId = @playlistId and songId = @songId";
            int status = this._connection.Execute(query, model, transaction);
            return status > 0;
        }
    }
}
