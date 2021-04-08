using EHR.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EHR.Client.Helpers
{
    public interface IActivable
    {
        Task ActivateAsync(string token, Patient patient);
    }

    public class SimpleNavigationService
    {
        private readonly IServiceProvider serviceProvider;

        public SimpleNavigationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task ShowAsync<T>(string token = null, Patient patient = null) where T : Window
        {
            var window = serviceProvider.GetRequiredService<T>();
            if (window is IActivable activableWindow)
            {
                await activableWindow.ActivateAsync(token, patient);
            }

            window.Show();
        }

        public async Task<bool?> ShowDialogAsync<T>(string token = null, Patient patient = null)
            where T : Window
        {
            var window = serviceProvider.GetRequiredService<T>();
            if (window is IActivable activableWindow)
            {
                await activableWindow.ActivateAsync(token, patient);
            }

            return window.ShowDialog();
        }
    }
}
