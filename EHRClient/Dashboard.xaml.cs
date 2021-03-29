using EHR.Data.Models;
using EHRClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
    public partial class Dashboard : Window
    {
        private string token;
        private Patient patient;
        private List<Patient> patients;
        public Dashboard(string token)
        {
            InitializeComponent();
            this.token = token;
            setPatients();
            this.PatientsList.ItemsSource = patients;
        }

        private void setPatients()
        {
            GetPatientInfo().Wait();
        }

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
                    client.BaseAddress = new Uri("https://localhost:44339/");

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow logon = new MainWindow();
            logon.Show();
            this.Close();
        }

        private void AddMed_Click(object sender, RoutedEventArgs e)
        {
            MedicationAdd medadd = new MedicationAdd(this.token, this.patient);
            bool? dialog = medadd.ShowDialog();
            if (dialog.HasValue)
            {
                GetPatientMedInfo().Wait();
                this.Tests.ItemsSource = this.patient.Tests;
            }
        }

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            TestAdd testadd = new TestAdd(this.token, this.patient);
            bool? dialog = testadd.ShowDialog();
            if (dialog.HasValue)
            {
                GetPatientTestInfo().Wait();
                this.Tests.ItemsSource = this.patient.Tests;
            }
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            NoteAdd noteadd = new NoteAdd(this.token, this.patient);
            bool? dialog = noteadd.ShowDialog();
            if (dialog.HasValue)
            {
                GetPatientNoteInfo().Wait();
                this.Notes.ItemsSource = this.patient.Notes;
            }
        }


        private void Medications_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Medication med = new Medication(this.token, this.patient, (EHR.Data.Models.Medication)this.Medications.SelectedValue);
            bool? dialog = med.ShowDialog();
            if (dialog.HasValue)
            {
                //GetPatientMedInfo().Wait();
                //this.Tests.ItemsSource = this.patient.Tests;
            }
        }

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
                    client.BaseAddress = new Uri("https://localhost:44339/");

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
                    client.BaseAddress = new Uri("https://localhost:44339/");

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
                    client.BaseAddress = new Uri("https://localhost:44339/");

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
    }
}
