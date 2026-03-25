using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class TeamRepository : GenericRepository<Teams>, ITeamRepository
{

    public TeamRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<Teams?> GetByNameAsync(string name)//Devuelve objeto tipo Team
    {
        return await _dbSet
        .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Teams>> GetByCityAsync(string city)//Devuelve lista de objetos tipo Team
    {
        return await _dbSet
        .Where(t => t.City.ToLower() == city.ToLower())
        .ToListAsync();
    }

}