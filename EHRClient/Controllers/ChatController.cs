using EHR.Client.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace EHR.Client.Controllers
{
    public class ChatController : ApiController
    {
        public void Post(MessageReceiver simpleMessage)
        {
            MessageArrived(new Message(simpleMessage));
        }

        public delegate void EventHandler(object sender, MessageEventArgs args);
        public static event EventHandler ThrowMessageArrivedEvent = delegate { };
        public void MessageArrived(Message m)
        {
            ThrowMessageArrivedEvent(this, new MessageEventArgs(m));
        }
    }
}
