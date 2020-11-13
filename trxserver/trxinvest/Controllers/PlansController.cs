using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trxinvest.Data;
using trxinvest.Models;

namespace trxinvest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly PlansContext _context;

        public PlansController(PlansContext context)
        {
            _context = context;
        }

        // GET: api/Plans
        [HttpGet]
        public IEnumerable<Plans> GetPlans()
        {
            return _context.Plans;
        }

        // GET: api/Plans/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlans([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plans = await _context.Plans.FindAsync(id);

            if (plans == null)
            {
                return NotFound();
            }

            return Ok(plans);
        }

        // PUT: api/Plans/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlans([FromRoute] long id, [FromBody] Plans plans)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != plans.Id)
            {
                return BadRequest();
            }

            _context.Entry(plans).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlansExists(id))
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

        // POST: api/Plans
        [HttpPost]
        public async Task<IActionResult> PostPlans([FromBody] Plans plans)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Plans.Add(plans);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlans", new { id = plans.Id }, plans);
        }

        // DELETE: api/Plans/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlans([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var plans = await _context.Plans.FindAsync(id);
            if (plans == null)
            {
                return NotFound();
            }

            _context.Plans.Remove(plans);
            await _context.SaveChangesAsync();

            return Ok(plans);
        }

        private bool PlansExists(long id)
        {
            return _context.Plans.Any(e => e.Id == id);
        }
    }
}