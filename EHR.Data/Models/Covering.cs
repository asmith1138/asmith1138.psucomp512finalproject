using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EHR.Data.Models
{
    [Keyless]
    public class Covering
    {
        public Guid PatientID { get; set; }
        public Guid UserId { get; set; }
        public bool Primary { get; set; }
        public virtual User User { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
