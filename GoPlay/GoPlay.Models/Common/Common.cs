using Platform.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay.Models
{
    public enum EGtokenAction
    {
        #region Account
        Login = 0,
        Register = 1,
        Profile = 2,
        EditProfile = 3,
        QueryUserId = 4,
        ConnectOauth = 5,
        DisconnectOauth = 6,
        ChangePassword = 7,
        CheckOauthConnection = 8,
        LoginOauth = 9,
        GetNotifications = 10,
        #endregion

        #region Friend
        GetFriendList = 11,
        /// <summary>
        /// Search user - used in friend api
        /// </summary>
        SearchUsers = 12,
        SendRequest = 13,
        RespondRequest = 14,
        AddFriend = 15,
        #endregion

        #region Transaction
        RecordTransaction = 16
        #endregion
    }
    public enum EUpointAction
    {
        BalanceDeduction =0,
        Speedy=1,
        TMoney=2,
        TelkomselVoucher=3,
        StandardVoucher=4,
        GetTicketTelkomsel=5,
    }
    public enum Active_page
    {
        Profile = 0,
        Transaction = 1,
        Friend = 2
    }
    public enum EUserStatus
    {
        [Description("Classic Reseller")]
        ClassicReseller = 2,
        [Description("Gold Reseller")]
        GoldReseller = 3
    }

    public enum EVoucherType
    {
        [Description("telkom")]
        Telkom,
        [Description("spin")]
        Spin
    }
    public static class EUserStatusExtension
    {
        public static string ToDescription(this EUserStatus? enumStatus)
        {
            return enumStatus.HasValue ? Helper.GetDescription(enumStatus) : string.Empty;
        }
        public static string ToDescription(this EUserStatus enumStatus)
        {
            return Helper.GetDescription(enumStatus);
        }
    }

}
