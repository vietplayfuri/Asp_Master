using Autofac;
using Microsoft.AspNet.Identity;
using System;

namespace GToken.Web.Identity
{
    public class IdentityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser, int>>();
        }
    }
}