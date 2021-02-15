using EHR.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EHR.Data.Repos
{
    interface IPatientRepository
    {
        void Add(Patient item);
        void Update(Patient item);
        void Remove(Guid key);
        Patient Get(Guid id);
        IEnumerable<Patient> GetAll();
    }
}
