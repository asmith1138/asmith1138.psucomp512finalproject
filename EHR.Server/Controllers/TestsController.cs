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
    public class TestsController : ControllerBase
    {
        private readonly EHRContext _context;

        public TestsController(EHRContext context)
        {
            _context = context;
        }

        // GET: api/Tests
        [HttpGet]
        [Authorize(Roles = "Physician,Nurse")]
        public async Task<ActionResult<IEnumerable<Test>>> GetTests()
        {
            return await _context.Tests.ToListAsync();
        }

        // GET: api/Tests/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Physician,Nurse")]
        public async Task<ActionResult<Test>> GetTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);

            if (test == null)
            {
                return NotFound();
            }

            return test;
        }

        // GET: api/Tests/Patient/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Physician,Nurse")]
        [Route("Patient/{id}")]
        public ActionResult<IEnumerable<Test>> GetPatientTest(Guid id)
        {
            var tests = _context.Tests.Include(t => t.TestType).Where(t => t.PatientId == id);

            if (tests == null)
            {
                return NotFound();
            }

            return this.Ok(tests);
        }

        // PUT: api/Tests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> PutTest(int id, Test test)
        {
            if (id != test.Id)
            {
                return BadRequest();
            }

            test.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
            _context.Entry(test).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestExists(id))
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

        // POST: api/Tests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<ActionResult<Test>> PostTest(Test test)
        {
            test.UserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTest", new { id = test.Id }, test);
        }

        // DELETE: api/Tests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.Id == id);
        }
    }
}
