using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Platform.Dal;
using Platform.Models;
using Platform.Utility;
using System.Transactions;
using System.Data;
using System.Linq;

namespace Platform.Core
{
    public partial class Api
    {
        public void CheckFriendTransaction(IDbConnection db, string myName, string myFriendname,
            Action<Friend> mySelf, Action<Friend> myfriend)
        {
            var repo = Repo.Instance;
            Friend myFriend = null;

            var me = repo.GetFriendByName(db, myName, myFriendname).Data;
            if (me != null)
            {
                myFriend = repo.GetFriendByName(db, myFriendname, myName).Data;
                mySelf(me);
                myfriend(myFriend);
            }
        }

        public void CheckFriend(string myName, string myFriendname,
            Action<Friend> mySelf, Action<Friend> myfriend)
        {
            var repo = Repo.Instance;
            Friend myFriend = null;
            using (var db = repo.OpenConnectionFromPool())
            {
                var me = repo.GetFriendByName(db, myName, myFriendname).Data;
                if (me != null)
                {
                    myFriend = repo.GetFriendByName(db, myFriendname, myName).Data;
                    mySelf(me);
                    myfriend(myFriend);
                }
            }
        }

        public Dictionary<string, object> GetFriendList(string myUsername, FriendStatus? status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                List<string> lstFriendname = repo.GetFriendNameList(db, myUsername, status.HasValue ? Helper.GetDescription(status.Value) : string.Empty).Data;

                if (lstFriendname != null && lstFriendname.Any())
                {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    foreach (var username in lstFriendname)
                    {
                        var user = repo.GetCustomerByUserName(db, username).Data;
                        result.Add(user.username, new
                        {
                            status = Helper.GetDescription(status.Value),
                            bio = user.bio,
                            account = user.username,
                            uid = user.id,
                            locale = !string.IsNullOrEmpty(user.locale) ? user.locale : "en",
                            gender = user.gender,
                            cover = user.GetValidCoverUrl(),
                            inviter = user.inviter_username,
                            vip = user.vip,
                            avatar = user.GetValidAvatarUrl(),
                            country_code = user.country_code,
                            country_name = user.country_name,
                            is_venvici_member = user.is_venvici_member,
                            nickname = user.nickname,
                            email = user.email
                        });
                    }
                    return result;
                }
                return null;
            }
        }
        //public List<FriendDto> GetFriendNameList(string myUsername, FriendStatus? status)
        //{
        //    return null;
        //}

        public List<CustomerAccount> FindFriends(out int totalcount, string username, string keyword, int offset = 0, int count = 10)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.FindFriends(db, out totalcount, username, keyword, offset, count);
            }
        }

        public Result<List<string>> GetListFriendName(string myUsername, FriendStatus? status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetFriendNameList(db, myUsername, status.HasValue ? Platform.Utility.Helper.GetDescription(status.Value) : string.Empty);
            }
        }
    }
}
