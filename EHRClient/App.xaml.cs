using EHR.Client;
using EHR.Client.Controllers;
using EHR.Client.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Host.HttpListener;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using System.Windows;

namespace EHRClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }
        //startup configuration
        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            //var appBuilder = new AppBuilder();
            //Configure(appBuilder);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var navigationService = ServiceProvider
                            .GetRequiredService<SimpleNavigationService>();
            var task = navigationService.ShowAsync<MainWindow>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Add SimpleNavigationService for the application.
            services.AddScoped<SimpleNavigationService>();

            //add config service
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //Transient for windows
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(Dashboard));
            services.AddTransient(typeof(TestAdd));
            services.AddTransient(typeof(MedicationAdd));
            services.AddTransient(typeof(NoteAdd));
            services.AddTransient(typeof(Test));
            services.AddTransient(typeof(Medication));
            services.AddTransient(typeof(Note));
            services.AddTransient(typeof(Chat));

            
            //services.AddControllers().AddApplicationPart(typeof(ChatController).Assembly);
        }

        public void Configure(IAppBuilder appBuilder)
        {
            
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
            //HttpListener listener =
            //    (HttpListener)appBuilder.Properties["System.Net.HttpListener"];
            //listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        }
    }
}
