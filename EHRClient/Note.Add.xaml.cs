﻿using EHR.Data.Models;
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
    /// Interaction logic for Note.xaml
    /// </summary>
    public partial class NoteAdd : Window
    {
        private string token;
        private EHR.Data.Models.Note note;
        private Patient patient;
        public NoteAdd(string token, Patient patient)
        {
            InitializeComponent();
            this.token = token;
            this.note = new EHR.Data.Models.Note();
            this.patient = patient;
            this.PatientName.Content = patient.Name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.note.Recorded = DateTime.Now;
            this.note.Text = new TextRange(this.NoteText.Document.ContentStart, this.NoteText.Document.ContentEnd).Text;
            this.note.PatientId = patient.MRN;
            postNote();
            this.Close();
        }

        private void postNote()
        {
            PostNoteInfo().Wait();
        }

        public async Task PostNoteInfo()
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
                    string json = JsonConvert.SerializeObject(this.note);
                    StringContent body = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                    // HTTP POST
                    response = await client.PostAsync("api/Notes", body).ConfigureAwait(false);

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
                        // Reading response
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
