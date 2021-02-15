using System;
using System.Collections.Generic;
using System.Text;

namespace EHR.Data.Models
{
    public class Medication
    {
        public Guid PatientId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
        public string Dosage { get; set; }
        public DateTime? Expires { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual User UserOrdered { get; set; }
    }
}
