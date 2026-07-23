namespace redil_backend.Dtos.Classes
{
    public record ClassStatsRequestDto(int? RedilId, DateTime FromDate, DateTime ToDate, int? GroupId, string? Search);
}
