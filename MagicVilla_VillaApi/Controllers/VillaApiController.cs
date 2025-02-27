using System.Net;
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
    protected ApiResponse _response;

    public VillaApiController(IVillaRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        this._response = new ApiResponse();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetVillas()
    {
        try
        {
            IEnumerable<Villa> villas = await _repository.GetAllAsync();
            _response.ApiContent = _mapper.Map<List<VillaDto>>(villas);
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSucess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return (_response);
    }

    [HttpGet("/api/VillaAPI/{id}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> GetVilla(int id)
    {
        try
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

            _response.ApiContent = _mapper.Map<VillaDto>(villa);
            _response.StatusCode = HttpStatusCode.OK;
        }
        catch (Exception e)
        {
            _response.IsSucess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return Ok(_response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse>> AddVilla([FromBody] CreateVillaDto createVillaDto)
    {
        try
        {
            if (createVillaDto == null)
            {
                return BadRequest("Invalid data"); 
            }
            var existingVilla = await _repository.GetByIdAsync(x => x.Name.ToLower() == createVillaDto.Name.ToLower());

            if (existingVilla != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }

            var villaToAdd = _mapper.Map<Villa>(createVillaDto);
            await _repository.CreateAsync(villaToAdd);

            _response.ApiContent = _mapper.Map<VillaDto>(villaToAdd);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetVilla", new { id = villaToAdd.Id }, _response);
        }
        catch (Exception e)
        {
            _response.IsSucess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return Ok(_response);
    }


    [HttpDelete("/api/VillaAPI/{id}", Name = "DeleteVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteVilla(int id)
    {
        try
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
            await _repository.DeleteAsync(villaToDelete);
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSucess = true;
        }
        catch (Exception e)
        {
            _response.IsSucess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return Ok(_response);
        
    }

    [HttpPut("/api/VillaAPI/{id}", Name = "UpdateVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromBody] UpdateVillaDto updateVillaDto)
    {
        try
        {
            if (updateVillaDto.Id == null || id != updateVillaDto.Id)
            {
                return BadRequest();
            }

            var villaToUpdate = _mapper.Map<Villa>(updateVillaDto);

            await _repository.UpdateVillaAsync(villaToUpdate);
        
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSucess = true;
        }
        catch (Exception e)
        {
            _response.IsSucess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }
        return Ok(_response);
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