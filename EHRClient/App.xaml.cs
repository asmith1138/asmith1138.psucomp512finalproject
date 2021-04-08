using EHR.Client;
using EHR.Client.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

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
            //services.AddTransient(typeof(Dashboard));
        }
    }
}
