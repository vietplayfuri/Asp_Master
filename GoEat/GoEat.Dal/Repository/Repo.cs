using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GoEat.Dal
{
    /// <summary>
    /// GoEatRepop is a singleton. 
    /// SqlConnection is using ADO.net Connection Pool.
    /// </summary>
    public partial class GoEatRepo
    {
        private static readonly string ConnectionString;

        #region Singleton
        private GoEatRepo() { }

        public static readonly GoEatRepo Instance;
      
        static GoEatRepo()
        {
            Instance = new GoEatRepo();
            ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection2"].ConnectionString;
        }
        #endregion


        public IDbConnection OpenConnectionFromPool()
        {
            var connection = new SqlConnection(ConnectionString);
            return connection;
        }

    } 
}
