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
            artistViewModel.ListArtist = new List<Artist>();
            artistViewModel.TotalPage = 0;
            int totalRows = this._connection.Query<int>(queryCount + query, new { keyword = keyword }).FirstOrDefault();
            if (totalRows > 0)
            {
                artistViewModel.TotalPage = (int)Math.Ceiling((decimal)totalRows / rowperpage);
            }
            if (page == 0)
            {
                page = 1;
            }
            int pageOffSet = (page - 1) * rowperpage;
            if (rowperpage < 0)
            {
                query += " order by createdAt desc";
                artistViewModel.TotalPage = 1;
            }
            else
            {
                query += " order by createdAt desc limit " + pageOffSet + "," + rowperpage;
            }
            artistViewModel.ListArtist = this._connection.Query<Artist>(querySelect + query, new { keyword = keyword }).ToList();

            return artistViewModel;
        }

        public Artist GetArtistDetail(int id, IDbTransaction transaction = null)
        {
            string query = "select * from `artist` where artistId = @id";
            return this._connection.Query<Artist>(query, new { id },transaction).FirstOrDefault();
        }

        public bool InsertArtist(Artist model, IDbTransaction transaction = null)
        {
            string insert = "INSERT INTO `artist`(`artistId`, `name`, `description`, `coverImageUrl`, `createdAt`, `updatedAt`, `nameSearch`)" +
                " VALUES ('[value-1]','[value-2]','[value-3]','[value-4]','[value-5]','[value-6]','[value-7]')";
            int status = this._connection.Execute(insert, model, transaction);
            return status > 0;
        }
    }
}
