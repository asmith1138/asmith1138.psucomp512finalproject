using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHR.Data.Models
{
    public class Patient
    {
        public Guid MRN { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public DateTime DOB { get; set; }
        public virtual IQueryable<Note> Notes { get; set; }
        public virtual IQueryable<Test> Tests { get; set; }
        public virtual IQueryable<Medication> Medications { get; set; }
        public virtual IQueryable<Covering> CareTeam { get; set; }
    }
}
