﻿using System;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace GoPlay.Web.Identity
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