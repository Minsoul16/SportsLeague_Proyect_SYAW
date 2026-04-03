using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.DataAccess.Repositories;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Services;
using SportsLeague.Domain.Services;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SponsorController : ControllerBase
{
    private readonly ISponsorService _sponsorService;
    private readonly IMapper _mapper;

    public SponsorController(
    ISponsorService sponsorService,
    IMapper mapper)
    {
        _sponsorService = sponsorService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
    {
        var sponsors = await _sponsorService.GetAllAsync();
        var sponsorsDto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
        return Ok(sponsorsDto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
    {
        var sponsor = await _sponsorService.GetByIdAsync(id);
        if (sponsor == null)
            return NotFound(new { message = $"Sponsor with ID {id} not found" });
        return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
    }

    [HttpGet("name/{name}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetByName(string name)
    {
        var sponsor = await _sponsorService.GetByNameAsync(name);
        if (sponsor == null)
            return NotFound(new { message = $"Sponsor with Name {name} not found" });
        return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<SponsorResponseDTO>> GetByCategory(SponsorCategory category)
    {
        var sponsor = await _sponsorService.GetByCategoryAsync(category);
        if (sponsor == null)
            return NotFound(new { message = $"Sponsor with Category {category} not found" });
        return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
    }

    [HttpGet("{id}/tournaments")]
    public async Task<ActionResult<IEnumerable<TournamentResponseDTO>>> GetTournamentsBySponsor(int id)
    {
        try
        {
            var tournaments = await _sponsorService.GetTournamentsBySponsorAsync(id);
            return Ok(_mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost]
    public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            var created = await _sponsorService.CreateAsync(sponsor);
            var responseDto = _mapper.Map<SponsorResponseDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
    {
        try
        {
            var sponsor = _mapper.Map<Sponsor>(dto);
            await _sponsorService.UpdateAsync(id, sponsor);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _sponsorService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpDelete("tournaments/{tournamentId}/sponsors/{sponsorId}")]
    public async Task<ActionResult> UnEnrollSponsorInTournament(int sponsorId, int tournamentId, TournamentSponsorRequestDTO dto)
    {
        try
        {
            await _sponsorService.UnEnrollSponsorInTournamentAsync(tournamentId, sponsorId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }

    [HttpPost("{sponsorId}/RegisterSponsorTournament")]
    public async Task<ActionResult> RegisterSponsorInTournament(int sponsorId, TournamentSponsorRequestDTO dto) 
    {
        try
        {
            await _sponsorService.RegisterSponsorInTournamentAsync(dto.TournamentId, sponsorId, dto.ContractAmount);
            return Ok(new { message = "Sponsor registered succesfully" });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
    }
}