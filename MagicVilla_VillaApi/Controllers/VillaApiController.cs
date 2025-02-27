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
    public ActionResult<List<VillaDto>> GetVillas()
    {
        return Ok(_context.Villas.ToList());
    }

    [HttpGet("/api/VillaAPI/{id}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDto> GetVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var villa = _context.Villas.FirstOrDefault(x => x.Id == id);
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
    public ActionResult<Villa> AddVilla([FromBody] VillaDto villaDto)
    {
        if (_context.Villas.FirstOrDefault(x => x.Name == villaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already exists");
            return BadRequest(ModelState);
        }
        if (villaDto == null)
        {
            return BadRequest(villaDto);
        }

        if (villaDto.Id > 0)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Villa villaToAdd = new()
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
        
        _context.Villas.Add(villaToAdd);
        _context.SaveChanges();
        
        return CreatedAtRoute("GetVilla", new {id = villaDto.Id},villaDto);
    }

    [HttpDelete("/api/VillaAPI/{id}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var villa = _context.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        _context.Villas.Remove(villa);
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPut("/api/VillaAPI/{id}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
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
        _context.SaveChanges();
        return NoContent();
    }

    [HttpPatch("/api/VillaAPI/{id}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villaFromDb = _context.Villas.FirstOrDefault(v => v.Id == id);
        if (villaFromDb == null)
        {
            return BadRequest();
        }

        VillaDto villaToPatch = new()
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
        _context.SaveChanges();

        return NoContent();
    }

}