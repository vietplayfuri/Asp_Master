using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Net;
using System.Data;
using System.Text.RegularExpressions;
using Platform.Models;
using Npgsql;


namespace Platform.Dal
{
    public partial class Repo
    {
        public Result<ExchangeRate> GetExchangeRate(IDbConnection db, string month, string year, string source_currency, string destination_currency)
        {
            string sql = @"SELECT * FROM exchange_rate 
                WHERE month = @month 
                  AND year = @year
                  AND source_currency = @source_currency
                  AND destination_currency = @destination_currency";

            var exchangeRate = db.Query<ExchangeRate>(sql, new { month, year, source_currency, destination_currency }).FirstOrDefault();

            return Result<ExchangeRate>.Make(exchangeRate, errorIfNull: ErrorCodes.EXCHANGE_REMOVED);
        }
        
        /// <summary>
        /// Create exchange rate -- get from yahoo and used in a month
        /// </summary>
        /// <param name="db"></param>
        /// <param name="exchangeRateData"></param>
        /// <returns>new id</returns>
        public int CreateExchangeRate(IDbConnection db, ExchangeRateData exchangeRateData)
        {
            string sql = @"INSERT INTO exchange_rate 
            (month, source_currency, destination_currency, exchange_rate, year) 
            VALUES 
            (@month, @source_currency, @destination_currency, @exchange_rate, @year)";

            return db.Query<int>(sql, exchangeRateData).FirstOrDefault();
        }
    }
}
