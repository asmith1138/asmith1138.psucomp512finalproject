using EHR.Client.Controllers;
using EHR.Data.Models;
using EHRClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EHR.Client.Helpers
{
    public class ChatProxy
    {
        //Declarations
        public bool Status { get; set; }
        public Patient _patient;
        public string _token;
        public string _username;
        //delegates for method/callback injections
        public delegate void ShowReceivedMessage(Message m);
        public delegate void ShowError(string txt);
        public delegate void ShowStatusMsg(string txt);
        private ShowReceivedMessage _srm;
        private ShowError _sst;
        private ShowStatusMsg _stm;
        //p2p connections
        private List<Tuple<string,string,HttpClient>> _clients;
        private IWebHost _host;
        private HubConnection _server;
        private AppSettings _settings;

        //constructor
        public ChatProxy(ShowReceivedMessage srm, ShowError sst, ShowStatusMsg stm, string token, Patient patient, AppSettings settings, string username)
        {
            _patient = patient;
            _token = token;
            _settings = settings;
            _username = username;
            //start chat on port 1138 and connect to server
            StartChatServer("1138");
            if (Status)
            {
                //setting method calls
                _srm = srm;
                _sst = sst;
                _stm = stm;

                _clients = new List<Tuple<string, string, HttpClient>>();

                //catching event throwing
                ChatController.ThrowMessageArrivedEvent += (sender, args) => { ShowMessage(args.Message); };
            }
        }

        //Add client to list
        private void AddNewClient(string ConnId, string username, string partneraddress)
        {
            //new client
            var client = new HttpClient() { BaseAddress = new Uri(partneraddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _clients.Add(new Tuple<string, string, HttpClient>(ConnId, username, client));
        }

        //start chat
        private void StartChatServer(string myport)
        {
            try
            {
                //Build listening URL
                string url = "http://" + NetworkIP.GetLocalIP() + ":" + myport + "/";

                // Start listening
                _host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls(url)
                    .UseStartup<P2PServer>()
                    .Build();

                //listen
                _host.RunAsync();
                
                //connection to server
                _server = new HubConnectionBuilder()
                    .WithUrl(_settings.HubUrl +  "/Chat")
                    .Build();

                //callback when connection errors
                _server.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _server.StartAsync();
                };

                //connect to server
                ConnectHub(url);

                Status = true;
            }
            catch (Exception e)
            {
                Status = false;
                ShowErrorMsg("Something happened!");
            }
        }

        private async void ConnectHub(string url)
        {
            //uri to be added endpoint
            _server.On<string,string,string>("Ready", (ConnId, username, uri) =>
            {
                if(ConnId != _server.ConnectionId)
                {
                    AddNewClient(ConnId, username, uri);
                    ShowStatus(username + " has joined");
                }       
            });

            //uri to be removed endpoint
            _server.On<string>("Left", (ConnId) =>
            {

                ShowStatus(_clients.SingleOrDefault(c => c.Item1 == ConnId).Item2 + " has left");
                _clients.Remove(_clients.SingleOrDefault(c => c.Item1 == ConnId));
            });

            //Server fallback messaging endpoint
            _server.On<object>("ServerMessage", (message) =>
            {
                ShowMessage((Message)message);
                //_clients.Remove(_clients.SingleOrDefault(c => c.Item1 == ConnId));
            });

            //Return MRN and list of clients in group endpoint
            _server.On<string[],string[],string[]>("Joined", (connids, names, urls) =>
            {
                if(connids.Length == names.Length && connids.Length == urls.Length)
                {
                    var parts = connids
                    .Zip(names, (c, n) => new { ConnId = c, Name = n })
                    .Zip(urls, (g, u) => new { ConnId = g.ConnId, Name = g.Name, Url = u });

                    //build client list
                    foreach(var part in parts)
                    {
                        if (part.ConnId != _server.ConnectionId)
                        {
                            AddNewClient(part.ConnId, part.Name, part.Url);
                            ShowStatus(part.Name + " has joined");
                        }
                    }
                }
            });

            try
            {
                //start server connection and join room
                await _server.StartAsync();
                await _server.InvokeAsync("Join", _patient.MRN, _username, url);
            }
            catch (Exception ex)
            {
                //Failed to connect to hub
                ShowErrorMsg(ex.Message);
            }
        }

        //not used but is server fallback for the entire chat
        private async void sendThruHub(string message)
        {
            try
            {
                await _server.InvokeAsync("SendMessage",
                    _patient.MRN, message);
            }
            catch (Exception ex)
            {
                ShowMessage(new Message("Error", ex.Message));
            }
        }

        //cleaning up
        public void stopChatServer()
        {
            _host?.StopAsync();
            _server?.StopAsync();
        }

        //callback show message
        private void ShowMessage(Message m)
        {
            _srm(m);
        }

        //callback error
        private void ShowErrorMsg(string txt)
        {
            _sst(txt);
        }

        //callback status
        private void ShowStatus(string txt)
        {
            _stm(txt);
        }

        //send a message to peers
        public async void SendMessage(Message m)
        {
            //loop through and send
            foreach (var client in _clients)
            {
                try
                {
                    HttpResponseMessage response = await client.Item3.PostAsync(
                        "api/chat",
                        new StringContent(m.serializedMessage, UnicodeEncoding.UTF8, "application/json"));
                    //send through server on failure
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        await _server.InvokeAsync("SendOneMessage", _patient.MRN, m, client.Item1);
                    //ShowErrorMsg("A partner responded, but not 200!");
                }
                catch (Exception e)
                {
                    //send thrrough server on failure
                    await _server.InvokeAsync("SendOneMessage", _patient.MRN, m, client.Item1);
                    //ShowErrorMsg("A member is unreachable!");
                }
            }
            ShowMessage(m);
        }
    }
}
