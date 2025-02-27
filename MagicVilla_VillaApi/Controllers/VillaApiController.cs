using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Controllers;

[ApiController]
[Route("/api/VillaAPI")]
public class VillaApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public VillaApiController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VillaDto>>> GetVillas()
    {
        return Ok(await _context.Villas.ToListAsync());
    }

    [HttpGet("/api/VillaAPI/{id}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VillaDto>> GetVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var villa = await _context.Villas.FirstOrDefaultAsync(x => x.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        return Ok(villa);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Villa>> AddVilla([FromBody] CreateVillaDto villaDto)
    {
        if (await _context.Villas.FirstOrDefaultAsync(x => x.Name == villaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already exists");
            return BadRequest(ModelState);
        }
        if (villaDto == null)
        {
            return BadRequest(villaDto);
        }

        Villa villaToAdd = new()
        {
            Name = villaDto.Name,
            Details = villaDto.Details,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft,
            Occupancy = villaDto.Occupancy,
            ImageUrl = villaDto.ImageUrl,
            Amenity = villaDto.Amenity,
        };
        
        await _context.Villas.AddAsync(villaToAdd);
        await _context.SaveChangesAsync();
        
        return CreatedAtRoute("GetVilla", new {id = villaToAdd.Id},villaToAdd);
    }

    [HttpDelete("/api/VillaAPI/{id}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var villa = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        _context.Villas.Remove(villa);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("/api/VillaAPI/{id}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] UpdateVillaDto villaDto)
    {
        if (villaDto.Id == null || id != villaDto.Id)
        {
            return BadRequest();
        }
        Villa villaToUpdate = new()
        {
            Id = villaDto.Id,
            Name = villaDto.Name,
            Details = villaDto.Details,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft,
            Occupancy = villaDto.Occupancy,
            ImageUrl = villaDto.ImageUrl,
            Amenity = villaDto.Amenity,
        };
        _context.Villas.Update(villaToUpdate);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("/api/VillaAPI/{id}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<UpdateVillaDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villaFromDb = await _context.Villas.FirstOrDefaultAsync(v => v.Id == id);
        if (villaFromDb == null)
        {
            return BadRequest();
        }

        UpdateVillaDto villaToPatch = new()
        {
            Name = villaFromDb.Name,
            Details = villaFromDb.Details,
            Rate = villaFromDb.Rate,
            Sqft = villaFromDb.Sqft,
            Occupancy = villaFromDb.Occupancy,
            ImageUrl = villaFromDb.ImageUrl,
            Amenity = villaFromDb.Amenity,
        };

        patchDto.ApplyTo(villaToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Atualizar a entidade existente
        villaFromDb.Name = villaToPatch.Name;
        villaFromDb.Details = villaToPatch.Details;
        villaFromDb.Rate = villaToPatch.Rate;
        villaFromDb.Sqft = villaToPatch.Sqft;
        villaFromDb.Occupancy = villaToPatch.Occupancy;
        villaFromDb.ImageUrl = villaToPatch.ImageUrl;
        villaFromDb.Amenity = villaToPatch.Amenity;

        _context.Villas.Update(villaFromDb);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}