using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CO.Payments.Api.Data.DbModels;
using CO.Payments.Api.Data.Database;

namespace CO.Payments.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardDetailsController : ControllerBase
    {
        private readonly PaymentsDbContext _context;

        public CardDetailsController(PaymentsDbContext context)
        {
            _context = context;
        }

        // GET: api/CardDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDetails>>> GetCardDetails()
        {
          if (_context.CardDetails == null)
          {
              return NotFound();
          }
            return await _context.CardDetails.ToListAsync();
        }

        // GET: api/CardDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CardDetails>> GetCardDetails(string id)
        {
          if (_context.CardDetails == null)
          {
              return NotFound();
          }
            var cardDetails = await _context.CardDetails.FindAsync(id);

            if (cardDetails == null)
            {
                return NotFound();
            }

            return cardDetails;
        }

        // PUT: api/CardDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCardDetails(string id, CardDetails cardDetails)
        {
            if (id != cardDetails.Token)
            {
                return BadRequest();
            }

            _context.Entry(cardDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardDetailsExists(id))
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

        // POST: api/CardDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CardDetails>> PostCardDetails(CardDetails cardDetails)
        {
          if (_context.CardDetails == null)
          {
              return Problem("Entity set 'PaymentsDbContext.CardDetails'  is null.");
          }
            _context.CardDetails.Add(cardDetails);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CardDetailsExists(cardDetails.Token))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCardDetails", new { id = cardDetails.Token }, cardDetails);
        }

        // DELETE: api/CardDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCardDetails(string id)
        {
            if (_context.CardDetails == null)
            {
                return NotFound();
            }
            var cardDetails = await _context.CardDetails.FindAsync(id);
            if (cardDetails == null)
            {
                return NotFound();
            }

            _context.CardDetails.Remove(cardDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardDetailsExists(string id)
        {
            return (_context.CardDetails?.Any(e => e.Token == id)).GetValueOrDefault();
        }
    }
}
