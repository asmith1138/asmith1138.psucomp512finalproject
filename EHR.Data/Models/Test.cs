using System;
using System.Collections.Generic;
using System.Text;

namespace EHR.Data.Models
{
    public class Test
    {
        public int TestTypeId { get; set; }
        public Guid PatientId { get; set; }
        public Guid UserId { get; set; }
        public string Results { get; set; }
        public DateTime? Performed { get; set; }
        public virtual TestType TestType { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual User UserOrdered { get; set; }
    }
}
