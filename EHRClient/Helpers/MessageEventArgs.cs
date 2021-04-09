using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHR.Client.Helpers
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Message m)
        {
            this.Message = m;
        }
        public Message Message;
    }
}
