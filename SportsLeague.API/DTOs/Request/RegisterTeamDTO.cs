namespace SportsLeague.API.DTOs.Request;
public class RegisterTeamDTO
{
    public int TeamId { get; set; } //Permite definir en el body del request una propiedad especifica (Permite un manejo descriptivo
                                    //a nivel de JSON)
}