using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CO.Payments.Api.Data.Database;
using CO.Payments.Api.Data.DbModels;

namespace CO.Payments.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MerchantPaymentProfilesController : ControllerBase
{
    private readonly PaymentsDbContext _context;

    public MerchantPaymentProfilesController(PaymentsDbContext context)
    {
        _context = context;
    }

    // GET: api/MerchantPaymentProfiles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MerchantPaymentProfile>>> GetMerchants()
    {
      if (_context.Merchants == null)
      {
          return NotFound();
      }
        return await _context.Merchants.ToListAsync();
    }

    // GET: api/MerchantPaymentProfiles/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MerchantPaymentProfile>> GetMerchantPaymentProfile(long id)
    {
      if (_context.Merchants == null)
      {
          return NotFound();
      }
        var merchantPaymentProfile = await _context.Merchants.FindAsync(id);

        if (merchantPaymentProfile == null)
        {
            return NotFound();
        }

        return merchantPaymentProfile;
    }

    // PUT: api/MerchantPaymentProfiles/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMerchantPaymentProfile(long id, MerchantPaymentProfile merchantPaymentProfile)
    {
        if (id != merchantPaymentProfile.MerchantId)
        {
            return BadRequest();
        }

        _context.Entry(merchantPaymentProfile).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MerchantPaymentProfileExists(id))
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

    // POST: api/MerchantPaymentProfiles
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<MerchantPaymentProfile>> PostMerchantPaymentProfile(MerchantPaymentProfile merchantPaymentProfile)
    {
      if (_context.Merchants == null)
      {
          return Problem("Entity set 'PaymentsDbContext.Merchants'  is null.");
      }
        _context.Merchants.Add(merchantPaymentProfile);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetMerchantPaymentProfile", new { id = merchantPaymentProfile.MerchantId }, merchantPaymentProfile);
    }

    // DELETE: api/MerchantPaymentProfiles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMerchantPaymentProfile(long id)
    {
        if (_context.Merchants == null)
        {
            return NotFound();
        }
        var merchantPaymentProfile = await _context.Merchants.FindAsync(id);
        if (merchantPaymentProfile == null)
        {
            return NotFound();
        }

        _context.Merchants.Remove(merchantPaymentProfile);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MerchantPaymentProfileExists(long id)
    {
        return (_context.Merchants?.Any(e => e.MerchantId == id)).GetValueOrDefault();
    }
}
