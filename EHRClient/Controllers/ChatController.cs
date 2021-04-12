using EHR.Client.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHR.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post([FromBody]MessageReceiver simpleMessage)
        {
            MessageArrived(new Message(simpleMessage));
            return Ok();
        }

        public delegate void EventHandler(object sender, MessageEventArgs args);
        public static event EventHandler ThrowMessageArrivedEvent = delegate { };
        public void MessageArrived(Message m)
        {
            ThrowMessageArrivedEvent(this, new MessageEventArgs(m));
        }
    }
}
