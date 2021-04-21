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
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        private string token;
        private EHR.Data.Models.Test test;
        private Patient patient;
        public Test(string token, Patient patient, EHR.Data.Models.Test test)
        {
            InitializeComponent();
            this.token = token;
            this.patient = patient;
            this.test = test;
            this.PatientName.Content = patient.Name;
            this.TestType.Content = test.TestType.Name;
            this.Results.Content = test.Results;
            this.Performed.Content = test.Performed;
            this.OrderedBy.Content = test.UserOrdered?.UserName;
        }
    }
}
