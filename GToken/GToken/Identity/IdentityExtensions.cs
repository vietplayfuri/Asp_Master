using System;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace GToken.Web.Identity
{
    public static class IdentityExtensions
    {
        public static T GetUserId<T>(this IIdentity identity)
        {
            string id = identity.GetUserId();
            return (T)Convert.ChangeType(id, typeof(T));
        }
    }
}