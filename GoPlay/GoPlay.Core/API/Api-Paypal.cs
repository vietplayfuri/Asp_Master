using GoPlay.Dal;
using GoPlay.Models;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public Result<List<paypal_preapproval>> FindPaypalPreapproval(bool is_active)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.FindPaypalPreapproval(is_active);
            }
        }

        public Result<List<paypal_preapproval>> FindPaypalPreapproval(bool is_active, string flag)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.FindPaypalPreapproval(is_active, flag);
            }
        }

        public bool UpdatePaypalPreapproval(int id, bool is_active)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdatePaypalPreapproval(id, is_active);
            }

        }

        public bool CreatePaypalPreapproval(paypal_preapproval paypal_preapproval)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreatePaypalPreapproval(paypal_preapproval);
            }

        }
    }
}