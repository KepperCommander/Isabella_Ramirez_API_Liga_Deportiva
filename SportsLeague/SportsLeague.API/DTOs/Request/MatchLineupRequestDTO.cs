namespace SportsLeague.API.DTOs.Request;

public class MatchLineupRequestDTO
{
    public int PlayerId { get; set; }

    public bool IsStarter { get; set; }

    // Posición asignada para este partido = GK, CB, CDM, CAM, ST
    public string Position { get; set; } = string.Empty;
}
