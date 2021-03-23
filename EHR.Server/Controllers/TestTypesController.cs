﻿using System;
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

namespace EHR.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TestTypesController : ControllerBase
    {
        private readonly EHRContext _context;

        public TestTypesController(EHRContext context)
        {
            _context = context;
        }

        // GET: api/TestTypes
        [HttpGet]
        [Authorize(Roles = "Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<TestType>>> GetTypesOfTests()
        {
            return await _context.TypesOfTests.ToListAsync();
        }

        // GET: api/TestTypes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Nurse")]
        public async Task<ActionResult<TestType>> GetTestType(int id)
        {
            var testType = await _context.TypesOfTests.FindAsync(id);

            if (testType == null)
            {
                return NotFound();
            }

            return testType;
        }

        // PUT: api/TestTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> PutTestType(int id, TestType testType)
        {
            if (id != testType.Id)
            {
                return BadRequest();
            }

            _context.Entry(testType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestTypeExists(id))
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

        // POST: api/TestTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<TestType>> PostTestType(TestType testType)
        {
            _context.TypesOfTests.Add(testType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestType", new { id = testType.Id }, testType);
        }

        // DELETE: api/TestTypes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteTestType(int id)
        {
            var testType = await _context.TypesOfTests.FindAsync(id);
            if (testType == null)
            {
                return NotFound();
            }

            _context.TypesOfTests.Remove(testType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TestTypeExists(int id)
        {
            return _context.TypesOfTests.Any(e => e.Id == id);
        }
    }
}
