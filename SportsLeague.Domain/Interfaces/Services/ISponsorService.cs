using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Services;

public interface ISponsorService
{
    Task<IEnumerable<Sponsor>> GetAllAsync();
    Task<Sponsor?> GetByIdAsync(int id);
    Task<Sponsor?> GetByNameAsync(string name);
    Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category);
    Task<Sponsor> CreateAsync(Sponsor sponsor);
    Task UpdateAsync(int id, Sponsor sponsor);
    Task RegisterSponsorInTournamentAsync(int tournamentId, int sponsorId, decimal contractAmount);
    Task DeleteAsync(int id);
}