using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GToken.Web.Startup))]
namespace GToken.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
