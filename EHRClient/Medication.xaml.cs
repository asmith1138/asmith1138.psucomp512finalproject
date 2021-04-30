using EHR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class Medication : Window
    {
        private string token;
        private EHR.Data.Models.Medication medication;
        private Patient patient;

        //load page
        public Medication(string token, Patient patient, EHR.Data.Models.Medication med)
        {
            InitializeComponent();
            this.token = token;
            this.patient = patient;
            this.medication = med;
            this.PatientName.Content = patient.Name;
            this.Frequency.Content = medication.Frequency;
            this.Dose.Content = medication.Dosage;
            this.ExpiresAt.Content = medication.Expires;
            this.OrderedBy.Content = medication.UserOrdered?.UserName;
        }
    }
}
