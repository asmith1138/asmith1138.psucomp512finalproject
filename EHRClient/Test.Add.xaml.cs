using EHR.Data.Models;
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
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class TestAdd : Window
    {
        private string token;
        private EHR.Data.Models.Test test;
        private Patient patient;
        private List<TestType> testTypes;
        public TestAdd(string token, Patient patient)
        {
            InitializeComponent();
            this.token = token;
            this.test = new EHR.Data.Models.Test();
            this.patient = patient;
            this.PatientName.Content = patient.Name;
            getTestTypes();
            this.TestTypeId.ItemsSource = testTypes;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.test.PatientId = patient.MRN;
            this.test.TestTypeId = (int)this.TestTypeId.SelectedValue;
            postTest();
            this.Close();
        }

        private void getTestTypes()
        {
            GetTestTypeInfo().Wait();
        }

        private void postTest()
        {
            PostTestInfo().Wait();
        }

        public async Task PostTestInfo()
        {
            try
            {
                // client call
                using (var client = new HttpClient())
                {
                    // Base address 
                    client.BaseAddress = new Uri("https://localhost:44339/");

                    // content type 
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // timeout 
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToDouble(1000000));

                    // Msg init  
                    HttpResponseMessage response = new HttpResponseMessage();

                    // Body
                    string json = JsonConvert.SerializeObject(this.test);
                    StringContent body = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                    // HTTP POST  
                    response = await client.PostAsync("api/Tests", body).ConfigureAwait(false);

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

        public async Task GetTestTypeInfo()
        {
            // Initialization.  
            List<TestType> testTypesList = new List<TestType>();

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
                    response = await client.GetAsync("api/TestTypes").ConfigureAwait(false);

                    // Verification  
                    if (response.IsSuccessStatusCode)
                    {
                        // Reading Response.  
                        string result = response.Content.ReadAsStringAsync().Result;
                        testTypesList = JsonConvert.DeserializeObject<List<TestType>>(result);

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

            this.testTypes = testTypesList;
        }
    }
}
