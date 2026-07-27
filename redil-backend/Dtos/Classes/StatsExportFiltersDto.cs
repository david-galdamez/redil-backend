namespace redil_backend.Dtos.Classes
{
    public record StatsExportFiltersDto(
        DateTime FromDate,
        DateTime ToDate,
        string? RedilName,
        string? GroupName,
        string? Search);
}
