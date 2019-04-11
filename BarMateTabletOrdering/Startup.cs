using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BarMateTabletOrdering.Startup))]
namespace BarMateTabletOrdering
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //app.MapSignalR();
        }
    }
}
