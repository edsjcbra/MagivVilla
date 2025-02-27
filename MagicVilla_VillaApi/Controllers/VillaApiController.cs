using AutoMapper;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.DTOs;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaApi.Controllers;

[ApiController]
[Route("/api/VillaAPI")]
public class VillaApiController : ControllerBase
{
    private readonly IVillaRepository _repository;
    private readonly IMapper _mapper;

    public VillaApiController(IVillaRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VillaDto>>> GetVillas()
    {
        IEnumerable<Villa> villas = await _repository.GetAllAsync();
        return Ok(_mapper.Map<List<VillaDto>>(villas));
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

        var villa = await _repository.GetByIdAsync(x => x.Id == id);
        if (villa == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<VillaDto>(villa));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Villa>> AddVilla([FromBody] CreateVillaDto createVillaDto)
    {
        if (await _repository.GetByIdAsync(x => x.Name == createVillaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already exists");
            return BadRequest(ModelState);
        }

        if (createVillaDto == null)
        {
            return BadRequest(createVillaDto);
        }

        var villaToAdd = _mapper.Map<Villa>(createVillaDto);

        await _repository.CreateAsync(villaToAdd);

        return CreatedAtRoute("GetVilla", new { id = villaToAdd.Id }, villaToAdd);
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

        var villaToDelete = await _repository.GetByIdAsync(x => x.Id == id);
        if (villaToDelete == null)
        {
            return NotFound();
        }

        _repository.DeleteAsync(villaToDelete);
        return NoContent();
    }

    [HttpPut("/api/VillaAPI/{id}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] UpdateVillaDto updateVillaDto)
    {
        if (updateVillaDto.Id == null || id != updateVillaDto.Id)
        {
            return BadRequest();
        }

        var villaToUpdate = _mapper.Map<Villa>(updateVillaDto);

        await _repository.UpdateVillaAsync(villaToUpdate);
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

        var villaFromDb = await _repository.GetByIdAsync(x => x.Id == id, tracked: false);
        if (villaFromDb == null)
        {
            return BadRequest();
        }
        
        var villaToPatch = _mapper.Map<UpdateVillaDto>(villaFromDb);
        patchDto.ApplyTo(villaToPatch, ModelState);
        
        villaFromDb = _mapper.Map<Villa>(villaToPatch);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _repository.UpdateVillaAsync(villaFromDb);

        return NoContent();
    }
}