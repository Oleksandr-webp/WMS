using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using ControllerBasedApi.Models;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly DatabaseContext _context;
    public LocationsController(DatabaseContext context)
    {
        _context = context;
    }

    // GET: api/Location
    [HttpGet]
    [EndpointSummary("Returns all locations")]
    [ProducesResponseType(typeof(IEnumerable<Location>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocation()
    {
        return await _context.Locations.ToListAsync();
    }

    // GET: api/Location/5
    [HttpGet("{id}")]
    [EndpointSummary("Returns location by id")]
    [ProducesResponseType(typeof(Location), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Location>> GetLocation(long id)
    {
        var location = await _context.Locations.FindAsync(id);

        if (location == null)
        {
            return NotFound();
        }

        return location;
    }

    // GET: api/Location/Warehouse/5
    [HttpGet("warehouse/{warehouseId}")]
    [EndpointSummary("Returns locations by warehouse id")]
    [ProducesResponseType(typeof(IEnumerable<Location>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Location>>> GetLocationsByWarehouseId(long warehouseId)
    {
        var locations = await _context.Locations
            .Where(l => l.WarehouseId == warehouseId)
            .ToListAsync();

        return Ok(locations);
    }

    // PUT: api/Location/5
    [HttpPut("{id}")]
    [EndpointSummary("Updates location in database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutLocation(long? id, Location location)
    {
        if (id != location.Id)
        {
            return BadRequest();
        }

        _context.Entry(location).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LocationExists(id))
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

    // POST: api/Location
    [HttpPost]
    [EndpointSummary("Inserts location into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Location>> PostLocation(Location location)
    {
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetLocation", new { id = location.Id }, location);
    }

    // DELETE: api/Location/5
    [HttpDelete("{id}")]
    [EndpointSummary("Deletes location from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLocation(long? id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
            return NotFound();
        }

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LocationExists(long? id)
    {
        return _context.Locations.Any(e => e.Id == id);
    }
}
