using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Numerics;
using System.Net.Mail;

namespace SportsLeague.Domain.Services;
public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ILogger<SponsorService> _logger;

    public SponsorService(
    ISponsorRepository sponsorRepository,
    ITournamentSponsorRepository tournamentSponsorRepository,
    ITournamentRepository tournamentRepository,
    ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _tournamentRepository = tournamentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all sponsors");
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
        var sponsor = await _sponsorRepository.GetByIdAsync(id);
        if (sponsor == null)
            _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
        return sponsor;
    }

    public async Task<Sponsor?> GetByNameAsync(string name)
    {
        _logger.LogInformation("Retrieving sponsor with Name: {SponsorName}", name);
        var sponsor = await _sponsorRepository.GetByNameAsync(name);
        if (sponsor == null)
            _logger.LogWarning("Sponsor with Name {SponsorName} not found", name);
        return sponsor;
    }

    public async Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category)
    {
        _logger.LogInformation("Retrieving all sponsors of {SponsorCategory}", category);
        return await _sponsorRepository.GetByCategoryAsync(category);
    }

    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        //Verification of Name being unique among sponsors
        var existingSponsor = await _sponsorRepository
        .GetByNameAsync(sponsor.Name);
        if (existingSponsor != null)
        {
            _logger.LogWarning(
            "The name {Name} is already used by an Sponsor",
            sponsor.Name);
            throw new InvalidOperationException(
            $"The name {sponsor.Name} is already used by an Sponsor");
        }

        //Verification of ContactEmail being valid
        var ValidationContactEmail = SponsorEmailIsValid(sponsor.ContactEmail);
        if(ValidationContactEmail == false)
        {
            _logger.LogWarning(
                "The given Contact Email {ContactEmail} is not valid",
                sponsor.ContactEmail);
            throw new InvalidOperationException(
                $"The Contact Email {sponsor.ContactEmail} is not valid");        }
       

        _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existingSponsor = await _sponsorRepository.GetByIdAsync(id);
        if (existingSponsor == null)
        {
            throw new KeyNotFoundException(
            $"There is no Sponsor with ID {id}");
        }

        //Verification of Name being unique among sponsors
        if (existingSponsor.Name != sponsor.Name)
        {
            var conflict = await _sponsorRepository
            .GetByNameAsync(sponsor.Name);
            if (conflict != null && conflict.Id != id)
            {
                throw new InvalidOperationException(
                $"The name {sponsor.Name} is already in use");
            }
        }

        //Verification of ContactEmail being valid
        var ValidationContactEmailUpdate = SponsorEmailIsValid(sponsor.ContactEmail);
        if (ValidationContactEmailUpdate == false)
        {
            _logger.LogWarning(
                "The given Contact Email {ContactEmail} is not valid",
                sponsor.ContactEmail);
            throw new InvalidOperationException(
                $"The Contact Email {sponsor.ContactEmail} is not valid");
        }

        existingSponsor.Name = sponsor.Name;
        existingSponsor.ContactEmail = sponsor.ContactEmail;
        existingSponsor.Phone = sponsor.Phone;
        existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
        existingSponsor.Category = sponsor.Category;

        _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.UpdateAsync(existingSponsor);
    }

    private bool SponsorEmailIsValid (string Email)
    {
        try
        {
            var ValidationEmail = new MailAddress(Email);
            return ValidationEmail.Address == Email;
        }
        catch
        {
            return false;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"There is no sponsor with id: {id}");

        _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.DeleteAsync(id);
    }

    public async Task RegisterSponsorInTournamentAsync(int tournamentId, int sponsorId, decimal contractAmount)
    {
        //Verify that tournament exists
        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new KeyNotFoundException(
            $"There is no tournament with ID {tournamentId}");

        //Only when the tournament status is Pending
        if (tournament.Status != TournamentStatus.Pending)
        {
            throw new InvalidOperationException(
            "Sponsors can only be enrolled in tournaments with status Pending");
        }

        //Verify sponsor exists
        var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
        if (!sponsorExists)
            throw new KeyNotFoundException(
            $"There is ni sponsors with ID {sponsorId}");

        //Verify the sponsor is not already enrolled
        var existing = await _tournamentSponsorRepository
        .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
        if (existing != null)
        {
            throw new InvalidOperationException(
            "This sponsor is already enrolled in the tournament");
        }

        //Verify that ContractAmount > 0
        if (contractAmount <= 0)
        {
            throw new InvalidOperationException(
            "This contract amount is invalid, it must be greater than 0");
        }

        var tournamentSponsor = new TournamentSponsor
        {
            TournamentId = tournamentId,
            SponsorId = sponsorId,
            ContractAmount = contractAmount,
            JoinedAt = DateTime.UtcNow
        };

        _logger.LogInformation(
        "Registering sponsor {SponsorId} in tournament {TournamentId}",
        sponsorId, tournamentId);
        await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
    }
}