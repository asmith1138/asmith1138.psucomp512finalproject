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
        //Post method for messages
        [HttpPost]
        public ActionResult Post([FromBody]MessageReceiver simpleMessage)
        {
            //Call Message Arrived and build new message object
            MessageArrived(new Message(simpleMessage));
            return Ok();
        }

        //Event handling for throwing event
        public delegate void EventHandler(object sender, MessageEventArgs args);
        public static event EventHandler ThrowMessageArrivedEvent = delegate { };
        public void MessageArrived(Message m)
        {
            //Throw event for chat proxy to catch
            ThrowMessageArrivedEvent(this, new MessageEventArgs(m));
        }
    }
}
