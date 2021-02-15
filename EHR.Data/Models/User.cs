using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHR.Data.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string Password { get; set; }
        public virtual Role Role { get; set; }
        public virtual IQueryable<Note> NotesWritten { get; set; }
        public virtual IQueryable<Test> TestsOrdered { get; set; }
        public virtual IQueryable<Medication> MedicationsOrdered { get; set; }
        public virtual IQueryable<Covering> PatientsCovering { get; set; }
    }
}
