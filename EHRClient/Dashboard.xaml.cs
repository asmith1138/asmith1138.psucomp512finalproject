using EHRClient;
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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow logon = new MainWindow();
            logon.Show();
            this.Close();
        }

        private void AddMed_Click(object sender, RoutedEventArgs e)
        {
            MedicationAdd medadd = new MedicationAdd();
            medadd.ShowDialog();
        }

        private void AddTest_Click(object sender, RoutedEventArgs e)
        {
            TestAdd testadd = new TestAdd();
            testadd.ShowDialog();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            NoteAdd noteadd = new NoteAdd();
            noteadd.ShowDialog();
        }
    }
}
