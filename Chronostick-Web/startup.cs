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
            //var idProvider = new CustomUserIdProvider();

            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);
            //app.MapSignalR();
            app.MapSignalR();
        }
    }
}
