using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories
{
    public interface ITeamRepository : IGenericRepository<Teams>
    {

        Task<Teams?> GetByNameAsync(string name);

        Task<IEnumerable<Teams>> GetByCityAsync(string city);

    }
}
