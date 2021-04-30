using EHR.Client.Helpers;
using EHR.Data.Models;
using EHRClient;
using Microsoft.Extensions.DependencyInjection;
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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, IActivable
    {
        private string token;//auth token
        private Patient patient;//current patient
        private List<Patient> patients;//all patients
        private readonly AppSettings settings;
        private readonly SimpleNavigationService navigationService;
        private string username;

        //set up dependency injection for settings and nav service
        public Dashboard(SimpleNavigationService navigationService, IOptions<AppSettings> settings)
        {
            InitializeComponent();
            this.navigationService = navigationService;
            this.settings = settings.Value;
        }

        //runs on window load
        public Task ActivateAsync(string token, Patient patient, string username)
        {
            this.token = token;
            setPatients();
            this.PatientsList.ItemsSource = patients;
            this.username = username;
            return Task.CompletedTask;
        }

        //set patient data
        private void setPatients()
        {
            GetPatientInfo().Wait();
        }

        //get all the patients from server
        public async Task GetPatientInfo()
        {
            // Initialization.  
            List<Patient> patientsList = new List<Patient>();

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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Setting timeout.  
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP POST  
                    response = await client.GetAsync("api/Patients").ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        patientsList = JsonConvert.DeserializeObject<List<Patient>>(result);

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

            this.patients = patientsList;
        }

        //logout
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            navigationService.ShowAsync<MainWindow>().Wait();
            this.Close();
        }

        //load add med and handle the return
        private void AddMed_Click(object sender, RoutedEventArgs e)
        {
            bool? dialog = navigationService.ShowDialogAsync<MedicationAdd>(this.token, this.patient, this.username).Result;
            if (dialog.HasValue)
            {
                GetPatientMedInfo().Wait();
                this.Tests.ItemsSource = this.patient.Tests;
            }
        }

        //load add test and handle the return
        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            bool? dialog = navigationService.ShowDialogAsync<TestAdd>(this.token, this.patient, this.username).Result;
            if (dialog.HasValue)
            {
                GetPatientTestInfo().Wait();
                this.Tests.ItemsSource = this.patient.Tests;
            }
        }

        //load add note and handle the return
        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            bool? dialog = navigationService.ShowDialogAsync<NoteAdd>(this.token, this.patient, this.username).Result;
            if (dialog.HasValue)
            {
                GetPatientNoteInfo().Wait();
                this.Notes.ItemsSource = this.patient.Notes;
            }
        }

        //open med window
        private void Medications_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Medication med = new Medication(this.token, this.patient, (EHR.Data.Models.Medication)this.Medications.SelectedValue);
            bool? dialog = med.ShowDialog();
        }

        //open test window
        private void Tests_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Test test = new Test(this.token, this.patient, (EHR.Data.Models.Test)this.Tests.SelectedValue);
            bool? dialog = test.ShowDialog();
        }

        //open note window
        private void Notes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Note note = new Note(this.token, this.patient, (EHR.Data.Models.Note)this.Notes.SelectedValue);
            bool? dialog = note.ShowDialog();
        }

        //change to another patient
        private void PatientsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            patient = (Patient)this.PatientsList.SelectedItem;
            this.PatientName.Content = patient.Name;
            this.PatientGender.Content = patient.Gender;
            this.PatientDOB.Content = patient.DOB;
            this.PatientWeight.Content = patient.Weight;
            this.PatientHeight.Content = patient.Height;

            this.Medications.ItemsSource = patient.Medications;
            this.Notes.ItemsSource = patient.Notes;
            this.Tests.ItemsSource = patient.Tests;
        }

        //get just the tests for this patient and reset them
        public async Task GetPatientTestInfo()
        {
            // Initialization.  
            List<EHR.Data.Models.Test> testList = new List<EHR.Data.Models.Test>();

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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Setting timeout.  
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP POST  
                    response = await client.GetAsync($"api/Tests/Patient/{patient.MRN}").ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        testList = JsonConvert.DeserializeObject<List<EHR.Data.Models.Test>>(result);

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

            this.patient.Tests = testList;
        }

        //get just the note for this patient and reset them
        public async Task GetPatientNoteInfo()
        {
            // Initialization.  
            List<EHR.Data.Models.Note> noteList = new List<EHR.Data.Models.Note>();

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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Setting timeout.  
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP POST  
                    response = await client.GetAsync($"api/Notes/Patient/{patient.MRN}").ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        noteList = JsonConvert.DeserializeObject<List<EHR.Data.Models.Note>>(result);

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

            this.patient.Notes = noteList;
        }

        //get just the meds for this patient and reset them
        public async Task GetPatientMedInfo()
        {
            // Initialization.  
            List<EHR.Data.Models.Medication> medList = new List<EHR.Data.Models.Medication>();

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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Setting timeout.  
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Initialization.  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // HTTP POST  
                    response = await client.GetAsync($"api/Medications/Patient/{patient.MRN}").ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        medList = JsonConvert.DeserializeObject<List<EHR.Data.Models.Medication>>(result);

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

            this.patient.Medications = medList;
        }

        //open chat window
        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            navigationService.ShowAsync<Chat>(token, patient, username).Wait();
            //Chat chat = new Chat();// (this.token, this.patient, (EHR.Data.Models.Note)this.Notes.SelectedValue);
        }
    }
}
