using Dapper;
using Muzique_Api.Models;
using System.Data;

namespace Muzique_Api.Services
{
    public class UserService : BaseService
    {
        public UserService() : base() { }
        public UserService(IDbConnection db) : base(db) { }

        public UserViewModel GetListUser(int page, int rowperpage, string keyword)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from `user` where 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and (email like @keyword or nameSearch like @keyword)";
            }
            UserViewModel userViewModel = new UserViewModel();
            userViewModel.ListData = new List<User>();
            userViewModel.TotalRes = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                userViewModel.TotalRes = totalRows;
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
            userViewModel.ListData = this._connection.Query<User>(querySelect + query, new { keyword = keyword }).ToList();

            return userViewModel;
        }

        public User GetUserById(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `user` where userId = @id";
            return this._connection.Query<User>(query, new { id }, transaction).FirstOrDefault();
        }

        public User GetUserByEmail(string email, IDbTransaction transaction = null)
        {
            string query = "select * from `user` where email = @email";
            return this._connection.Query<User>(query, new { email }, transaction).FirstOrDefault();
        }

        public bool UpdateUser(User model)
        {
            string query = "UPDATE `user` SET `email`=@email,`name`=@name,`updatedAt`=@updatedAt," +
                "`nameSearch`=@nameSearch,`coverImageUrl`=@coverImageUrl WHERE userId = @userId";
            int status = this._connection.Execute(query, model);
            return status > 0;
        }

        public bool DeleteUser(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `user` WHERE userId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public List<int> GetListLikeSongId(int id, IDbTransaction transaction = null)
        {
            string query = "select songId from `user_like_song` where userId = @id";
            return this._connection.Query<int>(query, new { id }, transaction).ToList();
        }

        public List<int> GetListLikeAlbumId(int id, IDbTransaction transaction = null)
        {
            string query = "select albumId from `user_like_album` where userId = @id";
            return this._connection.Query<int>(query, new { id }, transaction).ToList();
        }

        public List<int> GetListLikeArtistId(int id, IDbTransaction transaction = null)
        {
            string query = "select artistId from `user_like_artist` where userId = @id";
            return this._connection.Query<int>(query, new { id }, transaction).ToList();
        }

        public List<int> GetListLikePlaylistId(int id, IDbTransaction transaction = null)
        {
            string query = "select playlistId from `user_like_playlist` where userId = @id";
            return this._connection.Query<int>(query, new { id }, transaction).ToList();
        }

        public List<object> GetListHistorySongId(int id, IDbTransaction transaction = null)
        {
            string query = "select songId,createdAt from `user_history_song` where userId = @id";
            return this._connection.Query<object>(query, new { id }, transaction).ToList();
        }
        public List<object> GetListHistoryAlbumId(int id, IDbTransaction transaction = null)
        {
            string query = "select albumId,createdAt from `user_history_album` where userId = @id";
            return this._connection.Query<object>(query, new { id }, transaction).ToList();
        }
        public List<object> GetListHistoryArtistId(int id, IDbTransaction transaction = null)
        {
            string query = "select artistId,createdAt from `user_history_artist` where userId = @id";
            return this._connection.Query<object>(query, new { id }, transaction).ToList();
        }
        public List<object> GetListHistoryPlaylistId(int id, IDbTransaction transaction = null)
        {
            string query = "select playlistId,createdAt from `user_history_playlist` where userId = @id";
            return this._connection.Query<object>(query, new { id }, transaction).ToList();
        }

        public List<int> GetListCustomizePlaylistId(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `playlist` where userId = @id";
            return this._connection.Query<int>(query, new { id }, transaction).ToList();
        }

        public bool InsertHistorySong(HistorySong model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_history_song`(`userHistorySongId`, `userId`, `songId`, `createdAt`) VALUES (null,@userId,@songId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool InsertLikeSong(LikeSong model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_like_song`(`userLikeSongId`, `userId`, `songId`, `createdAt`) VALUES (null,@userId,@songId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool InsertHistoryAlbum(HistoryAlbum model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_history_album`(`userHistoryAlbumId`, `userId`, `albumId`, `createdAt`) VALUES (null,@userId,@albumId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }
        public bool InsertLikeAlbum(LikeAlbum model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_like_album`(`userLikeAlbumId`, `userId`, `albumId`, `createdAt`) VALUES (null,@userId,@albumId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }
        public bool InsertHistoryArtist(HistoryArtist model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_history_artist`(`userHistoryArtistId`, `userId`, `artistId`, `createdAt`) VALUES (null,@userId,@artistId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool InsertLikeArtist(LikeArtist model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_like_artist`(`userLikeArtistId`, `userId`, `artistId`, `createdAt`) VALUES (null,@userId,@artistId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool InsertHistoryPlaylist(HistoryPlaylist model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_history_playlist`(`userHistoryPlaylistId`, `userId`, `playlistId`, `createdAt`) VALUES (null,@userId,@playlistId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool InsertLikePlaylist(LikePlaylist model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `user_like_playlist`(`userLikePlaylistId`, `userId`, `playlistId`, `createdAt`) VALUES (null,@userId,@playlistId,@createdAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }
    }
}
