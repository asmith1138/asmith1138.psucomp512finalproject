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
    /// Interaction logic for Note.xaml
    /// </summary>
    public partial class Note : Window
    {
        private string token;
        private EHR.Data.Models.Note note;
        private Patient patient;
        public Note(string token, Patient patient, EHR.Data.Models.Note note)
        {
            InitializeComponent();
            this.token = token;
            this.patient = patient;
            this.note = note;
            this.PatientName.Content = patient.Name;
            this.NoteText.Content = note.Text;
            this.Recorded.Content = note.Recorded;
            this.NoteBy.Content = note.UserOrdered.UserName;
        }
    }
}
