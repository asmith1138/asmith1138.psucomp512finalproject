using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHR.Data.Models
{
    //Related to covering, notes, tests, meds, and roles
    //ASP.NET Identity handles the rest of this table definition through IdentityUser inheritence
    public class User : IdentityUser
    {
        //public Guid Id { get; set; }
        //public string UserName { get; set; }
        //public string Email { get; set; }
        public string RoleId { get; set; }
        //public string Password { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Note> NotesWritten { get; set; }
        public virtual ICollection<Test> TestsOrdered { get; set; }
        public virtual ICollection<Medication> MedicationsOrdered { get; set; }
        public virtual ICollection<Covering> PatientsCovering { get; set; }
    }
}
