using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GabDemoApp.Startup))]
namespace GabDemoApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
