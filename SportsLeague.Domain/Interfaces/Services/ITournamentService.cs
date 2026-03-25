using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Services;

public interface ITournamentService
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<Tournament?> GetByIdAsync(int id);
    Task<Tournament> CreateAsync(Tournament tournament);
    Task UpdateAsync(int id, Tournament tournament);
    Task DeleteAsync(int id);
    Task UpdateStatusAsync(int id, TournamentStatus newStatus); //Cambia el status de un torneo
    Task RegisterTeamAsync(int tournamentId, int teamId); //Registra equipo en torneo
    Task<IEnumerable<Teams>> GetTeamsByTournamentAsync(int tournamentId); //Obtener equipos por torneo
}