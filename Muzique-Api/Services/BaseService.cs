using MySqlConnector;
using System;
using System.Data;
using Dapper;
namespace Muzique_Api.Services
{
    public class BaseService
    {
        private const string _connectionString = "Server=127.0.0.1; User ID=root; Password=; Database=muzique;Convert Zero Datetime=True; Allow User Variables=true";
        protected IDbConnection _connection;
        public BaseService() {
            this._connection = new MySqlConnection(_connectionString);
        }

        public BaseService(IDbConnection _connection)
        {
            if (_connection == null)
            {
                this._connection = new MySqlConnection(_connectionString);
            }
            else
                this._connection = _connection;
        }

        public static IDbConnection Connect()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
