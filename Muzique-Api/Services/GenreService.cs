using Dapper;
using Muzique_Api.Models;
using System.Data;

namespace Muzique_Api.Services
{
    public class GenreService : BaseService
    {
        public GenreService() : base() { }

        public GenreService(IDbConnection db) : base(db) { }

        public GenreViewModel GetListGenre(int page, int rowperpage, string keyword)
        {
            string querySelect = "select * ";
            string queryCount = "select count(*) as Total ";
            string query = " from `genre` where 1=1 ";
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword.Replace(' ', '%') + "%";
                query += " and nameSearch like @keyword ";
            }
            GenreViewModel genreViewModel = new GenreViewModel();
            genreViewModel.ListArtist = new List<Genre>();
            genreViewModel.TotalRes = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                genreViewModel.TotalRes = totalRows;
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
            genreViewModel.ListArtist = this._connection.Query<Genre>(querySelect + query, new { keyword = keyword }).ToList();

            return genreViewModel;
        }

        public Genre GetGenreDetail(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `genre` where genreId = @id";
            return this._connection.Query<Genre>(query, new { id }, transaction).FirstOrDefault();
        }

        public bool InsertGenre(Genre model)
        {
            string insert = "INSERT INTO `genre`(`genreId`, `name`, `description`, `coverImageUrl`, `createdAt`, `nameSearch`)" +
                " VALUES (null,@name,@description,@coverImageUrl,@createdAt,@nameSearch)";
            int status = this._connection.Execute(insert, model);
            return status > 0;
        }

        public bool UpdateGenre(Genre model)
        {
            string query = "UPDATE `genre` SET `name`=@name,`description`=@description,`coverImageUrl`=@coverImageUrl" +
                ",`updatedAt`=@updatedAt,`nameSearch`=@nameSearch WHERE genreId = @genreId";
            int status = this._connection.Execute(query, model);
            return status > 0;
        }

        public bool DeleteGenre(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `genre` WHERE genreId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }

        public bool DeleteGenreSong(int id, IDbTransaction transaction = null)
        {
            string delete = "DELETE FROM `song_and_genre` WHERE genreId = @id";
            int status = this._connection.Execute(delete, new { id = id }, transaction);
            return status > 0;
        }
    }
}
