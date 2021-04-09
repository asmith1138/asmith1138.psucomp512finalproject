using EHR.Client.Controllers;
using EHRClient;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace EHR.Client.Helpers
{
    public class ChatProxy
    {
        public bool Status { get; set; }
        public delegate void ShowReceivedMessage(Message m);
        public delegate void ShowError(string txt);
        private ShowReceivedMessage _srm;
        private ShowError _sst;
        private HttpClient _client;
        private HttpSelfHostServer _server;
        private IDisposable _serverDep;

        //constructor
        public ChatProxy(ShowReceivedMessage srm, ShowError sst, string myport, string partneraddress)
        {
            StartChatServer(myport);
            if (Status)
            {
                _srm = srm;
                _sst = sst;
                _client = new HttpClient() { BaseAddress = new Uri(partneraddress) };
                //ChatController.ThrowMessageArrivedEvent += (sender, args) => { ShowMessage(args.Message); };
            }
        }

        private void StartChatServer(string myport)
        {
            try
            {
                string url = "http://localhost:" + myport + "/";
                //HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(url);
                //config.Routes.MapHttpRoute(
                //  name: "DefaultApi",
                //  routeTemplate: "api/{controller}/{id}",
                //  defaults: new { id = RouteParameter.Optional }
                //);
                // Start OWIN host
                _serverDep = WebApp.Start<P2PServer>(url);

                //using (WebApp.Start(url))
                //{
                //    var handler = new HttpClientHandler
                //    {
                //        UseDefaultCredentials = true
                //    };
                //    using (var client = new HttpClient(handler))
                //    {
                //        //client.BaseAddress = new Uri(url);
                //        //var response = await client.GetAsync("/api/Chat");
                //        //response.EnsureSuccessStatusCode();
                //        //var result = await response.Content.ReadAsAsync<List<string>>();
                //        //Assert.Equal(2, result.Count);
                //    }
                //}
                //_server = new HttpSelfHostServer(config);
                //_server.OpenAsync().Wait();
                Status = true;
            }
            catch (Exception e)
            {
                Status = false;
                ShowErrorMsg("Something happened!");
            }
        }
        private void stopChatServer()
        {
            //_server.CloseAsync().Wait();
            _serverDep.Dispose();
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
                HttpResponseMessage response = await _client.PostAsync("api/chat", m.serializedMessage);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    ShowErrorMsg("Partner responded, but awkwardly! Better hide!");
                ShowMessage(m);
            }
            catch (Exception e)
            {
                stopChatServer();
                ShowErrorMsg("Partner unreachable. Closing your server!");
            }
        }
    }
}
