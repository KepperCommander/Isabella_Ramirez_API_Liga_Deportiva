using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;

    public SponsorController(ISponsorService sponsorService, IMapper mapper)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors)); // 200
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null) return NotFound(new { message = "Sponsor no encontrado" }); // 404
        return Ok(_mapper.Map<SponsorResponseDTO>(sponsor)); // 200
    }

    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            var created = await _sponsorService.CreateAsync(sponsor);
            var responseDto = _mapper.Map<SponsorResponseDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto); // 201
        }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); } // 409
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent(); // 204
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); } // 404
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); } // 409
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent(); // 204
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); } // 404
    }

    

    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentResponseDTO>>> GetTournaments(int id)
    {
        try
        {
            var tournaments = await _sponsorService.GetTournamentsBySponsorAsync(id);
            return Ok(_mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments)); // 200
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); } // 404
    }

    [HttpPost("{id}/tournaments")]
    public async Task<ActionResult> RegisterToTournament(int id, TournamentSponsorRequestDTO dto)
    {
        try
        {
            await _sponsorService.RegisterSponsorToTournamentAsync(id, dto.TournamentId, dto.ContractAmount);
            return StatusCode(201, new { message = "Sponsor vinculado al torneo" }); // 201
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); } // 404
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); } // 409
    }

    [HttpDelete("{id}/tournaments/{tid}")]
    public async Task<ActionResult> RemoveFromTournament(int id, int tid)
    {
        try
        {
            await _sponsorService.RemoveSponsorFromTournamentAsync(id, tid);
            return NoContent(); // 204
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); } // 404
    }
}