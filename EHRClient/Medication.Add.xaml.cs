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
using System.Windows.Shapes;

namespace EHR.Client
{
    /// <summary>
    /// Interaction logic for Medication.xaml
    /// </summary>
    public partial class MedicationAdd : Window, IActivable
    {
        private string token;//auth token
        private EHR.Data.Models.Medication medication;//new med
        private Patient patient;//current patient
        private readonly AppSettings settings;
        private readonly SimpleNavigationService navigationService;
        public MedicationAdd(SimpleNavigationService navigationService, IOptions<AppSettings> settings)
        {
            InitializeComponent();
            this.navigationService = navigationService;
            this.settings = settings.Value;
        }

        //on load
        public Task ActivateAsync(string token, Patient patient, string username)
        {
            this.token = token;
            this.medication = new EHR.Data.Models.Medication();
            this.patient = patient;
            this.PatientName.Content = patient.Name;
            return Task.CompletedTask;
        }

        //build medication and post to server, then close
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.medication.Name = this.PatientMed.Text;
            this.medication.PatientId = this.patient.MRN;
            this.medication.Dosage = this.PatientDose.Text;
            this.medication.Frequency = this.PatientFreq.Text;
            var expAt = this.PatientExpAt.Text.Split('/');
            this.medication.Expires = new DateTime(int.Parse(expAt[2]), int.Parse(expAt[0]), int.Parse(expAt[1]));
            postMedication();
            this.Close();
        }


        private void postMedication()
        {
            PostMedicationInfo().Wait();
        }

        //post new medication to server
        public async Task PostMedicationInfo()
        {
            try
            {
                // client call
                using (var client = new HttpClient())
                {
                    // Base address 
                    client.BaseAddress = new Uri(settings.ApiUrl);

                    // content type 
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // timeout 
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Msg init  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // Body
                    string json = JsonConvert.SerializeObject(this.medication);
                    StringContent body = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                    // HTTP POST  
                    response = await client.PostAsync("api/Medications", body).ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading and ignoring response   
                        string result = response.Content.ReadAsStringAsync().Result;

                        // Releasing 
                        response.Dispose();
                    }
                    else
                    {
                        // Reading Response
                        string result = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
