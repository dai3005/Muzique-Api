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
    }
}
