using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using ControllerBasedApi.Models;

[Route("api/[controller]")]
[ApiController]
public class WarehousesController : ControllerBase
{
    private readonly DatabaseContext _context;
    public WarehousesController(DatabaseContext context)
    {
        _context = context;
    }

    // GET: api/Warehouse
    [HttpGet]
    [EndpointSummary("Returns all warehouses")]
    [ProducesResponseType(typeof(IEnumerable<Warehouse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetWarehouse()
    {
        return await _context.Warehouses.ToListAsync();
    }

    // GET: api/Warehouse/5
    [HttpGet("{id}")]
    [EndpointSummary("Returns warehouse by id")]
    [ProducesResponseType(typeof(Warehouse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Warehouse>> GetWarehouse(long id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);

        if (warehouse == null)
        {
            return NotFound();
        }

        return warehouse;
    }

    // PUT: api/Warehouse/5
    [EndpointSummary("Updates warehouse in database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutWarehouse(long? id, Warehouse warehouse)
    {
        if (id != warehouse.Id)
        {
            return BadRequest();
        }

        _context.Entry(warehouse).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WarehouseExists(id))
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

    // POST: api/Warehouse
    [HttpPost]
    [EndpointSummary("Inserts warehouse into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Warehouse>> PostWarehouse(Warehouse warehouse)
    {
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetWarehouse", new { id = warehouse.Id }, warehouse);
    }

    // DELETE: api/Warehouse/5
    [HttpDelete("{id}")]
    [EndpointSummary("Deletes warehouse from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWarehouse(long? id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return NotFound();
        }

        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WarehouseExists(long? id)
    {
        return _context.Warehouses.Any(e => e.Id == id);
    }
}
