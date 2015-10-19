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
using GoPlay.Models;


namespace GoPlay.Dal
{
    public partial class Repo
    {
        public Result<List<paypal_preapproval>> FindPaypalPreapproval(bool is_active)
        {
            using (var db = OpenConnectionFromPool())
            {
                var paypal = db.Query<GoPlay.Models.paypal_preapproval>("SELECT * FROM paypal_preapproval WHERE is_active=@is_active", new { is_active }).ToList();
                return Result<List<GoPlay.Models.paypal_preapproval>>.Make(paypal);
            }
        }

        public Result<List<paypal_preapproval>> FindPaypalPreapproval(bool is_active, string flag)
        {
            using (var db = OpenConnectionFromPool())
            {
                var paypal = db.Query<paypal_preapproval>("SELECT * FROM paypal_preapproval WHERE is_active=@is_active AND flag=@flag", new { is_active, flag }).ToList();
                return Result<List<paypal_preapproval>>.Make(paypal);
            }
        }
        public bool UpdatePaypalPreapproval(int id, bool is_active)
        {
            using (var db = OpenConnectionFromPool())
            {
                string sql = @"UPDATE paypal_preapproval SET is_active=@is_active WHERE id=@id";
                return 1 == db.Execute(sql, new
                {
                    id,
                    is_active
                });
            }
        }

        public bool CreatePaypalPreapproval(paypal_preapproval paypal_preapproval)
        {
            using (var db = OpenConnectionFromPool())
            {
                string sql = @"INSERT INTO paypal_preapproval(
                                created_at, 
                                starting_date,
                                ending_date,
                                max_amount_per_payment,
                                max_number_of_payments,
                                max_total_amount_of_all_payments,
                                sender_email,
                                preapproval_key,
                                current_number_of_payments,
                                current_total_amount_of_all_payments,
                                is_active,
                                flag
                                )
                            VALUES( 
                                @created_at,
                                @starting_date,
                                @ending_date,
                                @max_amount_per_payment,
                                @max_number_of_payments,
                                @max_total_amount_of_all_payments,
                                @sender_email,
                                @preapproval_key,
                                @current_number_of_payments,
                                @current_total_amount_of_all_payments,
                                @is_active,
                                @flag)";
                return 1 == db.Execute(sql, paypal_preapproval);
            }
        }
    }
}
