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
        private ChatProxy _cp { get; set; }//Proxy class that runs chat services
        private Patient patient;//patient info
        private string token;//auth token
        private readonly AppSettings settings;
        private string username;

        public Chat(IOptions<AppSettings> settings)
        {
            InitializeComponent();
            this.settings = settings.Value;
        }
        //Runs on window load
        public Task ActivateAsync(string token, Patient patient, string username)
        {
            this.token = token;
            this.patient = patient;
            this.username = username;
            return Task.CompletedTask;
        }

        //Send a message thru chat
        private void sendMessage(Message m)
        {
            _cp.SendMessage(m);
            inputText.Clear();
        }

        //warn user if chat hasnt begun, send on enter key
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

        //send click event, warn user if chat hasnt started
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

        //callback delegate to show messages coming in
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

        //callback delegate to show status coming in
        public void ShowStatusMsg(string msg)
        {
            chatArea.Dispatcher.Invoke(DispatcherPriority.Normal,
                new Action(delegate ()
                {
                    chatArea.Text += msg;
                    chatArea.Text += Environment.NewLine;
                    chatArea.ScrollToEnd();
                }
            ));
        }

        //badly named show error callback delegate
        public void ShowStatus(string txt)
        {
            chatArea.Dispatcher.Invoke(DispatcherPriority.Normal,
            new Action(
                delegate () { MessageBox.Show(txt); }
            ));
        }

        //start the chat proxy which starts the various services
        private void startChat(object sender, RoutedEventArgs e)
        {
            _cp = new ChatProxy(this.ShowMessage, this.ShowStatus, this.ShowStatusMsg, token, patient, settings, username);
            if (_cp.Status)
            {
                chatArea.Text += ("Ready to chat!");
                chatArea.Text += Environment.NewLine;
            }
        }

        //close connections
        private void Chat_Closed(object sender, EventArgs e)
        {
            _cp?.stopChatServer();
        }
    }
}
