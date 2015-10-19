using Npgsql;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;

namespace Platform.Dal
{
    public partial class Repo
    {

        private static readonly string ConnectionString;

        #region Singleton
        private Repo() { }

        public static readonly Repo Instance;

        static Repo()
        {
            Instance = new Repo();
            ConnectionString = ConfigurationManager.ConnectionStrings["GTokenConnection"].ConnectionString;
        }
        #endregion

        public IDbConnection OpenConnectionFromPool()
        {
            //var connection = new SqlConnection(ConnectionString);
            NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public NpgsqlConnection OpenTransactionFromPool()
        {
            NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    } 
}
