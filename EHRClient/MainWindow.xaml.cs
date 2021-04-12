using EHR.Client;
using EHR.Client.Helpers;
using EHR.Data.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EHRClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IActivable
    {
        private readonly AppSettings settings;
        private readonly SimpleNavigationService navigationService;

        public MainWindow(SimpleNavigationService navigationService, IOptions<AppSettings> settings)
        {
            InitializeComponent();
            this.settings = settings.Value;
            this.navigationService = navigationService;
        }

        public Task ActivateAsync(string token, Patient patient)
        {
            return Task.CompletedTask;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string token = PostRegInfo(this.Username.Text, this.Password.Password, this.Email.Text).Result;
            if (token != string.Empty)
            {
                //Dashboard dash = new Dashboard(token);
                //dash.Show();
                navigationService.ShowAsync<Dashboard>(token).Wait();
                this.Close();
            }
            else
            {

            }
        }
        public async Task<string> PostRegInfo(string username, string password, string email)
        {
            //Initialization
            string token = string.Empty;

            try
            {
                // Posting.  
                using (var client = new HttpClient())
                {
                    // Setting Base address.  
                    client.BaseAddress = new Uri(settings.ApiUrl);

                    // Setting content type.  
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Setting timeout.  
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP POST  
                    response = await client.PostAsJsonAsync("api/Token", new { Username = username, Password = password, ConfirmPassword = password, Email = email }).ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        dynamic tok = JsonConvert.DeserializeObject<object>(result);
                        token = tok["token"];

                        // Releasing.  
                        response.Dispose();
                    }
                    else
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        //responseObj.code = 602;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return token;
        }
        public static async Task<object> PostRegInfo(object requestObj)
        {
            // Initialization.  
            object responseObj = new object();

            try
            {
                // Posting.  
                using (var client = new HttpClient())
                {
                    // Setting Base address.  
                    client.BaseAddress = new Uri("http://localhost:19006/");

                    // Setting content type.  
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Setting timeout.  
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP POST  
                    response = await client.PostAsJsonAsync("api/WebApi/PostRegInfo", requestObj).ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        responseObj = JsonConvert.DeserializeObject<object>(result);

                        // Releasing.  
                        response.Dispose();
                    }
                    else
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        //responseObj.code = 602;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return responseObj;
        }
    }
}
