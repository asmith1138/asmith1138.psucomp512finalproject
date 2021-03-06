using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EHR.Data.Models
{
    //Test for a patient, related to user by user ordered, has a test type
    public class Test
    {
        public int Id { get; set; }
        public int TestTypeId { get; set; }
        public Guid PatientId { get; set; }
        public string UserId { get; set; }
        public string Results { get; set; }
        public DateTime? Performed { get; set; }
        public virtual TestType TestType { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }
        [ForeignKey("UserId")]
        public virtual User UserOrdered { get; set; }
    }
}
