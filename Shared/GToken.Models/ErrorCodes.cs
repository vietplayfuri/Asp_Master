using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform.Utility;

namespace Platform.Models
{
    public enum ErrorCodes
    {
        [Description("NOT_FOUND")]
        NotFound,

        [Description("INVALID_GAME_ID")]
        InvalidGameId,
        [Description("MISSING_FIELDS")]
        MissingFields,
        [Description("EXISTING_EMAIL")]
        EmailExist,
        [Description("EXISTING_USERNAME_EMAIL")]
        UserNameOrEmailExist,
        [Description("USERNAME_LENGTH")]
        UserNameLengthMustBe3To20,
        [Description("INVALID_USERNAME")]
        InvalidUserNameCharacters,

        [Description("INVALID_USERID")]
        InvalidUserId,

        [Description("INVALID_USERNAME")]
        InvalidUserName,

        [Description("INVALID_USERNAME_LENGTH")]
        InvalidUsernameLength,

        [Description("INVALID_PASSWORD_LENGTH")]
        InvalidPasswordLength,


        [Description("PASSWORD_LENGTH")]
        PasswordLengthMustAtLeast3,

        [Description("INVALID_USN_PWD")]
        InvalidUserNameOrPassword,

        [Description("NON_EXISTING_REFERRAL_CODE")]
        InvalidReferrerId,

        [Description("NON_EXISTING_OAUTH")]
        OauthUserNotExist,

        [Description("INVALID_SESSION")]
        InvalidSession,
        [Description("INVALID_TRANSACTION_ID")]
        InvalidTransactionId,
        [Description("INVALID_ORDER_ID")]
        InvalidOrderId,
        [Description("TRANSACTION_ALREADY_PROCESSED")]
        TransactionAlreadyProcessed,
        [Description("NOT_SUPPORTED_OAUTH_PROVIDER")]
        UnSupportedOauthProvider,
        [Description("EXISTING_OAUTH")]
        OauthUserAlreadyExist,
        [Description("OAUTH_ALREADY_CONNECTED")]
        OauthAlreadyConnected,
        [Description("OAUTH_USER_NOT_CONNECTED")]
        OauthAndUserNotConnected,


        // Additional Error Codes //
        [Description("USER_ALREADY_LOGGED_IN")]
        UserAlreadyLoggedIn = 1000,
        [Description("USER_NOT_LOGGED_IN")]
        UserNotLoggedIn,


        [Description("HTTP_REQUEST_ERROR")]
        HttpRequestError = 2000,

        // Restaurant Specific //
        [Description("INVALID_BARCODE")]
        InvalidBarCode = 3000,

        [Description("NON_EXISTING_USER")]
        NonExistingUser,

        [Description("EXISTING_ORDERID")]
        ExistingOrderId,

        [Description("TRANSACTION_NOTFOUND")]
        TransactionNotFound,

        [Description("MISMATCH_TRANSACTION_AMOUNT")]
        MismathcTransactionAmount,

        [Description("INVALID_TRANSACTION_STATUS")]
        InvalidTransactionStatus,

        [Description("SUBTOTAL_IS_WRONG")]
        SubtotalIsWrong,

        [Description("SERVER_ERROR")]
        ServerError,
        [Description("NOT_ENOUGHT_TOKEN")]
        NotEnoughtToken,

        [Description("Invalid country code or country name")]
        INVALID_COUNTRY,

        #region ErrorMessage of GToken Python
        [Description("Referral Code does not exist")]
        NON_EXISTING_REFERRAL_CODE,

        [Description("OAuth Account does not exist")]
        NON_EXISTING_OAUTH,

        [Description("User Account does not exist")]
        NON_EXISTING_USER,

        [Description("Filename does not exist")]
        NON_EXISTING_FILENAME,

        [Description("Friend request does not exist")]
        NON_EXISTING_FRIEND_REQUEST,

        [Description("OAuth Account is connected already")]
        OAUTH_ALREADY_CONNECTED,

        [Description("OAuth Account and Customer Account are not connected")]
        OAUTH_USER_NOT_CONNECTED,

        [Description("OAuth Account already exists")]
        EXISTING_OAUTH,

        [Description("Account with such username/email already exists")]
        EXISTING_USERNAME_EMAIL,

        [Description("Account with such email already exists")]
        EXISTING_EMAIL,

        [Description("Required field(s) is blank")]
        MISSING_FIELDS,

        [Description("Transaction has already been processed")]
        TRANSACTION_ALREADY_PROCESSED,

        [Description("Invalid Partner ID")]
        INVALID_PARTNER_ID,

        [Description("Invalid Transaction ID")]
        INVALID_TRANSACTION_ID,

        [Description("Invalid Transaction Type")]
        INVALID_TRANSACTION_TYPE,

        [Description("Invalid stat format. A stat JSON must include 3 keys title, value and public with their values")]
        INVALID_PARTNER_STAT,

        [Description("Username or Password is incorrect")]
        INVALID_USN_PWD,

        [Description("Invalid Session")]
        INVALID_SESSION,

        [Description("Invalid token")]
        INVALID_TOKEN,

        [Description("Invalid VIP status")]
        INVALID_VIP_STATUS,

        [Description("Invalid Exchange Option Identifier")]
        INVALID_EXCHANGE_OPTION,

        [Description("Username does not accept special characters")]
        INVALID_USERNAME,

        [Description("Friend request status must be either 'accepted' or 'rejected'")]
        INVALID_FRIEND_REQUEST_STATUS,

        [Description("Invalid email address")]
        INVALID_EMAIL,

        [Description("The amount is not sufficient to proceed transaction")]
        INSUFFICIENT_AMOUNT,

        [Description("The OAuth Provider is not supported")]
        NOT_SUPPORTED_OAUTH_PROVIDER,

        [Description("Exchange validation failed")]
        EXCHANGE_VALIDATION_FAILED,

        [Description("Exchange execution failed")]
        EXCHANGE_EXECUTION_FAILED,

        [Description("Exchange has already been recorded")]
        EXCHANGE_RECORDED,

        [Description("Username is between 3-20 characters")]
        USERNAME_LENGTH,

        [Description("Password must be more than 3 characters")]
        PASSWORD_LENGTH,

        [Description("Password and Confirm Pass are not identical")]
        UNIDENTICAL_PASSWORDS,

        [Description("Exchange Option does not belong to partner")]
        NO_EXCHANGE_PARTNER,

        [Description("Partner is not active or has been removed")]
        PARTNER_REMOVED,

        [Description("Exchange Option has been removed")]
        EXCHANGE_REMOVED,

        [Description("Insufficient Balance")]
        INSUFFICIENT_BALANCE,

        [Description("Exchange amount needs to be a positive integer")]
        NEGATIVE_TRANSACTION,

        [Description("Exchange amount needs to be 01")]
        PACKAGE_QUANTITY,

        [Description("Referrer cannot be referred back")]
        TWO_WAY_REFERRING,

        [Description("User does not have this permission")]
        PERMISSION_DENIED,

        [Description("Order ID already exists")]
        EXISTING_ORDERID,

        [Description("Invalid transaction status")]
        INVALID_TRANSACTION_STATUS,

        [Description("Invalid transaction info (JSON)")]
        INVALID_JSON_TRANSACTION,

        [Description("Transaction ID is not found")]
        TRANSACTION_NOT_FOUND,

        [Description("Price, final amount and discount do not match")]
        MISMATCH_TRANSACTION_AMOUNT,

        [Description("Currency code not found (ISO 4217)")]
        INVALID_CURRENCY_CODE,

        [Description("Friend request has already been sent")]
        REQUEST_ALREADY_SENT,

        [Description("Invalid hashed token")]
        INVALID_HASHED_TOKEN,

        [Description("Access failure")]
        FACEBOOK_ACCESS_ERROR,
        #endregion

        [Description("Image size is too small")]
        IMAGE_SIZE_TOO_SMALL,

        [Description("File path is not found")]
        FILE_PATH_NOT_FOUND,

        [Description("Stream is null")]
        STREAM_NULL,

        #region ERROR MESSAGE OF GOPLAY
        [Description("Invalid Game ID")]
        INVALID_GAME_ID,

        [Description("Invalid stat format. A stat JSON must include 3 keys title, value and public with their values")]
        INVALID_GAME_STAT,

        [Description("Exchange Option does not belong to game")]
        NO_EXCHANGE_GAME,

        [Description("Game is not active or has been removed")]
        GAME_REMOVED,

        [Description("Invalid package Id")]
        INVALID_PACKAGE_ID,

        [Description("Receiver account does not exist.")]
        RECEIVER_ACCOUNT_NOT_EXIST,

        [Description("Invalid Play Token amount.")]
        INVALID_PLAY_TOKEN_AMOUNT,

        [Description("Your Play Token balance is insufficient for the transaction.")]
        INSUFFICIENT_PLAY_TOKEN_AMOUNT,

        [Description("You can only transfer up to 2 decimal points.")]
        INVALID_DECIMAL_PRECISION,

        [Description("Transferring to yourself won\'t work, you know?")]
        SENDING_TO_SELF,

        [Description("Invalid voucher type")]
        INVALID_VOUCHER_TYPE,

        [Description("Invalid credit type Id")]
        INVALID_CREDIT_TYPE_ID,

        [Description("Create referral campaign history error")]
        CREATE_REFERRAL_CAMPAIGN_HISTORY_ERROR,

        [Description("Update cash back venvici error")]
        UPDATE_CASH_BACK_ERROR,

        [Description("Invalid Role name")]
        INVALID_ROLE,

        [Description("Invalid exchange rate")]
        INVALID_EXCHANGE_RATE,

        [Description("Invalid free exchange rate")]
        INVALID_FREE_EXCHANGE_RATE,

        [Description("Invalid attached file")]
        INVALID_ATTACHED_FILE,
        #endregion

        #region ERROR MESSAGE OF PROXY
        [Description("API does not exist")]
        NON_EXISTING_API,

        TSTAMP_ERROR,

        [Description("System error")]
        SYSTEM_ERROR,

        [Description("OAuth Disconnect - Invalid session")]
        OAUTH_DISCONNECT_INVALID_SESSION,
        #endregion

        [Description("Invalid referral campaign")]
        INVALID_REFERRAL_CAMPAIGN,

        [Description("Quantity is over")]
        REFERRAL_CAMPAIGN_QUANTITY_IS_OVER,

        [Description("Save file error")]
        SAVE_FILE_ERROR,
    }


    #region Enum extension
    public static class EnumEx
    {
        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
            //throw new ArgumentException("Not found.", "description");
            // or return default(T);
        }
    }

    public static class ErrorCodesExtension
    {

        public static string ToErrorCode(this ErrorCodes? error)
        {
            return error.HasValue ? Helper.GetEnumName(typeof(ErrorCodes), error) : string.Empty;

        }

        public static string ToErrorCode(this ErrorCodes error)
        {
            return Helper.GetEnumName(typeof(ErrorCodes), error);
        }

        public static string ToErrorMessage(this ErrorCodes? error)
        {
            return error.HasValue ? Helper.GetDescription(error) : string.Empty;
        }

        public static string ToErrorMessage(this ErrorCodes error)
        {
            return Helper.GetDescription(error);
        }
    }

    #endregion
}
