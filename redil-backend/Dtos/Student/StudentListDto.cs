namespace redil_backend.Dtos.Student
{
    public record StudentListDto(int Id, string Name, string GroupName, string? Phone, bool IsServer);
}
