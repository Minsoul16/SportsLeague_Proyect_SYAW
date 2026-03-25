namespace SportsLeague.Domain.Entities;

public class TournamentTeam : AuditBase
{
    public int TournamentId { get; set; } //FK
    public int TeamId { get; set; } //FK
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Tournament Tournament { get; set; } = null!;
    public Teams Team { get; set; } = null!;
}
