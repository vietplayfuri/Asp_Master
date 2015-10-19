using GoPlay.Dal;
using GoPlay.Models;
using Platform.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public void CheckFriendTransaction(int myId, int myFriendId,
            Action<friend> mySelf, Action<friend> myfriend)
        {
            var repo = Repo.Instance;
            friend myFriend = null;
            using (var db = repo.OpenConnectionFromPool())
            {
                var me = repo.GetFriendById(db, myId, myFriendId).Data;
                if (me != null)
                {
                    myFriend = repo.GetFriendById(db, myFriendId, myId).Data;
                    mySelf(me);
                    myfriend(myFriend);
                }
            }
        }

        public void CheckFriend(int myId, int myFriendId,
            Action<friend> mySelf, Action<friend> myfriend)
        {
            var repo = Repo.Instance;
            friend myFriend = null;
            using (var db = repo.OpenConnectionFromPool())
            {
                var me = repo.GetFriendById(db, myId, myFriendId).Data;
                if (me != null)
                {
                    myFriend = repo.GetFriendById(db, myFriendId, myId).Data;
                }
                mySelf(me);
                myfriend(myFriend);
            }
        }

        /// <summary>
        /// Get friend list of user base of status
        /// </summary>
        /// <param name="myUsername">friend of this user</param>
        /// <param name="status">status of request</param>
        /// <returns>list of friends</returns>
        public List<customer_account> GetFriendList(int myId, FriendStatus? status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetFriendList(db, myId, status.HasValue
                    ? Helper.GetDescription(status.Value)
                    : string.Empty).Data;
            }
        }

        public List<SimpleCustomerAccount> FindFriends(out int totalcount, int myId, string keyword, int offset = 0, int count = 10)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.FindFriends(db, out totalcount, myId, keyword, offset, count);
            }
        }

        public List<SimpleCustomerAccount> GetSimpleFriendList(int myId, FriendStatus? status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetSimpleFriendList(db, myId, status.HasValue
                    ? Helper.GetDescription(status.Value)
                    : string.Empty).Data;
            }
        }

        public Result<List<int>> GetListFriendName(int myId, FriendStatus? status)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetFriendNameList(db, myId, status.HasValue ? Helper.GetDescription(status.Value) : string.Empty);
            }
        }
    }
}
