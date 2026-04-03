using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;
public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
{
    public SponsorRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<Sponsor?> GetByNameAsync(string name)
    {
        return await _dbSet
        .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Tournament>> GetTournamentsBySponsorAsync(int sponsorId)
    {
        return await _dbSet
        .Where(s => s.Id == sponsorId)
        .SelectMany(s => s.TournamentSponsors)
        .Select(ts => ts.Tournament)
        .ToListAsync();
    }

    public async Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category)
    {
        return await _dbSet
        .Where(s => s.Category == category)
        .ToListAsync();
    }
}