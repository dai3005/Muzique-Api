using Dapper;
using Muzique_Api.Models;
using System.Data;

namespace Muzique_Api.Services
{
    public class ArtistService : BaseService
    {
        public ArtistService() : base() { }

        public ArtistService(IDbConnection db) : base(db) { }

        public ArtistViewModel GetListArtist(int page, int rowperpage, string keyword)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from `artist` where 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and nameSearch like @keyword ";
            }
            ArtistViewModel artistViewModel = new ArtistViewModel();
            artistViewModel.ListData = new List<Artist>();
            artistViewModel.TotalRes = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                artistViewModel.TotalRes = totalRows;
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
            artistViewModel.ListData = this._connection.Query<Artist>(querySelect + query, new { keyword = keyword }).ToList();

            return artistViewModel;
        }

        public Artist GetArtistDetail(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `artist` where artistId = @id";
            return this._connection.Query<Artist>(query, new { id },transaction).FirstOrDefault();
        }

        public bool InsertArtist(Artist model)
        {
            string insert = "INSERT INTO `artist`(`artistId`, `name`, `description`, `coverImageUrl`, `createdAt`, `nameSearch`)" +
                " VALUES (null,@name,@description,@coverImageUrl,@createdAt,@nameSearch)";
            int status = this._connection.Execute(insert, model);
            return status > 0;
        }

        public bool UpdateArtist(Artist model)
        {
            string query = "UPDATE `artist` SET `name`=@name,`description`=@description,`coverImageUrl`=@coverImageUrl" +
                ",`updatedAt`=@updatedAt,`nameSearch`=@nameSearch WHERE artistId = @artistId";
            int status = this._connection.Execute(query, model);
            return status > 0;
        }

        public bool DeleteArtist(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `artist` WHERE artistId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public bool DeleteArtistSong(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song_and_artist` WHERE artistId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }
    }
}
