using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EHR.Data;
using EHR.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace EHR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MedicationsController : ControllerBase
    {
        private readonly EHRContext _context;

        public MedicationsController(EHRContext context)
        {
            _context = context;
        }

        // GET: api/Medications
        [HttpGet]
        [Authorize(Roles = "Physician,Nurse")]
        public async Task<ActionResult<IEnumerable<Medication>>> GetMedications()
        {
            return await _context.Medications.ToListAsync();
        }

        // GET: api/Medications/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Physician,Nurse")]
        public async Task<ActionResult<Medication>> GetMedication(int id)
        {
            var medication = await _context.Medications.FindAsync(id);

            if (medication == null)
            {
                return NotFound();
            }

            return medication;
        }

        // GET: api/Medications/Patient/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Physician,Nurse")]
        [Route("Patient/{id}")]
        public ActionResult<IEnumerable<Medication>> GetPatientMedications(Guid id)
        {
            var meds = _context.Medications.Where(t => t.PatientId == id);

            if (meds == null)
            {
                return NotFound();
            }

            return this.Ok(meds);
        }

        // PUT: api/Medications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> PutMedication(int id, Medication medication)
        {
            if (id != medication.Id)
            {
                return BadRequest();
            }

            medication.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
            _context.Entry(medication).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Medications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<ActionResult<Medication>> PostMedication(Medication medication)
        {
            medication.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedication", new { id = medication.Id }, medication);
        }

        // DELETE: api/Medications/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> DeleteMedication(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }

            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicationExists(int id)
        {
            return _context.Medications.Any(e => e.Id == id);
        }
    }
}
