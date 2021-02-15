using EHR.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHR.Data.Repos
{
    public class PatientRepository : IPatientRepository
    {
        private readonly EHRContext db = new EHRContext();

        public PatientRepository()
        {
            //Add(new ContactModel { Id = 0, Name = "Item 1", Email = "This is an item description." });
            //Add(new ContactModel { Id = 1, Name = "Item 2", Email = "This is an item description." });
            //Add(new ContactModel { Id = 2, Name = "Item 3", Email = "This is an item description." });
        }

        public Patient Get(Guid id)
        {
            return db.Patients.FirstOrDefault(c => c.MRN == id);
        }

        public IEnumerable<Patient> GetAll()
        {
            return db.Patients.ToList();
        }

        public void Add(Patient item)
        {
            //item.MRN = NextId();
            Patient newPatient = db.Add(item).Entity;
            db.SaveChanges();
        }

        public Patient Find(Guid id)
        {
            return db.Patients.FirstOrDefault(c => c.MRN == id);
        }

        public void Remove(Guid id)
        {
            db.Patients.Remove(db.Patients.FirstOrDefault(c => c.MRN == id));
        }

        public void Update(Patient item)
        {
            db.Update(item);
            db.SaveChanges();
        }
    }
}
