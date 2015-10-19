﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoEat.Web.Startup))]
namespace GoEat.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
