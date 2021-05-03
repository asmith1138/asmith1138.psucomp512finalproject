using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EHR.Data.Models
{
    //Medication for a patient, related to user by user ordered
    public class Medication
    {
        public int Id { get; set; }
        public Guid PatientId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
        public string Dosage { get; set; }
        public DateTime? Expires { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        [ForeignKey("UserId")]
        public virtual User UserOrdered { get; set; }
    }
}
