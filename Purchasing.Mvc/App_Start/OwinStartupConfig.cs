using Owin;

namespace Purchasing.Mvc
{
    public class OwinStartupConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}