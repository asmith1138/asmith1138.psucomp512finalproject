using EHR.Client.Helpers;
using EHR.Data.Models;
using Microsoft.Extensions.Options;
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
using System.Windows.Threading;

namespace EHR.Client
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window, IActivable
    {
        private ChatProxy _cp { get; set; }
        private Patient patient;
        private string token;
        private readonly AppSettings settings;
        private string username;

        public Chat(IOptions<AppSettings> settings)
        {
            InitializeComponent();
            this.settings = settings.Value;
        }
        public Task ActivateAsync(string token, Patient patient, string username)
        {
            this.token = token;
            this.patient = patient;
            this.username = username;
            return Task.CompletedTask;
        }

        private void sendMessage(Message m)
        {
            _cp.SendMessage(m);
            inputText.Clear();
        }
        private void userInputText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_cp != null)
                {
                    if (!string.IsNullOrEmpty(inputText.Text))
                        sendMessage(new Message(username, inputText.Text));
                    else
                        ShowStatus("Nothing to send!");
                }
                else
                {
                    ShowStatus("Chat not started!");
                }
            }
        }

        private void click_sendMessage(object sender, RoutedEventArgs e)
        {
            if (_cp != null)
            {
                if (!string.IsNullOrEmpty(inputText.Text))
                    sendMessage(new Message(username, inputText.Text));
                else
                    ShowStatus("Nothing to send!");
            }
            else
            {
                ShowStatus("Chat not started!");
            }
        }

        public void ShowMessage(Message m)
        {
            chatArea.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    chatArea.Text += ("[" + m.Sent + "] " + m.Username + ": " + m.Text);
                    chatArea.Text += Environment.NewLine;
                    chatArea.ScrollToEnd();
                }
            ));
        }
        public void ShowStatus(string txt)
        {
            chatArea.Dispatcher.Invoke(DispatcherPriority.Normal,
            new Action(
                delegate () { MessageBox.Show(txt); }
            ));
        }

        private void startChat(object sender, RoutedEventArgs e)
        {
            _cp = new ChatProxy(this.ShowMessage, this.ShowStatus, token, patient, settings, username);
            if (_cp.Status)
            {
                chatArea.Text += ("Ready to chat!");
                chatArea.Text += Environment.NewLine;
            }
        }
    }
}
