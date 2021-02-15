using System;
using System.Collections.Generic;
using System.Text;

namespace EHR.Data.Models
{
    public class Note
    {
        public Guid PatientId { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public DateTime Recorded { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual User UserOrdered { get; set; }
    }
}
