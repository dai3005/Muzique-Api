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
            genreViewModel.TotalPage = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                genreViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRows / rowperpage);
            }
            if (page == 0)
            {
                page = 1;
            }
            int pageOffSet = (page - 1) * rowperpage;
            if (rowperpage < 0)
            {
                query += " order by createdAt desc";
                genreViewModel.TotalPage = 1;
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
    }
}
