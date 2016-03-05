using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VaWorks.Web.Startup))]
namespace VaWorks.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
