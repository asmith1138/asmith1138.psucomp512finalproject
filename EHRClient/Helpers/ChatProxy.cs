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
        public bool Status { get; set; }
        public Patient _patient;
        public string _token;
        public string _username;
        public delegate void ShowReceivedMessage(Message m);
        public delegate void ShowError(string txt);
        private ShowReceivedMessage _srm;
        private ShowError _sst;
        private List<Tuple<string,string,HttpClient>> _clients;
        private IWebHost _host;
        private HubConnection _server;
        private AppSettings _settings;

        //constructor
        public ChatProxy(ShowReceivedMessage srm, ShowError sst, string token, Patient patient, AppSettings settings, string username)
        {
            _patient = patient;
            _token = token;
            _settings = settings;
            _username = username;
            StartChatServer("9001");
            if (Status)
            {
                _srm = srm;
                _sst = sst;

                _clients = new List<Tuple<string, string, HttpClient>>();
                //AddNewClient("-1","Local",partneraddress);
                
                ChatController.ThrowMessageArrivedEvent += (sender, args) => { ShowMessage(args.Message); };
            }
        }

        private void AddNewClient(string ConnId, string username, string partneraddress)
        {
            //new client
            var client = new HttpClient() { BaseAddress = new Uri(partneraddress) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _clients.Add(new Tuple<string, string, HttpClient>(ConnId, username, client));
        }

        private void StartChatServer(string myport)
        {
            try
            {
                string url = "http://" + NetworkIP.GetLocalIPAddress() + ":" + myport + "/";

                // Start OWIN host
                _host = new WebHostBuilder()
                    .UseKestrel()
                    .UseUrls(url)
                    .UseStartup<P2PServer>()
                    .Build();

                _host.RunAsync();
                
                _server = new HubConnectionBuilder()
                    .WithUrl(_settings.HubUrl +  "/Chat")
                    .Build();

                _server.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await _server.StartAsync();
                };

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
            //uri to be added
            _server.On<string,string,string>("Ready", (ConnId, username, uri) =>
            {
                if(ConnId != _server.ConnectionId)
                {
                    AddNewClient(ConnId, username, uri);
                }       
            });

            //uri to be removed
            _server.On<string>("Left", (ConnId) =>
            {
                _clients.Remove(_clients.SingleOrDefault(c => c.Item1 == ConnId));
            });

            //Return MRN and list of clients in group
            _server.On<string[],string[],string[]>("Joined", (connids, names, urls) =>
            {
                if(connids.Length == names.Length && connids.Length == urls.Length)
                {
                    var parts = connids
                    .Zip(names, (c, n) => new { ConnId = c, Name = n })
                    .Zip(urls, (g, u) => new { ConnId = g.ConnId, Name = g.Name, Url = u });

                    foreach(var part in parts)
                    {
                        if (part.ConnId != _server.ConnectionId)
                        {
                            AddNewClient(part.ConnId, part.Name, part.Url);
                        }
                    }
                }
            });

            try
            {
                await _server.StartAsync();
                await _server.InvokeAsync("Join", _patient.MRN, _username, url);
            }
            catch (Exception ex)
            {
                //Failed to connect to hub
                ShowErrorMsg(ex.Message);
            }
        }

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

        private void stopChatServer()
        {
            _host.StopAsync();
        }
        private void ShowMessage(Message m)
        {
            _srm(m);
        }
        private void ShowErrorMsg(string txt)
        {
            _sst(txt);
        }

        public async void SendMessage(Message m)
        {
            try
            {
                foreach(var client in _clients)
                {
                    HttpResponseMessage response = await client.Item3.PostAsync(
                    "api/chat",
                    new StringContent(m.serializedMessage, UnicodeEncoding.UTF8, "application/json"));
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        ShowErrorMsg("A partner responded, but not 200!");
                }
                ShowMessage(m);
            }
            catch (Exception e)
            {
                //TODO: Call server for 1 partner that failed
                ShowErrorMsg("A member is unreachable!");

            }
        }
    }
}
