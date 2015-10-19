using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GoEat.Utility;

namespace GoEat.Models
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

        [Description("Image size is too small")]
        IMAGE_SIZE_TOO_SMALL,

        [Description("File path is not found")]
        FILE_PATH_NOT_FOUND,

        [Description("Stream is null")]
        STREAM_NULL,
        [Description("The amount is not sufficient to proceed transaction")]
        INSUFFICIENT_AMOUNT
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
