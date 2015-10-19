using Autofac;
using GoPlay.Web.Identity;
using Microsoft.AspNet.Identity;
using System;

namespace GoPlay.Web.Identity
{
    public class IdentityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApplicationUserStore>().As<IUserStore<ApplicationUser, int>>();
        }
    }
}