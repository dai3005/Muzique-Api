using Dapper;
using Muzique_Api.Models;
using System.Data;

namespace Muzique_Api.Services
{
    public class SongService : BaseService
    {
        public SongService() : base() { }

        public SongService(IDbConnection db) : base(db) { }

        public SongViewModel GetListSong(int page,int rowperpage, string keyword)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from `song` where 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and nameSearch like @keyword ";
            }
            SongViewModel songViewModel = new SongViewModel();
            songViewModel.ListSong = new List<Song>();
            songViewModel.TotalPage = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                songViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRows / rowperpage);
            }
            if (page == 0)
            {
                page = 1;
            }
            int pageOffSet = (page - 1) * rowperpage;
            if(rowperpage < 0)
            {
                query += " order by createdAt desc";
                songViewModel.TotalPage = 1;
            } else
            {
                query += " order by createdAt desc limit " + pageOffSet + "," + rowperpage;
            }        
            songViewModel.ListSong = this._connection.Query<Song>(querySelect + query, new { keyword = keyword }).ToList();

            return songViewModel;
        }

        public Song GetSongById(string id, IDbTransaction transaction = null)
        {
            string query = "select * from `song` where songId = @id";
            return this._connection.Query<Song>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool DeleteSong(string id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song` WHERE songID = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public object GetSongDetail(string id)
        {
            string querySong = "select * from `song` where songId = @id";
            string queryArtist = "select artistId from `song_and_artist` where songId=@id";
            string queryGenre = "select genreId from `song_and_genre` where songId=@id";
            string queryPlaylist = "select playlistId from `song_and_playlist` where songId=@id";

            object Song = this._connection.Query<object>(querySong, new { id }).FirstOrDefault();
            List<int> listArtist = this._connection.Query<int>(queryArtist, new { id }).ToList();
            List<int> listGenre = this._connection.Query<int>(queryGenre, new { id }).ToList();
            List<int> listPlaylist = this._connection.Query<int>(queryPlaylist, new { id }).ToList();

            return new
            {
                Song = Song,
                listArtistId = listArtist,
                listGenreId = listGenre,
                listPlaylistId = listPlaylist
            };
        }

        public bool InsertSong(Song model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `song`(`songId`,`name`, `audioUrl`, `description`, `coverImageUrl`, `albumId`, `createdAt`, `nameSearch`)" +
                " VALUES (@songId,@name,@audioUrl,@description,@coverImageUrl,@albumId,@createdAt, @nameSearch)";
            int status = this._connection.Execute(insert, model,transaction);
            return status > 0;
        }

        public bool InsertSongArtist(SongAndArtist model, IDbTransaction transaction = null)
        {
            string id = model.songId;
            string insert = "INSERT INTO `song_and_artist`(`songAndArtistId`, `songId`, `artistId`, `createdAt`)" +
                " VALUES (null,@songId,@artistId,@createAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool InsertSongGenre(SongAndGenre model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `song_and_genre`(`songAndGenreId`, `songId`, `genreId`, `createdAt`)" +
                " VALUES (null,@songId,@genreId,@createAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool InsertSongPlaylist(SongAndPlaylist model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `song_and_playlist`(`songAndPlaylistId`, `songId`, `playlistId`, `createdAt`)" +
                " VALUES (null,@songId,@playlistId,@createAt)";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }

        public bool DeleteSongArtist(string id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song_and_artist` WHERE songID = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public bool DeleteSongGenre(string id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song_and_genre` WHERE songID = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public bool DeleteSongPlaylist(string id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song_and_playlist` WHERE songID = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }
    }
}
