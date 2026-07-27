namespace redil_backend.Dtos.Redil
{
    public record RedilClassStatDto(string Name, string GroupName, IReadOnlyList<string> Rediles, bool IsServer, float AttendancePercentage);
}
