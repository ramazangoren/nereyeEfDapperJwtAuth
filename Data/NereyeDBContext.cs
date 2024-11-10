using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class NereyeDBContext
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public NereyeDBContext(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.Query<T>(sql);
        }

        public IEnumerable<T> LoadDataFilterAndSearch<T>(string sql, object parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.Query<T>(sql, parameters);
        }

        public T LoadDataSingle<T>(string sql, object parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.QuerySingle<T>(sql, parameters);
        }

        public T LoadDataSingleAuth<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql, object parameters)
        {
            using IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.Execute(sql, parameters) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql, object parameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionString);
            return dbConnection.QuerySingle<int>(sql, parameters);
        }

        // public int ExecuteSqlWithRowCount(string sql, object parameters)
        // {
        //     using IDbConnection dbConnection = new SqlConnection(_connectionString);
        //     return dbConnection.QuerySingle<int>(sql, parameters);
        // }

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            SqlCommand CommandWithParams = new SqlCommand(sql);
            foreach (SqlParameter parameter in parameters)
            {
                CommandWithParams.Parameters.Add(parameter);
            }
            using SqlConnection dbConnection = new SqlConnection(_connectionString);
            dbConnection.Open();
            CommandWithParams.Connection = dbConnection;
            int rowsAffected = CommandWithParams.ExecuteNonQuery();
            dbConnection.Close();
            return rowsAffected > 0;
        }
    }
}
