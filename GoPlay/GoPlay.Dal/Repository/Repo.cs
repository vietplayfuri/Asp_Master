using Npgsql;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GoPlay.Dal
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
            ConnectionString = ConfigurationManager.ConnectionStrings["GoplayConnection"].ConnectionString;
        }
        #endregion


        public IDbConnection OpenConnectionFromPool()
        {
            NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public IDbConnection OpenTransactionFromPool()
        {
            NpgsqlConnection connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return (IDbConnection)connection;
        }
    } 
}
