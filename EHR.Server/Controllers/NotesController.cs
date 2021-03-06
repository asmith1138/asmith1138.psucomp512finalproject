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
    public class NotesController : ControllerBase
    {
        //authorize via JwtBearer and route config
        //data context
        private readonly EHRContext _context;

        //DI
        public NotesController(EHRContext context)
        {
            _context = context;
        }

        // GET: api/Notes
        [HttpGet]
        [Authorize(Roles = "Physician,Nurse")]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            //Get notes, unused endpoint
            return await _context.Notes.ToListAsync();
        }

        // GET: api/Notes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Physician,Nurse")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            //get a single note by id, unused endpoint
            var note = await _context.Notes.FindAsync(id);

            if (note == null)
            {
                return NotFound();
            }

            return note;
        }

        // GET: api/Notes/Patient/5
        [HttpGet("{id:Guid}")]
        [Authorize(Roles = "Physician,Nurse")]
        [Route("Patient/{id}")]
        public ActionResult<IEnumerable<Note>> GetPatientNotes(Guid id)
        {
            //get patient notes, called from dashboard
            var notes = _context.Notes.Where(t => t.PatientId == id);

            if (notes == null)
            {
                return NotFound();
            }

            return this.Ok(notes);
        }

        // PUT: api/Notes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> PutNote(int id, Note note)
        {
            //update note, currently unused
            if (id != note.Id)
            {
                return BadRequest();
            }

            note.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(id))
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

        // POST: api/Notes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<ActionResult<Note>> PostNote(Note note)
        {
            //post new note used by add note window
            note.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNote", new { id = note.Id }, note);
        }

        // DELETE: api/Notes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            //delete note, currently unused
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //does note exist, unused
        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}
