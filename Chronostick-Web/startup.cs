using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Owin;
using Microsoft.Owin.Extensions;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: Microsoft.Owin.OwinStartup(typeof(Chronostick_Web.Startup))]
namespace Chronostick_Web
{
    public class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration
            {
#if DEBUG
                EnableDetailedErrors = true
#else
            EnableDetailedErrors = false
#endif
            };

            //var idProvider = new CustomUserIdProvider();

            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);
            //app.MapSignalR();
            app.MapSignalR();
            //Microsoft.AspNet.SignalR.StockTicker.Startup.ConfigureSignalR(app);
        }
    }
}
