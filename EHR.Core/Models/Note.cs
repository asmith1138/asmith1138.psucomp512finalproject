using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EHR.Data.Models
{
    //Note for a patient, related to user by user ordered
    public class Note
    {
        public int Id { get; set; }
        public Guid PatientId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime Recorded { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        [ForeignKey("UserId")]
        public virtual User UserOrdered { get; set; }
    }
}
