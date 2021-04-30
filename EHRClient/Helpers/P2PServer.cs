using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace EHR.Client.Helpers
{
    class P2PServer
    {
        //config for peer to peer local web server to listen for peers
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpListener listener =
                (HttpListener)appBuilder.Properties["System.Net.HttpListener"];

            //listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            //HttpSelfHostConfiguration config = new HttpSelfHostConfiguration();
            HttpConfiguration config = new HttpConfiguration();
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
            //appBuilder.UseWebApi()
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
            //app.UseWebApi(config);
        }
    }
}
