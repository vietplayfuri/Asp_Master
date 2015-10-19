using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoPlay.Web.Startup))]
namespace GoPlay.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
