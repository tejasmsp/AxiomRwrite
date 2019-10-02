using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Axiom.Web.Startup))]
namespace Axiom.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
